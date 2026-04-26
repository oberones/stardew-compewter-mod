using StardewModdingAPI;

namespace ComPewter.Config;

public sealed class ModConfig
{
    public int SchemaVersion { get; set; } = 1;

    public SButton OpenMenuKey { get; set; } = SButton.F8;

    public AiProviderType Provider { get; set; } = AiProviderType.Disabled;

    public int RequestTimeoutSeconds { get; set; } = 30;

    public int MaxResponseTokens { get; set; } = 700;

    public int MaxRetainedMessages { get; set; } = 20;

    public bool DebugLogging { get; set; } = false;

    public bool RestrictToStardewTopics { get; set; } = true;

    public PrivacySettings Privacy { get; set; } = new();

    public UiSettings Ui { get; set; } = new();

    public ProviderSettings Anthropic { get; set; } = new()
    {
        BaseUrl = "https://api.anthropic.com",
        Model = string.Empty
    };

    public ProviderSettings OpenAI { get; set; } = new()
    {
        BaseUrl = "https://api.openai.com",
        Model = string.Empty
    };

    public ProviderSettings Ollama { get; set; } = new()
    {
        BaseUrl = "http://localhost:11434",
        Model = string.Empty
    };

    public ProviderSettings Custom { get; set; } = new()
    {
        RequestFormat = "OpenAICompatible"
    };
}
