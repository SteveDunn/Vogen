using ConsumerTests;
using ServiceStack;

namespace Vogen.Tests.Types;

public class Hash<T> : IEquatable<T> where T : IEquatable<T>
{
    private readonly ImmutableEquatableArray<T> _items;
    
    public Hash(T[] items) => _items = items.ToImmutableEquatableArray();
    
    protected bool Equals(Hash<T> other) => _items.Equals(other._items);

    public bool Equals(T? other) => _items.Equals(other);
    
    public override bool Equals(object? obj) => _items.Equals((obj as Hash<T>)?._items);

    public override int GetHashCode() => _items.GetHashCode();

    public static implicit operator Hash<T>(T[] items) => new(items);
    public static explicit operator T[](Hash<T> h) => h._items.ToArray();
}
