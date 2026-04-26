using ComPewter.Models;

namespace ComPewter.Providers;

public interface IAiProvider
{
    Task<string> AskAsync(IReadOnlyList<ChatMessage> messages, CancellationToken cancellationToken = default);
}
