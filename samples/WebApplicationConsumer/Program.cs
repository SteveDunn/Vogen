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
        }
    }
} 