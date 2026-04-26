using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ComPewter.Config;
using ComPewter.Diagnostics;

namespace ComPewter.Providers;

public sealed class ProviderHttpClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient httpClient;
    private readonly ModLogger logger;

    public ProviderHttpClient(HttpClient httpClient, ModLogger logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<(JsonDocument? Document, AiProviderError? Error, TimeSpan Duration)> PostJsonAsync(
        AiProviderType providerType,
        string url,
        object body,
        Action<HttpRequestMessage>? configure,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        using CancellationTokenSource timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(timeout);

        try
        {
            using HttpRequestMessage request = new(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(body, JsonOptions), Encoding.UTF8, "application/json")
            };
            configure?.Invoke(request);

            using HttpResponseMessage response = await this.httpClient.SendAsync(request, timeoutCts.Token).ConfigureAwait(false);
            string responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                AiProviderErrorKind kind = response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => AiProviderErrorKind.UnsupportedSettings,
                    HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => AiProviderErrorKind.Authentication,
                    HttpStatusCode.TooManyRequests => AiProviderErrorKind.RateLimited,
                    HttpStatusCode.NotFound => AiProviderErrorKind.Unavailable,
                    _ => AiProviderErrorKind.Unknown
                };

                string detail = ExtractErrorDetail(responseText);
                string diagnostic = string.IsNullOrWhiteSpace(detail)
                    ? $"{providerType} provider returned {(int)response.StatusCode} {response.StatusCode}."
                    : $"{providerType} provider returned {(int)response.StatusCode} {response.StatusCode}: {detail}";

                return (null, CreateError(providerType, kind, diagnostic, (int)response.StatusCode), stopwatch.Elapsed);
            }

            try
            {
                return (JsonDocument.Parse(responseText), null, stopwatch.Elapsed);
            }
            catch (JsonException ex)
            {
                return (null, CreateError(providerType, AiProviderErrorKind.MalformedResponse, $"Malformed JSON response: {ex.Message}"), stopwatch.Elapsed);
            }
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return (null, CreateError(providerType, AiProviderErrorKind.Timeout, $"Provider request timed out after {timeout.TotalSeconds:0}s."), stopwatch.Elapsed);
        }
        catch (HttpRequestException ex)
        {
            return (null, CreateError(providerType, AiProviderErrorKind.Unavailable, $"Provider unavailable: {SecretRedactor.Redact(ex.Message)}"), stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            return (null, CreateError(providerType, AiProviderErrorKind.Unknown, $"Provider request failed: {SecretRedactor.Redact(ex.Message)}"), stopwatch.Elapsed);
        }
    }

    public static void SetBearer(HttpRequestMessage request, string token)
    {
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public static AiProviderError CreateError(AiProviderType providerType, AiProviderErrorKind kind, string diagnostic, int? statusCode = null)
    {
        AiProviderError stub = new(kind, string.Empty, SecretRedactor.Redact(diagnostic), providerType, statusCode);
        return stub with { PlayerMessage = ErrorMessageMapper.ToPlayerMessage(stub) };
    }

    private static string ExtractErrorDetail(string responseText)
    {
        if (string.IsNullOrWhiteSpace(responseText))
            return string.Empty;

        try
        {
            using JsonDocument document = JsonDocument.Parse(responseText);
            JsonElement root = document.RootElement;

            string? message = root.GetPropertyOrNull("error")?.ValueKind switch
            {
                JsonValueKind.Object => root.GetPropertyOrNull("error")?.GetPropertyOrNull("message")?.GetString(),
                JsonValueKind.String => root.GetPropertyOrNull("error")?.GetString(),
                _ => null
            };

            message ??= root.GetPropertyOrNull("message")?.GetString();
            if (!string.IsNullOrWhiteSpace(message))
                return SecretRedactor.Redact(message);
        }
        catch (JsonException)
        {
            // Fall back to a short redacted body snippet below.
        }

        string redacted = SecretRedactor.Redact(responseText);
        return redacted.Length <= 600 ? redacted : redacted[..600] + "...";
    }
}
