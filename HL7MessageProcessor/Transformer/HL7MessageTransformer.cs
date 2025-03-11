using HL7MessageProcessor.ReadOnlySpanProcessor;

namespace HL7MessageProcessor.Transformer;

public class HL7MessageTransformer(HL7Message hl7Message) : IHL7MessageTransformer
{
    private readonly HL7Message _hl7Message = hl7Message;

    public HL7Message Transform()
    {
        return _hl7Message;
    }
}
