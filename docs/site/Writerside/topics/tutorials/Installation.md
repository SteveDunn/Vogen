# Installing Vogen

<note>
These tutorials assume a working knowledge of .NET and C#, so won't include the basics necessary to start the
tutorials, e.g. things like creating new projects, creating new types, compiling, viewing error output, etc.
</note>

<tabs>
<tab title=".NET CLI">
<code xml:lang="bash">dotnet add package Vogen --version %latest_version%</code>
</tab>
<tab title="Package Manger">
<code xml:lang="bash">NuGet\Install-Package Vogen -Version %latest_version%</code>
</tab>
<tab title="Package Reference">
<code-block>
<![CDATA[
    <PackageReference Include="Vogen" Version="%latest_version%" />
]]>
</code-block>
</tab>
</tabs>

<note>
Change `%latest_version%` for the <a href="https://www.nuget.org/packages/Vogen">latest version listed on NuGet</a>
</note>

When added to your project, the **source generator** generates the wrappers for your primitives and the **code analyzer**
will let you know if you try to create invalid Value Objects.

Vogen consists of two dlls:

1. `Vogen.dll` - this is the source generator and analyzers and is not needed at runtime
2. `Vogen.SharedTypes.dll` - this contains the types that **are** referenced at runtime, e.g. `ValueObjectValidationException` and `ValueObjectOrError` which is used by the `TryFrom` method.

Unlike using some other source generators, specifying `PrivateAssets="all" ExcludeAssets="runtime"` with Vogen will cause exceptions at runtime because of missing types.

`Vogen.SharedTypes.dll` is about 18KB, so will add little to your application's size.
It is referenced on its own.
Here is a snippet from a `deps.json` file with it specified:

```xml
"Vogen/4.0.14": {
        "runtime": {
          "lib/net8.0/Vogen.SharedTypes.dll": {
            "assemblyVersion": "4.0.0.0",
            "fileVersion": "%latest_version%"
          }
        }
```
