namespace HL7MessageProcessor.ReadOnlySpanProcessor;

public static class HL7MessageHeaderParser
{
    public static HL7MessageHeader CreateFromMessage(ReadOnlySpan<char> hl7Message)
    {
        int endOfFirstLine = hl7Message.IndexOfAny('\n', '\r');
        if (endOfFirstLine == -1)
        {
            throw new FormatException("Invalid HL7 message: Missing MSH segment");
        }

        ReadOnlySpan<char> mshSegment = hl7Message[..endOfFirstLine]; // Slice first line

        // Field separator is the 4th character (MSH|^~\& -> ^ is at index 3)
        char fieldSeparator = mshSegment[3];

        // Find fields manually using Span-based parsing (avoiding string.Split)
        ReadOnlySpan<char> remaining = mshSegment.Slice(4); // Start after "MSH|"

        string[] fields = new string[19]; // HL7 MSH has up to 19 fields
        int fieldIndex = 0;

        while (!remaining.IsEmpty && fieldIndex < fields.Length)
        {
            int sepPos = remaining.IndexOf(fieldSeparator);

            // Extract field using Slice, avoiding new string allocations
            ReadOnlySpan<char> fieldValue = sepPos == -1 ? remaining : remaining[..sepPos];
            fields[fieldIndex++] = fieldValue.ToString(); // Only converts small spans to strings

            if (sepPos == -1) break; // No more separators, exit loop
            remaining = remaining[(sepPos + 1)..]; // Move past the separator
        }

        // Build the HL7MessageHeader struct
        return new HL7MessageHeader
        {
            FieldSeparator = fieldSeparator.ToString(),
            EncodingCharacters = fields[0] ?? "",
            SendingApplication = fields[1] ?? "",
            SendingFacility = fields[2] ?? "",
            ReceivingApplication = fields[3] ?? "",
            ReceivingFacility = fields[4] ?? "",
            DateTimeOfMessage = fields[5] ?? "",
            Security = fields[6] ?? "",
            MessageType = fields[7] ?? "",
            MessageControlId = fields[8] ?? "",
            ProcessingId = fields[9] ?? "",
            VersionId = fields[10] ?? "",
            SequenceNumber = fields[11] ?? "",
            ContinuationPointer = fields[12] ?? "",
            AcceptAcknowledgmentType = fields[13] ?? "",
            ApplicationAcknowledgmentType = fields[14] ?? "",
            CountryCode = fields[15] ?? "",
            CharacterSet = fields[16] ?? "",
            PrincipalLanguageOfMessage = fields[17] ?? ""
        };
    }

    public static HL7MessageHeader CreateForAcknoledgement(HL7MessageHeader originalHeader)
    {
        var originalHeaderMessageTypeVersion = originalHeader.MessageType.Split(originalHeader.ComponentSeparator.ToString())[1];

        return new HL7MessageHeader()
        {
            FieldSeparator = originalHeader.FieldSeparator,
            EncodingCharacters = originalHeader.EncodingCharacters,
            SendingApplication = originalHeader.ReceivingApplication,
            SendingFacility = originalHeader.ReceivingFacility,
            ReceivingApplication = originalHeader.SendingApplication,
            ReceivingFacility = originalHeader.SendingFacility,
            DateTimeOfMessage = DateTime.Now.ToString("yyyyMMddHHmmss"),
            Security = originalHeader.Security,
            MessageType = $"ACK^{originalHeaderMessageTypeVersion}",
            MessageControlId = originalHeader.MessageControlId,
            ProcessingId = originalHeader.ProcessingId,
            VersionId = originalHeader.VersionId,
            SequenceNumber = originalHeader.SequenceNumber,
            ContinuationPointer = originalHeader.ContinuationPointer,
            AcceptAcknowledgmentType = originalHeader.ApplicationAcknowledgmentType,
            ApplicationAcknowledgmentType = originalHeader.AcceptAcknowledgmentType,
            CountryCode = originalHeader.CountryCode,
            CharacterSet = originalHeader.CharacterSet,
            PrincipalLanguageOfMessage = originalHeader.PrincipalLanguageOfMessage
        };
    }
}
