using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Vogen;
[assembly: VogenDefaults(
    primitiveEqualityGeneration: PrimitiveEqualityGeneration.GenerateOperatorsAndMethods,
    staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon)]

//[ValueObject(primitiveEqualityGeneration: PrimitiveEqualityGeneration.Omit)]
[ValueObject]
public readonly partial struct CustomerId 
{
    
}

[ValueObject]
public readonly partial struct SomethingElseId 
{
    
}

[ValueObject]
public readonly partial struct SupplierId;
