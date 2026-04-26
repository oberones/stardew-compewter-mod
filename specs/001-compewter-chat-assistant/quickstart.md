# Quickstart: ComPewter Chat Assistant

## Build

```sh
dotnet build
```

## First Run

1. Install the built mod into the Stardew Valley `Mods/ComPewter` folder.
2. Launch Stardew Valley through SMAPI.
3. Load a new or existing save.
4. Press the configured ComPewter hotkey, default `F8`.
5. Confirm the chat UI opens.
6. With default config, ask a question and confirm ComPewter shows provider setup
   guidance because provider defaults to Disabled.

## Configure A Provider

Edit `config.json` after first launch or use Generic Mod Config Menu if enabled.

Provider defaults:
- `Disabled`: no provider calls; setup guidance only.
- `Ollama`: local-first testing, default base URL `http://localhost:11434`.
- `OpenAI`: requires API key and model.
- `Anthropic`: requires API key and model.
- `Custom`: requires compatible endpoint settings.

Never share `config.json` if it contains secrets.

## Enable Context Sharing

Context sharing defaults Off. To make answers use the current season, weather,
money, inventory summary, and other allowed state, enable `ShareGameContext`.

Sensitive categories such as friendship details, installed mods, and multiplayer
data remain opt-in.

## Manual Acceptance Checklist

- [ ] New save opens ComPewter with hotkey.
- [ ] Existing save opens ComPewter with hotkey.
- [ ] Disabled provider shows setup message.
- [ ] Ollama unavailable shows friendly error and no crash.
- [ ] Invalid OpenAI key shows friendly auth/setup error.
- [ ] Invalid Anthropic key shows friendly auth/setup error.
- [ ] Timeout shows friendly timeout error.
- [ ] Malformed provider response shows friendly error.
- [ ] Game remains responsive while request is pending.
- [ ] Repeated submit while pending creates no extra request.
- [ ] Context sharing Off excludes automatic game context.
- [ ] Context sharing On changes daily-planning answer across season/weather.
- [ ] Spoilers disabled avoids spoiler-heavy broad progression details.
- [ ] Current-session history respects max retained messages.
- [ ] Restarting the game clears chat history.
- [ ] Multiplayer smoke test keeps chat local/private.
- [ ] Removing the mod does not make the save unplayable.
- [ ] UI remains readable at common UI scales.

## Troubleshooting

- Provider Disabled: choose and configure a provider.
- Ollama unavailable: start Ollama, pull the configured model, and verify base
  URL.
- Invalid key: check API key/token and selected model.
- Timeout: increase timeout or check provider/network availability.
- Generic answers: enable context sharing if you want current-save guidance.

## Uninstall

For v1, ComPewter does not add save-required computer objects or persisted chat
history. Remove the mod folder and load the save normally. Keep a backup before
testing any mod removal, especially in modded saves.
