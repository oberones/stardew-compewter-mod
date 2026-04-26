using ComPewter.Config;
using ComPewter.Sessions;

namespace ComPewter.Providers;

public sealed class OpenAiProvider : IAiChatProvider
{
    private readonly ProviderHttpClient http;

    public OpenAiProvider(ProviderHttpClient http)
    {
        this.http = http;
    }

    public AiProviderType Type => AiProviderType.OpenAI;

    public ProviderValidationResult Validate(ProviderSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
            return ProviderValidationResult.Unavailable("OpenAI API key is missing.");

        if (string.IsNullOrWhiteSpace(settings.Model))
            return ProviderValidationResult.Unavailable("OpenAI model is missing.");

        return ProviderValidationResult.Available();
    }

    public async Task<AiChatResult> SendAsync(AiChatRequest request, CancellationToken cancellationToken)
    {
        ProviderSettings settings = request.ProviderType == AiProviderType.Custom
            ? ProviderRuntime.Settings.Custom
            : ProviderRuntime.Settings.OpenAI;
        string endpoint = request.ProviderType == AiProviderType.Custom && !string.IsNullOrWhiteSpace(settings.EndpointPath)
            ? settings.EndpointPath
            : "/v1/chat/completions";
        string url = settings.BaseUrl.TrimEnd('/') + "/" + endpoint.TrimStart('/');
        object body = request.ProviderType == AiProviderType.Custom
            ? new
            {
                model = request.Model,
                max_tokens = request.MaxResponseTokens,
                messages = request.Messages.Select(ToWireMessage).ToArray()
            }
            : new
            {
                model = request.Model,
                max_completion_tokens = request.MaxResponseTokens,
                messages = request.Messages.Select(ToWireMessage).ToArray()
            };

        var (document, error, duration) = await this.http.PostJsonAsync(
            request.ProviderType,
            url,
            body,
            message => ConfigureAuth(message, settings, request.ProviderType),
            request.Timeout,
            cancellationToken).ConfigureAwait(false);

        if (error is not null)
            return AiChatResult.FromError(error, duration);

        string? content = document?.RootElement.GetPropertyOrNull("choices")?.EnumerateArray()
            .Select(choice => choice.GetPropertyOrNull("message")?.GetPropertyOrNull("content")?.GetString())
            .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text));

        document?.Dispose();
        return string.IsNullOrWhiteSpace(content)
            ? AiChatResult.FromError(ProviderHttpClient.CreateError(this.Type, AiProviderErrorKind.MalformedResponse, "OpenAI response did not contain assistant content."), duration)
            : AiChatResult.FromContent(content, duration);
    }

    private static void ConfigureAuth(HttpRequestMessage message, ProviderSettings settings, AiProviderType providerType)
    {
        if (providerType == AiProviderType.Custom)
        {
            if (!string.IsNullOrWhiteSpace(settings.AuthHeaderName) && !string.IsNullOrWhiteSpace(settings.AuthToken))
                message.Headers.TryAddWithoutValidation(settings.AuthHeaderName, settings.AuthToken);
            else
                ProviderHttpClient.SetBearer(message, settings.AuthToken);

            return;
        }

        ProviderHttpClient.SetBearer(message, settings.ApiKey);
    }

    private static object ToWireMessage(ChatMessage message)
    {
        return new
        {
            role = message.Role switch
            {
                ChatRole.System => "system",
                ChatRole.Assistant => "assistant",
                _ => "user"
            },
            content = message.Content
        };
    }
}
