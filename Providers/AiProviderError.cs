using ComPewter.Config;

namespace ComPewter.Providers;

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

public sealed record AiProviderError(
    AiProviderErrorKind Kind,
    string PlayerMessage,
    string DiagnosticMessage,
    AiProviderType ProviderType,
    int? StatusCode = null);
