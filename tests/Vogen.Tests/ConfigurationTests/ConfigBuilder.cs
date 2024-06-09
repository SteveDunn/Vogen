namespace Vogen.Tests.ConfigurationTests;

public class ConfigBuilder
{
    private VogenConfiguration _c = VogenConfiguration.DefaultInstance;

    public ConfigBuilder WithPrimitiveEqualityGeneration(PrimitiveEqualityGeneration v)
    {
        _c = _c with { PrimitiveEqualityGeneration = v };
                
        return this;
    }

    public ConfigBuilder WithComparable(ComparisonGeneration comparable)
    {
        _c = _c with { Comparison = comparable };
                
        return this;
    }

    public ConfigBuilder WithStringComparersGeneration(StringComparersGeneration g)
    {
        _c = _c with { StringComparers = g };
                
        return this;
    }

    public ConfigBuilder WithTryFromGeneration(TryFromGeneration g)
    {
        _c = _c with { TryFromGeneration = g, ExplicitlySpecifyTypeInValueObject = false };
                
        return this;
    }

    public ConfigBuilder WithIsInitializedGeneration(IsInitializedMethodGeneration g)
    {
        _c = _c with { IsInitializedMethodGeneration = g, ExplicitlySpecifyTypeInValueObject = false };
                
        return this;
    }

    public ConfigBuilder WithSystemTextJsonConverterFactoryGeneration(SystemTextJsonConverterFactoryGeneration g)
    {
        _c = _c with { SystemTextJsonConverterFactoryGeneration = g };
                
        return this;
    }

    public ConfigBuilder WithParsableStringGeneration(ParsableForStrings g)
    {
        _c = _c with { ParsableForStrings = g };
                
        return this;
    }

    public ConfigBuilder WithParsablePrimitiveGeneration(ParsableForPrimitives g)
    {
        _c = _c with { ParsableForPrimitives = g };
                
        return this;
    }

    public ConfigBuilder WithStaticAbstractsGeneration(StaticAbstractsGeneration g)
    {
        _c = _c with { StaticAbstractsGeneration = g };
                
        return this;
    }

    public ConfigBuilder WithSwashbuckleSchemaFilterGeneration(OpenApiSchemaCustomizations g)
    {
        _c = _c with { OpenApiSchemaCustomizations = g };
                
        return this;
    }

    public ConfigBuilder WithPrimitiveTypeMustBeExplicit(bool b)
    {
        _c = _c with { ExplicitlySpecifyTypeInValueObject = b };
                
        return this;
    }
            
    public VogenConfiguration Build() => _c;
}