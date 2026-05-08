using System;

namespace Vogen;

/// <summary>
/// Marks a field or property as potentially uninitialized.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class MayBeUninitializedAttribute : Attribute;