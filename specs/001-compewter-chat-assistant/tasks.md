# Tasks: ComPewter Chat Assistant

**Input**: Design documents from `/specs/001-compewter-chat-assistant/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: Include focused automated tests where practical and manual in-game verification tasks because the ComPewter constitution requires testable acceptance for save safety, privacy, provider failures, and normal gameplay interaction.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing. The MVP is User Story 1 after foundational infrastructure is complete.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel with other tasks marked [P] in the same phase when files do not overlap
- **[Story]**: Maps a task to a user story phase, e.g. [US1]
- Every task includes concrete file paths

## Path Conventions

- Production code lives in repository-root folders: `Config/`, `Providers/`, `Prompts/`, `Context/`, `Sessions/`, `UI/`, `Interaction/`, `Persistence/`, `Diagnostics/`
- Documentation lives in `docs/` and `specs/001-compewter-chat-assistant/`
- Tests live in `tests/unit/`, `tests/integration/`, and `tests/manual/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Align the current scaffold with the planned SMAPI mod structure and create shared folders/docs.

- [X] T001 Create planned source folders `Prompts/`, `Context/`, `Sessions/`, `Interaction/`, `Persistence/`, `Diagnostics/`, `docs/`, `tests/unit/`, `tests/integration/`, and `tests/manual/`
- [X] T002 Update project metadata and package settings in `ComPewter.csproj`
- [X] T003 Update SMAPI metadata to match ComPewter v1 scope in `manifest.json`
- [X] T004 Replace the scaffold config example with v1 defaults in `config.example.json`
- [X] T005 [P] Create provider setup documentation stub in `docs/provider-setup.md`
- [X] T006 [P] Create privacy documentation stub in `docs/privacy.md`
- [X] T007 [P] Create troubleshooting documentation stub in `docs/troubleshooting.md`
- [X] T008 [P] Create uninstall documentation stub in `docs/uninstall.md`
- [X] T009 [P] Create developer architecture documentation stub in `docs/architecture.md`
- [X] T010 [P] Create manual acceptance checklist from quickstart in `tests/manual/compewter-acceptance.md`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Build shared contracts, config, diagnostics, and composition pieces that all user stories depend on.

**CRITICAL**: No user story work can begin until this phase is complete.

- [X] T011 Replace provider enum with Disabled/Anthropic/OpenAI/Ollama/Custom values in `Config/AiProviderType.cs`
- [X] T012 Implement provider-specific config records in `Config/ProviderSettings.cs`
- [X] T013 Implement privacy config records in `Config/PrivacySettings.cs`
- [X] T014 Implement UI config records in `Config/UiSettings.cs`
- [X] T015 Expand strongly typed mod config with safe defaults in `Config/ModConfig.cs`
- [X] T016 Implement config validation, clamping, schema versioning, and migration/default repair in `Config/ConfigValidator.cs`
- [X] T017 [P] Implement secret redaction helpers in `Diagnostics/SecretRedactor.cs`
- [X] T018 [P] Implement secret-safe logging wrapper in `Diagnostics/ModLogger.cs`
- [X] T019 [P] Implement provider error to player-message mapper in `Diagnostics/ErrorMessageMapper.cs`
- [X] T020 Define provider-neutral request/result/error contracts in `Providers/AiChatRequest.cs`, `Providers/AiChatResult.cs`, and `Providers/AiProviderError.cs`
- [X] T021 Replace the scaffold provider interface with v1 contract in `Providers/IAiChatProvider.cs`
- [X] T022 Implement Disabled provider setup response in `Providers/DisabledProvider.cs`
- [X] T023 Implement provider factory selection/defaulting in `Providers/AiProviderFactory.cs`
- [X] T024 Define session message models in `Sessions/ChatMessage.cs` and `Sessions/ChatMessageStatus.cs`
- [X] T025 Define chat session state model in `Sessions/ChatSession.cs`
- [X] T026 Define no-op v1 save data service in `Persistence/SaveDataService.cs`
- [X] T027 Refactor `ModEntry.cs` into a thin composition root using config validation, provider factory, diagnostics, and hotkey interaction services
- [ ] T028 [P] Add unit tests for config validation defaults, clamping, and missing/outdated config migration in `tests/unit/ConfigValidatorTests.cs`
- [ ] T029 [P] Add unit tests for secret redaction in `tests/unit/SecretRedactorTests.cs`
- [ ] T030 [P] Add unit tests for disabled provider validation/result behavior in `tests/unit/DisabledProviderTests.cs`

**Checkpoint**: Foundation ready. Config defaults to Disabled, secrets are redacted, provider contracts exist, and `ModEntry` is thin.

---

## Phase 3: User Story 1 - Ask The Computer For Gameplay Help (Priority: P1) MVP

**Goal**: A player opens ComPewter with the configured hotkey, asks a question, sees loading, and receives an answer or friendly setup/error response without freezing the game.

**Independent Test**: On a save, press the configured hotkey, submit "What should I plant today?", verify the UI shows the question, loading state, and provider response or setup message while the game remains responsive.

### Tests for User Story 1

- [ ] T031 [P] [US1] Add chat session contract tests for empty-submit and one-in-flight behavior in `tests/unit/ChatSessionManagerTests.cs`
- [ ] T032 [P] [US1] Add provider interface contract tests for normalized success/error result handling in `tests/unit/AiChatProviderContractTests.cs`
- [X] T033 [P] [US1] Add manual UI hotkey/open/loading checklist in `tests/manual/us1-chat-ui.md`

### Implementation for User Story 1

- [X] T034 [US1] Implement current-session manager submit flow, in-flight guard, timeout cancellation, and message updates in `Sessions/ChatSessionManager.cs`
- [X] T035 [US1] Implement hotkey open behavior in `Interaction/HotkeyComputerInteraction.cs`
- [X] T036 [US1] Replace scaffold menu with v1 chat menu shell in `UI/ChatMenu.cs`
- [X] T037 [US1] Implement keyboard text input helper in `UI/ChatTextInput.cs`
- [X] T038 [US1] Implement scroll state for message list in `UI/ChatScrollState.cs`
- [X] T039 [US1] Wire chat menu to session manager loading/success/error states in `UI/ChatMenu.cs`
- [X] T040 [US1] Update `ModEntry.cs` to use `HotkeyComputerInteraction` and open `ChatMenu`
- [ ] T041 [US1] Verify disabled provider setup message through normal in-game interaction and record result in `tests/manual/us1-chat-ui.md`

**Checkpoint**: MVP opens via hotkey, accepts input, shows loading/error/response states, and prevents duplicate submits while pending.

---

## Phase 4: User Story 2 - Configure AI Provider And Privacy Preferences (Priority: P1)

**Goal**: A player can configure Disabled, Anthropic, OpenAI, Ollama, or Custom provider settings and privacy/history/spoiler options with safe defaults and validation.

**Independent Test**: Configure each provider option, including incomplete settings, and verify ComPewter either selects the provider or shows setup guidance without crashing or logging secrets.

### Tests for User Story 2

- [ ] T042 [P] [US2] Add provider validation tests for missing model/key/url cases in `tests/unit/ProviderValidationTests.cs`
- [ ] T043 [P] [US2] Add config privacy default tests for Disabled provider and context sharing On with disable notice in `tests/unit/PrivacyDefaultsTests.cs`
- [X] T044 [P] [US2] Add manual provider configuration checklist in `tests/manual/us2-provider-config.md`

### Implementation for User Story 2

- [X] T045 [US2] Implement Anthropic provider validation, request construction, and successful response parsing in `Providers/AnthropicProvider.cs`
- [X] T046 [US2] Implement OpenAI provider validation, request construction, and successful response parsing in `Providers/OpenAiProvider.cs`
- [X] T047 [US2] Implement Ollama provider validation, request construction, and successful response parsing in `Providers/OllamaProvider.cs`
- [X] T048 [US2] Implement Custom OpenAI-compatible validation, request construction, and successful response parsing in `Providers/CustomProvider.cs`
- [X] T049 [US2] Add shared JSON/HTTP helper behavior for providers in `Providers/ProviderHttpClient.cs`
- [X] T050 [US2] Update provider factory to instantiate Anthropic/OpenAI/Ollama/Custom providers in `Providers/AiProviderFactory.cs`
- [X] T051 [US2] Implement optional Generic Mod Config Menu adapter with non-fatal load behavior in `Config/GenericModConfigMenuIntegration.cs`
- [X] T052 [US2] Document Anthropic/OpenAI/Ollama/Custom/Disabled setup and example snippets in `docs/provider-setup.md`
- [ ] T053 [US2] Verify config scenarios, secret-safe logs, and a configured Ollama assistant answer manually in `tests/manual/us2-provider-config.md`

**Checkpoint**: Provider selection and validation are usable, provider details stay out of UI/session code, and defaults are privacy-safe.

---

## Phase 5: User Story 3 - Receive Context-Aware, Spoiler-Aware Guidance (Priority: P2)

**Goal**: A player can opt into context sharing and receive concise, spoiler-aware answers informed by allowed current game state.

**Independent Test**: Enable context sharing, ask a daily planning question across different season/weather states, and confirm prompt content and answer behavior change while excluded categories remain absent.

### Tests for User Story 3

- [ ] T054 [P] [US3] Add prompt contract tests for section separation, spoiler rules, and history inclusion in `tests/unit/PromptBuilderTests.cs`
- [ ] T055 [P] [US3] Add context filtering tests for ShareGameContext Off and sensitive categories Off in `tests/unit/GameContextPrivacyTests.cs`
- [X] T056 [P] [US3] Add 10-prompt manual context/spoiler acceptance checklist in `tests/manual/us3-context-spoilers.md`

### Implementation for User Story 3

- [X] T057 [US3] Implement prompt options and envelope model in `Prompts/PromptOptions.cs`
- [X] T058 [US3] Implement prompt builder with system/privacy/context/history/question sections in `Prompts/PromptBuilder.cs`
- [X] T059 [US3] Implement context snapshot and collector contract in `Context/GameContextSnapshot.cs` and `Context/IGameContextCollector.cs`
- [X] T060 [US3] Implement aggregate context service with on-demand collection in `Context/GameContextService.cs`
- [X] T061 [P] [US3] Implement date/weather/luck collector in `Context/DateWeatherContextCollector.cs`
- [X] T062 [P] [US3] Implement player money/skills/location/farm type collector in `Context/PlayerContextCollector.cs`
- [X] T063 [P] [US3] Implement compact inventory collector in `Context/InventoryContextCollector.cs`
- [X] T064 [P] [US3] Implement active quests collector in `Context/QuestContextCollector.cs`
- [X] T065 [P] [US3] Implement opt-in friendship collector in `Context/FriendshipContextCollector.cs`
- [X] T066 [P] [US3] Implement progression/bundle collector where practical in `Context/ProgressionContextCollector.cs`
- [X] T067 [P] [US3] Implement opt-in installed mods collector in `Context/ModListContextCollector.cs`
- [X] T068 [US3] Wire context service and prompt builder into `Sessions/ChatSessionManager.cs`
- [X] T069 [US3] Add in-chat context enable hint when context is Off in `UI/ChatMenu.cs`
- [X] T070 [US3] Document context categories and spoiler behavior in `docs/privacy.md`
- [ ] T071 [US3] Verify context On/Off and spoiler behavior manually in `tests/manual/us3-context-spoilers.md`

**Checkpoint**: Context sharing is opt-in, easy to discover, compact, spoiler-aware, and never collected per tick.

---

## Phase 6: User Story 4 - Handle Provider And Network Failures Gracefully (Priority: P2)

**Goal**: Provider failures are normalized into friendly UI errors and useful secret-safe diagnostics without crashing the game.

**Independent Test**: Configure unavailable Ollama, invalid OpenAI/Anthropic credentials, timeout, malformed response, and unsupported Custom settings; verify friendly errors and secret-safe logs.

### Tests for User Story 4

- [ ] T072 [P] [US4] Add provider error normalization tests in `tests/unit/ProviderErrorMappingTests.cs`
- [ ] T073 [P] [US4] Add timeout and cancellation tests for chat session manager in `tests/unit/ChatSessionTimeoutTests.cs`
- [X] T074 [P] [US4] Add manual provider failure checklist in `tests/manual/us4-provider-failures.md`

### Implementation for User Story 4

- [X] T075 [US4] Implement HTTP status and exception mapping for Anthropic in `Providers/AnthropicProvider.cs`
- [X] T076 [US4] Implement HTTP status and exception mapping for OpenAI in `Providers/OpenAiProvider.cs`
- [X] T077 [US4] Implement unavailable server/model and malformed response mapping for Ollama in `Providers/OllamaProvider.cs`
- [X] T078 [US4] Implement unsupported settings and schema mismatch mapping for Custom in `Providers/CustomProvider.cs`
- [X] T079 [US4] Ensure all provider request logs use redacted diagnostics in `Diagnostics/ModLogger.cs`
- [X] T080 [US4] Render retry-safe friendly errors in `UI/ChatMenu.cs`
- [X] T081 [US4] Document provider error troubleshooting in `docs/troubleshooting.md`
- [ ] T082 [US4] Verify provider failure scenarios manually in `tests/manual/us4-provider-failures.md`

**Checkpoint**: Missing config, invalid keys, rate limits, timeouts, malformed responses, and unavailable providers never crash the game or leak secrets.

---

## Phase 7: User Story 5 - Use ComPewter Safely Across Saves And Multiplayer (Priority: P3)

**Goal**: ComPewter remains safe on new/existing saves, uses current-session history only, keeps multiplayer chat private, and can be removed without making saves unplayable.

**Independent Test**: Add the mod to an existing save, use the hotkey chat, save/reload, test multiplayer smoke behavior, remove the mod, and confirm the save remains playable.

### Tests for User Story 5

- [ ] T083 [P] [US5] Add save-data no-op behavior tests in `tests/unit/SaveDataServiceTests.cs`
- [X] T084 [P] [US5] Add current-session history cap/restart checklist in `tests/manual/us5-save-multiplayer.md`
- [X] T085 [P] [US5] Add multiplayer smoke checklist in `tests/manual/us5-save-multiplayer.md`

### Implementation for User Story 5

- [X] T086 [US5] Ensure `Persistence/SaveDataService.cs` does not write chat history or secrets to save data in v1
- [X] T087 [US5] Enforce current-session message cap in `Sessions/ChatSessionManager.cs`
- [X] T088 [US5] Ensure hotkey/chat behavior is local-only and performs no world mutation in `Interaction/HotkeyComputerInteraction.cs`
- [X] T089 [US5] Guard context collectors from sending multiplayer player data unless explicitly enabled in `Context/GameContextService.cs`
- [X] T090 [US5] Document multiplayer limitations in `docs/privacy.md`
- [X] T091 [US5] Document uninstall behavior and save-safety notes in `docs/uninstall.md`
- [ ] T092 [US5] Verify new save, existing save, save/reload, mod removal, and multiplayer smoke manually in `tests/manual/us5-save-multiplayer.md`

**Checkpoint**: v1 has no save-required custom content or persisted chat history, and multiplayer behavior is local/private by default.

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Final documentation, packaging, acceptance testing, and release-readiness checks.

- [X] T093 [P] Update README with v1 hotkey-only scope, provider setup links, privacy summary, and known limitations in `README.md`
- [X] T094 [P] Complete developer architecture documentation in `docs/architecture.md`
- [X] T095 [P] Add release readiness checklist in `tests/manual/release-readiness.md`
- [X] T096 Run `dotnet build` and record result in `tests/manual/release-readiness.md`
- [ ] T097 Run full manual acceptance checklist from `quickstart.md` and record results in `tests/manual/release-readiness.md`
- [X] T098 Review all docs for secret-handling/privacy consistency in `docs/provider-setup.md`, `docs/privacy.md`, and `docs/troubleshooting.md`
- [X] T099 Review generated mod package contents for config/example/docs correctness in `tests/manual/release-readiness.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 Setup**: No dependencies.
- **Phase 2 Foundational**: Depends on Phase 1. Blocks all user stories.
- **Phase 3 US1**: Depends on Phase 2. MVP.
- **Phase 4 US2**: Depends on Phase 2; can proceed in parallel with US1 after shared contracts exist, but provider behavior is surfaced through US1 UI.
- **Phase 5 US3**: Depends on Phase 2; integrates with US1 session/UI and US2 config.
- **Phase 6 US4**: Depends on provider implementations from US2 and session/UI from US1.
- **Phase 7 US5**: Depends on US1 hotkey/session behavior and foundational no-op persistence.
- **Phase 8 Polish**: Depends on target user stories being complete.

### User Story Dependencies

- **US1 (P1)**: Independent after Foundation; recommended MVP.
- **US2 (P1)**: Independent provider/config slice after Foundation; can be developed alongside US1 with careful file ownership.
- **US3 (P2)**: Requires config/session foundations and benefits from US1 UI.
- **US4 (P2)**: Requires provider clients and UI error path.
- **US5 (P3)**: Requires hotkey/session behavior and no-op persistence decisions.

### Within Each User Story

- Tests/checklists before implementation where practical.
- Models/contracts before services.
- Services before UI integration.
- Manual acceptance before marking story complete.

---

## Parallel Execution Examples

### User Story 1

```text
Parallel:
- T031 ChatSessionManagerTests.cs
- T032 AiChatProviderContractTests.cs
- T033 us1-chat-ui.md

Then:
- T034 ChatSessionManager.cs
- T035 HotkeyComputerInteraction.cs
- T036-T039 UI files
- T040 ModEntry.cs
- T041 manual verification
```

### User Story 2

```text
Parallel:
- T042 ProviderValidationTests.cs
- T043 PrivacyDefaultsTests.cs
- T044 us2-provider-config.md
- T045 AnthropicProvider.cs
- T046 OpenAiProvider.cs
- T047 OllamaProvider.cs
- T048 CustomProvider.cs

Then:
- T049 ProviderHttpClient.cs
- T050 AiProviderFactory.cs
- T051 GMCM adapter
- T052-T053 docs and manual verification
```

### User Story 3

```text
Parallel:
- T054 PromptBuilderTests.cs
- T055 GameContextPrivacyTests.cs
- T056 us3-context-spoilers.md
- T061-T067 individual context collectors

Then:
- T057-T060 prompt/context foundations
- T068 session integration
- T069 UI hint
- T070-T071 docs and manual verification
```

---

## Implementation Strategy

### MVP First

1. Complete Phase 1 Setup.
2. Complete Phase 2 Foundational.
3. Complete Phase 3 US1.
4. Stop and validate hotkey open, disabled-provider setup message, one in-flight request behavior, and no game freeze.

### Incremental Delivery

1. Add US2 provider/config support.
2. Add US3 context-aware prompt support.
3. Add US4 provider failure hardening.
4. Add US5 save/multiplayer safety verification.
5. Finish Phase 8 documentation and release readiness.

### Team Parallelization

After Phase 2, split by ownership:
- UI/session owner: `Sessions/`, `UI/`, `Interaction/`.
- Provider owner: `Providers/`, `Diagnostics/`.
- Context/prompt owner: `Context/`, `Prompts/`.
- Docs/test owner: `docs/`, `tests/manual/`, `tests/unit/`.

---

## Notes

- `[P]` tasks are parallel only when file ownership does not overlap.
- Story labels map to the five user stories in `spec.md`.
- Every story must be validated through normal in-game interaction before completion.
- Keep `ModEntry.cs` thin throughout implementation.
- Do not introduce placeable objects, mail flags, recipes, or persisted chat history in v1.
- Never log secrets, full prompts, full request bodies, or full response bodies that may include player context.
