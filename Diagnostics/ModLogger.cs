using StardewModdingAPI;

namespace ComPewter.Diagnostics;

public sealed class ModLogger
{
    private readonly IMonitor monitor;
    private readonly Func<bool> debugEnabled;

    public ModLogger(IMonitor monitor, Func<bool> debugEnabled)
    {
        this.monitor = monitor;
        this.debugEnabled = debugEnabled;
    }

    public void Info(string message)
    {
        this.monitor.Log(SecretRedactor.Redact(message), LogLevel.Info);
    }

    public void Warn(string message)
    {
        this.monitor.Log(SecretRedactor.Redact(message), LogLevel.Warn);
    }

    public void Error(string message)
    {
        this.monitor.Log(SecretRedactor.Redact(message), LogLevel.Error);
    }

    public void Debug(string message)
    {
        if (this.debugEnabled())
            this.monitor.Log(SecretRedactor.Redact(message), LogLevel.Debug);
    }
}
