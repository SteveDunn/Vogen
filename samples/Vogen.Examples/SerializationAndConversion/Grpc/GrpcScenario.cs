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
    public required Name Name { get; init; }
    public required Age Age { get; init; }
    public required BoxId BoxId { get; init; }
    public required Temperature Temperature { get; init; }
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

// TW = wrapper, TP = primitive (underlying)
[ProtoContract]
public class VogenSurrogate<TW, TP> where TW: IVogen<TW, TP>
{
    [ProtoMember(1)] public TP Value { get; set; } = default!;

    public static implicit operator TW(VogenSurrogate<TW, TP> surrogate) => TW.From(surrogate.Value);
    public static implicit operator VogenSurrogate<TW, TP>(TW value) => new() { Value = value.Value };
}




