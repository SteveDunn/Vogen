# Fenster's History

## Learnings

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
