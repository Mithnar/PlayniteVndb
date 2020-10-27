# Playnite VNDB Metadata Extension
> Enables VNDB as a Metadata source for Playnite

VNDB is a comprehensive database for Visual Novels

https://vndb.org/

## Installation

- Download the zipped extension: https://github.com/Mithnar/PlayniteVndb/releases
- Unzip and drop the "Extensions" and "ExtensionsData" folders into your Playnite directory
- Configure the extension to your needs (Settings../Metadata Sources/VNDB)
- You can now use "Download Metadata" under "Edit Game Details" to use VNDB as a metadata source

## Screenshots
![Settings](https://raw.githubusercontent.com/Mithnar/PlayniteVndb/readme/images/vndb_settings.png "Settings")
![Game Selection](https://raw.githubusercontent.com/Mithnar/PlayniteVndb/readme/images/vndb_selection.png "game selection")
![Background Selection](https://raw.githubusercontent.com/Mithnar/PlayniteVndb/readme/images/vndb_images.png "background selection")

## Release History
For the detailed release history, head over to the [Changelog](https://github.com/Mithnar/PlayniteVndb/blob/master/CHANGELOG.md)


### Recent Releases:
#### [1.1.1](https://github.com/Mithnar/PlayniteVndb/releases/tag/1.1.1)
##### Changed
- [Issue #10](https://github.com/Mithnar/PlayniteVndb/issues/10) Use VndbSharp Nuget Package
##### Fixed
- [Issue #11](https://github.com/Mithnar/PlayniteVndb/issues/11) Added Id property to manifest for automatic installation

#### [1.1](https://github.com/Mithnar/PlayniteVndb/releases/tag/1.1)
##### Added
- [Playnite #10](https://playnite.link/forum/thread-24-post-67.html#pid67) Add option to enable incomplete dates
##### Changed
- [Playnite #2](https://playnite.link/forum/thread-24-post-48.html#pid48) Game description in game selection will no longer show any url tags
##### Fixed
- Fix error when cancelling game selection

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