using ComPewter.Config;
using StardewValley;
using StardewValley.Quests;

namespace ComPewter.Context;

public sealed class QuestContextCollector : IGameContextCollector
{
    public void Collect(GameContextSnapshot snapshot, ModConfig config)
    {
        Farmer? player = Game1.player;
        if (player is null)
            return;

        foreach (Quest quest in player.questLog.Take(8))
        {
            string title = string.IsNullOrWhiteSpace(quest.questTitle) ? quest.GetName() : quest.questTitle;
            if (!string.IsNullOrWhiteSpace(title))
                snapshot.Quests.Add(title);
        }
    }
}
