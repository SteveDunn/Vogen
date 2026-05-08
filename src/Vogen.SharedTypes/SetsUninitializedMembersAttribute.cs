using System;

namespace Vogen;

/// <summary>
/// Marks a constructor that sets all uninitialized members.
/// This is equivalent to C# "SetsRequiredMembersAttribute"
/// </summary>
[AttributeUsage(AttributeTargets.Constructor)]
public sealed class SetsUninitializedMembersAttribute : Attribute;