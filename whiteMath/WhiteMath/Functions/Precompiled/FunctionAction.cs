using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteMath.Functions
{
    /// <summary>
    /// Represents the precompiled function action.
    /// </summary>
    internal interface IFunctionAction<TypeArg, TypeVal>: IFunction<TypeArg, TypeVal>
    {
    }

    internal class UnaryAction<TypeArg, TypeVal>: IFunctionAction<TypeArg, TypeVal>
    {
        private IFunction<TypeArg, TypeVal> operand;
        private Func<TypeVal, TypeVal> action;

        public UnaryAction(IFunction<TypeArg, TypeVal> operand, Func<TypeVal, TypeVal> action)
        {
            this.operand = operand;
            this.action = action;
        }

        public TypeVal Value(TypeArg x)
        {
            return action.Invoke(operand.Value(x));
        }
    }

    internal class BinaryAction<TypeArg, TypeVal> : IFunctionAction<TypeArg, TypeVal>
    {
        private IFunction<TypeArg, TypeVal> operand1, operand2;
        private Func<TypeVal, TypeVal, TypeVal> action;

        public BinaryAction(IFunction<TypeArg, TypeVal> operand1, IFunction<TypeArg, TypeVal> operand2, Func<TypeVal, TypeVal, TypeVal> action)
        {
            this.operand1 = operand1;
            this.operand2 = operand2;
            
            this.action = action;
        }

        public TypeVal Value(TypeArg x)
        {
            return action.Invoke(operand1.Value(x), operand2.Value(x));
        }
    }

    internal class TernaryAction<TypeArg, TypeVal> : IFunctionAction<TypeArg, TypeVal>
    {
        private IFunction<TypeArg, TypeVal> operand1, operand2, operand3;
        private Func<TypeVal, TypeVal, TypeVal, TypeVal> action;

        public TernaryAction(IFunction<TypeArg, TypeVal> operand1, IFunction<TypeArg, TypeVal> operand2, IFunction<TypeArg, TypeVal> operand3, Func<TypeVal, TypeVal, TypeVal, TypeVal> action)
        {
            this.operand1 = operand1;
            this.operand2 = operand2;
            this.operand3 = operand3;

            this.action = action;
        }

        public TypeVal Value(TypeArg x)
        {
            return action.Invoke(operand1.Value(x), operand2.Value(x), operand3.Value(x));
        }
    }

    // ------------------------------------------

    internal class ConstantReturner<TypeArg, TypeVal> : IFunction<TypeArg, TypeVal>
    {
        TypeVal constant;

        public ConstantReturner(TypeVal val)
        {
            this.constant = val;
        }

        public TypeVal Value(TypeArg x)
        {
            return this.constant;
        }
    }

    internal class FunctionExceptionThrower<TypeArg, TypeVal>: IFunction<TypeArg, TypeVal>
    {
        string message;

        public FunctionExceptionThrower(string message)
        {
            this.message = message;
        }

        public TypeVal Value(TypeArg x)
        {
            throw new FunctionActionUserThrownException(message);
        }
    }

    
}
