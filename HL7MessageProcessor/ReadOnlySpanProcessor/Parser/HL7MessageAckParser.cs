namespace HL7MessageProcessor.ReadOnlySpanProcessor;

public static class HL7MessageAckParser
{
    public static HL7MessageAck CreateFromOriginalMessage(HL7MessageHeader originalMSH, string acknowledgmentCode = "AA", string textMessage = "")
    {
        // Ensure acknowledgment code is valid
        if (acknowledgmentCode != "AA" && acknowledgmentCode != "AE" && acknowledgmentCode != "AR")
        {
            throw new ArgumentException("Invalid acknowledgment code. Must be 'AA', 'AE', or 'AR'.");
        }

        var ackMSH = HL7MessageHeaderParser.CreateForAcknoledgement(originalMSH);

        return new HL7MessageAck()
        {
            Header = ackMSH,
            AcknowledgmentCode = acknowledgmentCode,
            MessageControlID = originalMSH.MessageControlId,
            TextMessage = textMessage
        };
    }
}
