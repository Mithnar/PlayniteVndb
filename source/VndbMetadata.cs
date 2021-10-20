using Newtonsoft.Json;
using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VndbMetadata.Models;
using VndbSharp;
using VndbSharp.Models;

namespace VndbMetadata
{
    public class VndbMetadata : MetadataPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        private VndbMetadataSettingsViewModel settings { get; set; }

        public override Guid Id { get; } = Guid.Parse("1da026f7-442d-4d13-a547-13c02a07de50");

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

        public override string Name => "VNDBTest";

        private readonly List<TagName> tagNames;

        public Vndb VndbClient { get; private set; }
        public DescriptionFormatter descriptionFormatter { get; private set; }

        public VndbMetadata(IPlayniteAPI api) : base(api)
        {
            settings = new VndbMetadataSettingsViewModel(this);
            Properties = new MetadataPluginProperties
            {
                HasSettings = true
            };

            var tagDumpPath = DownloadTagDump(false);
            var jsonString = File.ReadAllText(tagDumpPath);
            tagNames = JsonConvert.DeserializeObject<List<TagName>>(jsonString);
            VndbClient = new Vndb(true)
                .WithClientDetails("PlayniteVndbExtension", "1.2")
                .WithFlagsCheck(true, HandleInvalidFlags)
                .WithTimeout(10000);
            descriptionFormatter = new DescriptionFormatter();
        }

        public string DownloadTagDump(bool forceDownload)
        {
            var tagDumpFile = $"{GetPluginUserDataPath()}/tag_dump.json";
            if (forceDownload || !File.Exists(tagDumpFile) || DateTime.Now.Subtract(settings.Settings.LastTagUpdate).Days > 7)
            {
                var archiveDownloadPath = $"{GetPluginUserDataPath()}/tagdump.json.gz";

                logger.Debug("Downloading new Tag Dump");
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
                settings.Settings.LastTagUpdate = DateTime.Now;
                SavePluginSettings(settings);
                File.Delete(archiveDownloadPath);
            }

            return tagDumpFile;
        }

        private static void HandleInvalidFlags(string method, VndbFlags provided, VndbFlags invalid)
        {
            //TODO
        }

        public override OnDemandMetadataProvider GetMetadataProvider(MetadataRequestOptions options)
        {
            return new VndbMetadataProvider(options, tagNames, descriptionFormatter, this);
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new VndbMetadataSettingsView();
        }
    }
}