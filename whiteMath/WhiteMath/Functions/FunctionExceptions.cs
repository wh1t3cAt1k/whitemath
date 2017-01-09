using System;

namespace WhiteMath
{
    [Serializable]
    public class FunctionException : Exception
    { 
		public FunctionException(string message) 
			: base(message) 
		{ } 
	}

    public class FunctionActionSyntaxException : FunctionException
    {
        public FunctionActionSyntaxException(string message) 
			: base(message) 
		{ }

        public override string Message 
			=> "Action syntax error: " + base.Message;
    }

    public class FunctionStringSyntaxException : FunctionException
    {
        public FunctionStringSyntaxException(string message) 
			: base(message) 
		{ }

        public override string Message 
			=> "Function string syntax error: " + base.Message;
    }

    public class FunctionActionExecutionException : FunctionException
    {
		private int _actionIndex;

		public FunctionActionExecutionException(string message, int actionIndex)
            : base(message)
        { 
			this._actionIndex = actionIndex; 
		}

        public override string Message 
			=> "Error occured while doing action #" + _actionIndex + ": " + base.Message;
    }

    public class FunctionBadArgumentException : FunctionException
    {
        public FunctionBadArgumentException(string message)
            : base(message) 
		{ }

        public override string Message 
			=> "Function called contains bad argument: " + base.Message;
    }

    class FunctionActionUserThrownException : FunctionException
    {
        public FunctionActionUserThrownException(string message) 
			: base(message) 
		{ }

        public override string Message 
			=> "Impossible to calculate the function value: " + base.Message;
    }
}
