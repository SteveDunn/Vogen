namespace Vogen.Types;

internal class ProjectName
{
    private ProjectName(string value) => Value = value;
    
    /// <summary>
    /// Returns a <see cref="ProjectName"/> from the MSBuild <c>RootNamespace</c> property when it is set,
    /// otherwise falls back to <see cref="FromAssemblyName"/>.
    /// </summary>
    public static ProjectName FromRootNamespaceOrAssemblyName(string? rootNamespace, string assemblyName) =>
        !string.IsNullOrWhiteSpace(rootNamespace) ? new(rootNamespace!) : FromAssemblyName(assemblyName);

    /// <summary>
    /// Replaces [., -] with [_] for use as type type names etc., and ensures the result
    /// does not start with a digit (which would produce an invalid C# identifier).
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    public static ProjectName FromAssemblyName(string assemblyName)
    {
        assemblyName = assemblyName.Replace(".", "_");
        assemblyName = assemblyName.Replace(",", "_");
        assemblyName = assemblyName.Replace(" ", "_");
        assemblyName = assemblyName.Replace("-", "_");

        if (assemblyName.Length > 0 && char.IsDigit(assemblyName[0]))
        {
            assemblyName = "_" + assemblyName;
        }

        return new(assemblyName);
    }

    public string Value { get; }
    
    public static implicit operator string(ProjectName name) => name.Value;
    public override string ToString() => Value;
}