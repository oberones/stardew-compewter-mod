using Microsoft.Xna.Framework.Input;

namespace ComPewter.UI;

public sealed class ChatTextInput
{
    public string Text { get; private set; } = string.Empty;

    public void Clear()
    {
        this.Text = string.Empty;
    }

    public void ReceiveKey(Keys key)
    {
        if (key == Keys.Back && this.Text.Length > 0)
        {
            this.Text = this.Text[..^1];
            return;
        }

        char? character = ConvertKeyToChar(key);
        if (character is not null && this.Text.Length < 500)
            this.Text += character.Value;
    }

    private static char? ConvertKeyToChar(Keys key)
    {
        KeyboardState state = Keyboard.GetState();
        bool shift = state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift);

        if (key is >= Keys.A and <= Keys.Z)
        {
            char letter = (char)('a' + key - Keys.A);
            return shift ? char.ToUpperInvariant(letter) : letter;
        }

        if (key is >= Keys.D0 and <= Keys.D9)
        {
            char digit = (char)('0' + key - Keys.D0);
            if (!shift)
                return digit;

            return digit switch
            {
                '1' => '!',
                '2' => '@',
                '3' => '#',
                '4' => '$',
                '5' => '%',
                '6' => '^',
                '7' => '&',
                '8' => '*',
                '9' => '(',
                '0' => ')',
                _ => digit
            };
        }

        if (key is >= Keys.NumPad0 and <= Keys.NumPad9)
            return (char)('0' + key - Keys.NumPad0);

        return key switch
        {
            Keys.Space => ' ',
            Keys.OemPeriod => shift ? '>' : '.',
            Keys.OemComma => shift ? '<' : ',',
            Keys.OemQuestion => shift ? '?' : '/',
            Keys.OemMinus => shift ? '_' : '-',
            Keys.OemPlus => shift ? '+' : '=',
            Keys.OemSemicolon => shift ? ':' : ';',
            Keys.OemQuotes => shift ? '"' : '\'',
            Keys.OemOpenBrackets => shift ? '{' : '[',
            Keys.OemCloseBrackets => shift ? '}' : ']',
            Keys.OemPipe => shift ? '|' : '\\',
            _ => null
        };
    }
}
