using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Documents;
using Newtonsoft.Json;
using Playnite.SDK;
using PlayniteVndbExtension;

namespace playnite.metadata.vndb.settings
{
    public class VndbMetadataSettings: ISettings
    {
        private readonly VndbMetadataPlugin _plugin;
        
        public bool AllowNsfwImages { get; set; }
        
        public bool TagEnableContent { get; set; } = true;
        public bool TagEnableSexual { get; set; }
        public bool TagEnableTechnical { get; set; } = true;

        public SpoilerLevel TagMaxSpoilerLevel { get; set; } = SpoilerLevel.Minor;
        
        [JsonIgnore]
        public IEnumerable<SpoilerLevel> AvailableSpoilerLevels { get; set; } = Enum.GetValues(typeof(SpoilerLevel)).Cast<SpoilerLevel>();
        public float TagMinScore { get; set; } = 1;
        
        public VndbMetadataSettings()
        {
        }
        public VndbMetadataSettings(VndbMetadataPlugin plugin)
        {
            _plugin = plugin;
            var savedSettings = plugin.LoadPluginSettings<VndbMetadataSettings>();
            if (savedSettings == null) return;
            AllowNsfwImages = savedSettings.AllowNsfwImages;
            TagEnableContent = savedSettings.TagEnableContent;
            TagEnableSexual = savedSettings.TagEnableSexual;
            TagEnableTechnical = savedSettings.TagEnableTechnical;
            TagMaxSpoilerLevel = savedSettings.TagMaxSpoilerLevel;
            TagMinScore = savedSettings.TagMinScore;
        }

        public void BeginEdit()
        {
        }

        public void EndEdit()
        {
            _plugin.SavePluginSettings(this);
        }

        public void CancelEdit()
        {
        }

        public bool VerifySettings(out List<string> errors)
        {
            errors = new List<string>();
            return true;
        }
    }
}