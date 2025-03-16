//using Testbench.SubNamespace;

// ReSharper disable UnusedVariable

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Vogen;
using iformattable_infinite_loop;
using static Vogen.StaticAbstractsGeneration;
//
// [assembly: VogenDefaults(
//     toPrimitiveCasting: CastOperator.Implicit,
//     staticAbstractsGeneration: ValueObjectsDeriveFromTheInterface |
//                                EqualsOperators |
//                                ImplicitCastFromPrimitive |
//                                ImplicitCastToPrimitive |
//                                FactoryMethods)]

var directNonEmpty = TestId.From("foobar");
var directEmpty = TestId.Empty;
var nonEmptyFromJson = JsonSerializer.Deserialize<TestContainer>("{ \"testId\": \"barfoo\" }");
var emptyFromJson = JsonSerializer.Deserialize<TestContainer>("{ \"testId\": \"\" }");

Console.ReadLine();

public record TestContainer(
    [property: JsonPropertyName("testId")]
    TestId TestId);

[ValueObject<string>]
[Instance("Empty", "")]
public partial record TestId
{
    private static Validation Validate(string value) => !string.IsNullOrWhiteSpace(value)
        ? Validation.Ok
        : Validation.Invalid("Test ID must be a non-blank, non-empty string.");

    private static string NormalizeInput(string value) => value.Trim();
}

// InfiniteLoopRunner.Run();




[ValueObject<double>(conversions: Conversions.DapperTypeHandler)]
public partial class DapperDoubleVo;



[ValueObject<int>]
public readonly partial record struct ToDoItemId;

