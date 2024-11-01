using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Vogen;
// ReSharper disable UnusedVariable

[assembly: VogenDefaults(
    openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateSwashbuckleSchemaFilter |
                                 OpenApiSchemaCustomizations.GenerateSwashbuckleMappingExtensionMethod,
    staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon | StaticAbstractsGeneration.InstanceMethodsAndProperties,
    conversions: Conversions.EfCoreValueConverter)]

namespace Testbench;


[ValueObject<short>(conversions: Conversions.XmlSerializable)]
public partial class MyShort;

[ValueObject<int>(conversions: Conversions.XmlSerializable, primitiveEqualityGeneration: PrimitiveEqualityGeneration.GenerateOperatorsAndMethods)]
public partial class MyInt;

[ValueObject<DateOnly>]
public partial struct MyDateOnly;

[ValueObject<DateTime>]
public partial struct MyDateTime;

[ValueObject<DateTimeOffset>]
public partial struct MyDateTimeOffset;

[ValueObject<byte>]
public partial struct MyByte;

[ValueObject<long>]
public partial struct MyLong;

[Serializable]
public class C
{
    public required MyInt MyInt { get; set; }
    public required MyShort MyShort { get; set; }
}

public static class Program
{
    public static void Main()
    {
        MyInt i1 = MyInt.From(100);
        MyInt i2 = MyInt.From(200);
        bool smaller = i1.CompareTo(i2) < 0;
        bool smaller2 = i1 == 100;
        
        C c = new()
        {
            MyInt = MyInt.From(42),
            MyShort = MyShort.From(123)
        };
        
        

        // C2 c = new()
        // {
        //     MyInt = 42,
        //     MyShort = 111,
        //     MyShortXml = 222,
        // };


// Serialize 'c' to XML
        XmlSerializer serializer = new XmlSerializer(typeof(C));
        using StringWriter textWriter = new StringWriter();

        serializer.Serialize(textWriter, c);
        string xmlString = textWriter.ToString();
        Console.WriteLine("Serialized XML:\n" + xmlString);


// Deserialize XML back to a 'C' object
        using (StringReader textReader = new StringReader(xmlString))
        {
            C deserializedC = (C) serializer.Deserialize(textReader)!;
            Console.WriteLine("Deserialized Object:");
            Console.WriteLine("MyInt: " + deserializedC.MyInt);
            Console.WriteLine("MyShort: " + deserializedC.MyShort);
        }
    }
}