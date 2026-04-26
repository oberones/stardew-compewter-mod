# Contract: AI Provider Interface

## Purpose

Provider clients implement a common chat-completion contract so UI, session, and
gameplay code never depend on provider-specific request or response shapes.

## Provider Types

- `Disabled`
- `Anthropic`
- `OpenAI`
- `Ollama`
- `Custom`

## Request Contract

```csharp
public sealed record AiChatRequest(
    AiProviderType ProviderType,
    string Model,
    IReadOnlyList<ChatMessage> Messages,
    int MaxResponseTokens,
    TimeSpan Timeout);
```

Rules:
- request contains no API keys or auth headers;
- messages are already privacy-filtered;
- timeout is validated before provider call;
- model is validated by selected provider.

## Provider Contract

```csharp
public interface IAiChatProvider
{
    AiProviderType Type { get; }

    ProviderValidationResult Validate(ProviderSettings settings);

    Task<AiChatResult> SendAsync(
        AiChatRequest request,
        CancellationToken cancellationToken);
}
```

Provider responsibilities:
- validate availability and required settings;
- construct provider-specific HTTP request;
- apply authentication without exposing secrets;
- parse provider response;
- normalize errors;
- respect cancellation and timeout where practical.

## Result Contract

```csharp
public sealed record AiChatResult(
    bool Success,
    string? Content,
    AiProviderError? Error,
    TimeSpan Duration);
```

Rules:
- success result has non-empty `Content`;
- failure result has `Error`;
- duration is measured for diagnostics;
- no result includes raw provider secrets.

## Error Contract

```csharp
public enum AiProviderErrorKind
{
    MissingConfiguration,
    Authentication,
    RateLimited,
    Timeout,
    Unavailable,
    MalformedResponse,
    UnsupportedSettings,
    Unknown
}
```

Every error maps to:
- a friendly player-facing message;
- a secret-safe diagnostic message;
- provider type;
- optional status code if safe.

## Safe Logging Contract

Allowed in normal logs:
- provider type;
- duration;
- normalized error kind;
- HTTP status code when useful.

Forbidden in all logs:
- API keys;
- tokens;
- auth headers;
- full prompts/request bodies;
- full response bodies containing player context.
