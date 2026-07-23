namespace OpenHardwarePlatform.Abstractions;

/// <summary>
/// A logging sink for structured diagnostic output.
/// </summary>
public interface ILogSink
{
    /// <summary>
    /// Writes a trace-level log entry.
    /// </summary>
    void Trace(string message);

    /// <summary>
    /// Writes a debug-level log entry.
    /// </summary>
    void Debug(string message);

    /// <summary>
    /// Writes an informational log entry.
    /// </summary>
    void Info(string message);

    /// <summary>
    /// Writes a warning log entry.
    /// </summary>
    void Warn(string message);

    /// <summary>
    /// Writes an error log entry.
    /// </summary>
    void Error(string message);

    /// <summary>
    /// Writes a critical log entry with optional exception.
    /// </summary>
    void Critical(string message, Exception? exception = null);
}