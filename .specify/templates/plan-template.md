# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]
**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

[Extract from feature spec: primary requirement + technical approach from research]

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  the iteration process.
-->

**Language/Version**: [e.g., Python 3.11, Swift 5.9, Rust 1.75 or NEEDS CLARIFICATION]  
**Primary Dependencies**: [e.g., FastAPI, UIKit, LLVM or NEEDS CLARIFICATION]  
**Storage**: [if applicable, e.g., PostgreSQL, CoreData, files or N/A]  
**Testing**: [e.g., pytest, XCTest, cargo test or NEEDS CLARIFICATION]  
**Target Platform**: [e.g., Linux server, iOS 15+, WASM or NEEDS CLARIFICATION]
**Project Type**: [e.g., library/cli/web-service/mobile-app/compiler/desktop-app or NEEDS CLARIFICATION]  
**Performance Goals**: [domain-specific, e.g., 1000 req/s, 10k lines/sec, 60 fps or NEEDS CLARIFICATION]  
**Constraints**: [domain-specific, e.g., <200ms p95, <100MB memory, offline-capable or NEEDS CLARIFICATION]  
**Scale/Scope**: [domain-specific, e.g., 10k users, 1M LOC, 50 screens or NEEDS CLARIFICATION]

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Answer each gate with PASS, FAIL, or N/A. Any FAIL requires an entry in
Complexity Tracking with rationale, risk, mitigation, and owner.

- **Vanilla-Friendly UX**: Does the feature preserve a cozy, practical,
  non-intrusive Stardew Valley experience and avoid spoilers by default?
- **Save Safety**: Can the feature be added, migrated, and removed without
  corrupting saves or making saves unplayable?
- **Provider Agnosticism**: Is provider-specific logic isolated behind common
  interfaces for Anthropic, OpenAI, Ollama, and Custom support?
- **Privacy**: Are outbound context categories minimized, documented, and
  configurable, with secrets excluded from logs?
- **Gameplay Knowledge**: Does the feature prefer deterministic local game data
  where available and frame AI output as guidance?
- **Local Context**: Is context collection modular, configurable, testable, and
  performed on demand instead of every tick?
- **Performance**: Are network calls asynchronous with timeout/failure states,
  rate limiting or debouncing, and quiet normal logs?
- **Architecture**: Does `ModEntry` remain thin, with clear systems for config,
  providers, prompts, context, session state, UI, input, persistence, and
  diagnostics?
- **Configurability**: Are new settings safe by default, validated, and
  documented?
- **Multiplayer**: Is chat private by default, with no provider access to other
  players' data unless explicitly enabled?
- **Content/Assets**: Are assets original or placeholders, data-driven where
  practical, and consistently named?
- **Testing/Acceptance**: Does the plan cover new save, existing save, missing
  provider, invalid key, timeout, malformed response, unavailable provider, mod
  removal, multiplayer join where practical, config migration, and normal
  in-game interaction?
- **Documentation**: Are player setup, privacy behavior, troubleshooting, and
  developer architecture docs updated where relevant?

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)
<!--
  ACTION REQUIRED: Replace the placeholder tree below with the concrete layout
  for this feature. Delete unused options and expand the chosen structure with
  real paths (e.g., apps/admin, packages/something). The delivered plan must
  not include Option labels.
-->

```text
# [REMOVE IF UNUSED] Option 1: SMAPI mod project (DEFAULT)
Config/
Models/
Providers/
Prompts/
Context/
Sessions/
UI/
Input/
Persistence/
Diagnostics/
assets/
docs/

tests/
├── unit/
├── integration/
└── manual/

# [REMOVE IF UNUSED] Option 2: Web application (when "frontend" + "backend" detected)
backend/
├── src/
│   ├── models/
│   ├── services/
│   └── api/
└── tests/

frontend/
├── src/
│   ├── components/
│   ├── pages/
│   └── services/
└── tests/

# [REMOVE IF UNUSED] Option 3: Mobile + API (when "iOS/Android" detected)
api/
└── [same as backend above]

ios/ or android/
└── [platform-specific structure: feature modules, UI flows, platform tests]
```

**Structure Decision**: [Document the selected structure and reference the real
directories captured above]

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
