#nullable enable

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using ProtoBuf;
using ProtoBuf.Grpc.Client;
using ProtoBuf.Grpc.Server;
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
        
        try
        {
            // Find an available port
            int port = GetAvailablePort();
            string serverUrl = $"http://127.0.0.1:{port}";
            
            Console.WriteLine($"Starting gRPC server on {serverUrl}...");

            // Build and start the ASP.NET Core gRPC server
            var builder = WebApplication.CreateBuilder();
            
            // Configure services for gRPC
            builder.Services.AddCodeFirstGrpc();
            builder.WebHost.UseUrls(serverUrl);
            builder.WebHost.UseKestrel(options =>
            {
                options.Listen(IPAddress.Loopback, port, listenOptions =>
                {
                    listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
                });
            });

            var app = builder.Build();
            
            // Map the gRPC service
            app.MapGrpcService<Service>();
            
            // Start the server in the background
            await app.StartAsync();
            
            Console.WriteLine("gRPC server started successfully.");

            try
            {
                // Create a gRPC channel and client
                using var channel = GrpcChannel.ForAddress(serverUrl);
                var client = channel.CreateGrpcService<IService>();
                
                Console.WriteLine("Calling gRPC service over HTTP/2...");
                
                // Call the service through the real gRPC channel
                var person = await client.GetDataAsync();
                
                Console.WriteLine($"✓ Received Person over gRPC: name={person.Name}, age={person.Age}, boxId={person.BoxId}, temperature={person.Temperature}");
                Console.WriteLine("✓ protobuf-net serialization over gRPC verified successfully!");
            }
            finally
            {
                // Shutdown the server
                await app.StopAsync();
                await app.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error running gRPC scenario: {ex.Message}");
            Console.WriteLine("This scenario requires HTTP/2 support.");
        }
    }

    private static int GetAvailablePort()
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        return ((IPEndPoint)socket.LocalEndPoint!).Port;
    }
}

[ProtoContract]
public class Person
{
    [ProtoMember(1)] public Name? Name { get; set; }
    [ProtoMember(2)] public Age? Age { get; set; }
    [ProtoMember(3)] public BoxId? BoxId { get; set; }
    [ProtoMember(4)] public Temperature? Temperature { get; set; }
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
    public static implicit operator VogenSurrogate<TW, TP>?(TW? value) => value is null ? null :  new VogenSurrogate<TW, TP> { Value = value.Value };
}
