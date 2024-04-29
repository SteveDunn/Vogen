# Performance

<note>
This topic is incomplete and is currently being improved.
</note>


(to run these yourself: `dotnet run -c Release --framework net8.0 -- --job short --filter *` in the `Vogen.Benchmarks` folder)

As mentioned previously, the goal of Vogen is to achieve similar performance compared to using primitives themselves.
Here's a benchmark comparing a validated Value Object with an underlying type of `int` vs. using an `int` natively (*primitively* 🤓)

``` ini
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22621.1194)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK=7.0.102
  [Host]   : .NET 7.0.2 (7.0.222.60605), X64 RyuJIT AVX2
  ShortRun : .NET 7.0.2 (7.0.222.60605), X64 RyuJIT AVX2
Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3    
```

|         Method         |   Mean   |  Error   |  StdDev  | Ratio | RatioSD | Gen0 | Allocated |
|:----------------------:|:--------:|:--------:|:--------:|:-----:|:-------:|:----:|:---------:|
|    UsingIntNatively    | 14.55 ns | 1.443 ns | 0.079 ns | 1.00  |  0.00   |  -   |     -     |
| UsingValueObjectStruct | 14.88 ns | 3.639 ns | 0.199 ns | 1.02  |  0.02   |  -   |     -     |

There is no discernible difference between using a native int and a VO struct; both are pretty much the same in terms of speed and memory.

The next most common scenario is using a VO class to represent a native `String`.  These results are:

``` ini
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22621.1194)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK=7.0.102
  [Host]   : .NET 7.0.2 (7.0.222.60605), X64 RyuJIT AVX2
  ShortRun : .NET 7.0.2 (7.0.222.60605), X64 RyuJIT AVX2
Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3 
```

| Method                   | Mean     | Error | StdDev | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|--------------------------|----------|-------|--------|-------|---------|--------|-----------|-------------|
| UsingStringNatively      | 151.8 ns | 32.19 | 1.76   | 1.00  | 0.00    | 0.0153 | 256 B     | 1.00        |
| UsingValueObjectAsStruct | 184.8 ns | 12.19 | 0.67   | 1.22  | 0.02    | 0.0153 | 256 B     | 1.00        |


There's a minor performance overhead, but these measurements are incredibly small. Also, there's no memory overhead.
