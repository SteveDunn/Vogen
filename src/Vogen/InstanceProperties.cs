namespace Vogen;

public class InstanceProperties
{
    public InstanceProperties(string name, string valueAsText, object value, string tripleSlashComments)
    {
        Name = name;
        ValueAsText = valueAsText;
        Value = value;
        TripleSlashComments = tripleSlashComments;
    }

    public string Name { get; }
    public string ValueAsText { get; }

    public object Value { get; }
    public string TripleSlashComments { get; }
}