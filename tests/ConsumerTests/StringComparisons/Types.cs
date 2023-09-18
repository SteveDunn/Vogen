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

[ValueObject(typeof(string), stringComparison: StringComparisonGeneration.OrdinalIgnoreCase)]
public partial class VoOrdinalIgnoreCase
{
}

