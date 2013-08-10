using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

using whiteMath.Graphers.Services;
using whiteMath.General;

namespace whiteMath.Graphers
{
    public class ArrayGrapher: StandardGrapher
    {
        public ArrayGrapher(IList<Point<double>> PointsArray)
        {
            // No need to sort?
            // GrapherArrayWork.DoublePointSort(PointsArray);
            
            this.PointsArray = PointsArray;
            FindMaximumsAndMinimums();
        }

        public ArrayGrapher(double[] xArray, double[] yArray)
        {
            if (xArray.Length!=yArray.Length)
                throw new Exception("The length of the X coordinate array equals "+
                    xArray.Length+" and is not equal to the length of the Y coordinate array: "
                    +yArray.Length);

            if (xArray.Length == 0) throw new Exception("The length of the X coordinate array equals zero.");
            if (yArray.Length == 0) throw new Exception("The length of the Y coordinate array equals zero.");

            this.PointsArray = new Point<double>[xArray.Length];
            
            for(int i=0; i < xArray.Length; i++)
                this.PointsArray[i] = new Point<double> (xArray[i], yArray[i]);
            
            FindMaximumsAndMinimums();
        }
    }
}
