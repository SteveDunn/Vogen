using System;
using System.Threading.Tasks;
using Vogen;
Task<CustomerId> t = Task.FromResult<CustomerId>(new());
Task<CustomerId> t2 = Task.FromResult<CustomerId>(default);

[ValueObject(typeof(int))]
public partial struct CustomerId { }