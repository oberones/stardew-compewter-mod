# ComPewter

<<<<<<< Updated upstream
ComPewter is a Stardew Valley SMAPI mod that adds an in-game farm computer chat assistant. Players can open the chat with `F8`, ask practical gameplay questions, and route answers through their chosen AI provider.
=======
<<<<<<< Updated upstream
ComPewter is a Stardew Valley mod concept for an in-game computer assistant. Players can open a computer-like chat interface and ask practical game questions, such as what to plant, where to find a fish, when a villager is available, or which gifts someone likes.
>>>>>>> Stashed changes

The first implementation is intentionally save-safe: it uses a hotkey-only computer interface, writes no custom objects into saves, persists no chat history, and defaults to `Disabled` until the player configures a provider.

## Features

<<<<<<< Updated upstream
- Custom in-game chat menu with keyboard input, scrolling, loading, success, and friendly error states.
- Provider-agnostic AI client architecture for Anthropic, OpenAI, Ollama, and OpenAI-compatible custom endpoints.
- Strongly typed `config.json` with safe defaults, validation, timeout handling, max response length, and bounded session history.
- Local game context collection for date, season, weather, luck, money, skills, location, inventory, quests, and progression, with optional sensitive categories.
- Soft Stardew-topic guardrails that redirect clearly unrelated questions while preserving Stardew-adjacent topics.
- Privacy-aware defaults: game context sharing is on with an in-chat opt-out notice, spoiler-heavy answers are off by default, and secrets are redacted from logs.
=======
## Current State

This repository contains the initial SMAPI mod scaffold:

- SMAPI manifest and C# project file
- Config model with provider, model, API key, endpoint, and open-menu key
- AI provider interface and factory
- Placeholder provider implementation
- Basic in-game chat menu opened with `F8`
- Console command fallback: `compewter <question>`

Live provider calls are intentionally stubbed for now. The scaffold is ready for provider-specific HTTP clients and Stardew data grounding.
=======
ComPewter is a Stardew Valley SMAPI mod that opens an in-game farm computer chat assistant. Ask practical gameplay questions like what to plant, where to catch a fish, what a villager likes, what to do on a rainy day, or how to plan before the season ends.

ComPewter uses the AI provider you choose: Anthropic, OpenAI, Ollama, or a custom OpenAI-compatible endpoint.

## What It Does

- Opens a custom chat window in-game with `F8`.
- Answers Stardew Valley gameplay questions in a cozy, concise style.
- Uses current save context by default, including season, weather, luck, money, skills, location, inventory, quests, and progression.
- Keeps sensitive context categories opt-in, including friendship data, installed mod list, and multiplayer data.
- Redirects clearly unrelated questions back toward Stardew help while treating ambiguous topics like cooking, money, weather, crops, fishing, and relationships as game questions.
- Handles provider setup problems, timeouts, bad keys, and unavailable local servers with friendly errors.
- Avoids save-risky behavior in v1: no custom objects, recipes, mail flags, persisted chat history, or world mutations.
>>>>>>> Stashed changes
>>>>>>> Stashed changes

## Requirements

- Stardew Valley 1.6+
- SMAPI 4.0+
- An AI provider account/key, a local Ollama server, or a compatible custom endpoint

## Quick Setup

<<<<<<< Updated upstream
=======
<<<<<<< Updated upstream
From this folder:

>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
Context sharing is one of ComPewter's most useful features, so it is enabled by default. ComPewter shows an in-chat notice explaining how to disable it. To turn context sharing off, set:
=======
The mod creates/reads `config.json` with these settings. A safe template is included as `config.example.json`:
=======
1. Install ComPewter into your Stardew Valley `Mods` folder.
2. Launch Stardew Valley through SMAPI once, then quit.
3. Open `Mods/ComPewter/config.json`.
4. Set `Provider` to `Anthropic`, `OpenAI`, `Ollama`, or `Custom`.
5. Fill in the matching provider settings.
6. Load a save and press `F8`.

If `Provider` is still `Disabled`, the chat window opens and shows setup guidance instead of contacting an AI provider.

## Provider Examples

### Ollama

```json
{
  "Provider": "Ollama",
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "Model": "llama3.1"
  }
}
```

### OpenAI

```json
{
  "Provider": "OpenAI",
  "OpenAI": {
    "BaseUrl": "https://api.openai.com",
    "Model": "gpt-4.1-mini",
    "ApiKey": "your-api-key-here"
  }
}
```

### Anthropic

```json
{
  "Provider": "Anthropic",
  "Anthropic": {
    "BaseUrl": "https://api.anthropic.com",
    "Model": "claude-sonnet-4-5",
    "ApiKey": "your-api-key-here"
  }
}
```

### Custom

```json
{
  "Provider": "Custom",
  "Custom": {
    "BaseUrl": "https://example.local",
    "EndpointPath": "/v1/chat/completions",
    "Model": "my-model",
    "AuthHeaderName": "Authorization",
    "AuthToken": "Bearer your-token-here",
    "RequestFormat": "OpenAICompatible"
  }
}
```

## Privacy Defaults

Game context sharing is on by default because current-save guidance is the main reason to use ComPewter. At the start of a new chat, ComPewter shows a notice explaining that context may be sent to your selected provider.

To disable game context sharing:
>>>>>>> Stashed changes
>>>>>>> Stashed changes

```json
{
  "Privacy": {
    "ShareGameContext": false
  }
}
```

<<<<<<< Updated upstream
See `docs/provider-setup.md` and `docs/privacy.md` for details.
=======
<<<<<<< Updated upstream
## Planned Provider Behavior
>>>>>>> Stashed changes

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

<<<<<<< Updated upstream
This project is governed by `.specify/memory/constitution.md`. Feature work must prioritize save safety, privacy, stability, vanilla-friendly player experience, and maintainable SMAPI architecture.
=======
The current UI is a minimal text-based menu. Good next steps:

- Add real provider HTTP implementations
- Add request cancellation and timeouts
- Persist chat history per save
- Ground prompts with live game state
- Add Generic Mod Config Menu integration
- Improve the computer art, input handling, scrolling, and response wrapping
=======
By default, ComPewter does not intentionally send player names, farm names, save names, machine paths, multiplayer player names, provider secrets, friendship data, or installed mod lists. API keys and tokens are redacted from logs.

## Useful Config Options

```json
{
  "OpenMenuKey": "F8",
  "RequestTimeoutSeconds": 30,
  "MaxResponseTokens": 700,
  "MaxRetainedMessages": 20,
  "RestrictToStardewTopics": true,
  "Privacy": {
    "ShareGameContext": true,
    "AllowSpoilers": false,
    "IncludeFriendshipData": false,
    "IncludeInstalledMods": false,
    "IncludeMultiplayerData": false,
    "RetainConversationHistory": true
  }
}
```

- `OpenMenuKey`: key used to open the chat window.
- `RequestTimeoutSeconds`: how long ComPewter waits for the provider.
- `MaxResponseTokens`: rough maximum answer length.
- `MaxRetainedMessages`: number of current-session chat messages retained for follow-up questions.
- `RestrictToStardewTopics`: keeps the assistant focused on Stardew.
- `ShareGameContext`: sends current save context with questions.
- `AllowSpoilers`: allows more spoiler-heavy answers.
- `RetainConversationHistory`: lets follow-up questions use recent chat history.

## Current Limitations

- v1 opens with a hotkey instead of a placeable computer item.
- Controller-specific UI polish is not implemented yet.
- Chat history is current-session only and is not saved.
- Custom providers must use an OpenAI-compatible chat-completions response shape.
- AI advice can be wrong, especially with modded content. Treat answers as guidance, not guaranteed truth.

## More Help

- `docs/quickstart.md`: first-time player setup
- `docs/provider-setup.md`: provider-specific config
- `docs/privacy.md`: context sharing and data notes
- `docs/troubleshooting.md`: common provider errors
- `docs/uninstall.md`: save-safety and removal notes
- `docs/architecture.md`: developer architecture

## Building From Source

```sh
dotnet build
```

The SMAPI mod build package copies the build to the configured Stardew Valley `Mods/ComPewter` folder and creates a zip under `bin/Debug/net6.0/`.
>>>>>>> Stashed changes
>>>>>>> Stashed changes
