using ComPewter.Models;
using ComPewter.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using ChatMessage = ComPewter.Models.ChatMessage;

namespace ComPewter.UI;

public sealed class ComputerMenu : IClickableMenu
{
    private readonly IAiProvider provider;
    private readonly List<ChatMessage> messages = new();
    private string input = string.Empty;
    private bool waitingForResponse;

    public ComputerMenu(IAiProvider provider)
        : base(
            Game1.uiViewport.Width / 2 - 400,
            Game1.uiViewport.Height / 2 - 300,
            800,
            600,
            showUpperRightCloseButton: true)
    {
        this.provider = provider;
    }

    public override void receiveKeyPress(Keys key)
    {
        if (key == Keys.Escape)
        {
            Game1.exitActiveMenu();
            return;
        }

        if (key == Keys.Enter && !this.waitingForResponse)
        {
            _ = this.SubmitQuestionAsync();
            return;
        }

        if (this.waitingForResponse)
            return;

        if (key == Keys.Back && this.input.Length > 0)
        {
            this.input = this.input[..^1];
            return;
        }

        char? typed = ConvertKeyToChar(key);
        if (typed is not null)
            this.input += typed.Value;

        base.receiveKeyPress(key);
    }

    public override void draw(SpriteBatch b)
    {
        IClickableMenu.drawTextureBox(b, this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.White);

        Vector2 cursor = new(this.xPositionOnScreen + 32, this.yPositionOnScreen + 32);
        Utility.drawTextWithShadow(b, "ComPewter", Game1.dialogueFont, cursor, Game1.textColor);
        cursor.Y += 64;

        foreach (ChatMessage message in this.messages.TakeLast(8))
        {
            string label = message.Role == ChatRole.User ? "You" : "ComPewter";
            string text = Game1.parseText($"{label}: {message.Content}", Game1.smallFont, this.width - 64);
            Utility.drawTextWithShadow(b, text, Game1.smallFont, cursor, Game1.textColor);
            cursor.Y += Game1.smallFont.MeasureString(text).Y + 12;
        }

        string prompt = this.waitingForResponse ? "Thinking..." : $"> {this.input}_";
        Utility.drawTextWithShadow(
            b,
            Game1.parseText(prompt, Game1.smallFont, this.width - 64),
            Game1.smallFont,
            new Vector2(this.xPositionOnScreen + 32, this.yPositionOnScreen + this.height - 88),
            Game1.textColor);

        this.drawMouse(b);
    }

    private async Task SubmitQuestionAsync()
    {
        string question = this.input.Trim();
        if (question.Length == 0)
            return;

        this.input = string.Empty;
        this.waitingForResponse = true;
        this.messages.Add(new ChatMessage(ChatRole.User, question));

        try
        {
            string answer = await this.provider.AskAsync(this.messages);
            this.messages.Add(new ChatMessage(ChatRole.Assistant, answer));
        }
        catch (Exception ex)
        {
            this.messages.Add(new ChatMessage(ChatRole.Assistant, $"I could not reach the configured AI provider: {ex.Message}"));
        }
        finally
        {
            this.waitingForResponse = false;
        }
    }

    private static char? ConvertKeyToChar(Keys key)
    {
        bool shift = Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift);

        if (key is >= Keys.A and <= Keys.Z)
        {
            char letter = (char)('a' + key - Keys.A);
            return shift ? char.ToUpperInvariant(letter) : letter;
        }

        if (key is >= Keys.D0 and <= Keys.D9)
            return (char)('0' + key - Keys.D0);

        if (key is >= Keys.NumPad0 and <= Keys.NumPad9)
            return (char)('0' + key - Keys.NumPad0);

        return key switch
        {
            Keys.Space => ' ',
            Keys.OemPeriod => '.',
            Keys.OemComma => ',',
            Keys.OemQuestion => '?',
            Keys.OemMinus => '-',
            Keys.OemPlus => '+',
            Keys.OemSemicolon => ';',
            Keys.OemQuotes => '\'',
            _ => null
        };
    }
}
