namespace ConsumerTests.StringComparisons;

[ValueObject(typeof(string))]
public partial class VoWithNothingSpecified
{
}

[ValueObject(typeof(string), stringComparison: StringComparisonGeneration.CurrentCulture)]
public partial class VoCurrentCulture
{
}

[ValueObject(typeof(string), stringComparison: StringComparisonGeneration.CurrentCultureIgnoreCase)]
public partial class VoCurrentCultureIgnoreCase
{
}

[ValueObject(typeof(string), stringComparison: StringComparisonGeneration.CurrentCultureIgnoreCase)]
public partial record VoRecordCurrentCultureIgnoreCase
{
}

[ValueObject(typeof(string), stringComparison: StringComparisonGeneration.OrdinalIgnoreCase)]
public partial class VoOrdinalIgnoreCase
{
}

[ValueObject(typeof(string), stringComparison: StringComparisonGeneration.OrdinalIgnoreCase)]
public partial struct VoOrdinalIgnoreCase_Struct
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

#if NET7_0_OR_GREATER
[ValueObject<string>(stringComparison: StringComparisonGeneration.OrdinalIgnoreCase)]
public partial class VoOrdinalIgnoreCase_Generic
{
}
#endif

