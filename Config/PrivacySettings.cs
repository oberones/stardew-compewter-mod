namespace ComPewter.Config;

public sealed class PrivacySettings
{
    public bool ShareGameContext { get; set; } = true;

    public bool AllowSpoilers { get; set; } = false;

    public bool IncludeFriendshipData { get; set; } = false;

    public bool IncludeInstalledMods { get; set; } = false;

    public bool IncludeMultiplayerData { get; set; } = false;

    public bool RetainConversationHistory { get; set; } = true;
}
