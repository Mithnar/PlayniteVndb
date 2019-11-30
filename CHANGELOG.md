# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
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

[Unreleased]: https://github.com/Mithnar/PlayniteVndb/compare/1.0.1...HEAD
[1.0.1]: https://github.com/Mithnar/PlayniteVndb/compare/1.0...1.0.1
[1.0]: https://github.com/Mithnar/PlayniteVndb/compare/1.0-RC3...1.0
[1.0-RC3]: https://github.com/Mithnar/PlayniteVndb/compare/1.0-RC2...1.0-RC3
[1.0-RC2]: https://github.com/Mithnar/PlayniteVndb/compare/1.0-RC1...1.0-RC2
[1.0-RC1]: https://github.com/Mithnar/PlayniteVndb/releases/tag/1.0-RC1
