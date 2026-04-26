namespace ComPewter.Providers;

public sealed record ProviderValidationResult(bool IsAvailable, string? Message)
{
    public static ProviderValidationResult Available() => new(true, null);

    public static ProviderValidationResult Unavailable(string message) => new(false, message);
}
