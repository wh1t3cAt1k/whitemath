using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Drawing;

using whiteMath.Graphers.Services;
using whiteMath.General;

using CoordinateTransformer = whiteMath.Graphers.CoordinateTransformer<double, whiteMath.Calculators.CalcDouble>;

namespace whiteMath.Graphers
{
    /// <summary>
    /// Represents a standard grapher with points array inside.
    /// Class itself is abstract, but can be derived from.
    /// 
    /// Features:
    /// 
    ///     1. Provides methods to get new graphers by argument [OR] by indexes
    ///     2. Provides methods to get minimal and maximal argument and function values.
    ///     
    ///     3. Is bounded, that is, calling Graph() without mentioning a range
    ///        will result in a call equivalent to drawing the overall points array.
    ///        No exception is thrown.
    ///     4. Is a multi-range grapher suitable for drawing discontinuous functions
    ///        with a bounded argument range.
    ///     
    ///     5. ???   
    ///     6. PROFIT!!!
    ///     
    /// </summary>
    [Serializable]
    public abstract class StandardGrapher: AbstractGrapher, IGrapherBounded
    {
        private CoordinateTransformer ctf;

        protected IList<Point<double>> PointsArray = null;
        protected List<List<PointF>> GraphingArray = null;

        protected double xMax = double.NaN;
        protected double yMax = double.NaN;
        protected double xMin = double.NaN;
        protected double yMin = double.NaN;

        public double MinAxis1 { get { return xMin; } }
        public double MaxAxis1 { get { return xMax; } }
        public double MinAxis2 { get { return yMin; } }
        public double MaxAxis2 { get { return yMax; } }


        // ----------------------------
        // ----------------------------
        // ----------------------------

        /// <summary>
        /// Gets the grapher points array as specified.
        /// </summary>
        /// <returns></returns>
        public Point<double>[] GetPointsArray()
        {
            Point<double>[] arr = new Point<double>[PointsArray.Count];

            ServiceMethods.Copy(PointsArray, arr);
            return arr;
        }

        /// <summary>
        /// Gets the Axis1 data array as specified.
        /// </summary>
        /// <returns></returns>
        public double[] GetAxis1Array()
        {
            if (PointsArray == null) throw new GrapherActionImpossibleException("Массива точек не существует в экземляре класса Grapher. Воспользуйтесь методом GetPoints() для получения.");

            double[] newArr = new double[PointsArray.Count];
            for (int i = 0; i < PointsArray.Count; i++) newArr[i] = PointsArray[i][0];
            return newArr;
        }

        /// <summary>
        /// Gets the Axis2 data array as specified.
        /// </summary>
        /// <returns></returns>
        public double[] GetAxis2Array()
        {
            if (PointsArray == null) throw new GrapherActionImpossibleException("Массива точек не существует в экземляре класса Grapher. Воспользуйтесь методом GetPoints() для получения.");

            double[] newArr = new double[PointsArray.Count];
            for (int i = 0; i < PointsArray.Count; i++) newArr[i] = PointsArray[i][1];
            return newArr;
        }

        // ----------------------------
        // --------SCALING-------------
        // ----------------------------

        /// <summary>
        /// Returns the standard array grapher scaled by argument bounds.
        /// Does not contain any points whose X argument is out of the bounds specified.
        /// </summary>
        /// <param name="Axis1Min">The lower bound of the argument scaling.</param>
        /// <param name="Axis1Max">The upper bound of the argument scaling.</param>
        /// <returns></returns>
        public StandardGrapher newGrapherByArgumentBounds(double Axis1Min, double Axis1Max)
        {
            int minInd = 0, maxInd = 0;
            FindMaximumsAndMinimums();
        
            if (Axis1Min >= Axis1Max) throw new GrapherActionImpossibleException("Неверно заданы границы отрезка масштабирования.");

            int i = 0;

            for (i = 0; i < PointsArray.Count; i++)
                if (PointsArray[i][0] >= Axis1Min)
                { minInd = i; break; }

            for (i = minInd; i < PointsArray.Count; i++)
            {
                if (PointsArray[i][0] > Axis1Max) { maxInd = i - 1; break; }
                else if (PointsArray[i][0] == Axis1Max) { maxInd = i; break; }
            }

            if (maxInd == 0 && i == PointsArray.Count) maxInd = PointsArray.Count - 1;
            
            if (minInd >= maxInd) throw new GrapherActionImpossibleException("В заданном диапазоне должно находиться не менее двух точек функции.");
            return newGrapherByIndexBounds(minInd, maxInd);
        }

        /// <summary>
        /// Returns the standard array grapher scaled by points array indices.
        /// </summary>
        /// <param name="minInd">The lower index of points array to copy to the new grapher.</param>
        /// <param name="maxInd">The upper index of points array to copy to the new grapher.</param>
        /// <returns></returns>
        public StandardGrapher newGrapherByIndexBounds(int minInd, int maxInd)
        {
            // проверяем корректность границ заданных индексов

            if (minInd >= maxInd) throw new GrapherActionImpossibleException(String.Format("Неверно заданы границы отрезка масштабирования: индексы начала ({0}) больше или совпадает с индексом конца ({1}).", minInd, maxInd));
            if (minInd < 0 || maxInd >= PointsArray.Count) throw new GrapherActionImpossibleException("Неверно заданы границы отрезка масштабирования: индексы выходят за границы массива.");

            // создаем копию объекта

            StandardGrapher temp = (StandardGrapher)this.MemberwiseClone();

            // temp.PointsArray = new Point<double>[maxInd - minInd + 1 + (minInd == 0 ? 0 : 1) + (maxInd == PointsArray.Count - 1 ? 0 : 1)];
            // ServiceMethods.Copy(this.PointsArray, minInd - (minInd == 0 ? 0 : 1), temp.PointsArray, 0, (maxInd == (PointsArray.Count - 1) ? maxInd : maxInd + 1) - (minInd == 0 ? minInd : (minInd - 1)) + 1);

            temp.PointsArray = new Point<double>[maxInd - minInd + 1];
            ServiceMethods.Copy(this.PointsArray, minInd, temp.PointsArray, 0, maxInd - minInd + 1);

            // находим новые минимальные и максимальные значения

            temp.FindMaximumsAndMinimums();

            if (Step1 != 0 && Step2 != 0)
            {
                temp.Step1 = this.Step1 * (temp.xMax - temp.xMin) / (this.xMax - this.xMin);
                temp.Step2 = this.Step2 * (temp.yMax - temp.yMin) / (this.yMax - this.yMin);
                if (this.yMax == this.yMin) temp.Step2 = this.Step2;
            }

            return temp;
        } // масштабированный по номеру измерения аргумента графер

        // ---------------------------------------------
        // ---------------------------------------------
        // ---------------------------------------------

        protected bool isNormalPoint(double x, double y)
        { 
            return (!double.IsNaN(x) && x != double.PositiveInfinity && x != double.NegativeInfinity
                  && !double.IsNaN(y) && y != double.PositiveInfinity && y != double.NegativeInfinity) ; 
        }

        // ---------------------------------------------
        // ---------------------------------------------
        // ---------------------------------------------

        protected void FindMaximumsAndMinimums() // находит минимумы и максимумы по осям. Предполагается, что массив точек упорядочен по аргументу.
        {
            if (PointsArray == null || PointsArray.Count == 0) { xMin = xMax = yMin = yMax = double.NaN; return; }

                xMin = PointsArray[0][0];
                xMax = PointsArray[PointsArray.Count - 1][0];

                yMin = PointsArray[0][1];
                yMax = PointsArray[0][1];
            
                for (int i = 0; i < PointsArray.Count; i++)
                {
                    if (PointsArray[i][1] < yMin) yMin = PointsArray[i][1];
                    else if (PointsArray[i][1] > yMax) yMax = PointsArray[i][1];

                    if (PointsArray[i][0] < xMin) xMin = PointsArray[i][0];
                    else if (PointsArray[i][0] > xMax) xMax = PointsArray[i][0];
                }
        }

        /// <summary>
        /// Creates a mid-ready graphing array with all the coefficients
        /// accounted.
        /// </summary>
        protected void MakeGraphingArray()
        {
            FindMaximumsAndMinimums();
            this.MakeGraphingArray(this.xMin, this.xMax, this.yMin, this.yMax);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xMin">The lower X boundary of the range.</param>
        /// <param name="xMax">The upper X boundary of the range.</param>
        /// <param name="yMin">The lower Y boundary of the range.</param>
        /// <param name="yMax">The upper Y boundary of the range.</param>
        protected void MakeGraphingArray(double xMin, double xMax, double yMin, double yMax) // делает массив функциональной зависимости с учетом k и s. Предполагается, что массив точек упорядочен по аргументу.
        {
            if (PointsArray == null) return;
                
            GraphingArray = new List<List<PointF>>();
            int diapasonNum = -1;
            bool insidePit = true; // находимся внутри разрыва, точки добавлять не надо.

            double divider = GrapherArrayWork.CheckForCastInfinities(PointsArray);

            for (int i = 0; i < PointsArray.Count; i++)
            {
                float xToWrite = (float)(PointsArray[i][0] / divider);
                float yToWrite = (float)(PointsArray[i][1] / divider);

                if (!isNormalPoint(xToWrite, yToWrite) && !insidePit || !(xToWrite>=((float)xMin) && xToWrite<=((float)xMax)) || !(yToWrite>=((float)yMin) && yToWrite<=((float)yMax)))
                {
                    insidePit = true;
                }
                else if (insidePit)
                {
                    insidePit = false;
                    diapasonNum++;
                    GraphingArray.Add(new List<PointF>());
                }

                if (!insidePit)
                    GraphingArray[diapasonNum].Add(new PointF(xToWrite, yToWrite));
            }
            
           }

        /// <summary>
        /// для метода Graph, который не знает об области
        /// значений данного диапазона.
        /// </summary>
        private void findYminYmax(double xMin, double xMax, out double yMin, out double yMax) 
        {
            yMin = double.PositiveInfinity;
            yMax = double.NegativeInfinity;

            for (int i = 0; i < PointsArray.Count; i++)
            {
                if (PointsArray[i][0] >= xMin && PointsArray[i][0] <= xMax)
                {
                    double y = PointsArray[i][1];
                    if (y < yMin) yMin = y;
                    if (y > yMax) yMax = y;
                }
            }

            if (!isNormalPoint(yMin, yMax)) yMin = yMax = 0;
        }

        // метод рисует график на основе массива точек.
        // хочется сразу сказать, что те xMin, xMax, yMin, yMax,
        // которые здесь фигурируют как локальные переменные - не имеют
        // ничего общего с нашими родными x...y... вы поняли.

        #region GraphMethods

        public override void Graph(Image dest, GraphingArgs ga)
        {
            this.Graph(dest, ga, xMin, xMax, yMin, yMax);
        }

        public override void Graph(Image dest, GraphingArgs ga, double xMin, double xMax)
        {
            double yMin, yMax;
            findYminYmax(xMin, xMax, out yMin, out yMax); // ищем макс и мин по y для заданного диапазона 

            this.Graph(dest, ga, xMin, xMax, yMin, yMax);
        }

        public override void Graph(Image dest, GraphingArgs ga, double xMin, double xMax, double yMin, double yMax)
        {
            if (!isNormalPoint(xMin, xMax) || !isNormalPoint(yMin, yMax))
                throw new GrapherActionImpossibleException("Invalid graphing bounds: containing NaNs or infinities.");

            // создаем массив точек для рисования
            MakeGraphingArray(xMin, xMax, yMin, yMax);
            
            // собственно рисуем все
            GraphMethodSkeleton(dest, ga, xMin, xMax, yMin, yMax);
        }

        private void GraphMethodSkeleton(Image dest, GraphingArgs graphingArgs, double xMin, double xMax, double yMin, double yMax)
        {
            if (dest.Width <= 0 || dest.Height <= 0) throw new GrapherActionImpossibleException("Размеры картинки являются отрицательными!");

            if (dest.Height < graphingArgs.IndentFromBounds * 2)
                graphingArgs.IndentFromBounds = dest.Height / 2;
         
            if (dest.Width <= graphingArgs.IndentFromBounds * 2)
                graphingArgs.IndentFromBounds = dest.Width / 2;

            if (GraphingArray == null) throw new GrapherActionImpossibleException("Массив точек является пустой ссылкой или массивом длины меньше 2! Если Вы используете ExcelGrapher, сначала вызовите метод GetPoints() из соответствующего экземпляра класса.");
            
            // ------------ Получение объекта Graphics и настройки

            Graphics G = Graphics.FromImage(dest);
            
            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // ------------ заливка фона

            if(graphingArgs.BackgroundBrush != null)
                G.FillRectangle(graphingArgs.BackgroundBrush, new Rectangle(0, 0, dest.Width, dest.Height));

            // ------------ Временные переменные для удобства

            int Width = dest.Width;
            int Height = dest.Height;
            int Shift = graphingArgs.IndentFromBounds;

            // ------------ Вырожденный случай, 
            // ------------ надо предусмотреть.

            if (xMin == xMax)
            { 
                xMin = xMin - 10; 
                xMax = xMax + 10;
            }

            if (yMax == yMin)
            {
                yMin = yMin - 10;
                yMax = yMax + 10;
            }

            // ------------ Изменение объекта CoordinateTransformer

            ctf = new CoordinateTransformer(xMin, xMax, yMin, yMax, (x => x), new Size(Width, Height), Shift);

            // ----------- НАЗНАЧЕНИЕ ШАГА НАСЕЧЕК ------------

            if (Step1 == 0) Step1 = (xMax - xMin) / 10; // шаги по умолчанию
            if (Step2 == 0) Step2 = (yMax - yMin) / 10; // шаги по умолчанию

            // ------------------------------------------------

            // ----------- Назначение координат точек в зависимости от экстремальных значений
            // ----------- аргумента и функции


            PointF Zero     = (PointF)ctf.CoordinateSystemCenter, 
                   Left     = (PointF)ctf.CoordinateSystemLeft,
                   Right    = (PointF)ctf.CoordinateSystemRight, 
                   Up       = (PointF)ctf.CoordinateSystemUp,
                   Down     = (PointF)ctf.CoordinateSystemDown;

            double kx = 1 / ctf.TransformerAxisX.ScaleFactor;
            double ky = 1 / ctf.TransformerAxisY.ScaleFactor;

            double alpha, beta;
            alpha = xMax <= 0 ? xMax : (xMin >= 0 ? xMin : 0);
            beta = yMax <= 0 ? yMax : (yMin >= 0 ? yMin : 0);

            // ------------ Рисуем начало координат
            // ------------ (в данный момент не рисуется)

            // G.DrawString("N(" + String.Format(formatter1, alpha) + ";" + String.Format(formatter2, beta) + ")", graphingArgs.CoordFont, Brushes.Black, new PointF(Zero.X - 10, Zero.Y));

            // ------------ Рисуем сетку и оси

            if (graphingArgs.GridPen != null) 
                DrawCoordGrid(G, graphingArgs.GridPen, Zero, Left, Right, Up, Down, kx, ky);
            
            if (graphingArgs.CoordPen != null) 
                DrawCoordinates(G, graphingArgs.CoordPen, Zero, Left, Right, Up, Down, kx, ky);

            // ------------ Рисуем графики

            if (graphingArgs.CurvePen != null)
            {
                foreach (List<PointF> diapPoints in GraphingArray)
                {
                    // xxx Вычисляем окончательные координаты точек на картинке.

                    for (int i = 0; i < diapPoints.Count; i++)
                        diapPoints[i] = (PointF)ctf.transformFunctionToPixel(diapPoints[i].X, diapPoints[i].Y);

                    // xxx Если единственная точка в диапазоне, рисуем ее в виде эллипса.

                    if (diapPoints.Count == 1) G.FillEllipse(new SolidBrush(graphingArgs.CurvePen.Color), diapPoints[0].X, diapPoints[0].Y, graphingArgs.CurvePen.Width + 2, graphingArgs.CurvePen.Width + 2);

                    // xxx Если несколько точек, проводим кривую / ломаную.

                    else switch (graphingArgs.CurveType)
                        {
                            case LineType.Polygon: G.DrawPolygon(graphingArgs.CurvePen, diapPoints.ToArray()); break;
                            case LineType.Line: G.DrawLines(graphingArgs.CurvePen, diapPoints.ToArray()); break;
                            case LineType.CardinalCurve: G.DrawCurve(graphingArgs.CurvePen, diapPoints.ToArray()); break;
                        }
                }
            }

            // ------ Рисование цифр на насечках
            // ------ (в последнюю очередь, чтобы цифры располагались НАД кривыми)

            if (graphingArgs.CoordFont != null) 
                DrawCoordNumbers(G, graphingArgs.CoordFont, Zero, Left, Right, Up, Down, kx, ky);
            
            // ------ Уничтожение объекта Graphics

            G.Dispose();
        }

        #endregion

        # region StepPoints

        private PointF[] getAxis1StepPoints(PointF Left, PointF Zero, PointF Right, double kx)
        {
            List<PointF> temp = new List<PointF>();

            decimal i;

            for (i = (decimal)Left.X; i <= (decimal)Right.X; i += (decimal)(Step1 * kx))
                temp.Add(new PointF((float)i, Zero.Y));

            return temp.ToArray();
        }

        private PointF[] getAxis2StepPoints(PointF Down, PointF Zero, PointF Up, double ky)
        {
            List<PointF> temp = new List<PointF>();

            decimal i;

            for (i = (decimal)Down.Y; i >= (decimal)Up.Y; i -= (decimal)(Step2 * ky))
                temp.Add(new PointF(Zero.X, (float)i));

            return temp.ToArray();
        }

        #endregion

        /// <summary>
        /// Draws the coordinate grid.
        /// </summary>
        private void DrawCoordGrid(Graphics G, Pen Pe, PointF Zero, PointF Left, PointF Right, PointF Up, PointF Down, double kx, double ky)
        {
            G.TextRenderingHint = TextRenderingHint.AntiAlias;

            decimal i;

            for (i = (decimal)Left.X; i <= (decimal)Right.X; i += (decimal)(Step1 * kx))
                G.DrawLine(Pe, new PointF((float)i, Down.Y), new PointF((float)i, Up.Y));

            for (i = (decimal)Down.Y; i >= (decimal)Up.Y; i -= (decimal)(Step2 * ky))
                G.DrawLine(Pe, new PointF(Left.X, (float)i), new PointF(Right.X, (float)i));
        }

        /// <summary>
        /// Рисует линии координатных осей с насечками.
        /// А также внешнюю окантовку графика (с насечками +))
        /// </summary>
        private void DrawCoordinates(Graphics G, Pen Pe, PointF Zero, PointF Left, PointF Right, PointF Up, PointF Down, double kx, double ky)
        {
            G.DrawLine(Pe, Left, Right);    // Рисуем ось X
            G.DrawLine(Pe, Down, Up);       // Рисуем ось Y

            PointF DownLeft = new PointF(Left.X, Down.Y);
            PointF DownRight = new PointF(Right.X, Down.Y);
            PointF UpLeft = new PointF(Left.X, Up.Y);
            PointF UpRight = new PointF(Right.X, Up.Y);

            // --- Рисуем контур

            Pen temp = (Pen)Pe.Clone();
            temp.Color = Color.FromArgb(100, Pe.Color);
            temp.Width = 2;

            G.DrawLine(temp, DownLeft, UpLeft);
            G.DrawLine(temp, UpLeft, UpRight);
            G.DrawLine(temp, UpRight, DownRight);
            G.DrawLine(temp, DownRight, DownLeft);
            
            decimal i;
            
            decimal leftX = (decimal)Left.X;
            decimal rightX = (decimal)Right.X;
            decimal downY = (decimal)Down.Y;
            decimal upY = (decimal)Up.Y;

            // ---- насечки

            for (i = leftX; i <= rightX + 1; i += (decimal)(Step1 * kx))
            {
                G.DrawLine(Pe, new PointF((float)i, Zero.Y - 3), new PointF((float)i, Zero.Y + 3));     // насечка на оси
                G.DrawLine(Pe, new PointF((float)i, DownLeft.Y - 6), new PointF((float)i, DownLeft.Y)); // насечка на границе
            }

            for (i = downY; i >= upY - 1; i -= (decimal)(Step2 * ky))
            {
                G.DrawLine(Pe, new PointF(Zero.X - 3, (float)i), new PointF(Zero.X + 3, (float)i));
                G.DrawLine(Pe, new PointF(DownLeft.X, (float)i), new PointF(DownLeft.X + 6, (float)i)); // насечка на границе
            }
        }

        // ------- метод рисует цифровые отметки на насечках осей
        // ------- + названия осей
        private void DrawCoordNumbers(Graphics G, Font CoordinateFont, PointF Zero, PointF Left, PointF Right, PointF Up, PointF Down, double kx, double ky)
        {
            decimal i;

            decimal leftX = (decimal)Left.X;
            decimal rightX = (decimal)Right.X;

            decimal downY = (decimal)Down.Y;
            decimal upY = (decimal)Up.Y;

            // (float)(xMin + Step1 * j++))

            SizeF tempSize;
            string tempStr;

            for (i = leftX; i <= rightX; i += (decimal)(Step1 * kx))
            {
                tempStr = String.Format(formatter1, ctf.transformImageXtoFunctionX((float)i));
                tempSize = G.MeasureString(tempStr, CoordinateFont);

                // if (Math.Abs(Down.Y-Zero.Y)>tempSize.Height || Math.Abs(i - (decimal)Zero.X) > (decimal)tempSize.Width) 
                G.DrawString(tempStr, CoordinateFont, Brushes.Black, new PointF((float)i - tempSize.Width/2, Down.Y + 5));
            }

            // (float)(yMin + Step2 * j++))

            for (i = downY; i >= upY; i -= (decimal)(Step2 * ky))
            {
                tempStr = String.Format(formatter2, ctf.transformImageYToFunctionY((float)i));
                tempSize = G.MeasureString(tempStr, CoordinateFont);

                // if (Math.Abs(Left.X-Zero.X)>tempSize.Width || Math.Abs(i - (decimal)Zero.Y) > (decimal)tempSize.Height) 
                G.DrawString(tempStr, CoordinateFont, Brushes.Black, new PointF(Left.X - tempSize.Width - 5, (float)i - tempSize.Height/2));
            }

            // названия осей

            tempSize = G.MeasureString(this.Axis1Name, CoordinateFont);
            G.DrawString(this.Axis1Name, CoordinateFont, Brushes.Black, new PointF(Right.X + 10, Right.Y - tempSize.Height/2));

            tempSize = G.MeasureString(this.Axis2Name, CoordinateFont);
            G.DrawString(this.Axis2Name, CoordinateFont, Brushes.Black, new PointF(Up.X, Up.Y - tempSize.Height - 10));

        }

    }
}
