# Architecture

ComPewter keeps `ModEntry` as a composition root. It loads and validates config, creates diagnostics, provider clients, prompt/context/session services, and wires the hotkey to the chat menu.

## Modules

- `Config/`: strongly typed config, validation, safe defaults, and optional Generic Mod Config Menu hook.
- `Diagnostics/`: secret redaction, quiet logging, and provider error to player-message mapping.
- `Providers/`: provider-neutral chat contracts plus Anthropic, OpenAI, Ollama, Custom, and Disabled implementations.
- `Prompts/`: system prompt and prompt envelope construction.
- `Context/`: on-demand SMAPI/Stardew game context collectors.
- `Sessions/`: current-session message state, in-flight guard, prompt creation, provider dispatch, and history cap.
- `UI/`: custom in-game menu, text input, scrolling, and loading/error rendering.
- `Interaction/`: local hotkey open behavior.
- `Persistence/`: v1 no-op save service to make save-safety explicit.

## Request Flow

1. Player presses `OpenMenuKey`.
2. `HotkeyComputerInteraction` allows opening only when a save is ready and no menu is active.
3. `ChatMenu` submits text to `ChatSessionManager`.
4. The session validates the selected provider.
5. Context is collected only if `Privacy.ShareGameContext` is enabled.
6. `PromptBuilder` assembles system instructions, optional context, optional history, and the question.
7. Provider client sends an async HTTP request with timeout/cancellation.
8. The session stores a complete assistant message or friendly error.
9. The UI redraws from session state.

Provider-specific JSON and authentication never leak into UI or gameplay code.
