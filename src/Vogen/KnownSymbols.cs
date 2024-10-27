using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable ReplaceAutoPropertyWithComputedProperty

namespace Vogen;

/// <summary>
/// Provides a caching layer for common known symbols wrapping a <see cref="Compilation"/> instance.
/// </summary>
/// <param name="compilation">The compilation from which information is being queried.</param>
public class KnownSymbols(Compilation compilation)
{
    public Compilation Compilation { get; } = compilation;

    public IAssemblySymbol CoreLibAssembly => _CoreLibAssembly ??= Compilation.GetSpecialType(SpecialType.System_Int32).ContainingAssembly;
    private IAssemblySymbol? _CoreLibAssembly;

    public INamedTypeSymbol? MemberInfoType => GetOrResolveType("System.Reflection.MemberInfo", ref _MemberInfoType);
    private Option<INamedTypeSymbol?> _MemberInfoType;

    public INamedTypeSymbol? IParsableOfT => GetOrResolveType("System.IParsable`1", ref _IParsableOfT);
    private Option<INamedTypeSymbol?> _IParsableOfT;

    public INamedTypeSymbol? IFormattable => GetOrResolveType("System.IFormattable", ref _IFormattable);
    private Option<INamedTypeSymbol?> _IFormattable;

    public INamedTypeSymbol? ISpanFormattable => GetOrResolveType("System.ISpanFormattable", ref _ISpanFormattable);
    private Option<INamedTypeSymbol?> _ISpanFormattable;

    public INamedTypeSymbol? IUtf8SpanFormattable => GetOrResolveType("System.IUtf8SpanFormattable", ref _IUtf8SpanFormattable);
    private Option<INamedTypeSymbol?> _IUtf8SpanFormattable;

    public INamedTypeSymbol? ISpanParsableOfT => GetOrResolveType("System.ISpanParsable`1", ref _ISpanParsableOfT);
    private Option<INamedTypeSymbol?> _ISpanParsableOfT;

    public INamedTypeSymbol? IUtf8SpanParsableOfT => GetOrResolveType("System.IUtf8SpanParsable`1", ref _IUtf8SpanParsableOfT);
    private Option<INamedTypeSymbol?> _IUtf8SpanParsableOfT;

    public INamedTypeSymbol? IFormatProvider => GetOrResolveType("System.IFormatProvider", ref _IFormatProvider);
    private Option<INamedTypeSymbol?> _IFormatProvider;

    public INamedTypeSymbol? IReadOnlyDictionaryOfTKeyTValue => GetOrResolveType("System.Collections.Generic.IReadOnlyDictionary`2", ref _IReadOnlyDictionaryOfTKeyTValue);
    private Option<INamedTypeSymbol?> _IReadOnlyDictionaryOfTKeyTValue;

    public INamedTypeSymbol? IDictionaryOfTKeyTValue => GetOrResolveType("System.Collections.Generic.IDictionary`2", ref _IDictionaryOfTKeyTValue);
    private Option<INamedTypeSymbol?> _IDictionaryOfTKeyTValue;

    public INamedTypeSymbol? IDictionary => GetOrResolveType("System.Collections.IDictionary", ref _IDictionary);
    private Option<INamedTypeSymbol?> _IDictionary;

    public INamedTypeSymbol IEnumerableOfT => _IEnumerableOfT ??= Compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T);
    private INamedTypeSymbol? _IEnumerableOfT;

    public INamedTypeSymbol Int32 => _Int32 ??= Compilation.GetSpecialType(SpecialType.System_Int32);
    private INamedTypeSymbol? _Int32;

    public INamedTypeSymbol IEnumerable => _IEnumerable ??= Compilation.GetSpecialType(SpecialType.System_Collections_IEnumerable);
    private INamedTypeSymbol? _IEnumerable;

    public INamedTypeSymbol? SpanOfT => GetOrResolveType("System.Span`1", ref _SpanOfT);
    private Option<INamedTypeSymbol?> _SpanOfT;

    public INamedTypeSymbol? ReadOnlySpanOfT => GetOrResolveType("System.ReadOnlySpan`1", ref _ReadOnlySpanOfT);
    private Option<INamedTypeSymbol?> _ReadOnlySpanOfT;

    public INamedTypeSymbol? MemoryOfT => GetOrResolveType("System.Memory`1", ref _MemoryOfT);
    private Option<INamedTypeSymbol?> _MemoryOfT;

    public INamedTypeSymbol? ReadOnlyMemoryOfT => GetOrResolveType("System.ReadOnlyMemory`1", ref _ReadOnlyMemoryOfT);
    private Option<INamedTypeSymbol?> _ReadOnlyMemoryOfT;

    public INamedTypeSymbol? ListOfT => GetOrResolveType("System.Collections.Generic.List`1", ref _ListOfT);
    private Option<INamedTypeSymbol?> _ListOfT;

    public INamedTypeSymbol? HashSetOfT => GetOrResolveType("System.Collections.Generic.HashSet`1", ref _HashSetOfT);
    private Option<INamedTypeSymbol?> _HashSetOfT;

    public INamedTypeSymbol? KeyValuePairOfKV => GetOrResolveType("System.Collections.Generic.KeyValuePair`2", ref _KeyValuePairOfKV);
    private Option<INamedTypeSymbol?> _KeyValuePairOfKV;

    public INamedTypeSymbol? DictionaryOfTKeyTValue => GetOrResolveType("System.Collections.Generic.Dictionary`2", ref _DictionaryOfTKeyTValue);
    private Option<INamedTypeSymbol?> _DictionaryOfTKeyTValue;

    public INamedTypeSymbol? IList => GetOrResolveType("System.Collections.IList", ref _IList);
    private Option<INamedTypeSymbol?> _IList;

    public INamedTypeSymbol? ImmutableArray => GetOrResolveType("System.Collections.Immutable.ImmutableArray`1", ref _ImmutableArray);
    private Option<INamedTypeSymbol?> _ImmutableArray;

    public INamedTypeSymbol? ImmutableList => GetOrResolveType("System.Collections.Immutable.ImmutableList`1", ref _ImmutableList);
    private Option<INamedTypeSymbol?> _ImmutableList;

    public INamedTypeSymbol? ImmutableQueue => GetOrResolveType("System.Collections.Immutable.ImmutableQueue`1", ref _ImmutableQueue);
    private Option<INamedTypeSymbol?> _ImmutableQueue;

    public INamedTypeSymbol? ImmutableStack => GetOrResolveType("System.Collections.Immutable.ImmutableStack`1", ref _ImmutableStack);
    private Option<INamedTypeSymbol?> _ImmutableStack;

    public INamedTypeSymbol? ImmutableHashSet => GetOrResolveType("System.Collections.Immutable.ImmutableHashSet`1", ref _ImmutableHashSet);
    private Option<INamedTypeSymbol?> _ImmutableHashSet;

    public INamedTypeSymbol? ImmutableSortedSet => GetOrResolveType("System.Collections.Immutable.ImmutableSortedSet`1", ref _ImmutableSortedSet);
    private Option<INamedTypeSymbol?> _ImmutableSortedSet;

    public INamedTypeSymbol? ImmutableDictionary => GetOrResolveType("System.Collections.Immutable.ImmutableDictionary`2", ref _ImmutableDictionary);
    private Option<INamedTypeSymbol?> _ImmutableDictionary;

    public INamedTypeSymbol? ImmutableSortedDictionary => GetOrResolveType("System.Collections.Immutable.ImmutableSortedDictionary`2", ref _ImmutableSortedDictionary);
    private Option<INamedTypeSymbol?> _ImmutableSortedDictionary;
    
    public INamedTypeSymbol? FSharpList => GetOrResolveType("Microsoft.FSharp.Collections.FSharpList`1", ref _FSharpList);
    private Option<INamedTypeSymbol?> _FSharpList;
    
    public INamedTypeSymbol? FSharpMap => GetOrResolveType("Microsoft.FSharp.Collections.FSharpMap`2", ref _FSharpMap);
    private Option<INamedTypeSymbol?> _FSharpMap;


    /// <summary>
    /// A "simple type" in this context defines a type that is either 
    /// a primitive, string or represents an irreducible value such as Guid or DateTime.
    /// </summary>
    public bool IsSimpleType(ITypeSymbol type)
    {
        // ReSharper disable once ConvertSwitchStatementToSwitchExpression
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (type.SpecialType)
        {
            // Primitive types
            case SpecialType.System_Boolean:
            case SpecialType.System_Char:
            case SpecialType.System_SByte:
            case SpecialType.System_Byte:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_Int32:
            case SpecialType.System_UInt32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt64:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            // CoreLib non-primitives that represent a single value.
            case SpecialType.System_String:
            case SpecialType.System_Decimal:
            case SpecialType.System_DateTime:
                return true;
        }

        return (_simpleTypes ??= CreateSimpleTypes(Compilation)).Contains(type);

        static HashSet<ITypeSymbol> CreateSimpleTypes(Compilation compilation)
        {
            ReadOnlySpan<string> simpleTypeNames =
            [
                "System.Half",
                "System.Int128",
                "System.UInt128",
                "System.Guid",
                "System.DateTimeOffset",
                "System.DateOnly",
                "System.TimeSpan",
                "System.TimeOnly",
                "System.Version",
                "System.Uri",
                "System.Text.Rune",
                "System.Numerics.BigInteger",
            ];

            var simpleTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
            foreach (string simpleTypeName in simpleTypeNames)
            {
                INamedTypeSymbol? simpleType = compilation.GetTypeByMetadataName(simpleTypeName);
                if (simpleType is not null)
                {
                    simpleTypes.Add(simpleType);
                }
            }

            return simpleTypes;
        }
    }

    private HashSet<ITypeSymbol>? _simpleTypes;

    /// <summary>
    /// Get or resolve a type by its fully qualified name.
    /// </summary>
    /// <param name="fullyQualifiedName">The fully qualified name of the type to resolve.</param>
    /// <param name="field">A field in which to cache the result for future use.</param>
    /// <returns>The type symbol result or null if not found.</returns>
    protected INamedTypeSymbol? GetOrResolveType(string fullyQualifiedName, ref Option<INamedTypeSymbol?> field)
    {
        if (field.HasValue)
        {
            return field.Value;
        }

        INamedTypeSymbol? type = Compilation.GetTypeByMetadataName(fullyQualifiedName);
        field = new(type);
        return type;
    }

    /// <summary>
    /// Defines a true optional type that supports Some(null) representations.
    /// </summary>
    /// <typeparam name="T">The optional value contained.</typeparam>
    protected readonly struct Option<T>(T value)
    {
        public bool HasValue { get; } = true;
        public T Value { get; } = value;
    }
}
