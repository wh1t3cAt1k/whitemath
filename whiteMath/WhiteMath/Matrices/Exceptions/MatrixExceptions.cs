using System;

namespace WhiteMath.Matrices
{
    /// <summary>
    /// The matrix exception signalizing that the matrix being processed is singular and 
    /// thus no futher actions may be performed.
    /// </summary>
    public class MatrixSingularityException: ArgumentException
    {
        public override string Message
        {
            get
            {
                return "The matrix is singular (has zero determinant): "+base.Message;
            }
        }

        public MatrixSingularityException(string msg)
            : base(msg)
        { }
    }

    /// <summary>
    /// The matrix exception signalizing that the matrix being processed has wrong size and 
    /// thus no futher actions may be performed.
    /// </summary>
    public class MatrixSizeException : ArgumentException
    {
        public override string Message
        {
            get
            {
                return "Matrix size is wrong: "+base.Message;
            }
        }

        public MatrixSizeException(string msg)
            : base(msg)
        { }
    }
}
