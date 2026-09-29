# Jellyfin.Plugin.TmdbMultiLanguage

A multi-language TMDB image provider for Jellyfin 12.1.

The plugin extends Jellyfin's image search with configurable TMDB language priorities, making it possible to prefer localized artwork while still falling back to other languages or language-neutral images.

## Features

- Supports movies, series, seasons and episodes
- Movie and series posters, backdrops and logos
- Season posters
- Episode still images
- Separate language priorities for poster, backdrop and logo images
- Language-neutral TMDB images using `null`
- Configurable fallback order, for example `de,en,null`
- Optional filtering of unrated images
- Uses your own TMDB API key
- Compatible with Jellyfin 12.1 / .NET 10

## Installation

The recommended installation method is through Jellyfin's plugin repository system.

1. Open **Dashboard → Plugins → Repositories** in Jellyfin.
2. Add a new repository, for example with the name **TMDB Multi-Language Images**.
3. Use the following repository URL:

   `https://raw.githubusercontent.com/GibbershGuru/Jellyfin.Plugin.TmdbMultiLanguage/main/manifest.json`

4. Save the repository.
5. Open the Jellyfin plugin catalog and install **TMDB Multi-Language Images**.
6. Restart the Jellyfin server after installation or an update.

## Configuration

Open the plugin settings in the Jellyfin dashboard and enter your TMDB API key.

Language priorities are comma-separated and evaluated from left to right.

Example:

```text
de,en,null
```

This means:

1. Prefer German images.
2. Fall back to English images.
3. Fall back to language-neutral TMDB images.

The priorities for posters, backdrops and logos can be configured independently. If an image-type-specific value is empty, the general preferred-language setting is used.

### Language-neutral images

TMDB can provide artwork without an assigned language. Use `null` in the priority list to include these images.

### Episode images

Episode stills are retrieved from TMDB without applying the poster/logo language filter because episode still images are generally not language-specific.

## Compatibility

Current target:

- Jellyfin 12.1.x
- .NET 10

Older Jellyfin versions are not the target of this port.

## Updating

When a new version is published, it is added to `manifest.json`. Jellyfin can then discover the update through the configured plugin repository. Restart Jellyfin after installing an update so the new plugin assembly is loaded.

## Credits

This project is based on the functionality of the original **TmdbMultiLanguage** plugin by Iceshadow1404 and has been ported and extended for Jellyfin 12.x.
