namespace Jwc
{
    using System;

    /// <summary>
    /// The config of Jwc.
    /// </summary>
    internal class Config
    {
        /// <summary>
        /// Gets the daytime of the first Monday of the term.
        /// </summary>
        public static DateTime TermStart { get; } = new (2022, 2, 28);
    }
}
