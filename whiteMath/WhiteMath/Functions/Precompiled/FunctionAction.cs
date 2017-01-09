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

        public TypeVal GetValue(TypeArg x)
        {
            return action.Invoke(operand.GetValue(x));
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

        public TypeVal GetValue(TypeArg x)
        {
            return action.Invoke(operand1.GetValue(x), operand2.GetValue(x));
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

        public TypeVal GetValue(TypeArg x)
        {
            return action.Invoke(operand1.GetValue(x), operand2.GetValue(x), operand3.GetValue(x));
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

        public TypeVal GetValue(TypeArg x)
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

        public TypeVal GetValue(TypeArg x)
        {
            throw new FunctionActionUserThrownException(message);
        }
    }

    
}
