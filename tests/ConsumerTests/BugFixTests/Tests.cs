namespace ConsumerTests.BugFixTests;

// There are no particular tests for this. The fact that it compiles without an issue
public class Test<T> : Attribute { }

[Test<byte[]>]
public class Foo { }

[ValueObject]
public partial struct Vo { }

public class Bar
{
    public Bar()
    {
        Vo.From(123);
    }
}

    
