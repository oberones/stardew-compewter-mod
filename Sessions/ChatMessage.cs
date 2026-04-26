namespace ComPewter.Sessions;

public sealed record ChatMessage(
    ChatRole Role,
    string Content,
    ChatMessageStatus Status = ChatMessageStatus.Complete);
