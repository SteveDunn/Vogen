using Vogen;

[ValueObject(typeof(int), conversions: Conversions.TypeConverter | Conversions.DapperTypeHandler)]
public partial struct DapperCustomerIdInt { }

[ValueObject(typeof(int), conversions: Conversions.TypeConverter | Conversions.EfCoreValueConverter)]
public partial struct EfCoreCustomerIdInt { }