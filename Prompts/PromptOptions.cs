using ComPewter.Context;
using ComPewter.Sessions;

namespace ComPewter.Prompts;

public sealed record PromptOptions(
    string Question,
    bool AllowSpoilers,
    bool RestrictToStardewTopics,
    bool IncludeGameContext,
    GameContextSnapshot? GameContext,
    IReadOnlyList<ChatMessage> History);
