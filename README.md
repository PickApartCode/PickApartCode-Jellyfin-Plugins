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
