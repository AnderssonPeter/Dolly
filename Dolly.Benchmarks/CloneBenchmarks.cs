using BenchmarkDotNet.Attributes;
using CloneExtensions;
using Force.DeepCloner;
using NClone;

namespace Dolly.Benchmarks;

[MemoryDiagnoser]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
public class CloneBenchmarks
{
    private readonly ComplexClass _complexClass;

    public CloneBenchmarks()
    {
        _complexClass = new ComplexClass
        {
            SimpleClass = new SimpleClass()
            {
                Int = 10,
                UInt = 1231,
                Long = 1231234561L,
                ULong = 1516524352UL,
                Double = 1235.1235762,
                Float = 1.333F,
                String = "Lorem ipsum ...",
            },
            Array = [
                new SimpleClass()
                {
                    Int = 10,
                    UInt = 1231,
                    Long = 1231234561L,
                    ULong = 1516524352UL,
                    Double = 1235.1235762,
                    Float = 1.333F,
                    String = "Lorem ipsum ...",
                },
                new SimpleClass()
                {
                    Int = 10,
                    UInt = 1231,
                    Long = 1231234561L,
                    ULong = 1516524352UL,
                    Double = 1235.1235762,
                    Float = 1.333F,
                    String = "Lorem ipsum ...",
                },
            ],
            List = [
                new SimpleClass()
                {
                    Int = 10,
                    UInt = 1231,
                    Long = 1231234561L,
                    ULong = 1516524352UL,
                    Double = 1235.1235762,
                    Float = 1.333F,
                    String = "Lorem ipsum ...",
                },
                new SimpleClass()
                {
                    Int = 10,
                    UInt = 1231,
                    Long = 1231234561L,
                    ULong = 1516524352UL,
                    Double = 1235.1235762,
                    Float = 1.333F,
                    String = "Lorem ipsum ...",
                },
            ]
        };
    }

    [Benchmark(Baseline = true, Description = "Dolly")]
    public void TestDolly()
    {
        var clone = _complexClass.DeepClone();
    }

    [Benchmark(Description = "CloneExtensions")]
    public void TestCloneExtensions()
    {
        var clone = CloneFactory.GetClone(_complexClass);
    }

    [Benchmark(Description = "DeepCloner")]
    public void TestDeepCloner()
    {
        DeepClonerExtensions.DeepClone(_complexClass);
    }

    [Benchmark(Description = "FastDeepCloner")]
    public void TestFastDeepCloner()
    {
        var clone = FastDeepCloner.DeepCloner.Clone(_complexClass);
    }

    [Benchmark(Description = "NClone")]
    public void TestNClone()
    {
        var clone = Clone.ObjectGraph(_complexClass);
    }

    [Benchmark(Description = "AnyClone")]
    public void TestAnyClone()
    {
        var clone = AnyClone.CloneExtensions.Clone(_complexClass);
    }
    
    [Benchmark(Description = "FastCloner")]
    public void TestFastCloner()
    {
        var clone = _complexClass.FastDeepClone();
    }
}
