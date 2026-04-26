using ComPewter.Config;
using ComPewter.Context;
using ComPewter.Diagnostics;
using ComPewter.Prompts;
using ComPewter.Providers;

namespace ComPewter.Sessions;

public sealed class ChatSessionManager
{
    private readonly ModConfig config;
    private readonly AiProviderFactory providerFactory;
    private readonly PromptBuilder promptBuilder;
    private readonly GameContextService contextService;
    private readonly ModLogger logger;
    private readonly ChatSession session = new();

    public ChatSessionManager(
        ModConfig config,
        AiProviderFactory providerFactory,
        PromptBuilder promptBuilder,
        GameContextService contextService,
        ModLogger logger)
    {
        this.config = config;
        this.providerFactory = providerFactory;
        this.promptBuilder = promptBuilder;
        this.contextService = contextService;
        this.logger = logger;

        if (this.config.Privacy.ShareGameContext)
        {
            this.session.Add(new ChatMessage(
                ChatRole.Assistant,
                "Context sharing is on. ComPewter may send relevant game details like season, weather, money, skills, location, inventory, quests, and progression to your selected provider. Set Privacy.ShareGameContext to false in config to disable it.",
                ChatMessageStatus.Info));
        }
    }

    public IReadOnlyList<ChatMessage> Messages => this.session.Messages;

    public bool IsRequestInFlight => this.session.IsRequestInFlight;

    public event Action? Changed;

    public async Task SubmitAsync(string question, CancellationToken cancellationToken)
    {
        question = question.Trim();
        if (question.Length == 0 || this.session.IsRequestInFlight)
            return;

        this.session.Add(new ChatMessage(ChatRole.User, question));
        this.session.Add(new ChatMessage(ChatRole.Assistant, "Thinking...", ChatMessageStatus.Pending));
        this.session.IsRequestInFlight = true;
        this.NotifyChanged();

        try
        {
            IAiChatProvider provider = this.providerFactory.Create(this.config.Provider);
            ProviderSettings settings = this.ResolveSettings(this.config.Provider);
            ProviderValidationResult validation = provider.Validate(settings);
            if (!validation.IsAvailable)
            {
                this.SetError(validation.Message);
                return;
            }

            GameContextSnapshot? context = this.config.Privacy.ShareGameContext
                ? this.contextService.Collect()
                : null;

            int currentQuestionIndex = Math.Max(0, this.session.Messages.Count - 2);
            IReadOnlyList<ChatMessage> history = this.config.Privacy.RetainConversationHistory
                ? this.session.RecentHistoryBefore(currentQuestionIndex, Math.Max(0, this.config.MaxRetainedMessages - 2))
                : Array.Empty<ChatMessage>();

            IReadOnlyList<ChatMessage> promptMessages = this.promptBuilder.Build(new PromptOptions(
                question,
                this.config.Privacy.AllowSpoilers,
                this.config.RestrictToStardewTopics,
                this.config.Privacy.ShareGameContext,
                context,
                history));

            AiChatRequest request = new(
                this.config.Provider,
                settings.Model,
                promptMessages,
                this.config.MaxResponseTokens,
                TimeSpan.FromSeconds(this.config.RequestTimeoutSeconds));

            AiChatResult result = await provider.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (result.Success && !string.IsNullOrWhiteSpace(result.Content))
            {
                this.session.ReplaceLast(new ChatMessage(ChatRole.Assistant, result.Content.Trim()));
                this.logger.Debug($"{this.config.Provider} answered in {result.Duration.TotalMilliseconds:0}ms.");
            }
            else
            {
                this.SetError(result.Error?.PlayerMessage ?? "ComPewter could not get a useful answer from the selected provider.");
                if (result.Error is not null)
                    this.logger.Warn(result.Error.DiagnosticMessage);
            }
        }
        catch (OperationCanceledException)
        {
            this.SetError("ComPewter stopped waiting for that answer.");
        }
        catch (Exception ex)
        {
            this.SetError("ComPewter ran into an unexpected problem. Check the SMAPI log for details.");
            this.logger.Error($"Unexpected chat failure: {ex}");
        }
        finally
        {
            this.session.IsRequestInFlight = false;
            this.session.EnforceCap(this.config.MaxRetainedMessages);
            this.NotifyChanged();
        }
    }

    private ProviderSettings ResolveSettings(AiProviderType providerType)
    {
        return providerType switch
        {
            AiProviderType.Anthropic => this.config.Anthropic,
            AiProviderType.OpenAI => this.config.OpenAI,
            AiProviderType.Ollama => this.config.Ollama,
            AiProviderType.Custom => this.config.Custom,
            _ => new ProviderSettings()
        };
    }

    private void SetError(string? message)
    {
        this.session.ReplaceLast(new ChatMessage(
            ChatRole.Assistant,
            string.IsNullOrWhiteSpace(message) ? "ComPewter needs a little setup before it can answer." : message,
            ChatMessageStatus.Error));
    }

    private void NotifyChanged()
    {
        this.Changed?.Invoke();
    }
}
