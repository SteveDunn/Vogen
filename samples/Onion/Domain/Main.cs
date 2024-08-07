﻿using Vogen;

// We don't need to emit the System.Text.Json converter factory because
// System.Text.Json, in the Infra project (or anything that references this)
// will have access to the 'fully formed' value objects.
[assembly: VogenDefaults(
    systemTextJsonConverterFactoryGeneration: SystemTextJsonConverterFactoryGeneration.Omit, 
    conversions: Conversions.SystemTextJson | 
                 Conversions.TypeConverter)]

namespace Domain;

[ValueObject]
public partial struct CustomerId;

[ValueObject<string>]
public partial struct CustomerName;

[ValueObject]
public partial struct OrderId;

public class Order
{
    public CustomerId CustomerId { get; set; }
    public OrderId OrderId { get; set; }
    public CustomerName CustomerName { get; set; }
}

public class EmployeeEntity
{
    public Id Id { get; set; } = null!; // must be null in order for EF core to generate a value
    public required Name Name { get; set; } = Name.NotSet;
    public required Age Age { get; set; }
    
    public required Department Department { get; set; }
    
    public required HireDate HireDate { get; set; }
}

[ValueObject]
public partial class Id;

[ValueObject<string>]
[Instance("NotSet", "[NOT_SET]")]
public partial class Name;

[ValueObject]
public readonly partial struct Age;

[ValueObject<string>]
public readonly partial record struct Department;

[ValueObject<DateOnly>]
public partial record class HireDate;