using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Text.Json;

namespace OpenHardwarePlatform.Desktop.ViewModels;

public partial class PacketLoggerViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isLogging;

    [ObservableProperty]
    private string _logPath = string.Empty;

    [ObservableProperty]
    private string _statusText = "Ready";

    public ObservableCollection<LogEntry> LogEntries { get; } = new();

    [RelayCommand]
    public void ToggleLogging()
    {
        IsLogging = !IsLogging;
        StatusText = IsLogging ? "Logging..." : "Paused";
    }

    public void AddEntry(LogEntry entry)
    {
        LogEntries.Add(entry);
    }

    [RelayCommand]
    public async Task ExportJson()
    {
        var path = LogPath;
        if (string.IsNullOrWhiteSpace(path))
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "openhid_log.json");
        
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(LogEntries.ToList(), options);
        await File.WriteAllTextAsync(path, json);
        StatusText = $"Exported to {path}";
    }

    [RelayCommand]
    public async Task ExportCsv()
    {
        var path = LogPath;
        if (string.IsNullOrWhiteSpace(path))
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "openhid_log.csv");
        
        var lines = new List<string> { "Timestamp,Direction,ReportId,Length,Hex" };
        lines.AddRange(LogEntries.Select(e => 
            $"{e.Timestamp:O},{e.Direction},{e.ReportId},{e.Length},{e.Hex}"));
        await File.WriteAllLinesAsync(path, lines);
        StatusText = $"Exported to {path}";
    }

    [RelayCommand]
    public async Task ExportMarkdown()
    {
        var path = LogPath;
        if (string.IsNullOrWhiteSpace(path))
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "openhid_log.md");
        
        var md = "# OpenHID Packet Log\n\n";
        md += "| Timestamp | Direction | Report ID | Length | Hex |\n";
        md += "|-----------|-----------|-----------|--------|-----|\n";
        foreach (var entry in LogEntries)
        {
            md += $"| {entry.Timestamp:HH:mm:ss.fff} | {entry.Direction} | {entry.ReportId} | {entry.Length} | {entry.Hex} |\n";
        }
        await File.WriteAllTextAsync(path, md);
        StatusText = $"Exported to {path}";
    }

    [RelayCommand]
    public void ClearLog()
    {
        LogEntries.Clear();
        StatusText = "Cleared";
    }
}

public class LogEntry
{
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    public string Direction { get; set; } = string.Empty;
    public string ReportId { get; set; } = string.Empty;
    public int Length { get; set; }
    public string Hex { get; set; } = string.Empty;
    public string DecodedData { get; set; } = string.Empty;
    public double LatencyMs { get; set; }
}