# Changelog

## 1.0.0

Initial release.

- Class-based starter loadouts, one file per character class under
  `ModConfig/StarterChestClasses/` (e.g. `hunter.json`), seeded on first run with defaults for
  all 6 vanilla classes (`commoner`, `hunter`, `malefactor`, `clockmaker`, `blackguard`,
  `tailor`). Each is built around 2-3 guaranteed, class-themed `FixedItems` (a Hunter's bow and
  knife, a Blackguard's shield and spear, a Malefactor's trap, ...) so every chest is
  recognizable on sight, plus at least one guaranteed food item - no class starts without food,
  and no class gets a mechanical advantage (e.g. bonus inventory slots) another lacks.
  `commoner` stays the plain, unthemed baseline. Each non-commoner class also has a rare,
  low-weight copper-tier "jackpot" item in its pool (a Hunter's copper arrows, a Tailor's copper
  shears, a Clockmaker's copper hammer, a Malefactor's copper chisel, a Blackguard's copper
  spear) for the occasional lucky roll.
- Classes without a file (including any added by other mods) fall back to Starter Chest's own
  top-level config, unchanged.
- Waits for character creation to finish (not just for `characterClass` to be set, which the
  game assigns a default value for immediately) before giving the automatic chest, so it always
  matches the class the player actually ends up with.
- Built entirely on the base Starter Chest mod's public addon API
  (`StarterChestModSystem.RegisterLoadoutProvider`) - no forked or duplicated placement logic.
- `examples/` has a `templateclass` at 3 tiers (low/medium/high) to copy as a starting point for
  a new class file. Not loaded by the mod.
