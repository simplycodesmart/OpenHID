using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenHardwarePlatform.Abstractions;
using OpenHardwarePlatform.Core;
using OpenHardwarePlatform.Desktop.Models;

namespace OpenHardwarePlatform.Desktop.ViewModels;

public partial class ConnectionManagerViewModel : ObservableObject
{
    private readonly DeviceManager _deviceManager;

    [ObservableProperty]
    private DeviceInfoModel? _selectedDevice;

    [ObservableProperty]
    private bool _isConnected;

    [ObservableProperty]
    private string _connectionStatus = "Disconnected";

    public ObservableCollection<ConnectionEntry> ActiveConnections { get; } = new();

    public ConnectionManagerViewModel(DeviceManager deviceManager)
    {
        _deviceManager = deviceManager;
    }

    [RelayCommand]
    public async Task Connect()
    {
        if (SelectedDevice == null) return;
        try
        {
            ConnectionStatus = "Connecting...";
            var deviceInfo = await _deviceManager.EnumerateAllDevicesAsync()
                .FirstOrDefaultAsync(d => d.DevicePath == SelectedDevice.DevicePath);
            
            if (deviceInfo != null)
            {
                var connection = await _deviceManager.OpenDeviceAsync(deviceInfo);
                ActiveConnections.Add(new ConnectionEntry
                {
                    DevicePath = SelectedDevice.DevicePath,
                    DisplayName = SelectedDevice.DisplayName,
                    IsConnected = true
                });
                IsConnected = true;
                ConnectionStatus = $"Connected to {SelectedDevice.DisplayName}";
            }
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    public void Disconnect()
    {
        if (SelectedDevice == null) return;
        var entry = ActiveConnections.FirstOrDefault(c => c.DevicePath == SelectedDevice.DevicePath);
        if (entry != null)
        {
            ActiveConnections.Remove(entry);
        }
        IsConnected = false;
        ConnectionStatus = "Disconnected";
    }

    partial void OnSelectedDeviceChanged(DeviceInfoModel? value)
    {
        if (value != null)
        {
            IsConnected = ActiveConnections.Any(c => c.DevicePath == value.DevicePath);
            ConnectionStatus = IsConnected ? $"Connected to {value.DisplayName}" : "Disconnected";
        }
    }
}

public class ConnectionEntry
{
    public string DevicePath { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
}