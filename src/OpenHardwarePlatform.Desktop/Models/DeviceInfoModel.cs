using OpenHardwarePlatform.Abstractions;

namespace OpenHardwarePlatform.Desktop.Models;

public class DeviceInfoModel
{
    public string TransportName { get; set; } = string.Empty;
    public int VendorId { get; set; }
    public int ProductId { get; set; }
    public string? Manufacturer { get; set; }
    public string? Product { get; set; }
    public string? SerialNumber { get; set; }
    public string DevicePath { get; set; } = string.Empty;
    public Version? Version { get; set; }
    public string VendorIdHex => $"0x{VendorId:X4}";
    public string ProductIdHex => $"0x{ProductId:X4}";
    public string DisplayName => $"{Manufacturer ?? "Unknown"} {Product ?? "Device"}";
    public string Summary => $"{VendorIdHex}:{ProductIdHex}";

    public static DeviceInfoModel FromDeviceInfo(IDeviceInfo info) => new()
    {
        TransportName = info.TransportName,
        VendorId = info.VendorId,
        ProductId = info.ProductId,
        Manufacturer = info.Manufacturer,
        Product = info.Product,
        SerialNumber = info.SerialNumber,
        DevicePath = info.DevicePath,
        Version = info.Version
    };
}