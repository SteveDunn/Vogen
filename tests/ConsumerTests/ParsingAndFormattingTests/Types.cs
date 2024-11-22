using System.Linq;
// ReSharper disable RedundantRecordClassKeyword
// ReSharper disable ArrangeStaticMemberQualifier

namespace ConsumerTests.ParsingAndFormattingTests;

[ValueObject<decimal>]
public partial struct MyDecimal;


[ValueObject(typeof(int), parsableForPrimitives: ParsableForPrimitives.GenerateNothing)]
public partial struct VoNoParsableNoHoisting;

[ValueObject(typeof(int))]
public partial struct StructIntVoNoValidation;

[ValueObject(typeof(string), parsableForStrings: ParsableForStrings.GenerateMethodsAndInterface  )]
public partial struct VoWithOwnInstanceParseMethod
{
    public VoWithOwnInstanceParseMethod Parse(string s) => VoWithOwnInstanceParseMethod.From(new string(s.Reverse().ToArray()));
}

[ValueObject(typeof(string), parsableForStrings: ParsableForStrings.GenerateMethodsAndInterface  )]
public partial struct VoStringWithOwnStaticParseMethod
{
    public static VoStringWithOwnStaticParseMethod Parse(string s) => VoStringWithOwnStaticParseMethod.From(new string(s.Reverse().ToArray()));
}

[ValueObject(typeof(string), parsableForStrings: ParsableForStrings.GenerateMethodsAndInterface  )]
public partial struct VoStringWithOwnStaticParseMethodWithFormatProvider
{
    public static VoStringWithOwnStaticParseMethodWithFormatProvider Parse(string s, IFormatProvider? p) => VoStringWithOwnStaticParseMethodWithFormatProvider.From(new string(s.Reverse().ToArray()));
}

[ValueObject(typeof(string), parsableForStrings: ParsableForStrings.GenerateMethodsAndInterface  )]
public partial struct VoStringWithOwnStaticParseMethodButReturningDifferentType
{
    public static string Parse(string s, IFormatProvider _) => new(s.Reverse().ToArray());
}

[ValueObject(typeof(int), parsableForPrimitives: ParsableForPrimitives.HoistMethodsAndInterfaces  )]
public partial struct VoIntWithOwnStaticParseMethod
{
    public static VoIntWithOwnStaticParseMethod Parse(string s) => VoIntWithOwnStaticParseMethod.From(int.Parse(new string(s.Reverse().ToArray())));
}

[ValueObject(typeof(int), parsableForPrimitives: ParsableForPrimitives.HoistMethodsAndInterfaces  )]
public partial struct VoIntWithOwnStaticParseMethodWithFormatProvider
{
    public static VoIntWithOwnStaticParseMethodWithFormatProvider Parse(string s, IFormatProvider? p) =>
        VoIntWithOwnStaticParseMethodWithFormatProvider.From(int.Parse(new string(s.Reverse().ToArray())));
}

[ValueObject(typeof(int))]
public partial class ClassIntVoNoValidation;

[ValueObject(typeof(int))]
public partial class RecordClassIntVoNoValidation;

[ValueObject(typeof(int))]
public partial class RecordStructIntVoNoValidation;

[ValueObject<int>]
public partial struct StructIntVo
{
    private static Validation Validate(int input) =>
        input < 100 ? Validation.Ok : Validation.Invalid("must be less than 100");
}

[ValueObject<int>]
public partial record struct RecordStructIntVo
{
    private static Validation Validate(int input) =>
        input < 100 ? Validation.Ok : Validation.Invalid("must be less than 100");
}

[ValueObject<int>]
public partial struct StructIntVoWithNormalization
{
    private static int NormalizeInput(int n) => n / 2;
    
    private static Validation Validate(int input) =>
        input < 100 ? Validation.Ok : Validation.Invalid("must be less than 100");
}

[ValueObject<int>]
public partial class ClassIntVo
{
    private static Validation Validate(int input) =>
        input < 100 ? Validation.Ok : Validation.Invalid("must be less than 100");
}

[ValueObject<int>]
public partial record class RecordClassIntVo
{
    private static Validation Validate(int input) =>
        input < 100 ? Validation.Ok : Validation.Invalid("must be less than 100");
}

[ValueObject(typeof(byte))]
public partial struct ByteVo;

[ValueObject(typeof(bool))]
public partial struct StructBoolVo;

[ValueObject(typeof(bool))]
public partial struct RecordStructBoolVo;

[ValueObject(typeof(bool))]
public partial class ClassBoolVo;

[ValueObject(typeof(bool))]
public partial record class RecordClassBoolVo;

[ValueObject(typeof(char))]
public partial struct CharVo;

[ValueObject(typeof(decimal))]
public partial struct DecimalVo;

[ValueObject(typeof(double))]
public partial struct DoubleVo;

public class C : IParsable<C>
{
    public static C Parse(string s, IFormatProvider? provider) => throw new NotImplementedException();

    public static bool TryParse(string? s, IFormatProvider? provider, out C result) => throw new NotImplementedException();
}

[ValueObject<C>]
public partial struct MyCustomVo;
