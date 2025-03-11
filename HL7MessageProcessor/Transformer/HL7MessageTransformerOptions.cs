namespace HL7MessageProcessor.Transformer
{
    public class HL7MessageTransformerOptions
    {
        public bool IsEnabled { get; set; }
        public required List<HL7MessageTransformerOptionEntry> Options { get; set; }
    }

    public class HL7MessageTransformerOptionEntry
    {
        public required string SourceFieldIdentifier { get; set; }
        public required string TargetFieldIdentifier { get; set; }
    }
}
