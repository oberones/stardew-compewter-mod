using System.Text;

namespace ComPewter.Context;

public sealed class GameContextSnapshot
{
    public Dictionary<string, string> Values { get; } = new(StringComparer.OrdinalIgnoreCase);

    public List<string> Inventory { get; } = new();

    public List<string> Quests { get; } = new();

    public List<string> Friendships { get; } = new();

    public List<string> InstalledMods { get; } = new();

    public List<string> Progression { get; } = new();

    public bool IsEmpty =>
        this.Values.Count == 0
        && this.Inventory.Count == 0
        && this.Quests.Count == 0
        && this.Friendships.Count == 0
        && this.InstalledMods.Count == 0
        && this.Progression.Count == 0;

    public string ToPromptText()
    {
        StringBuilder builder = new();
        foreach ((string key, string value) in this.Values)
            builder.AppendLine($"- {key}: {value}");

        AppendList(builder, "Inventory", this.Inventory);
        AppendList(builder, "Active quests", this.Quests);
        AppendList(builder, "Friendship summary", this.Friendships);
        AppendList(builder, "Progression", this.Progression);
        AppendList(builder, "Installed mods", this.InstalledMods);

        return builder.Length == 0 ? "- No local game context was available." : builder.ToString().Trim();
    }

    private static void AppendList(StringBuilder builder, string heading, IReadOnlyList<string> values)
    {
        if (values.Count == 0)
            return;

        builder.AppendLine($"- {heading}: {string.Join("; ", values)}");
    }
}
