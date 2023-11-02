namespace ConsumerTests.BugFixTests.BugFix516
{
    [ValueObject<string>(conversions: Conversions.Default | Conversions.NewtonsoftJson)]
    public readonly partial record struct T1 {
        public readonly bool Equals(T1 other) => Value == other.Value;

        public override int GetHashCode() => 0;
    }

    [ValueObject<string>]
    internal sealed partial record class T2 {
        public bool Equals(T2? other) => Value == other?.Value;

        public override int GetHashCode() => 0;
    }

    [ValueObject<string>]
    internal sealed partial class T3 {
        public bool Equals(T3? other) => Value == other?.Value;

        public override int GetHashCode() => 0;
    }
}