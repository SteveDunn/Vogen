#nullable disable
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ConsumerTests;

// Taken from the excellent library TypeShape - https://github.com/eiriktsarpalis/typeshape-csharp

[DebuggerDisplay("Length = {Length}")]
[DebuggerTypeProxy(typeof(ImmutableEquatableArray<>.DebugView))]
[CollectionBuilder(typeof(ImmutableEquatableArray), nameof(ImmutableEquatableArray.Create))]
public sealed class ImmutableEquatableArray<T> : 
    IEquatable<ImmutableEquatableArray<T>>, 
    IReadOnlyList<T>, 
    IList<T>, 
    IList

    where T : IEquatable<T>
{
    public static ImmutableEquatableArray<T> Empty { get; } = new([]);

    private readonly T[] _values;
    public ref readonly T this[int index] => ref _values[index];
    public int Length => _values.Length;

    private ImmutableEquatableArray(T[] values)
        => _values = values;

    public bool Equals(ImmutableEquatableArray<T> other)
        => ReferenceEquals(this, other) || ((ReadOnlySpan<T>)_values).SequenceEqual(other._values);

    public override bool Equals(object obj) 
        => obj is ImmutableEquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
        int hash = 0;
        foreach (T value in _values)
        {
            hash = CombineHashCodes(hash, value is null ? 0 : value.GetHashCode());
        }

        return hash;
    }

    public Enumerator GetEnumerator() => new(_values);

    public struct Enumerator
    {
        private readonly T[] _values;
        private int _index;

        internal Enumerator(T[] values)
        {
            _values = values;
            _index = -1;
        }

        public bool MoveNext() => ++_index < _values.Length;
        public readonly ref T Current => ref _values[_index];
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static ImmutableEquatableArray<T> UnsafeCreateFromArray(T[] values)
        => new(values);

    #region Explicit interface implementations
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)_values).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)_values).GetEnumerator();
    bool ICollection<T>.IsReadOnly => true;
    bool IList.IsFixedSize => true;
    bool IList.IsReadOnly => true;
    T IReadOnlyList<T>.this[int index] => _values[index];
    T IList<T>.this[int index] { get => _values[index]; set => throw new InvalidOperationException(); }
    object IList.this[int index] { get => _values[index]; set => throw new InvalidOperationException(); }
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => _values.CopyTo(array, arrayIndex);
    void ICollection.CopyTo(Array array, int index) => _values.CopyTo(array, index);
    int IList<T>.IndexOf(T item) => _values.AsSpan().IndexOf(item);
    int IList.IndexOf(object value) => ((IList)_values).IndexOf(value);
    bool ICollection<T>.Contains(T item) => _values.AsSpan().IndexOf(item) >= 0;
    bool IList.Contains(object value) => ((IList)_values).Contains(value);
    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => this;

    int IReadOnlyCollection<T>.Count => Length;
    int ICollection<T>.Count => Length;
    int ICollection.Count => Length;

    void ICollection<T>.Add(T item) => throw new InvalidOperationException();
    bool ICollection<T>.Remove(T item) => throw new InvalidOperationException();
    void ICollection<T>.Clear() => throw new InvalidOperationException();
    void IList<T>.Insert(int index, T item) => throw new InvalidOperationException();
    void IList<T>.RemoveAt(int index) => throw new InvalidOperationException();
    int IList.Add(object value) => throw new InvalidOperationException();
    void IList.Clear() => throw new InvalidOperationException();
    void IList.Insert(int index, object value) => throw new InvalidOperationException();
    void IList.Remove(object value) => throw new InvalidOperationException();
    void IList.RemoveAt(int index) => throw new InvalidOperationException();
    #endregion

    private sealed class DebugView(ImmutableEquatableArray<T> array)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => array.ToArray();
    }
    
    public static int CombineHashCodes(int h1, int h2)
    {
        // RyuJIT optimizes this to use the ROL instruction
        // Related GitHub pull request: https://github.com/dotnet/coreclr/pull/1830
        uint rol5 = ((uint)h1 << 5) | ((uint)h1 >> 27);
        return ((int)rol5 + h1) ^ h2;
    }
    
}
public static class ImmutableEquatableArray
{
    public static ImmutableEquatableArray<T> ToImmutableEquatableArray<T>(this IEnumerable<T> values) where T : IEquatable<T>
        => values is ICollection<T> { Count: 0 } ? ImmutableEquatableArray<T>.Empty : ImmutableEquatableArray<T>.UnsafeCreateFromArray(values.ToArray());

    public static ImmutableEquatableArray<T> Create<T>(ReadOnlySpan<T> values) where T : IEquatable<T>
        => values.IsEmpty ? ImmutableEquatableArray<T>.Empty : ImmutableEquatableArray<T>.UnsafeCreateFromArray(values.ToArray());
}