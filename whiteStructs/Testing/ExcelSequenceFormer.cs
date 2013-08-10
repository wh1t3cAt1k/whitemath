using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IEX = Microsoft.Office.Interop.Excel;
using System.Diagnostics.Contracts;

namespace whiteStructs.Testing
{
    /// <summary>
    /// This class enables to form sequences of values 
    /// in Microsoft Excel worksheets automatically.
    /// </summary>
    public class ExcelSequenceFormer
    {
        /// <summary>
        /// Represents a collection of column names related to 
        /// an <c>ExcelSequenceFormer</c> object.
        /// </summary>
        public class ColumnNameCollection
        {
            private ExcelSequenceFormer former;
            
            /// <summary>
            /// Gets or sets the name of the excel column.
            /// </summary>
            /// <param name="index">The zero-based index of the desired column.</param>
            /// <returns>The name of the column at the specified index.</returns>
            public string this[int index]
            {
                get 
                {
                    if (index < 0 || index >= former.ColumnCount)
                        throw new ArgumentOutOfRangeException("The index of the column is zero-based and should be within the range [0; columnCount).");

                    return former.ws.Cells[1, index + 1].Value2; 
                }

                set 
                {
                    if (index < 0 || index >= former.ColumnCount)
                        throw new ArgumentOutOfRangeException("The index of the column is zero-based and should be within the range [0; columnCount).");

                    if (value == null)
                        throw new ArgumentNullException("value");

                    former.ws.Cells[1, index + 1].Value2 = value; 
                } 
            }

            /// <summary>
            /// Initializes a new instance of <c>ColumnNameCollection</c>
            /// and associates it with an <c>ExcelSequenceFormer</c>.
            /// </summary>
            /// <param name="former">An <c>ExcelSequenceFormer</c> object to associate with.</param>
            internal ColumnNameCollection(ExcelSequenceFormer former)
            {
                this.former = former;
            }
        }
        
        int rowsToFlush = 0;
        int currentIndex = 2;   // текущая строчка записи

        private IEX.Worksheet           ws          = null;
        private Queue<object[]>         rows        = new Queue<object[]>();

        /// <summary>
        /// Gets the Microsoft Excel worksheet associated with the 
        /// current <see cref="ExcelSequenceFormer"/>.
        /// </summary>
        public IEX.Worksheet WorkSheet { get { return ws; } }

        /// <summary>
        /// Gets or sets the amount of columns
        /// in the worksheet.
        /// </summary>
        public int ColumnCount { get; private set; }

        /// <summary>
        /// Returns the collection of column names associated
        /// with the current <c>ExcelSequenceFormer</c>.
        /// </summary>
        public ColumnNameCollection ColumnNames { get; private set; } 

        /// <summary>
        /// Initializes the <c>ExcelSequenceFormer</c> object 
        /// with a newly created, visible <c>Excel</c> application instance.
        /// </summary>
        public ExcelSequenceFormer(int columns)
        {
            this.ColumnCount = columns;
            this.ColumnNames = new ColumnNameCollection(this); 

            IEX.Application app = new IEX.Application();
            app.Visible = true;

            app.Workbooks.Add();
            IEX.Worksheet ws = app.ActiveSheet;
            this.ws = ws;
        }

        /// <summary>
        /// Initializes the <c>ExcelSequenceFormer</c>
        /// with an existing Excel worksheet object.
        /// </summary>
        /// <param name="sheet">An empty worksheet to be dealt with.</param>
        public ExcelSequenceFormer(IEX.Worksheet sheet, int columns)
        {
            Contract.Requires<ArgumentNullException>(sheet != null, "sheet");

            this.ColumnCount = columns;
            this.ws = sheet;
        }

        /// <summary>
        /// Можно ли сохранить в Excel без приведения к строчке.
        /// </summary>
        private static bool excelStorable<T>(T obj)
        {

            return (
                obj is string   ||
                obj is char     ||
                obj is long     ||
                obj is int      ||
                obj is short    ||
                obj is byte     ||
                obj is double   ||
                obj is float    ||
                obj is decimal  ||
                obj is bool     ||
                obj is ulong    ||
                obj is uint     ||
                obj is ushort);
        }

        /// <summary>
        /// Assigns the column names for the <c>ExcelSequenceFormer</c>
        /// from the names array.
        /// </summary>
        /// <param name="values">An array of length equal to the current object's <c>ColumnCount</c>, containing non-<c>null</c> names for the columns.</param>
        public void SetColumnNames(params string[] values)
        {
            Contract.Requires<ArgumentNullException>(values != null, "values");
            Contract.Requires<ArgumentException>(values.Length == this.ColumnCount, "The amount of column names should be equal to the amount of columns.");

            for (int i = 0; i < values.Length; i++)
                this.ColumnNames[i] = values[i];
        }

        /// <summary>
        /// Stores a row of arbitrary values in the internal queue.
        /// </summary>
        /// <param name="values">A parameter array of length equal to the current object's <c>ColumnCount</c>, containing arbitrary values.</param>
        public void AddRow(params object[] values)
        {
            Contract.Requires<ArgumentNullException>(values != null, "values");
            Contract.Requires<ArgumentException>(values.Length == ColumnCount);

            AddRow<object>(values);
        }

        /// <summary>
        /// Stores a row of identically-typed values in the internal queue.
        /// </summary>
        /// <typeparam name="T">The type of values in the row.</typeparam>
        /// <param name="values">A sequence of length equal to the current object's <c>ColumnCount</c>, containing values of type <typeparamref name="T"/>.</param>
        public void AddRow<T>(IEnumerable<T> values)
        {
            Contract.Requires<ArgumentNullException>(values != null, "values");
            Contract.Requires<ArgumentException>(values.Count() == ColumnCount);

            ++rowsToFlush;

            object[] row = new object[values.Count()];
            int indexInRow = 0;

            foreach (T obj in values)
            {
                if (excelStorable(obj))
                    row[indexInRow++] = obj;
                else
                    row[indexInRow++] = obj.ToString();
            }

            rows.Enqueue(row);
        }

        /// <summary>
        /// Flushes a single row into the worksheet.
        /// </summary>
        /// <returns>True if there were any rows to flush, false otherwise.</returns>
        public bool FlushRow()
        {
            if (rowsToFlush == 0)
                return false;

            object[] row = rows.Dequeue();
            object[,] fake = new object[1, row.Length];

            for (int i = 0; i < row.Length; i++)
                fake[0, i] = row[i];

            IEX.Range rowRange = ws.Range[ws.Cells[currentIndex, 1], ws.Cells[currentIndex, ColumnCount]];
            rowRange.Value2 = fake;

            --rowsToFlush;
            ++currentIndex;

            return true;
        }

        /// <summary>
        /// Flushes all the available rows in the <c>ExcelSequenceFormer</c> into
        /// the Excel worksheet.
        /// </summary>
        /// <returns>The total amount of rows flushed into the worksheet.</returns>
        public int Flush()
        {
            object[,] columnList = new object[rowsToFlush, ColumnCount];

            int rowIndex = 0;

            foreach (object[] row in rows)
            {
                for (int j = 0; j < ColumnCount; j++)
                    columnList[rowIndex, j] = row[j];

                ++rowIndex;
            }

            IEX.Range range = ws.Range[ws.Cells[currentIndex, 1], ws.Cells[currentIndex + rowsToFlush - 1, ColumnCount]];
            range.Value2 = columnList;

            currentIndex += rowsToFlush;

            int flushedRows = rowsToFlush;

            rowsToFlush = 0;
            rows.Clear();

            return flushedRows;
        }
    }
}
