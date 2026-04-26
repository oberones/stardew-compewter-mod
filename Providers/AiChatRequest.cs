using ComPewter.Config;
using ComPewter.Sessions;

namespace ComPewter.Providers;

public sealed record AiChatRequest(
    AiProviderType ProviderType,
    string Model,
    IReadOnlyList<ChatMessage> Messages,
    int MaxResponseTokens,
    TimeSpan Timeout);
