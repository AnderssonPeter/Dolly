using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dolly.Tests;
public class ParseGeneratorTests
{
    [Test]
    public async Task ParseSimpleClass()
    {
        var model = GetModel(@"
namespace Dolly;
[Clonable]
public partial class SimpleClass
{
    public string First { get; set; }
    public int Second { get; set; }
    [CloneIgnore]
    public float DontClone { get; set; }
}
");
        var expected = new Model("Dolly", "SimpleClass", ModelFlags.None, new Member[] {
            new Member("First", false, MemberFlags.None),
            new Member("Second", false, MemberFlags.None)
        }, EquatableArray<Member>.Empty());

        await Assert.That(model).IsEquivalentTo(expected);
    }

    [Test]
    public async Task ParseSimpleSealedClass()
    {
        var model = GetModel(@"
namespace Dolly;
[Clonable]
public sealed partial class SimpleClass
{
    public string First { get; set; }
    public int Second { get; set; }
    [CloneIgnore]
    public float DontClone { get; set; }
}
");
        var generated = SourceTextConverter.ToSourceText(model);
        var expected =
            """
            using global::System.Linq;
            namespace Dolly;
            partial class SimpleClass : global::Dolly.IClonable<SimpleClass>
            {
                object global::System.ICloneable.Clone() => this.DeepClone();
                public global::Dolly.SimpleClass DeepClone() =>
                    new ()
                    {
                        First = First,
                        Second = Second
                    };

                public global::Dolly.SimpleClass ShallowClone() =>
                    new ()
                    {
                        First = First,
                        Second = Second
                    };
            }
            """.Replace("\r\n", "\n");

        await Assert.That(generated.ToString().Replace("\r\n", "\n")).IsEquivalentTo(expected);
    }

    [Test]
    public async Task ParseSimpleSealedRecord()
    {
        var model = GetModel(@"
namespace Dolly;
[Clonable]
public sealed partial record SimpleClass(string Foo);
");
        var generated = SourceTextConverter.ToSourceText(model);
        var expected =
            """
            using global::System.Linq;
            namespace Dolly;
            partial record SimpleClass : global::Dolly.IClonable<SimpleClass>
            {
                object global::System.ICloneable.Clone() => this.DeepClone();
                public global::Dolly.SimpleClass DeepClone() =>
                    new (Foo)
                    {

                    };

                public global::Dolly.SimpleClass ShallowClone() =>
                    new (Foo)
                    {

                    };
            }
            """.Replace("\r\n", "\n");

        await Assert.That(generated.ToString().Replace("\r\n", "\n")).IsEquivalentTo(expected);
    }

    [Test]
    public async Task ParseSimpleStruct()
    {
        var model = GetModel(@"
namespace Dolly;
[Clonable]
public partial struct SimpleStruct
{
    public string First { get; set; }
    public int Second { get; set; }
    [CloneIgnore]
    public float DontClone { get; set; }
}
");
        var expected = new Model("Dolly", "SimpleStruct", ModelFlags.Struct | ModelFlags.IsSealed, new Member[] {
            new Member("First", false, MemberFlags.None),
            new Member("Second", false, MemberFlags.None)
        }, EquatableArray<Member>.Empty());

        await Assert.That(model).IsEquivalentTo(expected);
    }

    [Test]
    public async Task ParseCollectionsNotNullable()
    {
        var model = GetModel(@"
using System.Collections.Generic;
namespace Dolly;
[Clonable]
public partial class SimpleClass
{
    public string First { get; set; }
    public int Second { get; set; }
    [CloneIgnore]
    public float DontClone { get; set; }
}

[Clonable]
public partial struct SimpleStruct
{
    public string First { get; set; }
    public int Second { get; set; }
    [CloneIgnore]
    public float DontClone { get; set; }
}

[Clonable]
public partial class ComplexClass
{
    public int[] IntArray { get; set; }
    public List<int> IntList { get; set; }
    public IEnumerable<int> IntIEnumerable { get; set; }

    public string[] StringArray { get; set; }
    public List<string> StringList { get; set; }
    public IEnumerable<string> StringIEnumerable { get; set; }

    public SimpleClass[] ReferenceArray { get; set; }
    public List<SimpleClass> ReferenceList { get; set; }
    public IEnumerable<SimpleClass> ReferenceIEnumerable { get; set; }

    public SimpleStruct[] ValueArray { get; set; }
    public List<SimpleStruct> ValueList { get; set; }
    public IEnumerable<SimpleStruct> ValueIEnumerable { get; set; }
}
", name => name == "ComplexClass");
        var expected = new Model("Dolly", "ComplexClass", ModelFlags.None, new Member[] {

            new Member("IntArray", false, MemberFlags.Enumerable),
            new Member("IntList", false, MemberFlags.Enumerable | MemberFlags.NewCollection),
            new Member("IntIEnumerable", false, MemberFlags.Enumerable),

            new Member("StringArray", false, MemberFlags.Enumerable),
            new Member("StringList", false, MemberFlags.Enumerable | MemberFlags.NewCollection),
            new Member("StringIEnumerable", false, MemberFlags.Enumerable),

            new Member("ReferenceArray", false, MemberFlags.Clonable | MemberFlags.Enumerable),
            new Member("ReferenceList", false, MemberFlags.Clonable | MemberFlags.Enumerable | MemberFlags.NewCollection),
            new Member("ReferenceIEnumerable", false, MemberFlags.Clonable | MemberFlags.Enumerable),

            new Member("ValueArray", false, MemberFlags.Clonable | MemberFlags.Enumerable),
            new Member("ValueList", false, MemberFlags.Clonable | MemberFlags.Enumerable | MemberFlags.NewCollection),
            new Member("ValueIEnumerable", false, MemberFlags.Clonable | MemberFlags.Enumerable)
        }, EquatableArray<Member>.Empty());

        await Assert.That(model).IsEquivalentTo(expected);
    }

    [Test]
    public async Task ParseCollectionMemberNullable()
    {
        var model = GetModel(@"
using System.Collections.Generic;
namespace Dolly;
[Clonable]
public partial class SimpleClass
{
    public string First { get; set; }
    public int Second { get; set; }
    [CloneIgnore]
    public float DontClone { get; set; }
}

[Clonable]
public partial struct SimpleStruct
{
    public string First { get; set; }
    public int Second { get; set; }
    [CloneIgnore]
    public float DontClone { get; set; }
}

[Clonable]
public partial class ComplexClass
{
    public int[]? IntArray { get; set; }
    public List<int>? IntList { get; set; }
    public IEnumerable<int>? IntIEnumerable { get; set; }

    public string[]? StringArray { get; set; }
    public List<string>? StringList { get; set; }
    public IEnumerable<string>? StringIEnumerable { get; set; }

    public SimpleClass[]? ReferenceArray { get; set; }
    public List<SimpleClass>? ReferenceList { get; set; }
    public IEnumerable<SimpleClass>? ReferenceIEnumerable { get; set; }

    public SimpleStruct[]? ValueArray { get; set; }
    public List<SimpleStruct>? ValueList { get; set; }
    public IEnumerable<SimpleStruct>? ValueIEnumerable { get; set; }
}
", name => name == "ComplexClass");
        var expected = new Model("Dolly", "ComplexClass", ModelFlags.None, new Member[] {

            new Member("IntArray", false, MemberFlags.Enumerable | MemberFlags.MemberNullable),
            new Member("IntList", false, MemberFlags.Enumerable | MemberFlags.NewCollection | MemberFlags.MemberNullable),
            new Member("IntIEnumerable", false, MemberFlags.Enumerable | MemberFlags.MemberNullable),

            new Member("StringArray", false, MemberFlags.Enumerable | MemberFlags.MemberNullable),
            new Member("StringList", false, MemberFlags.Enumerable | MemberFlags.NewCollection | MemberFlags.MemberNullable),
            new Member("StringIEnumerable", false, MemberFlags.Enumerable | MemberFlags.MemberNullable),

            new Member("ReferenceArray", false, MemberFlags.Clonable | MemberFlags.Enumerable | MemberFlags.MemberNullable),
            new Member("ReferenceList", false, MemberFlags.Clonable | MemberFlags.Enumerable | MemberFlags.NewCollection | MemberFlags.MemberNullable),
            new Member("ReferenceIEnumerable", false, MemberFlags.Clonable | MemberFlags.Enumerable | MemberFlags.MemberNullable),

            new Member("ValueArray", false, MemberFlags.Clonable | MemberFlags.Enumerable | MemberFlags.MemberNullable),
            new Member("ValueList", false, MemberFlags.Clonable | MemberFlags.Enumerable | MemberFlags.NewCollection | MemberFlags.MemberNullable),
            new Member("ValueIEnumerable", false, MemberFlags.Clonable | MemberFlags.Enumerable | MemberFlags.MemberNullable)
        }, EquatableArray<Member>.Empty());

        await Assert.That(model).IsEquivalentTo(expected);
    }

    [Test]
    public async Task ParseCollectionElementNullable()
    {
        var model = GetModel(@"
using System.Collections.Generic;
namespace Dolly;
[Clonable]
public partial class SimpleClass
{
    public string First { get; set; }
    public int Second { get; set; }
    [CloneIgnore]
    public float DontClone { get; set; }
}

[Clonable]
public partial struct SimpleStruct
{
    public string First { get; set; }
    public int Second { get; set; }
    [CloneIgnore]
    public float DontClone { get; set; }
}

[Clonable]
public partial class ComplexClass
{
    public int?[] IntArray { get; set; }
    public List<int?> IntList { get; set; }
    public IEnumerable<int?> IntIEnumerable { get; set; }

    public string?[] StringArray { get; set; }
    public List<string?> StringList { get; set; }
    public IEnumerable<string?> StringIEnumerable { get; set; }

    public SimpleClass?[] ReferenceArray { get; set; }
    public List<SimpleClass?> ReferenceList { get; set; }
    public IEnumerable<SimpleClass?> ReferenceIEnumerable { get; set; }

    public SimpleStruct?[] ValueArray { get; set; }
    public List<SimpleStruct?> ValueList { get; set; }
    public IEnumerable<SimpleStruct?> ValueIEnumerable { get; set; }
}
", name => name == "ComplexClass");
        var expected = new Model("Dolly", "ComplexClass", ModelFlags.None, new Member[] {

            new Member("IntArray", false, MemberFlags.Enumerable | MemberFlags.ElementNullable),
            new Member("IntList", false, MemberFlags.Enumerable | MemberFlags.NewCollection | MemberFlags.ElementNullable),
            new Member("IntIEnumerable", false, MemberFlags.Enumerable | MemberFlags.ElementNullable),

            new Member("StringArray", false, MemberFlags.Enumerable | MemberFlags.ElementNullable),
            new Member("StringList", false, MemberFlags.Enumerable | MemberFlags.NewCollection | MemberFlags.ElementNullable),
            new Member("StringIEnumerable", false, MemberFlags.Enumerable | MemberFlags.ElementNullable),

            new Member("ReferenceArray", false, MemberFlags.Clonable | MemberFlags.Enumerable | MemberFlags.ElementNullable),
            new Member("ReferenceList", false, MemberFlags.Clonable | MemberFlags.Enumerable | MemberFlags.NewCollection | MemberFlags.ElementNullable),
            new Member("ReferenceIEnumerable", false, MemberFlags.Clonable | MemberFlags.Enumerable | MemberFlags.ElementNullable),

            new Member("ValueArray", false, MemberFlags.Clonable | MemberFlags.Enumerable | MemberFlags.ElementNullable),
            new Member("ValueList", false, MemberFlags.Clonable | MemberFlags.Enumerable | MemberFlags.NewCollection | MemberFlags.ElementNullable),
            new Member("ValueIEnumerable", false, MemberFlags.Clonable | MemberFlags.Enumerable | MemberFlags.ElementNullable)
        }, EquatableArray<Member>.Empty());

        await Assert.That(model).IsEquivalentTo(expected);
    }

    [Test]
    public async Task ParseCollectionMemberAndElementNullable()
    {
        var model = GetModel(@"
using System.Collections.Generic;
namespace Dolly;
[Clonable]
public partial class SimpleClass
{
    public string First { get; set; }
    public int Second { get; set; }
    [CloneIgnore]
    public float DontClone { get; set; }
}

[Clonable]
public partial struct SimpleStruct
{
    public string First { get; set; }
    public int Second { get; set; }
    [CloneIgnore]
    public float DontClone { get; set; }
}

[Clonable]
public partial class ComplexClass
{
    public int?[]? IntArray { get; set; }
    public List<int?>? IntList { get; set; }
    public IEnumerable<int?>? IntIEnumerable { get; set; }

    public string?[]? StringArray { get; set; }
    public List<string?>? StringList { get; set; }
    public IEnumerable<string?>? StringIEnumerable { get; set; }

    public SimpleClass?[]? ReferenceArray { get; set; }
    public List<SimpleClass?>? ReferenceList { get; set; }
    public IEnumerable<SimpleClass?>? ReferenceIEnumerable { get; set; }

    public SimpleStruct?[]? ValueArray { get; set; }
    public List<SimpleStruct?>? ValueList { get; set; }
    public IEnumerable<SimpleStruct?>? ValueIEnumerable { get; set; }
}
", name => name == "ComplexClass");
        var expected = new Model("Dolly", "ComplexClass", ModelFlags.None, new Member[] {
            new Member("IntArray", false, MemberFlags.Enumerable | MemberFlags.MemberNullable | MemberFlags.ElementNullable),
            new Member("IntList", false, MemberFlags.Enumerable | MemberFlags.NewCollection | MemberFlags.MemberNullable | MemberFlags.ElementNullable),
            new Member("IntIEnumerable", false, MemberFlags.Enumerable | MemberFlags.MemberNullable | MemberFlags.ElementNullable),

            new Member("StringArray", false, MemberFlags.Enumerable | MemberFlags.MemberNullable | MemberFlags.ElementNullable),
            new Member("StringList", false, MemberFlags.Enumerable | MemberFlags.NewCollection | MemberFlags.MemberNullable | MemberFlags.ElementNullable),
            new Member("StringIEnumerable", false, MemberFlags.Enumerable | MemberFlags.MemberNullable | MemberFlags.ElementNullable),

            new Member("ReferenceArray", false, MemberFlags.Clonable | MemberFlags.Enumerable | MemberFlags.MemberNullable | MemberFlags.ElementNullable),
            new Member("ReferenceList", false, MemberFlags.Clonable | MemberFlags.Enumerable | MemberFlags.NewCollection | MemberFlags.MemberNullable | MemberFlags.ElementNullable),
            new Member("ReferenceIEnumerable", false, MemberFlags.Clonable | MemberFlags.MemberNullable | MemberFlags.Enumerable | MemberFlags.ElementNullable),

            new Member("ValueArray", false, MemberFlags.Clonable | MemberFlags.Enumerable | MemberFlags.MemberNullable | MemberFlags.ElementNullable),
            new Member("ValueList", false, MemberFlags.Clonable | MemberFlags.Enumerable | MemberFlags.NewCollection | MemberFlags.MemberNullable | MemberFlags.ElementNullable),
            new Member("ValueIEnumerable", false, MemberFlags.Clonable | MemberFlags.Enumerable | MemberFlags.MemberNullable | MemberFlags.ElementNullable)
        }, EquatableArray<Member>.Empty());

        await Assert.That(model).IsEquivalentTo(expected);
    }

    [Test]
    public async Task ParseNullable()
    {
        var model = GetModel(@"
using System.Collections.Generic;
namespace Dolly;
[Clonable]
public partial class SimpleClass
{
    public string First { get; set; }
    public int Second { get; set; }
    [CloneIgnore]
    public float DontClone { get; set; }
}

[Clonable]
public partial struct SimpleStruct
{
    public string First { get; set; }
    public int Second { get; set; }
    [CloneIgnore]
    public float DontClone { get; set; }
}

[Clonable]
public partial class ComplexClass
{
    public SimpleClass ReferenceTypeNotNull { get; set; }
    public SimpleClass? ReferenceTypeNull { get; set; }
    public string StringReferenceTypeNotNull { get; set; }
    public string? StringReferenceTypeNull { get; set; }
    public SimpleStruct ValueTypeNotNull { get; set; }
    public SimpleStruct? ValueTypeNull { get; set; }
    public int IntValueTypeNotNull { get; set; }
    public int? IntValueTypeNull { get; set; }
}
", name => name == "ComplexClass");
        var expected = new Model("Dolly", "ComplexClass", ModelFlags.None, new Member[] {
            new Member("ReferenceTypeNotNull", false, MemberFlags.Clonable),
            new Member("ReferenceTypeNull", false, MemberFlags.Clonable | MemberFlags.MemberNullable),
            new Member("StringReferenceTypeNotNull", false, MemberFlags.None),
            new Member("StringReferenceTypeNull", false, MemberFlags.MemberNullable),

            new Member("ValueTypeNotNull", false, MemberFlags.Clonable),
            new Member("ValueTypeNull", false, MemberFlags.Clonable | MemberFlags.MemberNullable),
            new Member("IntValueTypeNotNull", false, MemberFlags.None),
            new Member("IntValueTypeNull", false, MemberFlags.MemberNullable)
        }, EquatableArray<Member>.Empty());

        await Assert.That(model).IsEquivalentTo(expected);
    }


    [Test]
    [Arguments("class", false, ModelFlags.None)]
    [Arguments("record", false, ModelFlags.Record)]
    [Arguments("record struct", false, ModelFlags.Record | ModelFlags.Struct | ModelFlags.IsSealed)]
    [Arguments("record", true, ModelFlags.Record | ModelFlags.ClonableBase)]
    [Arguments("struct", false, ModelFlags.Struct | ModelFlags.IsSealed)]
    // [Arguments("struct", true, ModelFlags.Struct | ModelFlags.ClonableBase)] // Cannot occur
    [Arguments("class", true, ModelFlags.ClonableBase)]
    public async Task ParseModelFlags(string modifiers, bool hasClonableBase, ModelFlags expected)
    {
        var model = GetModel($$"""
using System.Collections.Generic;
namespace Dolly;
[Clonable]
public partial {{modifiers}} SimpleClass
{
}

[Clonable]
public partial {{modifiers}} ComplexClass{{(hasClonableBase ? ": SimpleClass" : "")}}
{
}
""", name => name == "ComplexClass");

        await Assert.That(model.Flags).IsEquivalentTo(expected);
    }

    //Ignore attribute
    //Ctor tests

    //[Test]
    //public async Task TestGenerator()
    //{
    //    Compilation inputCompilation = CreateCompilation();
    //    var syntaxTree = inputCompilation.SyntaxTrees.Single();

    //    var semanticModel = inputCompilation.GetSemanticModel(syntaxTree);

    //    var node = syntaxTree.GetRoot().RecursiveFlatten(n => n.ChildNodes()).OfType<ClassDeclarationSyntax>().Single();
    //    var symbol = semanticModel.GetDeclaredSymbol(node);
    //    if (symbol is INamedTypeSymbol namedTypeSymbol)
    //    {
    //        if (Model.TryCreate(symbol, true, out var model, out var error))
    //        {

    //        }
    //    }

    //    //todo: how do we set nullability when using generator
    //    var generator = new DollyGenerator();

    //    GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
    //    driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

    //    // We can now assert things about the resulting compilation:
    //    await Assert.That(diagnostics).IsEmpty();
    //    await Assert.That(outputCompilation.SyntaxTrees).HasCount().EqualTo(2);
    //    await Assert.That(outputCompilation.GetDiagnostics()).IsEmpty();

    //    // Or we can look at the results directly:
    //    GeneratorDriverRunResult runResult = driver.GetRunResult();

    //    // The runResult contains the combined results of all generators passed to the driver
    //    await Assert.That(runResult.GeneratedTrees).HasCount().EqualToZero();
    //    await Assert.That(runResult.Diagnostics).IsEmpty();

    //    // Or you can access the individual results on a by-generator basis
    //    GeneratorRunResult generatorResult = runResult.Results[0];
    //    await Assert.That(generatorResult.Generator == generator).IsTrue();
    //    await Assert.That(generatorResult.Diagnostics).IsEmpty();
    //    await Assert.That(generatorResult.GeneratedSources).HasCount().EqualTo(1);
    //    await Assert.That(generatorResult.Exception).IsNull();
    //}


    private static Compilation CreateCompilation(string source, bool addAttributes)
           => CSharpCompilation.Create("compilation",
               addAttributes ? [
                   CSharpSyntaxTree.ParseText(source),
                   CSharpSyntaxTree.ParseText(DollyGenerator.ClonableAttribute, path: "ClonableAttribute.g.cs"),
                   CSharpSyntaxTree.ParseText(DollyGenerator.CloneIgnoreAttribute, path: "CloneIgnoreAttribute.g.cs")
                   ] :
               [CSharpSyntaxTree.ParseText(source)],
               new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
               new CSharpCompilationOptions(
                   OutputKind.NetModule,
                   nullableContextOptions: NullableContextOptions.Enable));


    private Model GetModel(string code, Func<string, bool>? filter = null)
    {
        var compilation = CreateCompilation(code, true);
        var diagnostics = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error);
        if (diagnostics.Any())
        {
            throw new Exception("Failed to compile code, errors:" + string.Join(", ", diagnostics));
        }

        var syntaxTree = compilation.SyntaxTrees.Single(syntaxTree => syntaxTree.FilePath == "");

        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        var allNodes = syntaxTree
            .GetRoot()
            .RecursiveFlatten(n => n.ChildNodes())
            .ToArray();

        var node = syntaxTree
            .GetRoot()
            .RecursiveFlatten(n => n.ChildNodes())
            .Single(node =>
            (node is ClassDeclarationSyntax classNode && (filter == null || filter(classNode.Identifier.Text))) ||
            (node is RecordDeclarationSyntax recordNode && (filter == null || filter(recordNode.Identifier.Text))) ||
            (node is StructDeclarationSyntax structNode && (filter == null || filter(structNode.Identifier.Text))));
        var symbol = semanticModel.GetDeclaredSymbol(node);
        if (symbol is INamedTypeSymbol namedTypeSymbol)
        {
            if (Model.TryCreate(namedTypeSymbol, true, out var model, out var error))
            {
                return model;
            }
            throw new Exception("Failed to create model, error: " + error.Descriptor.Description);
        }

        throw new Exception("Symbol is not of type INamedTypeSymbol");
    }
}
