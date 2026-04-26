using ComPewter.Config;
using StardewValley;

namespace ComPewter.Context;

public sealed class InventoryContextCollector : IGameContextCollector
{
    public void Collect(GameContextSnapshot snapshot, ModConfig config)
    {
        Farmer? player = Game1.player;
        if (player is null)
            return;

        foreach (Item? item in player.Items.Where(item => item is not null).Take(24))
            snapshot.Inventory.Add($"{item!.DisplayName} x{item.Stack}");
    }
}
