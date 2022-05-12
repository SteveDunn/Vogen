namespace Vogen;

public class InstanceProperties
{
    public InstanceProperties(string name, object value, string tripleSlashComments)
    {
        Name = name;
        Value = value;
        TripleSlashComments = tripleSlashComments;
    }

    public string Name { get; }
    
    public object Value { get; }
    public string TripleSlashComments { get; }
}