using OpenHardwarePlatform.Abstractions;

namespace OpenHardwarePlatform.Core;

/// <summary>
/// Default implementation of <see cref="IPluginContext"/>.
/// </summary>
public sealed class PluginContext : IPluginContext
{
    public PluginContext(IDeviceManager deviceManager, ILogSink log, string dataDirectory)
    {
        DeviceManager = deviceManager;
        Log = log;
        DataDirectory = dataDirectory;
    }

    /// <inheritdoc/>
    public IDeviceManager DeviceManager { get; }

    /// <inheritdoc/>
    public ILogSink Log { get; }

    /// <inheritdoc/>
    public string DataDirectory { get; }
}