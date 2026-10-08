# Schemventory

Schemventory is a Windows application for creating and tracking Minecraft material lists from schematic files.

It reads supported schematic formats, resolves block states into inventory materials and helps track what has already been collected, what is still missing and which materials have been replaced or ignored.

## Features

- Read Litematica `.litematic` files
- Read Sponge Schematic v2 and v3 `.schem` files
- Generate material lists from schematic contents
- Track required, collected and remaining material amounts
- Enter collected amounts in stacks and blocks
- Mark materials as missing, collected, replaced or ignored
- Replace a required material with another Minecraft item
- Search available Minecraft items when selecting replacements
- Use virtualised and debounced item search when selecting replacement materials
- Reset a replacement back to the original material
- Filter the material list by state
- Sort materials by state and required amount
- Virtualise material rendering to keep large material lists responsive
- Persist projects and material progress with SQLite
- Display rendered Minecraft item and block icons
- Respect each item's maximum stack size
- Cache item metadata and rendered icons in memory and on disk
- Revalidate cached item metadata and avoid unnecessary repeated downloads
- Cache missing icon lookups to reduce repeated failed requests

## Supported schematic formats

| Format | Support |
| --- | --- |
| Litematica `.litematic` | Supported |
| Sponge Schematic v2 `.schem` | Supported |
| Sponge Schematic v3 `.schem` | Supported |

## Material tracking

Each project material can be in one of four states:

| State | Description |
| --- | --- |
| **Missing** | The material is still required. |
| **Collected** | The required amount has been collected. |
| **Replaced** | The original material has been replaced with another Minecraft item. |
| **Ignored** | The material is excluded from collection statistics. |

Collected amounts can be adjusted using stacks and individual blocks, so values do not have to be converted manually.

For example:

```text
1 Stack + 24 Blocks
```

Schemventory uses the selected item's maximum stack size when calculating collected and remaining amounts.

## Material replacement

Required materials can be replaced with another Minecraft item while keeping the original schematic material stored separately.

For example:

```text
Oak Planks
    ↓
Spruce Planks
```

The replacement dialog provides a searchable, virtualised item list with item IDs, stack-size information and an icon preview.

The search is debounced to avoid unnecessary filtering while typing.

A replacement can also be reset to restore the original material.

## Material resolution

Schemventory resolves schematic block states into the corresponding inventory materials.

The resolver includes handling for cases such as:

- doors and beds
- double-height plants
- potted plants
- candle cakes
- wall signs and hanging signs
- wall banners
- wall skull and head variants
- wall coral fans
- crops
- cauldrons
- farmland and dirt paths
- other block-to-item mappings

Blocks that do not represent collectable build materials, such as air, fluids, portals and fire, are excluded.

## Projects

Projects are stored locally in SQLite.

Stored project data includes:

- project name
- schematic path
- creation and last-opened timestamps
- required material amounts
- collected amounts
- material states
- replacement material IDs

## Item data and caching

Schemventory uses item metadata from [`misode/mcmeta`](https://github.com/misode/mcmeta) to determine Minecraft item stack sizes and populate the replacement-material list.

Item metadata is stored in a compact local cache. Cached metadata is reused between sessions and periodically revalidated to avoid unnecessary downloads.

Rendered item and block icons are provided by [`blockrender.dev`](https://blockrender.dev/) and cached both in memory and on disk to avoid repeated downloads and repeated image decoding.

Missing icon lookups are also cached temporarily so unavailable renders are not repeatedly requested.

## Performance

The material list uses virtualised rendering so only the material controls currently visible in the viewport are kept active.

Controls are recycled while scrolling instead of recreating the complete list. Item metadata and icon loading can be cancelled when a control is rebound, preventing unnecessary background work during fast scrolling.

Rendered icons are additionally cached in memory once decoded, which reduces repeated disk reads when scrolling back to previously displayed materials.

## Technology

Schemventory is built with:

- C#
- .NET 10 for Windows
- Windows Forms
- Guna.UI2.WinForms
- Microsoft.Data.Sqlite
- fNbt

## Building from source

Clone the repository:

```bash
git clone https://github.com/PIN0L33KZ/Schemventory.git
```

Open `Schemventory.slnx` in Visual Studio, restore the NuGet packages and build the project.

Schemventory targets `net10.0-windows` and requires Windows.

## External resources

Schemventory uses or integrates resources from:

- [`misode/mcmeta`](https://github.com/misode/mcmeta) for Minecraft item metadata
- [`blockrender.dev`](https://blockrender.dev/) for rendered Minecraft item and block icons
- [Icons8](https://icons8.com/) as a provider for icons used in the application

## Usage

1. Create or open a project.
2. Select a supported schematic file.
3. Let Schemventory generate the material list.
4. Track collected resources as you gather them.
5. Adjust collected amounts in stacks and blocks when needed.
6. Replace or ignore materials where appropriate.
7. Use the material-state filters to focus on the resources relevant to you.

## Project structure

The application is organised around separate readers, services, data models, forms and controls.

```text
Schemventory/
├── App/
├── Controls/
├── Data/
├── Forms/
├── Interfaces/
├── Properties/
└── Services/
```

Schematic readers produce block-state data, which is resolved into inventory materials and then persisted as project materials for display and tracking in the Windows Forms UI.

---

Schemventory is an independent project and is not affiliated with or endorsed by Mojang Studios or Microsoft.
