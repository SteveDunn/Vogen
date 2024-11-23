namespace Vogen.Types;

internal class ProjectName
{
    private ProjectName(string value) => Value = value;
    
    public static ProjectName FromAssemblyName(string assemblyName)
    {
        assemblyName = assemblyName.Replace(".", "");
        assemblyName = assemblyName.Replace(",", "");
        assemblyName = assemblyName.Replace(" ", "");

        return new(assemblyName);
    }

    public string Value { get; }
    
    public static implicit operator string(ProjectName name) => name.Value;
    public override string ToString() => Value;
}