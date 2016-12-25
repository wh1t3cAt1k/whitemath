using System;

namespace whiteMath.Calculators
{
    /// <summary>
    /// The exception class which signalizes that the operation
    /// is supported only for integer numeric types.
    /// </summary>
	public class NonIntegerTypeException : NotSupportedException
    {
        /// <summary>
        /// Creates a new instance of <see cref="NonIntegerTypeException"/>.
        /// </summary>
        /// <param name="typeName">
		/// The name of the type that does not support the 
		/// operation, resulting in the constructed exception.
		/// </param>
        public NonIntegerTypeException(string typeName)
			: base(string.Format(Messages.OperationNotSupportedForNonIntegralType, typeName))
        { }
    }

    /// <summary>
    /// The exception class which signalizes that the operation
    /// is supported only for fractional numeric types.
    /// </summary>
    public class NonFractionalTypeException : NotSupportedException
    {
        /// <summary>
        /// Creates a new instance of <see cref="NonFractionalTypeException"/>.
        /// </summary>
        /// <param name="typeName">
		/// The name of the type that does not support the operation, 
		/// resulting in the constructed exception.
		/// </param>
		public NonFractionalTypeException(string typeName)
			: base(string.Format(Messages.OperationNotSupportedForNonFractionalType, typeName))
        { }
    }

    /// <summary>
    /// The exception class which signalizes that the operation
    /// is not supported because an object has non-negative numeric type.
    /// </summary>
    public class NonNegativeTypeException : NotSupportedException
    {
        /// <summary>
        /// Creates a new instance of <see cref="NonNegativeTypeException"/>
        /// </summary>
        /// <param name="message">The name of the type that does not support the operation which thus results in an exception.</param>
        public NonNegativeTypeException(string message): base("This operation is not supported for non-negative type " + message)
        { }
    }

    /// <summary>
    /// The interface used for different numeric calculating purposes.
    /// Provides means for <c>Numeric&lt;T,C&gt;</c> generic abstraction layer class to overload
    /// operators, get default values etc.
    /// 
    /// If a calculator for a type is made correctly, one can perform "arithmetic" operations
    /// with <c>Numeric&lt;T,C&gt;</c> objects using operator syntax as if they were primitive number types.
    /// </summary>
    /// <see cref="Numeric&lt;T,C&gt;"/>
    public interface ICalc<T>
    {
        /// <summary>
        /// Returns the sum of two T numbers.
        /// </summary>
        /// <param name="one">The first operand.</param>
        /// <param name="two">The second operand.</param>
        /// <returns>The sum of the operands.</returns>
        T Add(T one, T two);        
        
        /// <summary>
        /// Returns the difference of two numbers.
        /// </summary>
        /// <param name="one">The first operand.</param>
        /// <param name="two">The second operand.</param>
        /// <returns>The difference of the operands.</returns>
        T Subtract(T one, T two);        
        
        /// <summary>
        /// Returns the multiplication product of two numbers.
        /// </summary>
        /// <param name="one">The first operand.</param>
        /// <param name="two">The second operand.</param>
        /// <returns>The multiplication product of the operands.</returns>
        T Multiply(T one, T two);        
        
        /// <summary>
        /// Returns the first number divided by the second number.
        /// </summary>
        /// <param name="one">The first operand.</param>
        /// <param name="two">The second operand.</param>
        /// <returns>The first operand divided by the second number.</returns>
        T Divide(T one, T two);        
        
        /// <summary>
        /// Returns the integral remainder of number division.
        /// </summary>
        /// <exception cref="NonIntegerTypeException">
        /// Calculators for fractional number types MUST
        /// throw a <c>NonIntegerTypeException</c> in the implementation of this method.
        /// </exception>
        /// <param name="one">The first operand.</param>
        /// <param name="two">The second operand.</param>
        /// <returns>The integral remainder of the division.</returns>
        T Modulo(T one, T two);

        /// <summary>
        /// Returns true if the integral number is even, that is,
        /// has no remainder of division by two.
        /// </summary>
        /// <exception cref="NonIntegerTypeException">
        /// This method MUST throw a <c>NonIntegerTypeException</c> if
        /// the <typeparamref name="T"/> type is fractional.
        /// </exception>
        /// <param name="one">The number to test.</param>
        /// <returns>True if the number is even,that is, has no remainder of division by two.</returns>
        bool IsEven(T one);
        
        /// <summary>
        /// Negates the number so that:
        /// <list type="number">
        /// <item><c>a := -a</c></item>
        /// <item><c>a + (-a) = 0.</c></item>
        /// </list>
        /// </summary>
        /// <param name="one">The number to negate.</param>
        /// <returns>The negated number.</returns>
        T Negate(T one);            
        
        /// <summary>
        /// Increments a number so that <c>a := a + fromInt(1)</c>.
        /// This method requires implementation because in many cases the increment 
		/// can be implemented much more effectively than by just adding <c>fromInt(1)</c> 
		/// to the current value.
        /// </summary>
        /// <param name="one">The number to be incremented. Will be modified during the operation.</param>
        /// <returns>Current number incremented by one. For reference types, must return a reference to the same object!</returns>
        T Increment(T one);         

        /// <summary>
        /// Decrements a number so that a := a - <c>fromInt(1)</c>.
        /// This method requires implementation because in many cases 
        /// the decrement operation can be implemented
        /// much more effective than by just substracting <c>fromInt(1)</c> from the current value.
        /// </summary>
        /// <param name="one">The number to be decremented. Will be modified during the operation.</param>
        /// <returns>Current number decremented by one. For reference types, must return a reference to the same object!</returns>
        T Decrement(T one);         

        /// <summary>
        /// Returns the integer part for the number.
        /// Should have the same sign as the number itself.
        /// </summary>
        /// <param name="one">The number whose integer part is to be evaluated.</param>
        /// <returns>The integer part of the number.</returns>
        T IntegerPart(T one);

        /// <summary>
        /// Tests if the first number is greater than the second.
        /// <para>
        /// (mor(one, two)) means that - but not equivalent to - : (!mor(two, one))
        /// </para>
        /// </summary>
        /// <param name="one">The number to be tested if it's bigger than the second.</param>
        /// <param name="two">The number to be tested if it's smaller than the first.</param>
        /// <returns>True if <paramref name="one"/> is bigger than <paramref name="two"/>, false otherwise.</returns>
        bool GreaterThan(T one, T two);     

        /// <summary>
        /// Tests if the first number is equal to the second.
        /// Should be symmetric: a==b must be equivalent to b==a.
        /// </summary>
        /// <param name="one">The first operand.</param>
        /// <param name="two">THe second operand.</param>
        /// <returns>The flag indicating whether the two numbers are equal.</returns>
		bool Equal(T one, T two);     

        /// <summary>
        /// Tests whether the value is not a number (NaN) value.
        /// If the type does not support NaN's, this method should always return false.
        /// </summary>
        /// <param name="one">The value to be tested.</param>
        /// <returns>True if the value is NaN, false otherwise.</returns>
        bool IsNaN(T one);          

        /// <summary>
        /// Tests whether the value is a positive infinity.
        /// If the type does not support infinity values, this method should always return false.
        /// </summary>
        /// <param name="one">The value to be tested.</param>
        /// <returns>True if the value passed is a positive infinity, false otherwise.</returns>
        bool IsPositiveInfinity(T one);       

        /// <summary>
        /// Tests whether the value is a negative infinity.
        /// If the type does not support infinity values, this method should always return false.
        /// </summary>
        /// <param name="one">The value to be tested.</param>
        /// <returns>True if the value passed is a negative infinity, false otherwise.</returns>
        bool IsNegativeInfinity(T one);       

        /// <summary>
        /// Returns an independent copy of the number.
        /// </summary>
        /// <param name="source">The number to be copied.</param>
        /// <returns>An independent copy of the number.</returns>
        T GetCopy(T source);        

        /// <summary>
        /// Creates a number of <typeparamref name="T"/> type 
        /// from an integer value.
        /// </summary>
        /// <param name="equivalent">A long number whose <typeparamref name="T"/> equivalent is to be created.</param>
        /// <returns>
        /// A <typeparamref name="T"/> type equivalent of 
        /// the passed number.
        /// </returns>
        T FromInteger(long equivalent);         

        /// <summary>
        /// Creates a number of <typeparamref name="T"/> type
        /// from a double value.
        /// </summary>
        /// <exception cref="NonFractionalTypeException">
        /// This method MUST throw a <c>NonFractionalTypeException</c> 
        /// for any <typeparamref name="T"/> type that does not support lossless 
        /// conversions from double values, e.g. integer types.
        /// </exception>
        /// <param name="equivalent">A double number whose <typeparamref name="T"/> equivalent is to be created.</param>
        /// <returns>
        /// A <typeparamref name="T"/> type equivalent of
        /// the passed number.
        /// </returns>
        T FromDouble(double equivalent);

        /// <summary>
        /// Gets the zero value of <typeparamref name="T"/> type.
        /// </summary>
        T Zero { get; }

        /// <summary>
        /// Gets the boolean flag which signalizes if the calculator was designed for handling 
        /// integer type operations. This information will be used by WhiteMath's arithmetic functions 
        /// to choose an optimal computation method.
        /// </summary>
        bool IsIntegerCalculator { get; }

        /// <summary>
        /// Creates a number of <typeparamref name="T"/> type
        /// from a string representing the resulting value.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// This kind of exception will be thrown in case when the calculator
        /// does not provide means of converting strings to <typeparamref name="T"/> values.
        /// </exception>
        /// <param name="str">The string to be parsed into a number.</param>
        /// <returns>A <typeparamref name="T"/> value represented by the incoming string object.</returns>
        T Parse(string str);       
    }
}
