namespace OpenHardwarePlatform.Protocols.Decoders;

public class LogitechHidPlusDecoder : HidProtocolDecoder
{
    public override string ProtocolName => "Logitech HID++";
    public override string Vendor => "Logitech";

    private static readonly int[] LogitechVids = { 0x046D };

    public override bool CanDecode(int vendorId, int productId, byte[] report)
    {
        return LogitechVids.Contains(vendorId) && report.Length >= 7;
    }

    public override DecodedReport Decode(byte[] report)
    {
        var result = new DecodedReport
        {
            Protocol = ProtocolName,
            ReportId = report.Length > 0 ? report[0] : 0,
            Summary = "Logitech HID++ report"
        };

        if (report.Length >= 7)
        {
            result.Fields.Add(new DecodedField { Name = "Report ID", Value = $"0x{report[0]:X2}", ByteOffset = 0 });
            result.Fields.Add(new DecodedField { Name = "Device Index", Value = $"0x{report[1]:X2}", ByteOffset = 1 });
            result.Fields.Add(new DecodedField { Name = "Sub-ID", Value = $"0x{report[2]:X2}", ByteOffset = 2 });
            result.Fields.Add(new DecodedField { Name = "Address", Value = $"0x{report[3]:X2}", ByteOffset = 3 });
            result.Fields.Add(new DecodedField { Name = "Payload 1", Value = $"0x{report[4]:X2}", ByteOffset = 4 });
            result.Fields.Add(new DecodedField { Name = "Payload 2", Value = $"0x{report[5]:X2}", ByteOffset = 5 });
            result.Fields.Add(new DecodedField { Name = "Payload 3", Value = $"0x{report[6]:X2}", ByteOffset = 6 });

            result.Summary = (report[2]) switch
            {
                0x00 => "HID++ Error",
                0x10 => "HID++ Ping",
                0x11 => "HID++ Sleep Mode",
                0x20 => "HID++ Device Info",
                0x30 => "HID++ Battery Status",
                0x40 => "HID++ Register Read",
                0x41 => "HID++ Register Write",
                0x50 => "HID++ Factory Reset",
                0x60 => "HID++ DFU / Firmware",
                _ => $"HID++ SubID 0x{report[2]:X2}"
            };
        }

        return result;
    }
}