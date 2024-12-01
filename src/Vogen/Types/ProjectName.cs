namespace Vogen.Types;

internal class ProjectName
{
    private ProjectName(string value) => Value = value;
    
    /// <summary>
    /// Replaces [., -] with [_] for use as type type names etc.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    public static ProjectName FromAssemblyName(string assemblyName)
    {
        assemblyName = assemblyName.Replace(".", "_");
        assemblyName = assemblyName.Replace(",", "_");
        assemblyName = assemblyName.Replace(" ", "_");
        assemblyName = assemblyName.Replace("-", "_");

        return new(assemblyName);
    }

    public string Value { get; }
    
    public static implicit operator string(ProjectName name) => name.Value;
    public override string ToString() => Value;
}