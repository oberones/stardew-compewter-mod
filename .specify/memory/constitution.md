<!--
Sync Impact Report
Version change: 1.0.0 -> 1.0.1
Modified principles:
- Previous project name corrected to ComPewter across the constitution
Added sections:
- None
Removed sections:
- None
Templates requiring updates:
- ✅ .specify/templates/spec-template.md
- ✅ .specify/templates/tasks-template.md
- ✅ .specify/templates/checklist-template.md
- ✅ README.md
- ✅ specs/001-compewter-chat-assistant/spec.md
- ⚠ .specify/templates/commands/*.md not present in this project
Follow-up TODOs: None
-->

# ComPewter Constitution

## Core Principles

### I. Vanilla-Friendly Player Experience

ComPewter MUST feel like it belongs naturally in Stardew Valley. The
in-game computer MUST behave like a helpful farm planning tool, not an intrusive
external assistant. Assistant responses MUST be cozy, friendly, concise, and
practical. The assistant MUST help players enjoy the game rather than optimize
all fun out of it. Spoilers MUST be avoided by default unless the player
explicitly enables spoiler-heavy answers. The chat interface MUST be readable,
low-friction, and usable with keyboard and mouse. Controller support SHOULD be
considered where practical because Stardew Valley is commonly played that way.

Rationale: The mod succeeds only if it preserves the pace, charm, and agency of
Stardew Valley while reducing friction for players who want guidance.

### II. Save Safety and Stability

The mod MUST never corrupt saves. Removing the mod MUST NOT make a save
unplayable. Any custom objects, furniture, mail flags, recipes, or persistent
state MUST be designed for safe add/remove behavior. Invalid config, missing API
keys, provider errors, malformed responses, and network failures MUST NOT crash
the game. Failures MUST show clear player-facing messages and useful SMAPI logs
without exposing secrets.

Rationale: Save safety is the highest technical obligation for a Stardew Valley
mod because players invest long-running, personal progress in their farms.

### III. Provider-Agnostic AI Integration

The mod MUST support Anthropic, OpenAI, Ollama, and Custom provider options.
Provider logic MUST be isolated behind a common interface so additional
providers can be added without changing chat UI, prompt construction, or context
collection. Players MUST be able to choose their provider in config. API keys,
tokens, and base URLs MUST be read from config or environment-supported
mechanisms and MUST never be logged. The mod MUST work with no provider
configured by showing a helpful setup message. The Custom provider MUST support
configurable base URL, model name, authentication header or token, and request
format where practical.

Rationale: Provider choice protects player autonomy, supports local/offline
preferences through Ollama, and prevents architecture from hard-coding one
vendor.

### IV. Privacy and Transparency

Documentation MUST clearly state what information may be sent to the selected AI
provider. The mod MUST send only the minimum game context needed to answer the
player's question. The mod MUST NOT send personally identifying information,
machine paths, save names, player names, farm names, or multiplayer player data
unless explicitly required, disabled by default, and documented. Players MUST be
able to disable automatic game-context sharing. Players MUST be able to inspect
or understand the categories of context included in requests.

Rationale: AI features can leave the local machine. Players deserve clear
control over what leaves their game and why.

### V. Gameplay Knowledge Boundaries

The assistant MUST focus on Stardew Valley gameplay guidance, including crops,
crop profitability, villager gifts, fishing times/weather/seasons/locations,
bundles, community center planning, mining, combat, crafting, cooking,
relationships, and daily planning based on season, weather, luck, money,
inventory, and unlocked content. The assistant MUST be honest when it does not
know something. The mod MUST prefer deterministic local game data when available
instead of relying entirely on model memory. AI-generated advice MUST be framed
as guidance, not guaranteed truth, especially when modded content may alter
gameplay.

Rationale: Clear knowledge boundaries reduce hallucination risk and keep the
assistant useful for game decisions without pretending to be authoritative about
unknown or modded data.

### VI. Local Game Context First

The mod MUST gather relevant in-game context through SMAPI and Stardew Valley
APIs where possible. Context MAY include date, season, weather, daily luck,
player location, known villagers, inventory summary, wallet, farm type, unlocked
areas, active quests, bundle progress, and installed mod list when enabled.
Context gathering MUST be modular, configurable, and testable. Expensive context
collection MUST NOT run every tick. Context MUST be collected on demand when the
player submits a question, with limited caching where appropriate.

Rationale: Local context improves answer quality while modular collection keeps
privacy controls, tests, and performance understandable.

### VII. Performance Discipline

The mod MUST avoid expensive per-frame or per-tick work. Network calls to AI
providers MUST be asynchronous or otherwise structured so the game does not
freeze. The UI MUST display loading, success, and failure states clearly.
Requests MUST support timeout handling. Repeated requests MUST be rate-limited
or debounced to prevent accidental spam. Logs MUST be useful for debugging and
quiet during normal gameplay.

Rationale: A chat assistant must never make the farm feel sluggish or unstable.

### VIII. Maintainable SMAPI Architecture

`ModEntry` MUST stay thin. Code MUST be organized into clear systems for
configuration, provider clients, prompt construction, game context collection,
chat session state, UI rendering, input handling, item/object integration,
persistence, and logging/diagnostics. Names and interfaces MUST be explicit and
simple. Monolithic classes MUST be avoided. Dependency injection or simple
composition MUST be preferred over global static state. Harmony patches MAY be
used only when SMAPI events or content APIs are insufficient, and the rationale
MUST be documented.

Rationale: The mod crosses UI, networking, configuration, and game data. Clear
boundaries keep those concerns testable and safe to evolve.

### IX. Configurability

Players MUST be able to configure selected AI provider, model name, API key or
token reference, base URL for Ollama and Custom providers, max response length,
request timeout, game-context sharing, spoiler allowance, conversation history
retention, maximum retained chat messages, and the hotkey to open the
computer/chat interface when supported. Defaults MUST be safe and understandable.
Invalid config values MUST fall back to safe defaults with warnings.

Rationale: AI and privacy preferences vary widely. Safe configurability lets
players choose their comfort level without editing code.

### X. Multiplayer Awareness

The mod MUST be safe in multiplayer. AI chat MUST be private to the local player
by default. The mod MUST NOT send other players' information to providers unless
explicitly enabled. World-changing behavior MUST NOT depend on AI responses. If
the computer object is placeable or shared, interactions MUST avoid desync and
SHOULD remain local UI interactions unless explicitly designed otherwise.

Rationale: Multiplayer saves add privacy and synchronization risks that are not
acceptable for a guidance-only assistant.

### XI. Content and Asset Rules

The mod MUST use original assets or clearly marked placeholders. It MUST NOT
redistribute Stardew Valley assets. Pixel art SHOULD match the spirit of Stardew
Valley without copying protected assets. Asset paths, object IDs, and content
keys MUST be named consistently. Content SHOULD be data-driven where practical
because data-driven content is easier to review, patch, and remove safely.

Rationale: Asset discipline protects the project legally and supports clean
content integration.

### XII. Testing and Acceptance

Each feature MUST include testable acceptance criteria. Tests and acceptance
passes MUST cover new saves, existing saves, no provider configured, invalid API
key, provider timeout, malformed provider response, provider unavailable, mod
removal, multiplayer join where practical, and config migration. Before a
feature is complete, it MUST be tested through normal in-game interaction.

Rationale: The most important risks are save compatibility, provider failure,
and player-facing behavior inside Stardew Valley, so tests must cover those
paths directly.

### XIII. Documentation

The mod MUST include player-facing documentation for Anthropic, OpenAI, Ollama,
and Custom provider setup. Documentation MUST explain privacy behavior and the
categories of game context that may be sent. Documentation MUST include
troubleshooting steps. Developer documentation MUST explain provider interfaces,
prompt construction, context collection, and UI architecture.

Rationale: Provider setup and privacy choices are part of the feature, not
afterthoughts.

### XIV. Governance Priority

These principles MUST override implementation convenience. Any exception MUST be
documented with rationale, risk, and mitigation. When principles conflict,
decisions MUST prioritize save safety, privacy, stability, vanilla-friendly
experience, and maintainability in that order.

Rationale: Explicit priority order prevents convenience-driven decisions from
weakening the player's save, privacy, or experience.

## SMAPI Architecture and Operational Constraints

ComPewter targets Stardew Valley through SMAPI. Features MUST use SMAPI
events, content APIs, and Stardew Valley APIs before considering Harmony patches.
Persistent data MUST be versioned or migratable. Provider clients MUST avoid
logging request bodies, response bodies that may contain player context, API
keys, tokens, and authentication headers. Prompt construction MUST be separated
from transport so privacy and context behavior can be reviewed independently.
All network transport MUST have timeouts, cancellation paths where practical,
and graceful error mapping for player-facing UI.

## Delivery and Review Workflow

Every feature specification MUST identify privacy impact, save-safety impact,
provider behavior, local context categories, multiplayer behavior, config
changes, failure states, and in-game acceptance criteria. Every implementation
plan MUST pass the Constitution Check before Phase 0 research and again after
Phase 1 design. Task lists MUST include explicit work for documentation,
configuration validation, provider failure handling, logging, and normal
in-game verification when those areas are touched.

## Governance

This constitution supersedes conflicting implementation preferences, generated
plans, templates, and informal conventions. Amendments MUST update this file,
include a Sync Impact Report, update dependent templates and runtime guidance,
and explain the semantic version bump.

Versioning policy:
- MAJOR: Principle removals, priority order changes, or governance changes that
  weaken existing obligations.
- MINOR: New principles, new required sections, or materially expanded guidance.
- PATCH: Clarifications, typo fixes, or non-semantic wording improvements.

Compliance review MUST happen during feature specification, implementation
planning, task generation, code review, and pre-release validation. Any approved
exception MUST document rationale, risk, mitigation, and owner in the relevant
spec or plan.

**Version**: 1.0.1 | **Ratified**: 2026-04-26 | **Last Amended**: 2026-04-26
