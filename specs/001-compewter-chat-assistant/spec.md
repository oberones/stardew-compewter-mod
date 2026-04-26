# Feature Specification: ComPewter Chat Assistant

**Feature Branch**: `001-compewter-chat-assistant`  
**Created**: 2026-04-26  
**Status**: Draft  
**Input**: User description: "Build a Stardew Valley SMAPI mod called ComPewter. The mod adds an in-game computer that players can interact with through a chat interface. The computer acts as a Stardew Valley gameplay assistant with configurable Anthropic, OpenAI, Ollama, and Custom AI providers, optional local game context, spoiler-aware answers, private chat behavior, safe persistence, clear errors, and player/developer documentation."

## Clarifications

### Session 2026-04-26

- Q: What first-version access path should ComPewter use for the in-game computer? → A: Config hotkey opens the ComPewter chat; placeable/obtainable computer is future work.
- Q: What provider should ComPewter use by default? → A: Default provider is Disabled; player must explicitly configure a provider.
- Q: What should the default game-context sharing behavior be? → A: Context sharing defaults On because context-aware answers are a core value of the mod, and each new chat must clearly explain how to disable it.
- Q: How should ComPewter retain chat history in the first version? → A: Current session only; no chat history persisted to save data.
- Q: How should ComPewter handle multiple submitted questions while a request is pending? → A: One in-flight request per chat session; disable submit until response or error.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Ask The Computer For Gameplay Help (Priority: P1)

A player presses the configured ComPewter hotkey, opens a chat interface, types
a Stardew Valley gameplay question, and receives a friendly, concise answer from
their configured assistant.

**Why this priority**: This is the core value of the mod. Without a usable
computer chat flow, provider configuration, context gathering, and documentation
do not deliver player value.

**Independent Test**: On a save with at least one provider configured, press the
configured ComPewter hotkey, ask "What should I plant today?", confirm the game remains
responsive while the request is pending, and confirm the answer appears in the
chat UI.

**Acceptance Scenarios**:

1. **Given** a player has configured a supported provider, **When** the player presses the configured ComPewter hotkey and submits a gameplay question, **Then** the chat UI shows the player's question, a loading state, and a friendly assistant response.
2. **Given** a provider request is in progress, **When** the player waits for the answer, **Then** the game remains responsive and the chat UI does not become permanently blocked.
3. **Given** a provider request is in progress, **When** the player tries to submit another question, **Then** the chat UI prevents the duplicate submission until the current request completes with a response or error.
4. **Given** the player asks about crops, fish, villagers, bundles, mines, money, or end-of-season planning, **When** the assistant responds, **Then** the answer is concise, practical, Stardew-focused, and framed as guidance rather than guaranteed truth.

---

### User Story 2 - Configure AI Provider And Privacy Preferences (Priority: P1)

A player chooses whether ComPewter is disabled or uses Anthropic, OpenAI, Ollama,
or a Custom provider, configures model and connection settings, and controls
whether game context, spoilers, and chat history are included.

**Why this priority**: Provider choice and privacy controls are required before
the assistant can be trusted or broadly usable.

**Independent Test**: Configure each provider option in turn, including Disabled,
and confirm the mod either sends a request through the selected provider or shows
a helpful setup message without crashing.

**Acceptance Scenarios**:

1. **Given** the provider is set to Disabled or required settings are missing, **When** the player asks a question, **Then** ComPewter displays a helpful setup message instead of failing.
2. **Given** game-context sharing is disabled in config, **When** the player asks a question, **Then** provider-bound content excludes local game context beyond the player's typed question and required assistant instructions.
3. **Given** conversation history retention is disabled, **When** the player asks multiple questions, **Then** only the current question and required instructions are considered for the provider request.
4. **Given** invalid config values are present, **When** the mod loads, **Then** safe defaults are used and the player receives clear guidance through logs or UI where appropriate.

---

### User Story 3 - Receive Context-Aware, Spoiler-Aware Guidance (Priority: P2)

A player enables local game context sharing so ComPewter can tailor answers using
current date, season, weather, money, skills, inventory, quests, progression,
and other allowed context while avoiding spoilers by default.

**Why this priority**: Context-aware guidance makes the assistant meaningfully
better than a generic chatbot, but the feature remains useful with provider-only
questions while context grows over time.

**Independent Test**: Ask the same planning question on saves or days with
different seasons or weather and confirm the assistant's guidance changes when
context sharing is enabled.

**Acceptance Scenarios**:

1. **Given** context sharing is enabled, **When** the player asks "What should I do today?", **Then** the assistant considers available current-state details such as season, weather, day, money, and inventory summary.
2. **Given** spoiler mode is disabled, **When** the player asks a broad progression question, **Then** the assistant avoids revealing spoiler-heavy details unless the player explicitly asks for them.
3. **Given** context sharing is enabled but a context category is unavailable, **When** the player asks a question that could use that category, **Then** the assistant still answers and does not imply unavailable data was known.
4. **Given** context sharing is enabled, **When** the player opens a new ComPewter chat, **Then** the mod explains that context is shared and tells the player how to disable it.

---

### User Story 4 - Handle Provider And Network Failures Gracefully (Priority: P2)

A player receives clear, polite feedback when provider setup is incomplete,
credentials are invalid, requests time out, a local provider is unavailable, or
the provider returns an unusable response.

**Why this priority**: External services fail often; player saves and game
sessions must remain stable.

**Independent Test**: Configure an unavailable local provider or invalid
credential, ask a question, and confirm the game does not crash and the chat UI
shows a useful error.

**Acceptance Scenarios**:

1. **Given** Ollama is selected but the local server is unavailable, **When** the player asks a question, **Then** the chat UI explains that the provider cannot be reached and suggests checking setup.
2. **Given** Anthropic or OpenAI credentials are invalid, **When** the player asks a question, **Then** the chat UI shows an authentication-related setup message without exposing secrets.
3. **Given** a provider times out, rate limits, or returns malformed data, **When** the request fails, **Then** ComPewter shows a polite failure message and records secret-safe diagnostic information.

---

### User Story 5 - Use ComPewter Safely Across Saves And Multiplayer (Priority: P3)

A player can add ComPewter to a new or existing save, use current-session chat
history privately in multiplayer, and remove the mod without making the save
unplayable.

**Why this priority**: Save safety and multiplayer privacy are non-negotiable,
but richer persistence and shared-world behavior can be limited in the first
version as long as the limits are clear.

**Independent Test**: Add the mod to an existing save, use the computer, save,
reload, optionally remove the mod, and confirm the save remains playable.

**Acceptance Scenarios**:

1. **Given** an existing save, **When** the player adds ComPewter and opens chat with the configured hotkey, **Then** the save remains playable and the chat interaction works.
2. **Given** ComPewter is removed after use, **When** the player loads the save, **Then** the save remains playable with no required AI-provider dependency.
3. **Given** the player is in multiplayer, **When** they use ComPewter, **Then** chat content is private to the local player by default and other players' information is not sent to providers unless explicitly enabled.

### Edge Cases

- No provider is configured, or the provider is set to Disabled.
- The selected provider has a missing, invalid, or expired API key or token.
- A provider reports rate limiting, times out, is unavailable, or returns malformed data.
- The local Ollama server is not running or uses a non-default base URL.
- Custom provider settings are incomplete or unsupported.
- The player disables automatic game-context sharing.
- The player disables conversation history retention.
- Spoiler mode is disabled, but the player asks for broad progression advice.
- Context data is unavailable, incomplete, or affected by other mods.
- Config values are missing, invalid, outdated, or require migration.
- Current-session chat history exceeds configured limits.
- The feature is used on both new saves and existing saves.
- The mod is removed after hotkey chat has been used during the current session.
- The player uses ComPewter in multiplayer, including joining another farm.
- The conversation exceeds the visible chat area.
- The player submits repeated requests quickly.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: ComPewter MUST provide a configurable hotkey that opens the computer chat interface.
- **FR-002**: Placeable, craftable, mailed, or shop-purchased computer access MUST be treated as future work unless a later feature specification explicitly adds it.
- **FR-003**: The chat interface MUST show the current conversation, accept typed player questions, and display assistant responses.
- **FR-004**: The chat interface MUST show clear loading, success, and error states for each submitted question.
- **FR-005**: The chat interface MUST allow only one in-flight provider request per chat session.
- **FR-006**: While a provider request is in flight, the chat interface MUST disable or otherwise prevent submitting another question until the current request completes with a response or error.
- **FR-007**: The chat interface MUST remain recoverable after errors and MUST NOT permanently block play.
- **FR-008**: Keyboard and mouse interaction MUST be supported for the first version.
- **FR-009**: The conversation display SHOULD support scrolling when messages exceed the visible area.
- **FR-010**: The mod MUST provide strongly typed configuration for provider selection, provider-specific settings, context sharing, spoiler behavior, conversation history, response length, request timeout, and open-chat controls where supported.
- **FR-011**: Provider selection MUST support Anthropic, OpenAI, Ollama, Custom, and Disabled.
- **FR-012**: The default provider MUST be Disabled so no provider request is made until the player explicitly configures one.
- **FR-013**: Invalid configuration values MUST fall back to safe defaults with clear warnings.
- **FR-014**: The mod MUST support a common chat-completion capability across all enabled providers.
- **FR-015**: Anthropic, OpenAI, Ollama, and Custom provider options MUST handle authentication needs, request setup, response interpretation, timeout, and user-facing errors.
- **FR-016**: Ollama MUST default to a local provider address while allowing the player to override that address.
- **FR-017**: Custom provider settings MUST allow configurable endpoint, model name, and authentication token or header where applicable.
- **FR-018**: Provider credentials, tokens, authentication headers, and sensitive request content MUST NOT be displayed or logged.
- **FR-019**: If no provider is configured or the provider is Disabled, ComPewter MUST show a helpful setup message instead of attempting a request.
- **FR-020**: The assistant's instructions MUST describe it as a friendly, concise, practical Stardew Valley gameplay assistant.
- **FR-021**: Assistant instructions MUST include spoiler-aware behavior that avoids spoilers by default unless spoilers are enabled or the player explicitly requests them.
- **FR-022**: Provider-bound content MUST clearly separate assistant instructions, optional game context, optional conversation history, and the player's current question.
- **FR-023**: Provider-bound content MUST remain reasonably compact by limiting context categories and retained messages according to configuration.
- **FR-024**: When context sharing is enabled, ComPewter MUST be able to include relevant local game state categories such as date, season, year, day of week, weather, tomorrow's weather when available, daily luck, money, farm type, current location, skills, inventory summary, active quests, allowed friendship data, community progress, progression flags, installed mods list when enabled, and spoiler preference.
- **FR-025**: When context sharing is disabled, ComPewter MUST exclude automatic local game context from provider-bound content.
- **FR-026**: Context sharing MUST default to On.
- **FR-027**: The mod MUST make context sharing easy to understand and disable through player-facing setup guidance or in-chat setup messaging.
- **FR-028**: The player MUST be able to separately control sensitive context categories, including friendship data, installed mod list, and multiplayer player data if those categories are ever supported.
- **FR-029**: Conversation history retention MUST be limited to the current game session in the first version.
- **FR-030**: Current-session conversation history MUST be bounded by a configurable maximum message count.
- **FR-031**: Persisted chat history MUST be treated as future work unless a later feature specification explicitly adds it.
- **FR-032**: Secrets MUST NOT be stored in save data.
- **FR-033**: ComPewter MUST handle missing provider configuration, invalid keys, rate limits, timeout, malformed responses, unsupported settings, and unavailable local providers without crashing the game.
- **FR-034**: Player-facing error messages MUST be polite, clear, and actionable without exposing secrets.
- **FR-035**: Diagnostic logs MUST provide enough detail for troubleshooting while excluding secrets and full sensitive prompts.
- **FR-036**: Chat interactions MUST be private to the local player by default in multiplayer.
- **FR-037**: ComPewter MUST NOT send farm name, player name, multiplayer player names, local file paths, or secrets to providers unless explicitly enabled and documented.
- **FR-038**: Computer interaction and AI responses MUST NOT perform world-changing actions based on assistant output.
- **FR-039**: The hotkey chat access and any persistent data MUST be safe to add to existing saves.
- **FR-040**: Removing the mod MUST NOT make a save unplayable.
- **FR-041**: The mod MUST include player-facing setup documentation for Anthropic, OpenAI, Ollama, and Custom providers.
- **FR-042**: The mod MUST document privacy behavior, context categories that may be sent, troubleshooting steps, uninstall notes, and known limitations.
- **FR-043**: Developer documentation MUST explain the provider abstraction, prompt construction, game context collection, chat session behavior, and UI architecture.

### Constitution-Aligned Requirements *(mandatory for ComPewter)*

- **Privacy Impact**: Provider-bound content may include the player's typed
  question, assistant instructions, enabled conversation history, and enabled
  local game context. Automatic context sharing MUST be configurable and may be
  fully disabled. Context sharing MUST default On and MUST be easy to understand
  and disable because context-aware answers are a primary value of ComPewter.
  Sensitive personal, machine, save, farm, player, and
  multiplayer identity data MUST be excluded by default.
- **Save-Safety Impact**: The hotkey chat access MUST be safe for new saves,
  existing saves, migration, and mod removal. Chat history is current-session
  only in the first version, and save-affecting computer objects, unlock methods,
  or persisted chat history are outside the first-version scope.
- **Provider Behavior**: Anthropic, OpenAI, Ollama, Custom, and Disabled MUST
  each have documented setup and expected behavior. Disabled and incomplete
  setups MUST show helpful setup messages.
- **Failure States**: Invalid credentials, missing config, provider timeout,
  rate limiting, malformed response, unavailable provider, unavailable Ollama,
  and unsupported custom settings MUST produce friendly UI errors and
  secret-safe diagnostics.
- **Multiplayer Behavior**: Chat is local/private by default. Other players'
  information MUST NOT be sent unless explicitly enabled. Shared computer
  behavior may be limited in the first version if documented.
- **Spoiler Behavior**: Spoilers are avoided by default. Spoiler-heavy answers
  are allowed only when spoiler mode is enabled or the player explicitly asks.
- **In-Game Acceptance**: Completion MUST be verified through normal in-game
  interaction by pressing the configured hotkey, submitting questions, observing response or
  error states, saving/reloading, and validating add/remove behavior.

### Key Entities *(include if feature involves data)*

- **Computer Access Point**: The configured hotkey that opens ComPewter's computer chat in the first version; placeable or obtainable objects are future work.
- **Chat Session**: The current local conversation for the active game session, including visible player messages, assistant messages, loading state, and error state.
- **Chat Message**: A single player or assistant message with role, content, timestamp/order, and optional status.
- **Provider Configuration**: Player-selected provider and related model, credential reference, base URL, timeout, response length, and custom settings.
- **Provider Request**: The provider-bound payload derived from assistant instructions, optional game context, optional history, and the current question.
- **Game Context Snapshot**: A privacy-filtered set of local game state categories collected when the player submits a question.
- **Privacy Preferences**: Configuration controlling context sharing, spoiler behavior, current-session conversation history limits, sensitive context categories, and multiplayer data sharing.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A player can open ComPewter with the configured hotkey and submit a question in under 30 seconds after entering a save.
- **SC-002**: With one supported provider configured, at least 95% of valid gameplay questions in a manual acceptance set produce either an assistant answer or a clear provider error without crashing.
- **SC-003**: During provider requests, the game remains responsive in every acceptance test run.
- **SC-003A**: While a request is pending, repeated submit attempts do not create additional provider requests in every acceptance test run.
- **SC-004**: No-provider, invalid-key, timeout, malformed-response, rate-limit, unavailable-provider, and unavailable-Ollama scenarios all show player-facing messages and do not crash the game.
- **SC-005**: With context sharing enabled, answers to the same daily-planning question differ appropriately across at least two different seasons or weather states.
- **SC-006**: With context sharing disabled, provider-bound content excludes automatic local game context in every privacy acceptance check.
- **SC-006A**: A tester can find how to disable context sharing from player-facing setup guidance or in-chat setup messaging in under 60 seconds.
- **SC-007**: With spoiler mode disabled, at least 9 of 10 broad progression prompts in the manual spoiler acceptance set avoid spoiler-heavy details unless the player's question explicitly asks for them.
- **SC-008**: Current-session conversation history never exceeds the configured retained message limit after repeated use and is not restored after restarting the game.
- **SC-009**: The mod can be added to an existing save, used, saved, reloaded, and removed without making the save unplayable.
- **SC-010**: Player-facing documentation enables a tester to configure Anthropic, OpenAI, Ollama, Custom, and Disabled modes without reading source code.

## Assumptions

- The first version uses a configurable hotkey as the access path and documents placeable or obtainable computer objects as future work.
- Shared multiplayer behavior may be limited to local/private chat interactions in the first version.
- Provider-specific account creation, billing, and model availability are handled outside the mod.
- The default provider is Disabled, context sharing defaults On with a prominent disable notice, and sensitive categories require explicit opt-in.
- Chat history is current-session only in the first version and is not persisted to save data.
- The assistant provides gameplay guidance only and does not automate world-changing decisions.
- Local game data and installed mods may change exact gameplay truth; answers should acknowledge uncertainty when relevant.
