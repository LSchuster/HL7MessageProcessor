using BenchmarkDotNet.Running;
using HL7MessageProcessorBenchmark;

Console.WriteLine("Running Benchmarks!");

BenchmarkRunner.Run<HL7MessageParserBenchmark>();

//BenchmarkRunner.Run<ReadOnlySpanProcessorBenchmarkWIP>();
//BenchmarkRunner.Run<ReadOnlySpanProcessorBenchmark>();
