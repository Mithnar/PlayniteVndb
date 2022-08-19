# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.1] - 2022-08-19
I planned on releasing Bulk Metadata Import in 2.1, but decided to get some features out before, as the bulk import will still take awhile
### Added
- [Issue #22](https://github.com/Mithnar/PlayniteVndb/issues/22) Option to completely disable specific fields for metadata import
- [Issue #24](https://github.com/Mithnar/PlayniteVndb/issues/24) Option to import estimated playtime as a tag

## [2.0] - 2021-12-06
Thank you [darklinkpower](https://github.com/darklinkpower) for working on the 2.0 Release
### Added
- [Issue #18](https://github.com/Mithnar/PlayniteVndb/issues/18) Option to add a prefix to tags to order and identify them on import
### Fixed
- [Issue #20](https://github.com/Mithnar/PlayniteVndb/issues/20) Extensions fails to load in the latest version of Playnite

## [1.2] - 2020-10-31
### Added
- [Issue #13](https://github.com/Mithnar/PlayniteVndb/issues/13) Enable searching for a vndb entry by id (syntax: id:v2002)
- [Issue #16](https://github.com/Mithnar/PlayniteVndb/issues/16) Tag Dumps are now downloaded on start up if not present or outdated
### Changed
- [Issue #14](https://github.com/Mithnar/PlayniteVndb/issues/14) Adjust handling of nsfw images to changes in vndb api
- [Issue #15](https://github.com/Mithnar/PlayniteVndb/issues/15) It's now possible to set a limit to tags by category and overall
### Fixed
- [Issue #12](https://github.com/Mithnar/PlayniteVndb/issues/12) Selecting a VN when multiple VNs with the same name exist should now return the selected VN

## [1.1.1] - 2020-10-27
### Changed
- [Issue #10](https://github.com/Mithnar/PlayniteVndb/issues/10) Use VndbSharp Nuget Package
### Fixed
- [Issue #11](https://github.com/Mithnar/PlayniteVndb/issues/11) Added Id property to manifest for automatic installation

## [1.1] - 2019-12-01
### Added
- [Playnite #10](https://playnite.link/forum/thread-24-post-67.html#pid67) Add option to enable incomplete dates
### Changed
- [Playnite #2](https://playnite.link/forum/thread-24-post-48.html#pid48) Game description in game selection will no longer show any url tags
### Fixed
- Fix error when cancelling game selection

## [1.0.1] - 2019-11-25
### Changed
- Tags without a matching name will now be ignored, instead of being shown as "UNKNOWN TAG"
### Fixed
- [Playnite #3](https://playnite.link/forum/thread-24-post-49.html#pid49) Incomplete release dates will now longer crash metadata downloading

## [1.0] - 2019-11-24
### Fixed
- [Issue #1](https://github.com/Mithnar/PlayniteVndb/issues/1) Format URLs in a way Playnite understands it
- [Issue #2](https://github.com/Mithnar/PlayniteVndb/issues/2) Tags should not show as "UNKNOWN TAG" if they are filtered out by Category
- [Issue #3](https://github.com/Mithnar/PlayniteVndb/issues/3) Line breaks should now be properly formatted
- [Issue #4](https://github.com/Mithnar/PlayniteVndb/issues/4) Tags should not show as "UNKNOWN TAG" if they are filtered out by Score

Special Thanks to [darklinkpower](https://github.com/darklinkpower) for testing this early release and finding those issues.

## [1.0-RC3] - 2019-11-24
### Fixed
- Fixed a bug that needed settings to be saved once, before using the plugin
- Cancelling background image selection no longer leads to cancelling the metadata download

## [1.0-RC2] - 2019-11-24
### Added
- Changelog
### Fixed
- Wikidata links will now be labeled as Wikidata
- Community scores will now be calculated correctly

## [1.0-RC1] - 2019-11-24
### Added
- Initial release

[Unreleased]: https://github.com/Mithnar/PlayniteVndb/compare/2.1...HEAD
[2.1]: https://github.com/Mithnar/PlayniteVndb/compare/2.0...2.1
[2.0]: https://github.com/Mithnar/PlayniteVndb/compare/1.2...2.0
[1.2]: https://github.com/Mithnar/PlayniteVndb/compare/1.1.1...1.2
[1.1.1]: https://github.com/Mithnar/PlayniteVndb/compare/1.1...1.1.1
[1.1]: https://github.com/Mithnar/PlayniteVndb/compare/1.0.1...1.1
[1.0.1]: https://github.com/Mithnar/PlayniteVndb/compare/1.0...1.0.1
[1.0]: https://github.com/Mithnar/PlayniteVndb/compare/1.0-RC3...1.0
[1.0-RC3]: https://github.com/Mithnar/PlayniteVndb/compare/1.0-RC2...1.0-RC3
[1.0-RC2]: https://github.com/Mithnar/PlayniteVndb/compare/1.0-RC1...1.0-RC2
[1.0-RC1]: https://github.com/Mithnar/PlayniteVndb/releases/tag/1.0-RC1
