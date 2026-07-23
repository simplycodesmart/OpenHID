using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenHardwarePlatform.Desktop.Models;

namespace OpenHardwarePlatform.Desktop.ViewModels;

public partial class DescriptorExplorerViewModel : ObservableObject
{
    [ObservableProperty]
    private DeviceInfoModel? _selectedDevice;

    [ObservableProperty]
    private string _rawDescriptor = string.Empty;

    [ObservableProperty]
    private string _parsedDescriptor = string.Empty;

    public ObservableCollection<DescriptorItem> DescriptorItems { get; } = new();

    public void ParseDescriptor(byte[] descriptorData)
    {
        DescriptorItems.Clear();
        RawDescriptor = BitConverter.ToString(descriptorData).Replace("-", " ");
        
        int i = 0;
        while (i < descriptorData.Length)
        {
            if (i + 1 >= descriptorData.Length) break;
            var size = descriptorData[i];
            var type = descriptorData[i + 1];
            if (size == 0) break;
            
            var item = new DescriptorItem
            {
                Offset = i,
                Size = size,
                Type = GetDescriptorTypeName(type),
                Data = descriptorData.Skip(i).Take(size).ToArray()
            };
            DescriptorItems.Add(item);
            i += size;
        }
        
        ParsedDescriptor = string.Join("\n", DescriptorItems.Select(d => 
            $"[0x{d.Offset:X4}] {d.Type}: {BitConverter.ToString(d.Data).Replace("-", " ")}"));
    }

    private static string GetDescriptorTypeName(byte type) => type switch
    {
        0x01 => "Input",
        0x02 => "Output",
        0x03 => "Feature",
        0x04 => "Collection",
        0x05 => "Usage Page",
        0x06 => "Usage",
        0x07 => "Usage Minimum",
        0x08 => "Usage Maximum",
        0x09 => "Physical Minimum",
        0x0A => "Physical Maximum",
        0x0B => "Unit Exponent",
        0x0C => "Unit",
        0x0D => "Report Size",
        0x0E => "Report ID",
        0x0F => "Report Count",
        0x10 => "Push",
        0x11 => "Pop",
        0x12 => "Usage Page (Extended)",
        0x14 => "Logical Minimum",
        0x15 => "Logical Maximum",
        0x20 => "Designator Index",
        0x21 => "Designator Minimum",
        0x22 => "Designator Maximum",
        0x30 => "String Index",
        0x31 => "String Minimum",
        0x32 => "String Maximum",
        0x40 => "Delimiter",
        0xA0 => "Collection (Application)",
        0xA1 => "Collection (Physical)",
        0xA2 => "Collection (Logical)",
        0xA3 => "Collection (Report)",
        0xA4 => "Collection (Named Array)",
        0xA5 => "Collection (Usage Switch)",
        0xA6 => "Collection (Usage Modifier)",
        _ => $"Unknown (0x{type:X2})"
    };
}

public class DescriptorItem
{
    public int Offset { get; set; }
    public int Size { get; set; }
    public string Type { get; set; } = string.Empty;
    public byte[] Data { get; set; } = Array.Empty<byte>();
}