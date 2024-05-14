using RefitExample;
using ServiceStackDotTextExample;
using Vogen;

[assembly: VogenDefaults(conversions:Conversions.ServiceStackDotText | Conversions.SystemTextJson, staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon)]

namespace Whatever
{
    public static class Program
    {
        public static async Task Main()
        {
            await ServiceStackTextRunner.Run();
            await RefitRunner.Run();

            var x = Factory<CustomerId>(11);

            static T Factory<T>(int value) where T : IVogen<T, int> => T.From(value);
        }
    }

    [ValueObject]
    public readonly partial struct CustomerId
    {
    }

    [ValueObject]
    public partial struct SupplierId;
} 