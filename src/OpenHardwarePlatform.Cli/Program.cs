using OpenHardwarePlatform.Abstractions;
using OpenHardwarePlatform.Core;
using OpenHardwarePlatform.Transports.Hid;

Console.WriteLine("OpenHID CLI - Device Scanner");
Console.WriteLine("============================\n");

// Set up platform
var deviceManager = new DeviceManager();
deviceManager.RegisterTransport(new HidTransport());

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
    Console.WriteLine("\nCancelling...");
};

try
{
    Console.WriteLine("Scanning for HID devices...\n");
    var deviceCount = 0;

    await foreach (var device in deviceManager.EnumerateAllDevicesAsync(cts.Token))
    {
        deviceCount++;
        Console.WriteLine($"Device #{deviceCount}");
        Console.WriteLine($"  Transport : {device.TransportName}");
        Console.WriteLine($"  VID       : 0x{device.VendorId:X4}");
        Console.WriteLine($"  PID       : 0x{device.ProductId:X4}");
        Console.WriteLine($"  Product   : {device.Product ?? "(unknown)"}");
        Console.WriteLine($"  Manufacturer : {device.Manufacturer ?? "(unknown)"}");
        Console.WriteLine($"  Serial    : {device.SerialNumber ?? "(none)"}");
        Console.WriteLine($"  Path      : {device.DevicePath}");
        Console.WriteLine();
    }

    if (deviceCount == 0)
    {
        Console.WriteLine("No HID devices found.");
    }
    else
    {
        Console.WriteLine($"Total: {deviceCount} device(s) found.");
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("Scan cancelled by user.");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}
finally
{
    await deviceManager.DisposeAsync();
}

return 0;