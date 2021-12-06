using System.ComponentModel;
using BenchmarkDotNet.Attributes;
using Vogen;

namespace Vogen.Benchmarks.NoValidation
{
    [MemoryDiagnoser, Description("The underlying type is int and there is no validation")]
    public class Underlying_Int_With_No_Validation
    {
        [Benchmark(Baseline = true)]
        public int UsingIntNatively()
        {
            int x = TestData.RandomNumberBetween(1, 10_000);
            int y = TestData.RandomNumberBetween(1, 10_000);
            return Calculator.Add(x, y);
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

    [MemoryDiagnoser, Description("The underlying type is string and there is no validation")]
    public class Underlying_string_With_No_Validation
    {
        private string Combine(string s1, string s2) => $"{s1}-{s2}";
        
        public NameAsClass Combine(NameAsClass s1, NameAsClass s2) => NameAsClass.From($"{s1}-{s2}");
        
        public NameAsStruct Combine(NameAsStruct s1, NameAsStruct s2) => NameAsStruct.From($"{s1}-{s2}");
        
        [Benchmark(Baseline = true)]
        public string UsingStringNatively() => 
            Combine(TestData.GetRandomString(), TestData.GetRandomString());

        [Benchmark]
        public NameAsStruct UsingValueObjectAsStruct() => 
            Combine(NameAsStruct.From(TestData.GetRandomString()), NameAsStruct.From(TestData.GetRandomString()));

        [Benchmark]
        public NameAsClass UsingValueObjectAsClass() => 
            Combine(NameAsClass.From(TestData.GetRandomString()), NameAsClass.From(TestData.GetRandomString()));
    }

    public static class Calculator
    {
        public static int Add(int n1, int n2) => n1 + n2;

        public static NumberAsStruct Add(NumberAsStruct n1, NumberAsStruct n2) => NumberAsStruct.From(n1.Value + n2.Value);

        public static NumberAsClass Add(NumberAsClass n1, NumberAsClass n2) => NumberAsClass.From(n1.Value + n2.Value);
    }

    [ValueObject(typeof(int))]
    public partial struct NumberAsStruct
    {
    }

    [ValueObject(typeof(int))]
    public partial class NumberAsClass
    {
    }

    [ValueObject(typeof(string))]
    public partial struct NameAsStruct
    {
    }

    [ValueObject(typeof(string))]
    public partial class NameAsClass
    {
    }


}
