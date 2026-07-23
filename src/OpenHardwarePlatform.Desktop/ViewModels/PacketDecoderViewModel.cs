using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenHardwarePlatform.Desktop.Models;

namespace OpenHardwarePlatform.Desktop.ViewModels;

public partial class PacketDecoderViewModel : ObservableObject
{
    [ObservableProperty]
    private DeviceInfoModel? _selectedDevice;

    [ObservableProperty]
    private string _rawHex = string.Empty;

    [ObservableProperty]
    private string _decodedOutput = string.Empty;

    [ObservableProperty]
    private string _usagePage = string.Empty;

    public ObservableCollection<DecodedField> DecodedFields { get; } = new();

    [RelayCommand]
    public void Decode()
    {
        DecodedFields.Clear();
        var hex = RawHex.Replace(" ", "").Replace("-", "");
        if (hex.Length % 2 != 0) return;

        var bytes = Enumerable.Range(0, hex.Length / 2)
            .Select(i => Convert.ToByte(hex.Substring(i * 2, 2), 16))
            .ToArray();

        DecodedOutput = $"Raw: {BitConverter.ToString(bytes).Replace("-", " ")}\n";
        DecodedOutput += $"Length: {bytes.Length} bytes\n";
        DecodedOutput += $"Report ID: {bytes[0]:X2}\n";

        for (int i = 0; i < bytes.Length; i++)
        {
            var field = new DecodedField
            {
                Index = i,
                Offset = $"Byte {i}",
                HexValue = $"0x{bytes[i]:X2}",
                BinaryValue = Convert.ToString(bytes[i], 2).PadLeft(8, '0'),
                DecimalValue = bytes[i].ToString(),
                Interpretation = InterpretByte(i, bytes[i])
            };
            DecodedFields.Add(field);
        }

        DecodedOutput += "Decoded successfully.";
    }

    private string InterpretByte(int index, byte value)
    {
        if (index == 0) return "Report ID";
        if (value == 0x00) return "Idle/No data";
        if (value < 0x10) return "Control/Bit field";
        if (value < 0x80) return "ASCII printable";
        return "Extended/Non-standard";
    }

    [RelayCommand]
    public void Clear()
    {
        RawHex = string.Empty;
        DecodedOutput = string.Empty;
        DecodedFields.Clear();
    }
}

public class DecodedField
{
    public int Index { get; set; }
    public string Offset { get; set; } = string.Empty;
    public string HexValue { get; set; } = string.Empty;
    public string BinaryValue { get; set; } = string.Empty;
    public string DecimalValue { get; set; } = string.Empty;
    public string Interpretation { get; set; } = string.Empty;
}