namespace Domain;

public class EmployeeEntity
{
    public Id Id { get; set; } = null!; // must be null in order for EF core to generate a value
    public required Name Name { get; set; } = Name.NotSet;
    public required Age Age { get; set; }
    
    public required Department Department { get; set; }
    
    public required HireDate HireDate { get; set; }
}