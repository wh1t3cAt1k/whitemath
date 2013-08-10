using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using whiteMath.Graphers;
using whiteMath.Functions;

namespace whiteMath.General
{
    /// <summary>
    /// Class providing the extension methods used by graphers.
    /// </summary>
    public static class GrapherExtensions
    {
        /// <summary>
        /// Creates a piece-linear function from the grapher points array
        /// and then a FunctionGrapher object.
        /// 
        /// This trick allows the grapher to behave more smoothly during scaling,
        /// no points usually are 'lost'.
        /// </summary>
        /// <returns></returns>
        public static FunctionGrapher CreateSmoothPieceLinearGrapher(this StandardGrapher obj)
        {
            IFunction<double, double> function = Interpolation<double, CalcDouble>.CreatePieceLinearFunction(obj.GetPointsArray(), double.NaN);
            return new FunctionGrapher(function);
        }
    }
}
