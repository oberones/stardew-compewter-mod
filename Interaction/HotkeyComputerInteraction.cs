using ComPewter.Config;
using StardewModdingAPI;
using StardewValley;

namespace ComPewter.Interaction;

public sealed class HotkeyComputerInteraction
{
    private readonly ModConfig config;

    public HotkeyComputerInteraction(ModConfig config)
    {
        this.config = config;
    }

    public bool ShouldOpen(SButton button)
    {
        return StardewModdingAPI.Context.IsWorldReady
            && Game1.activeClickableMenu is null
            && button == this.config.OpenMenuKey;
    }
}
