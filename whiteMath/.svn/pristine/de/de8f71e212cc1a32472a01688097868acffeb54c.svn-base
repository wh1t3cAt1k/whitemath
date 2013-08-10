using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

using whiteMath.General;

// предоставляет интерфейс-наследник IComparer для сортировки считанного массива точек.

namespace whiteMath.Graphers.Services
{
    /// <summary>
    /// Compares the two points on the basis of their X coordinate.
    /// </summary>
    /// <typeparam name="T">The type of point coordinates.</typeparam>
    internal class DotListComparer<T, C>: Comparer<Point<T>> where C: ICalc<T>, new()
    {
        private static C calc = new C();

        public override int Compare(Point<T> a, Point<T> b)
        {
            if (calc.mor(a[0], b[0])) return 1;
            else if (calc.mor(b[0], a[0])) return -1;
            else return 0;
        }
    }

    public static class GrapherArrayWork
    {
        public static double Max(double[] arr)
        {
            if (arr==null || arr.Length == 0) return 0;
            
            double Max=arr[0];
            
            foreach (double a in arr)
                if (a > Max) Max = a;

            return Max;
        }
        
        internal static void DoublePointSort(IList<double>[] pointsArray)
        {
            for (int i = 0; i < pointsArray.Length; i++)
                if (pointsArray[i].Count != 2) throw new Exception("Метод предназначен только для сортировки массива точек, длина каждого Double[] элемента должна быть равна двум (2 координаты).");

            DotListComparer<double, CalcDouble> comp = new DotListComparer<double, CalcDouble>(); 
            Array.Sort(pointsArray, comp);      
        }

        /// <summary>
        /// Checks for infinities which may occur while converting from double to float.
        /// </summary>
        /// <param name="pointsArray"></param>
        /// <returns></returns>
        internal static double CheckForCastInfinities(IList<Point<double>> pointsArray) // метод возвращает число, на которое нужно поделить все, чтобы уложить в диапазон float
        {
            double divider = 1;
            for(int i=0; i<pointsArray.Count; i++)
            {
                if (!double.IsInfinity(pointsArray[i][0]) && pointsArray[i][0] < float.MinValue)
                {
                    double tmp = pointsArray[i][0] / float.MinValue;
                    if (tmp > divider) divider = tmp;
                }
                if (!double.IsInfinity(pointsArray[i][0]) && pointsArray[i][0] > float.MaxValue)
                {
                    double tmp = pointsArray[i][0] / float.MaxValue;
                    if (tmp > divider) divider = tmp;
                }
                if (!double.IsInfinity(pointsArray[i][1]) && pointsArray[i][1] < float.MinValue)
                {
                    double tmp = pointsArray[i][1] / float.MinValue;
                    if (tmp > divider) divider = tmp;
                }
                if (!double.IsInfinity(pointsArray[i][1]) && pointsArray[i][1] > float.MaxValue)
                {
                    double tmp = pointsArray[i][1] / float.MaxValue;
                    if (tmp > divider) divider = tmp;
                }
            }

            return divider;
        }
    }

}
