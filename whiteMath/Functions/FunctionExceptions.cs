using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath
{
    // ------------- EXCEPTIONS для функций

    [Serializable]
    public class FunctionException : Exception
    { public FunctionException(string message) : base(message) { } }

    public class FunctionActionSyntaxException : FunctionException
    {
        public FunctionActionSyntaxException(string message) : base(message) { }

        public override string Message
        { get { return "Action syntax error: " + base.Message; } }
    }

    public class FunctionStringSyntaxException : FunctionException
    {
        public FunctionStringSyntaxException(string message) : base(message) { }

        public override string Message
        { get { return "Function string syntax error: " + base.Message; } }
    }

    public class FunctionActionExecutionException : FunctionException
    {
        private int actionNum;

        public FunctionActionExecutionException(string message, int actionNum)
            : base(message)
        { this.actionNum = actionNum; }

        public override string Message
        { get { return "Error occured while doing action №" + actionNum + ": " + base.Message; } }
    }

    public class FunctionBadArgumentException : FunctionException
    {
        public FunctionBadArgumentException(string message)
            : base(message) { }

        public override string Message
        { get { return "Function called contains bad argument: "+base.Message; } }
    }

    class FunctionActionUserThrownException : FunctionException
    {
        public FunctionActionUserThrownException(string message) : base(message) { }

        public override string Message
        { get { return "Impossible to calculate the function value: " + base.Message; } }
    }
}
