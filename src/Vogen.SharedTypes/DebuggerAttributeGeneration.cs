namespace Vogen
{
    /// <summary>
    /// Controls what debugger attributes are generated. Useful if you want to debug in Rider as that doesn't yet show
    /// all of the debugger attributes that Visual Studio can.
    /// </summary>
    public enum DebuggerAttributeGeneration
    {
        /// <summary>
        /// Decided at compile team based on whether or not the <see cref="VogenDefaultsAttribute"/> has this flag set.
        /// </summary>
        Default,

        /// <summary>
        /// Just create the basic debug attributes - Rider friendly as Rider doesn't show some of the
        /// debugger attributes that Visual Studio shows.
        /// </summary>
        Basic ,

        /// <summary>
        /// Creates the full debug attributes, including debugger type proxies. These attributes don't work well in the
        /// Rider debugger, so if you use Rider and want to debug, then use <see cref="Basic"/> which will
        /// just generate ToString.
        /// </summary>
        Full
    }
}