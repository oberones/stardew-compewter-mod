using StardewModdingAPI;

namespace ComPewter.Config;

public sealed class ModConfig
{
    public SButton OpenMenuKey { get; set; } = SButton.F8;

    public AiProviderType Provider { get; set; } = AiProviderType.OpenAI;

    public string Model { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string Endpoint { get; set; } = string.Empty;

    public string SystemPrompt { get; set; } =
        "You are ComPewter, a helpful in-game Stardew Valley assistant. Give concise, practical answers about crops, villagers, fishing, mining, crafting, and daily planning. If exact game data may vary by version or mods, say so.";
}
