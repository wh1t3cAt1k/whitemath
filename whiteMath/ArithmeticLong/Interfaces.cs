using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace whiteMath.ArithmeticLong
{
    /// <summary>
    /// Provides the needed information on long integers numeric base
    /// and serves as the type argument for the LongInt class.
    /// 
    /// No two numbers with different IBase could be used in the same
    /// arithmetic operation unless the user explicitly calls the
    /// baseConvert() method or the appropriate LongInt constructor.
    /// </summary>
    [ContractClass(typeof(___IBaseContract))]
    public interface IBase
    {
        /// <summary>
        /// Returns the value of the digits numeric base.
        /// </summary>
        /// <returns></returns>
        int getBase();
    }
    
    /// <summary>
    /// Provides the needed information on numeric precision and serves as the 
    /// type argument for the LongExp class.
    /// 
    /// No two numbers with different IPrecision could be used in the same
    /// arithmetic operation unless the user explicitly calls the 
    /// precisionConvert() method for the number (precision may be extended or lost depending
    /// on the calculator to convert to).
    /// </summary>
    [ContractClass(typeof(___IPrecisionContract))]
    public interface IPrecision: IBase
    {
        /// <summary>
        /// Returns the precision, in decimal signs, for the LongExp mantiss.
        /// </summary>
        /// <returns></returns>
        int getPrecision();
    }

    // ---------------------------------------------------------------------------
    // ------------------------------- CONTRACTS ---------------------------------

    [ContractClassFor(typeof(IBase))]
    abstract class ___IBaseContract: IBase
    {
        int IBase.getBase()
        {
            Contract.Ensures(Contract.Result<int>() > 1, "The base of number digits should be positive and more than 1.");
            return 0;
        }
    }

    [ContractClassFor(typeof(IPrecision))]
    abstract class ___IPrecisionContract : IPrecision
    {
        int IBase.getBase()
        {
            return 0;
        }

        int IPrecision.getPrecision()
        {
            Contract.Ensures(Contract.Result<int>() > 0, "The precision of numbers specified by an IPrecision interface implementation should be positive.");
            return 0;
        }
    }
}
