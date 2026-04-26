using ComPewter.Config;

namespace ComPewter.Context;

public interface IGameContextCollector
{
    void Collect(GameContextSnapshot snapshot, ModConfig config);
}
