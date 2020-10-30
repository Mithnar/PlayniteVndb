using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Newtonsoft.Json;
using Playnite.SDK;
using VndbSharp.Models.Errors;

namespace PlayniteVndbExtension
{
    public enum SpoilerLevel
    {
        None,
        Minor,
        Major
    }
    
    public enum ViolenceLevel
    {
        Tame,
        Violent,
        Brutal
    }
    
    public enum SexualityLevel
    {
        Safe,
        Suggestive,
        Explicit
    }
    
    public class VndbMetadataSettings : ISettings
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        
        private readonly VndbMetadataPlugin _plugin;
        public const int CurrentVersion = 2;

        public int Version { get; set; }
        public bool TagEnableContent { get; set; } = true;
        public bool TagEnableSexual { get; set; } = false;
        public bool TagEnableTechnical { get; set; } = true;
        public SpoilerLevel TagMaxSpoilerLevel { get; set; } = SpoilerLevel.Minor;
        public ViolenceLevel ImageMaxViolenceLevel { get; set; } = ViolenceLevel.Tame;
        public SexualityLevel ImageMaxSexualityLevel { get; set; } = SexualityLevel.Safe;
        public float TagMinScore { get; set; } = 1;
        public bool AllowIncompleteDates { get; set; } = false;

        public VndbMetadataSettings()
        {
        }

        public VndbMetadataSettings(VndbMetadataPlugin plugin)
        {
            
            _plugin = plugin;
            var savedSettings = plugin.LoadPluginSettings<VndbMetadataSettings>();
            Logger.Debug("After Load Settings");
            if (savedSettings == null) return;
            if (savedSettings.Version != CurrentVersion)
            {
                MigrateSettingsVersion(savedSettings, _plugin);
            }
            ImageMaxViolenceLevel = savedSettings.ImageMaxViolenceLevel;
            ImageMaxSexualityLevel = savedSettings.ImageMaxSexualityLevel;
            TagEnableContent = savedSettings.TagEnableContent;
            TagEnableSexual = savedSettings.TagEnableSexual;
            TagEnableTechnical = savedSettings.TagEnableTechnical;
            TagMaxSpoilerLevel = savedSettings.TagMaxSpoilerLevel;
            TagMinScore = savedSettings.TagMinScore;
            AllowIncompleteDates = savedSettings.AllowIncompleteDates;
            Version = savedSettings.Version;
        }

        public static void MigrateSettingsVersion(VndbMetadataSettings savedSettings, VndbMetadataPlugin plugin)
        {
            if (savedSettings.Version != CurrentVersion)
            {
                MigrateToV1(savedSettings);
                MigrateToV2(savedSettings);
                plugin.SavePluginSettings(savedSettings);
            }
            
        }

#pragma warning disable 618
        private static void MigrateToV1(VndbMetadataSettings savedSettings)
        {
            if (savedSettings.Version == 0)
            {
                savedSettings.AllowIncompleteDates = false;
                savedSettings.Version = 1;
            }
        }
        
        private static void MigrateToV2(VndbMetadataSettings savedSettings)
        {
            if (savedSettings.Version == 1)
            {
                if (savedSettings.AllowNsfwImages.HasValue && savedSettings.AllowNsfwImages.Value)
                {
                    savedSettings.ImageMaxSexualityLevel = SexualityLevel.Explicit;
                    savedSettings.ImageMaxViolenceLevel = ViolenceLevel.Brutal;
                }
                else
                {
                    savedSettings.ImageMaxSexualityLevel = SexualityLevel.Safe;
                    savedSettings.ImageMaxViolenceLevel = ViolenceLevel.Tame;
                }

                savedSettings.AllowNsfwImages = null;
                savedSettings.Version = 2;
            }
        }
#pragma warning restore 618

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
        
        //Old Configuration Values
        
        [ObsoleteAttribute("This property is obsolete. Use ImageMaxViolenceLevel and ImageMaxSexualityLevel instead.", false)]
        public bool? AllowNsfwImages { get; set; }
        
        public bool ShouldSerializeAllowNsfwImages()
        {
            return false;
        }
    }
}