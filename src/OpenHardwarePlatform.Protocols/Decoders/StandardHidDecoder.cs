namespace OpenHardwarePlatform.Protocols.Decoders;

public class StandardHidDecoder : HidProtocolDecoder
{
    public override string ProtocolName => "Standard HID";
    public override string Vendor => "Generic";

    public override bool CanDecode(int vendorId, int productId, byte[] report) => true;

    public override DecodedReport Decode(byte[] report)
    {
        var result = new DecodedReport
        {
            Protocol = ProtocolName,
            ReportId = report.Length > 0 ? report[0] : 0,
            Summary = $"Standard HID report, {report.Length} bytes"
        };

        for (int i = 0; i < report.Length; i++)
        {
            result.Fields.Add(new DecodedField
            {
                Name = i == 0 ? "Report ID" : $"Byte {i}",
                Value = report[i].ToString(),
                RawHex = $"0x{report[i]:X2}",
                ByteOffset = i
            });
        }

        return result;
    }
}