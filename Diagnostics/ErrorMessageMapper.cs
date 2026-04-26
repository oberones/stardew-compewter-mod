using ComPewter.Providers;

namespace ComPewter.Diagnostics;

public static class ErrorMessageMapper
{
    public static string ToPlayerMessage(AiProviderError error)
    {
        return error.Kind switch
        {
            AiProviderErrorKind.MissingConfiguration => "ComPewter needs a little setup before it can answer. Check your provider, model, and credentials in config.",
            AiProviderErrorKind.Authentication => "ComPewter could not sign in to the selected provider. Please check the configured key or token.",
            AiProviderErrorKind.RateLimited => "The selected provider is asking us to slow down. Try again in a little while.",
            AiProviderErrorKind.Timeout => "ComPewter waited too long for a reply. Try again, or increase the timeout in config.",
            AiProviderErrorKind.Unavailable => "ComPewter could not reach the selected provider. Check that it is running and reachable.",
            AiProviderErrorKind.MalformedResponse => "The provider replied in a format ComPewter could not understand.",
            AiProviderErrorKind.UnsupportedSettings => "The selected provider rejected ComPewter's request. Please check the configured model and endpoint, then see the SMAPI log for the provider's message.",
            _ => "ComPewter hit an unexpected provider problem. Check the SMAPI log for details."
        };
    }
}
