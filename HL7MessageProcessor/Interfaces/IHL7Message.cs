using HL7MessageProcessor.ReadOnlySpanProcessor;

namespace HL7MessageProcessor;

public interface IHL7Message
{
    public HL7MessageHeader MessageHeader { get; }
    public HL7MessageAck MessageAck { get; }
    public string GetValue(string segmentName, int elementIndex, int subElementIndex = 0, int subSubElementIndex = 0);
    public IReadOnlyList<string> GetValues(string segmentName, int elementIndex, int subElementIndex = 0, int subSubElementIndex = 0);
    public bool IsValid();
}
