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
        private static readonly ILogger logger = LogManager.GetLogger();
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
                if (isVndbId(searchString))
                {
                    search = vndbClient.GetVisualNovelAsync(VndbFilters.Id.Equals(retrieveVndbId(searchString)),
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
                    return new VndbItemOption(vn.Name, descriptionFormatter.RemoveTags(vn.Description), vn.Id);
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

        private List<MetadataField> GetAvailableFields()
        {
            if (vnData == null && !GetVndbMetadata())
            {
                return new List<MetadataField>();
            }

            var fields = new List<MetadataField>
            {
                MetadataField.Name,
                MetadataField.Genres,
                MetadataField.Links
            };

            if (!string.IsNullOrEmpty(vnData.Description))
            {
                fields.Add(MetadataField.Description);
            }
                

            if (vnData.Image != null && IsImageAllowed(vnData))
            {
                fields.Add(MetadataField.CoverImage);
            }

            if (vnData.Screenshots.HasItems() &&
                vnData.Screenshots.Any(IsImageAllowed))
            {
                fields.Add(MetadataField.BackgroundImage);
            }

            if (vnData.Released != null && vnData.Released.Year != null)
            {
                fields.Add(MetadataField.ReleaseDate);
            }

            if (HasViableTags())
            {
                fields.Add(MetadataField.Tags);
            }

            if (vnData.Rating != 0)
            {
                fields.Add(MetadataField.CommunityScore);
            }

            if (vnProducers != null && vnProducers.Count(p => p.IsDeveloper) > 0)
            {
                fields.Add(MetadataField.Developers);
            }

            if (vnProducers != null && vnProducers?.Count(p => p.IsPublisher) > 0)
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

        private bool isVndbId(string searchString)
        {
            var onlyNumbersMatcher = new Regex("^[\\d]+$");
            return searchString.ToLower().StartsWith("id:v") && onlyNumbersMatcher.IsMatch(searchString.Substring(4));
        }

        private uint retrieveVndbId(string searchString)
        {
            return uint.Parse(searchString.Substring(4));
        }

        public override string GetName(GetMetadataFieldArgs args)
        {
            if (AvailableFields.Contains(MetadataField.Name) && vnData != null)
                return vnData.Name;

            return base.GetName(args);
        }

        public override IEnumerable<MetadataProperty> GetGenres(GetMetadataFieldArgs args)
        {
            if (AvailableFields.Contains(MetadataField.Genres) && vnData != null)
            {
                return new List<MetadataProperty>
                {
                    new MetadataNameProperty("Visual Novel")
                };
            }

            return base.GetGenres(args);
        }

        public override ReleaseDate? GetReleaseDate(GetMetadataFieldArgs args)
        {
            if (AvailableFields.Contains(MetadataField.ReleaseDate) && vnData != null)
            {
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
            }

            return base.GetReleaseDate(args);
        }

        public override IEnumerable<MetadataProperty> GetDevelopers(GetMetadataFieldArgs args)
        {
            if (AvailableFields.Contains(MetadataField.Developers) && vnData != null)
            {
                return vnProducers.Where(p => p.IsDeveloper)
                    .Select(p => p.Name).Distinct()
                    .Select(s => new MetadataNameProperty(s)).ToList();
            }

            return base.GetDevelopers(args);
        }

        public override IEnumerable<MetadataProperty> GetPublishers(GetMetadataFieldArgs args)
        {
            if (AvailableFields.Contains(MetadataField.Developers) && vnData != null)
            {
                return vnProducers.Where(p => p.IsPublisher)
                    .Select(p => p.Name).Distinct()
                    .Select(s => new MetadataNameProperty(s)).ToList();
            }

            return base.GetDevelopers(args);
        }

        public override IEnumerable<MetadataProperty> GetTags(GetMetadataFieldArgs args)
        {
            if (AvailableFields.Contains(MetadataField.Tags) && vnData != null)
            {
                var tags = new List<string>();
                var contentTags = 0;
                var sexualTags = 0;
                var technicalTags = 0;
                foreach (var (tagMetadata, tagName) in vnData.Tags.OrderByDescending(tag => tag.Score).Select(MapTagToNamedTuple))
                {
                    if (tagName == null)
                    {
                        logger.Warn("VndbMetadataProvider: Could not find tag: " + tagMetadata.Id);
                    }
                    else if (TagIsAvailableForScoreAndSpoiler(tagMetadata) &&
                             TagIsInEnabledCategory(tagName) &&
                             tags.Count < settings.MaxAllTags)
                    {
                        if (tagName.Cat.Equals("cont"))
                        {
                            contentTags = AddContentTagIfNotMax(contentTags, tags, tagName);
                        }
                        else if (tagName.Cat.Equals("ero"))
                        {
                            sexualTags = AddSexualTagIfNotMax(sexualTags, tags, tagName);
                        }
                        else if (tagName.Cat.Equals("tech"))
                        {
                            technicalTags = AddTechnicalTagIfNotMax(technicalTags, tags, tagName);
                        }
                    }
                }
                if (tags.HasNonEmptyItems())
                {
                    return tags.Select(s => new MetadataNameProperty(s)).ToList();
                }
            }

            return base.GetTags(args);
        }

        private int AddTechnicalTagIfNotMax(int technicalTags, List<string> tags, TagName tagName)
        {
            if (technicalTags < settings.MaxTechnicalTags)
            {
                ++technicalTags;
                tags.Add(tagName.Name);
            }

            return technicalTags;
        }

        private int AddSexualTagIfNotMax(int sexualTags, List<string> tags, TagName tagName)
        {
            if (sexualTags < settings.MaxSexualTags)
            {
                ++sexualTags;
                tags.Add(tagName.Name);
            }

            return sexualTags;
        }

        private int AddContentTagIfNotMax(int contentTags, List<string> tags, TagName tagName)
        {
            if (contentTags < settings.MaxContentTags)
            {
                ++contentTags;
                tags.Add(tagName.Name);
            }

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
            if (AvailableFields.Contains(MetadataField.Description) && vnData != null)
            {
                return descriptionFormatter.Format(vnData.Description);
            }

            return base.GetDescription(args);
        }

        public override int? GetCommunityScore(GetMetadataFieldArgs args)
        {
            if (AvailableFields.Contains(MetadataField.CommunityScore) && vnData != null)
            {
                return (int)(vnData.Rating * 10.0);
            }

            return base.GetCommunityScore(args);
        }

        public override MetadataFile GetCoverImage(GetMetadataFieldArgs args)
        {
            if (AvailableFields.Contains(MetadataField.CoverImage) && vnData != null)
            {
                if (IsImageAllowed(vnData))
                {
                    return new MetadataFile(vnData.Image);
                }
            }

            return base.GetCoverImage(args);
        }

        public override MetadataFile GetBackgroundImage(GetMetadataFieldArgs args)
        {
            if (AvailableFields.Contains(MetadataField.BackgroundImage) && vnData != null)
            {
                var selection = vnData.Screenshots
                    .Where(x => IsImageAllowed(x))
                    .Select(x => new ImageFileOption(x.Url))
                    .ToList();

                var background = playniteApi.Dialogs.ChooseImageFile(selection, "Screenshots");
                if (background != null)
                {
                    return new MetadataFile(background.Path);
                }
            }

            return base.GetBackgroundImage(args);
        }

        public override IEnumerable<Link> GetLinks(GetMetadataFieldArgs args)
        {
            if (AvailableFields.Contains(MetadataField.Links) && vnData != null)
            {
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

            return base.GetLinks(args);
        }
    }
}