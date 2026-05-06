using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ProtoBuf;
using System.ServiceModel;

namespace Vogen.Examples.SerializationAndConversion.Grpc;

[UsedImplicitly]
public class GrpcScenario : IScenario
{
    public async Task Run()
    {
        var generator = new ProtoBuf.Grpc.Reflection.SchemaGenerator();

        string schema = generator.GetSchema<IService>();
        Console.WriteLine("Schema is: " + schema);
        
        IService service = new Service();
        var person = await service.GetDataAsync();
        
        Console.WriteLine($"Received Person:  name={person.Name}, age={person.Age}, boxId={person.BoxId}, temperature={person.Temperature}");
    }
}

public class Person
{
    public Name Name { get; init; }
    public Age Age { get; init; }
    public BoxId BoxId { get; init; }
    public Temperature Temperature { get; init; }
}

[ServiceContract]
public interface IService
{
    Task<Person> GetDataAsync(CancellationToken cancellation = default);
}

public class Service : IService
{
    public Task<Person> GetDataAsync(CancellationToken cancellation = default)
    {
        return Task.FromResult(
            new Person
            {
                Age = Age.From(42),
                BoxId = BoxId.From("123"),
                Name = Name.From("Fred"),
                Temperature = Temperature.From(12.3)
            }
        );
    }
}

[ValueObject<string>]
[ProtoContract(Surrogate = typeof(VogenSurrogate<BoxId, string>))]
public partial class BoxId;

[ValueObject<string>]
[ProtoContract(Surrogate = typeof(VogenSurrogate<Name, string>))]
public partial class Name;

[ValueObject<int>]
[ProtoContract(Surrogate = typeof(VogenSurrogate<Age, int>))]
public partial class Age;

[ValueObject<double>]
[ProtoContract(Surrogate = typeof(VogenSurrogate<Temperature, double>))]
public partial class Temperature;

[ProtoContract]
public class VogenSurrogate<TW, TP> where TW: IVogen<TW, TP>
{
    [ProtoMember(1)]
    public string Value { get; set; } = "";

    public static implicit operator BoxId(VogenSurrogate<TW, TP> surrogate) => BoxId.From(surrogate.Value);
    public static implicit operator VogenSurrogate<TW, TP>(BoxId value) => new() { Value = value.Value };
}




