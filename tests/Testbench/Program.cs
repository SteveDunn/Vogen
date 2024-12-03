//using Testbench.SubNamespace;

// ReSharper disable UnusedVariable

using Vogen;
using iformattable_infinite_loop;
using static Vogen.StaticAbstractsGeneration;

[assembly: VogenDefaults(
    toPrimitiveCasting: CastOperator.Implicit,
    staticAbstractsGeneration: ValueObjectsDeriveFromTheInterface |
                               EqualsOperators |
                               ImplicitCastFromPrimitive |
                               ImplicitCastToPrimitive |
                               FactoryMethods)]


InfiniteLoopRunner.Run();


[ValueObject<int>]
public readonly partial record struct ToDoItemId;

