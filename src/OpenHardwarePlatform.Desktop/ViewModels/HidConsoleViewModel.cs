using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenHardwarePlatform.Desktop.Models;

namespace OpenHardwarePlatform.Desktop.ViewModels;

public partial class HidConsoleViewModel : ObservableObject
{
    [ObservableProperty]
    private DeviceInfoModel? _selectedDevice;

    [ObservableProperty]
    private string _outputData = string.Empty;

    [ObservableProperty]
    private string _inputData = string.Empty;

    [ObservableProperty]
    private string _featureData = string.Empty;

    [ObservableProperty]
    private string _consoleLog = string.Empty;

    [ObservableProperty]
    private string _reportIdInput = "0";

    [ObservableProperty]
    private string _reportIdFeature = "0";

    public ObservableCollection<ConsoleEntry> History { get; } = new();

    [RelayCommand]
    public void SendOutput()
    {
        if (string.IsNullOrWhiteSpace(OutputData)) return;
        var entry = new ConsoleEntry
        {
            Timestamp = DateTimeOffset.Now,
            Direction = "OUT",
            Data = OutputData,
            ReportId = ReportIdInput
        };
        History.Add(entry);
        ConsoleLog = $"OUT [{ReportIdInput}]: {OutputData}\n" + ConsoleLog;
        OutputData = string.Empty;
    }

    [RelayCommand]
    public void ReadInput()
    {
        var entry = new ConsoleEntry
        {
            Timestamp = DateTimeOffset.Now,
            Direction = "IN",
            Data = InputData,
            ReportId = "0"
        };
        History.Add(entry);
        ConsoleLog = $"IN: {InputData}\n" + ConsoleLog;
    }

    [RelayCommand]
    public void SendFeature()
    {
        if (string.IsNullOrWhiteSpace(FeatureData)) return;
        var entry = new ConsoleEntry
        {
            Timestamp = DateTimeOffset.Now,
            Direction = "FEAT",
            Data = FeatureData,
            ReportId = ReportIdFeature
        };
        History.Add(entry);
        ConsoleLog = $"FEAT [{ReportIdFeature}]: {FeatureData}\n" + ConsoleLog;
        FeatureData = string.Empty;
    }

    [RelayCommand]
    public void ClearLog()
    {
        History.Clear();
        ConsoleLog = string.Empty;
    }
}

public class ConsoleEntry
{
    public DateTimeOffset Timestamp { get; set; }
    public string Direction { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public string ReportId { get; set; } = "0";
    public string TimestampStr => Timestamp.ToString("HH:mm:ss.fff");
}