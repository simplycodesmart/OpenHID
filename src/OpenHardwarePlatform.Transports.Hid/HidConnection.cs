using OpenHardwarePlatform.Abstractions;

namespace OpenHardwarePlatform.Transports.Hid;

/// <summary>
/// Wraps a <see cref="HidSharp.HidStream"/> as an <see cref="IDeviceConnection"/>.
/// </summary>
internal sealed class HidConnection : IDeviceConnection
{
    private readonly HidSharp.HidStream _stream;
    private readonly HidDeviceInfo _deviceInfo;
    private readonly int _maxInputLength;
    private bool _disposed;

    public HidConnection(HidDeviceInfo deviceInfo, HidSharp.HidStream stream)
    {
        _deviceInfo = deviceInfo;
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        _maxInputLength = stream.Device.MaxInputReportLength;
        IsConnected = true;

        // Start background read loop
        _ = Task.Run(ReadLoopAsync);
    }

    /// <inheritdoc/>
    public IDeviceInfo DeviceInfo => _deviceInfo;

    /// <inheritdoc/>
    public bool IsConnected { get; private set; }

    /// <inheritdoc/>
    public Task<ReadResult> ReadAsync(CancellationToken cancellationToken = default)
    {
        var buffer = new byte[_maxInputLength];
        var bytesRead = _stream.Read(buffer);

        var data = bytesRead > 0
            ? new ReadOnlyMemory<byte>(buffer, 0, bytesRead)
            : ReadOnlyMemory<byte>.Empty;

        return Task.FromResult(new ReadResult(buffer[0], data, DateTimeOffset.UtcNow));
    }

    /// <inheritdoc/>
    public Task WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var array = data.ToArray();
        _stream.Write(array);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task SendFeatureReportAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var array = data.ToArray();
        _stream.SetFeature(array);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<ReadResult> ReadFeatureReportAsync(byte reportId, CancellationToken cancellationToken = default)
    {
        var buffer = new byte[_maxInputLength];
        buffer[0] = reportId;
        _stream.GetFeature(buffer);
        return Task.FromResult(new ReadResult(reportId, buffer.AsMemory(1), DateTimeOffset.UtcNow));
    }

    /// <inheritdoc/>
    public event EventHandler<ReadResult>? DataReceived;

    /// <inheritdoc/>
    public event EventHandler? Disconnected;

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        IsConnected = false;
        _stream.Dispose();
        Disconnected?.Invoke(this, EventArgs.Empty);
        await Task.CompletedTask;
    }

    private async Task ReadLoopAsync()
    {
        try
        {
            while (!_disposed)
            {
                var result = await ReadAsync();
                DataReceived?.Invoke(this, result);
            }
        }
        catch
        {
            // Stream closed or error - disconnect
            IsConnected = false;
            Disconnected?.Invoke(this, EventArgs.Empty);
        }
    }
}