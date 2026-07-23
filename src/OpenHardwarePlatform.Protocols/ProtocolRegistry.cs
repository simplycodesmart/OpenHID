using OpenHardwarePlatform.Protocols.Decoders;

namespace OpenHardwarePlatform.Protocols;

public class ProtocolRegistry
{
    private readonly List<HidProtocolDecoder> _decoders = new();

    public ProtocolRegistry()
    {
        Register(new StandardHidDecoder());
        Register(new LogitechHidPlusDecoder());
    }

    public void Register(HidProtocolDecoder decoder)
    {
        _decoders.Add(decoder);
    }

    public DecodedReport Decode(int vendorId, int productId, byte[] report)
    {
        foreach (var decoder in _decoders)
        {
            if (decoder.CanDecode(vendorId, productId, report))
            {
                return decoder.Decode(report);
            }
        }
        return _decoders[0].Decode(report);
    }

    public IReadOnlyList<HidProtocolDecoder> Decoders => _decoders.AsReadOnly();
}