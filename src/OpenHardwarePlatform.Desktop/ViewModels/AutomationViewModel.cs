using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OpenHardwarePlatform.Desktop.ViewModels;

public partial class AutomationViewModel : ObservableObject
{
    [ObservableProperty]
    private string _scriptText = string.Empty;

    [ObservableProperty]
    private string _statusText = "Ready";

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private int _repeatCount = 1;

    [ObservableProperty]
    private string _delayMs = "100";

    public ObservableCollection<ScriptEntry> ScriptHistory { get; } = new();

    [RelayCommand]
    public async Task RunScript()
    {
        if (string.IsNullOrWhiteSpace(ScriptText)) return;
        IsRunning = true;
        StatusText = "Running...";

        try
        {
            var lines = ScriptText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            for (int r = 0; r < RepeatCount; r++)
            {
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("//")) continue;
                    
                    var entry = new ScriptEntry
                    {
                        Timestamp = DateTimeOffset.Now,
                        Command = trimmed,
                        Status = "Executed"
                    };
                    ScriptHistory.Add(entry);
                    
                    if (int.TryParse(DelayMs, out var delay) && delay > 0)
                        await Task.Delay(delay);
                }
            }
            StatusText = $"Completed {RepeatCount} iteration(s)";
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
        finally
        {
            IsRunning = false;
        }
    }

    [RelayCommand]
    public void StopScript()
    {
        IsRunning = false;
        StatusText = "Stopped";
    }

    [RelayCommand]
    public void ClearHistory()
    {
        ScriptHistory.Clear();
        StatusText = "Cleared";
    }

    [RelayCommand]
    public void LoadExample()
    {
        ScriptText = "// HID Automation Script\n" +
                     "// Commands: OUT <hex>, FEATURE <hex>, WAIT <ms>\n\n" +
                     "OUT 01 00 00 00\n" +
                     "WAIT 100\n" +
                     "FEATURE 02 01\n" +
                     "WAIT 50\n" +
                     "OUT 01 01 00 00";
    }
}

public class ScriptEntry
{
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    public string Command { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}