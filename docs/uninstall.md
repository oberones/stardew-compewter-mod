# Uninstall And Save Safety

ComPewter v1 is designed for safe add/remove behavior.

## What v1 Stores

- `config.json` in the mod folder.
- Current-session chat messages in memory only.

## What v1 Does Not Store

- No custom objects, furniture, recipes, mail flags, or world state.
- No provider secrets in save data.
- No persisted chat history in save data.

## Removing The Mod

Exit the game, remove the `ComPewter` folder from `Mods`, and relaunch. Because v1 does not add saved content, removing it should not make a save unplayable.

Future versions that add a placeable computer must preserve this same safe removal rule.
