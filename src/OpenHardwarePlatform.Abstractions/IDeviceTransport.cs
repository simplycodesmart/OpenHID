namespace OpenHardwarePlatform.Abstractions;

/// <summary>
/// Represents a transport layer for communicating with hardware devices.
/// </summary>
public interface IDeviceTransport : IAsyncDisposable
{
    /// <summary>
    /// Gets the display name of this transport (e.g. "HID", "USB", "Bluetooth").
    /// </summary>
    string TransportName { get; }

    /// <summary>
    /// Enumerates all currently connected devices available through this transport.
    /// </summary>
    IAsyncEnumerable<IDeviceInfo> EnumerateDevicesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens a connection to the specified device.
    /// </summary>
    Task<IDeviceConnection> OpenAsync(IDeviceInfo deviceInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Raised when a device is connected.
    /// </summary>
    event EventHandler<IDeviceInfo>? DeviceConnected;

    /// <summary>
    /// Raised when a device is disconnected.
    /// </summary>
    event EventHandler<IDeviceInfo>? DeviceDisconnected;
}