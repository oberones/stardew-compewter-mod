namespace ComPewter.UI;

public sealed class ChatScrollState
{
    public int FirstVisibleLine { get; private set; }

    public void Scroll(int delta, int lineCount, int visibleCount)
    {
        int max = Math.Max(0, lineCount - visibleCount);
        this.FirstVisibleLine = Math.Clamp(this.FirstVisibleLine + delta, 0, max);
    }

    public void SnapToBottom(int lineCount, int visibleCount)
    {
        this.FirstVisibleLine = Math.Max(0, lineCount - visibleCount);
    }
}
