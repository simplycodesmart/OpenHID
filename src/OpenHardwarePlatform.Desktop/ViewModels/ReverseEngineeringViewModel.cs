using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OpenHardwarePlatform.Desktop.ViewModels;

public partial class ReverseEngineeringViewModel : ObservableObject
{
    [ObservableProperty]
    private string _inputData = string.Empty;

    [ObservableProperty]
    private string _analysisResult = string.Empty;

    [ObservableProperty]
    private string _suggestedProtocol = string.Empty;

    public ObservableCollection<PatternMatch> DetectedPatterns { get; } = new();

    [RelayCommand]
    public void Analyze()
    {
        DetectedPatterns.Clear();
        var hex = InputData.Replace(" ", "").Replace("-", "");
        if (hex.Length % 2 != 0) return;

        var bytes = Enumerable.Range(0, hex.Length / 2)
            .Select(i => Convert.ToByte(hex.Substring(i * 2, 2), 16))
            .ToArray();

        AnalysisResult = $"Analyzed {bytes.Length} bytes\n\n";

        // Bit analysis
        for (int i = 0; i < bytes.Length; i++)
        {
            var b = bytes[i];
            var pattern = new PatternMatch
            {
                ByteOffset = i,
                HexValue = $"0x{b:X2}",
                BinaryValue = Convert.ToString(b, 2).PadLeft(8, '0'),
                BitPattern = AnalyzeBits(b),
                Significance = GetSignificance(i, b)
            };
            DetectedPatterns.Add(pattern);
        }

        // Pattern detection
        if (bytes.Length >= 3 && bytes[0] == bytes[^1])
            AnalysisResult += "→ First and last bytes match (possible framing)\n";

        if (bytes.Length >= 2)
        {
            var checksum = bytes.Aggregate(0, (acc, b) => acc ^ b);
            if (checksum == 0)
                AnalysisResult += "→ XOR checksum = 0 (simple checksum detected)\n";
        }

        var zeroCount = bytes.Count(b => b == 0);
        if (zeroCount > bytes.Length / 2)
            AnalysisResult += "→ High proportion of zero bytes (possible padding/alignment)\n";

        AnalysisResult += $"\nDetected {DetectedPatterns.Count} fields";
        SuggestedProtocol = GuessProtocol(bytes);
    }

    private string AnalyzeBits(byte b)
    {
        var flags = new List<string>();
        if ((b & 0x01) != 0) flags.Add("Bit0");
        if ((b & 0x02) != 0) flags.Add("Bit1");
        if ((b & 0x04) != 0) flags.Add("Bit2");
        if ((b & 0x08) != 0) flags.Add("Bit3");
        if ((b & 0x10) != 0) flags.Add("Bit4");
        if ((b & 0x20) != 0) flags.Add("Bit5");
        if ((b & 0x40) != 0) flags.Add("Bit6");
        if ((b & 0x80) != 0) flags.Add("Bit7");
        return flags.Count > 0 ? string.Join(", ", flags) : "No bits set";
    }

    private string GetSignificance(int index, byte value)
    {
        if (index == 0) return "Likely Report ID";
        if (value == 0x00) return "Zero/padding";
        if (value < 0x20) return "Likely control flags";
        if (value < 0x80) return "Likely ASCII data";
        return "Likely analog/axis value";
    }

    private string GuessProtocol(byte[] bytes)
    {
        if (bytes.Length == 8) return "Standard HID mouse (8 bytes)";
        if (bytes.Length == 64) return "Standard HID keyboard (64 bytes)";
        if (bytes.Length == 16) return "Possible gamepad/joystick report";
        if (bytes.Length == 32) return "Possible vendor-defined HID report";
        return $"Unknown protocol ({bytes.Length} bytes)";
    }

    [RelayCommand]
    public void Clear()
    {
        InputData = string.Empty;
        AnalysisResult = string.Empty;
        SuggestedProtocol = string.Empty;
        DetectedPatterns.Clear();
    }
}

public class PatternMatch
{
    public int ByteOffset { get; set; }
    public string HexValue { get; set; } = string.Empty;
    public string BinaryValue { get; set; } = string.Empty;
    public string BitPattern { get; set; } = string.Empty;
    public string Significance { get; set; } = string.Empty;
}