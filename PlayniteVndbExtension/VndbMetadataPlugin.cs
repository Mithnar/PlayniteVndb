using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows.Controls;
using Newtonsoft.Json;
using Playnite.SDK;
using Playnite.SDK.Plugins;
using VndbSharp;
using VndbSharp.Models;

namespace PlayniteVndbExtension
{
    public class VndbMetadataPlugin : MetadataPlugin
    {
        private const string Guid = "1da026f7-442d-4d13-a547-13c02a07de50";

        private static readonly ILogger Logger = LogManager.GetLogger();

        private readonly DescriptionFormatter _descriptionFormatter;

        private readonly List<TagName> _tagNames;
        
        private VndbMetadataSettingsViewModel _settings;

        public VndbMetadataPlugin(IPlayniteAPI playniteApi) : base(playniteApi)
        {
            _settings = new VndbMetadataSettingsViewModel(this);
            Properties = new MetadataPluginProperties
            {
                HasSettings = true
            };

            var tagDumpPath = DownloadTagDump(false);
            var jsonString = File.ReadAllText(tagDumpPath);
            _tagNames = JsonConvert.DeserializeObject<List<TagName>>(jsonString);
            VndbClient = new Vndb(true)
                .WithClientDetails("PlayniteVndbExtension", "1.2")
                .WithFlagsCheck(true, HandleInvalidFlags)
                .WithTimeout(10000);
            _descriptionFormatter = new DescriptionFormatter();
        }
        
        public string DownloadTagDump(bool forceDownload)
        {
            var tagDumpFile = $"{GetPluginUserDataPath()}/tag_dump.json";
            if (forceDownload || !File.Exists(tagDumpFile) || DateTime.Now.Subtract(_settings.Settings.LastTagUpdate).Days > 7)
            {
                var archiveDownloadPath = $"{GetPluginUserDataPath()}/tagdump.json.gz";

                Logger.Debug("Downloading new Tag Dump");
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile("https://dl.vndb.org/dump/vndb-tags-latest.json.gz", archiveDownloadPath);
                }

                if (File.Exists(tagDumpFile))
                {
                    File.Delete(tagDumpFile);
                }
                
                using (var input = File.OpenRead(archiveDownloadPath))
                using (var output = File.OpenWrite(tagDumpFile))
                using (var gz = new GZipStream(input, CompressionMode.Decompress))
                {
                    gz.CopyTo(output);
                }
                _settings.Settings.LastTagUpdate = DateTime.Now;
                SavePluginSettings(_settings);
                File.Delete(archiveDownloadPath);
            }

            return tagDumpFile;
        }

        public Vndb VndbClient { get; }

        public override Guid Id { get; } = System.Guid.Parse(Guid);

        public override string Name { get; } = "VNDB";

        public override List<MetadataField> SupportedFields { get; } = new List<MetadataField>
        {
            MetadataField.Description,
            MetadataField.CoverImage,
            MetadataField.BackgroundImage,
            MetadataField.ReleaseDate,
            MetadataField.Developers,
            MetadataField.Publishers,
            MetadataField.Genres,
            MetadataField.Links,
            MetadataField.Tags,
            MetadataField.CriticScore,
            MetadataField.CommunityScore
        };

        private static void HandleInvalidFlags(string method, VndbFlags provided, VndbFlags invalid)
        {
            //TODO
        }

        public override OnDemandMetadataProvider GetMetadataProvider(MetadataRequestOptions options)
        {
            return new VndbMetadataProvider(options, _tagNames, _descriptionFormatter, this);
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return _settings;
        }

        public override UserControl GetSettingsView(bool firstRunView)
        {
            return new VndbMetadataSettingsView();
        }
    }
}