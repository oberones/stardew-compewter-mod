using ComPewter.Config;
using ComPewter.Diagnostics;

namespace ComPewter.Providers;

public sealed class AiProviderFactory
{
    private readonly ProviderHttpClient http;
    private readonly ModLogger logger;

    public AiProviderFactory(ProviderHttpClient http, ModLogger logger)
    {
        this.http = http;
        this.logger = logger;
    }

    public IAiChatProvider Create(AiProviderType providerType)
    {
        return providerType switch
        {
            AiProviderType.Anthropic => new AnthropicProvider(this.http),
            AiProviderType.OpenAI => new OpenAiProvider(this.http),
            AiProviderType.Ollama => new OllamaProvider(this.http),
            AiProviderType.Custom => new CustomProvider(this.http),
            _ => new DisabledProvider()
        };
    }
}
