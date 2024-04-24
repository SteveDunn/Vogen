namespace Vogen.IntegrationTests.TestTypes;

public static class Primitives
{
    public static readonly DateTime DateTime1 = new DateTime(1970, 6, 10, 14, 01, 02, DateTimeKind.Utc) + TimeSpan.FromTicks(12345678);
    public static readonly DateTime DateTime2 = DateTime.Now.AddMinutes(42.69);

    public static readonly DateTimeOffset DateTimeOffset1 = new DateTimeOffset(1970, 6, 10, 14, 01, 02, TimeSpan.Zero) +
                                                   TimeSpan.FromTicks(12345678);
    public static readonly DateTimeOffset DateOffset2 = DateTimeOffset.Now.AddMinutes(42.69);
    public static readonly TimeOnly Time1 = new(13, 12, 59, 123);
    public static readonly TimeOnly Time2 = new(1, 59, 58, 123);

    public static readonly Guid Guid1 = Guid.NewGuid();
    public static readonly Guid Guid2 = Guid.NewGuid();
}