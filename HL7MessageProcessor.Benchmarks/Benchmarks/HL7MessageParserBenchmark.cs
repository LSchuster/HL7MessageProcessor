using BenchmarkDotNet.Attributes;
using HL7MessageProcessor.ReadOnlySpanProcessor;

namespace HL7MessageProcessorBenchmark;

[MemoryDiagnoser]
public class HL7MessageParserBenchmark
{
    private string _message = string.Empty;

    [GlobalSetup]
    public void Setup()
    {
        _message = File.ReadAllText("ExampleMessages\\ORM_1.hl7");
    }

    [Benchmark]
    public void BenchmarkIsValid()
    {
        var hl7Message = new HL7Message(_message);
        hl7Message.IsValid();
    }

    [Benchmark]
    public void BenchmarkCreateAck()
    {
        var hl7Message = new HL7Message(_message);
        var ackString = hl7Message.MessageAck.ToString();
    }

    [Benchmark]
    public void BenchmarkGetDate()
    {
        var hl7Message = new HL7Message(_message);
        var date = hl7Message.MessageHeader.ParsedDateTime;
    }

    [Benchmark]
    public void BenchmarkGetDateAndHeader()
    {
        var hl7Message = new HL7Message(_message);
        var header = hl7Message.MessageHeader;
        var date = hl7Message.MessageHeader.ParsedDateTime;
    }

    [Benchmark]
    public void GetValue_Level_1()
    {
        var hl7Message = new HL7Message(_message);
        hl7Message.GetValue("PV1", 3);
    }

    [Benchmark]
    public void GetValue_Level_1_Times_100()
    {
        var hl7Message = new HL7Message(_message);

        for (int i = 0; i < 100; i++)
        {
            hl7Message.GetValue("PV1", 3);
        }
    }

    [Benchmark]
    public void GetValue_Level_2()
    {
        var hl7Message = new HL7Message(_message);
        hl7Message.GetValue("PV1", 3, 2);
    }

    [Benchmark]
    public void GetValue_Level_2_Times_100()
    {
        var hl7Message = new HL7Message(_message);

        for (int i = 0; i < 100; i++)
        {
            hl7Message.GetValue("PV1", 3, 2);
        }
    }

    [Benchmark]
    public void GetValue_Level_3()
    {
        var hl7Message = new HL7Message(_message);
        hl7Message.GetValue("PID", 3, 4, 2);
    }

    [Benchmark]
    public void GetValue_Level_3_Times_100()
    {
        var hl7Message = new HL7Message(_message);

        for (int i = 0; i < 100; i++)
        {
            hl7Message.GetValue("PID", 3, 4, 2);
        }
    }
}