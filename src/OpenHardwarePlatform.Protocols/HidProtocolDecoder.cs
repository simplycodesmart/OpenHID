namespace OpenHardwarePlatform.Protocols;

public abstract class HidProtocolDecoder
{
    public abstract string ProtocolName { get; }
    public abstract string Vendor { get; }
    public abstract bool CanDecode(int vendorId, int productId, byte[] report);
    public abstract DecodedReport Decode(byte[] report);
}

public class DecodedReport
{
    public string Protocol { get; set; } = string.Empty;
    public List<DecodedField> Fields { get; set; } = new();
    public string Summary { get; set; } = string.Empty;
    public int ReportId { get; set; }
}

public class DecodedField
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public int ByteOffset { get; set; }
    public int BitOffset { get; set; }
    public int BitLength { get; set; }
    public string RawHex { get; set; } = string.Empty;
}