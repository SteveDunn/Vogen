using System;
using System.Globalization;
using System.Runtime.Serialization;
using Vogen;
using Vogen.EfCoreTest;

[assembly: VogenDefaults(
    openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateSwashbuckleSchemaFilter | OpenApiSchemaCustomizations.GenerateSwashbuckleMappingExtensionMethod,
    staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon | StaticAbstractsGeneration.InstanceMethodsAndProperties, 
    conversions: Conversions.EfCoreValueConverter)]

namespace Testbench;

public static class Program
{
    public static void Main()
    {
        EfCoreScenario.Run();
    }
}
