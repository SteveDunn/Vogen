using System.Threading;
using Xunit.Abstractions;

namespace ConsumerTests.StringComparisons;


[Collection("Sequential")]
[UseCulture("tr-TR")]
public class LocaleTests
{
    private readonly ITestOutputHelper _logger;

    public LocaleTests(ITestOutputHelper logger) => _logger = logger;

    [Fact]
    public void Class()
    {
        _logger.WriteLine(Thread.CurrentThread.CurrentCulture.Name);
        
        // i = İ => true
        var left = StringVo_Class.From("i");
        var right = StringVo_Class.From("\u0130");

        _logger.WriteLine($"Left: {left} Right: {right}");

        "i".Equals("\u0130", StringComparison.CurrentCultureIgnoreCase).Should().BeTrue();
        
        left.Equals(right, StringVo_Class.Comparers.CurrentCultureIgnoreCase).Should().BeTrue();
    }

    [Fact]
    public void RecordStruct()
    {
        _logger.WriteLine(Thread.CurrentThread.CurrentCulture.Name);
        // i = İ => true
        var left = StringVo_RecordStruct.From("i");
        var right = StringVo_RecordStruct.From("\u0130");

        _logger.WriteLine($"Left: {left} Right: {right}");
        "i".Equals("\u0130", StringComparison.CurrentCultureIgnoreCase).Should().BeTrue();
        left.Equals(right, StringVo_RecordStruct.Comparers.CurrentCultureIgnoreCase).Should().BeTrue();
    }

    [Fact]
    public void RecordClass()
    {
        _logger.WriteLine(Thread.CurrentThread.CurrentCulture.Name);
        // i = İ => true
        var left = StringVo_RecordClass.From("i");
        var right = StringVo_RecordClass.From("\u0130");

        _logger.WriteLine($"Left: {left} Right: {right}");

        "i".Equals("\u0130", StringComparison.CurrentCultureIgnoreCase).Should().BeTrue();
        left.Equals(right, StringVo_RecordClass.Comparers.CurrentCultureIgnoreCase).Should().BeTrue();
    }

    [Fact]
    public void Struct()
    {
        _logger.WriteLine(Thread.CurrentThread.CurrentCulture.Name);
        // i = İ => true
        var left = StringVo_Struct.From("i");
        var right = StringVo_Struct.From("\u0130");

        _logger.WriteLine($"Left: {left} Right: {right}");
        "i".Equals("\u0130", StringComparison.CurrentCultureIgnoreCase).Should().BeTrue();
        left.Equals(right, StringVo_Struct.Comparers.CurrentCultureIgnoreCase).Should().BeTrue();
    }
}