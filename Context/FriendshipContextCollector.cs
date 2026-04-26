using ComPewter.Config;
using StardewValley;

namespace ComPewter.Context;

public sealed class FriendshipContextCollector : IGameContextCollector
{
    public void Collect(GameContextSnapshot snapshot, ModConfig config)
    {
        if (!config.Privacy.IncludeFriendshipData || Game1.player is null)
            return;

        foreach ((string name, Friendship friendship) in Game1.player.friendshipData.Pairs.Take(12))
            snapshot.Friendships.Add($"{name}: {friendship.Points} friendship points");
    }
}
