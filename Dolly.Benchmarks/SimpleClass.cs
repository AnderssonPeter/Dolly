using FastCloner.SourceGenerator.Shared;

namespace Dolly.Benchmarks;

public class SimpleClassBase
{
    public int BaseInt { get; set; }
}

[Clonable]
[FastClonerClonable]
public partial class SimpleClass : SimpleClassBase
{
    public int Int { get; set; }
    public uint UInt { get; set; }
    public long Long { get; set; }
    public ulong ULong { get; set; }
    public double Double { get; set; }
    public float Float { get; set; }
    public string String { get; set; }
}

[Clonable]
[FastClonerClonable]
public partial class ComplexClass
{
    public SimpleClass SimpleClass { get; set; }
    public SimpleClass[] Array { get; set; }
    public List<SimpleClass> List { get; set; }
    //public IEnumerable<SimpleClass> IEnumerable { get; set; } //Not supported by CloneExtensions
}