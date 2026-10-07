# Schemventory

Schemventory is a Windows application for creating and tracking material lists from Minecraft schematic files.

It reads supported schematic formats, resolves the blocks into inventory materials and helps you keep track of what you still need to collect for a build.

## Features

- Read **Litematica `.litematic`** files
- Read **Sponge `.schem`** files
- Generate material lists from schematic contents
- Track collected materials and remaining amounts
- Enter collected amounts conveniently as **stacks and blocks**
- Mark materials as:
  - Missing
  - Collected
  - Replaced
  - Ignored
- Replace one required material with another Minecraft item
- Filter the material list by state
- Sort materials by state and required amount
- Persist projects and material progress using SQLite
- Display Minecraft item/block icons
- Cache item metadata and icons locally to reduce repeated downloads
- Respect Minecraft item stack sizes when calculating stacks and blocks

## Supported schematic formats

| Format | Support |
| --- | --- |
| `.litematic` | Supported |
| `.schem` | Supported |
| `.schematic` | Planned |

## Material tracking

Each material can have one of four states:

| State | Description |
| --- | --- |
| **Missing** | The material is still required. |
| **Collected** | The required amount has been collected. |
| **Replaced** | The original material has been replaced with another Minecraft item. |
| **Ignored** | The material is excluded from collection statistics. |

Collected amounts can be adjusted without manually converting everything into blocks. For example, you can enter:

```text
1 Stack + 24 Blocks
```

Schemventory automatically calculates the remaining amount using the item's actual maximum stack size.

## Material replacement

Materials can be replaced with another Minecraft item while keeping the original schematic requirement intact.

For example:

```text
Oak Planks
    ↓ replace with
Spruce Planks
```

The replacement is stored separately from the original material, allowing Schemventory to continue tracking the schematic requirement while showing the material you actually intend to use.

## Projects

Schemventory stores projects locally and keeps track of material progress between sessions.

Project data includes information such as:

- Project name
- Schematic path
- Required materials
- Collected amounts
- Material states
- Replacement materials

## Item data and icons

Schemventory uses Minecraft item metadata to determine item stack sizes and uses rendered item/block icons for the material list.

Downloaded metadata and icons are cached locally so they do not need to be fetched again every time the application starts.

## Technology

Schemventory is built with:

- **C# / .NET**
- **Windows Forms**
- **Guna.UI2**
- **SQLite** via `Microsoft.Data.Sqlite`
- **fNbt** for NBT data

External data/services currently used by the application include:

- [`misode/mcmeta`](https://github.com/misode/mcmeta) for Minecraft item metadata
- [`blockrender.dev`](https://blockrender.dev/) for item and block renders

## Building from source

1. Clone the repository:

```bash
git clone <repository-url>
```

2. Open the solution in Visual Studio.
3. Restore the NuGet packages.
4. Build and run the project.

A Windows environment is required because Schemventory uses Windows Forms.

## Usage

1. Create or open a project.
2. Select a supported Minecraft schematic file.
3. Let Schemventory generate the material list.
4. Track collected materials as you gather resources.
5. Replace or ignore materials where necessary.
6. Use the filters to focus on the materials you currently need.

## Roadmap

Some features that may be added or expanded in future versions include:

- Legacy `.schematic` support
- Further improvements to material replacement workflows
- Additional material-list tools and filters
- More project-management features

## Screenshots

Screenshots can be added here once the repository contains suitable images.

```md
![Material list](docs/images/material-list.png)
```

## Contributing

Issues and pull requests are welcome.

If you find a bug or have an idea for an improvement, please open an issue with as much detail as possible.

## Licence

Add the licence used by this repository here, for example:

```text
MIT Licence
```

If the project does not currently have a licence, remove this section until one is added.

---

Schemventory is an independent project and is not affiliated with or endorsed by Mojang Studios or Microsoft.