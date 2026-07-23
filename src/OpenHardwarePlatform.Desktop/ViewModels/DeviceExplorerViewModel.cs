using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenHardwarePlatform.Abstractions;
using OpenHardwarePlatform.Core;
using OpenHardwarePlatform.Desktop.Models;

namespace OpenHardwarePlatform.Desktop.ViewModels;

public partial class DeviceExplorerViewModel : ObservableObject
{
    private readonly DeviceManager _deviceManager;
    private readonly HidTransportService _hidTransport;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _statusText = "Ready";

    [ObservableProperty]
    private int _deviceCount;

    public ObservableCollection<DeviceInfoModel> Devices { get; } = new();
    public ObservableCollection<DeviceInfoModel> FilteredDevices { get; } = new();
    public ObservableCollection<string> SortOptions { get; } = new() { "Name", "Vendor ID", "Product ID", "Manufacturer" };
    
    [ObservableProperty]
    private int _selectedSortIndex;

    public DeviceExplorerViewModel(DeviceManager deviceManager, HidTransportService hidTransport)
    {
        _deviceManager = deviceManager;
        _hidTransport = hidTransport;
    }

    partial void OnSearchTextChanged(string value) => FilterDevices();
    partial void OnSelectedSortIndexChanged(int value) => FilterDevices();

    [RelayCommand]
    public async Task RefreshDevices()
    {
        IsRefreshing = true;
        StatusText = "Enumerating devices...";
        Devices.Clear();
        
        await foreach (var device in _deviceManager.EnumerateAllDevicesAsync())
        {
            Devices.Add(DeviceInfoModel.FromDeviceInfo(device));
        }
        
        DeviceCount = Devices.Count;
        FilterDevices();
        IsRefreshing = false;
        StatusText = $"{DeviceCount} device(s) found";
    }

    private void FilterDevices()
    {
        FilteredDevices.Clear();
        var query = Devices.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            query = query.Where(d =>
                d.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                d.Summary.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                (d.Manufacturer?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        query = SelectedSortIndex switch
        {
            0 => query.OrderBy(d => d.DisplayName),
            1 => query.OrderBy(d => d.VendorId),
            2 => query.OrderBy(d => d.ProductId),
            3 => query.OrderBy(d => d.Manufacturer),
            _ => query
        };

        foreach (var device in query)
            FilteredDevices.Add(device);
    }
}