namespace Jwc
{
    using System;

    /// <summary>
    /// The config of Jwc.
    /// </summary>
    internal class JwcConfig
    {
        /// <summary>
        /// Gets the datetime of the first Monday of the term.
        /// </summary>
        public static DateTime TermStart { get; } = new (2021, 9, 6);
    }
}
