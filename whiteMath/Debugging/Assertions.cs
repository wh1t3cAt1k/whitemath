using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath
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

        /// <summary>
        /// Performs an assertion that the value passed
        /// is not null. If it is, throws an ArgumentNullException 
        /// with the message passed.
        /// </summary>
        /// <param name="obj">The object to be tested for null.</param>
        /// <param name="exceptionMessage">The message for the ArgumentNullException.</param>
        public static void Assert_NotNull(this object obj, string exceptionMessage)
        {
            if (obj == null)
                throw new ArgumentNullException(exceptionMessage);
        }
    }
}
