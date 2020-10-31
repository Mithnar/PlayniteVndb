# Playnite VNDB Metadata Extension
> Enables VNDB as a Metadata source for Playnite

VNDB is a comprehensive database for Visual Novels

https://vndb.org/

## Installation

### .pext
- Download the pext extension: https://github.com/Mithnar/PlayniteVndb/releases
- Drag and Drop the pext file onto your Playnite application
- Accept the installion and restart Playnite
- Configure the extension to your needs (Settings../Metadata Sources/VNDB)
- You can now use "Download Metadata" under "Edit Game Details" to use VNDB as a metadata source

### .zip
- Download the zipped extension: https://github.com/Mithnar/PlayniteVndb/releases
- Unzip and drop the "Extensions" folders into your Playnite directory
- Configure the extension to your needs (Settings../Metadata Sources/VNDB)
- You can now use "Download Metadata" under "Edit Game Details" to use VNDB as a metadata source

## Screenshots
![Settings](https://raw.githubusercontent.com/Mithnar/PlayniteVndb/readme/images/vndb_settings.png "Settings")
![Game Selection](https://raw.githubusercontent.com/Mithnar/PlayniteVndb/readme/images/vndb_selection.png "game selection")
![Background Selection](https://raw.githubusercontent.com/Mithnar/PlayniteVndb/readme/images/vndb_images.png "background selection")

## Release History
For the detailed release history, head over to the [Changelog](https://github.com/Mithnar/PlayniteVndb/blob/master/CHANGELOG.md)


### Recent Releases:
#### [1.2](https://github.com/Mithnar/PlayniteVndb/releases/tag/1.2)
##### Added
- [Issue #13](https://github.com/Mithnar/PlayniteVndb/issues/13) Enable searching for a vndb entry by id (syntax: id:v2002)
- [Issue #16](https://github.com/Mithnar/PlayniteVndb/issues/16) Tag Dumps are now downloaded on start up if not present or outdated
##### Changed
- [Issue #14](https://github.com/Mithnar/PlayniteVndb/issues/14) Adjust handling of nsfw images to changes in vndb api
- [Issue #15](https://github.com/Mithnar/PlayniteVndb/issues/15) It's now possible to set a limit to tags by category and overall
##### Fixed
- [Issue #12](https://github.com/Mithnar/PlayniteVndb/issues/12) Selecting a VN when multiple VNs with the same name exist should now return the selected VN

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