using HL7MessageProcessor.ReadOnlySpanProcessor;

namespace HL7MessageProcessor;

public interface IHL7MessageTransformer
{
    public HL7Message Transform();
}
