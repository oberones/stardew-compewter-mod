using StardewModdingAPI;

namespace ComPewter.Config;

public sealed class ConfigValidator
{
    public const int CurrentSchemaVersion = 1;

    private readonly IMonitor monitor;

    public ConfigValidator(IMonitor monitor)
    {
        this.monitor = monitor;
    }

    public ModConfig Normalize(ModConfig? config)
    {
        config ??= new ModConfig();
        bool changed = false;

        if (config.SchemaVersion < CurrentSchemaVersion)
        {
            config.SchemaVersion = CurrentSchemaVersion;
            changed = true;
        }

        config.Privacy ??= new PrivacySettings();
        config.Ui ??= new UiSettings();
        config.Anthropic ??= new ProviderSettings { BaseUrl = "https://api.anthropic.com" };
        config.OpenAI ??= new ProviderSettings { BaseUrl = "https://api.openai.com" };
        config.Ollama ??= new ProviderSettings { BaseUrl = "http://localhost:11434" };
        config.Custom ??= new ProviderSettings { RequestFormat = "OpenAICompatible" };

        int timeout = Clamp(config.RequestTimeoutSeconds, 5, 120, 30, nameof(config.RequestTimeoutSeconds), ref changed);
        int tokens = Clamp(config.MaxResponseTokens, 100, 4000, 700, nameof(config.MaxResponseTokens), ref changed);
        int retained = Clamp(config.MaxRetainedMessages, 0, 100, 20, nameof(config.MaxRetainedMessages), ref changed);

        config.RequestTimeoutSeconds = timeout;
        config.MaxResponseTokens = tokens;
        config.MaxRetainedMessages = retained;

        if (config.Ui.UiScale is < 0.75f or > 2.0f)
        {
            this.monitor.Log($"Invalid UiScale '{config.Ui.UiScale}'. Falling back to 1.0.", LogLevel.Warn);
            config.Ui.UiScale = 1.0f;
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(config.Ollama.BaseUrl))
        {
            config.Ollama.BaseUrl = "http://localhost:11434";
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(config.OpenAI.BaseUrl))
        {
            config.OpenAI.BaseUrl = "https://api.openai.com";
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(config.Anthropic.BaseUrl))
        {
            config.Anthropic.BaseUrl = "https://api.anthropic.com";
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(config.Custom.RequestFormat))
        {
            config.Custom.RequestFormat = "OpenAICompatible";
            changed = true;
        }

        if (changed)
            this.monitor.Log("Config was repaired with safe defaults. Review config.json if needed.", LogLevel.Warn);

        return config;
    }

    private int Clamp(int value, int min, int max, int fallback, string name, ref bool changed)
    {
        if (value >= min && value <= max)
            return value;

        this.monitor.Log($"Invalid {name} '{value}'. Falling back to {fallback}.", LogLevel.Warn);
        changed = true;
        return fallback;
    }
}
