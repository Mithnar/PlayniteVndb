using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VndbMetadata.Models;
using VndbSharp;
using VndbSharp.Models;
using VndbSharp.Models.Release;
using VndbSharp.Models.VisualNovel;

namespace VndbMetadata
{
    public class VndbMetadataProvider : OnDemandMetadataProvider
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        private readonly MetadataRequestOptions options;
        private readonly List<TagName> tagDetails;
        private readonly IPlayniteAPI playniteApi;
        private readonly Vndb vndbClient;
        private readonly string pluginUserDataPath;
        private readonly VndbMetadataSettings settings;
        private readonly VndbMetadata plugin;
        private DescriptionFormatter descriptionFormatter;
        private VisualNovel vnData;
        private List<ProducerRelease> vnProducers;
        private List<MetadataField> availableFields;
        public override List<MetadataField> AvailableFields
        {
            get
            {
                if (availableFields == null)
                {
                    availableFields = GetAvailableFields();
                }

                return availableFields;
            }
        }

        public VndbMetadataProvider(MetadataRequestOptions options, List<TagName> tagDetails,
            DescriptionFormatter descriptionFormatter, VndbMetadata plugin)
        {
            this.options = options;
            this.tagDetails = tagDetails;
            this.plugin = plugin;
            playniteApi = plugin.PlayniteApi;
            vndbClient = plugin.VndbClient;
            pluginUserDataPath = plugin.GetPluginUserDataPath();
            settings = plugin.LoadPluginSettings<VndbMetadataSettings>();
            VndbMetadataSettingsViewModel.MigrateSettingsVersion(settings, plugin);
            this.descriptionFormatter = descriptionFormatter;
        }

        private bool GetVndbMetadata()
        {
            if (vnData != null)
            {
                return true;
            }

            if (options.IsBackgroundDownload)
            {
                return false;
            }

            ReadOnlyCollection<VisualNovel> results = new ReadOnlyCollection<VisualNovel>(new List<VisualNovel>());
            VndbItemOption item = (VndbItemOption)playniteApi.Dialogs.ChooseItemWithSearch(null, searchString =>
            {
                ReadOnlyCollection<VisualNovel> search;
                if (IsVndbId(searchString))
                {
                    search = vndbClient.GetVisualNovelAsync(VndbFilters.Id.Equals(RetrieveVndbId(searchString)),
                            VndbFlags.FullVisualNovel).Result
                            .Items;
                }
                else
                {
                    search = vndbClient
                        .GetVisualNovelAsync(VndbFilters.Search.Fuzzy(searchString), VndbFlags.FullVisualNovel).Result
                        .Items;
                }

                results = search;
                return new List<GenericItemOption>(search.Select(vn =>
                    {
                        return new VndbItemOption(RetrieveOriginalOrLocalizedName(vn), descriptionFormatter.RemoveTags(vn.Description), vn.Id);
                    })
                    .ToList());
            }, options.GameData.Name);

            if (item != null && results.Any(vn => vn.Id.Equals(item.Id)))
            {
                vnData = results.First(vn => vn.Id.Equals(item.Id));
                vnProducers = vndbClient
                    .GetReleaseAsync(VndbFilters.VisualNovel.Equals(vnData.Id), VndbFlags.Producers)
                    .Result
                    .Items
                    .SelectMany(producers => producers.Producers)
                    .ToList();
                return true;
            }

            vnData = null;
            return false;
        }

        private string RetrieveOriginalOrLocalizedName(VisualNovel vn)
        {
            var name = vn.Name;
            if (!settings.PreferLocalizedName && !string.IsNullOrEmpty(vn.OriginalName))
            {
                name = vn.OriginalName;
            }

            return name;
        }

        private List<MetadataField> GetAvailableFields()
        {
            if (vnData == null && !GetVndbMetadata())
            {
                return new List<MetadataField>();
            }

            var fields = new List<MetadataField> { MetadataField.Links };
            if (!settings.IgnoreName)
            {
                fields.Add(MetadataField.Name);
            }

            if (!settings.IgnoreGenre)
            {
                fields.Add(MetadataField.Genres);
            }

            if (!settings.IgnoreDescription && !string.IsNullOrEmpty(vnData.Description))
            {
                fields.Add(MetadataField.Description);
            }

            if (!settings.IgnoreCover && vnData.Image != null && IsImageAllowed(vnData))
            {
                fields.Add(MetadataField.CoverImage);
            }

            if (!settings.IgnoreBackground && vnData.Screenshots.HasItems() &&
                vnData.Screenshots.Any(IsImageAllowed))
            {
                fields.Add(MetadataField.BackgroundImage);
            }

            if (!settings.IgnoreReleaseDate && vnData.Released != null && vnData.Released.Year != null)
            {
                fields.Add(MetadataField.ReleaseDate);
            }

            if (!settings.IgnoreTags && tagDetails != null && HasViableTags())
            {
                fields.Add(MetadataField.Tags);
            }

            if (!settings.IgnoreScore && vnData.Rating != 0)
            {
                fields.Add(MetadataField.CommunityScore);
            }

            if (!settings.IgnoreDevelopers && vnProducers != null && vnProducers.Count(p => p.IsDeveloper) > 0)
            {
                fields.Add(MetadataField.Developers);
            }

            if (!settings.IgnorePublishers && vnProducers != null && vnProducers?.Count(p => p.IsPublisher) > 0)
            {
                fields.Add(MetadataField.Publishers);
            }

            return fields;
        }

        private bool IsImageAllowed(VisualNovel vn)
        {
            return !(vn.ImageRating.SexualAvg >= (int)settings.ImageMaxSexualityLevel + 0.5 ||
                     vn.ImageRating.ViolenceAvg >= (int)settings.ImageMaxViolenceLevel + 0.5);
        }

        private bool IsImageAllowed(ScreenshotMetadata screenshot)
        {
            return !(screenshot.ImageRating.SexualAvg >= (int)settings.ImageMaxSexualityLevel + 0.5 ||
                     screenshot.ImageRating.ViolenceAvg >= (int)settings.ImageMaxViolenceLevel + 0.5);
        }

        private bool HasViableTags()
        {
            return vnData.Tags.HasItems() && vnData.Tags.Any(tag =>
            {
                if (TagIsAvailableForScoreAndSpoiler(tag))
                {
                    return tagDetails.Any(tagDetails =>
                    {
                        if (tag.Id.Equals(tagDetails.Id))
                        {
                            return TagIsInEnabledCategory(tagDetails);
                        }

                        return false;
                    });
                }

                return false;
            });
        }

        private bool TagIsAvailableForScoreAndSpoiler(TagMetadata tag)
        {
            return Math.Round(tag.Score, 1) >= settings.TagMinScore && LowerSpoilerLevel(tag);
        }

        private static bool IsVndbId(string searchString)
        {
            var onlyNumbersMatcher = new Regex("^[\\d]+$");
            return searchString.ToLower().StartsWith("id:v") && onlyNumbersMatcher.IsMatch(searchString.Substring(4));
        }

        private uint RetrieveVndbId(string searchString)
        {
            return uint.Parse(searchString.Substring(4));
        }

        public override string GetName(GetMetadataFieldArgs args)
        {
            if (settings.IgnoreName || !AvailableFields.Contains(MetadataField.Name) || vnData == null)
            {
                return base.GetName(args);
            }

            return RetrieveOriginalOrLocalizedName(vnData);
        }

        public override IEnumerable<MetadataProperty> GetGenres(GetMetadataFieldArgs args)
        {
            if (settings.IgnoreGenre || !AvailableFields.Contains(MetadataField.Genres) || vnData == null)
            {
                return base.GetGenres(args);
            }
            
            return new List<MetadataProperty>
            {
                new MetadataNameProperty("Visual Novel")
            };

        }

        public override ReleaseDate? GetReleaseDate(GetMetadataFieldArgs args)
        {
            if (settings.IgnoreReleaseDate || !AvailableFields.Contains(MetadataField.ReleaseDate) || vnData == null)
            {
                return base.GetReleaseDate(args);
            }
            
            if (vnData.Released.Year != null && vnData.Released.Month != null && vnData.Released.Day != null)
            {
                return new ReleaseDate
                (
                    (int)vnData.Released.Year.Value,
                    vnData.Released.Month.Value,
                    vnData.Released.Day.Value
                );
            }
            else if (vnData.Released.Year != null && vnData.Released.Month != null && settings.AllowIncompleteDates)
            {
                return new ReleaseDate
                (
                    (int)vnData.Released.Year.Value,
                    vnData.Released.Month.Value
                );
            }
            else if (vnData.Released.Year != null && settings.AllowIncompleteDates)
            {
                return new ReleaseDate
                (
                    (int)vnData.Released.Year.Value
                );
            }

            return base.GetReleaseDate(args);
        }

        public override IEnumerable<MetadataProperty> GetDevelopers(GetMetadataFieldArgs args)
        {
            if (settings.IgnoreDevelopers || !AvailableFields.Contains(MetadataField.Developers) || vnData == null)
            {
                return base.GetDevelopers(args);
            }
            
            return vnProducers.Where(p => p.IsDeveloper)
                .Select(p => p.Name).Distinct()
                .Select(s => new MetadataNameProperty(s)).ToList();

        }

        public override IEnumerable<MetadataProperty> GetPublishers(GetMetadataFieldArgs args)
        {
            if (settings.IgnorePublishers || !AvailableFields.Contains(MetadataField.Developers) || vnData == null)
            {
                return base.GetDevelopers(args);
            }
            
            return vnProducers.Where(p => p.IsPublisher)
                .Select(p => p.Name).Distinct()
                .Select(s => new MetadataNameProperty(s)).ToList();

        }

        public override IEnumerable<MetadataProperty> GetTags(GetMetadataFieldArgs args)
        {
            if (settings.IgnoreTags || !AvailableFields.Contains(MetadataField.Tags) || vnData == null)
            {
                return base.GetTags(args);
            }
            
            var tags = new List<string>();
            var contentTags = 0;
            var sexualTags = 0;
            var technicalTags = 0;
            foreach (var (tagMetadata, tagName) in vnData.Tags.OrderByDescending(tag => tag.Score).Select(MapTagToNamedTuple))
            {
                if (tagName == null)
                {
                    Logger.Warn("VndbMetadataProvider: Could not find tag: " + tagMetadata.Id);
                }
                else if (TagIsAvailableForScoreAndSpoiler(tagMetadata) &&
                         TagIsInEnabledCategory(tagName) &&
                         tags.Count < settings.MaxAllTags)
                {
                    switch (tagName.Cat)
                    {
                        case "cont":
                            contentTags = AddContentTagIfNotMax(contentTags, tags, tagName);
                            break;
                        case "ero":
                            sexualTags = AddSexualTagIfNotMax(sexualTags, tags, tagName);
                            break;
                        case "tech":
                            technicalTags = AddTechnicalTagIfNotMax(technicalTags, tags, tagName);
                            break;
                    }
                }
            }

            if (settings.PlaytimeTagEnabled)
            {
                AddPlaytimeTag(tags);
            }

            if (!tags.HasNonEmptyItems())
            {
                return base.GetTags(args);
            }
            
            tags.Sort();
            return tags.Select(s => new MetadataNameProperty(s)).ToList();

        }

        private void AddPlaytimeTag(List<string> tags)
        {
            switch (vnData.Length)
            {
                case VisualNovelLength.VeryShort:
                    var veryShortTag = "Very Short";
                    if (!string.IsNullOrEmpty(settings.VeryShortPlaytimeName))
                    {
                        veryShortTag = settings.VeryShortPlaytimeName;
                    }

                    tags.Add(veryShortTag);
                    break;
                case VisualNovelLength.Short:
                    var shortTag = "Short";
                    if (!string.IsNullOrEmpty(settings.ShortPlaytimeName))
                    {
                        shortTag = settings.ShortPlaytimeName;
                    }

                    tags.Add(shortTag);
                    break;
                case VisualNovelLength.Medium:
                    var mediumTag = "Medium";
                    if (!string.IsNullOrEmpty(settings.MediumPlaytimeName))
                    {
                        mediumTag = settings.MediumPlaytimeName;
                    }

                    tags.Add(mediumTag);
                    break;
                case VisualNovelLength.Long:
                    var longTag = "Long";
                    if (!string.IsNullOrEmpty(settings.LongPlaytimeName))
                    {
                        longTag = settings.LongPlaytimeName;
                    }

                    tags.Add(longTag);
                    break;
                case VisualNovelLength.VeryLong:
                    var veryLongTag = "Very Long";
                    if (!string.IsNullOrEmpty(settings.VeryLongPlaytimeName))
                    {
                        veryLongTag = settings.VeryLongPlaytimeName;
                    }

                    tags.Add(veryLongTag);
                    break;
                default:
                    var unknownTag = "Unknown Length";
                    if (!string.IsNullOrEmpty(settings.UnknownPlaytimeName))
                    {
                        unknownTag = settings.UnknownPlaytimeName;
                    }

                    tags.Add(unknownTag);
                    break;
            }
        }

        private int AddTechnicalTagIfNotMax(int technicalTags, List<string> tags, TagName tagName)
        {
            if (technicalTags >= settings.MaxTechnicalTags)
            {
                return technicalTags;
            }
            ++technicalTags;
            tags.Add(string.Concat(settings.TechnicalTagPrefix, tagName.Name));

            return technicalTags;
        }

        private int AddSexualTagIfNotMax(int sexualTags, List<string> tags, TagName tagName)
        {
            if (sexualTags >= settings.MaxSexualTags)
            {
                return sexualTags;
            }
            ++sexualTags;
            tags.Add(string.Concat(settings.SexualTagPrefix, tagName.Name));

            return sexualTags;
        }

        private int AddContentTagIfNotMax(int contentTags, List<string> tags, TagName tagName)
        {
            if (contentTags >= settings.MaxContentTags)
            {
                return contentTags;
            }
            ++contentTags;
            tags.Add(string.Concat(settings.ContentTagPrefix, tagName.Name));

            return contentTags;
        }

        private bool TagIsInEnabledCategory(TagName tagInfo)
        {
            return tagInfo.Cat.Equals("cont") && settings.MaxContentTags > 0 ||
                   tagInfo.Cat.Equals("ero") && settings.MaxSexualTags > 0 ||
                   tagInfo.Cat.Equals("tech") && settings.MaxTechnicalTags > 0;
        }

        private (TagMetadata, TagName) MapTagToNamedTuple(TagMetadata tag)
        {
            var details = tagDetails.Find(tagDetails =>
            {
                if (tagDetails.Id.Equals(tag.Id))
                {
                    return true;
                }

                return false;
            });

            return (tag, details);
        }

        private bool LowerSpoilerLevel(TagMetadata tag)
        {
            switch (tag.SpoilerLevel)
            {
                case VndbSharp.Models.Common.SpoilerLevel.None:
                    return true;
                case VndbSharp.Models.Common.SpoilerLevel.Minor:
                    return !settings.TagMaxSpoilerLevel.Equals(SpoilerLevel.None);
                case VndbSharp.Models.Common.SpoilerLevel.Major:
                    return settings.TagMaxSpoilerLevel.Equals(SpoilerLevel.Major);
                default:
                    return false;
            }
        }

        public override string GetDescription(GetMetadataFieldArgs args)
        {
            if (settings.IgnoreDescription || !AvailableFields.Contains(MetadataField.Description) || vnData == null)
            {
                return base.GetDescription(args);
            }
            
            return descriptionFormatter.Format(vnData.Description);
        }

        public override int? GetCommunityScore(GetMetadataFieldArgs args)
        {
            if (settings.IgnoreScore || !AvailableFields.Contains(MetadataField.CommunityScore) || vnData == null)
            {
                return base.GetCommunityScore(args);
            }
            
            return (int)(vnData.Rating * 10.0);
        }

        public override MetadataFile GetCoverImage(GetMetadataFieldArgs args)
        {
            if (settings.IgnoreCover || !AvailableFields.Contains(MetadataField.CoverImage) || vnData == null)
            {
                return base.GetCoverImage(args);
            }
            
            return IsImageAllowed(vnData) ? new MetadataFile(vnData.Image) : base.GetCoverImage(args);
        }

        public override MetadataFile GetBackgroundImage(GetMetadataFieldArgs args)
        {
            if (settings.IgnoreBackground || !AvailableFields.Contains(MetadataField.BackgroundImage) || vnData == null)
            {
                return base.GetBackgroundImage(args);
            }
            
            var selection = vnData.Screenshots
                .Where(x => IsImageAllowed(x))
                .Select(x => new ImageFileOption(x.Url))
                .ToList();

            var background = playniteApi.Dialogs.ChooseImageFile(selection, "Screenshots");
            
            return background != null ? new MetadataFile(background.Path) : base.GetBackgroundImage(args);
        }

        public override IEnumerable<Link> GetLinks(GetMetadataFieldArgs args)
        {
            if (!AvailableFields.Contains(MetadataField.Links) || vnData == null)
            {
                return base.GetLinks(args);
            }
            
            var links = new List<Link> { new Link("VNDB", "https://vndb.org/v" + vnData.Id) };
            
            if (!string.IsNullOrWhiteSpace(vnData.VisualNovelLinks.Renai))
            {
                links.Add(new Link("Renai", "https://renai.us/game/" + vnData.VisualNovelLinks.Renai));
            }
                
            if (!string.IsNullOrWhiteSpace(vnData.VisualNovelLinks.Wikidata))
            {
                links.Add(new Link("Wikidata", "https://www.wikidata.org/wiki/" + vnData.VisualNovelLinks.Wikidata));
            }

            return links;

        }
    }
}