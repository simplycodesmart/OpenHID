using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenHardwarePlatform.Desktop.Models;

namespace OpenHardwarePlatform.Desktop.ViewModels;

public partial class LiveMonitorViewModel : ObservableObject
{
    [ObservableProperty]
    private DeviceInfoModel? _selectedDevice;

    [ObservableProperty]
    private bool _isMonitoring;

    [ObservableProperty]
    private bool _autoScroll = true;

    [ObservableProperty]
    private string _filterText = string.Empty;

    [ObservableProperty]
    private string _statusText = "Ready";

    public ObservableCollection<PacketEntry> Packets { get; } = new();
    public ObservableCollection<PacketEntry> FilteredPackets { get; } = new();

    [RelayCommand]
    public void ToggleMonitoring()
    {
        IsMonitoring = !IsMonitoring;
        StatusText = IsMonitoring ? "Monitoring..." : "Paused";
    }

    public void AddPacket(PacketEntry packet)
    {
        Packets.Add(packet);
        if (string.IsNullOrWhiteSpace(FilterText) ||
            packet.Hex.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
            packet.Direction.Contains(FilterText, StringComparison.OrdinalIgnoreCase))
        {
            FilteredPackets.Add(packet);
            if (AutoScroll && FilteredPackets.Count > 0)
            {
                // Auto-scroll handled by view
            }
        }
    }

    [RelayCommand]
    public void ClearPackets()
    {
        Packets.Clear();
        FilteredPackets.Clear();
    }

    partial void OnFilterTextChanged(string value)
    {
        FilteredPackets.Clear();
        var filtered = string.IsNullOrWhiteSpace(value)
            ? Packets
            : Packets.Where(p =>
                p.Hex.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                p.Direction.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                p.ReportId.Contains(value));

        foreach (var p in filtered)
            FilteredPackets.Add(p);
    }
}

public class PacketEntry
{
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    public string Direction { get; set; } = string.Empty;
    public string ReportId { get; set; } = string.Empty;
    public int Length { get; set; }
    public string Hex { get; set; } = string.Empty;
    public string Decoded { get; set; } = string.Empty;
    public string TimestampStr => Timestamp.ToString("HH:mm:ss.fff");
}