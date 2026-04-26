using ComPewter.Config;
using StardewModdingAPI;

namespace ComPewter.Context;

public sealed class ModListContextCollector : IGameContextCollector
{
    private readonly IModRegistry modRegistry;

    public ModListContextCollector(IModRegistry modRegistry)
    {
        this.modRegistry = modRegistry;
    }

    public void Collect(GameContextSnapshot snapshot, ModConfig config)
    {
        if (!config.Privacy.IncludeInstalledMods)
            return;

        foreach (IModInfo mod in this.modRegistry.GetAll().Take(30))
            snapshot.InstalledMods.Add($"{mod.Manifest.Name} {mod.Manifest.Version}");
    }
}
