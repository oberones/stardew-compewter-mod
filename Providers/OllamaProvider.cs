using ComPewter.Config;
using ComPewter.Sessions;

namespace ComPewter.Providers;

public sealed class OllamaProvider : IAiChatProvider
{
    private readonly ProviderHttpClient http;

    public OllamaProvider(ProviderHttpClient http)
    {
        this.http = http;
    }

    public AiProviderType Type => AiProviderType.Ollama;

    public ProviderValidationResult Validate(ProviderSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.BaseUrl))
            return ProviderValidationResult.Unavailable("Ollama base URL is missing.");

        if (string.IsNullOrWhiteSpace(settings.Model))
            return ProviderValidationResult.Unavailable("Ollama model is missing.");

        return ProviderValidationResult.Available();
    }

    public async Task<AiChatResult> SendAsync(AiChatRequest request, CancellationToken cancellationToken)
    {
        ProviderSettings settings = ProviderRuntime.Settings.Ollama;
        string url = settings.BaseUrl.TrimEnd('/') + "/api/chat";
        var body = new
        {
            model = request.Model,
            stream = false,
            messages = request.Messages.Select(message => new
            {
                role = message.Role switch
                {
                    ChatRole.System => "system",
                    ChatRole.Assistant => "assistant",
                    _ => "user"
                },
                content = message.Content
            }).ToArray(),
            options = new
            {
                num_predict = request.MaxResponseTokens
            }
        };

        var (document, error, duration) = await this.http.PostJsonAsync(this.Type, url, body, null, request.Timeout, cancellationToken).ConfigureAwait(false);
        if (error is not null)
            return AiChatResult.FromError(error, duration);

        string? content = document?.RootElement.GetPropertyOrNull("message")?.GetPropertyOrNull("content")?.GetString();
        document?.Dispose();
        return string.IsNullOrWhiteSpace(content)
            ? AiChatResult.FromError(ProviderHttpClient.CreateError(this.Type, AiProviderErrorKind.MalformedResponse, "Ollama response did not contain message content."), duration)
            : AiChatResult.FromContent(content, duration);
    }
}
