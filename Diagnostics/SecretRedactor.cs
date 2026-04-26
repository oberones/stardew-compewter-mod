using System.Text.RegularExpressions;

namespace ComPewter.Diagnostics;

public static class SecretRedactor
{
    private static readonly Regex BearerTokenPattern = new("Bearer\\s+[A-Za-z0-9._\\-]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex KeyValuePattern = new("(api[_-]?key|token|authorization|x-api-key)(\\s*[:=]\\s*)([^\\s,;]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static string Redact(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        string redacted = BearerTokenPattern.Replace(value, "Bearer [redacted]");
        redacted = KeyValuePattern.Replace(redacted, "$1$2[redacted]");
        return redacted;
    }

    public static string RedactHeaderName(string? headerName)
    {
        if (string.IsNullOrWhiteSpace(headerName))
            return string.Empty;

        return headerName.Contains("auth", StringComparison.OrdinalIgnoreCase)
            || headerName.Contains("key", StringComparison.OrdinalIgnoreCase)
            || headerName.Contains("token", StringComparison.OrdinalIgnoreCase)
                ? "[redacted-header]"
                : headerName;
    }
}
