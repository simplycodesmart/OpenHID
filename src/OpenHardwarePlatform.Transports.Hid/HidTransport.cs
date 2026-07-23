using OpenHardwarePlatform.Abstractions;

namespace OpenHardwarePlatform.Transports.Hid;

/// <summary>
/// HID transport implementation using HidSharp.
/// </summary>
public sealed class HidTransport : IDeviceTransport
{
    private readonly HidSharp.HidDeviceLoader _loader;
    private bool _disposed;

    public HidTransport()
    {
        _loader = new HidSharp.HidDeviceLoader();
    }

    /// <inheritdoc/>
    public string TransportName => "HID";

    /// <inheritdoc/>
    public async IAsyncEnumerable<IDeviceInfo> EnumerateDevicesAsync(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var devices = _loader.GetDevices();
        foreach (var device in devices)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return new HidDeviceInfo(device);
        }

        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task<IDeviceConnection> OpenAsync(IDeviceInfo deviceInfo, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(deviceInfo);

        if (deviceInfo is not HidDeviceInfo hidInfo)
        {
            throw new ArgumentException($"Expected a HID device info. Got '{deviceInfo.GetType().Name}'.");
        }

        var stream = hidInfo.NativeDevice.Open();
        stream.ReadTimeout = Timeout.Infinite; // Blocking read
        var connection = new HidConnection(hidInfo, stream);

        return await Task.FromResult(connection);
    }

    /// <inheritdoc/>
    public event EventHandler<IDeviceInfo>? DeviceConnected;

    /// <inheritdoc/>
    public event EventHandler<IDeviceInfo>? DeviceDisconnected;

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        await Task.CompletedTask;
    }
}