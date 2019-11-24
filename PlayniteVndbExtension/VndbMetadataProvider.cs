using System;
using System.Collections.Generic;
using System.Linq;
using playnite.metadata.vndb;
using playnite.metadata.vndb.settings;
using Playnite.SDK;
using Playnite.SDK.Metadata;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using VndbSharp;
using VndbSharp.Interfaces;
using VndbSharp.Models;
using VndbSharp.Models.Common;
using VndbSharp.Models.Release;
using VndbSharp.Models.VisualNovel;

namespace playnite.metadata.vndb.provider
{
    public class VndbMetadataProvider : OnDemandMetadataProvider
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        
        private readonly MetadataRequestOptions _options;
        private readonly List<TagName> _tagNames;
        private readonly VndbMetadataPlugin _plugin;

        private readonly Vndb _vndbClient;

        private VisualNovel _vnData;
        private List<ProducerRelease> _vnProducers;
        private VndbMetadataSettings _settings;


        public VndbMetadataProvider(MetadataRequestOptions options, List<TagName> tagNames, VndbMetadataPlugin plugin)
        {
            logger.Debug("Constuct Provider");
            _options = options;
            _tagNames = tagNames;
            _plugin = plugin;
            _vndbClient = plugin.VndbClient;
           _settings = plugin.LoadPluginSettings<VndbMetadataSettings>();
        }

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

        private List<MetadataField> GetAvailableFields()
        {
            logger.Debug("GetAvailableFields");

            if (_vnData == null)
            {
                if (!GetVndbMetadata())
                {
                    return new List<MetadataField>();
                }
            }

            var fields = new List<MetadataField> {MetadataField.Name};
            if (!string.IsNullOrEmpty(_vnData.Description))
            {
                fields.Add(MetadataField.Description);
            }

            if (_vnData.Image != null && (!_vnData.IsImageNsfw || _settings.AllowNsfwImages))
            {
                fields.Add(MetadataField.CoverImage);
            }

            if (_vnData.Screenshots.HasItems() && (_vnData.Screenshots.Any(image => !image.IsNsfw || _settings.AllowNsfwImages )))
            {
                fields.Add(MetadataField.BackgroundImage);
            }

            if (_vnData.Released != null)
            {
                fields.Add(MetadataField.ReleaseDate);
            }

            fields.Add(MetadataField.Genres);
            fields.Add(MetadataField.Links);

            if (HasViableTags())
            {
                fields.Add(MetadataField.Tags);
            }

            if (_vnData.Rating != 0)
            {
                fields.Add(MetadataField.CommunityScore);
            }

            if (_vnProducers != null && _vnProducers.Count(p => p.IsDeveloper) > 0)
            {
                fields.Add(MetadataField.Developers);
            }
            
            if (_vnProducers != null && _vnProducers.Count(p => p.IsPublisher) > 0)
            {
                fields.Add(MetadataField.Publishers);
            }
            return fields;
        }

        private bool HasViableTags()
        {
            return _vnData.Tags.HasItems() && _vnData.Tags.Any(tag =>
            {
                return _tagNames.Any(namedTag => (namedTag.Cat.Equals("cont") && _settings.TagEnableContent) ||
                                                 (namedTag.Cat.Equals("ero") && _settings.TagEnableSexual) ||
                                                 (namedTag.Cat.Equals("tech") && _settings.TagEnableTechnical));
            });
        }

        private bool GetVndbMetadata()
        {
            if (_vnData != null)
            {
                return true;
            }

            if (_options.IsBackgroundDownload) return false;
            var item = _plugin.PlayniteApi.Dialogs.ChooseItemWithSearch(null, (searchString) =>
            {
                var results = _vndbClient.GetVisualNovelAsync(VndbFilters.Search.Fuzzy(searchString), VndbFlags.FullVisualNovel).Result.Items;
                return results.Select(vn => vn as GenericItemOption).ToList();
            }, _options.GameData.Name);

            if (item != null)
            {
                _vnData = item as VisualNovel;
                _vnProducers = this._vndbClient
                    .GetReleaseAsync(VndbFilters.VisualNovel.Equals(_vnData.Id), VndbFlags.Producers)
                    .Result
                    .Items
                    .SelectMany(producers => producers.Producers)
                    .ToList();
                return true;
            }
            else
            {
                _vnData = null;
                return false;
            }

        }

        public override string GetName()
        {
            if (AvailableFields.Contains(MetadataField.Name))
            {
                return _vnData.Name;
            }

            return base.GetName();
        }

        public override List<string> GetGenres()
        {
            if (!AvailableFields.Contains(MetadataField.Genres)) return base.GetGenres();
            var genres = new List<String>();
            genres.Add("Visual Novel");
            return genres;

        }

        public override DateTime? GetReleaseDate()
        {
            if (AvailableFields.Contains(MetadataField.ReleaseDate))
            {
                return new DateTime
                (
                    (int) _vnData.Released.Year.Value, 
                    _vnData.Released.Month.Value,
                    _vnData.Released.Day.Value
                );
            }

            return base.GetReleaseDate();
        }


        public override List<string> GetDevelopers()
        {
            if (AvailableFields.Contains(MetadataField.Developers))
            {
                return new ComparableList<String>(_vnProducers.Where(p => p.IsDeveloper).Select(p => p.Name).Distinct());
            }
            
            return base.GetDevelopers();
        }

        public override List<string> GetPublishers()
        {
            if (AvailableFields.Contains(MetadataField.Developers))
            {
                return new ComparableList<String>(_vnProducers.Where(p => p.IsPublisher).Select(p => p.Name).Distinct());
            }
            
            return base.GetDevelopers();
        }

        public override List<string> GetTags()
        {
            if (AvailableFields.Contains(MetadataField.Tags))
            {
                return _vnData.Tags.Select(tag =>
                {
                    var tag_name = _tagNames.Find(namedTag =>
                    {
                        if (namedTag.Id.Equals(tag.Id) && tag.Score > _settings.TagMinScore && lowerSpoilerLevel(tag))
                        {
                            return namedTag.Cat.Equals("cont") && _settings.TagEnableContent ||
                                   namedTag.Cat.Equals("ero") && _settings.TagEnableSexual ||
                                   namedTag.Cat.Equals("tech") && _settings.TagEnableTechnical;
                        }

                        return false;
                    });
                    if(tag_name != null)
                    {
                        return tag_name.Name;
                    }
                    return "UNKNOWN TAG";
                }).DefaultIfEmpty().ToList();
            }

            return base.GetTags();
        }

        private bool lowerSpoilerLevel(TagMetadata tag)
        {
            switch (tag.SpoilerLevel)
            {
                case SpoilerLevel.None:
                    return true;
                case SpoilerLevel.Minor:
                    return !_settings.TagMaxSpoilerLevel.Equals(PlayniteVndbExtension.SpoilerLevel.None);
                case SpoilerLevel.Major:
                    return _settings.TagMaxSpoilerLevel.Equals(PlayniteVndbExtension.SpoilerLevel.Major);
                default:
                    return false;
            }
        }

        public override string GetDescription()
        {
            if (AvailableFields.Contains(MetadataField.Description))
            {
                return _vnData.Description;
            }
            
            return base.GetDescription();
        }

        public override int? GetCommunityScore()
        {
            if (AvailableFields.Contains(MetadataField.CommunityScore))
            {
                return (int) (_vnData.Rating*10.0);
            }

            return base.GetCommunityScore();
        }

        public override MetadataFile GetCoverImage()
        {
            if (!AvailableFields.Contains(MetadataField.CoverImage)) return base.GetCoverImage();
            if (!_vnData.IsImageNsfw || _settings.AllowNsfwImages)
            {
                return new MetadataFile(_vnData.Image);
            }
            return base.GetCoverImage();
        }

        public override MetadataFile GetBackgroundImage()
        {
            if (!AvailableFields.Contains(MetadataField.BackgroundImage)) return base.GetBackgroundImage();
            var selection = (from screenshot in _vnData.Screenshots where !screenshot.IsNsfw || _settings.AllowNsfwImages select new ImageFileOption(screenshot.Url)).ToList();

            var background = _plugin.PlayniteApi.Dialogs.ChooseImageFile(selection, "Screenshots");
            return new MetadataFile(background.Path);
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