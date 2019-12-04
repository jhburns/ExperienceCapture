// - Name: Jonathan Hirokazu Burns
// - ID: 2288851
// - email: jburns@chapman.edu
// - Course: 353-01
// - Assignment: Submission #4
// - Purpose: Thrown when an env var that is needed is unset

namespace Deploy.App.CustomExceptions
{
    using System;

    // EnviromentVarNotSet class is an exception
    public class EnviromentVarNotSet : Exception
    {
        // EnviromentVarNotSet
          // Default constructor
        public EnviromentVarNotSet()
        {
        }

        // EnviromentVarNotSet
          // Constructor with message
        public EnviromentVarNotSet(string message)
            : base(message)
        {
        }

        // EnviromentVarNotSet
          // Constructor two messages
        public EnviromentVarNotSet(string message, string varName)
            : base(string.Format("{0}: environment variable {1}", message, varName))
        {
        }

        // EnviromentVarNotSet
          // Constructor with message and inner exception
        public EnviromentVarNotSet(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}