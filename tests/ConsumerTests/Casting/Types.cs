namespace ConsumerTests.CastOperators;

[ValueObject(typeof(string))]
public partial class Class_default
{
}

[ValueObject(typeof(string), toPrimitiveCasting: CastOperator.Implicit, fromPrimitiveCasting: CastOperator.None)]
public partial class Class_implicit_to_primitive_nothing_from_primitive
{
}

[ValueObject(typeof(string), toPrimitiveCasting: CastOperator.Implicit, fromPrimitiveCasting: CastOperator.Implicit)]
public partial class Class_implicit_both_ways
{
}

#if NET7_0_OR_GREATER

[ValueObject<string>(toPrimitiveCasting: CastOperator.None, fromPrimitiveCasting: CastOperator.None)]
public partial class Class_default_generic
{
}
#endif

[ValueObject(typeof(string))]
public partial class Struct_default
{
}

[ValueObject(typeof(string), toPrimitiveCasting: CastOperator.Implicit, fromPrimitiveCasting: CastOperator.None)]
public partial class Struct_implicit_to_primitive_nothing_from_primitive
{
}

[ValueObject(typeof(string), toPrimitiveCasting: CastOperator.Implicit, fromPrimitiveCasting: CastOperator.Implicit)]
public partial class Struct_implicit_both_ways
{
}

#if NET7_0_OR_GREATER

[ValueObject<string>(toPrimitiveCasting: CastOperator.None, fromPrimitiveCasting: CastOperator.None)]
public partial class Struct_default_generic
{
}
#endif
