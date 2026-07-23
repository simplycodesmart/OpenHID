namespace OpenHardwarePlatform.Abstractions;

/// <summary>
/// Manages device discovery, connection lifecycle, and transport registration.
/// </summary>
public interface IDeviceManager
{
    /// <summary>
    /// Gets all registered transport providers.
    /// </summary>
    IReadOnlyList<IDeviceTransport> Transports { get; }

    /// <summary>
    /// Registers a transport provider with the manager.
    /// </summary>
    void RegisterTransport(IDeviceTransport transport);

    /// <summary>
    /// Unregisters a transport provider.
    /// </summary>
    void UnregisterTransport(IDeviceTransport transport);

    /// <summary>
    /// Enumerates all devices across all registered transports.
    /// </summary>
    IAsyncEnumerable<IDeviceInfo> EnumerateAllDevicesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens a connection to the specified device.
    /// </summary>
    Task<IDeviceConnection> OpenDeviceAsync(IDeviceInfo deviceInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Raised when any device is connected.
    /// </summary>
    event EventHandler<IDeviceInfo>? DeviceConnected;

    /// <summary>
    /// Raised when any device is disconnected.
    /// </summary>
    event EventHandler<IDeviceInfo>? DeviceDisconnected;
}