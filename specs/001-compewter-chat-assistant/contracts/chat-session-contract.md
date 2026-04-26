# Contract: Chat Session And UI State

## Session States

- `Idle`: input enabled, no provider request pending.
- `Submitting`: one provider request is in flight, submit is disabled.
- `Error`: latest request failed, input is enabled for retry.
- `Closed`: menu closed; pending request cancellation requested.

## Submit Rules

1. Empty or whitespace-only questions are ignored.
2. If state is `Submitting`, additional submits are rejected locally.
3. A valid question appends a user message and loading/status message.
4. On provider success, loading/status is replaced with assistant response.
5. On provider failure, loading/status is replaced with friendly error message.

## History Rules

- history is current-session only in v1;
- history may be excluded from provider prompt by config;
- retained message count is capped by config;
- restarting the game clears chat history.

## Cancellation Rules

- closing the menu requests cancellation where practical;
- provider completion after close must not crash or mutate disposed UI;
- timeout maps to normalized provider timeout error.

## UI Requirements

- opens via configured hotkey;
- supports keyboard and mouse input;
- renders previous messages in current session;
- displays loading, error, and response states;
- supports scrolling when messages exceed visible area;
- remains readable at common UI scales.
