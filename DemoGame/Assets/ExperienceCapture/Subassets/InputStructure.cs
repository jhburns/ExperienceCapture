namespace InputStructure
{
    using System;

    public class SpecificPair
    {
        public string name {  get; private set; }
        public string key { get; private set; }

        public SpecificPair(string n, string k)
        {
            name = n;
            key = k;
        }
    }

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

    public class SpecificPairsNotFoundException : Exception
    {
        public SpecificPairsNotFoundException()
        {
        }

        public SpecificPairsNotFoundException(string message)
            : base(message)
        {
        }

        public SpecificPairsNotFoundException(string message, string objectName, string keyName)
            : base(string.Format("{0}: of {1}.{2}", message, objectName, keyName))
        {
        }

        public SpecificPairsNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}