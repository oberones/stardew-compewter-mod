namespace ComPewter.Providers;

public sealed record AiChatResult(
    bool Success,
    string? Content,
    AiProviderError? Error,
    TimeSpan Duration)
{
    public static AiChatResult FromContent(string content, TimeSpan duration)
    {
        return new AiChatResult(true, content, null, duration);
    }

    public static AiChatResult FromError(AiProviderError error, TimeSpan duration)
    {
        return new AiChatResult(false, null, error, duration);
    }
}
