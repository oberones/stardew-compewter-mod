using ComPewter.Config;

namespace ComPewter.Providers;

public sealed class CustomProvider : IAiChatProvider
{
    private readonly OpenAiProvider openAiCompatible;

    public CustomProvider(ProviderHttpClient http)
    {
        this.openAiCompatible = new OpenAiProvider(http);
    }

    public AiProviderType Type => AiProviderType.Custom;

    public ProviderValidationResult Validate(ProviderSettings settings)
    {
        if (!string.Equals(settings.RequestFormat, "OpenAICompatible", StringComparison.OrdinalIgnoreCase))
            return ProviderValidationResult.Unavailable("Only OpenAICompatible custom providers are supported in v1.");

        if (string.IsNullOrWhiteSpace(settings.BaseUrl))
            return ProviderValidationResult.Unavailable("Custom provider base URL is missing.");

        return string.IsNullOrWhiteSpace(settings.Model)
            ? ProviderValidationResult.Unavailable("Custom provider model is missing.")
            : ProviderValidationResult.Available();
    }

    public async Task<AiChatResult> SendAsync(AiChatRequest request, CancellationToken cancellationToken)
    {
        return await this.openAiCompatible.SendAsync(request with { ProviderType = AiProviderType.Custom }, cancellationToken).ConfigureAwait(false);
    }
}
