using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Runtime.InteropServices;

using whiteMath.Graphers.Services;
using whiteMath.General;

namespace whiteMath.Graphers
{
    public enum FileType
    {
        Excel2007=0, 
        ExcelOld=1
    }

    public enum Direction
    {
        Left=-3, Right=3, Up=-1, Down=1
    }

    [Serializable]
    public class ExcelGrapher: StandardGrapher
    {
        // --------------------------------- THE DLL NEEDED TO KILL THE EXCEL PROCESS

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(int hWnd, ref int lpdwProcessId);

        // --------------------------------------------------------------------------

        private string StartCell1 = null, StartCell2 = null;        // стартовые ячейки, 
                                                                    // откуда начинается считывание точек
        private string FileName;                    // имя файла

        private FileType ext;                       // расширение файла
        private Direction 
            d1 = Direction.Right, 
            d2 = Direction.Right;                   // направление, в котором читаются точки по осям
        
        private string ColumnIndex1, ColumnIndex2;
        
        private int RowIndex1, RowIndex2;
        private int Count=0;

        private string LastRC1=null, LastRC2=null;  // последние прочитанные ячейки
        
        GrapherAddMultiplyConverter amc = new GrapherAddMultiplyConverter(1, 0, 1, 0);
            public GrapherAddMultiplyConverter Converter { get { return amc; } set { amc = value; } }

        // ------------------- КОНСТРУКТОРЫ

        public ExcelGrapher(string Path, string sc1, string sc2, Direction d1, Direction d2)
        {
            try
            {
                if (File.Exists(Path)) { FileName = Path; }
                    else throw new GrapherSettingsException("Файл не существует: " + Path);
                
                FileInfo FI = new FileInfo(Path);
                
                if (FI.Extension == ".xls") ext = FileType.ExcelOld;
                else if (FI.Extension == ".xlsx") ext = FileType.Excel2007;
                    else throw new GrapherSettingsException("Файл должен иметь расширение *.xls или *.xlsx!");
                
                uint temp;

                if (sc1.Trim().Length == 0 || sc1==null) throw new GrapherSettingsException("Не указана стартовая ячейка для оси X!");
                if (sc2.Trim().Length == 0 || sc2==null) throw new GrapherSettingsException("Не указана стартовая ячейка для оси Y!");

                List<char> letters = new List<char>();
                int i=0;

                while (!char.IsNumber(sc1[i]))
                {
                    if (char.IsLetter(sc1[i]) && Char.ToUpper(sc1[i]) >= 'A' && Char.ToUpper(sc1[i]) <= 'Z')
                        letters.Add(Char.ToUpper(sc1[i++]));
                    else throw new GrapherSettingsException("Неизвестный формат стартовой ячейки для оси X!");
                    if (i == sc1.Length) throw new GrapherSettingsException("Ошибка стартовой ячейки для оси X: не указан номер строки!");
                }

                if (i == 0) throw new GrapherSettingsException("Ошибка стартовой ячейки для оси X: не указан индекс колонки!");

                if (!uint.TryParse(sc1.Substring(i), out temp))
                    throw new GrapherSettingsException("Неизвестный формат стартовой ячейки для оси X!");

                if (temp == 0) throw new GrapherSettingsException("Ошибка стартовой ячейки для оси X: нумерация идет с 1!");

                RowIndex1 = (int)temp;
                ColumnIndex1 = new String(letters.ToArray());

                letters.Clear();
                i = 0;

                while (!char.IsNumber(sc2[i]))
                {
                    if (char.IsLetter(sc2[i]) && Char.ToUpper(sc2[i]) >= 'A' && Char.ToUpper(sc2[i])<='Z')
                        letters.Add(Char.ToUpper(sc2[i++]));
                    else throw new GrapherSettingsException("Неизвестный формат стартовой ячейки для оси Y!");
                    if (i == sc2.Length) throw new GrapherSettingsException("Ошибка стартовой ячейки для оси Y: не указан номер строки!");
                }

                if (i == 0) throw new GrapherSettingsException("Ошибка стартовой ячейки для оси Y: не указан индекс колонки!");

                if (!uint.TryParse(sc2.Substring(i), out temp))
                    throw new GrapherSettingsException("Неизвестный формат стартовой ячейки для оси Y!");

                if (temp == 0) throw new GrapherSettingsException("Ошибка стартовой ячейки для оси Y: нумерация идет с 1!");

                RowIndex2 = (int)temp;
                ColumnIndex2 = new String(letters.ToArray());

                StartCell1 = sc1;
                StartCell2 = sc2;

                this.d1 = d1;
                this.d2 = d2;
            }
            catch { throw; }
        }

        public ExcelGrapher(string Path)
        {
            try
            {
                if (File.Exists(Path)) { FileName = Path; }
                    else throw new GrapherSettingsException("Не существует файла с именем: " + Path);
         
                FileInfo FI = new FileInfo(Path);
                
                if (FI.Extension == ".xls") ext = FileType.ExcelOld;
                    else if (FI.Extension == ".xlsx") ext = FileType.Excel2007;
                    else throw new GrapherSettingsException("Невозможно определить тип файла (расширение)");
            }
            catch { throw; }
        }

        public string Axis1StartCell
        {
            get
            {
                return StartCell1 == null ? "No starting cell for axis 1!" : StartCell1;
            }

            set
            {
                try
                {
                    if (value.Trim().Length == 0 || value == null) throw new GrapherSettingsException("Error: no argument for axis 1");

                    List<char> letters = new List<char>();
                    int i = 0;

                    while (!char.IsNumber(value[i]))
                    {
                        if (char.IsLetter(value[i]) && Char.ToUpper(value[i]) >= 'A' && Char.ToUpper(value[i]) <= 'Z')
                            letters.Add(Char.ToUpper(value[i++]));
                        else throw new GrapherSettingsException("Unknown starting cell format for axis 1!");
                        if (i == value.Length) throw new GrapherSettingsException("Starting cell error for axis 1: no row number can be found!");
                    }

                    if (i == 0) throw new GrapherSettingsException("Starting cell error for axis 1: no column index can be found!");

                    uint temp;

                    if (!uint.TryParse(value.Substring(i), out temp))
                        throw new GrapherSettingsException("Unknown starting cell format for axis 1!");

                    if (temp == 0) throw new GrapherSettingsException("Starting cell error for axis 1: row numeration starts from 1!");

                    RowIndex1 = (int)temp;
                    ColumnIndex1 = new String(letters.ToArray());

                    StartCell1 = value;
                }
                catch { throw; }
            }
        }

        public string Axis2StartCell
        {
            get { return StartCell2 == null ? "No starting cell for axis 2!" : StartCell2; }
            set
            {
                if (value.Trim().Length == 0 || value == null) throw new GrapherSettingsException("Error: no argument for axis 2");

                int i = 0;
                List<char> letters = new List<char>();

                while (!char.IsNumber(value[i]))
                {
                    if (char.IsLetter(value[i]) && Char.ToUpper(value[i]) >= 'A' && Char.ToUpper(value[i]) <= 'Z')
                        letters.Add(Char.ToUpper(value[i++]));
                    else throw new GrapherSettingsException("Unknown starting cell format for axis 2!");
                    if (i == value.Length) throw new GrapherSettingsException("Starting cell error for axis 1: no row number can be found!");
                }


                if (i == 0) throw new GrapherSettingsException("Starting cell error for axis 2: no column index can be found!");

                uint temp;

                if (!uint.TryParse(value.Substring(i), out temp))
                    throw new GrapherSettingsException("Unknown starting cell format for axis 2!");

                if (temp == 0) throw new GrapherSettingsException("Starting cell error for axis 2: row numeration starts from 1!");

                RowIndex2 = (int)temp;
                ColumnIndex2 = new String(letters.ToArray());

                StartCell2 = value;
            }
        }

        public string Axis1LastReadCell // на какой ячейке закончилось считывание точек по оси 1
        { get { return LastRC1; } }
        
        public string Axis2LastReadCell // на какой ячейке закончилось считывание точек по оси 2
        { get { return LastRC2; } }
        
        public FileType ExcelType
        {
            get { return ext; }
        }
        
        public Direction Axis1PointsReadDirection
        {
            get { return d1; }
            set { d1 = value; }
        }
        public Direction Axis2PointsReadDirection
        {
            get { return d2; }
            set { d2 = value; }
        }
        public int AmountOfPointsRead { get { return Count; } }

        /* метод GetPoints предназначен для считывания точек из таблицы
         * по заданному пути, из заданных начальных ячеек в заданном направлении.
         * чтение заканчивается, когда встречается пустая ячейка или достигается край таблицы */

        public void GetPoints(int SheetNumber)
        {
            Application app = new Application();
            app.DisplayAlerts = false;

            try
            {
                if (SheetNumber <= 0) throw new GrapherActionImpossibleException("Неверно задан номер листа: нумерация идет со значения 1");
                if (StartCell1 == null || StartCell2 == null) throw new GrapherActionImpossibleException("Не заданы стартовые ячейки для считывания точек!");

                Workbook wb = app.Workbooks.Open(FileName, Type.Missing, true, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                if (SheetNumber > wb.Worksheets.Count) throw new Exception("Не существует листа с номером: " + SheetNumber);
                Worksheet ws = (Worksheet)wb.Worksheets[SheetNumber];

                Range alpha = (Range)ws.Cells[RowIndex1, ColumnIndex1];

                int i1 = alpha.Row;
                int j1 = alpha.Column;

                Range beta = (Range)ws.Cells[RowIndex2, ColumnIndex2];

                int i2 = beta.Row;
                int j2 = beta.Column;

                List<Point<double>> Coll = new List<Point<double>>();

                double x;
                double y;

                if (alpha.Value2 == null || !double.TryParse(alpha.Value2.ToString().Trim(), out x))
                    throw new GrapherActionImpossibleException("Данные в стартовой ячейке оси X отсутствуют или являются некорректными!");
                if (beta.Value2 == null || !double.TryParse(beta.Value2.ToString().Trim(), out y))
                    throw new GrapherActionImpossibleException("Данные в стартовой ячейке оси Y отсутствуют или являются некорректными!");

                Coll.Add(new Point<double> ( x*amc.Axis1Coefficient + amc.Axis1Addition, y*amc.Axis2Coefficient + amc.Axis2Addition )); // добавляем первую точку

                LastRC1 = alpha.get_AddressLocal(Type.Missing, Type.Missing, XlReferenceStyle.xlA1,
                        Type.Missing, Type.Missing).Replace("$", "");
                LastRC2 = beta.get_AddressLocal(Type.Missing, Type.Missing, XlReferenceStyle.xlA1,
                    Type.Missing, Type.Missing).Replace("$", "");

                int count = 1; // количество считанных точек.
                
                while (true)
                {
                    i1 += (int)d1 % 3; j1 += (int)d1 / 3;
                    i2 += (int)d2 % 3; j2 += (int)d2 / 3;
                    
                    if (i1 == 0 || j1 == 0 || i2 == 0 || j2 == 0) break;
                    
                    alpha = (Range)ws.Cells[i1, j1];
                    beta = (Range)ws.Cells[i2, j2];
                    if (alpha.Value2 == null || !double.TryParse(alpha.Value2.ToString().Trim(), out x)
                        || beta.Value2 == null || !double.TryParse(beta.Value2.ToString().Trim(), out y))
                        break;
                  
                    Coll.Add(new Point<double> ( x*amc.Axis1Coefficient + amc.Axis1Addition, y*amc.Axis2Coefficient + amc.Axis2Addition ) );
                  
                    LastRC1 = alpha.get_AddressLocal(Type.Missing, Type.Missing, XlReferenceStyle.xlA1,
                        Type.Missing, Type.Missing).Replace("$", "");
                    LastRC2 = beta.get_AddressLocal(Type.Missing, Type.Missing, XlReferenceStyle.xlA1,
                        Type.Missing, Type.Missing).Replace("$", "");

                    count++;
                }
                
                PointsArray = Coll.ToArray();
                FindMaximumsAndMinimums();
               
                int excelProcessId = -1;
                    GetWindowThreadProcessId(app.Hwnd, ref excelProcessId);

                wb.Saved = true;
                app.Quit();

                try
                {
                    app.DisplayAlerts = true;
                    
                    Marshal.ReleaseComObject(alpha);
                    Marshal.ReleaseComObject(beta);
                    Marshal.ReleaseComObject(ws);
                    Marshal.ReleaseComObject(wb);
                    Marshal.ReleaseComObject(app);
                    
                    if(excelProcessId>0)
                        Process.GetProcessById(excelProcessId).Kill();
                }
                catch
                {
                    // все нормально, все само закрылось :)
                }

                this.Count = count;
            }
            catch { base.yMin = base.yMax = 0; app.Quit(); throw; }
        }
    }
}
