namespace HL7MessageProcessor.ReadOnlySpanProcessor
{

    /// <summary>
    /// Parses HL7 messages efficiently using ReadOnlySpan char to minimize allocations
    /// </summary>
    public static class HL7MessageParser
    {
        /// <summary>
        /// Extracts a specific value from an HL7 message based on segment name and element indices.
        /// </summary>
        /// <param name="message">The HL7 message as a ReadOnlySpan of type char.</param>
        /// <param name="segmentName">The 3-letter HL7 segment name (e.g., "PID").</param>
        /// <param name="elementIndex">The 1-based index of the element within the segment.</param>
        /// <param name="componentIndex">The 1-based index of the sub-element within the element (optional).</param>
        /// <param name="subComponentIndex">The 1-based index of the sub-sub-element within the sub-element (optional).</param>
        /// <returns>The extracted value as a string, or an empty string if not found.</returns>
        /// 
        public static string GetValue(ReadOnlySpan<char> message, HL7MessageHeader msh, string segmentName, int elementIndex, int componentIndex = 0, int subComponentIndex = 0)
        {
            while (!message.IsEmpty)
            {
                // Find the next segment boundary (`\r`, `\n`, or `\r\n`)
                int lineBreakPos = FindNextLineBreak(message, out int skipLength);
                var segment = TrimLeadingNewlines(message[..lineBreakPos]);

                // Check if the segment matches the requested name (e.g., "PID")
                if (message.StartsWith(segmentName))
                {
                    return ExtractElementValue(segment, msh, elementIndex, componentIndex, subComponentIndex).ToString();
                }

                // Stop if no more segments exist
                if (lineBreakPos == -1)
                {
                    break;
                }

                // Move to the next segment after the line break
                message = message.Slice(lineBreakPos + skipLength);
            }

            return string.Empty;
        }

        /// <summary>
        /// Retrieves multiple values from segments matching the given name.
        /// </summary>
        public static IReadOnlyList<string> GetValues(ReadOnlySpan<char> message, HL7MessageHeader msh, string segmentName, int elementIndex, int componentIndex = 0, int subComponentIndex = 0)
        {
            var values = new List<string>();

            while (!message.IsEmpty)
            {
                // Find the next segment boundary (`\r`, `\n`, or `\r\n`)
                int lineBreakPos = FindNextLineBreak(message, out int skipLength);
                var segment = TrimLeadingNewlines(message[..lineBreakPos]);

                // Check if the segment matches the requested name (e.g., "OBR")
                if (segment.StartsWith(segmentName))
                {
                    var segmentValue = ExtractElementValue(segment, msh, elementIndex, componentIndex, subComponentIndex).ToString();
                    if (!string.IsNullOrEmpty(segmentValue))
                    {
                        values.Add(segmentValue);
                    }
                }

                // Stop if no more segments exist
                if (lineBreakPos == -1)
                {
                    break;
                }

                // Move to the next segment after the line break
                message = message.Slice(lineBreakPos + skipLength);
            }

            return values;
        }

        private static ReadOnlySpan<char> ExtractElementValue(ReadOnlySpan<char> segment, HL7MessageHeader msh, int elementIndex, int componentIndex = 0, int subComponentIndex = 0)
        {
            int currentElementIndex = 0;

            while (!segment.IsEmpty)
            {
                // Locate the next element separator ('|')
                int separatorPos = segment.IndexOf(msh.FieldSeparator);

                // If the requested element is found, process its sub-elements if necessary
                if (currentElementIndex == elementIndex)
                {
                    if (separatorPos == -1)
                    {
                        separatorPos = segment.Length;
                    }

                    ReadOnlySpan<char> elementValue = segment[..separatorPos];

                    return componentIndex == 0
                        ? TrimSeparators(elementValue, msh)
                        : ExtractComponentValue(elementValue, msh, componentIndex, subComponentIndex);
                }

                // Move past the current element
                if (separatorPos == -1)
                {
                    break;
                }

                segment = segment.Slice(separatorPos + 1);
                currentElementIndex++;
            }

            return string.Empty;
        }

        private static ReadOnlySpan<char> ExtractComponentValue(ReadOnlySpan<char> component, HL7MessageHeader msh, int componentIndex, int subComponentIndex = 0)
        {
            int currentComponentIndex = 1;

            while (!component.IsEmpty)
            {
                int separatorPos = component.IndexOf(msh.ComponentSeparator);

                if (currentComponentIndex == componentIndex)
                {
                    if (separatorPos == -1)
                    {
                        separatorPos = component.Length;
                    }

                    ReadOnlySpan<char> componentValue = component[..separatorPos];

                    return subComponentIndex == 0
                        ? TrimSeparators(componentValue, msh)
                        : ExtractSubComponentValue(componentValue, msh, subComponentIndex);
                }

                if (separatorPos == -1)
                {
                    break;
                }

                component = component.Slice(separatorPos + 1);
                currentComponentIndex++;
            }

            return string.Empty;
        }

        private static ReadOnlySpan<char> ExtractSubComponentValue(ReadOnlySpan<char> subComponent, HL7MessageHeader msh, int subComponentIndex)
        {
            int currentSubComponentIndex = 1;

            while (!subComponent.IsEmpty)
            {
                int separatorPos = subComponent.IndexOf(msh.SubComponentSeparator);

                if (currentSubComponentIndex == subComponentIndex)
                {
                    return TrimSeparators(subComponent[..separatorPos], msh);
                }

                if (separatorPos == -1)
                {
                    break;
                }

                subComponent = subComponent.Slice(separatorPos + 1);
                currentSubComponentIndex++;
            }

            return string.Empty;
        }

        private static int FindNextLineBreak(ReadOnlySpan<char> message, out int skipLength)
        {
            int lineBreakPos = message.IndexOfAny('\r', '\n');

            if (lineBreakPos == -1)
            {
                skipLength = 0;
                return message.Length; // If no line break found, return the entire message
            }

            // Detect `\r\n` and skip both characters
            if (message.Length > lineBreakPos + 1 && message[lineBreakPos] == '\r' && message[lineBreakPos + 1] == '\n')
            {
                skipLength = 2; // Skip `\r\n`
            }
            else
            {
                skipLength = 1; // Skip `\r` or `\n`
            }

            return lineBreakPos;
        }

        private static ReadOnlySpan<char> TrimSeparators(ReadOnlySpan<char> value, HL7MessageHeader msh)
        {
            Span<char> separators = [msh.ComponentSeparator[0], msh.SubComponentSeparator[0]];

            int separatorPos = value.IndexOfAny(separators);

            return separatorPos > 0 ? value[..separatorPos] : value;
        }

        private static ReadOnlySpan<char> TrimLeadingNewlines(ReadOnlySpan<char> line)
        {
            while (!line.IsEmpty && (line[0] == '\r' || line[0] == '\n'))
            {
                line = line.Slice(1);
            }
            return line;
        }
    }
}
