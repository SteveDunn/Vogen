namespace ConsumerTests.StringComparisons;

[ValueObject(typeof(string))]
public partial class StringVo_Class_NothingSpecified
{
}

[ValueObject(typeof(string))]
public partial record class StringVo_RecordClass_NothingSpecified
{
}

[ValueObject(typeof(string))]
public partial record struct StringVo_RecordStruct_NothingSpecified
{
}

[ValueObject(typeof(string), stringComparers: StringComparersGeneration.Omit)]
public partial struct StringVo_Struct_NothingSpecified
{
}

[ValueObject(typeof(string), stringComparers: StringComparersGeneration.Omit)]
public readonly partial struct StringVo_ReadOnly_Struct_NothingSpecified
{
}

[ValueObject(typeof(string), stringComparers: StringComparersGeneration.Generate)]
public readonly partial struct StringVo_ReadOnly_Struct
{
}


[ValueObject(typeof(string), stringComparers: StringComparersGeneration.Generate)]
public partial class StringVo_Class
{
}

[ValueObject(typeof(string), stringComparers: StringComparersGeneration.Generate)]
public partial class StringVo_RecordClass
{
}

[ValueObject(typeof(string), stringComparers: StringComparersGeneration.Generate)]
public partial record struct StringVo_RecordStruct
{
}

[ValueObject(typeof(string), stringComparers: StringComparersGeneration.Generate)]
public readonly partial record struct StringVo_ReadOnly_RecordStruct
{
}

[ValueObject(typeof(string), stringComparers: StringComparersGeneration.Omit)]
public readonly partial record struct StringVo_ReadOnly_RecordStruct_NothingSpecified
{
}

[ValueObject(typeof(string), stringComparers: StringComparersGeneration.Generate)]
public partial struct StringVo_Struct
{
}

[ValueObject(typeof(string))]
public partial struct Vo1
{
}

[ValueObject(typeof(string))]
public partial struct Vo2
{
}

[ValueObject<string>(stringComparers: StringComparersGeneration.Generate)]
public partial class StringVo_Class_Generic
{
}

[ValueObject<string>(stringComparers: StringComparersGeneration.Generate)]
public partial record class StringVo_RecordClass_Generic
{
}

[ValueObject<string>(stringComparers: StringComparersGeneration.Generate)]
public partial record struct StringVo_RecordStruct_Generic
{
}

[ValueObject<string>(stringComparers: StringComparersGeneration.Generate)]
public partial struct StringVo_Struct_Generic
{
}

[ValueObject(typeof(string), Conversions.Default, null, Customizations.None, DeserializationStrictness.AllowValidAndKnownInstances, DebuggerAttributeGeneration.Default, ComparisonGeneration.Default, StringComparersGeneration.Unspecified, CastOperator.Unspecified, CastOperator.Unspecified, ParsableForStrings.Unspecified, ParsableForPrimitives.Unspecified, TryFromGeneration.Unspecified, IsInitializedMethodGeneration.Unspecified, PrimitiveEqualityGeneration.Unspecified, NumericsGeneration.Unspecified, StringComparisonDefault.OrdinalIgnoreCase)]
public partial class StringVo_Class_DefaultOrdinalIgnoreCase
{
}

[ValueObject(typeof(string), Conversions.Default, null, Customizations.None, DeserializationStrictness.AllowValidAndKnownInstances, DebuggerAttributeGeneration.Default, ComparisonGeneration.Default, StringComparersGeneration.Unspecified, CastOperator.Unspecified, CastOperator.Unspecified, ParsableForStrings.Unspecified, ParsableForPrimitives.Unspecified, TryFromGeneration.Unspecified, IsInitializedMethodGeneration.Unspecified, PrimitiveEqualityGeneration.Unspecified, NumericsGeneration.Unspecified, StringComparisonDefault.OrdinalIgnoreCase)]
public partial record class StringVo_RecordClass_DefaultOrdinalIgnoreCase
{
}

[ValueObject(typeof(string), Conversions.Default, null, Customizations.None, DeserializationStrictness.AllowValidAndKnownInstances, DebuggerAttributeGeneration.Default, ComparisonGeneration.Default, StringComparersGeneration.Unspecified, CastOperator.Unspecified, CastOperator.Unspecified, ParsableForStrings.Unspecified, ParsableForPrimitives.Unspecified, TryFromGeneration.Unspecified, IsInitializedMethodGeneration.Unspecified, PrimitiveEqualityGeneration.Unspecified, NumericsGeneration.Unspecified, StringComparisonDefault.OrdinalIgnoreCase)]
public partial record struct StringVo_RecordStruct_DefaultOrdinalIgnoreCase
{
}

[ValueObject(typeof(string), Conversions.Default, null, Customizations.None, DeserializationStrictness.AllowValidAndKnownInstances, DebuggerAttributeGeneration.Default, ComparisonGeneration.Default, StringComparersGeneration.Unspecified, CastOperator.Unspecified, CastOperator.Unspecified, ParsableForStrings.Unspecified, ParsableForPrimitives.Unspecified, TryFromGeneration.Unspecified, IsInitializedMethodGeneration.Unspecified, PrimitiveEqualityGeneration.Unspecified, NumericsGeneration.Unspecified, StringComparisonDefault.OrdinalIgnoreCase)]
public partial struct StringVo_Struct_DefaultOrdinalIgnoreCase
{
}
