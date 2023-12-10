# Installing Vogen

<note>
These tutorials assume a working knowledge of .NET and C#, so won't include the basics necessary to start the
tutorials, e.g. things like creating new projects, creating new types, compiling, viewing error output, etc.
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

<note>
Change `3.0.23` for the <a href="https://www.nuget.org/packages/Vogen">latest version listed on NuGet</a>
</note>


When added to your project, the **source generator** generates the wrappers for your primitives and the **code analyzer**
will let you know if you try to create invalid Value Objects. 
