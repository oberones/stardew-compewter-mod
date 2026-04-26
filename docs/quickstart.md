# ComPewter Quickstart

This guide is for players who already installed ComPewter and want to configure it for the first time.

## 1. Launch Once

Start Stardew Valley with SMAPI once, then quit the game. This lets ComPewter create its `config.json` file in the mod folder:

```text
Stardew Valley/Mods/ComPewter/config.json
```

If `config.json` does not exist yet, launch the game once with the mod installed.

## 2. Choose A Provider

Open `config.json` in a text editor and set `Provider` to one of:

- `Anthropic`
- `OpenAI`
- `Ollama`
- `Custom`
- `Disabled`

`Disabled` is the safe default. The chat window will still open, but ComPewter will show setup guidance instead of contacting an AI provider.

## 3. Configure Your Provider

Use one of these examples as a starting point.

### Ollama

Ollama runs locally on your computer, so it is a good first option if you already use it.

```json
{
  "Provider": "Ollama",
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "Model": "llama3.1"
  }
}
```

Make sure Ollama is running and the model is installed before asking a question.

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

Custom providers must use an OpenAI-compatible chat response format in v1.

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

## 4. Enable Game Context

ComPewter can answer much better Stardew questions when it knows your current season, weather, money, skills, inventory, quests, and progression. This is off by default for privacy.

To enable it:

```json
{
  "Privacy": {
    "ShareGameContext": true
  }
}
```

Optional context categories stay off unless you enable them:

```json
{
  "Privacy": {
    "IncludeFriendshipData": true,
    "IncludeInstalledMods": true
  }
}
```

ComPewter does not intentionally send player names, farm names, save names, machine paths, multiplayer player names, or provider secrets.

## 5. Open ComPewter In Game

Load a save and press:

```text
F8
```

Type a question and press Enter. Try:

- What should I plant today?
- What fish are available in this weather?
- What gifts does Abigail like?
- What should I do before the season ends?
- What should I bring to the mines?

Use Escape to close the chat window.

## Stardew-Focused Answers

ComPewter is intentionally focused on Stardew Valley. By default, it treats ambiguous questions through a Stardew lens, so questions about cooking, money, weather, fishing, crops, or relationships are answered as game questions unless you clearly ask otherwise.

For clearly unrelated questions, ComPewter is instructed to give a short redirect back toward Stardew help. To relax this behavior:

```json
{
  "RestrictToStardewTopics": false
}
```

## Common Settings

```json
{
  "OpenMenuKey": "F8",
  "RequestTimeoutSeconds": 30,
  "MaxResponseTokens": 700,
  "MaxRetainedMessages": 20,
  "RestrictToStardewTopics": true,
  "Privacy": {
    "AllowSpoilers": false,
    "RetainConversationHistory": true
  }
}
```

- `OpenMenuKey`: hotkey used to open ComPewter.
- `RequestTimeoutSeconds`: how long to wait for the provider.
- `MaxResponseTokens`: rough maximum answer length.
- `MaxRetainedMessages`: how much current-session chat history to keep.
- `RestrictToStardewTopics`: whether ComPewter should redirect clearly unrelated questions.
- `AllowSpoilers`: whether ComPewter may give spoiler-heavy answers.
- `RetainConversationHistory`: whether recent messages are included for follow-up questions.

## Troubleshooting

- If ComPewter says no provider is configured, check `Provider`, model, and key/token settings.
- If Ollama is unavailable, start Ollama and confirm the model is installed.
- If OpenAI or Anthropic reports an auth problem, check the API key.
- If answers ignore your current season or inventory, enable `Privacy.ShareGameContext`.
- If responses time out, increase `RequestTimeoutSeconds` or choose a faster model.

See `docs/troubleshooting.md` for more detail.
