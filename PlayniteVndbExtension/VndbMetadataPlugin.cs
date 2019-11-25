using System;
using System.Collections.Generic;
using System.IO;
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

        private readonly DescriptionFormatter _descriptionFormatter;

        private readonly List<TagName> _tagNames;

        public VndbMetadataPlugin(IPlayniteAPI playniteApi) : base(playniteApi)
        {
            var jsonString = File.ReadAllText
            (
                playniteApi.Paths.ExtensionsDataPath +
                Path.DirectorySeparatorChar +
                Guid +
                Path.DirectorySeparatorChar +
                "tag_dump.json"
            );
            _tagNames = JsonConvert.DeserializeObject<List<TagName>>(jsonString);
            VndbClient = new Vndb(true)
                .WithClientDetails("PlayniteVndbExtension", "0.1")
                .WithFlagsCheck(true, HandleInvalidFlags)
                .WithTimeout(10000);
            _descriptionFormatter = new DescriptionFormatter();
            CreateSettingsIfNotExists();
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

        private void CreateSettingsIfNotExists()
        {
            var settings = LoadPluginSettings<VndbMetadataSettings>();
            if (settings == null)
            {
                settings = new VndbMetadataSettings();
                SavePluginSettings(settings);
            }
        }

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
            return new VndbMetadataSettings(this);
        }

        public override UserControl GetSettingsView(bool firstRunView)
        {
            return new VndbMetadataSettingsView();
        }
    }
}