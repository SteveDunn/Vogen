using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyXunit;
using Vogen;
using Xunit;

namespace LargeTests.ToString;

[UsesVerify]
public class ToStringGenerationTests
{
    private class Types : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (string type in Factory.TypeVariations)
            {
                yield return new object[]
                {
                    type,
                    CreateClassName(type, ToStringMethod.None),
                    ToStringMethod.None,
                };

                yield return new object[]
                {
                    type,
                    CreateClassName(type, ToStringMethod.Method),
                    ToStringMethod.Method,
                };

                yield return new object[]
                {
                    type,
                    CreateClassName(type, ToStringMethod.ExpressionBodiedMethod),
                    ToStringMethod.ExpressionBodiedMethod,
                };
            }
        }

        private static string CreateClassName(string type, ToStringMethod toStringMethod) =>
            type.Replace(" ", "_") + "_" + toStringMethod;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(Types))]
    public Task Test(string type, string className, ToStringMethod addToStringMethod)
    {
        string declaration = $@"
  [ValueObject]
  public {type} {className} {{ {WriteToStringMethod(addToStringMethod, type.Contains("record"))} }}";
        var source = @"using Vogen;
namespace Whatever
{
" + declaration + @"
}";

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseFileName(className))
            .RunOnAllFrameworks();
    }

    private static string WriteToStringMethod(ToStringMethod toStringMethod, bool isARecord)
    {
        string s = isARecord ? "public override sealed string ToString()" : "public override string ToString()";
        return toStringMethod switch
        {
            ToStringMethod.None => string.Empty,
            ToStringMethod.Method => $"{s} {{return \"!\"; }}",
            ToStringMethod.ExpressionBodiedMethod => $"{s} => \"!\"",
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