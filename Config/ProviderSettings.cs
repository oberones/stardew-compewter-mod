namespace ComPewter.Config;

public sealed class ProviderSettings
{
    public string BaseUrl { get; set; } = string.Empty;

    public string EndpointPath { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string AuthHeaderName { get; set; } = string.Empty;

    public string AuthToken { get; set; } = string.Empty;

    public string RequestFormat { get; set; } = "OpenAICompatible";
}
