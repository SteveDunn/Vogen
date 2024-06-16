namespace Vogen.Generators.Conversions;

internal static class GenerateEfCoreExtensions
{
    public static string GenerateInnerIfNeeded(VoWorkItem item)
    {
        if (!HasEfCoreFlagSetOnAttribute(item.Config.Conversions))
        {
            return string.Empty;
        }

        string accessibility = item.AccessibilityKeyword;

        return $$"""
                 #if NETCOREAPP3_0_OR_GREATER
                             public static class __{{item.VoTypeName}}EfCoreExtensions 
                             {
                                 {{accessibility}} static global::Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder<{{item.VoTypeName}}> HasVogenConversion(this global::Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder<{{item.VoTypeName}}> propertyBuilder) =>
                                     propertyBuilder.HasConversion<{{item.VoTypeName}}.EfCoreValueConverter, {{item.VoTypeName}}.EfCoreValueComparer>();
                             }
                 #endif
                 """;
    }

    private static bool HasEfCoreFlagSetOnAttribute(Vogen.Conversions conversions) => conversions.HasFlag(Vogen.Conversions.EfCoreValueConverter);
}