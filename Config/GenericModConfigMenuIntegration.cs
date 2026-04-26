using StardewModdingAPI;

namespace ComPewter.Config;

public static class GenericModConfigMenuIntegration
{
    public static void Register(IModHelper helper, IMonitor monitor, Func<ModConfig> getConfig, Action<ModConfig> saveConfig)
    {
        try
        {
            if (!helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
                return;

            monitor.Log("Generic Mod Config Menu is installed. ComPewter will use config.json until GMCM wiring is expanded.", LogLevel.Trace);
        }
        catch (Exception ex)
        {
            monitor.Log($"Generic Mod Config Menu integration skipped: {ex.Message}", LogLevel.Trace);
        }
    }
}
