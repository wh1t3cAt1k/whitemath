#if DEBUG

using System;
using System.Collections.Generic;
using System.IO;

using WhiteMath.Matrices;

namespace WhiteMath
{
    public static class DebugMethods
    {
        // -----------------------------------
        // -------- For lists ----------------
        // -----------------------------------

		public static IList<string> ElementwiseToString<T>(this IList<T> list)
        {
            string[] arr = new string[list.Count];

            for (int i = 0; i < list.Count; i++)
                arr[i] = list[i].ToString();

            return arr;
        }

        // ----------------------------------------
        // ------------- prints the list ----------
        // ----------------------------------------

		public static void PrintElements<T>(this IList<T> list)
        {
            PrintElements(list, Console.Out);
        }

		public static void PrintElements<T>(this IList<T> list, TextWriter tw)
        {
            PrintElements(list, tw, "{0} ");
        }

		public static void PrintElements<T>(this IList<T> list, TextWriter tw, string formatter)
        {
            IList<string> strings = list.ElementwiseToString();

            foreach (string obj in strings)
                tw.Write(formatter, obj);

            tw.WriteLine();
            tw.Flush();
        }

        // -----------------------------------
        // ------------- for matrices --------
        // -----------------------------------

		public static string[,] Describe(this IMatrix matrix)
        {
            string[,] matString = new string[matrix.RowCount, matrix.ColumnCount];

            for (int i = 0; i < matrix.RowCount; i++)
                for (int j = 0; j < matrix.ColumnCount; j++)
                    matString[i, j] = matrix.GetElementAt(i, j).ToString();

            return matString;
        }

		public static void PrintElements(this IMatrix matrix)
        {
            PrintElements(matrix, Console.Out);
        }

		public static void PrintElements(
			this IMatrix matrix, 
			TextWriter textWriter)
        {
            PrintElements(matrix, textWriter, "{0} ");
        }

		public static void PrintElements(
			this IMatrix matrix, 
			TextWriter textWriter, 
			string formatter)
        {
            PrintElements(matrix, textWriter, formatter, "\n");
        }

		public static void PrintElements(
			this IMatrix matrix, 
			TextWriter textWriter, 
			string formatter, 
			string rowEndString)
        {
			string[,] elementDescriptions = matrix.Describe();

			for (int rowIndex = 0; rowIndex < matrix.RowCount; rowIndex++)
            {
				for (int columnIndex = 0; columnIndex < matrix.ColumnCount; columnIndex++)
				{
                    textWriter.Write(formatter, elementDescriptions[rowIndex, columnIndex]);
				}

                textWriter.Write(rowEndString);
            }

            if (rowEndString[rowEndString.Length - 1] != '\n')
			{
                textWriter.WriteLine();
			}

            return;
        }
    }
}

#endif