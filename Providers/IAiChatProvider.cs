using ComPewter.Config;

namespace ComPewter.Providers;

public interface IAiChatProvider
{
    AiProviderType Type { get; }

    ProviderValidationResult Validate(ProviderSettings settings);

    Task<AiChatResult> SendAsync(AiChatRequest request, CancellationToken cancellationToken);
}
