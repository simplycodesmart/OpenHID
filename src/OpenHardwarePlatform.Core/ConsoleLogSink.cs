using OpenHardwarePlatform.Abstractions;

namespace OpenHardwarePlatform.Core;

/// <summary>
/// A log sink that writes to <see cref="Console"/> with colored output.
/// </summary>
public sealed class ConsoleLogSink : ILogSink
{
    /// <inheritdoc/>
    public void Trace(string message) => WriteLine("TRCE", ConsoleColor.DarkGray, message);

    /// <inheritdoc/>
    public void Debug(string message) => WriteLine("DBUG", ConsoleColor.Gray, message);

    /// <inheritdoc/>
    public void Info(string message) => WriteLine("INFO", ConsoleColor.White, message);

    /// <inheritdoc/>
    public void Warn(string message) => WriteLine("WARN", ConsoleColor.Yellow, message);

    /// <inheritdoc/>
    public void Error(string message) => WriteLine("ERRO", ConsoleColor.Red, message);

    /// <inheritdoc/>
    public void Critical(string message, Exception? exception = null)
    {
        WriteLine("CRIT", ConsoleColor.DarkRed, message);
        if (exception is not null)
        {
            WriteLine("CRIT", ConsoleColor.DarkRed, exception.ToString());
        }
    }

    private static void WriteLine(string level, ConsoleColor color, string message)
    {
        var original = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine($"[{level}] {DateTimeOffset.Now:HH:mm:ss.fff} {message}");
        Console.ForegroundColor = original;
    }
}