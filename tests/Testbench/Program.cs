using Vogen;

namespace Whatever;


CustomerId c = default;

[ValueObject(typeof(int))]
public partial class CustomerId { }

public class Foo
{
    public void DoSomething(CustomerId customerId = default) { }
}
