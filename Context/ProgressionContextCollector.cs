using ComPewter.Config;
using StardewValley;

namespace ComPewter.Context;

public sealed class ProgressionContextCollector : IGameContextCollector
{
    public void Collect(GameContextSnapshot snapshot, ModConfig config)
    {
        Farmer? player = Game1.player;
        if (player is null)
            return;

        if (Game1.MasterPlayer.mailReceived.Contains("ccDoorUnlock"))
            snapshot.Progression.Add("Community Center unlocked");
        if (Game1.MasterPlayer.mailReceived.Contains("jojaMember"))
            snapshot.Progression.Add("Joja membership purchased");
        if (player.hasRustyKey)
            snapshot.Progression.Add("Sewer key unlocked");
        if (player.hasSkullKey)
            snapshot.Progression.Add("Skull Key unlocked");
    }
}
