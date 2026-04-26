using ComPewter.Config;
using StardewValley;

namespace ComPewter.Context;

public sealed class DateWeatherContextCollector : IGameContextCollector
{
    public void Collect(GameContextSnapshot snapshot, ModConfig config)
    {
        snapshot.Values["Season"] = Game1.currentSeason;
        snapshot.Values["Day"] = Game1.dayOfMonth.ToString();
        snapshot.Values["Year"] = Game1.year.ToString();
        snapshot.Values["Weather"] = Game1.isRaining ? "Rain" : Game1.isSnowing ? "Snow" : "Clear";
        snapshot.Values["Daily luck"] = Game1.player?.DailyLuck.ToString("0.000") ?? "Unknown";
        snapshot.Values["Spoilers allowed"] = config.Privacy.AllowSpoilers ? "Yes" : "No";
    }
}
