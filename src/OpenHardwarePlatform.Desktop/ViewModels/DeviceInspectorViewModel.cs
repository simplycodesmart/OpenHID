using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenHardwarePlatform.Desktop.Models;

namespace OpenHardwarePlatform.Desktop.ViewModels;

public partial class DeviceInspectorViewModel : ObservableObject
{
    private readonly ConnectionManagerViewModel _connectionManager;

    public event Action? NavigateToConnectionManager;

    [ObservableProperty]
    private DeviceInfoModel? _selectedDevice;

    public string VendorIdHex => SelectedDevice?.VendorIdHex ?? "N/A";
    public string ProductIdHex => SelectedDevice?.ProductIdHex ?? "N/A";
    public string Manufacturer => SelectedDevice?.Manufacturer ?? "N/A";
    public string Product => SelectedDevice?.Product ?? "N/A";
    public string SerialNumber => SelectedDevice?.SerialNumber ?? "N/A";
    public string DevicePath => SelectedDevice?.DevicePath ?? "N/A";
    public string Version => SelectedDevice?.Version?.ToString() ?? "N/A";
    public string TransportName => SelectedDevice?.TransportName ?? "N/A";

    public bool IsConnected => _connectionManager.IsConnected;
    public string ConnectionStatus => _connectionManager.ConnectionStatus;

    public DeviceInspectorViewModel(ConnectionManagerViewModel connectionManager)
    {
        _connectionManager = connectionManager;
    }

    partial void OnSelectedDeviceChanged(DeviceInfoModel? value)
    {
        _connectionManager.SelectedDevice = value;
        OnPropertyChanged(nameof(VendorIdHex));
        OnPropertyChanged(nameof(ProductIdHex));
        OnPropertyChanged(nameof(Manufacturer));
        OnPropertyChanged(nameof(Product));
        OnPropertyChanged(nameof(SerialNumber));
        OnPropertyChanged(nameof(DevicePath));
        OnPropertyChanged(nameof(Version));
        OnPropertyChanged(nameof(TransportName));
        OnPropertyChanged(nameof(IsConnected));
        OnPropertyChanged(nameof(ConnectionStatus));
    }

    [RelayCommand]
    private async Task Connect()
    {
        await _connectionManager.Connect();
        OnPropertyChanged(nameof(IsConnected));
        OnPropertyChanged(nameof(ConnectionStatus));
        if (IsConnected)
        {
            NavigateToConnectionManager?.Invoke();
        }
    }

    [RelayCommand]
    private void Disconnect()
    {
        _connectionManager.Disconnect();
        OnPropertyChanged(nameof(IsConnected));
        OnPropertyChanged(nameof(ConnectionStatus));
    }
}