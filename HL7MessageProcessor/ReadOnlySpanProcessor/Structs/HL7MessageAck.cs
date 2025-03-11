using System.Text;

namespace HL7MessageProcessor.ReadOnlySpanProcessor;

public class HL7MessageAck
{
    public required HL7MessageHeader Header { get; set; }
    public required string AcknowledgmentCode { get; set; }
    public required string MessageControlID { get; set; }
    public required string TextMessage { get; set; }

    /// <summary>
    /// Converts the HL7 ACK message to its raw HL7 string format.
    /// </summary>
    public override string ToString()
    {
        var builder = new StringBuilder();

        // MSH Segment (Message Header)
        builder.Append("MSH").Append(Header.FieldSeparator)
            .Append(Header.EncodingCharacters).Append(Header.FieldSeparator)
            .Append(Header.SendingApplication).Append(Header.FieldSeparator) // Swap Sender ↔ Receiver
            .Append(Header.SendingFacility).Append(Header.FieldSeparator)
            .Append(Header.ReceivingApplication).Append(Header.FieldSeparator)
            .Append(Header.ReceivingFacility).Append(Header.FieldSeparator)
            .Append(Header.DateTimeOfMessage).Append(Header.FieldSeparator)
            .Append(Header.Security).Append(Header.FieldSeparator)
            .Append(Header.MessageType).Append(Header.FieldSeparator)
            .Append(Header.MessageControlId).Append(Header.FieldSeparator)
            .Append(Header.ProcessingId).Append(Header.FieldSeparator)
            .Append(Header.VersionId).Append(Header.FieldSeparator)
            .Append(Header.SequenceNumber).Append(Header.FieldSeparator)
            .Append(Header.ContinuationPointer).Append(Header.FieldSeparator)
            .Append(Header.AcceptAcknowledgmentType).Append(Header.FieldSeparator)
            .Append(Header.ApplicationAcknowledgmentType).Append(Header.FieldSeparator)
            .Append(Header.CountryCode).Append(Header.FieldSeparator)
            .Append(Header.CharacterSet).Append(Header.FieldSeparator)
            .Append(Header.PrincipalLanguageOfMessage).Append("\r\n"); // End MSH with CR

        // MSA Segment (Message Acknowledgment)
        builder.Append("MSA").Append(Header.FieldSeparator)
            .Append(AcknowledgmentCode).Append(Header.FieldSeparator) // AA, AE, AR
            .Append(MessageControlID).Append(Header.FieldSeparator)
            .Append(TextMessage).Append("\r\n"); // End MSA with CR

        return builder.ToString();
    }
}
