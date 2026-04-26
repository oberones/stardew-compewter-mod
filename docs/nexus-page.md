# ComPewter Nexus Mods Page Draft

## Short Description

An in-game farm computer chat assistant for Stardew Valley. Ask what to plant, where to fish, what gifts villagers like, how to plan your day, and more using your chosen AI provider.

## About

ComPewter adds a hotkey-opened in-game chat window that acts like a cozy Stardew Valley farm computer. It can answer practical gameplay questions about crops, fish, villagers, bundles, mines, crafting, cooking, money, relationships, rainy days, end-of-season planning, and current-save decisions.

Supported providers:

- Anthropic
- OpenAI
- Ollama
- Custom OpenAI-compatible endpoint
- Disabled setup mode

## Important: Network And Privacy Notice

ComPewter's core feature is sending chat requests to the AI provider selected in `config.json`. If you choose Anthropic, OpenAI, or a custom remote endpoint, your questions and enabled game context are sent to that provider. If you choose Ollama with the default local URL, requests are sent to your local Ollama server.

Game context sharing is enabled by default because current-save guidance is the main reason to use ComPewter. At the start of a new chat, ComPewter shows a notice explaining this. To disable automatic game context sharing:

```json
{
  "Privacy": {
    "ShareGameContext": false
  }
}
```

When enabled, context may include season, day, year, weather, daily luck, money, current location, farm type, skill levels, inventory summary, active quest titles, major progression flags, and spoiler preference.

By default, ComPewter does not intentionally send player names, farm names, save names, machine paths, multiplayer player names, provider secrets, friendship data, or installed mod lists. Friendship data and installed mod lists are separate opt-in settings.

API keys and tokens are read from local config and are redacted from SMAPI logs.

## Requirements

- Stardew Valley 1.6+
- SMAPI 4.0+
- One configured AI provider:
  - Anthropic API key
  - OpenAI API key
  - local Ollama server and model
  - or a custom OpenAI-compatible endpoint

## Installation

1. Install SMAPI.
2. Unzip ComPewter into your Stardew Valley `Mods` folder.
3. Launch Stardew Valley through SMAPI once, then quit.
4. Open `Mods/ComPewter/config.json`.
5. Set `Provider` to `Anthropic`, `OpenAI`, `Ollama`, or `Custom`.
6. Fill in the matching provider settings.
7. Load a save and press `F8`.

If `Provider` remains `Disabled`, ComPewter opens normally but shows setup guidance instead of contacting a provider.

## Example Config

Ollama:

```json
{
  "Provider": "Ollama",
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "Model": "llama3.1"
  }
}
```

OpenAI:

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

Anthropic:

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

## Current Limitations

- v1 opens with `F8`; it does not add a placeable computer item yet.
- Controller-specific UI polish is not implemented yet.
- Chat history is current-session only and is not saved.
- Custom providers must use an OpenAI-compatible chat-completions response shape.
- AI advice can be wrong, especially with modded content. Treat answers as guidance, not guaranteed truth.

## Save Safety

ComPewter v1 does not add custom objects, furniture, recipes, mail flags, persisted chat history, or world mutations. Removing the mod should not make a save unplayable.

## Recommended Tags

- SMAPI
- Utilities for Players
- Quality of Life
- Gameplay
- English
- Version 1.6 Compatible

## Suggested Images

- Chat window opened with `F8`.
- First-chat context sharing notice.
- Provider setup/error message.
- Successful gameplay answer.

## Upload Checklist

- Confirm `manifest.json` version matches the zip version.
- Add Nexus mod ID to `UpdateKeys` after the Nexus page exists.
- Upload the generated `ComPewter 0.3.0.zip`.
- Mark SMAPI as a requirement.
- Include the network/privacy notice near the top of the description.
- Contact Nexus Mods support if requested due to intentional provider network access.
