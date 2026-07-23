using System.Collections.Concurrent;
using OpenHardwarePlatform.Abstractions;

namespace OpenHardwarePlatform.Core;

/// <summary>
/// Default implementation of <see cref="IDeviceManager"/>.
/// Manages transport registration and device lifecycle.
/// </summary>
public sealed class DeviceManager : IDeviceManager, IAsyncDisposable
{
    private readonly ConcurrentDictionary<string, IDeviceTransport> _transports = new();
    private readonly ConcurrentDictionary<string, IDeviceConnection> _connections = new();
    private readonly ConcurrentDictionary<string, IDeviceInfo> _knownDevices = new();
    private readonly object _lock = new();
    private bool _disposed;
    private Timer? _hotPlugTimer;
    private static readonly TimeSpan HotPlugInterval = TimeSpan.FromSeconds(2);

    /// <inheritdoc/>
    public IReadOnlyList<IDeviceTransport> Transports
    {
        get
        {
            lock (_lock)
            {
                return _transports.Values.ToArray();
            }
        }
    }

    /// <inheritdoc/>
    public void RegisterTransport(IDeviceTransport transport)
    {
        ArgumentNullException.ThrowIfNull(transport);

        lock (_lock)
        {
            if (!_transports.TryAdd(transport.TransportName, transport))
            {
                throw new InvalidOperationException(
                    $"A transport with the name '{transport.TransportName}' is already registered.");
            }
        }

        transport.DeviceConnected += OnTransportDeviceConnected;
        transport.DeviceDisconnected += OnTransportDeviceDisconnected;
    }

    /// <inheritdoc/>
    public void UnregisterTransport(IDeviceTransport transport)
    {
        ArgumentNullException.ThrowIfNull(transport);

        lock (_lock)
        {
            _transports.TryRemove(transport.TransportName, out _);
        }

        transport.DeviceConnected -= OnTransportDeviceConnected;
        transport.DeviceDisconnected -= OnTransportDeviceDisconnected;
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<IDeviceInfo> EnumerateAllDevicesAsync(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        IReadOnlyList<IDeviceTransport> snapshot;
        lock (_lock)
        {
            snapshot = _transports.Values.ToArray();
        }

        foreach (var transport in snapshot)
        {
            await foreach (var device in transport.EnumerateDevicesAsync(cancellationToken))
            {
                yield return device;
            }
        }
    }

    /// <inheritdoc/>
    public async Task<IDeviceConnection> OpenDeviceAsync(
        IDeviceInfo deviceInfo, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(deviceInfo);

        IDeviceTransport? transport;
        lock (_lock)
        {
            _transports.TryGetValue(deviceInfo.TransportName, out transport);
        }

        if (transport is null)
        {
            throw new InvalidOperationException(
                $"No transport registered for '{deviceInfo.TransportName}'.");
        }

        var connection = await transport.OpenAsync(deviceInfo, cancellationToken);
        _connections.TryAdd(deviceInfo.DevicePath, connection);
        connection.Disconnected += (_, _) => _connections.TryRemove(deviceInfo.DevicePath, out _);
        return connection;
    }

    /// <inheritdoc/>
    public event EventHandler<IDeviceInfo>? DeviceConnected;

    /// <inheritdoc/>
    public event EventHandler<IDeviceInfo>? DeviceDisconnected;

    /// <summary>
    /// Starts periodic hot-plug polling to detect device changes.
    /// </summary>
    public void StartHotPlugMonitoring()
    {
        if (_hotPlugTimer != null) return;
        _hotPlugTimer = new Timer(async _ => await PollDevicesAsync(), null, TimeSpan.Zero, HotPlugInterval);
    }

    /// <summary>
    /// Stops hot-plug monitoring.
    /// </summary>
    public void StopHotPlugMonitoring()
    {
        _hotPlugTimer?.Dispose();
        _hotPlugTimer = null;
    }

    private async Task PollDevicesAsync()
    {
        if (_disposed) return;
        try
        {
            var currentDevices = new HashSet<string>();
            await foreach (var device in EnumerateAllDevicesAsync())
            {
                currentDevices.Add(device.DevicePath);
                if (!_knownDevices.ContainsKey(device.DevicePath))
                {
                    _knownDevices.TryAdd(device.DevicePath, device);
                    DeviceConnected?.Invoke(this, device);
                }
            }

            var removed = _knownDevices.Keys.Except(currentDevices).ToList();
            foreach (var path in removed)
            {
                if (_knownDevices.TryRemove(path, out var removedDevice))
                {
                    DeviceDisconnected?.Invoke(this, removedDevice);
                }
            }
        }
        catch
        {
            // Silently handle polling errors
        }
    }

    /// <summary>
    /// Releases all managed and unmanaged resources.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        StopHotPlugMonitoring();

        var connections = _connections.Values.ToArray();
        foreach (var connection in connections)
        {
            await connection.DisposeAsync();
        }

        var transports = _transports.Values.ToArray();
        foreach (var transport in transports)
        {
            await transport.DisposeAsync();
        }

        _connections.Clear();
        _transports.Clear();
        _knownDevices.Clear();
    }

    private void OnTransportDeviceConnected(object? sender, IDeviceInfo deviceInfo)
    {
        DeviceConnected?.Invoke(this, deviceInfo);
    }

    private void OnTransportDeviceDisconnected(object? sender, IDeviceInfo deviceInfo)
    {
        DeviceDisconnected?.Invoke(this, deviceInfo);
    }
}
