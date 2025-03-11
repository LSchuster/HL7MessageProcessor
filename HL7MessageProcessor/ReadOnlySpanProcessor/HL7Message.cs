using HL7MessageProcessor.Transformer;

namespace HL7MessageProcessor.ReadOnlySpanProcessor;

public class HL7Message(string message) : IHL7Message
{
    private readonly string _message = message;
    private HL7MessageHeader? _messageHeader;
    private HL7MessageAck? _messageAck;
    private HL7MessageTransformer? _messageTransformer;

    public HL7MessageHeader MessageHeader
    {
        get
        {
            _messageHeader ??= HL7MessageHeaderParser.CreateFromMessage(_message.AsSpan());
            return _messageHeader;
        }
    }

    public HL7MessageAck MessageAck
    {
        get
        {
            _messageAck ??= HL7MessageAckParser.CreateFromOriginalMessage(MessageHeader);
            return _messageAck;
        }
    }

    public HL7MessageTransformer MessageTransformer
    {
        get
        {
            _messageTransformer ??= new HL7MessageTransformer(this);
            return _messageTransformer;
        }
    }

    public string GetValue(string segmentName, int elementIndex, int componentIndex = 0, int subComponentIndex = 0)
    {
        return HL7MessageParser.GetValue(_message.AsSpan(), MessageHeader, segmentName, elementIndex, componentIndex, subComponentIndex);
    }

    public IReadOnlyList<string> GetValues(string segmentName, int elementIndex, int componentIndex = 0, int subComponentIndex = 0)
    {
        return HL7MessageParser.GetValues(_message.AsSpan(), MessageHeader, segmentName, elementIndex, componentIndex, subComponentIndex);
    }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(MessageHeader.SendingApplication) &&
                !string.IsNullOrEmpty(MessageHeader.ReceivingApplication) &&
                !string.IsNullOrEmpty(MessageHeader.SendingFacility) &&
                !string.IsNullOrEmpty(MessageHeader.ReceivingFacility) &&
                !string.IsNullOrEmpty(MessageHeader.FieldSeparator) &&
                MessageHeader.ComponentSeparator != null &&
                MessageHeader.SubComponentSeparator != null;
    }

    public override string ToString()
    {
        return _message;
    }
}
