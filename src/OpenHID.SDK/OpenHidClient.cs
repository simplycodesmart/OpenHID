using OpenHardwarePlatform.Abstractions;
using OpenHardwarePlatform.Core;
using OpenHardwarePlatform.Protocols;
using OpenHardwarePlatform.Transports.Hid;

namespace OpenHID.SDK;

/// <summary>
/// High-level SDK for OpenHID. Provides a simple API for HID device operations.
/// </summary>
public sealed class OpenHidClient : IAsyncDisposable
{
    private readonly DeviceManager _deviceManager;
    private readonly ProtocolRegistry _protocolRegistry;
    private bool _disposed;

    public OpenHidClient()
    {
        _deviceManager = new DeviceManager();
        _deviceManager.RegisterTransport(new HidTransport());
        _protocolRegistry = new ProtocolRegistry();
    }

    /// <summary>
    /// Enumerates all connected HID devices.
    /// </summary>
    public async Task<IReadOnlyList<IDeviceInfo>> ListDevicesAsync()
    {
        var devices = new List<IDeviceInfo>();
        await foreach (var device in _deviceManager.EnumerateAllDevicesAsync())
        {
            devices.Add(device);
        }
        return devices;
    }

    /// <summary>
    /// Opens a connection to a device.
    /// </summary>
    public async Task<IDeviceConnection> ConnectAsync(IDeviceInfo deviceInfo)
    {
        return await _deviceManager.OpenDeviceAsync(deviceInfo);
    }

    /// <summary>
    /// Decodes a raw HID report using the protocol registry.
    /// </summary>
    public DecodedReport DecodeReport(int vendorId, int productId, byte[] report)
    {
        return _protocolRegistry.Decode(vendorId, productId, report);
    }

    /// <summary>
    /// Starts hot-plug monitoring.
    /// </summary>
    public void StartMonitoring() => _deviceManager.StartHotPlugMonitoring();

    /// <summary>
    /// Stops hot-plug monitoring.
    /// </summary>
    public void StopMonitoring() => _deviceManager.StopHotPlugMonitoring();

    /// <summary>
    /// Fired when a device is connected.
    /// </summary>
    public event EventHandler<IDeviceInfo>? DeviceConnected
    {
        add => _deviceManager.DeviceConnected += value;
        remove => _deviceManager.DeviceConnected -= value;
    }

    /// <summary>
    /// Fired when a device is disconnected.
    /// </summary>
    public event EventHandler<IDeviceInfo>? DeviceDisconnected
    {
        add => _deviceManager.DeviceDisconnected += value;
        remove => _deviceManager.DeviceDisconnected -= value;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        await _deviceManager.DisposeAsync();
    }
}