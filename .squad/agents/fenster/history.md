# Fenster's History

## Learnings

### 2026-05-06: Issue #909 - VOG038 for Nullable<T>.GetValueOrDefault()

**Issue**: `Nullable<T>.GetValueOrDefault()` with no arguments can manufacture `default(T)` for Vogen struct value objects, bypassing validation and producing an invalid instance.

**Implementation Summary**:
- Added analyzer `DoNotUseGetValueOrDefaultAnalyzer` using `OperationKind.Invocation`
- Restricted the rule to the zero-argument overload on `System.Nullable<T>` only
- Reused `VoFilter.IsTarget(...)` to ensure `T` is a Vogen value object before reporting `VOG038`
- Left the one-argument overload alone because callers supply an explicit fallback value
- Added analyzer tests covering struct/readonly struct/record struct forms plus safe non-diagnostic cases

**Verification**:
- `dotnet build src\\Vogen\\Vogen.csproj -c Release -v q`

---

### 2026-05-05: Issue #838 - Protobuf Surrogate Investigation

**Issue**: GitHub issue #838 reports that the README example for protobuf-net usage fails with:
```
'Data of this type has inbuilt behaviour, and cannot be added to a model in this way: System.String'
```

**Investigation Summary**:

1. **Vogen does NOT generate protobuf-specific code**
   - Searched entire `src/Vogen/` directory
   - No protobuf-related code generation exists
   - No `Conversions.ProtoBuf` flag in `src/Vogen.SharedTypes/Conversions.cs` (only has: TypeConverter, NewtonsoftJson, SystemTextJson, EfCoreValueConverter, DapperTypeHandler, LinqToDbValueConverter, ServiceStackDotText, Bson, Orleans, XmlSerializable, MessagePack)
   - The only static constructor generation is in `src/Vogen/GenerateCodeForStaticConstructors.cs` (lines 36-100) for **ServiceStack only**

2. **The Problem is in the Documentation**
   - Current README (line 624) and FAQ (line 428) show this pattern:
     ```csharp
     [ValueObject<string>]
     [ProtoContract(Surrogate = typeof(string))]
     public partial class BoxId;
     ```
   - This pattern is **INVALID** according to protobuf-net documentation
   - You **CANNOT** use a primitive type like `string` as a surrogate in protobuf-net
   - protobuf-net throws the error when `SchemaGenerator` or `RuntimeTypeModel` encounters this pattern

3. **Root Cause**
   - When `protobuf-net.Grpc.Reflection.SchemaGenerator` scans types, it discovers `BoxId` with `[ProtoContract(Surrogate = typeof(string))]`
   - It tries to register: `RuntimeTypeModel.Default.Add(typeof(BoxId), ...).SetSurrogate(typeof(string))`
   - protobuf-net rejects this because `string` has "inbuilt behaviour" and cannot be a surrogate target
   - **A surrogate MUST be a [ProtoContract] class, not a primitive**

4. **The Fix**
   - The documentation pattern in README.md (line 624) and FAQ.md (line 428) is incorrect and should be removed or corrected
   - Users should NOT use `[ProtoContract(Surrogate = typeof(string))]` on their ValueObjects
   - Correct approaches:
     - **Option A**: Don't use `[ProtoContract]` at all - let protobuf-net serialize the `Value` property directly
     - **Option B**: Create a proper surrogate class (a `[ProtoContract]` type with conversion logic) if custom serialization is needed
     - **Option C**: Add a `Conversions.ProtoBuf` flag to Vogen and generate proper surrogate classes (future enhancement)

**Files Examined**:
- `src/Vogen/GenerateCodeForStaticConstructors.cs` - Only generates for ServiceStack
- `src/Vogen/GenerateCodeForOrleansSerializers.cs` - Orleans-specific, not protobuf
- `src/Vogen/Generators/ClassGenerator.cs` (line 65) - Calls GenerateCodeForStaticConstructors
- `src/Vogen/Generators/StructGenerator.cs` (line 65) - Calls GenerateCodeForStaticConstructors
- `src/Vogen/Generators/RecordClassGenerator.cs` (line 75) - Calls GenerateCodeForStaticConstructors
- `src/Vogen/Generators/RecordStructGenerator.cs` (line 74) - Calls GenerateCodeForStaticConstructors
- `src/Vogen.SharedTypes/Conversions.cs` - No protobuf flag exists
- `tests/SnapshotTests/` - No protobuf-related snapshot tests found

**Conclusion**:
This is not a bug in Vogen's code generation. It's **incorrect documentation** that suggests an invalid protobuf-net pattern. The README should be corrected to remove or fix the protobuf example.

---

### 2026-05-15: Enhanced gRPC Scenario with Real Server Testing

**Task**: Enhance `samples\Vogen.Examples\SerializationAndConversion\Grpc\GrpcScenario.cs` to test real protobuf-net serialization over HTTP/2 gRPC.

**Implementation**: 
- Used in-process ASP.NET Core hosting approach (standard pattern for .NET gRPC integration tests)
- Created real HTTP/2 server with `WebApplication` and Kestrel
- Used `protobuf-net.Grpc.AspNetCore` for server-side code-first gRPC hosting
- Used `Grpc.Net.Client` for client channel creation
- Dynamic port allocation via socket binding to avoid port conflicts

**Key Patterns Discovered**:
1. **In-process ASP.NET Core testing pattern** is preferred over Docker/Testcontainers for custom gRPC services (unlike MongoDb scenario which uses pre-built images)
2. **Required NuGet packages for code-first gRPC**:
   - `protobuf-net.Grpc.AspNetCore` (v1.2.2) - for `AddCodeFirstGrpc()` and `MapGrpcService<T>()`
   - `Grpc.Net.Client` (v2.70.0) - for `GrpcChannel.ForAddress()`
   - `ProtoBuf.Grpc.Client` namespace - for `channel.CreateGrpcService<T>()` extension
   - `ProtoBuf.Grpc.Server` namespace - for `AddCodeFirstGrpc()` extension
3. **Framework reference needed**: Added `<FrameworkReference Include="Microsoft.AspNetCore.App" />` to csproj since it uses `Microsoft.NET.Sdk` (not Web SDK)
4. **Port allocation**: Use socket binding to `IPAddress.Loopback:0` to get OS-assigned available port
5. **HTTP/2 enforcement**: Must configure Kestrel with `HttpProtocols.Http2` explicitly for gRPC
6. **Proper cleanup**: Server must be stopped with `StopAsync()` and disposed in finally block
7. **Error handling**: Wrap in try/catch similar to Mongo scenario pattern

**Files Modified**:
- `samples\Vogen.Examples\Vogen.Examples.csproj` - Added packages and framework reference
- `samples\Vogen.Examples\SerializationAndConversion\Grpc\GrpcScenario.cs` - Complete rewrite with server hosting

**Build Verification**: ✓ `dotnet build samples\Vogen.Examples\Vogen.Examples.csproj -c Release` succeeds

**Architecture Notes**:
- This pattern tests the FULL serialization stack: Vogen value objects → VogenSurrogate → protobuf wire format → HTTP/2 → deserialize back
- The existing `VogenSurrogate<TW, TP>` pattern (using `IVogen<TW, TP>` static abstracts) works perfectly with code-first gRPC
- No changes needed to value object definitions or surrogate class

