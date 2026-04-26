using ComPewter.Config;
using ComPewter.Diagnostics;

namespace ComPewter.Providers;

public sealed class DisabledProvider : IAiChatProvider
{
    public AiProviderType Type => AiProviderType.Disabled;

    public ProviderValidationResult Validate(ProviderSettings settings)
    {
        return ProviderValidationResult.Unavailable("ComPewter is ready, but no AI provider is configured yet. Choose Anthropic, OpenAI, Ollama, or Custom in config to enable answers.");
    }

    public Task<AiChatResult> SendAsync(AiChatRequest request, CancellationToken cancellationToken)
    {
        AiProviderError error = new(
            AiProviderErrorKind.MissingConfiguration,
            "ComPewter is ready, but no AI provider is configured yet. Choose Anthropic, OpenAI, Ollama, or Custom in config to enable answers.",
            "Provider is Disabled.",
            AiProviderType.Disabled);

        return Task.FromResult(AiChatResult.FromError(error, TimeSpan.Zero));
    }
}
