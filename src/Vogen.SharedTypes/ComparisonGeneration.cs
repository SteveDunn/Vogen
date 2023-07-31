namespace Vogen
{
    public enum ComparisonGeneration
    {
        /// <summary>
        /// Use the default 
        /// </summary>
        Default,

        /// <summary>
        /// Omits the IComparable interfaces and implementation. 
        /// </summary>
        Omit,
        
        /// <summary>
        /// Uses the default IComparable from the underlying type.
        /// </summary>
        UseUnderlying,

        /// <summary>
        /// Prefers orginal comparison if a string, but if not a string, uses the <see cref="UseUnderlying"/> comparisons.
        /// </summary>
        PreferOrdinal,
        
        /// <summary>
        /// Prefers orginal ignore case comparison if a string, but if not a string, uses the <see cref="UseUnderlying"/> comparisons.
        /// </summary>
        PreferOrdinalAndIgnoreCase
    }
}