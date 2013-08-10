#if DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using whiteMath.Matrices;

namespace whiteMath
{
    public static class DebugMethods
    {
        // -----------------------------------
        // -------- ToStrings the list -------
        // -----------------------------------

        public static IList<string> _DB_StringMe<T>(this IList<T> list)
        {
            string[] arr = new string[list.Count];

            for (int i = 0; i < list.Count; i++)
                arr[i] = list[i].ToString();

            return arr;
        }

        // ----------------------------------------
        // ------------- prints the list ----------
        // ----------------------------------------

        public static void _DB_PrintMe<T>(this IList<T> list)
        {
            _DB_PrintMe(list, Console.Out);
        }

        public static void _DB_PrintMe<T>(this IList<T> list, TextWriter tw)
        {
            _DB_PrintMe(list, tw, "{0} ");
        }

        public static void _DB_PrintMe<T>(this IList<T> list, TextWriter tw, string formatter)
        {
            IList<string> strings = list._DB_StringMe();

            foreach (string obj in strings)
                tw.Write(formatter, obj);

            tw.WriteLine();
            tw.Flush();
        }

        // -----------------------------------
        // ------------- for matrices --------
        // -----------------------------------

        // ------- string me -----------------

        public static string[,] _DB_StringMe(this IMatrix matrix)
        {
            string[,] matString = new string[matrix.RowCount, matrix.ColumnCount];

            for (int i = 0; i < matrix.RowCount; i++)
                for (int j = 0; j < matrix.ColumnCount; j++)
                    matString[i, j] = matrix.GetElementValue(i, j).ToString();

            return matString;
        }

        // ------- print me ------------------

        public static void _DB_PrintMe(this IMatrix matrix)
        {
            _DB_PrintMe(matrix, Console.Out);
        }

        public static void _DB_PrintMe(this IMatrix matrix, TextWriter tw)
        {
            _DB_PrintMe(matrix, tw, "{0} ");
        }

        public static void _DB_PrintMe(this IMatrix matrix, TextWriter tw, string formatter)
        {
            _DB_PrintMe(matrix, tw, formatter, "\n");
        }

        public static void _DB_PrintMe(this IMatrix matrix, TextWriter tw, string formatter, string rowEndString)
        {
            string[,] arr = matrix._DB_StringMe();

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                    tw.Write(formatter, arr[i, j]);

                tw.Write(rowEndString);
            }

            if (rowEndString[rowEndString.Length - 1] != '\n')
                tw.WriteLine();

            return;
        }
    }
}

#endif