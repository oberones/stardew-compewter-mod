using ComPewter.Config;
using ComPewter.Models;
using ComPewter.Providers;
using ComPewter.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ComPewter;

public sealed class ModEntry : Mod
{
    private ModConfig config = new();
    private IAiProvider provider = null!;

    public override void Entry(IModHelper helper)
    {
        this.config = helper.ReadConfig<ModConfig>();
        this.provider = ProviderFactory.Create(this.config);

        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        helper.ConsoleCommands.Add("compewter", "Ask ComPewter a question. Usage: compewter <question>", this.HandleConsoleCommand);
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady || !e.Button.Equals(this.config.OpenMenuKey))
            return;

        Game1.activeClickableMenu = new ComputerMenu(this.provider);
    }

    private async void HandleConsoleCommand(string command, string[] args)
    {
        if (args.Length == 0)
        {
            this.Monitor.Log("Usage: compewter <question>", LogLevel.Info);
            return;
        }

        string question = string.Join(' ', args);
        try
        {
            string response = await this.provider.AskAsync(new[]
            {
                new ChatMessage(ChatRole.System, this.config.SystemPrompt),
                new ChatMessage(ChatRole.User, question)
            });

            this.Monitor.Log(response, LogLevel.Info);
        }
        catch (Exception ex)
        {
            this.Monitor.Log($"ComPewter failed to answer: {ex}", LogLevel.Error);
        }
    }
}
