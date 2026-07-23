namespace OpenHardwarePlatform.Abstractions;

/// <summary>
/// Provides information about a discovered hardware device.
/// </summary>
public interface IDeviceInfo
{
    /// <summary>
    /// Gets the transport that discovered this device.
    /// </summary>
    string TransportName { get; }

    /// <summary>
    /// Gets the vendor identifier.
    /// </summary>
    int VendorId { get; }

    /// <summary>
    /// Gets the product identifier.
    /// </summary>
    int ProductId { get; }

    /// <summary>
    /// Gets the manufacturer string, if available.
    /// </summary>
    string? Manufacturer { get; }

    /// <summary>
    /// Gets the product string, if available.
    /// </summary>
    string? Product { get; }

    /// <summary>
    /// Gets the serial number, if available.
    /// </summary>
    string? SerialNumber { get; }

    /// <summary>
    /// Gets a transport-specific path or identifier for reconnecting.
    /// </summary>
    string DevicePath { get; }

    /// <summary>
    /// Gets the version number reported by the device.
    /// </summary>
    Version? Version { get; }
}