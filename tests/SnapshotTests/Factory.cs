using System.Collections.Generic;
using System.Collections.Immutable;

namespace SnapshotTests
{
    public static class Factory
    {
        public static ImmutableHashSet<string> TypeVariations = new List<string>()
        {
            "partial class",
            "partial record",
            "partial struct",

#if THOROUGH
            "readonly partial struct",
            
            "sealed partial class",

            "partial record struct",
            "readonly partial record struct",
            
            "partial record class",
            "sealed partial record class",

            "sealed partial record"
#endif
        }.ToImmutableHashSet();

        public static readonly ImmutableHashSet<string> UnderlyingTypes = new List<string>()
        {
            "", // don't include underlying type - should default to int
            "int",
            "string",
            "decimal",
#if THOROUGH
            "byte",
            "char",
            "bool",
            "System.DateTimeOffset",
            "System.DateTime",
            "double",
            "float",
            "System.Guid",
            "long",
            "short",
#endif
        }.ToImmutableHashSet();
    };
}
