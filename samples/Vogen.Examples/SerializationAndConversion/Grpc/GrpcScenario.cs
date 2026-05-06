
using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ProtoBuf;
using System.ServiceModel;

namespace Vogen.Examples.SerializationAndConversion.Grpc;


[ValueObject(typeof(string))]
[ProtoContract(Surrogate = typeof(BoxIdSurrogate))]
public partial class BoxId;

[ProtoContract]
public class BoxIdSurrogate
{
    [ProtoMember(1)]
    public string Value { get; set; } = "";

    public static implicit operator BoxId(BoxIdSurrogate surrogate) => BoxId.From(surrogate.Value);
    public static implicit operator BoxIdSurrogate(BoxId value) => new() { Value = value.Value };
}


[UsedImplicitly]
public class GrpcScenario : IScenario
{
    public async Task Run()
    {
        var generator = new ProtoBuf.Grpc.Reflection.SchemaGenerator();

        string schema = generator.GetSchema<IService>();
        
        IService service = new Service();
        BoxId boxId = await service.GetDataAsync();
        
        Console.WriteLine($"Received BoxId: {boxId}");
    }
}

[ServiceContract]
public interface IService
{
    Task<BoxId> GetDataAsync(CancellationToken cancellation = default);
}

public class Service : IService
{
    public Task<BoxId> GetDataAsync(CancellationToken cancellation = default)
    {
        return Task.FromResult(BoxId.From("hardcoded-box-id"));
    }
}



