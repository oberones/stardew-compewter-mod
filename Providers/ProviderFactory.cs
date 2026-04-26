using ComPewter.Config;

namespace ComPewter.Providers;

public static class ProviderFactory
{
    public static IAiProvider Create(ModConfig config)
    {
        return config.Provider switch
        {
            AiProviderType.Anthropic => new StubAiProvider("Anthropic", config),
            AiProviderType.OpenAI => new StubAiProvider("OpenAI", config),
            AiProviderType.Ollama => new StubAiProvider("Ollama", config),
            AiProviderType.Custom => new StubAiProvider("custom endpoint", config),
            _ => new StubAiProvider("unknown provider", config)
        };
    }
}
