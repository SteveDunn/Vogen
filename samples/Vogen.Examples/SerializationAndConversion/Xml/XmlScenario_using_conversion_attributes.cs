#if NULLABLE_DISABLED_BUILD
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#endif

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Vogen.Examples.SerializationAndConversion.XmlScenario.UsingConversionAttributes;

// There are issues deserializing back to structs, so we use classes instead. https://github.com/dotnet/runtime/issues/99613
[ValueObject<int>(conversions: Conversions.XmlSerializable)]
public partial class Age;

[ValueObject<int>(conversions: Conversions.XmlSerializable)]
public partial class PersonId;

[ValueObject<string>(conversions: Conversions.XmlSerializable)]
public partial class Name;

public class Person
{
    public PersonId Id { get; set; }
    
    public Name Name { get; set; }
    
    public Age Age { get; set; }
}

[DataContract(Name = "Person", Namespace = "urn:sample")] // customize root
public class PersonDc
{
    [DataMember(Order = 3)] public PersonId Id { get; set; }
    [DataMember(Order = 2)] public Name Name { get; set; }
    [DataMember(Order = 1, Name = "SomethingElse")] public Age Age { get; set; }

    [IgnoreDataMember] // never serialized
    public string InternalNote { get; set; }
}

[UsedImplicitly]
public class XmlScenario : IScenario
{
    public Task Run()
    {
        RunXmlSerializer();
        DataContractSerializer();
        
        return Task.CompletedTask;
    }

    private void RunXmlSerializer()
    {
        var originalObject = new Person
        {
            Id = PersonId.From(123),
            Name = Name.From("Test"),
            Age = Age.From(42)
        };
        
        var serializer = new XmlSerializer(typeof(Person));
        string xml;
        using (var sw = new StringWriter())
        using (var xw = XmlWriter.Create(sw, new XmlWriterSettings { Indent = true }))
        {
            serializer.Serialize(xw, originalObject);
            xml = sw.ToString();
        }

        Person deserializedObject;
        using (var sr = new StringReader(xml))
        {
            deserializedObject = (Person)serializer.Deserialize(sr)!;
        }

        Console.WriteLine($"Id: {deserializedObject.Id}, Name: {deserializedObject.Name}, Active: {deserializedObject.Age}");
    }

    private void DataContractSerializer()
    {
        var originalObject = new PersonDc
        {
            Id = PersonId.From(123),
            Name = Name.From("Test"),
            Age = Age.From(42)
        };

        var dcs = new DataContractSerializer(
            type: typeof(PersonDc),
            settings: new DataContractSerializerSettings
            {
                PreserveObjectReferences = false
            });

        // Serialize to UTF-8 bytes in memory
        byte[] xmlBytes;
        using (var ms = new MemoryStream())
        using (var xw = XmlWriter.Create(ms, new XmlWriterSettings
               {
                   Indent = true,
                   Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)
               }))
        {
            dcs.WriteObject(xw, originalObject);
            xw.Flush();
            xmlBytes = ms.ToArray();
        }

        // For display
        string xml = Encoding.UTF8.GetString(xmlBytes);

        // Deserialize from UTF-8 bytes
        PersonDc deserializedObject;
        using (var ms = new MemoryStream(xmlBytes))
        using (var xr = XmlReader.Create(ms))
        {
            deserializedObject = (PersonDc)dcs.ReadObject(xr)!;
        }

        // Show resulting XML and values
        Console.WriteLine(xml);
        Console.WriteLine($"Id: {deserializedObject.Id}, Name: {deserializedObject.Name}, Age: {deserializedObject.Age}");
    }}
