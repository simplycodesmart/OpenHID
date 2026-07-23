using OpenHardwarePlatform.Abstractions;

namespace OpenHardwarePlatform.Core.Tests;

public class DeviceManagerTests
{
    [Fact]
    public void RegisterTransport_AddsTransport()
    {
        var mgr = new DeviceManager();
        var transport = new MockTransport("HID");
        mgr.RegisterTransport(transport);
        Assert.Single(mgr.Transports);
    }

    [Fact]
    public void RegisterTransport_Duplicate_Throws()
    {
        var mgr = new DeviceManager();
        mgr.RegisterTransport(new MockTransport("HID"));
        Assert.Throws<InvalidOperationException>(() => mgr.RegisterTransport(new MockTransport("HID")));
    }

    [Fact]
    public void UnregisterTransport_RemovesTransport()
    {
        var mgr = new DeviceManager();
        var transport = new MockTransport("HID");
        mgr.RegisterTransport(transport);
        mgr.UnregisterTransport(transport);
        Assert.Empty(mgr.Transports);
    }

    [Fact]
    public async Task EnumerateAllDevicesAsync_ReturnsDevices()
    {
        var mgr = new DeviceManager();
        mgr.RegisterTransport(new MockTransport("HID", 2));
        var count = 0;
        await foreach (var _ in mgr.EnumerateAllDevicesAsync())
            count++;
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task OpenDeviceAsync_OpensConnection()
    {
        var mgr = new DeviceManager();
        var transport = new MockTransport("HID");
        mgr.RegisterTransport(transport);
        var devices = await mgr.EnumerateAllDevicesAsync().ToListAsync();
        if (devices.Count > 0)
        {
            var conn = await mgr.OpenDeviceAsync(devices[0]);
            Assert.NotNull(conn);
            Assert.True(conn.IsConnected);
        }
    }

    [Fact]
    public void StartStopHotPlugMonitoring_DoesNotThrow()
    {
        var mgr = new DeviceManager();
        mgr.RegisterTransport(new MockTransport("HID"));
        mgr.StartHotPlugMonitoring();
        mgr.StopHotPlugMonitoring();
    }

    [Fact]
    public async Task DeviceConnected_Event_Fires()
    {
        var mgr = new DeviceManager();
        var transport = new MockTransport("HID");
        mgr.RegisterTransport(transport);
        var fired = false;
        mgr.DeviceConnected += (_, _) => fired = true;
        transport.SimulateConnect(new MockDeviceInfo("HID", 0x1234, 0x5678, "/test/path"));
        Assert.True(fired);
    }

    [Fact]
    public async Task DeviceDisconnected_Event_Fires()
    {
        var mgr = new DeviceManager();
        var transport = new MockTransport("HID");
        mgr.RegisterTransport(transport);
        var fired = false;
        mgr.DeviceDisconnected += (_, _) => fired = true;
        transport.SimulateDisconnect(new MockDeviceInfo("HID", 0x1234, 0x5678, "/test/path"));
        Assert.True(fired);
    }

    [Fact]
    public async Task DisposeAsync_CleansUp()
    {
        var mgr = new DeviceManager();
        mgr.RegisterTransport(new MockTransport("HID"));
        await mgr.DisposeAsync();
        Assert.Empty(mgr.Transports);
    }
}

public class MockTransport(string name, int deviceCount = 1) : IDeviceTransport
{
    public string TransportName { get; } = name;
    public event EventHandler<IDeviceInfo>? DeviceConnected;
    public event EventHandler<IDeviceInfo>? DeviceDisconnected;

    public async IAsyncEnumerable<IDeviceInfo> EnumerateDevicesAsync(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        for (int i = 0; i < deviceCount; i++)
        {
            yield return new MockDeviceInfo(name, 0x1234, 0x5678 + i, $"/test/device{i}");
        }
        await Task.CompletedTask;
    }

    public Task<IDeviceConnection> OpenAsync(IDeviceInfo deviceInfo, CancellationToken ct = default)
    {
        return Task.FromResult<IDeviceConnection>(new MockConnection(deviceInfo, true));
    }

    public void SimulateConnect(IDeviceInfo info) => DeviceConnected?.Invoke(this, info);
    public void SimulateDisconnect(IDeviceInfo info) => DeviceDisconnected?.Invoke(this, info);

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

public class MockDeviceInfo(string transport, int vid, int pid, string path) : IDeviceInfo
{
    public string TransportName => transport;
    public int VendorId => vid;
    public int ProductId => pid;
    public string? Manufacturer => "Mock Corp";
    public string? Product => "Mock Device";
    public string? SerialNumber => "SN001";
    public string DevicePath => path;
    public Version? Version => new(1, 0);
}

public class MockConnection(IDeviceInfo info, bool connected) : IDeviceConnection
{
    public IDeviceInfo DeviceInfo => info;
    public bool IsConnected { get; private set; } = connected;
    public event EventHandler<ReadResult>? DataReceived;
    public event EventHandler? Disconnected;

    public Task<ReadResult> ReadAsync(CancellationToken ct = default)
        => Task.FromResult(new ReadResult(0, new byte[] { 0x01, 0x02, 0x03 }, DateTimeOffset.UtcNow));

    public Task WriteAsync(ReadOnlyMemory<byte> data, CancellationToken ct = default) => Task.CompletedTask;
    public Task SendFeatureReportAsync(ReadOnlyMemory<byte> data, CancellationToken ct = default) => Task.CompletedTask;
    public Task<ReadResult> ReadFeatureReportAsync(byte reportId, CancellationToken ct = default)
        => Task.FromResult(new ReadResult(reportId, new byte[] { 0x00 }, DateTimeOffset.UtcNow));

    public ValueTask DisposeAsync()
    {
        IsConnected = false;
        Disconnected?.Invoke(this, EventArgs.Empty);
        return ValueTask.CompletedTask;
    }
}