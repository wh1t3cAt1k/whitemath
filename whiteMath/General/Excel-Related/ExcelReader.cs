using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XS = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace whiteMath.General
{
    /// <summary>
    /// Specifies the direction in which the Excel file reading should occur.
    /// </summary>
    public enum ExcelReadDirection
    {
        Left = -3, Right = 3, Up = -1, Down = 1
    }

    //------------------------------------------------

    /// <summary>
    /// Reads arrays of elements from a Microsoft Office Excel (tm) file.
    /// </summary>
    [Serializable]
    public class ExcelArrayReader
    {
        // --------------------------------- THE DLL NEEDED TO KILL THE EXCEL PROCESS

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(int hWnd, ref int lpdwProcessId);

        // --------------------------------------------------------------------------

        private XS.Workbook wb;         // excel workbook
        private XS.Range startCell;     // starting cell
        private XS.Range endCell;       // ending cell

        private ExcelReadDirection direction;   // direction of reading

        public XS.Workbook Connection { get { return wb; } }
        public ExcelArrayReader(XS.Workbook wb) { this.wb = wb; }   // constructor

        /// <summary>
        /// Opens a connection by running Microsoft Excel application and opening the file at the path specified.
        /// Never forget to close the connection after usage.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XS.Workbook openConnection(string path)
        {
            XS.Application app = new XS.Application();
            app.DisplayAlerts = false;

            XS.Workbook wb = app.Workbooks.Open(path);

            return wb;
        }

        // ----------------------
        private ManualResetEventSlim signal;
        // ----------------------

        public bool getStartCellFromApplication(int timeOutMilliseconds = 60000)
        {
            // get worksheet
            XS.Worksheet ws = wb.ActiveSheet as XS.Worksheet;

            // get cell select handler
            XS.DocEvents_ChangeEventHandler handler = new XS.DocEvents_ChangeEventHandler(checkerStart);
            ws.Change += handler;

            // make app visible and wait for signal
            wb.Application.Visible = true;

            signal = new ManualResetEventSlim(false);

            if (!signal.Wait(timeOutMilliseconds))
                return false;

            ws.Change -= handler;
            wb.Application.Visible = false;

            // ------- first cell is set here.

            return true;
        }

        public bool getEndCellFromApplication(int timeOutMilliseconds = 60000)
        {
            // check start cell
            this.checkStartCellIsSet();

            // get worksheet
            XS.Worksheet ws = wb.ActiveSheet as XS.Worksheet;

            // get cell select handler
            XS.DocEvents_ChangeEventHandler handler = new XS.DocEvents_ChangeEventHandler(checkerEnd);
            ws.Change += handler;

            // make app visible and wait for signal
            wb.Application.Visible = true;

            signal = new ManualResetEventSlim(false);

            if (!signal.Wait(timeOutMilliseconds))
                return false;

            ws.Change -= handler;
            wb.Application.Visible = false;

            // ------- second cell is set here
            // ------- now analyze the direction

            this.setDirection();

            return true;
        }

        /// <summary>
        /// -SERVICE- check if the start cell is set
        /// </summary>
        private void checkStartCellIsSet()
        {
            if (this.startCell == null)
                throw new InvalidOperationException("The starting cell should be set before setting the ending cell.");
        }

        /// <summary>
        /// -SERVICE- if both start and end cells are set, we can guess the reading direction
        /// </summary>
        private void setDirection()
        {
            if(startCell==null || endCell==null)
                throw new InvalidOperationException("Both starting and ending cells should be set before determining the direction");

            if (startCell.Row != endCell.Row && startCell.Column != endCell.Column)
                throw new ArgumentException("The starting and the ending cells should be equal in row or column number.");

            int direction = 0;
            direction += Math.Sign(endCell.Column - startCell.Column) * 3;
            direction += Math.Sign(endCell.Row - startCell.Row);

            this.direction = (ExcelReadDirection)direction;

            return;
        }

        /// <summary>
        /// Handler for cell changing during start cell capturing.
        /// </summary>
        /// <param name="range"></param>
        void checkerStart(XS.Range range)
        {
            if (range.Count != 1)
                return;
            
            this.startCell = range;
            range.Interior.ColorIndex = 3;
            
            // range.Interior.Color = XS.XlRgbColor.rgbLightGreen;
            
            signal.Set();
        }

        /// <summary>
        /// Handler for cell changing during end cell capturing.
        /// </summary>
        /// <param name="range"></param>
        void checkerEnd(XS.Range range)
        {
            if (range.Count != 1)
                return;

            this.endCell = range;
            range.Interior.ColorIndex = 3;
            
            // range.Interior.Color = XS.XlRgbColor.rgbLightGreen;
            
            signal.Set();
        }

        /// <summary>
        /// Closes the previously opened Excel connection and cleans up all the resources.
        /// </summary>
        public static void closeConnection(XS.Workbook wb)
        {
            XS.Application app = wb.Application;

            int excelProcessId = -1;
            GetWindowThreadProcessId(app.Hwnd, ref excelProcessId);

            wb.Saved = true;
    
            app.Quit();

            try
            {
                app.DisplayAlerts = true;
                Marshal.FinalReleaseComObject(wb);
                Marshal.FinalReleaseComObject(app);

                if (excelProcessId > 0)
                    Process.GetProcessById(excelProcessId).Kill();
            }
            catch
            {
                // все нормально, все само закрылось :)
            }
        }
    }
}
