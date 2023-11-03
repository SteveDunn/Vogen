# String Comparisons

It is possible to generate `StringComparer` types for your Value Objects that wrap strings.

This is done by specifying the `stringComparers` parameter in either local or global config:

```c#
[ValueObject<string>(stringComparers: StringComparersGeneration.Generate)]
public partial class MyVo
{
}
```

This parameter is an enum with options `Omit` and `Generate`. It defaults to `Omit`.

If it's set to `Generate`, then it generates a bunch of comparers (`Ordinal`, `IgnoreCase` etc.) which can then be used in `Equals` or in collections, e.g.

```c#
        var left = MyVo.From("abc");
        var right = MyVo.From("AbC");

        var comparer = MyVo.Comparers.OrdinalIgnoreCase;

        left.Equals(right, comparer).Should().BeTrue();
```

... and in a dictionary

```c#
        Dictionary<MyVo, int> d = new(MyVo.Comparers.OrdinalIgnoreCase);

        MyVo key1Lower = MyVo.From("abc");
        MyVo key2Mixed = MyVo.From("AbC");

        d.Add(key1Lower, 1);
        d.Should().ContainKey(key2Mixed);
```

Also generated is an `Equals` method that takes an `IEqualityComparer<>`:

```c#
public bool Equals(MyVo other, IEqualityComparer<MyVo> comparer)
{
    return comparer.Equals(this, other);
}

```





As with strings, the Value Object itself doesn't change. `GetHashCode` is different for two objects with different strings if you don't specify a comparer.

```c#
MyString s1 = MyString.From("abc");
MyString s2 = MyString.From("ABC");

// different
s1.GetHashCode().Should.NotBe(s2.GetHashCode());

// same
s1.GetHashCode(StringComparison.OrdinalIgnoreCode).Should.Be(s2.GetHashCode(StringComparison.OrdinalIgnoreCode));
```

For storing in a dictionary, you can ask for an equality comparer, e.g.

`Dictionary<MyString, int> d = new(MyString.EqualityComparerFor(StringComparison.OrdinalIgnoreCase))`


