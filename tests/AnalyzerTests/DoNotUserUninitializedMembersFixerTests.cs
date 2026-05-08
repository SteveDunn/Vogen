using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Vogen.Diagnostics;
using Vogen.Rules;
using Vogen.Rules.UninitializedMemberFixers;

namespace AnalyzerTests;

public sealed class DoNotUserUninitializedMembersFixerTests
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


        [Fact]
    public async Task CodeFix_Adds_Required_Modifier()
    {
        const string code = $$"""
                              using Vogen;
                              [ValueObject<int>] public readonly partial struct MyVO;
                              public sealed class C
                              {
                                  public MyVO {|{{RuleIdentifiers.DoNotUseUninitializedMembers}}:P|} { get; set; }
                              }
                              """;
        const string fixedCode = """
                                 using Vogen;
                                 [ValueObject<int>] public readonly partial struct MyVO;
                                 public sealed class C
                                 {
                                     public required MyVO P { get; set; }
                                 }
                                 """;
        await CreateCodeFixTest(code, fixedCode,  UninitializedMemberFixersFixers.MakeRequiredEquivalenceKey)
            .RunAsync();
    }

    [Fact]
    public async Task CodeFix_Makes_Type_Nullable()
    {
        const string code = $$"""
                              using Vogen;
                              [ValueObject<int>] public readonly partial struct MyVO;
                              public sealed class C
                              {
                                  public MyVO {|{{RuleIdentifiers.DoNotUseUninitializedMembers}}:P|} { get; set; }
                              }
                              """;
        const string fixedCode = """
                                 using Vogen;
                                 [ValueObject<int>] public readonly partial struct MyVO;
                                 public sealed class C
                                 {
                                     public MyVO? P { get; set; }
                                 }
                                 """;
        await CreateCodeFixTest(code, fixedCode, UninitializedMemberFixersFixers.MakeNullableEquivalenceKey)
            .RunAsync();
    }


    private static CSharpCodeFixTest<DoNotUseUninitializedMembersAnalyzer, UninitializedMemberFixersFixers, DefaultVerifier> CreateCodeFixTest(
        string testCode, string fixedCode, string equivalenceKey) =>
        new()
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net100,
            CodeActionEquivalenceKey = equivalenceKey,
            TestState =
            {
                Sources = { VogenStub, testCode }
            },
            FixedState =
            {
                Sources = { VogenStub, fixedCode }
            }
        };
}