using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteMath
{
    /// <summary>
    /// Static class used for runtime assertions.
    /// </summary>
    public static class Assertions
    {
        /// <summary>
        /// Performs an assertion of boolean value.
        /// Of it is false, throws the exception specified.
        /// </summary>
        /// <param name="value">The statement to be asserted.</param>
        /// <param name="ex">The exception to be thrown if the statement is false.</param>
        public static void Assert(this bool value, Exception ex)
        {
            if (!value)
                throw ex;
        }
    }
}
