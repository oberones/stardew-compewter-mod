using ComPewter.Config;
using ComPewter.Sessions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using ChatMessage = ComPewter.Sessions.ChatMessage;

namespace ComPewter.UI;

public sealed class ChatMenu : IClickableMenu
{
    private const int Padding = 32;
    private const int ScrollLinesPerWheel = 3;

    private readonly ChatSessionManager sessionManager;
    private readonly ModConfig config;
    private readonly ChatTextInput input = new();
    private readonly ChatScrollState scroll = new();
    private readonly CancellationTokenSource closeCts = new();

    public ChatMenu(ChatSessionManager sessionManager, ModConfig config)
        : base(
            Game1.uiViewport.Width / 2 - 430,
            Game1.uiViewport.Height / 2 - 310,
            860,
            620,
            showUpperRightCloseButton: true)
    {
        this.sessionManager = sessionManager;
        this.config = config;
        this.sessionManager.Changed += this.OnSessionChanged;
        this.SnapScrollToBottom();
    }

    protected override void cleanupBeforeExit()
    {
        this.closeCts.Cancel();
        this.closeCts.Dispose();
        this.sessionManager.Changed -= this.OnSessionChanged;
        base.cleanupBeforeExit();
    }

    public override void receiveKeyPress(Keys key)
    {
        this.ReceiveSuppressedKeyPress(key);
    }

    public void ReceiveSuppressedKeyPress(Keys key)
    {
        if (key == Keys.Escape)
        {
            Game1.exitActiveMenu();
            return;
        }

        if (key == Keys.Enter)
        {
            this.Submit();
            return;
        }

        if (key == Keys.Up || key == Keys.PageUp)
        {
            this.ScrollMessages(key == Keys.PageUp ? -8 : -1);
            return;
        }

        if (key == Keys.Down || key == Keys.PageDown)
        {
            this.ScrollMessages(key == Keys.PageDown ? 8 : 1);
            return;
        }

        if (!this.sessionManager.IsRequestInFlight)
            this.input.ReceiveKey(key);
    }

    public override void receiveScrollWheelAction(int direction)
    {
        this.ScrollMessages(direction > 0 ? -ScrollLinesPerWheel : ScrollLinesPerWheel);
    }

    public override void draw(SpriteBatch b)
    {
        IClickableMenu.drawTextureBox(b, this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.White);

        Vector2 cursor = new(this.xPositionOnScreen + Padding, this.yPositionOnScreen + Padding);
        Utility.drawTextWithShadow(b, "ComPewter", Game1.dialogueFont, cursor, Game1.textColor);

        int messagesTop = this.DrawHeaderAndHint(b, cursor);

        this.DrawMessages(b, messagesTop);
        this.DrawInput(b);
        this.drawMouse(b);
    }

    private int DrawHeaderAndHint(SpriteBatch b, Vector2 cursor)
    {
        if (this.config.Privacy.ShareGameContext || !this.config.Ui.ShowContextEnableHint)
            return this.yPositionOnScreen + 116;

        string hint = "Context sharing is off. Enable Privacy.ShareGameContext in config for season, weather, inventory, and daily planning help.";
        string wrappedHint = Game1.parseText(hint, Game1.smallFont, this.width - (Padding * 2));
        Vector2 hintPosition = new(cursor.X, cursor.Y + 56);
        Utility.drawTextWithShadow(b, wrappedHint, Game1.smallFont, hintPosition, Color.DarkSlateGray);

        return (int)(hintPosition.Y + Game1.smallFont.MeasureString(wrappedHint).Y + 18);
    }

    private void DrawMessages(SpriteBatch b, int top)
    {
        int bottom = this.yPositionOnScreen + this.height - 110;
        int lineHeight = Math.Max(28, (int)Game1.smallFont.MeasureString("Ay").Y);
        int visibleLines = this.GetVisibleLineCount(top, bottom, lineHeight);
        IReadOnlyList<DisplayLine> lines = this.BuildDisplayLines();
        int start = Math.Clamp(this.scroll.FirstVisibleLine, 0, Math.Max(0, lines.Count - 1));
        Vector2 cursor = new(this.xPositionOnScreen + Padding, top);

        foreach (DisplayLine line in lines.Skip(start).Take(visibleLines))
        {
            if (cursor.Y + lineHeight > bottom)
                break;

            if (line.Text.Length > 0)
                Utility.drawTextWithShadow(b, line.Text, Game1.smallFont, cursor, line.Color);

            cursor.Y += lineHeight;
        }
    }

    private void DrawInput(SpriteBatch b)
    {
        Rectangle inputBox = new(
            this.xPositionOnScreen + Padding,
            this.yPositionOnScreen + this.height - 82,
            this.width - (Padding * 2),
            50);

        IClickableMenu.drawTextureBox(b, inputBox.X, inputBox.Y, inputBox.Width, inputBox.Height, Color.White);
        string prompt = this.sessionManager.IsRequestInFlight ? "Waiting for an answer..." : $"> {this.input.Text}{this.GetBlinkingCursor()}";
        Utility.drawTextWithShadow(
            b,
            Game1.parseText(prompt, Game1.smallFont, inputBox.Width - 24),
            Game1.smallFont,
            new Vector2(inputBox.X + 12, inputBox.Y + 12),
            Game1.textColor);
    }

    private string GetBlinkingCursor()
    {
        double milliseconds = Game1.currentGameTime?.TotalGameTime.TotalMilliseconds ?? 0;
        return ((int)(milliseconds / 500) % 2) == 0 ? "_" : " ";
    }

    private void Submit()
    {
        if (this.sessionManager.IsRequestInFlight)
            return;

        string question = this.input.Text;
        this.input.Clear();
        this.SnapScrollToBottom();
        _ = this.sessionManager.SubmitAsync(question, this.closeCts.Token);
    }

    private void OnSessionChanged()
    {
        this.SnapScrollToBottom();
    }

    private void ScrollMessages(int delta)
    {
        int top = this.GetMessagesTop();
        int bottom = this.yPositionOnScreen + this.height - 110;
        int lineHeight = Math.Max(28, (int)Game1.smallFont.MeasureString("Ay").Y);
        this.scroll.Scroll(delta, this.BuildDisplayLines().Count, this.GetVisibleLineCount(top, bottom, lineHeight));
    }

    private void SnapScrollToBottom()
    {
        int top = this.GetMessagesTop();
        int bottom = this.yPositionOnScreen + this.height - 110;
        int lineHeight = Math.Max(28, (int)Game1.smallFont.MeasureString("Ay").Y);
        this.scroll.SnapToBottom(this.BuildDisplayLines().Count, this.GetVisibleLineCount(top, bottom, lineHeight));
    }

    private int GetMessagesTop()
    {
        int top = this.yPositionOnScreen + 116;
        if (!this.config.Privacy.ShareGameContext && this.config.Ui.ShowContextEnableHint)
        {
            string hint = "Context sharing is off. Enable Privacy.ShareGameContext in config for season, weather, inventory, and daily planning help.";
            string wrappedHint = Game1.parseText(hint, Game1.smallFont, this.width - (Padding * 2));
            top = (int)(this.yPositionOnScreen + Padding + 56 + Game1.smallFont.MeasureString(wrappedHint).Y + 18);
        }

        return top;
    }

    private int GetVisibleLineCount(int top, int bottom, int lineHeight)
    {
        return Math.Max(1, (bottom - top) / lineHeight);
    }

    private IReadOnlyList<DisplayLine> BuildDisplayLines()
    {
        List<DisplayLine> lines = new();
        int wrapWidth = this.width - (Padding * 2);
        foreach (ChatMessage message in this.sessionManager.Messages)
        {
            string label = message.Role == ChatRole.User ? "You" : "ComPewter";
            Color color = message.Status == ChatMessageStatus.Error ? Color.DarkRed : Game1.textColor;
            if (message.Status == ChatMessageStatus.Pending)
                color = Color.DarkSlateGray;
            if (message.Status == ChatMessageStatus.Info)
                color = Color.DarkSlateGray;

            string wrapped = Game1.parseText($"{label}: {message.Content}", Game1.smallFont, wrapWidth);
            foreach (string line in wrapped.Replace("\r\n", "\n").Split('\n'))
                lines.Add(new DisplayLine(line, color));

            lines.Add(new DisplayLine(string.Empty, color));
        }

        if (lines.Count > 0 && lines[^1].Text.Length == 0)
            lines.RemoveAt(lines.Count - 1);

        return lines;
    }

    private sealed record DisplayLine(string Text, Color Color);
}
