using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Vogen.Diagnostics;
using Vogen.Rules;

namespace AnalyzerTests;

public sealed class DoNotUseUninitializedMembersAnalyzerTests
{
    /// <summary>
    /// A stub of the Vogen attributes so that we don't have to reference the Vogen assembly.
    /// </summary>
    private const string VogenStub = """
                                     namespace Vogen
                                     {
                                         [System.AttributeUsage(System.AttributeTargets.Struct | System.AttributeTargets.Class)]
                                         public class ValueObjectAttribute : System.Attribute { }
                                         public sealed class ValueObjectAttribute<T> : ValueObjectAttribute { }
                                         public sealed class SetsUninitializedMembersAttribute : System.Attribute { }
                                         public sealed class MayBeUninitializedAttribute : System.Attribute { }
                                     }
                                     """;


    public static readonly TheoryData<string> AllVogenDeclarations =
    [
        "partial class",
        "partial struct",
        "readonly partial struct",
        "partial record class",
        "partial record struct",
        "readonly partial record struct"
    ];

    private static CSharpAnalyzerTest<DoNotUseUninitializedMembersAnalyzer, DefaultVerifier> CreateAnalyzerTest(
        string testCode) =>
        new()
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
            TestState =
            {
                Sources = { VogenStub, testCode }
            }
        };

    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Reports_AutoProperty_Without_Initializer(string vogenTypeDeclaration)
    {
        var code = $$"""
                     using Vogen;
                     [ValueObject<int>] public {{vogenTypeDeclaration}} MyVO;
                     public sealed class C
                     {
                         public MyVO {|{{RuleIdentifiers.DoNotUseUninitializedMembers}}:SomeProperty|} { get; set; }
                     }
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }

    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Reports_Field_Without_Initializer(string vogenTypeDeclaration)
    {
        var code = $$"""
                     using Vogen;
                     [ValueObject<int>] public {{vogenTypeDeclaration}} MyVO;
                     public sealed class C
                     {
                         public MyVO {|{{RuleIdentifiers.DoNotUseUninitializedMembers}}:_field|};
                     }
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }

    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Reports_When_Constructor_Does_Not_Assign(string vogenTypeDeclaration)
    {
        var code = $$"""
                     using Vogen;
                     [ValueObject<int>] public {{vogenTypeDeclaration}} MyVO;
                     public sealed class C
                     {
                         public MyVO {|{{RuleIdentifiers.DoNotUseUninitializedMembers}}:P|} { get; set; }
                         public C() { }
                     }
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }

    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Reports_Get_Only(string vogenTypeDeclaration)
    {
        var code = $$"""
                     using Vogen;
                     [ValueObject<int>] public {{vogenTypeDeclaration}} MyVO;
                     public sealed class C
                     {
                         public MyVO {|{{RuleIdentifiers.DoNotUseUninitializedMembers}}:P|} { get; }
                     }
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }

    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Reports_Init(string vogenTypeDeclaration)
    {
        var code = $$"""
                     using Vogen;
                     [ValueObject<int>] public {{vogenTypeDeclaration}} MyVO;
                     public sealed class C
                     {
                         public MyVO {|{{RuleIdentifiers.DoNotUseUninitializedMembers}}:P|} { get; init; }
                     }
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }

    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Reports_When_Any_Constructor_Does_Not_Assign(string vogenTypeDeclaration)
    {
        var code = $$"""
                     using Vogen;
                     [ValueObject<int>] public {{vogenTypeDeclaration}} MyVO;
                     public sealed class C
                     {
                         public MyVO {|{{RuleIdentifiers.DoNotUseUninitializedMembers}}:P|} { get; set; }
                         public C(MyVO p) { P = p; }
                         public C(int x) { }
                     }
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }

    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Reports_Static_Property_Without_StaticCtor(string vogenTypeDeclaration)
    {
        var code = $$"""
                     using Vogen;
                     [ValueObject<int>] public {{vogenTypeDeclaration}} MyVO;
                     public static class Holder
                     {
                         public static MyVO {|{{RuleIdentifiers.DoNotUseUninitializedMembers}}:Current|} { get; set; }
                     }
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }

    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Does_Not_Report_When_Required(string vogenTypeDeclaration)
    {
        var code = $$"""
                     using Vogen;
                     [ValueObject<int>] public {{vogenTypeDeclaration}} MyVO;
                     public sealed class C
                     {
                         public required MyVO P { get; set; }
                     }
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }

    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Does_Not_Report_When_Annotated_With_MayBeUninitialized(string vogenTypeDeclaration)
    {
        var code = $$"""
                     using Vogen;
                     [ValueObject<int>] public {{vogenTypeDeclaration}} MyVO;
                     public sealed class C
                     {
                         [MayBeUninitialized]
                         public MyVO P { get; set; }
                     }
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }

    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Does_Not_Report_When_Interface_Is_Annotated_With_MayBeUninitialized(string vogenTypeDeclaration)
    {
        var code = $$"""
                     using Vogen;
                     public interface IC
                     {
                       [MayBeUninitialized]
                        MyVO P { get; set; }
                     }
                     
                     [ValueObject<int>] public {{vogenTypeDeclaration}} MyVO;
                     public sealed class C : IC
                     {
                         public MyVO P { get; set; }
                     }
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }


    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Does_Not_Report_When_Nullable(string vogenTypeDeclaration)
    {
        var code = $$"""
                     using Vogen;
                     [ValueObject<int>] public {{vogenTypeDeclaration}} MyVO;
                     public sealed class C
                     {
                         public MyVO? P { get; set; }
                     }
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }

    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Does_Not_Report_When_Inline_Initialized(string vogenTypeDeclaration)
    {
        var code = $$"""
                     using Vogen;
                     [ValueObject<int>] public {{vogenTypeDeclaration}} MyVO
                     {
                         public static MyVO From(int v) => default;
                     }
                     public sealed class C
                     {
                         public MyVO P { get; set; } = MyVO.From(0);
                     }
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }

    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Does_Not_Report_When_Inline_Initialized_Field(string vogenTypeDeclaration)
    {
        var code = $$"""
                     using Vogen;
                     [ValueObject<int>] public {{vogenTypeDeclaration}} MyVO
                     {
                         public static MyVO From(int v) => default;
                     }
                     public sealed class C
                     {
                         public readonly MyVO _field = MyVO.From(0);
                     }
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }


    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Does_Not_Report_When_Assigned_In_Every_Constructor(string vogenTypeDeclaration)
    {
        var code = $$"""
                     using Vogen;
                     [ValueObject<int>] public {{vogenTypeDeclaration}} MyVO;
                     public sealed class C
                     {
                         public MyVO P { get; set; }
                         public C(MyVO p) { P = p; }
                         public C(int x) : this(default(MyVO)) { }
                     }
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }

    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Does_Not_Report_When_Assigned_In_Every_Static_Constructor(string vogenTypeDeclaration)
    {
        var code = $$"""
                     using Vogen;
                     [ValueObject<int>] public {{vogenTypeDeclaration}} MyVO;
                     public sealed class C
                     {
                         public static MyVO P { get; set; }
                         static C()
                         {
                             P = default(MyVO);
                         }
                     }
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }


    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Does_Not_Report_When_Constructor_Is_Annotated_With_SetsUninitializedMembers(string vogenTypeDeclaration)
    {
        var code = $$"""
                     using Vogen;
                     using System.Diagnostics.CodeAnalysis;
                     [ValueObject<int>] public {{vogenTypeDeclaration}} MyVO;
                     public sealed class C
                     {
                         public MyVO P { get; set; }
                         [SetsUninitializedMembers]
                         public C(int x){}
                     }
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }

    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Does_Not_Report_For_Type_Without_ValueObject_Attribute(string vogenTypeDeclaration)
    {
        var code = $$"""
                     public {{vogenTypeDeclaration}} PlainStruct;
                     public sealed class C
                     {
                         public PlainStruct P { get; set; }
                     }
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }

    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Does_Not_Report_For_Positional_Record_Property(string vogenTypeDeclaration)
    {
        var code = $$"""
                     using Vogen;
                     [ValueObject<int>] public {{vogenTypeDeclaration}} MyVO;
                     public record R(MyVO P);
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }

    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Does_Not_Report_For_Computed_Property(string vogenTypeDeclaration)
    {
        var code = $$"""
                     using Vogen;
                     [ValueObject<int>] public {{vogenTypeDeclaration}} MyVO
                     {
                         public static MyVO From(int v) => default;
                     }
                     public sealed class C
                     {
                         public MyVO P => MyVO.From(0);
                     }
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }

    [Theory]
    [MemberData(nameof(AllVogenDeclarations))]
    public async Task Does_Not_Report_For_Property_With_Assigned_Backing_Field(string vogenTypeDeclaration)
    {
        var code = $$"""
                     using Vogen;
                     [ValueObject<int>] public {{vogenTypeDeclaration}} MyVO
                     {
                         public static MyVO From(int v) => default;
                     }
                     public sealed class C
                     {
                         private MyVO _backingField;

                         public MyVO Property
                         {
                             get => _backingField;
                             set => _backingField = value;
                         }
                         
                         public C()
                         {
                             _backingField = MyVO.From(0);
                         }

                     }
                     """;
        await CreateAnalyzerTest(code).RunAsync();
    }
}