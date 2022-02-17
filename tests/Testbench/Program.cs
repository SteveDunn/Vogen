using System;
using Vogen;

[assembly: VogenDefaults(underlyingType: typeof(int), conversions: Conversions.None, throws: typeof(Whatever.BadException1))]

namespace Whatever;

[ValueObject(throws: typeof(Whatever.BadException2))]
public partial struct CustomerId
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("xxxx");
}

public class BadException1 { }
public class BadException2 { }
public class NotMentionedException { }