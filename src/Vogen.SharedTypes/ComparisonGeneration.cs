using System.ComponentModel;

namespace Vogen
{
    public enum ComparisonGeneration
    {
        /// <summary>
        /// Use the default, which is <see cref="UseUnderlying"/>. This entry generally isn't used but is present
        /// to signify a default value which has not been specified in the attributes. 
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        Default,

        /// <summary>
        /// Omits the IComparable interface and implementation. Useful for opaque types such as tokens or IDs where comparison
        /// doesn't make sense.
        /// </summary>
        Omit,
        
        /// <summary>
        /// Uses the default IComparable from the underlying type.
        /// </summary>
        UseUnderlying,
    }
}