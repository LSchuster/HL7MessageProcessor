using FluentAssertions;
using HL7MessageProcessor.ReadOnlySpanProcessor;

namespace HL7MessageProcessor.Tests;

public class ORUTests
{
    private readonly string _message;
    private readonly HL7Message _hl7Message;

    public ORUTests()
    {
        _message = File.ReadAllText("ExampleMessages\\ORU_1.hl7");
        _hl7Message = new HL7Message(_message);
    }

    [Fact]
    public void Test_IsValid()
    {
        var isValid = _hl7Message.IsValid();
    }

    [Fact]
    public void Test_Header_Parsing()
    {
        var msh = _hl7Message.MessageHeader;

        msh.Should().NotBeNull();

        msh.FieldSeparator.Should().Be("|");
        msh.ComponentSeparator.ToString().Should().Be("^");
        msh.SubComponentSeparator.ToString().Should().Be("&");
        msh.RepetitionSeparator.ToString().Should().Be("~");
        msh.EscapeCharacter.ToString().Should().Be("\\");

        _hl7Message.MessageHeader.ParsedDateTime.Should().NotBeNull();
    }

    [Fact]
    public void Test_Creating_ACK()
    {
        var ack = _hl7Message.MessageAck.ToString();
        ack.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Test_Transfrom()
    {
        var transformedMessage = _hl7Message.MessageTransformer.Transform();
        var message = transformedMessage.ToString();
        message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Test_GetValue_Level_1()
    {
        _hl7Message.GetValue("PV1", 44).Should().Be("20120410160227");
    }

    [Fact]
    public void Test_GetValue_Level_2()
    {
        _hl7Message.GetValue("PID", 11, 5).Should().Be("46808");
    }


    [Fact]
    public void Test_GetValue_Level_3()
    {
        _hl7Message.GetValue("PID", 3, 4, 1).Should().Be("MIE");
    }

    [Fact]
    public void Test_GetValues_Level_2()
    {
        var values = _hl7Message.GetValues("OBX", 3, 1);
        values.Should().BeEquivalentTo(["wbc", "neutros", "lymphs", "monos", "eo", "baso", "ig", "rbc", "hgb", "hct", "mcv", "mch", "mchc", "plt"]);
    }
}
