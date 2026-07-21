# Starter Chest: Class Loadouts

An addon for [Starter Chest](https://github.com/mmaust005/VS-StarterChest) that gives new players
a starter loadout matching the character class they picked at creation, instead of everyone
sharing the same random pool - a Hunter and a Clockmaker get different, class-appropriate gear.

**Requires the base [Starter Chest](https://github.com/mmaust005/VS-StarterChest) mod (v1.1.0 or
later) to be installed.** This addon has no effect on its own - it plugs into Starter Chest's
public addon API to override what specific players get.

- Ships with a default loadout for all 6 vanilla character classes (`commoner`, `hunter`,
  `malefactor`, `clockmaker`, `blackguard`, `tailor`), each themed around that class's traits.
- A class with no loadout file - including any class added by another mod - just falls back to
  Starter Chest's own top-level config, unchanged.
- Adding support for a new (e.g. modded) class is one JSON file, no code and no editing of any
  other config.
- Waits for character creation to actually finish before giving the automatic chest, so a
  brand-new player's chest always matches the class they end up picking.

## Config

On first run, this addon creates `ModConfig/StarterChestClasses/` in your server/game data folder
and seeds it with one file per vanilla class (e.g. `hunter.json`), copied from the defaults
packaged with the mod. Edit any of them freely - like Starter Chest's own config, this folder is
only ever seeded once and never touched again afterwards.

Each file is named `<classcode>.json` and has the exact same shape as Starter Chest's top-level
config:

```json
{
  "RandomMode": true,
  "RandomPickCount": 3,
  "AllowDuplicatePicks": false,
  "FixedItems": [
    { "Code": "game:knife-generic-flint", "Type": "item", "MinQuantity": 1, "MaxQuantity": 1 }
  ],
  "RandomPool": [
    { "Code": "game:bow-simple", "Type": "item", "MinQuantity": 1, "MaxQuantity": 1, "Weight": 15 },
    { "Code": "game:arrow-flint", "Type": "item", "MinQuantity": 6, "MaxQuantity": 12, "Weight": 25 }
  ]
}
```

See the base mod's README for what `RandomMode`/`RandomPickCount`/`AllowDuplicatePicks`/
`FixedItems`/`RandomPool` do, and how `Weight` works - all identical here.

### Adding a class

To support a class this addon doesn't ship a default for (e.g. one added by another mod), create
`ModConfig/StarterChestClasses/<theirclasscode>.json` matching the schema above, and restart the
server. That's it - no other config to touch. This also means a mod author (or anyone in the
community) can share a single file for others to drop in.

### Removing/disabling a class

Delete that class's file from `ModConfig/StarterChestClasses/` (or edit `RandomPickCount`/
`FixedItems` down to nothing) and that class falls back to Starter Chest's own default loadout.
To disable class-based loadouts entirely, remove this addon - the base mod works standalone.

## Testing

This addon doesn't add its own commands - it plugs into Starter Chest's existing ones. `/starterchest
reset <player>` and `/starterchest preview <player>` (see the base mod's README) both automatically
use a matching class loadout when one exists, exactly like a real first join would.

## Building

Requires the base Starter Chest mod to already be built and deployed once (this addon references
its `StarterChest.dll` from the deployed Mods folder) - see that repo's README for its own build
setup. Then, from this project:

```
& "$env:USERPROFILE\dotnet-sdk10\dotnet.exe" build
```

This copies `StarterChestClassLoadouts.dll` and its assets into
`%APPDATA%\VintagestoryData\Mods\StarterChestClassLoadouts` automatically (see the `DeployMod`
target in `StarterChestClassLoadouts.csproj`), so a restart of the game/server picks up the change.

By default, the project looks for the game/data folders under `%APPDATA%`. Set the
`VINTAGE_STORY` / `VINTAGE_STORY_DATA` environment variables to override either path.

### Packaging a release

```
& "$env:USERPROFILE\dotnet-sdk10\dotnet.exe" build -c Release -t:PackMod
```

Writes `release/StarterChestClassLoadouts.zip` (gitignored, rebuilt fresh each time) - just the
runtime files, no source or dev files.

## Changelog

See [CHANGELOG.md](CHANGELOG.md).

## License

Source code is [MIT licensed](LICENSE).
