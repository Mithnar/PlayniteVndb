# Playnite VNDB Metadata Extension
> Enables VNDB as a Metadata source for Playnite

Initial Release of the VNDB Playnite Extension.

https://vndb.org/

VNDB is a comprehensive database for Visual Novels

## Installation

- Download the zipped extension: https://github.com/Mithnar/PlayniteVndb/releases
- Unzip and drop the "Extensions" and "ExtensionsData" folders into your Playnite directory
- Configure the extension to your needs (Settings../Metadata Sources/VNDB)
- You can now use "Download Metadata" under "Edit Game Details" to use VNDB as a metadata source

## Screenshots
![alt text](https://raw.githubusercontent.com/Mithnar/PlayniteVndb/readme/images/vndb_settings.png "Settings")
![alt text](https://raw.githubusercontent.com/Mithnar/PlayniteVndb/readme/images/vndb_selection.png "game selection")
![alt text](https://raw.githubusercontent.com/Mithnar/PlayniteVndb/readme/images/vndb_images.png "background selection")

## Release History
For the detailed release history, head over to the [Changelog](https://github.com/Mithnar/PlayniteVndb/blob/master/CHANGELOG.md)

### Recent Releases:
#### [1.0.1](https://github.com/Mithnar/PlayniteVndb/releases/tag/1.0.1)
##### Fixed
- [Playnite #3](https://playnite.link/forum/thread-24-post-49.html#pid49) Incomplete release dates will now longer crash metadata downloading
##### Changed
- Tags without a matching name will now be ignored, instead of being shown as "UNKNOWN TAG"

#### [1.0](https://github.com/Mithnar/PlayniteVndb/releases/tag/1.0)
##### Fixed
- [Issue #1](https://github.com/Mithnar/PlayniteVndb/issues/1) Format URLs in a way Playnite understands it
- [Issue #2](https://github.com/Mithnar/PlayniteVndb/issues/2) Tags should not show as "UNKNOWN TAG" if they are filtered out by Category
- [Issue #3](https://github.com/Mithnar/PlayniteVndb/issues/3) Line breaks should now be properly formatted
- [Issue #4](https://github.com/Mithnar/PlayniteVndb/issues/4) Tags should not show as "UNKNOWN TAG" if they are filtered out by Score

## Credits
https://github.com/KuroThing

For hacking the VNDB Source into an older Playnite build without Metadata Extension support.

https://github.com/Nikey646/VndbSharp

For making my life easier by writing a VNDB API client.

https://github.com/JosefNemec/Playnite

For creating Playnite

## Meta
Distributed under the MIT license. See [LICENSE](https://github.com/Mithnar/PlayniteVndb/blob/master/LICENSE) for more information.

[https://github.com/Mithnar/PlayniteVndb](https://github.com/Mithnar/)

## Contributing

1. Fork it (<https://github.com/Mithnar/PlayniteVndb/fork>)
2. Create your feature branch (`git checkout -b feature/fooBar`)
3. Commit your changes (`git commit -am 'Add some fooBar'`)
4. Push to the branch (`git push origin feature/fooBar`)
5. Create a new Pull Request