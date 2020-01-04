namespace Carter.App.Lib.CustomExceptions
{
    using System;

    public class EnviromentVarNotSet : Exception
    {
        public EnviromentVarNotSet()
        {
        }

        public EnviromentVarNotSet(string message)
            : base(message)
        {
        }

        public EnviromentVarNotSet(string message, string varName)
            : base(string.Format("{0}: environment variable {1}", message, varName))
        {
        }

        public EnviromentVarNotSet(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}