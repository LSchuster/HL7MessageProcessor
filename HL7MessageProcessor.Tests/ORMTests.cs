using FluentAssertions;
using HL7MessageProcessor.ReadOnlySpanProcessor;

namespace HL7MessageProcessor.Tests;

public class ORMTests
{
    private readonly string _message;
    private readonly HL7Message _hl7Message;

    public ORMTests()
    {
        _message = File.ReadAllText("ExampleMessages\\ORM_1.hl7");
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
        _hl7Message.GetValue("PV1", 3).Should().Be("7");
    }

    [Fact]
    public void Test_GetValue_Level_2()
    {
        _hl7Message.GetValue("PV1", 3, 2).Should().Be("Disney");
    }


    [Fact]
    public void Test_GetValue_Level_3()
    {
        _hl7Message.GetValue("PID", 3, 4, 2).Should().Be("1.2.840.114398.1.100");
    }

    [Fact]
    public void Test_GetValues_Level_2()
    {
        _hl7Message.GetValues("OBR", 4, 2).Should().BeEquivalentTo(["CREATININE", "LIPID PROFILE"]);
    }
}
