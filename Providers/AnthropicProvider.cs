using System.Text.Json;
using ComPewter.Config;
using ComPewter.Sessions;

namespace ComPewter.Providers;

public sealed class AnthropicProvider : IAiChatProvider
{
    private readonly ProviderHttpClient http;

    public AnthropicProvider(ProviderHttpClient http)
    {
        this.http = http;
    }

    public AiProviderType Type => AiProviderType.Anthropic;

    public ProviderValidationResult Validate(ProviderSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
            return ProviderValidationResult.Unavailable("Anthropic API key is missing.");

        if (string.IsNullOrWhiteSpace(settings.Model))
            return ProviderValidationResult.Unavailable("Anthropic model is missing.");

        return ProviderValidationResult.Available();
    }

    public async Task<AiChatResult> SendAsync(AiChatRequest request, CancellationToken cancellationToken)
    {
        string url = BuildUrl(request);
        string? system = request.Messages.FirstOrDefault(message => message.Role == ChatRole.System)?.Content;
        var body = new
        {
            model = request.Model,
            max_tokens = request.MaxResponseTokens,
            system = system ?? string.Empty,
            messages = CollapseMessages(request.Messages)
        };

        var (document, error, duration) = await this.http.PostJsonAsync(
            this.Type,
            url,
            body,
            message =>
            {
                message.Headers.TryAddWithoutValidation("x-api-key", GetApiKey(request));
                message.Headers.TryAddWithoutValidation("anthropic-version", "2023-06-01");
            },
            request.Timeout,
            cancellationToken).ConfigureAwait(false);

        if (error is not null)
            return AiChatResult.FromError(error, duration);

        string? content = document?.RootElement.GetPropertyOrNull("content")?.EnumerateArray()
            .Select(item => item.GetPropertyOrNull("text")?.GetString())
            .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text));

        document?.Dispose();
        return string.IsNullOrWhiteSpace(content)
            ? AiChatResult.FromError(ProviderHttpClient.CreateError(this.Type, AiProviderErrorKind.MalformedResponse, "Anthropic response did not contain text content."), duration)
            : AiChatResult.FromContent(content, duration);
    }

    private static string BuildUrl(AiChatRequest request) => ProviderRuntime.Settings.Anthropic.BaseUrl.TrimEnd('/') + "/v1/messages";

    private static string GetApiKey(AiChatRequest request) => ProviderRuntime.Settings.Anthropic.ApiKey;

    private static object[] CollapseMessages(IReadOnlyList<ChatMessage> messages)
    {
        List<(string Role, string Content)> collapsed = new();
        foreach (ChatMessage message in messages.Where(message => message.Role is ChatRole.User or ChatRole.Assistant))
        {
            string role = message.Role == ChatRole.Assistant ? "assistant" : "user";
            if (collapsed.Count > 0 && collapsed[^1].Role == role)
            {
                (string existingRole, string existingContent) = collapsed[^1];
                collapsed[^1] = (existingRole, $"{existingContent}\n\n{message.Content}");
            }
            else
            {
                collapsed.Add((role, message.Content));
            }
        }

        return collapsed
            .Select(message => new
            {
                role = message.Role,
                content = message.Content
            })
            .Cast<object>()
            .ToArray();
    }
}

internal static class JsonElementExtensions
{
    public static JsonElement? GetPropertyOrNull(this JsonElement element, string propertyName)
    {
        return element.ValueKind == JsonValueKind.Object && element.TryGetProperty(propertyName, out JsonElement value)
            ? value
            : null;
    }
}
