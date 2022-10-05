using System.ComponentModel;
using BenchmarkDotNet.Attributes;

namespace Vogen.Benchmarks.WithValidation
{
    [MemoryDiagnoser, Description("The underlying type is int and the VOs validation that they're > 0")]
    public class Underlying_Int_With_Validation
    {
        [GlobalSetup]
        public void GlobalSetup()
        {
        }

        [Benchmark(Baseline = true)]
        public int UsingIntNatively()
        {
            int x = TestData.RandomNumberBetween(1, 10_000);
            int y = TestData.RandomNumberBetween(1, 10_000);
            return Calculator.Add(x, y);
        }

        [Benchmark]
        public NumberAsRecordStruct UsingRecordStruct()
        {
            int x = TestData.RandomNumberBetween(1, 10_000);
            int y = TestData.RandomNumberBetween(1, 10_000);
            return Calculator.Add(new NumberAsRecordStruct(x), new NumberAsRecordStruct(y));
        }

        [Benchmark]
        public NumberAsStruct UsingValueObjectStruct()
        {
            int x = TestData.RandomNumberBetween(1, 10_000);
            int y = TestData.RandomNumberBetween(1, 10_000);
            return Calculator.Add(NumberAsStruct.From(x), NumberAsStruct.From(y));
        }

        [Benchmark]
        public NumberAsClass UsingValueObjectAsClass()
        {
            int x = TestData.RandomNumberBetween(1, 10_000);
            int y = TestData.RandomNumberBetween(1, 10_000);
            return Calculator.Add(NumberAsClass.From(x), NumberAsClass.From(y));
        }
    }

    [MemoryDiagnoser, Description("The underlying type is string and the VOs validation that they're not null or empty")]
    public class Underlying_string_With_Validation
    {
        private static string Combine(string s1, string s2) => $"{s1}-{s2}";
        
        public static NameAsClass Combine(NameAsClass s1, NameAsClass s2) => NameAsClass.From($"{s1}-{s2}");
        
        public static NameAsStruct Combine(NameAsStruct s1, NameAsStruct s2) => NameAsStruct.From($"{s1}-{s2}");
        
        [Benchmark(Baseline = true)]
        public string UsingStringNatively()
        {
            var random1 = TestData.GetRandomString();
            if (string.IsNullOrEmpty(random1)) throw new ValueObjectValidationException("Must not be null or empty");

            var random2 = TestData.GetRandomString();
            if (string.IsNullOrEmpty(random2)) throw new ValueObjectValidationException("Must not be null or empty");

            return Combine(random1, random2);
        }

        [Benchmark]
        public NameAsClass UsingValueObjectAsClass()
        {
            return Combine(NameAsClass.From(TestData.GetRandomString()), NameAsClass.From(TestData.GetRandomString()));
        }

        [Benchmark]
        public NameAsStruct UsingValueObjectAsStruct()
        {
            return Combine(NameAsStruct.From(TestData.GetRandomString()), NameAsStruct.From(TestData.GetRandomString()));
        }
    }

    public static class Calculator
    {
        public static int Add(int n1, int n2)
        {
            if (n1 <= 0) throw new ValueObjectValidationException("Must be greater than 0");
            if (n2 <= 0) throw new ValueObjectValidationException("Must be greater than 0");
            return n1 + n2;
        }

        public static NumberAsStruct Add(NumberAsStruct n1, NumberAsStruct n2) => NumberAsStruct.From(n1.Value + n2.Value);

        public static NumberAsClass Add(NumberAsClass n1, NumberAsClass n2) => NumberAsClass.From(n1.Value + n2.Value);

        public static NumberAsRecordStruct Add(NumberAsRecordStruct n1, NumberAsRecordStruct n2) => new(n1.Value + n2.Value);
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

    [ValueObject(typeof(int))]
    public partial struct NumberAsStruct
    {
        private static Validation Validate(int value) =>
            value > 0 ? Validation.Ok : Validation.Invalid("Must be greater than zero");
    }

    [ValueObject(typeof(int))]
    public partial class NumberAsClass
    {
        private static Validation Validate(int value) =>
            value > 0 ? Validation.Ok : Validation.Invalid("Must be greater than zero");
    }

    [ValueObject(typeof(string))]
    public partial class NameAsClass
    {
        private static Validation Validate(string value) =>
            string.IsNullOrEmpty(value) ? Validation.Invalid("Must not be null or empty") : Validation.Ok;
    }

    [ValueObject(typeof(string))]
    public partial struct NameAsStruct
    {
        private static Validation Validate(string value) =>
            string.IsNullOrEmpty(value) ? Validation.Invalid("Must not be null or empty") : Validation.Ok;
    }

}
