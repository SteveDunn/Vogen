# Overriding Methods

## GetHashCode

If you supply your own `GetHashCode()`, then Vogen won't generate it.

## Equals

You can override `Equals` for the Value Object itself (e.g. `Equals(MyId myId)`, or equals for the underlying primitive, e.g. `Equals(int primitive)`).

All other `Equals` methods are always generated, e.g.

* `Equals(ValueObject, IEqualityComparer)`
* `Equals(primitive, IEqualityComparer)`
* `Equals(stringPrimitive, StringComparer)`
* `Equals(Object)` (structs only)

## ToString
If you supply your own, Vogen won't generate one.