namespace OpenHardwarePlatform.Abstractions;

/// <summary>
/// Provides the runtime context for a plugin to interact with the platform.
/// </summary>
public interface IPluginContext
{
    /// <summary>
    /// Gets the device manager for discovering and connecting to devices.
    /// </summary>
    IDeviceManager DeviceManager { get; }

    /// <summary>
    /// Gets the logging sink for this plugin.
    /// </summary>
    ILogSink Log { get; }

    /// <summary>
    /// Gets a directory path where the plugin can store persistent data.
    /// </summary>
    string DataDirectory { get; }
}