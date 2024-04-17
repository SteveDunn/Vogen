namespace ConsumerTests.ParseAndTryParseTests;

[ValueObject(typeof(int))]
public partial struct StructIntVoNoValidation
{
}

[ValueObject(typeof(int))]
public partial struct VoWithOwnParseMethod
{
    public VoWithOwnParseMethod Parse(string s) => VoWithOwnParseMethod.From(int.Parse(s));
}

[ValueObject(typeof(int))]
public partial class ClassIntVoNoValidation
{
}

[ValueObject(typeof(int))]
public partial class RecordClassIntVoNoValidation
{
}

[ValueObject(typeof(int))]
public partial class RecordStructIntVoNoValidation
{
}

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
public partial struct ByteVo { }

[ValueObject(typeof(bool))]
public partial struct StructBoolVo { }

[ValueObject(typeof(bool))]
public partial struct RecordStructBoolVo { }

[ValueObject(typeof(bool))]
public partial class ClassBoolVo { }

[ValueObject(typeof(bool))]
public partial record class RecordClassBoolVo { }

[ValueObject(typeof(char))]
public partial struct CharVo { }

[ValueObject(typeof(decimal))]
public partial struct DecimalVo { }

[ValueObject(typeof(double))]
public partial struct DoubleVo { }