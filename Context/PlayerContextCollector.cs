using ComPewter.Config;
using StardewValley;

namespace ComPewter.Context;

public sealed class PlayerContextCollector : IGameContextCollector
{
    public void Collect(GameContextSnapshot snapshot, ModConfig config)
    {
        Farmer? player = Game1.player;
        if (player is null)
            return;

        snapshot.Values["Money"] = player.Money.ToString();
        snapshot.Values["Current location"] = Game1.currentLocation?.NameOrUniqueName ?? Game1.currentLocation?.Name ?? "Unknown";
        snapshot.Values["Farm type"] = Game1.whichFarm.ToString();
        snapshot.Values["Skills"] = $"Farming {player.FarmingLevel}, Mining {player.MiningLevel}, Foraging {player.ForagingLevel}, Fishing {player.FishingLevel}, Combat {player.CombatLevel}";
    }
}
