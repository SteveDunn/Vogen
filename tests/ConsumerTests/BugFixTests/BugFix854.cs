namespace ConsumerTests.BugFixTests.BugFix854;

// https://github.com/SteveDunn/Vogen/issues/854

[ValueObject<Guid>]
public readonly partial struct UserId
{
    public static readonly UserId Empty = new(Guid.Empty);
    public static readonly UserId System = new(Guid.Empty);
    public static UserId New() => From(Guid.NewGuid());
    
    // no assertions needed - the fact that this compiles is enough.
}