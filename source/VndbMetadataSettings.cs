﻿using Playnite.SDK;
using Playnite.SDK.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VndbMetadata.Models;

namespace VndbMetadata
{
    public class VndbMetadataSettings : ObservableObject
    {
        public int Version { get; set; } = 2;
        public SpoilerLevel TagMaxSpoilerLevel { get; set; } = SpoilerLevel.Minor;
        public ViolenceLevel ImageMaxViolenceLevel { get; set; } = ViolenceLevel.Tame;
        public SexualityLevel ImageMaxSexualityLevel { get; set; } = SexualityLevel.Safe;

        public uint MaxContentTags { get; set; } = 8;
        public uint MaxSexualTags { get; set; } = 0;
        public uint MaxTechnicalTags { get; set; } = 8;
        public uint MaxAllTags { get; set; } = 15;
        public float TagMinScore { get; set; } = 1;
        public bool AllowIncompleteDates { get; set; } = false;
        public string ContentTagPrefix { get; set; } = string.Empty;
        public string SexualTagPrefix { get; set; } = string.Empty;
        public string TechnicalTagPrefix { get; set; } = string.Empty;
        public DateTime LastTagUpdate { get; set; }

        public bool PlaytimeTagEnabled { get; set; } = false;

        public string VeryShortPlaytimeName { get; set; } = "Very Short";

        public string ShortPlaytimeName { get; set; } = "Short";

        public string MediumPlaytimeName { get; set; } = "Medium";

        public string LongPlaytimeName { get; set; } = "Long";

        public string VeryLongPlaytimeName { get; set; } = "Very Long";
        
        public string UnknownPlaytimeName { get; set; } = "Unknown Length";

        public bool IgnoreName { get; set; } = false;
        public bool IgnoreGenre { get; set; } = false;
        public bool IgnoreDevelopers { get; set; } = false;
        public bool IgnorePublishers { get; set; } = false;
        public bool IgnoreTags { get; set; } = false;
        public bool IgnoreScore { get; set; } = false;
        public bool IgnoreReleaseDate { get; set; } = false;
        public bool IgnoreDescription { get; set; } = false;
        public bool IgnoreBackground { get; set; } = false;
        public bool IgnoreCover { get; set; } = false;

        public bool PreferLocalizedName { get; set; } = true;
        
            //Deprecated Configuration Values kept for configuration migration

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

    public class VndbMetadataSettingsViewModel : ObservableObject, ISettings
    {
        private VndbMetadataSettings editingClone { get; set; }
        private static readonly ILogger Logger = LogManager.GetLogger();
        public const int CurrentVersion = 2;

        private readonly VndbMetadata plugin;

        private VndbMetadataSettings settings;
        public VndbMetadataSettings Settings
        {
            get => settings;
            set
            {
                settings = value;
                OnPropertyChanged();
            }
        }
        
        public Dictionary<ViolenceLevel, string> ViolenceLevelEnums { get; } =
            new Dictionary<ViolenceLevel, string>()
            {
                { ViolenceLevel.Tame, ResourceProvider.GetString("LOCVndbMd_Settings_ViolenceLevel_" + ViolenceLevel.Tame) },
                { ViolenceLevel.Violent, ResourceProvider.GetString("LOCVndbMd_Settings_ViolenceLevel_" + ViolenceLevel.Violent) },
                { ViolenceLevel.Brutal, ResourceProvider.GetString("LOCVndbMd_Settings_ViolenceLevel_" + ViolenceLevel.Brutal) }
            };

        public ViolenceLevel ViolenceLevelProperty
        {
            get { return settings.ImageMaxViolenceLevel; }
            set { settings.ImageMaxViolenceLevel = value; }
        }
        
        public Dictionary<SexualityLevel, string> SexualLevelEnums { get; } =
            new Dictionary<SexualityLevel, string>()
            {
                { SexualityLevel.Safe, ResourceProvider.GetString("LOCVndbMd_Settings_SexualLevel_" + SexualityLevel.Safe) },
                { SexualityLevel.Suggestive, ResourceProvider.GetString("LOCVndbMd_Settings_SexualLevel_" + SexualityLevel.Suggestive) },
                { SexualityLevel.Explicit, ResourceProvider.GetString("LOCVndbMd_Settings_SexualLevel_" + SexualityLevel.Explicit) }
            };

        public SexualityLevel SexualLevelProperty
        {
            get { return settings.ImageMaxSexualityLevel; }
            set { settings.ImageMaxSexualityLevel = value; }
        }
        
        public Dictionary<SpoilerLevel, string> SpoilerLevelEnums { get; } =
            new Dictionary<SpoilerLevel, string>()
            {
                { SpoilerLevel.None, ResourceProvider.GetString("LOCVndbMd_Settings_SpoilerLevel_" + SpoilerLevel.None) },
                { SpoilerLevel.Minor, ResourceProvider.GetString("LOCVndbMd_Settings_SpoilerLevel_" + SpoilerLevel.Minor) },
                { SpoilerLevel.Major, ResourceProvider.GetString("LOCVndbMd_Settings_SpoilerLevel_" + SpoilerLevel.Major) }
            };

        public SpoilerLevel SpoilerLevelProperty
        {
            get { return settings.TagMaxSpoilerLevel; }
            set { settings.TagMaxSpoilerLevel = value; }
        }

        public VndbMetadataSettingsViewModel(VndbMetadata plugin)
        {
            this.plugin = plugin;
            var savedSettings = plugin.LoadPluginSettings<VndbMetadataSettings>();
            if (savedSettings != null)
            {
                Settings = savedSettings;
            }
            else
            {
                Settings = new VndbMetadataSettings();
            }

            if (settings.Version != CurrentVersion)
            {
                MigrateSettingsVersion(savedSettings, plugin);
            }
        }

        public static void MigrateSettingsVersion(VndbMetadataSettings savedSettings, VndbMetadata plugin)
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
            editingClone = Serialization.GetClone(Settings);
        }

        public void CancelEdit()
        {
            Settings = editingClone;
        }

        public void EndEdit()
        {
            plugin.SavePluginSettings(Settings);
        }

        public bool VerifySettings(out List<string> errors)
        {
            errors = new List<string>();
            return true;
        }
    }
}