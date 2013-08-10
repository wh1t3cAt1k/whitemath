using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath.Graphers
{
    [Serializable]
    public class GrapherAddMultiplyConverter
    {
        public double Axis1Addition { get; set; }
        public double Axis2Addition { get; set; }

        public double Axis1Coefficient { get; set; }
        public double Axis2Coefficient { get; set; }

        /// <summary>
        /// Allows automatic conversion of the points array by formula: 
        /// 
        /// xNew = x*k1 + s1; 
        /// yNew = y*k2 + s2; 
        /// 
        /// </summary>
        /// <param name="k1">Axis X multiply coefficient</param>
        /// <param name="s1">Axis X addition coefficient</param>
        /// <param name="k2">Axis Y multiply coefficient</param>
        /// <param name="s2">Axis Y addition coefficient</param>
        public GrapherAddMultiplyConverter(double k1, double s1, double k2, double s2)
        {
            Axis1Coefficient = k1;
            Axis1Addition = s1;
            Axis2Coefficient = k2;
            Axis2Addition = s2;
        }

        /// <summary>
        /// Makes a new array by formula xNew = xOld * k1 + s1;
        /// </summary>
        /// <param name="xArray">A source array of X</param>
        /// <returns>New array!</returns>
        public double[] convertArrayOfX(double[] xArray)
        {
            double[] temp = new double[xArray.Length];

            for (int i = 0; i < xArray.Length; i++)
                temp[i] = xArray[i] * Axis1Coefficient + Axis1Addition;

            return temp;
        }

        /// <summary>
        /// Restores the former array by formula xOld = (xNew - s1)/k1
        /// </summary>
        /// <param name="modifiedArray"></param>
        /// <returns></returns>
        public double[] deConvertArrayOfX(double[] modifiedArray)
        {
            double[] temp = new double[modifiedArray.Length];

            for (int i = 0; i < modifiedArray.Length; i++)
                temp[i] = (modifiedArray[i] - Axis1Addition) / Axis1Coefficient;

            return temp;
        }

        /// <summary>
        /// Makes a new array by formula yNew = yOld * k2 + s2;
        /// </summary>
        /// <param name="yArray">A source array of Y</param>
        /// <returns>New array!</returns>
        public double[] convertArrayOfY(double[] yArray)
        {
            double[] temp = new double[yArray.Length];

            for (int i = 0; i < yArray.Length; i++)
                temp[i] = yArray[i] * Axis2Coefficient + Axis2Addition;

            return temp;
        }

        /// <summary>
        /// Restores the former array by formula yOld = (yNew - s2)/k2
        /// </summary>
        /// <param name="modifiedArray"></param>
        /// <returns></returns>
        public double[] deConvertArrayOfY(double[] modifiedArray)
        {
            double[] temp = new double[modifiedArray.Length];

            for (int i = 0; i < modifiedArray.Length; i++)
                temp[i] = (modifiedArray[i] - Axis2Addition) / Axis2Coefficient;

            return temp;
        }
    }
}
