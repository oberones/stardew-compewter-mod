namespace ComPewter.Sessions;

public sealed class ChatSession
{
    private readonly List<ChatMessage> messages = new();

    public IReadOnlyList<ChatMessage> Messages => this.messages;

    public bool IsRequestInFlight { get; set; }

    public void Add(ChatMessage message)
    {
        this.messages.Add(message);
    }

    public void ReplaceLast(ChatMessage message)
    {
        if (this.messages.Count == 0)
        {
            this.messages.Add(message);
            return;
        }

        this.messages[^1] = message;
    }

    public IReadOnlyList<ChatMessage> RecentHistory(int maxMessages)
    {
        if (maxMessages <= 0 || this.messages.Count == 0)
            return Array.Empty<ChatMessage>();

        return this.RecentHistoryBefore(this.messages.Count, maxMessages);
    }

    public IReadOnlyList<ChatMessage> RecentHistoryBefore(int exclusiveEndIndex, int maxMessages)
    {
        if (maxMessages <= 0 || exclusiveEndIndex <= 0 || this.messages.Count == 0)
            return Array.Empty<ChatMessage>();

        return this.messages
            .Take(Math.Min(exclusiveEndIndex, this.messages.Count))
            .Where(message => message.Status == ChatMessageStatus.Complete)
            .TakeLast(maxMessages)
            .ToArray();
    }

    public void EnforceCap(int maxMessages)
    {
        if (maxMessages <= 0)
        {
            this.messages.Clear();
            return;
        }

        int overflow = this.messages.Count - maxMessages;
        if (overflow > 0)
            this.messages.RemoveRange(0, overflow);
    }
}
