namespace Vogen;

public class InstanceProperties
{
    public InstanceProperties(string name, object value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }
    
    public object Value { get; }
}