# Changelog

## 1.0.0

Initial release.

- Class-based starter loadouts, one file per character class under
  `ModConfig/StarterChestClasses/` (e.g. `hunter.json`), seeded on first run with defaults for
  all 6 vanilla classes (`commoner`, `hunter`, `malefactor`, `clockmaker`, `blackguard`,
  `tailor`). Every class shares the same baseline survival essentials and food as Starter
  Chest's own default pool, plus one guaranteed class-themed item - no class starts without
  food, and no class gets a mechanical advantage (e.g. bonus inventory slots) another lacks.
- Classes without a file (including any added by other mods) fall back to Starter Chest's own
  top-level config, unchanged.
- Waits for character creation to finish (not just for `characterClass` to be set, which the
  game assigns a default value for immediately) before giving the automatic chest, so it always
  matches the class the player actually ends up with.
- Built entirely on the base Starter Chest mod's public addon API
  (`StarterChestModSystem.RegisterLoadoutProvider`) - no forked or duplicated placement logic.
