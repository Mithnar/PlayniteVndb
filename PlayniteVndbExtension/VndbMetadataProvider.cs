using System;
using System.Collections.Generic;
using System.Linq;
using Playnite.SDK;
using Playnite.SDK.Metadata;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using VndbSharp;
using VndbSharp.Models;
using VndbSharp.Models.Release;
using VndbSharp.Models.VisualNovel;

namespace PlayniteVndbExtension
{
    public class VndbMetadataProvider : OnDemandMetadataProvider
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        private readonly DescriptionFormatter _descriptionFormatter;

        private readonly MetadataRequestOptions _options;
        private readonly IPlayniteAPI _playniteApi;
        private readonly VndbMetadataSettings _settings;
        private readonly List<TagName> _tagDetails;
        private readonly Vndb _vndbClient;

        private List<MetadataField> _availableFields;

        private VisualNovel _vnData;
        private List<ProducerRelease> _vnProducers;

        public VndbMetadataProvider(MetadataRequestOptions options, List<TagName> tagDetails,
            DescriptionFormatter descriptionFormatter, VndbMetadataPlugin plugin)
        {
            _options = options;
            _tagDetails = tagDetails;
            _playniteApi = plugin.PlayniteApi;
            _vndbClient = plugin.VndbClient;
            _settings = plugin.LoadPluginSettings<VndbMetadataSettings>();
            _descriptionFormatter = descriptionFormatter;
        }

        public override List<MetadataField> AvailableFields
        {
            get
            {
                if (_availableFields == null) _availableFields = GetAvailableFields();

                return _availableFields;
            }
        }

        private List<MetadataField> GetAvailableFields()
        {
            if (_vnData == null)
                if (!GetVndbMetadata())
                    return new List<MetadataField>();

            var fields = new List<MetadataField> {MetadataField.Name};

            if (!string.IsNullOrEmpty(_vnData.Description)) fields.Add(MetadataField.Description);

            if (_vnData.Image != null && (!_vnData.IsImageNsfw || _settings.AllowNsfwImages))
                fields.Add(MetadataField.CoverImage);

            if (_vnData.Screenshots.HasItems() &&
                _vnData.Screenshots.Any(image => !image.IsNsfw || _settings.AllowNsfwImages))
                fields.Add(MetadataField.BackgroundImage);

            if (_vnData.Released != null && _vnData.Released.Day != null && _vnData.Released.Month != null &&
                _vnData.Released.Year != null)
                fields.Add(MetadataField.ReleaseDate);

            fields.Add(MetadataField.Genres);
            fields.Add(MetadataField.Links);

            if (HasViableTags()) fields.Add(MetadataField.Tags);

            if (_vnData.Rating != 0) fields.Add(MetadataField.CommunityScore);

            if (_vnProducers != null && _vnProducers.Count(p => p.IsDeveloper) > 0)
                fields.Add(MetadataField.Developers);

            if (_vnProducers != null && _vnProducers.Count(p => p.IsPublisher) > 0)
                fields.Add(MetadataField.Publishers);

            return fields;
        }

        private bool HasViableTags()
        {
            return _vnData.Tags.HasItems() && _vnData.Tags.Any(tag =>
            {
                if (TagIsAvailableForScoreAndSpoiler(tag))
                    return _tagDetails.Any(tagDetails =>
                    {
                        if (tag.Id.Equals(tagDetails.Id)) return TagIsInEnabledCategory(tagDetails);

                        return false;
                    });

                return false;
            });
        }

        private bool TagIsAvailableForScoreAndSpoiler(TagMetadata tag)
        {
            return tag.Score >= _settings.TagMinScore && LowerSpoilerLevel(tag);
        }

        private bool GetVndbMetadata()
        {
            if (_vnData != null) return true;

            if (_options.IsBackgroundDownload) return false;
            var item = _playniteApi.Dialogs.ChooseItemWithSearch(null, searchString =>
            {
                var results = _vndbClient
                    .GetVisualNovelAsync(VndbFilters.Search.Fuzzy(searchString), VndbFlags.FullVisualNovel).Result
                    .Items;
                return results.Select(vn => vn as GenericItemOption).ToList();
            }, _options.GameData.Name);

            if (item != null)
            {
                _vnData = item as VisualNovel;
                _vnProducers = _vndbClient
                    .GetReleaseAsync(VndbFilters.VisualNovel.Equals(_vnData.Id), VndbFlags.Producers)
                    .Result
                    .Items
                    .SelectMany(producers => producers.Producers)
                    .ToList();
                return true;
            }

            _vnData = null;
            return false;
        }

        public override string GetName()
        {
            if (AvailableFields.Contains(MetadataField.Name)) return _vnData.Name;

            return base.GetName();
        }

        public override List<string> GetGenres()
        {
            if (!AvailableFields.Contains(MetadataField.Genres)) return base.GetGenres();
            var genres = new List<string>();
            genres.Add("Visual Novel");
            return genres;
        }

        public override DateTime? GetReleaseDate()
        {
            if (AvailableFields.Contains(MetadataField.ReleaseDate))
                if (_vnData.Released.Day != null && _vnData.Released.Month != null && _vnData.Released.Year != null)
                    return new DateTime
                    (
                        (int) _vnData.Released.Year.Value,
                        _vnData.Released.Month.Value,
                        _vnData.Released.Day.Value
                    );


            return base.GetReleaseDate();
        }


        public override List<string> GetDevelopers()
        {
            if (AvailableFields.Contains(MetadataField.Developers))
                return new ComparableList<string>(_vnProducers.Where(p => p.IsDeveloper).Select(p => p.Name)
                    .Distinct());

            return base.GetDevelopers();
        }

        public override List<string> GetPublishers()
        {
            if (AvailableFields.Contains(MetadataField.Developers))
                return new ComparableList<string>(_vnProducers.Where(p => p.IsPublisher).Select(p => p.Name)
                    .Distinct());

            return base.GetDevelopers();
        }

        public override List<string> GetTags()
        {
            if (AvailableFields.Contains(MetadataField.Tags))
            {
                var tags = _vnData.Tags.Select(MapTagToNamedTuple)
                    .Where(tag =>
                    {
                        var (tagMetadata, tagDetails) = tag;
                        if (TagIsAvailableForScoreAndSpoiler(tagMetadata))
                        {
                            if (tagDetails != null) return TagIsInEnabledCategory(tagDetails);

                            Logger.Warn("VndbMetadataProvider: Could not find tag: " + tagMetadata.Id);
                            return false;
                        }

                        return false;
                    }).Select(tag => tag.Item2.Name).DefaultIfEmpty().ToList();
                if (tags.HasNonEmptyItems()) return tags;
            }

            return base.GetTags();
        }

        private bool TagIsInEnabledCategory(TagName tagInfo)
        {
            return tagInfo.Cat.Equals("cont") && _settings.TagEnableContent ||
                   tagInfo.Cat.Equals("ero") && _settings.TagEnableSexual ||
                   tagInfo.Cat.Equals("tech") && _settings.TagEnableTechnical;
        }

        private (TagMetadata, TagName) MapTagToNamedTuple(TagMetadata tag)
        {
            var details = _tagDetails.Find(tagDetails =>
            {
                if (tagDetails.Id.Equals(tag.Id)) return true;

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
                    return !_settings.TagMaxSpoilerLevel.Equals(PlayniteVndbExtension.SpoilerLevel.None);
                case VndbSharp.Models.Common.SpoilerLevel.Major:
                    return _settings.TagMaxSpoilerLevel.Equals(PlayniteVndbExtension.SpoilerLevel.Major);
                default:
                    return false;
            }
        }

        public override string GetDescription()
        {
            if (AvailableFields.Contains(MetadataField.Description))
                return _descriptionFormatter.Format(_vnData.Description);

            return base.GetDescription();
        }

        public override int? GetCommunityScore()
        {
            if (AvailableFields.Contains(MetadataField.CommunityScore)) return (int) (_vnData.Rating * 10.0);

            return base.GetCommunityScore();
        }

        public override MetadataFile GetCoverImage()
        {
            if (!AvailableFields.Contains(MetadataField.CoverImage) && _vnData.Image != null)
                if (!_vnData.IsImageNsfw || _settings.AllowNsfwImages)
                    return new MetadataFile(_vnData.Image);


            return base.GetCoverImage();
        }

        public override MetadataFile GetBackgroundImage()
        {
            if (!AvailableFields.Contains(MetadataField.BackgroundImage)) return base.GetBackgroundImage();
            var selection = (from screenshot in _vnData.Screenshots
                where !screenshot.IsNsfw || _settings.AllowNsfwImages
                select new ImageFileOption(screenshot.Url)).ToList();

            var background = _playniteApi.Dialogs.ChooseImageFile(selection, "Screenshots");
            if (background != null) return new MetadataFile(background.Path);

            return base.GetBackgroundImage();
        }

        public override List<Link> GetLinks()
        {
            if (!AvailableFields.Contains(MetadataField.Links)) return base.GetLinks();
            var links = new List<Link> {new Link("VNDB", "https://vndb.org/v" + _vnData.Id)};
            if (!string.IsNullOrWhiteSpace(_vnData.VisualNovelLinks.Wikipedia))
                links.Add(new Link("Wikipedia", "https://en.wikipedia.org/wiki/" + _vnData.VisualNovelLinks.Wikipedia));
            if (!string.IsNullOrWhiteSpace(_vnData.VisualNovelLinks.Renai))
                links.Add(new Link("Renai", "https://renai.us/game/" + _vnData.VisualNovelLinks.Renai));
            if (!string.IsNullOrWhiteSpace(_vnData.VisualNovelLinks.Wikidata))
                links.Add(new Link("Wikidata", "https://www.wikidata.org/wiki/" + _vnData.VisualNovelLinks.Wikidata));

            return links;
        }
    }
}