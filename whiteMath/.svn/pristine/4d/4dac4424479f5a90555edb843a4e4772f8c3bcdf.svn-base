using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath.Graphers
{
    // ----------- EXCEPTIONS для графера

    [Serializable]
    public class GrapherException : Exception
        { public GrapherException(string message) : base(message) { } }

    /// <summary>
    /// This type of exception is thrown when the graphing operation has not been completed successfully.
    /// </summary>
    public class GrapherGraphException: GrapherException
    {
        public GrapherGraphException(string message) : base(message) { }

        public override string Message
        {
            get
            {
                return "Graphing error. "+base.Message;
            }
        }
    }

    public class GrapherActionImpossibleException : GrapherException
    {
        public GrapherActionImpossibleException(string message) : base(message) { }

        public override string Message
        {
            get
            {
                return base.Message+" Any further actions are impossible.";
            }
        }
    }

    public class GrapherSettingsException : GrapherException
    {
        public GrapherSettingsException(string message) : base(message) { }

        public override string Message
        {
            get
            {
                return "An error occured while setting up the grapher. "+base.Message;
            }
        }
    }

}
