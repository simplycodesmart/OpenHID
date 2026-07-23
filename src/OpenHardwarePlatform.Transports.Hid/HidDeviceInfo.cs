using OpenHardwarePlatform.Abstractions;

namespace OpenHardwarePlatform.Transports.Hid;

/// <summary>
/// Wraps a <see cref="HidSharp.HidDevice"/> as an <see cref="IDeviceInfo"/>.
/// </summary>
internal sealed class HidDeviceInfo : IDeviceInfo
{
    private readonly HidSharp.HidDevice _device;

    public HidDeviceInfo(HidSharp.HidDevice device)
    {
        _device = device ?? throw new ArgumentNullException(nameof(device));
    }

    /// <inheritdoc/>
    public string TransportName => "HID";

    /// <inheritdoc/>
    public int VendorId => _device.VendorID;

    /// <inheritdoc/>
    public int ProductId => _device.ProductID;

    /// <inheritdoc/>
    public string? Manufacturer => _device.Manufacturer;

    /// <inheritdoc/>
    public string? Product => _device.ProductName;

    /// <inheritdoc/>
    public string? SerialNumber => _device.SerialNumber;

    /// <inheritdoc/>
    public string DevicePath => _device.DevicePath;

    /// <inheritdoc/>
    public Version? Version => _device.ReleaseNumber;

    /// <summary>
    /// Gets the underlying HidSharp device.
    /// </summary>
    public HidSharp.HidDevice NativeDevice => _device;
}