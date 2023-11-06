# Installation

Vogen is compatible with .NET projects targeting:

<note>
Change `3.0.23` for the <a href="https://www.nuget.org/packages/Vogen">latest version listed on NuGet</a>
</note>


<tabs>
<tab title=".NET CLI">
<code xml:lang="bash">dotnet add package Vogen --version 3.0.23</code>
</tab>
<tab title="Package Manger">
<code xml:lang="bash">NuGet\Install-Package Vogen -Version 3.0.23</code>
</tab>
<tab title="Package Reference">
<code-block>
<![CDATA[
    <PackageReference Include="Vogen" Version="3.0.23" />
]]>
</code-block>
</tab>
</tabs>

When added to your project, the **source generator** generates the wrappers for your primitives and the **code analyzer**
will let you know if you try to create invalid Value Objects.
