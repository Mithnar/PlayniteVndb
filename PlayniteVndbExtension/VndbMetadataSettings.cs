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

        public SpoilerLevel TagMaxSpoilerLevel { get; set; } = SpoilerLevel.Minor;
        public ViolenceLevel ImageMaxViolenceLevel { get; set; } = ViolenceLevel.Tame;
        public SexualityLevel ImageMaxSexualityLevel { get; set; } = SexualityLevel.Safe;

        public uint MaxContentTags { get; set; } = 8;
        public uint MaxSexualTags { get; set; } = 0;
        public uint MaxTechnicalTags { get; set; } = 8;
        public uint MaxAllTags { get; set; } = 15;
        
        
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
            MaxContentTags = savedSettings.MaxContentTags;
            MaxSexualTags = savedSettings.MaxSexualTags;
            MaxTechnicalTags = savedSettings.MaxTechnicalTags;
            MaxAllTags = savedSettings.MaxAllTags;
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
                
                if (savedSettings.TagEnableContent.HasValue && savedSettings.TagEnableContent.Value)
                {
                    savedSettings.MaxContentTags = 8;
                }
                if (savedSettings.TagEnableSexual.HasValue && savedSettings.TagEnableSexual.Value)
                {
                    savedSettings.MaxSexualTags = 8;
                }
                if (savedSettings.TagEnableTechnical.HasValue && savedSettings.TagEnableTechnical.Value)
                {
                    savedSettings.MaxTechnicalTags = 8;
                }

                savedSettings.TagEnableContent = null;
                savedSettings.TagEnableSexual = null;
                savedSettings.TagEnableTechnical = null;
                savedSettings.MaxAllTags = 15;

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
        
        [ObsoleteAttribute("This property is obsolete. Use MaxContentTags and ImageMaxSexualityLevel instead.", false)]
        public bool? AllowNsfwImages { get; set; }
        public bool ShouldSerializeAllowNsfwImages()
        {
            return false;
        }
        
        [ObsoleteAttribute("This property is obsolete. Use MaxContentTags instead.", false)]
        public bool? TagEnableContent { get; set; }
        public bool ShouldSerializeTagEnableContent()
        {
            return false;
        }
        
        [ObsoleteAttribute("This property is obsolete. Use MaxSexualTags instead.", false)]
        public bool? TagEnableSexual { get; set; }
        public bool ShouldSerializeTagEnableSexual()
        {
            return false;
        }
        
        [ObsoleteAttribute("This property is obsolete. Use MaxTechnicalTags instead.", false)]
        public bool? TagEnableTechnical { get; set; }
        public bool ShouldSerializeTagEnableTechnical()
        {
            return false;
        }
    }
}