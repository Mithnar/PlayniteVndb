using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using playnite.metadata.vndb.provider;
using Playnite.SDK;
using Playnite.SDK.Plugins;
using System.Windows.Controls;
using playnite.metadata.vndb.settings;
using PlayniteVndbExtension;
using VndbSharp;
using VndbSharp.Models;

namespace playnite.metadata.vndb
{
    public class VndbMetadataPlugin : MetadataPlugin
    {
        public VndbMetadataPlugin(IPlayniteAPI playniteApi) : base(playniteApi)
        {
            
            var jsonString = File.ReadAllText
                (
                playniteApi.Paths.ExtensionsDataPath + 
                Path.DirectorySeparatorChar + 
                guid +
                Path.DirectorySeparatorChar + 
                "tag_dump.json"
                );
            _tagNames = JsonConvert.DeserializeObject<List<TagName>>(jsonString);
            VndbClient = new Vndb(true)
                .WithClientDetails("PlayniteVndbExtension", "0.1")
                .WithFlagsCheck(true, (method, provided, invalid) => handleInvalidFlags(method, provided, invalid))
                .WithTimeout(10000);
        }

        private void handleInvalidFlags(string method, VndbFlags provided, VndbFlags invalid)
        {
        }

        private List<TagName> _tagNames;
        private static string guid = "1da026f7-442d-4d13-a547-13c02a07de50";
        public Vndb VndbClient { get; }

        public override Guid Id { get; } = Guid.Parse(guid);

        public override OnDemandMetadataProvider GetMetadataProvider(MetadataRequestOptions options)
        {
            return new VndbMetadataProvider(options, _tagNames, this);
        }

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