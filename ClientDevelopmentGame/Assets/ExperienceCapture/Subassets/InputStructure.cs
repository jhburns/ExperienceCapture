namespace Capture.Internal.InputStructure
{
    using System;

    /// <summary>
    /// Structure for the Limit Output list.
    /// </summary>
    public class SpecificPair
    {
        /// <summary>
        /// How to identify and value.
        /// </summary>
        public string key {  get; private set; }
        
        /// <summary>
        /// A value.
        /// </summary>
        public string value { get; private set; }

        /// <summary>
        /// Construct a SpecificPair
        /// </summary>
        /// <param name="k">A key.</param>
        /// <param name="v">A value.</param>
        public SpecificPair(string k, string v)
        {
            this.key = k;
            this.value = v;
        }
    }

    /// <summary>
    /// Should be used whenever a pair fails parsing.
    /// </summary>
    public class SpecificPairsParsingException : Exception
    {
        public SpecificPairsParsingException()
        {
        }

        public SpecificPairsParsingException(string message)
            : base(message)
        {
        }

        public SpecificPairsParsingException(string message, string varName)
            : base(string.Format("{0}: Failed to parse \'{1}\'", message, varName))
        {
        }

        public SpecificPairsParsingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}