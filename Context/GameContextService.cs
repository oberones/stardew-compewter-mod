using ComPewter.Config;
using ComPewter.Diagnostics;
using StardewModdingAPI;

namespace ComPewter.Context;

public sealed class GameContextService
{
    private readonly IReadOnlyList<IGameContextCollector> collectors;
    private readonly ModConfig config;
    private readonly ModLogger logger;

    public GameContextService(IModHelper helper, ModConfig config, ModLogger logger)
    {
        this.config = config;
        this.logger = logger;
        this.collectors = new IGameContextCollector[]
        {
            new DateWeatherContextCollector(),
            new PlayerContextCollector(),
            new InventoryContextCollector(),
            new QuestContextCollector(),
            new FriendshipContextCollector(),
            new ProgressionContextCollector(),
            new ModListContextCollector(helper.ModRegistry)
        };
    }

    public GameContextSnapshot Collect()
    {
        GameContextSnapshot snapshot = new();
        if (!this.config.Privacy.ShareGameContext)
            return snapshot;

        foreach (IGameContextCollector collector in this.collectors)
        {
            try
            {
                collector.Collect(snapshot, this.config);
            }
            catch (Exception ex)
            {
                this.logger.Warn($"Context collector {collector.GetType().Name} failed: {ex.Message}");
            }
        }

        return snapshot;
    }
}
