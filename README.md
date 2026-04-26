# ComPewter

ComPewter is a Stardew Valley mod concept for an in-game computer assistant.
Players can open a computer-like chat interface and ask practical game
questions, such as what to plant, where to find a fish, when a villager is
available, or which gifts someone likes.

The long-term goal is to let players choose their AI provider:

- Anthropic
- OpenAI
- Ollama
- Custom OpenAI-compatible endpoint

## Current State

This repository contains the initial SMAPI mod scaffold:

- SMAPI manifest and C# project file
- Config model with provider, model, API key, endpoint, and open-menu key
- AI provider interface and factory
- Placeholder provider implementation
- Basic in-game chat menu opened with `F8`
- Console command fallback: `compewter <question>`

Live provider calls are intentionally stubbed for now. The scaffold is ready for provider-specific HTTP clients and Stardew data grounding.

## Requirements

- Stardew Valley 1.6+
- SMAPI 4.0+
- .NET SDK 6.0 or newer

## Build

From this folder:

```sh
dotnet build
```

The `Pathoschild.Stardew.ModBuildConfig` package handles Stardew/SMAPI references and build output conventions.

## Install Locally

After building, copy the built mod output into your Stardew Valley `Mods` folder, or configure your local build output using SMAPI mod build settings.

The mod creates/reads `config.json` with these settings. A safe template is included as `config.example.json`:

```json
{
  "OpenMenuKey": "F8",
  "Provider": "OpenAI",
  "Model": "",
  "ApiKey": "",
  "Endpoint": "",
  "SystemPrompt": "You are ComPewter, a helpful in-game Stardew Valley assistant..."
}
```

## Planned Provider Behavior

Provider implementations should convert the shared chat history into each provider's request format:

- `Anthropic`: use Messages API with a configured model and API key.
- `OpenAI`: use Chat Completions or Responses API with a configured model and API key.
- `Ollama`: use the local Ollama HTTP API, usually at `http://localhost:11434`.
- `Custom`: call a user-specified endpoint, preferably using an OpenAI-compatible schema.

API keys should remain local in `config.json` and should never be committed.

## Planned Game Knowledge

The assistant should eventually ground answers in Stardew Valley data instead of relying only on model memory. Useful sources include:

- Crop data and seasons
- Fish locations, times, weather, and seasons
- NPC gift tastes and schedules
- Bundles, crafting recipes, and quests
- Current save context, such as day, season, weather, location, inventory, and known villagers

## Development Notes

The current UI is a minimal text-based menu. Good next steps:

- Add real provider HTTP implementations
- Add request cancellation and timeouts
- Persist chat history per save
- Ground prompts with live game state
- Add Generic Mod Config Menu integration
- Improve the computer art, input handling, scrolling, and response wrapping

## Project Constitution

This project is governed by the ComPewter constitution in
`.specify/memory/constitution.md`. Feature work must prioritize save safety,
privacy, stability, vanilla-friendly player experience, and maintainable SMAPI
architecture.
