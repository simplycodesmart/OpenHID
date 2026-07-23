using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenHardwarePlatform.Abstractions;
using OpenHardwarePlatform.Core;
using OpenHardwarePlatform.Desktop.Models;

namespace OpenHardwarePlatform.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly DeviceManager _deviceManager;

    [ObservableProperty] private bool _isDarkTheme = true;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private DeviceInfoModel? _selectedDevice;
    [ObservableProperty] private int _selectedNavIndex;
    [ObservableProperty] private bool _isRefreshing;
    [ObservableProperty] private int _deviceCount;
    [ObservableProperty] private string _statusText = "Ready";
    [ObservableProperty] private string _pageTitle = "Device Explorer";
    [ObservableProperty] private string _pageSubtitle = "Discover and inspect connected HID devices";
    [ObservableProperty] private object? _currentPage;

    public ObservableCollection<DeviceInfoModel> Devices { get; } = new();
    public ObservableCollection<DeviceInfoModel> FilteredDevices { get; } = new();
    public ObservableCollection<NavItem> NavItems { get; } = new();

    public DeviceExplorerViewModel DeviceExplorer { get; }
    public DeviceInspectorViewModel DeviceInspector { get; }
    public ConnectionManagerViewModel ConnectionManager { get; }
    public DescriptorExplorerViewModel DescriptorExplorer { get; }
    public HidConsoleViewModel HidConsole { get; }
    public LiveMonitorViewModel LiveMonitor { get; }
    public PacketDecoderViewModel PacketDecoder { get; }
    public PacketLoggerViewModel PacketLogger { get; }
    public AutomationViewModel Automation { get; }
    public ReverseEngineeringViewModel ReverseEngineering { get; }
    public DocumentationGeneratorViewModel DocumentationGenerator { get; }

    public MainViewModel(
        DeviceManager deviceManager,
        HidTransportService hidTransport,
        DeviceExplorerViewModel deviceExplorer,
        DeviceInspectorViewModel deviceInspector,
        ConnectionManagerViewModel connectionManager,
        DescriptorExplorerViewModel descriptorExplorer,
        HidConsoleViewModel hidConsole,
        LiveMonitorViewModel liveMonitor,
        PacketDecoderViewModel packetDecoder,
        PacketLoggerViewModel packetLogger,
        AutomationViewModel automation,
        ReverseEngineeringViewModel reverseEngineering,
        DocumentationGeneratorViewModel documentationGenerator)
    {
        _deviceManager = deviceManager;
        DeviceExplorer = deviceExplorer;
        DeviceInspector = deviceInspector;
        ConnectionManager = connectionManager;
        DescriptorExplorer = descriptorExplorer;
        HidConsole = hidConsole;
        LiveMonitor = liveMonitor;
        PacketDecoder = packetDecoder;
        PacketLogger = packetLogger;
        Automation = automation;
        ReverseEngineering = reverseEngineering;
        DocumentationGenerator = documentationGenerator;

        NavItems.Add(new NavItem("Device Explorer", "🔍", 0, "Discover HID devices"));
        NavItems.Add(new NavItem("Device Inspector", "📋", 1, "Hardware properties"));
        NavItems.Add(new NavItem("Connection Manager", "🔌", 2, "Open sessions"));
        NavItems.Add(new NavItem("Descriptor Explorer", "📄", 3, "Report descriptors"));
        NavItems.Add(new NavItem("HID Console", "⌨️", 4, "Send & receive"));
        NavItems.Add(new NavItem("Live Monitor", "📡", 5, "Real-time traffic"));
        NavItems.Add(new NavItem("Packet Decoder", "🔢", 6, "Decode bytes"));
        NavItems.Add(new NavItem("Packet Logger", "📝", 7, "Capture & export"));
        NavItems.Add(new NavItem("Automation", "⚡", 8, "Scripts & replay"));
        NavItems.Add(new NavItem("Reverse Engineering", "🔬", 9, "Pattern analysis"));
        NavItems.Add(new NavItem("Documentation", "📚", 10, "Generate docs"));

        _deviceManager.DeviceConnected += OnDeviceConnected;
        _deviceManager.DeviceDisconnected += OnDeviceDisconnected;

        DeviceInspector.NavigateToConnectionManager += () => SelectedNavIndex = 2;

        CurrentPage = DeviceExplorer;
        _ = RefreshDevices();
    }

    partial void OnSearchTextChanged(string value) => FilterDevices();

    partial void OnSelectedDeviceChanged(DeviceInfoModel? value)
    {
        DeviceInspector.SelectedDevice = value;
        ConnectionManager.SelectedDevice = value;
        DescriptorExplorer.SelectedDevice = value;
        HidConsole.SelectedDevice = value;
        LiveMonitor.SelectedDevice = value;
        PacketDecoder.SelectedDevice = value;

        if (value != null)
        {
            DocumentationGenerator.DeviceName = value.DisplayName;
            DocumentationGenerator.VendorInfo = $"{value.VendorIdHex} / {value.ProductIdHex}";
            StatusText = $"Selected {value.DisplayName}";
        }
    }

    partial void OnSelectedNavIndexChanged(int value)
    {
        foreach (var nav in NavItems)
            nav.IsActive = nav.Index == value;

        (CurrentPage, PageTitle, PageSubtitle) = value switch
        {
            0 => ((object)DeviceExplorer, "Device Explorer", "Discover and inspect connected HID devices"),
            1 => (DeviceInspector, "Device Inspector", "Every property of the selected device"),
            2 => (ConnectionManager, "Connection Manager", "Safely open and manage sessions"),
            3 => (DescriptorExplorer, "Descriptor Explorer", "Read and interpret HID report descriptors"),
            4 => (HidConsole, "HID Console", "Communicate directly with devices"),
            5 => (LiveMonitor, "Live Monitor", "Observe incoming reports in real time"),
            6 => (PacketDecoder, "Packet Decoder", "Convert raw bytes into meaningful fields"),
            7 => (PacketLogger, "Packet Logger", "Record communication history"),
            8 => (Automation, "Automation", "Repeat testing automatically"),
            9 => (ReverseEngineering, "Reverse Engineering", "Understand unknown HID devices"),
            10 => (DocumentationGenerator, "Documentation", "Generate docs from connected devices"),
            _ => (DeviceExplorer, "Device Explorer", "Discover and inspect connected HID devices")
        };
    }

    [RelayCommand]
    private async Task RefreshDevices()
    {
        IsRefreshing = true;
        StatusText = "Scanning HID devices...";
        Devices.Clear();
        try
        {
            await foreach (var device in _deviceManager.EnumerateAllDevicesAsync())
            {
                Devices.Add(DeviceInfoModel.FromDeviceInfo(device));
            }
            DeviceCount = Devices.Count;
            FilterDevices();
            StatusText = $"{DeviceCount} device(s) found";
            DeviceExplorer.StatusText = StatusText;
            DeviceExplorer.DeviceCount = DeviceCount;
        }
        catch (Exception ex)
        {
            StatusText = $"Scan failed: {ex.Message}";
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private void SelectNav(int index) => SelectedNavIndex = index;

    [RelayCommand]
    private void SelectDevice(DeviceInfoModel? device)
    {
        if (device == null) return;
        SelectedDevice = device;
        SelectedNavIndex = 1;
    }

    private void FilterDevices()
    {
        FilteredDevices.Clear();
        DeviceExplorer.FilteredDevices.Clear();
        DeviceExplorer.Devices.Clear();

        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? Devices.AsEnumerable()
            : Devices.Where(d =>
                d.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                d.Summary.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                (d.Manufacturer?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (d.Product?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false));

        foreach (var device in filtered.OrderBy(d => d.DisplayName))
        {
            FilteredDevices.Add(device);
            DeviceExplorer.FilteredDevices.Add(device);
            DeviceExplorer.Devices.Add(device);
        }
        DeviceExplorer.DeviceCount = FilteredDevices.Count;
    }

    private void OnDeviceConnected(object? sender, IDeviceInfo info)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (Devices.Any(d => d.DevicePath == info.DevicePath)) return;
            Devices.Add(DeviceInfoModel.FromDeviceInfo(info));
            DeviceCount = Devices.Count;
            FilterDevices();
            StatusText = $"Connected: {info.Product ?? info.DevicePath}";
        });
    }

    private void OnDeviceDisconnected(object? sender, IDeviceInfo info)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var device = Devices.FirstOrDefault(d => d.DevicePath == info.DevicePath);
            if (device == null) return;
            Devices.Remove(device);
            if (SelectedDevice?.DevicePath == info.DevicePath)
                SelectedDevice = null;
            DeviceCount = Devices.Count;
            FilterDevices();
            StatusText = $"Disconnected: {info.Product ?? info.DevicePath}";
        });
    }

    [RelayCommand]
    private void ToggleTheme() => IsDarkTheme = !IsDarkTheme;
}

public partial class NavItem : ObservableObject
{
    public string Name { get; }
    public string Icon { get; }
    public int Index { get; }
    public string Description { get; }

    [ObservableProperty] private bool _isActive;

    public NavItem(string name, string icon, int index, string description = "")
    {
        Name = name;
        Icon = icon;
        Index = index;
        Description = description;
        IsActive = index == 0;
    }
}
