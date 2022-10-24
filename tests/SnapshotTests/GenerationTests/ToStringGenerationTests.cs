using System.Collections;
using Shared;
using Vogen;

namespace MediumTests.SnapshotTests.GenerationTests;

[UsesVerify]
public class ToStringGenerationTests
{
    private class Types : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (string type in _types)
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

        private static string CreateClassName(string type, ToStringMethod toStringMethod) => type.Replace(" ", "_") + "_" + toStringMethod;

        private readonly string[] _types =
        {
            "struct",
            "class",
            "record struct",
            "record class",
            "record",
        };

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(Types))]
    public Task GenerationTest(string type, string className, ToStringMethod addToStringMethod)
    {
        string declaration = $@"
  [ValueObject]
  public partial {type} {className} {{ {WriteToStringMethod(addToStringMethod, type.Contains("record"))} }}";
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