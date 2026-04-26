# ComPewter

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

## Requirements

- Stardew Valley 1.6+
- SMAPI 4.0+
- An AI provider account/key, a local Ollama server, or a compatible custom endpoint

## Quick Setup

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

```json
{
  "Privacy": {
    "ShareGameContext": false
  }
}
```

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
- `docs/nexus-page.md`: draft Nexus Mods page text

## Building From Source

```sh
dotnet build
```

The SMAPI mod build package copies the build to the configured Stardew Valley `Mods/ComPewter` folder and creates a zip under `bin/Debug/net6.0/`.
