# Privacy

ComPewter sends only the player question by default. Local game context sharing is off until the player enables `Privacy.ShareGameContext`.

## Context Sent When Enabled

When `ShareGameContext` is true, ComPewter may include:

- season, day, year, weather, and daily luck
- current money
- current location
- farm type number
- player skill levels
- a compact inventory summary
- active quest titles
- major progression flags where available
- spoiler preference

## Extra Opt-In Context

These categories stay off unless enabled separately:

- `IncludeFriendshipData`: NPC friendship point summaries.
- `IncludeInstalledMods`: installed mod names and versions.
- `IncludeMultiplayerData`: reserved for future use; v1 does not send other players' data.

ComPewter does not intentionally send player names, farm names, save names, machine paths, multiplayer player names, or provider secrets.

## Spoilers

`AllowSpoilers` defaults to false. The prompt instructs the assistant to avoid spoiler-heavy answers unless spoilers are enabled or the player explicitly asks.

## Topic Scope

`RestrictToStardewTopics` defaults to true. This adds prompt-level instructions telling the selected provider to stay focused on Stardew Valley and ComPewter setup, while treating ambiguous real-world-looking topics as Stardew questions. It does not collect or send any extra local game data.

## Logs

API keys, bearer tokens, and common auth fields are redacted from SMAPI logs. Debug logging is quiet by default and controlled by `DebugLogging`.

## Multiplayer

Chat is local/private by default. Opening ComPewter does not mutate the world, send chat to other players, or depend on AI output for shared game state.
