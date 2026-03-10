# Jellyfin Content Suggestions Plugin

This plugin adds a full suggestion workflow so users can request:

- Movies
- TV Shows
- Books / Audiobooks
- Songs

## Features

- User-facing API to submit suggestions.
- Metadata lookup using Jellyfin metadata providers to normalize the requested item.
- Duplicate detection against the existing Jellyfin library.
- Admin moderation endpoint to approve, reject, or mark requests as added.
- Admin web panel to review and moderate requests.

## API

- `POST /Suggestions` create a suggestion.
- `GET /Suggestions` list suggestions (filterable by status/user).
- `POST /Suggestions/{id}` moderate suggestion (`Approved`, `Rejected`, `Added`).

## Why the plugin might not show up in Jellyfin

Jellyfin plugin repositories **must point to a JSON manifest URL**, not a repository page.

If you add a GitHub repository URL directly (for example `https://github.com/.../PickApartCode-Jellyfin-Plugins`), Jellyfin will not find plugins because it expects the response body to be a `manifest.json` array.

Use the raw file URL instead, for example:

- `https://raw.githubusercontent.com/PickApartCode/PickApartCode-Jellyfin-Plugins/main/manifest.json`

## Publishing checklist

1. Build the plugin DLL.
2. Package the release zip containing at least:
   - `Jellyfin.Plugin.Suggestions.dll`
   - `meta.json`
3. Upload the zip to a GitHub Release.
4. Update `manifest.json`:
   - `versions[].sourceUrl` to the release zip URL
   - `versions[].checksum` with the zip checksum
   - `versions[].timestamp` and `versions[].version`
5. In Jellyfin Dashboard → Plugins → Repositories, add the raw `manifest.json` URL.

Without a valid downloadable release zip in `manifest.json`, Jellyfin may show the plugin entry but installation will fail.
