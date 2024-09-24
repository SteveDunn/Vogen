using System.ComponentModel;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Jobs;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Vogen.Benchmarks.NoValidation
{
    [MemoryDiagnoser, InliningDiagnoser(logFailuresOnly: false, filterByNamespace: true), Description("The underlying type is int and there is no validation")]
    // [SimpleJob(RuntimeMoniker.Net60)]
    // [SimpleJob(RuntimeMoniker.Net80)]
    public class Underlying_Int_With_No_Validation
    {
        private volatile int _n1;
        private volatile int _n2;
        private volatile NumberAsClass _c1;
        private volatile NumberAsClass _c2;
        private NumberAsStruct _s1;
        private NumberAsStruct _s2;
        

        [GlobalSetup]
        public void GlobalSetup()
        {
            _n1 = TestData.RandomNumberBetween(1, 10_000);
            _n2 = TestData.RandomNumberBetween(1, 10_000);
            
            _s1 = NumberAsStruct.From(_n1);
            _s2 = NumberAsStruct.From(_n2);
            
            _c1 = NumberAsClass.From(_n1);
            _c2 = NumberAsClass.From(_n2);
        }

        [Benchmark(Baseline = true)]
        public int UsingIntNatively() => Calculator.Add(_n1, _n2);

        [Benchmark]
        public NumberAsStruct UsingValueObjectStruct() => Calculator.Add(_s1, _s2);

        [Benchmark]
        public NumberAsClass UsingValueObjectAsClass() => Calculator.Add(_c1, _c2);
    }

    [MemoryDiagnoser, InliningDiagnoser(logFailuresOnly: false, filterByNamespace: true), Description("The underlying type is string and there is no validation")]
    // [SimpleJob(RuntimeMoniker.Net60)]
    // [SimpleJob(RuntimeMoniker.Net80)]
    public class Underlying_string_With_No_Validation
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
        public string UsingStringNatively() => Combine(_s1, _s2);

        [Benchmark]
        public NameAsStruct UsingValueObjectAsStruct() => Combine(_wrapperStruct1, _wrapperStruct2);

        [Benchmark]
        public NameAsClass UsingValueObjectAsClass() => Combine(_wrapperClass1, _wrapperClass2);
    }

    public static class Calculator
    {
        public static int Add(int n1, int n2)
        {
            int n = 0;
            for (int i = 0; i < 1000; i++) n += n1 + n2;
            return n;
        }

        public static NumberAsStruct Add(NumberAsStruct n1, NumberAsStruct n2)
        {
            int n = 0;
            for (int i = 0; i < 1000; i++) n += n1.Value + n2.Value;
            return NumberAsStruct.From(n);
        }

        public static NumberAsClass Add(NumberAsClass n1, NumberAsClass n2)
        {
            int n = 0;
            for (int i = 0; i < 1000; i++) n += n1.Value + n2.Value;
            return NumberAsClass.From(n);
        }
    }

    [ValueObject<int>]
    public partial struct NumberAsStruct;

    [ValueObject<int>]
    public partial class NumberAsClass;

    [ValueObject<string>]
    public partial struct NameAsStruct;

    [ValueObject<string>]
    public partial class NameAsClass;
}
