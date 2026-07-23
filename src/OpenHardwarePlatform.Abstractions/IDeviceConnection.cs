namespace OpenHardwarePlatform.Abstractions;

/// <summary>
/// Represents an open connection to a hardware device.
/// </summary>
public interface IDeviceConnection : IAsyncDisposable
{
    /// <summary>
    /// Gets the device info for this connection.
    /// </summary>
    IDeviceInfo DeviceInfo { get; }

    /// <summary>
    /// Gets whether the connection is currently open.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Reads data from the device.
    /// </summary>
    Task<ReadResult> ReadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes data to the device.
    /// </summary>
    Task WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a feature report to the device.
    /// </summary>
    Task SendFeatureReportAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads a feature report from the device.
    /// </summary>
    Task<ReadResult> ReadFeatureReportAsync(byte reportId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Raised when data is received from the device.
    /// </summary>
    event EventHandler<ReadResult>? DataReceived;

    /// <summary>
    /// Raised when the connection is closed.
    /// </summary>
    event EventHandler? Disconnected;
}

/// <summary>
/// Represents the result of a read operation.
/// </summary>
/// <param name="ReportId">The report identifier.</param>
/// <param name="Data">The data read from the device.</param>
/// <param name="Timestamp">The time at which the data was received.</param>
public readonly record struct ReadResult(byte ReportId, ReadOnlyMemory<byte> Data, DateTimeOffset Timestamp);