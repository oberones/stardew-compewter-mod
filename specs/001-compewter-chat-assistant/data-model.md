# Data Model: ComPewter Chat Assistant

## ModConfig

Purpose: Player-editable configuration controlling provider, privacy, UI, and
request behavior.

Fields:
- `OpenMenuKey`: hotkey, default `F8`.
- `Provider`: `Disabled`, `Anthropic`, `OpenAI`, `Ollama`, or `Custom`.
- `RequestTimeoutSeconds`: integer, clamped 5-120, default 30.
- `MaxResponseTokens`: integer, clamped 100-4000, default 700.
- `ShareGameContext`: boolean, default true.
- `AllowSpoilers`: boolean, default false.
- `RetainConversationHistory`: boolean, controls current-session history sent
  to provider, default true.
- `MaxRetainedMessages`: integer, clamped 0-100, default 20.
- `DebugLogging`: boolean, default false.
- `UiScale`: decimal, bounded to readable range, default 1.0.
- `ShowContextEnableHint`: boolean, default true.
- `Anthropic`, `OpenAI`, `Ollama`, `Custom`: provider settings.

Validation:
- invalid provider falls back to `Disabled`;
- invalid numeric values are clamped;
- provider selected with missing required values produces setup error, not crash.

## ProviderSettings

Purpose: Provider-specific connection and model configuration.

Common fields:
- `BaseUrl`
- `Model`
- `MaxResponseTokens`
- `TimeoutSeconds`

Secret-bearing fields:
- `ApiKey`
- `AuthToken`
- `AuthHeaderName`

Provider-specific requirements:
- Anthropic: API key and model required.
- OpenAI: API key and model required.
- Ollama: base URL and model required; base URL defaults local.
- Custom: base URL required; model and auth depend on request format.

Validation:
- secrets are never logged;
- empty required fields return `ProviderValidationResult` with setup guidance.

## ChatSession

Purpose: Current in-memory conversation and request state.

Fields:
- `Messages`: bounded list of `ChatMessage`.
- `IsRequestInFlight`: boolean.
- `CurrentRequestStartedAt`: nullable timestamp.
- `CurrentCancellation`: cancellation handle for pending request.
- `LastError`: nullable normalized error.

State transitions:
- Idle -> Submitting when valid question accepted.
- Submitting -> Idle when response or error is recorded.
- Submitting -> Cancelled when menu closes and cancellation succeeds.
- Any state -> Cleared when session is reset or game restarts.

Rules:
- one in-flight request per session;
- no save persistence in first version;
- message count never exceeds configured max.

## ChatMessage

Purpose: A visible or provider-bound message.

Fields:
- `Role`: `System`, `User`, `Assistant`, or `Status`.
- `Content`: display/provider text.
- `Status`: `Complete`, `Loading`, or `Error`.
- `CreatedOrder`: monotonic order within session.

Validation:
- empty user questions are not submitted;
- error messages must be player-safe and secret-free.

## AiChatRequest

Purpose: Provider-neutral request produced by prompt construction.

Fields:
- `Model`
- `Messages`
- `MaxResponseTokens`
- `Timeout`
- `ProviderType`

Rules:
- contains no secrets;
- includes context only when enabled;
- includes current-session history only when enabled.

## AiChatResult

Purpose: Normalized provider response.

Fields:
- `Success`
- `Content`
- `Error`
- `Duration`

Rules:
- exactly one of `Content` or `Error` is present;
- malformed provider responses become `MalformedResponse` errors.

## AiProviderError

Purpose: Provider-neutral error for UI and diagnostics.

Fields:
- `Kind`: `MissingConfiguration`, `Authentication`, `RateLimited`, `Timeout`,
  `Unavailable`, `MalformedResponse`, `UnsupportedSettings`, `Unknown`.
- `PlayerMessage`
- `DiagnosticMessage`
- `ProviderType`

Rules:
- `PlayerMessage` is polite and actionable;
- `DiagnosticMessage` is secret-redacted.

## GameContextSnapshot

Purpose: Privacy-filtered game state captured on demand.

Fields:
- safe categories: date, season, year, day of week, weather, tomorrow weather,
  daily luck, money, farm type, current location, skill levels, inventory
  summary, active quests, spoiler preference.
- opt-in categories: friendship summary, installed mods list, multiplayer data.
- unavailable categories with optional reason.

Rules:
- collected only on submit;
- no player/farm/save names by default;
- compact summaries only.

## PromptEnvelope

Purpose: Intermediate prompt structure before provider conversion.

Fields:
- `SystemInstructions`
- `PrivacyAndSpoilerSettings`
- `GameContextSection`
- `ConversationHistory`
- `CurrentQuestion`

Rules:
- sections remain distinct for review and testing;
- final provider conversion happens after envelope creation.

## PrivacyPreferences

Purpose: Effective privacy settings after config validation.

Fields:
- `ShareGameContext`
- `AllowSpoilers`
- `IncludeFriendshipData`
- `IncludeInstalledMods`
- `IncludeMultiplayerData`
- `RetainConversationHistory`

Rules:
- context sharing defaults true with an in-chat disable notice;
- sensitive categories default false;
- provider-bound output excludes disallowed categories.
