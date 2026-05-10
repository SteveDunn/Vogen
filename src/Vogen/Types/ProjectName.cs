namespace Vogen.Types;

internal class ProjectName
{
    private ProjectName(string value) => Value = value;
    
    /// <summary>
    /// Returns a <see cref="ProjectName"/> from the MSBuild <c>RootNamespace</c> property when it is set,
    /// otherwise falls back to <see cref="FromAssemblyName"/>.  Either value is normalised.
    /// </summary>
    public static ProjectName FromRootNamespaceOrAssemblyName(string? rootNamespace, string assemblyName) =>
        !string.IsNullOrWhiteSpace(rootNamespace) ? new(Normalise(rootNamespace)) : FromAssemblyName(assemblyName);

    /// <summary>
    /// Replaces [., ,, space, -] with [_] for use as type names etc., and ensures the result
    /// does not start with a digit (which would produce an invalid C# identifier).
    /// </summary>
    public static ProjectName FromAssemblyName(string assemblyName) => new(Normalise(assemblyName));

    private static string Normalise(string value)
    {
        value = value.Replace(".", "_");
        value = value.Replace(",", "_");
        value = value.Replace(" ", "_");
        value = value.Replace("-", "_");

        if (value.Length > 0 && char.IsDigit(value[0]))
        {
            value = "_" + value;
        }

        return value;
    }

    public string Value { get; }
    
    public static implicit operator string(ProjectName name) => name.Value;
    public override string ToString() => Value;
}