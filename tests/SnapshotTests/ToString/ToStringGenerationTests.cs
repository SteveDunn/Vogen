using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vogen;

namespace SnapshotTests.ToString;

public class ToStringGenerationTests
{
    [Fact]
    public Task Uses_user_provided_ToString()
    {
        var source = $$"""
                       using System;
                       using Vogen;
                       namespace Whatever;

                       
                       [ValueObject<string>]
                       public partial record class Name
                       {
                           public sealed override string ToString() => "!!";
                       }
                       """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Hoisted_ToString_from_record_is_readonly()
    {
        var source = $$"""
                       using System;
                       using Vogen;
                       namespace Whatever;

                       [ValueObject<string>(conversions: Conversions.None)]
                       public readonly partial record struct Name;
                       """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Hoists_methods_onto_the_struct_wrapper()
    {
        var source = $$"""
                       using System;
                       using Vogen;
                       namespace Whatever;

                       [ValueObject<DateOnly>(conversions: Conversions.None)]
                       public partial struct CreationDate;
                       """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnNet8AndGreater();
    }

    [Fact]
    public Task Generates_default_ToString_if_none_found()
    {
        var source = $$"""
                       using System;
                       using Vogen;
                       namespace Whatever;

                       public class Hash;
                       
                       [ValueObject<Hash>]
                       public readonly partial struct FileHash; 
                       
                       """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Hoists_methods_onto_the_struct_wrapper_and_skips_user_supplied_ones()
    {
        var source = $$"""
                       using System;
                       using Vogen;
                       namespace Whatever;

                       [ValueObject<DateOnly>(conversions: Conversions.None)]
                       public partial struct CreationDate
                       {
                            public string ToString(string format) => "!!";
                       }
                       """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnNet8AndGreater();
    }

    [Theory]
    [ClassData(typeof(Types))]
    public Task Test(string type, string className, ToStringMethod addToStringMethod)
    {
        string declaration = $$"""
                               
                                 [ValueObject]
                                 public {{type}} {{className}} { {{WriteToStringMethod(addToStringMethod, type.EndsWith("record class") || type.EndsWith("record"))}} }
                               """;
        var source = $$"""
                       using Vogen;
                       namespace Whatever
                       {
                       {{declaration}}
                       }
                       """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseFileName(className))
            .RunOnAllFrameworks();
    }

    private class Types : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (string type in Factory.TypeVariations)
            {
                yield return
                [
                    type,
                    CreateClassName(type, ToStringMethod.None),
                    ToStringMethod.None
                ];

                yield return
                [
                    type,
                    CreateClassName(type, ToStringMethod.Method),
                    ToStringMethod.Method
                ];

                yield return
                [
                    type,
                    CreateClassName(type, ToStringMethod.ExpressionBodiedMethod),
                    ToStringMethod.ExpressionBodiedMethod
                ];
            }
        }

        private static string CreateClassName(string type, ToStringMethod toStringMethod) =>
            type.Replace(" ", "_") + "_" + toStringMethod;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private static string WriteToStringMethod(ToStringMethod toStringMethod, bool isARecordClass)
    {
        string s = isARecordClass ? "public override sealed string ToString()" : "public override string ToString()";
        return toStringMethod switch
        {
            ToStringMethod.None => string.Empty,
            ToStringMethod.Method => $$"""{{s}} {return "!"; }""",
            ToStringMethod.ExpressionBodiedMethod => $"""{s} => "!";""",
            _ => throw new InvalidOperationException($"Don't know what a {toStringMethod} is!")
        };
    }
}

public enum ToStringMethod
{
    None,
    Method,
    ExpressionBodiedMethod
}