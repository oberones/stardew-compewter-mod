# Research: ComPewter Chat Assistant

## Decision: Hotkey-only access for first version

**Rationale**: A configurable hotkey avoids custom object, furniture, recipe,
shop, and mail state. It is safest for existing saves, easiest to remove, and
still validates chat, provider, prompt, context, privacy, and UI behavior.

**Alternatives considered**: Custom object, furniture item, shop item, crafting
recipe, mail reward. These are more immersive but introduce content/asset and
save-removal risk before the core assistant behavior is proven.

## Decision: Provider default is Disabled

**Rationale**: No provider request should happen until the player explicitly
configures provider settings. This aligns with privacy and avoids accidental
network calls or setup failures.

**Alternatives considered**: Default Ollama for local-first behavior; default
OpenAI with setup message. Both are less privacy-explicit and can surprise users.

## Decision: Context sharing defaults Off but must be prominent

**Rationale**: Game context is one of the mod's main values, but it can leave the
machine. Default Off preserves trust; prominent setup guidance makes the useful
path obvious.

**Alternatives considered**: Basic context On by default; all non-sensitive
context On by default. Both send gameplay state before explicit player consent.

## Decision: Current-session chat history only

**Rationale**: In-memory history supports natural conversation while avoiding
save-data privacy and migration risks. The history cap remains useful for prompt
size and UI limits.

**Alternatives considered**: Persist history by default; persist on opt-in.
Both require save schema, deletion controls, migration, and additional privacy
documentation better handled in a later feature.

## Decision: One in-flight provider request per chat session

**Rationale**: Prevents accidental spam, simplifies UI state, makes cancellation
and timeout behavior testable, and avoids concurrent response ordering issues.

**Alternatives considered**: Queue requests sequentially; cancel previous
request on new submit. Queues complicate state and cost; cancel-on-submit can
surprise players by discarding questions.

## Decision: Provider abstraction normalizes requests and errors

**Rationale**: Anthropic, OpenAI, Ollama, and Custom differ in schemas and error
formats. A common `IAiChatProvider` keeps UI/session code stable and makes new
providers additive.

**Alternatives considered**: Let UI/session call provider-specific clients
directly. Rejected because it leaks transport details and multiplies error
handling logic.

## Decision: Use provider-specific clients over one generic HTTP client

**Rationale**: Each provider has different auth, request, response, and error
semantics. Small provider classes are easier to validate and redact safely.

**Alternatives considered**: One configurable generic client for all providers.
Rejected because it would obscure provider-specific validation and error
normalization. Custom provider can still support OpenAI-compatible behavior.

## Decision: On-demand modular context collection

**Rationale**: Context should be fresh when the player asks a question, but
collection must not run every tick. Modular collectors make privacy, testing,
and future categories easier.

**Alternatives considered**: Collect every update tick; gather one giant context
object inline in the session manager. Both violate performance/maintainability.

## Decision: Prompt builder owns section boundaries

**Rationale**: A single prompt builder can enforce compactness, spoiler rules,
privacy filtering, and clear separation between instructions, context, history,
and the current question.

**Alternatives considered**: Providers build prompts independently. Rejected
because behavior would drift across providers and privacy review would be harder.

## Decision: No save-data schema in first version

**Rationale**: Hotkey access and current-session chat make save data unnecessary.
This directly supports safe add/remove behavior.

**Alternatives considered**: Store session history or unlock state. Deferred
until a future feature explicitly needs persistent state.

## Decision: Manual in-game testing remains required

**Rationale**: UI, SMAPI event behavior, save add/remove, and multiplayer smoke
cannot be fully trusted through build-only validation.

**Alternatives considered**: Unit tests only. Rejected because the highest risks
are in-game integration and player-facing failure behavior.
