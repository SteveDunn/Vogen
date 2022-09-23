using System;
using System.Globalization;
using FluentAssertions;
using Vogen;
using Xunit;

namespace SmallTests.TryParseTests;

[ValueObject(typeof(int))]
public partial struct IntVoNoValidation
{
}

[ValueObject(typeof(int))]
public partial struct IntVo
{
    private static Validation Validate(int input) =>
        input < 100 ? Validation.Ok : Validation.Invalid("must be less than 100");
}

[ValueObject(typeof(byte))]
public partial struct ByteVo { }

[ValueObject(typeof(char))]
public partial struct CharVo { }

[ValueObject(typeof(decimal))]
public partial struct DecimalVo { }

[ValueObject(typeof(double))]
public partial struct DoubleVo { }