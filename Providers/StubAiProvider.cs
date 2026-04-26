using ComPewter.Config;
using ComPewter.Models;

namespace ComPewter.Providers;

public sealed class StubAiProvider : IAiProvider
{
    private readonly string providerName;
    private readonly ModConfig config;

    public StubAiProvider(string providerName, ModConfig config)
    {
        this.providerName = providerName;
        this.config = config;
    }

    public Task<string> AskAsync(IReadOnlyList<ChatMessage> messages, CancellationToken cancellationToken = default)
    {
        string latestQuestion = messages.LastOrDefault(message => message.Role == ChatRole.User)?.Content ?? "your question";
        string endpointHint = string.IsNullOrWhiteSpace(this.config.Endpoint)
            ? "No endpoint is configured yet."
            : $"Configured endpoint: {this.config.Endpoint}";

        return Task.FromResult(
            $"ComPewter is wired for {this.providerName}, but live AI calls are not implemented yet. " +
            $"{endpointHint} Last question: \"{latestQuestion}\"");
    }
}
