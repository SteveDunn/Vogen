using System.ComponentModel;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Vogen.Benchmarks.WithValidation
{
    [MemoryDiagnoser, Description("The underlying type is int and the VOs validation that they're > 0")]
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net80)]
    public class Underlying_Int_With_Validation
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private volatile int _n1;
        private volatile int _n2;
        private volatile NumberAsClass _c1;
        private volatile NumberAsClass _c2;
        private NumberAsStruct _s1;
        private NumberAsStruct _s2;
        private NumberAsRecordStruct _rs1;
        private NumberAsRecordStruct _rs2;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _n1 = TestData.RandomNumberBetween(1, 10_000);
            _n2 = TestData.RandomNumberBetween(1, 10_000);
            
            _s1 = NumberAsStruct.From(_n1);
            _s2 = NumberAsStruct.From(_n2);
            
            _rs1 = new NumberAsRecordStruct(_n1);
            _rs2 = new NumberAsRecordStruct(_n2);
            
            _c1 = NumberAsClass.From(_n1);
            _c2 = NumberAsClass.From(_n2);
        }

        [Benchmark(Baseline = true)]
        public int UsingIntNatively() => Calculator.Add(_n1, _n2);

        [Benchmark]
        public NumberAsRecordStruct UsingRecordStruct() => Calculator.Add(_rs1, _rs2);

        [Benchmark]
        public NumberAsStruct UsingValueObjectStruct() => Calculator.Add(_s1, _s2);

        [Benchmark]
        public NumberAsClass UsingValueObjectAsClass() => Calculator.Add(_c1, _c2);
    }

    [MemoryDiagnoser, Description("The underlying type is string and the VOs validation that they're not null or empty")]
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net80)]
    public class Underlying_string_With_Validation
    {
        private volatile string _s1;
        private volatile string _s2;
        private volatile NameAsClass _wrapperClass1;
        private volatile NameAsClass _wrapperClass2;
        private NameAsStruct _wrapperStruct1;
        private NameAsStruct _wrapperStruct2;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _s1 = TestData.GetRandomString();
            _s2 = TestData.GetRandomString();

            _wrapperStruct1 = NameAsStruct.From(_s1);
            _wrapperStruct2 = NameAsStruct.From(_s2);

            _wrapperClass1 = NameAsClass.From(_s1);
            _wrapperClass2 = NameAsClass.From(_s2);
        }

        private static string Combine(string s1, string s2) => $"{s1}-{s2}";
        
        public static NameAsClass Combine(NameAsClass s1, NameAsClass s2) => NameAsClass.From($"{s1}-{s2}");
        
        public static NameAsStruct Combine(NameAsStruct s1, NameAsStruct s2) => NameAsStruct.From($"{s1}-{s2}");
        
        [Benchmark(Baseline = true)]
        public string UsingStringNatively()
        {
            var random1 = _s1;
            if (string.IsNullOrEmpty(random1)) throw new ValueObjectValidationException("Must not be null or empty");

            var random2 = _s2;
            if (string.IsNullOrEmpty(random2)) throw new ValueObjectValidationException("Must not be null or empty");

            return Combine(random1, random2);
        }

        [Benchmark]
        public NameAsClass UsingValueObjectAsClass() => Combine(_wrapperClass1, _wrapperClass2);

        [Benchmark]
        public NameAsStruct UsingValueObjectAsStruct() => Combine(_wrapperStruct1, _wrapperStruct2);
    }

    public static class Calculator
    {
        public static int Add(int n1, int n2)
        {
            if (n1 <= 0) throw new ValueObjectValidationException("Must be greater than 0");
            if (n2 <= 0) throw new ValueObjectValidationException("Must be greater than 0");
            int n = 0;
            for (int i = 0; i < 1000; i++) n += n1 + n2;
            return n;
        }

        public static int AddNoValidate(int n1, int n2)
        {
            int n = 0;
            for (int i = 0; i < 1000; i++) n += n1 + n2;
            return n;
        }

        public static NumberAsStruct Add(NumberAsStruct n1, NumberAsStruct n2) => NumberAsStruct.From(AddNoValidate(n1.Value, n2.Value));

        public static NumberAsClass Add(NumberAsClass n1, NumberAsClass n2) => NumberAsClass.From(AddNoValidate(n1.Value, n2.Value));

        public static NumberAsRecordStruct Add(NumberAsRecordStruct n1, NumberAsRecordStruct n2) => new(AddNoValidate(n1.Value, n2.Value));
    }

    public record struct NumberAsRecordStruct
    {
        public NumberAsRecordStruct(int value)
        {
            if (value <= 0) throw new ValueObjectValidationException("Must be greater than zero");
            Value = value;
        }

        public int Value { get; }
    }

    [ValueObject<int>]
    public partial struct NumberAsStruct
    {
        private static Validation Validate(int value) =>
            value > 0 ? Validation.Ok : Validation.Invalid("Must be greater than zero");
    }

    [ValueObject<int>]
    public partial class NumberAsClass
    {
        private static Validation Validate(int value) =>
            value > 0 ? Validation.Ok : Validation.Invalid("Must be greater than zero");
    }

    [ValueObject<string>]
    public partial class NameAsClass
    {
        private static Validation Validate(string value) =>
            string.IsNullOrEmpty(value) ? Validation.Invalid("Must not be null or empty") : Validation.Ok;
    }

    [ValueObject<string>]
    public partial struct NameAsStruct
    {
        private static Validation Validate(string value) =>
            string.IsNullOrEmpty(value) ? Validation.Invalid("Must not be null or empty") : Validation.Ok;
    }

}
