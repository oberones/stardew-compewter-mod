# ComPewter

ComPewter is a Stardew Valley SMAPI mod that adds an in-game farm computer chat assistant. Players can open the chat with `F8`, ask practical gameplay questions, and route answers through their chosen AI provider.

The first implementation is intentionally save-safe: it uses a hotkey-only computer interface, writes no custom objects into saves, persists no chat history, and defaults to `Disabled` until the player configures a provider.

## Features

- Custom in-game chat menu with keyboard input, scrolling, loading, success, and friendly error states.
- Provider-agnostic AI client architecture for Anthropic, OpenAI, Ollama, and OpenAI-compatible custom endpoints.
- Strongly typed `config.json` with safe defaults, validation, timeout handling, max response length, and bounded session history.
- Optional local game context collection for date, season, weather, luck, money, skills, location, inventory, quests, progression, friendships, and installed mods.
- Soft Stardew-topic guardrails that redirect clearly unrelated questions while preserving Stardew-adjacent topics.
- Privacy-first defaults: game context sharing is off until enabled, spoiler-heavy answers are off by default, and secrets are redacted from logs.

## Requirements

- Stardew Valley 1.6+
- SMAPI 4.0+
- .NET SDK 6.0 or newer

## Build

```sh
dotnet build
```

The SMAPI mod build package copies the debug build to the configured Stardew Valley `Mods/ComPewter` folder and creates a zip package under `bin/Debug/net6.0/`.

## Setup

1. Install/build the mod.
2. Launch Stardew Valley once so SMAPI creates `config.json`.
3. Edit `config.json`.
4. Set `Provider` to `Anthropic`, `OpenAI`, `Ollama`, or `Custom`.
5. Fill in the provider model, key/token, and base URL where needed.
6. Press `F8` in-game to open ComPewter.

Context sharing is one of ComPewter's most useful features, but it is privacy-sensitive. To enable context-aware answers, set:

```json
{
  "Privacy": {
    "ShareGameContext": true
  }
}
```

See `docs/provider-setup.md` and `docs/privacy.md` for details.

## Current Scope

ComPewter v1 opens from a hotkey rather than a placed computer object. This keeps the first release safe for existing saves while the provider, chat, privacy, and context systems settle.

Known limitations:

- No controller-specific UI polish yet.
- No custom sprite/object/furniture is added yet.
- No persisted chat history in v1.
- Custom providers currently use an OpenAI-compatible chat-completions shape.

## Documentation

- `docs/quickstart.md`
- `docs/provider-setup.md`
- `docs/privacy.md`
- `docs/troubleshooting.md`
- `docs/uninstall.md`
- `docs/architecture.md`

## Constitution

This project is governed by `.specify/memory/constitution.md`. Feature work must prioritize save safety, privacy, stability, vanilla-friendly player experience, and maintainable SMAPI architecture.
