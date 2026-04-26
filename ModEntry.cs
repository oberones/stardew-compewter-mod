using ComPewter.Config;
using ComPewter.Context;
using ComPewter.Diagnostics;
using ComPewter.Interaction;
using ComPewter.Persistence;
using ComPewter.Prompts;
using ComPewter.Providers;
using ComPewter.Sessions;
using ComPewter.UI;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ComPewter;

public sealed class ModEntry : Mod
{
    private ModConfig config = new();
    private ModLogger logger = null!;
    private ChatSessionManager chatSessionManager = null!;
    private HotkeyComputerInteraction hotkeyInteraction = null!;
    private SaveDataService saveDataService = null!;

    public override void Entry(IModHelper helper)
    {
        this.logger = new ModLogger(this.Monitor, () => false);

        ConfigValidator validator = new(this.Monitor);
        this.config = validator.Normalize(helper.ReadConfig<ModConfig>());
        helper.WriteConfig(this.config);

        this.logger = new ModLogger(this.Monitor, () => this.config.DebugLogging);
        ProviderRuntime.Settings = this.config;

        ProviderHttpClient providerHttpClient = new(new HttpClient(), this.logger);
        AiProviderFactory providerFactory = new(providerHttpClient, this.logger);
        PromptBuilder promptBuilder = new();
        GameContextService contextService = new(helper, this.config, this.logger);

        this.chatSessionManager = new ChatSessionManager(this.config, providerFactory, promptBuilder, contextService, this.logger);
        this.hotkeyInteraction = new HotkeyComputerInteraction(this.config);
        this.saveDataService = new SaveDataService();

        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.Saving += this.OnSaving;
        helper.ConsoleCommands.Add("compewter", "Ask ComPewter a question. Usage: compewter <question>", this.HandleConsoleCommand);

        GenericModConfigMenuIntegration.Register(helper, this.Monitor, () => this.config, _ => helper.WriteConfig(this.config));
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (Game1.activeClickableMenu is ChatMenu chatMenu && TryGetKeyboardKey(e.Button, out Keys key))
        {
            chatMenu.ReceiveSuppressedKeyPress(key);
            this.Helper.Input.Suppress(e.Button);
            return;
        }

        if (!this.hotkeyInteraction.ShouldOpen(e.Button))
            return;

        this.Helper.Input.Suppress(e.Button);
        Game1.activeClickableMenu = new ChatMenu(this.chatSessionManager, this.config);
    }

    private static bool TryGetKeyboardKey(SButton button, out Keys key)
    {
        return Enum.TryParse(button.ToString(), out key);
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        this.saveDataService.Load();
    }

    private void OnSaving(object? sender, SavingEventArgs e)
    {
        this.saveDataService.Save();
    }

    private async void HandleConsoleCommand(string command, string[] args)
    {
        if (args.Length == 0)
        {
            this.Monitor.Log("Usage: compewter <question>", LogLevel.Info);
            return;
        }

        string question = string.Join(' ', args);
        await this.chatSessionManager.SubmitAsync(question, CancellationToken.None).ConfigureAwait(false);
        string? lastAnswer = this.chatSessionManager.Messages.LastOrDefault(message => message.Role == ChatRole.Assistant)?.Content;
        if (!string.IsNullOrWhiteSpace(lastAnswer))
            this.Monitor.Log(lastAnswer, LogLevel.Info);
    }
}
