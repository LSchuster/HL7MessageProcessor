using System.Globalization;

namespace HL7MessageProcessor.ReadOnlySpanProcessor;

public class HL7MessageHeader
{
    private const int ComponentSeparatorIndex = 0;
    private const int RepetitionSeparatorIndex = 1;
    private const int EscapeCharacterIndex = 2;
    private const int SubComponentSeparatorIndex = 3;

    public required string FieldSeparator { get; init; }
    public required string EncodingCharacters { get; init; }
    public required string SendingApplication { get; init; }
    public required string SendingFacility { get; init; }
    public required string ReceivingApplication { get; init; }
    public required string ReceivingFacility { get; init; }
    public required string DateTimeOfMessage { get; init; }
    public required string Security { get; init; }
    public required string MessageType { get; init; }
    public required string MessageControlId { get; init; }
    public required string ProcessingId { get; init; }
    public required string VersionId { get; init; }
    public required string SequenceNumber { get; init; }
    public required string ContinuationPointer { get; init; }
    public required string AcceptAcknowledgmentType { get; init; }
    public required string ApplicationAcknowledgmentType { get; init; }
    public required string CountryCode { get; init; }
    public required string CharacterSet { get; init; }
    public required string PrincipalLanguageOfMessage { get; init; }

    /// <summary>
    /// Gets the HL7 EncodingCharacters (MSH-2) and extracts the Component Separator.
    /// Returns null if EncodingCharacters is null
    /// </summary>
    public ReadOnlySpan<char> ComponentSeparator
    {
        get
        {
            if (string.IsNullOrWhiteSpace(EncodingCharacters))
            {
                return null;
            }

            if (EncodingCharacters.Length > ComponentSeparatorIndex)
            {
                return new ReadOnlySpan<char>([EncodingCharacters[ComponentSeparatorIndex]]);
            }

            return null; // Not present
        }
    }

    /// <summary>
    /// Gets the HL7 EncodingCharacters (MSH-2) and extracts the Sub Component Separator.
    /// Returns null if EncodingCharacters is null
    /// </summary>
    public ReadOnlySpan<char> SubComponentSeparator
    {
        get
        {
            if (string.IsNullOrWhiteSpace(EncodingCharacters))
            {
                return null;
            }

            if (EncodingCharacters.Length > SubComponentSeparatorIndex)
            {
                return new ReadOnlySpan<char>([EncodingCharacters[SubComponentSeparatorIndex]]);
            }

            return null; // Not present
        }
    }

    /// <summary>
    /// Gets the HL7 EncodingCharacters (MSH-2) and extracts the Escape Charakter.
    /// Returns null if EncodingCharacters is null
    /// </summary>
    public ReadOnlySpan<char> EscapeCharacter
    {
        get
        {
            if (string.IsNullOrWhiteSpace(EncodingCharacters))
            {
                return null;
            }

            if (EncodingCharacters.Length > EscapeCharacterIndex)
            {
                return new ReadOnlySpan<char>([EncodingCharacters[EscapeCharacterIndex]]);
            }

            return null; // Not present
        }
    }

    /// <summary>
    /// Gets the HL7 EncodingCharacters (MSH-2) and extracts the Repetition Separator.
    /// Returns null if EncodingCharacters is null
    /// </summary>
    public ReadOnlySpan<char> RepetitionSeparator
    {
        get
        {
            if (string.IsNullOrWhiteSpace(EncodingCharacters))
            {
                return null;
            }

            if (EncodingCharacters.Length > RepetitionSeparatorIndex)
            {
                return new ReadOnlySpan<char>([EncodingCharacters[RepetitionSeparatorIndex]]);
            }

            return null; // Not present
        }
    }

    /// <summary>
    /// Parses the HL7 DateTime (MSH-7) field into a DateTime object.
    /// Returns null if parsing fails.
    /// </summary>
    public DateTime? ParsedDateTime
    {
        get
        {
            if (string.IsNullOrWhiteSpace(DateTimeOfMessage))
                return null;

            string[] formats = {
                    "yyyyMMddHHmmss",   // Standard format with seconds
                    "yyyyMMddHHmm",     // Without seconds
                    "yyyyMMdd"          // Date only
                };

            if (DateTime.TryParseExact(DateTimeOfMessage, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }

            return null; // Invalid format
        }
    }
}
