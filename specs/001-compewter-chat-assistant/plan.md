# Implementation Plan: ComPewter Chat Assistant

**Branch**: `001-compewter-chat-assistant` | **Date**: 2026-04-26 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-compewter-chat-assistant/spec.md`

## Summary

Build ComPewter as a C# SMAPI mod that opens a custom in-game chat UI through a
configurable hotkey. The player chooses an AI provider from Disabled,
Anthropic, OpenAI, Ollama, or Custom. Provider-specific transport is hidden
behind a common chat-completion interface. The mod builds compact prompts from
system instructions, optional current-session history, optional privacy-filtered
game context, and the current player question. Provider requests run
asynchronously with timeout/error normalization so the game remains responsive.

The first version deliberately avoids save-affecting computer objects and
persisted chat history. This keeps add/remove behavior safe for existing saves
while the chat, provider, context, and privacy foundations are proven.

## Technical Context

**Language/Version**: C# targeting `net6.0` for Stardew Valley 1.6 / SMAPI 4.x  
**Primary Dependencies**: SMAPI, Stardew Valley / MonoGame assemblies via
`Pathoschild.Stardew.ModBuildConfig`, `System.Net.Http`, `System.Text.Json`  
**Storage**: SMAPI config file only for v1; no save-data chat persistence  
**Testing**: `dotnet build`, focused unit tests where practical, manual SMAPI
in-game acceptance matrix  
**Target Platform**: Stardew Valley 1.6+ on SMAPI-supported desktop platforms  
**Project Type**: SMAPI game mod  
**Performance Goals**: No network work per tick; provider calls do not freeze
gameplay; one in-flight request per chat session; repeated submits create zero
extra provider requests while pending  
**Constraints**: Provider default Disabled; context sharing default Off; no
secrets in logs or save data; hotkey-only v1 access; current-session history
only; no AI-driven world mutation  
**Scale/Scope**: Single local player UI session at a time; multiplayer-safe
local/private behavior; initial provider set is Anthropic, OpenAI, Ollama,
Custom, and Disabled

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Vanilla-Friendly UX**: PASS. Hotkey access is temporary but non-intrusive;
  UI copy and prompt tone remain cozy, practical, spoiler-aware, and concise.
- **Save Safety**: PASS. v1 avoids placeable objects, mail flags, recipes,
  furniture, and persisted chat history. Removing the mod leaves no required
  save dependency.
- **Provider Agnosticism**: PASS. Provider behavior is isolated behind a common
  interface; UI and gameplay code consume normalized request/result models.
- **Privacy**: PASS. Provider default Disabled, context sharing default Off,
  sensitive categories opt-in, and logs redact secrets and prompt bodies.
- **Gameplay Knowledge**: PASS. Prompt builder prefers local context when
  enabled, instructs uncertainty, and frames AI answers as guidance.
- **Local Context**: PASS. Context is collected on demand when submitting a
  question, through modular collectors with unavailable-data handling.
- **Performance**: PASS. Async request flow, timeout, cancellation-on-close,
  single in-flight request, and no per-tick network work are planned.
- **Architecture**: PASS. `ModEntry` remains a thin composition/event layer;
  systems are split by config, providers, prompts, context, sessions, UI,
  persistence, and diagnostics.
- **Configurability**: PASS. Config schema covers provider, provider settings,
  timeouts, response length, hotkey, context, spoiler, history, UI, and debug
  logging with safe defaults and validation.
- **Multiplayer**: PASS. Chat is local/private by default; no other-player
  context is sent; no world mutation depends on AI responses.
- **Content/Assets**: PASS. v1 has no custom save-affecting assets. Placeholder
  UI styling is original and data/content additions are deferred.
- **Testing/Acceptance**: PASS. Plan includes new/existing saves, disabled
  provider, provider failures, timeouts, malformed responses, mod removal,
  multiplayer smoke, config migration, UI scale, and normal in-game use.
- **Documentation**: PASS. Plan includes setup, privacy, troubleshooting,
  uninstall, limitations, and developer architecture docs.

## Architecture

`ModEntry`
: Thin entry point. Reads and validates config, creates the composition root,
  registers SMAPI events and console command if retained, opens the chat menu on
  hotkey, and delegates all work to services.

`Config/`
: Strongly typed `ModConfig`, provider settings, privacy settings, UI settings,
  validation, migration/defaulting, and optional Generic Mod Config Menu adapter.

`Providers/`
: Common provider interface plus concrete provider clients for Disabled,
  Anthropic, OpenAI, Ollama, and Custom. Provider-specific JSON and HTTP details
  stay here.

`Prompts/`
: Builds prompt/message payloads from system instructions, spoiler/privacy
  settings, game context, current-session history, and the player question.

`Context/`
: On-demand Stardew context collectors. Each collector is small, optional, and
  returns safe, compact facts or an unavailable marker.

`Sessions/`
: Chat session manager, message list, one-in-flight request state, cancellation,
  bounded current-session history, and public methods used by UI.

`UI/`
: Custom `IClickableMenu` chat UI, message list rendering, input handling,
  scrolling, loading/error states, and close behavior.

`Interaction/`
: v1 hotkey access. Future extension point for computer object, furniture, shop,
  mail, or crafting integration.

`Persistence/`
: Config is separate from save data. v1 save-data persistence is intentionally
  empty/no-op except future migration scaffolding if needed.

`Diagnostics/`
: Secret-safe logging, redaction helpers, normalized error-to-message mapping,
  request timing, and debug logging gates.

Communication flow:
1. `ModEntry` receives hotkey and opens `ChatMenu`.
2. `ChatMenu` calls `ChatSessionManager.SubmitAsync(question)`.
3. `ChatSessionManager` rejects submit if a request is already in flight.
4. Manager requests `GameContextSnapshot` only if config permits context.
5. Manager asks `PromptBuilder` for provider-neutral chat messages.
6. Manager sends messages to selected `IAiChatProvider`.
7. Provider returns `AiChatResult`; manager updates session messages.
8. UI redraws loading, response, or friendly error state.

## Provider Abstraction

Major interfaces/classes:

```csharp
public interface IAiChatProvider
{
    AiProviderType Type { get; }
    ProviderValidationResult Validate(ProviderSettings settings);
    Task<AiChatResult> SendAsync(AiChatRequest request, CancellationToken cancellationToken);
}

public sealed record AiChatRequest(
    string Model,
    IReadOnlyList<ChatMessage> Messages,
    int MaxResponseTokens,
    TimeSpan Timeout);

public sealed record AiChatResult(
    bool Success,
    string? Content,
    AiProviderError? Error,
    TimeSpan Duration);
```

Interface responsibilities:
- availability validation before sending;
- provider-neutral request inputs;
- async sending with cancellation where practical;
- timeout handling either through `HttpClient.Timeout` or linked cancellation;
- response parsing into normalized success/error;
- model configuration through settings;
- no provider-specific exceptions leaking into UI/session code.

Provider selection is performed by `AiProviderFactory`, which returns
`DisabledProvider` by default. UI and gameplay code only see
`AiChatResult`/`AiProviderError`.

## Provider Implementations

### Anthropic

- Config: `ApiKey`, `BaseUrl`, `Model`, `MaxResponseTokens`, `TimeoutSeconds`.
- Authentication: `x-api-key` header and Anthropic version header; redact all
  auth headers.
- Base URL: default to Anthropic Messages API base; override allowed for
  compatible proxies.
- Model: required when Anthropic selected; validation error if empty.
- Timeout: linked cancellation with configured timeout.
- Common errors: invalid key, rate limit, quota, model not found, timeout,
  malformed response.
- Parsing: read first text content block from the response; empty content maps
  to malformed response.
- Logging: status code, provider type, duration, normalized error only.

### OpenAI

- Config: `ApiKey`, `BaseUrl`, `Model`, `MaxResponseTokens`, `TimeoutSeconds`.
- Authentication: bearer token; redact.
- Base URL: default to OpenAI-compatible chat endpoint base; override allowed.
- Model: required when OpenAI selected.
- Timeout: linked cancellation.
- Common errors: invalid key, rate limit, quota, model not found, timeout,
  malformed response.
- Parsing: read assistant message text from chat-style response. If Responses
  API is chosen later, adapter remains inside provider.
- Logging: never log prompt, key, or response body in normal mode.

### Ollama

- Config: `BaseUrl`, `Model`, `MaxResponseTokens`, `TimeoutSeconds`.
- Authentication: none by default; optional token/header only if shared custom
  settings later require it.
- Base URL: default `http://localhost:11434`; override allowed.
- Model: required when Ollama selected.
- Timeout: linked cancellation; unavailable local server maps to friendly
  setup error.
- Common errors: connection refused, model not pulled, timeout, malformed JSON.
- Parsing: read non-streaming chat response message content. Streaming is future
  work unless specifically added.
- Logging: provider type, duration, normalized local server error.

### Custom

- Config: `BaseUrl`, `EndpointPath`, `Model`, `AuthHeaderName`,
  `AuthToken`, `RequestFormat`, `MaxResponseTokens`, `TimeoutSeconds`.
- Authentication: optional configurable header/token; redact both name/value
  when name suggests auth or token.
- Base URL: required when Custom selected.
- Model: optional only if selected request format allows it; otherwise required.
- Timeout: linked cancellation.
- Common errors: unsupported format, missing URL, 401/403, timeout, malformed
  response, incompatible schema.
- Parsing: first version supports OpenAI-compatible chat response shape; other
  request formats are documented as future extensibility unless implemented.
- Logging: never log custom request body by default because it may contain
  player context.

## Configuration Plan

Default values:

```json
{
  "OpenMenuKey": "F8",
  "Provider": "Disabled",
  "RequestTimeoutSeconds": 30,
  "MaxResponseTokens": 700,
  "ShareGameContext": false,
  "AllowSpoilers": false,
  "RetainConversationHistory": true,
  "MaxRetainedMessages": 20,
  "DebugLogging": false,
  "UiScale": 1.0,
  "ShowContextEnableHint": true,
  "Anthropic": {
    "ApiKey": "",
    "BaseUrl": "https://api.anthropic.com",
    "Model": ""
  },
  "OpenAI": {
    "ApiKey": "",
    "BaseUrl": "https://api.openai.com",
    "Model": ""
  },
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "Model": ""
  },
  "Custom": {
    "BaseUrl": "",
    "EndpointPath": "",
    "Model": "",
    "AuthHeaderName": "",
    "AuthToken": "",
    "RequestFormat": "OpenAICompatible"
  }
}
```

Validation rules:
- invalid provider -> Disabled with warning;
- timeout outside 5-120 seconds -> clamp to 30;
- max response tokens outside 100-4000 -> clamp to 700;
- max retained messages outside 0-100 -> clamp to 20;
- if `RetainConversationHistory` is false, only current question is sent;
- v1 history is in-memory only regardless of retention setting;
- provider selected but missing required model/key/url -> no request, friendly
  setup message;
- context sharing default Off; enabling must be easy to discover in docs/UI;
- debug logging default Off and still secret-redacted.

Optional Generic Mod Config Menu:
- adapter lives in `Config/GenericModConfigMenuIntegration.cs`;
- failure to load GMCM is non-fatal;
- expose provider, hotkey, context sharing, spoiler mode, timeout, retained
  messages, and debug logging first.

## Game Context Collection

Context is collected only on question submit, never per tick. `GameContextService`
aggregates small `IGameContextCollector` implementations and produces
`GameContextSnapshot`.

Safe-by-default context, available when context sharing is enabled:
- date, season, year, day of week;
- weather and tomorrow weather when available;
- daily luck;
- current money;
- farm type;
- current location type/name if not personally identifying;
- player skill levels;
- compact inventory summary: item display names and counts, no player/farm name;
- active quest titles/objectives if available;
- spoiler setting.

Opt-in/sensitive context:
- friendship summary;
- installed mods list;
- multiplayer player data;
- any player/farm/save names, default excluded even when context sharing is on.

Compactness:
- include summaries, not full inventories or full save data;
- cap list sizes, e.g. top inventory stacks and active quests;
- omit empty categories;
- include "unavailable" only when it materially prevents bad claims.

Unavailable context:
- collectors return `ContextItem.Unavailable(reason)` rather than throwing;
- prompt builder omits unavailable categories unless the absence is useful;
- diagnostics log collector failures at debug level only unless severe.

## Prompt Construction

Prompt sections are provider-neutral:

1. System instructions:
   - "You are ComPewter, a cozy, concise Stardew Valley gameplay helper."
   - give practical advice, avoid over-optimizing fun away;
   - avoid spoilers unless allowed or explicitly requested;
   - admit uncertainty and recommend checking in-game sources when unsure;
   - do not claim modded-content knowledge unless context provides it.
2. Privacy/spoiler settings:
   - context sharing enabled/disabled;
   - spoilers allowed/disallowed;
   - sensitive categories included/excluded.
3. Current game context:
   - compact labeled bullet sections generated from `GameContextSnapshot`.
4. Optional current-session history:
   - bounded to config limit and excluded when disabled.
5. Player question:
   - current input as the final user message.

Example shape, not final hardcoded wording:

```text
System: You are ComPewter...
Developer/Context: Spoilers allowed: false. Use current state when provided.
Game Context: Spring 12, Year 1; raining; luck neutral; money 1200g...
Conversation History: ...
User: What should I do today?
```

## Chat UI Plan

- Opened with configurable hotkey, default `F8`.
- Custom menu centered with readable Stardew-style text box.
- Message list displays role, content, loading/error status.
- Text input supports keyboard and mouse; controller is future improvement.
- Submit creates one in-flight request; submit is disabled/prevented until
  response or error.
- Loading state appears inline as "ComPewter is thinking..." or equivalent.
- Errors render as assistant messages with actionable, friendly wording.
- Scrolling is supported once messages exceed visible area.
- Close behavior cancels or detaches pending request safely; closed menu must
  not crash when provider returns later.
- UI scale uses stable dimensions and text wrapping; test common UI scales.
- Game pause/menu rules follow standard menu behavior; no per-tick heavy work.

## In-Game Computer Integration

Options compared:
- Custom object: immersive but adds asset/content/save-removal risk.
- Furniture item: thematic but similar save/content risk.
- Shop item: vanilla-friendly but requires content integration.
- Crafting recipe: fun but touches recipes and progression flags.
- Mail reward: cozy but touches mail flags and unlock state.
- Hotkey-only fallback: lowest save risk, fastest to test, removable.

Recommended first implementation: configurable hotkey only. Placeable or
obtainable computer is deferred to a later feature specification once chat,
provider, context, and privacy behavior are stable.

Asset strategy:
- no redistributed Stardew assets;
- v1 UI uses original simple texture boxes/fonts already available through game
  UI APIs;
- placeholder custom pixel art may be added later as original assets.

Uninstall behavior:
- no custom objects, recipes, mail flags, or persisted chat history in v1;
- removing the mod leaves no required save references.

## Async and Performance Plan

- `ChatSessionManager.SubmitAsync` creates a linked cancellation token with
  timeout.
- Provider calls use `HttpClient.SendAsync` and `System.Text.Json`.
- UI updates session state before and after await; handoff back to game-facing
  state is guarded so closed menus do not receive unsafe callbacks.
- One in-flight request per chat session prevents accidental spam.
- No provider work runs from update ticks except lightweight UI polling/redraw.
- Request duration is measured and logged without prompt bodies or secrets.
- Timeout maps to normalized provider error and friendly UI message.
- Menu close requests cancellation where practical.

## Persistence Plan

Persisted in v1:
- SMAPI `config.json` only.

Not persisted in v1:
- chat history;
- provider request/response bodies;
- API keys in save data;
- computer placement or unlock state.

Schema:
- no save-data schema for first version.

Migration:
- config validation handles missing/invalid fields with safe defaults;
- save-data migration is unnecessary until a later persisted feature exists.

Missing/corrupt handling:
- corrupt config values fall back to safe defaults with warnings;
- current-session history can be cleared safely at any time.

## Privacy and Security Plan

- Provider default Disabled.
- Context sharing default Off, with obvious opt-in guidance.
- Never log API keys, tokens, auth headers, full prompts, full request bodies,
  or response bodies that may include player context.
- Redact likely secrets in all diagnostics.
- Do not send farm name, player name, save name, machine paths, or multiplayer
  names by default.
- Sensitive context categories require explicit opt-in.
- Debug logging remains secret-safe.
- Documentation lists every context category and whether it is default,
  opt-in, or excluded.

## Multiplayer Plan

- Chat UI and AI requests are local/private by default.
- No AI response performs world mutation.
- No other-player context is sent unless a future opt-in setting explicitly
  enables and documents it.
- Hotkey-only v1 avoids shared placed-object synchronization concerns.
- Multiplayer smoke test: client opens UI, asks disabled-provider question,
  verifies no crash/desync; host/client with provider configured verifies local
  privacy behavior.

## Testing Plan

Manual and automated matrix:

| Area | Cases |
|------|-------|
| Save safety | new save, existing save, save/reload, mod removal |
| Provider selection | Disabled, Anthropic, OpenAI, Ollama, Custom |
| Provider failures | invalid API key, unavailable Ollama, timeout, rate limit, malformed response |
| Privacy | context sharing enabled, context sharing disabled, sensitive categories off |
| Spoilers | spoilers enabled, spoilers disabled, explicit spoiler request |
| History | current-session history enabled, disabled, max message cap, restart clears history |
| UI | hotkey open, text input, loading, response, error, scrolling, close while pending |
| Performance | repeated submit while pending creates no extra request, no freeze |
| Multiplayer | host/client smoke, local private chat, no world mutation |
| Config | invalid provider, invalid timeout, invalid token limit, migration/defaults |
| UI scale | common UI scales and window sizes |

Build/test commands:
- `dotnet build`
- future unit test command once test project exists
- manual Stardew/SMAPI acceptance checklist in `quickstart.md`

## Milestones

1. Project skeleton alignment and namespaces.
2. Config schema, defaults, validation, and example config docs.
3. Provider interface, normalized result/error model, disabled provider.
4. Ollama provider first for local testing.
5. Prompt builder with spoiler/privacy guardrails.
6. Chat session manager with one in-flight request and cancellation.
7. Basic custom chat UI with hotkey open.
8. Modular game context collector with safe default categories.
9. OpenAI, Anthropic, and Custom providers.
10. Secret-safe diagnostics and friendly error mapping.
11. Optional GMCM integration layer.
12. Privacy, setup, troubleshooting, uninstall, and developer docs.
13. Acceptance test pass and release packaging.

## Technical Risks

- SMAPI UI complexity: keep v1 simple, readable, and manually tested at UI
  scales.
- Provider API differences: isolate schemas in providers and normalize errors.
- Game freezing: enforce async flow, timeout, and no per-tick network calls.
- Prompt hallucination: prefer local context, uncertainty instruction, and
  concise guidance wording.
- Privacy concerns: default Disabled/context Off; no prompt/secret logging.
- Multiplayer edge cases: hotkey-only local UI and no world mutation.
- Save safety: avoid custom objects and persisted history in v1.
- Compatibility with other mods: treat modded data as uncertain unless included
  through explicit context.
- Token/context growth: compact collectors and bounded history.

## Deliverables

- Filled implementation plan: `plan.md`
- Research decisions: `research.md`
- Data model: `data-model.md`
- Contracts:
  - `contracts/provider-interface.md`
  - `contracts/prompt-context-contract.md`
  - `contracts/chat-session-contract.md`
- Quickstart and acceptance guide: `quickstart.md`
- Updated agent context: `AGENTS.md`

## Project Structure

### Documentation (this feature)

```text
specs/001-compewter-chat-assistant/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── provider-interface.md
│   ├── prompt-context-contract.md
│   └── chat-session-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
Config/
├── ModConfig.cs
├── ProviderSettings.cs
├── PrivacySettings.cs
├── UiSettings.cs
├── ConfigValidator.cs
└── GenericModConfigMenuIntegration.cs

Providers/
├── IAiChatProvider.cs
├── AiProviderFactory.cs
├── AiChatRequest.cs
├── AiChatResult.cs
├── AiProviderError.cs
├── DisabledProvider.cs
├── AnthropicProvider.cs
├── OpenAiProvider.cs
├── OllamaProvider.cs
└── CustomProvider.cs

Prompts/
├── PromptBuilder.cs
└── PromptOptions.cs

Context/
├── GameContextService.cs
├── IGameContextCollector.cs
├── GameContextSnapshot.cs
├── DateWeatherContextCollector.cs
├── PlayerContextCollector.cs
├── InventoryContextCollector.cs
├── QuestContextCollector.cs
├── FriendshipContextCollector.cs
├── ProgressionContextCollector.cs
└── ModListContextCollector.cs

Sessions/
├── ChatSessionManager.cs
├── ChatSession.cs
├── ChatMessage.cs
└── ChatMessageStatus.cs

UI/
├── ChatMenu.cs
├── ChatTextInput.cs
└── ChatScrollState.cs

Interaction/
└── HotkeyComputerInteraction.cs

Persistence/
└── SaveDataService.cs

Diagnostics/
├── SecretRedactor.cs
├── ModLogger.cs
└── ErrorMessageMapper.cs

docs/
├── provider-setup.md
├── privacy.md
├── troubleshooting.md
├── uninstall.md
└── architecture.md

tests/
├── unit/
├── integration/
└── manual/
```

**Structure Decision**: Use a single SMAPI mod project with top-level folders
matching constitution systems. Existing files can be moved incrementally as
implementation begins; `ModEntry` should remain a thin composition root.

## Complexity Tracking

No constitution violations. No complexity exceptions required.
