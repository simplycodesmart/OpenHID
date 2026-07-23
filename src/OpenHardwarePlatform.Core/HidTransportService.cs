using OpenHardwarePlatform.Abstractions;

namespace OpenHardwarePlatform.Core;

public class HidTransportService
{
    private readonly DeviceManager _deviceManager;

    public HidTransportService(DeviceManager deviceManager)
    {
        _deviceManager = deviceManager;
    }

    public async Task<IReadOnlyList<IDeviceInfo>> EnumerateDevicesAsync(CancellationToken ct = default)
    {
        var devices = new List<IDeviceInfo>();
        await foreach (var device in _deviceManager.EnumerateAllDevicesAsync(ct))
        {
            devices.Add(device);
        }
        return devices;
    }

    public async Task<IDeviceConnection> OpenConnectionAsync(IDeviceInfo deviceInfo, CancellationToken ct = default)
    {
        return await _deviceManager.OpenDeviceAsync(deviceInfo, ct);
    }
}