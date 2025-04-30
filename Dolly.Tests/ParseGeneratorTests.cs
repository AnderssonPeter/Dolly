using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mono.Cecil.Cil;
using Snapshooter.TUnit;

namespace Dolly.Tests;
public class ParseGeneratorTests
{
    [Test]
    public void ParseSimpleClass()
    {
        var sourceCode = """
            namespace Dolly;
            [Clonable]
            public partial class SimpleClass
            {
                public string First { get; set; }
                public int Second { get; set; }
                [CloneIgnore]
                public float DontClone { get; set; }
            }
            """;

        VerifyGeneratedCode(sourceCode);
    }

    [Test]
    public void ParseSimpleSealedClass()
    {
        var sourceCode = """
            namespace Dolly;
            [Clonable]
            public sealed partial class SimpleClass
            {
                public string First { get; set; }
                public int Second { get; set; }
                [CloneIgnore]
                public float DontClone { get; set; }
            }
            """;

        VerifyGeneratedCode(sourceCode);
    }

    [Test]
    public void ParseSimpleSealedRecord()
    {
        var sourceCode = """
            namespace Dolly;

            [Clonable]
            public sealed partial record SimpleClass(string Foo);
            """;

        VerifyGeneratedCode(sourceCode);
    }

    [Test]
    public void ParseSimpleStruct()
    {
        var sourceCode = """
            namespace Dolly;
            [Clonable]
            public partial struct SimpleStruct
            {
                public string First { get; set; }
                public int Second { get; set; }
                [CloneIgnore]
                public float DontClone { get; set; }
            }
            """;

        VerifyGeneratedCode(sourceCode);
    }

    [Test]
    public void ParseCollectionsNotNullable()
    {
        var sourceCode = """
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
            """;

        VerifyGeneratedCode(sourceCode);
    }

    [Test]
    public void ParseCollectionMemberNullable()
    {
        var sourceCode = """
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
            """;

        VerifyGeneratedCode(sourceCode);
    }

    [Test]
    public void ParseCollectionElementNullable()
    {
        var sourceCode = """
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
            """;

        VerifyGeneratedCode(sourceCode);
    }

    [Test]
    public void ParseCollectionMemberAndElementNullable()
    {
        var sourceCode = """
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
            """;

        VerifyGeneratedCode(sourceCode);
    }

    [Test]
    public void ParseNullable()
    {
        var sourceCode = """
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
            """;

        VerifyGeneratedCode(sourceCode);
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
        var sourceCode = $$"""
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
            """;

        VerifyGeneratedCode(sourceCode, out var models);

        var complexClassModel = models.Single(m => m.Name == "ComplexClass");
        await Assert.That(complexClassModel.Flags).IsEquivalentTo(expected);
    }

    private void VerifyGeneratedCode([StringSyntax("c#-test")] string sourceCode) => VerifyGeneratedCode(sourceCode, out _);

    private void VerifyGeneratedCode([StringSyntax("c#-test")] string sourceCode, out IReadOnlyCollection<Model> models)
    {
        var stringBuilder = new StringBuilder();
        models = GetModels(sourceCode).ToArray();
        foreach (var model in models)
        {
            if (stringBuilder.Length > 0)
            {
                stringBuilder.AppendLine();
            }

            var generatedSourceText = SourceTextConverter.ToSourceText(model);
            stringBuilder.AppendLine(generatedSourceText.ToString());
        }

        var generatedCode = stringBuilder.Replace("\r\n", "\n").ToString();
        generatedCode.MatchSnapshot();
    }

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

    private IEnumerable<Model> GetModels(string code, Func<string, bool>? filter = null)
    {
        var compilation = CreateCompilation(code, true);
        var diagnostics = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error);

        if (diagnostics.Any())
        {
            throw new Exception("Failed to compile code, errors: " + string.Join(", ", diagnostics));
        }

        var syntaxTree = compilation.SyntaxTrees.Single(syntaxTree => syntaxTree.FilePath == "");
        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        var typeDeclarationNodes = syntaxTree
            .GetRoot()
            .RecursiveFlatten(n => n.ChildNodes())
            .Where(node => node is TypeDeclarationSyntax typeNode && (filter == null || filter(typeNode.Identifier.Text)));

        foreach (var typeDeclarationNode in typeDeclarationNodes)
        {
            var symbol = semanticModel.GetDeclaredSymbol(typeDeclarationNode);
            if (symbol is INamedTypeSymbol namedTypeSymbol)
            {
                if (Model.TryCreate(namedTypeSymbol, true, out var model, out var error))
                {
                    yield return model;
                }
                else
                {
                    throw new Exception("Failed to create model, error: " + error.Descriptor.Description);
                }
            }
        }

        if (!typeDeclarationNodes.Any())
        {
            throw new Exception("No valid type declarations found in the provided code.");
        }
    }
}
