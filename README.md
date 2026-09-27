# Jellyfin.Plugin.TmdbMultiLanguage

TMDB multi-language image provider for Jellyfin 12.1.

## Features

- Movies and series
- Poster, backdrop and logo images
- Separate language priority for each image type
- Language-neutral TMDB images via `null`
- Example priority: `de,en,null`
- Optional filtering of unrated images
- Personal TMDB API key

## Install a test build

Open **Actions → Build → latest successful run** and download the `Jellyfin.Plugin.TmdbMultiLanguage` artifact. Extract the plugin ZIP and place the DLL in a dedicated Jellyfin plugin directory, then restart Jellyfin.

## Repository installation

Release builds update `manifest.json`. Once a release exists, add the raw `manifest.json` URL as a custom Jellyfin plugin repository.

## Compatibility

Current development target: Jellyfin 12.1.x / .NET 10.

Based on the functionality of Iceshadow1404/TmdbMultiLanguage and ported for Jellyfin 12.x.
