namespace Vogen.Types;

internal class SanitizedAssemblyName
{
    public SanitizedAssemblyName(string? unsanitized)
    {
        var assemblyName = unsanitized?.Replace(".", "_") ?? "";
        if (assemblyName.EndsWith("_dll") || assemblyName.EndsWith("_exe"))
        {
            assemblyName = assemblyName[..^4];
        }

        Value = assemblyName;
    }

    public string Value { get; set; }
    public static implicit operator string(SanitizedAssemblyName name) => name.Value;
    public override string ToString() => Value;
}