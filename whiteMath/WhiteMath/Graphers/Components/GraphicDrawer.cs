#if (!NON_WINDOWS_ENVIRONMENT)

using System;
using whiteMath.Graphers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Text;
using System.IO;
using System.Windows.Forms;
using whiteMath.Imaging;

using CoordinateTransformer = whiteMath.Graphers.CoordinateTransformer<double, whiteMath.CalcDouble>;

namespace whiteMath.Graphers
{
    /// <summary>
    /// Windows form for drawing graphs using Grapher objects.
    /// Suitable for bounded and unbounded graphers.
    /// 
    /// author: Pavel Kabir
    /// revised: 21.01.2010
    /// version: 1.1
    /// </summary>
    public partial class GraphicDrawer: UserControl
    {
        CoordinateTransformer ctf;
        AbstractGrapher G;  // the grapher object
        
        Image drawnImage;                // an image that is fully or partially shown
        List<IDecal> decals;           // the list of decals to draw onto the image

        int xImageCoordinate=0;            // coordinates of image to draw
        int yImageCoordinate=0;            //

                            // изначальный диапазон рисования

        readonly double xMinInitial;
        readonly double xMaxInitial;
        readonly double yMinInitial;
        readonly double yMaxInitial;

        readonly double keyboardScaleFactor = 2.0;

                            // диапазон рисования
        double xMin;
        double xMax;
        double yMin;
        double yMax;
                            //--------------------

        // ----------------- service values

        bool somethingIsDrawn = false;
        
        // ----------------- graphing parameters
        
        Font CoordFont = SystemFonts.SmallCaptionFont;
        LineType LT = LineType.Line;

        // ----------------- Events

        public event GraphicDrawerGraphClickEventHandler GraphicDrawerGraphClick;

        //  ------------- КОНСТРУКТОРЫ

        public GraphicDrawer(StandardGrapher obj):
            this(obj, obj.MinAxis1, obj.MaxAxis1, obj.MinAxis2, obj.MaxAxis2) {}

        public GraphicDrawer(AbstractGrapher grapher, double xMin, double xMax, double yMin, double yMax)
        {
            InitializeComponent();

            G = grapher;

            this.xMinInitial = xMin;
            this.xMaxInitial = xMax;
            this.yMinInitial = yMin;
            this.yMaxInitial = yMax;
            
            restoreTextBoxesInitial();                      // set textboxes

            this.decals = new List<IDecal>();               // set decals empty
            this.multiGrapherPens1.setGrapher(grapher);     // set the component
            this.Text = "GraphicDrawer - " + grapher.Name;  // set the form header

            // ---------- if user chooses another color

            this.multiGrapherPens1.ColorValueChanged += new EventHandler(
                delegate
                {
                    buttonGraph_Click(this, EventArgs.Empty);
                }
                );

            // ----------------------

            pictureBoxWindow.SizeMode = PictureBoxSizeMode.Normal;
            pictureBoxWindow.Image = new Bitmap(pictureBoxWindow.Width, pictureBoxWindow.Height);

            // ---------- максимальный отступ осей и заданный уже отступ

            Graphics tmp = Graphics.FromImage(pictureBoxWindow.Image);
            
            numericUpDownShift.Value = G.getRecommendedIndentFromBorder(tmp, CoordFont, string.Format("{{0:G{0}}}", numericUpDownEX.Value), string.Format("{{0:G{0}}}", numericUpDownEX2.Value));
            numericUpDownShift.Maximum = this.pictureBoxWindow.Height / 2 - (this.pictureBoxWindow.Height/2) % 100;
            
            pictureBoxWindow.MinimumSize = new Size((int)numericUpDownShift.Maximum, (int)numericUpDownShift.Maximum);

            // ---------- обработчики событий

            // показ текущих координат
            pictureBoxWindow.MouseMove += new MouseEventHandler(
                delegate(object source, MouseEventArgs e)
                {
                    if(somethingIsDrawn)
                        toolStripStatusLabelCoord.Text = String.Format("X={0}, Y={1}", ctf.transformImageXtoFunctionX(this.xImageCoordinate + e.X), ctf.transformImageYToFunctionY(this.yImageCoordinate + e.Y));
                });

            // набоссить иксов
            pictureBoxWindow.MouseClick += new MouseEventHandler(
                delegate(object source, MouseEventArgs e)
                {
                    if (e.Button == MouseButtons.Left) return;

                    if (somethingIsDrawn && GraphicDrawerGraphClick != null)
                    {
                        double x = ctf.transformImageXtoFunctionX(this.xImageCoordinate + e.X);
                        double y = ctf.transformImageYToFunctionY(this.yImageCoordinate + e.Y);
                        GraphicDrawerGraphClick.Invoke(this, new GraphicDrawerGraphClickEventArgs(e.X, e.Y, x, y));
                    }
                });

            richTextBoxFontTest.Font = CoordFont;
            
            // надо вызвать, чтобы установить границы и сделать первоначальное рисование
            buttonRestore_Click(this, EventArgs.Empty);

            tmp.Dispose();
        }

        /// <summary>
        /// Обработчик события на то, если пользователь хочет изменить разрешение.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxWindowed_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxWindowed.Checked)
            {
                checkBoxNotWindowed.Checked = false;
                numericUpDownHeight.Enabled = numericUpDownWidth.Enabled = false;
            }
            else 
            {
                checkBoxNotWindowed.Checked = true;
                numericUpDownHeight.Enabled = numericUpDownWidth.Enabled = true;
                numericUpDownWidth.Value = 1024;
                numericUpDownHeight.Value = 768;
            }
        }

        /// <summary>
        /// Restores the initial xMin and xMax values in the textboxes.
        /// </summary>
        private void restoreTextBoxesInitial()
        {
            textBoxFromX.Text = xMinInitial.ToString();
            textBoxFromY.Text = yMinInitial.ToString();
            textBoxToX.Text = xMaxInitial.ToString();
            textBoxToY.Text = yMaxInitial.ToString();
        }

        /// <summary>
        /// Sets the textboxes with new xMin and so on values.
        /// </summary>
        private void setTextBoxesWithValues()
        {
            textBoxFromX.Text = xMin.ToString();
            textBoxFromY.Text = yMin.ToString();
            textBoxToX.Text = xMax.ToString();
            textBoxToY.Text = yMax.ToString();
        }

        /// <summary>
        /// Восстанавливает умолчания по границам рисования
        /// и перерисовывает график.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRestore_Click(object sender, EventArgs e)
        {
            restoreTextBoxesInitial();
            buttonGraph_Click(this, EventArgs.Empty);
        }

        /// <summary>
        /// Проверка, указал ли пользователь иное разрешение,
        /// чем разрешение рабочего окна.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxNotWindowed_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxNotWindowed.Checked)
                checkBoxWindowed.Checked = false;
            else checkBoxWindowed.Checked = true;
        }

        private void buttonChooseFont_Click(object sender, EventArgs e)
        {
            if(fontDialogForGrapher.ShowDialog()==DialogResult.OK)
             CoordFont = fontDialogForGrapher.Font;
            richTextBoxFontTest.Font = CoordFont;
        }

        // ---------------------------------
        // ------- MAIN GRAPH METHOD -------
        // ---------------------------------


        private void buttonGraph_Click(object sender, EventArgs e)
        {
            this.somethingIsDrawn = false;      // reset the bool value to false.

            Pen CurvePen = new Pen(new SolidBrush(multiGrapherPens1.getCurveColor()), (int)numericUpDownCW.Value);
            Pen CoordPen = new Pen(new SolidBrush(multiGrapherPens1.getAxisColor()), (int)numericUpDownAW.Value);
            Brush BackBrush = new SolidBrush(multiGrapherPens1.getBackgroundColor());

            if (checkBoxNotWindowed.Checked)
                drawnImage = new Bitmap((int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value);
            else
                drawnImage = new Bitmap(pictureBoxWindow.Width, pictureBoxWindow.Height);

            // --------- Работа с масштабированием

            #region CheckingTextboxValidating

            // если юзер перемещается мышкой, то не надо парсить текстбоксы.
            // это занимает время.

            if (!(e is EventArgs_NoCheck))
            {
                if (!double.TryParse(textBoxFromX.Text, out xMin))
                {
                    MessageBox.Show(this, "Невозможно определить значение нижней границы масштабирования. Проверьте правильность ввода.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxFromX.Clear();
                    textBoxFromX.Focus();
                    return;
                }
                if (!double.TryParse(textBoxToX.Text, out xMax))
                {
                    MessageBox.Show(this, "Невозможно определить значение верхней границы масштабирования. Проверьте правильность ввода.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxToX.Clear();
                    textBoxToX.Focus();
                    return;
                }
                if (!double.TryParse(textBoxFromY.Text, out yMin))
                {
                    MessageBox.Show(this, "Невозможно определить значение нижней границы масштабирования. Проверьте правильность ввода.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxFromY.Clear();
                    textBoxFromY.Focus();
                    return;
                }
                if (!double.TryParse(textBoxToY.Text, out yMax))
                {
                    MessageBox.Show(this, "Невозможно определить значение верхней границы масштабирования. Проверьте правильность ввода.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxToY.Clear();
                    textBoxToY.Focus();
                    return;
                }
            }

            // --------- Назначаем шаг

            if (yMax != yMin && !double.IsNaN(yMax) && !double.IsInfinity(yMin))
            {
                G.Axis1CoordinateStep = (xMax - xMin) / (int)numericUpDownParts.Value;
                G.Axis2CoordinateStep = (yMax - yMin) / (int)numericUpDownParts.Value;
            }

            #endregion

            string formatter; 

            // --------- Точность на оси X

            formatter = "{0:G";
            formatter+=numericUpDownEX.Value.ToString()+"}";

            G.Axis1NumbersFormatter = formatter;

            // --------- Точность на оси Y

            formatter = "{0:G";
            formatter += numericUpDownEX2.Value.ToString() + "}";

            G.Axis2NumbersFormatter = formatter;

            // -------------------------------------

            try
            {
                GraphingArgs graphingArgs = new GraphingArgs(
                    (int)numericUpDownShift.Value,
                    BackBrush, 
                    CoordPen,
                    CoordFont, 
                    new Pen(Color.FromArgb(CoordPen.Color.A/4, CoordPen.Color)),
                    CurvePen, 
                    LT);

                // ---- if multigrapher, set pens
                // ---- and the width!

				var multiGrapher = G as MultiGrapher;

                if (multiGrapher != null)
                {
                    Pen[] arr = multiGrapherPens1.getCurveColors().pensFromColors();

                    for (int i = 0; i < arr.Length; i++)
                        arr[i].Width = (int)numericUpDownCW.Value;

                    multiGrapher.setPens(arr);
                }

                // ---- do it.
                
                G.Graph(drawnImage, graphingArgs, xMin, xMax, yMin, yMax);
                
                // ---- re-initialize the transformer

                ctf = new CoordinateTransformer(xMin, xMax, yMin, yMax, (x => x), drawnImage.Size, (int)numericUpDownShift.Value);

                // ------------------------------------

                if (drawnImage.Width <= pictureBoxWindow.Width && drawnImage.Height <= pictureBoxWindow.Height)
                     pictureBoxWindow.Image = drawnImage;
                else ShowImageInPictureBox(); 
            }
            catch (GrapherException ex)
            {
                MessageBox.Show(this, "Не удалось построить график: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CurvePen.Dispose();
            CoordPen.Dispose();
            this.somethingIsDrawn = true;
        }

        /// <summary>
        /// Shows part of the picture in the picture box, taking the current
		/// coordinates into account.
        /// </summary>
        private void ShowImageInPictureBox()
        {
            int pictureBoxWidth = pictureBoxWindow.Width;
            int pictureBoxHeight = pictureBoxWindow.Height;
            
            pictureBoxWindow.Image = new Bitmap(pictureBoxWidth, pictureBoxHeight);
            Graphics graphics = Graphics.FromImage(pictureBoxWindow.Image);

			if (xImageCoordinate > drawnImage.Width - pictureBoxWidth)
			{
				xImageCoordinate = drawnImage.Width - pictureBoxWidth;
			}
			else if (xImageCoordinate < 0)
			{
				xImageCoordinate = 0;
			}

			if (yImageCoordinate >= drawnImage.Height - pictureBoxHeight)
			{
				yImageCoordinate = drawnImage.Height - pictureBoxHeight;
			}
			else if (yImageCoordinate < 0)
			{
				yImageCoordinate = 0;
			}

			graphics.DrawImage(
				drawnImage, 
				new Rectangle(0, 0, pictureBoxWidth, pictureBoxHeight), 
				new Rectangle(xImageCoordinate, yImageCoordinate, pictureBoxWidth, pictureBoxHeight), 
				GraphicsUnit.Pixel);
            
            pictureBoxWindow.Refresh();
        }

        private void checkBoxLineCurve_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxLineCurve.Checked) { LT = LineType.CardinalCurve; checkBoxLineBroken.Checked = false; }
            else { LT = LineType.Line; checkBoxLineBroken.Checked = true; }
        }

        private void checkBoxLineBroken_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxLineCurve.Checked = !checkBoxLineBroken.Checked;
        }


        /// <summary>
        /// Saves the displayed graph into a file.
        /// </summary>
        /// <param name="sender">Event sender. Not used.</param>
        /// <param name="e">Event arguments. Not used.</param>
        private void buttonGraphToFile_Click(object sender, EventArgs e)
        {
            if (!somethingIsDrawn || saveFileDialog.ShowDialog() != DialogResult.OK)
				return;

            System.Drawing.Imaging.ImageFormat imageFormat;

			switch (saveFileDialog.FilterIndex)
			{
				case 1:
					imageFormat = System.Drawing.Imaging.ImageFormat.Png;
					break;
				case 2:
					imageFormat = System.Drawing.Imaging.ImageFormat.Bmp;
					break;
				default:
					imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
					break;
			}                

            buttonGraph_Click(this, EventArgs.Empty);
            drawnImage.Save(saveFileDialog.FileName, imageFormat);

			MessageBox.Show(
				this, 
				string.Format(Graphers.Messages.GraphSuccessfullySaved, saveFileDialog.FileName),
				Graphers.Messages.Success,
				MessageBoxButtons.OK, 
				MessageBoxIcon.Asterisk);
        }

        #region MouseOperations

        bool ctrlButtonPressed = false;
        bool 
            moveOperation = false,      // передвижение по координатной оси
            hoverOperation = false,     // передвижение по графику высокого разрешения
            scaleOperation = false;     // масштабирование мышью
        
        Point firstPosition;
        int deltaX, deltaY;

        RectangleDecal rectangle;

        // ----------- keyboard

        // надо обрабатывать родителя, чтобы перехватить его клавиатурные сигналы.
        private void GraphicDrawer_ParentChanged(object sender, EventArgs e)
        {
            if (this.ParentForm != null)
            {
                ParentForm.KeyDown += new KeyEventHandler(GraphicDrawer_KeyDown);
                ParentForm.KeyUp += new KeyEventHandler(GraphicDrawer_KeyUp);
            }
        }

        private void GraphicDrawer_KeyDown(object sender, KeyEventArgs e)
        {
            // сначала проверяем. если ничего не нарисовано, то забить и не обрабатывать.
            if (!somethingIsDrawn || e.Handled) 
                return;

            if (e.KeyCode == Keys.ControlKey)
            {
                ctrlButtonPressed = true;
                return;
            }

            // а здесь уменьшаем масштаб, если юзер нажал Ctrl+"-"
            // и увеличиваем если Ctrl+"+".
            // в два раза.

            if ((e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus) && ctrlButtonPressed)
            {
                double xMed = (xMax + xMin) / 2;
                double yMed = (yMax + yMin) / 2;

                double additionX = Math.Abs((xMax - xMed) * keyboardScaleFactor);
                double additionY = Math.Abs((yMax - yMed) * keyboardScaleFactor);

                xMin = xMed - additionX;
                xMax = xMed + additionX;

                yMin = yMed - additionY;
                yMax = yMed + additionY;

                setTextBoxesWithValues();
                buttonGraph_Click(this, Do_Not_Check_Correctness);
            }
            else if ((e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus) && ctrlButtonPressed)
            {
                double xMed = (xMax + xMin) / 2;
                double yMed = (yMax + yMin) / 2;

                double additionX = Math.Abs((xMax - xMed) / keyboardScaleFactor);
                double additionY = Math.Abs((yMax - yMed) / keyboardScaleFactor);

                xMin = xMed - additionX;
                xMax = xMed + additionX;

                yMin = yMed - additionY;
                yMax = yMed + additionY;

                setTextBoxesWithValues();
                buttonGraph_Click(this, Do_Not_Check_Correctness);
            }

            e.Handled = true;
        }

        private void GraphicDrawer_KeyUp(object sender, KeyEventArgs e)
        {
            if (!somethingIsDrawn || e.Handled)
                return;

            if (e.KeyData == Keys.ControlKey)
                ctrlButtonPressed = false;

            e.Handled = true;
        }

        // ---------------------

        private void pictureBoxWindow_MouseDown(object sender, MouseEventArgs e)
        {
            // если ничего не нарисовано - забить и не обрабатывать ничего
            if (!somethingIsDrawn)
                return;

            deltaX = deltaY = 0;

            // будем перемещаться по координатной системе
            if (e.Button == MouseButtons.Left && !ctrlButtonPressed && ctf.pointInBound(e.Location))
            {
                Cursor.Current = Cursors.NoMove2D;
                firstPosition = Cursor.Position;

                moveOperation = true;
            }

            // будем перемещаться по графику высокого разрешения
            else if (e.Button == MouseButtons.Left && ctrlButtonPressed)
            {
                if (drawnImage.Height > pictureBoxWindow.Height || drawnImage.Width > pictureBoxWindow.Height)
                {
                    Cursor.Current = Cursors.SizeAll;
                    firstPosition = Cursor.Position;

                    hoverOperation = true;
                }
            }

            // будем выделять регион для масштабирования
            else if (e.Button == MouseButtons.Right)
            {
                // todo - "притягивать к сетке"

                Cursor.Current = Cursors.UpArrow;
                firstPosition = ctf.getInBoundPoint(e.Location);   // в молоко нельзя попадать, надо внутреннюю точку

                // создаем "прямоугольник выделения"

                rectangle = new RectangleDecal(new Rectangle(firstPosition, new Size()), multiGrapherPens1.getAxisColor(), Color.FromArgb(100, multiGrapherPens1.getBackgroundColor().invert()));
                this.decals.Add(rectangle);

                scaleOperation = true;
            }   
        }

        private void pictureBoxWindow_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Cross;

            if (moveOperation || hoverOperation)
            {
                moveOperation = false;
                hoverOperation = false;

                firstPosition = Cursor.Position;
                deltaX = deltaY = 0;
            }
            else if (scaleOperation)
            {
                scaleOperation = false;

                Cursor.Current = Cursors.Cross;

                firstPosition = e.Location;
                deltaX = deltaY = 0;

                this.decals.Remove(rectangle);      // удаляем декаль

                // самое интересное - масштабируем по выделенному квадратику.

                // todo - "притягивать к сетке"

                // прямоугольник не должен быть пустым
                if (rectangle.rectangle.Width * rectangle.rectangle.Height > 0)
                {
                    textBoxFromX.Text = ctf.transformImageXtoFunctionX(rectangle.rectangle.Left).ToString();
                    textBoxToX.Text = ctf.transformImageXtoFunctionX(rectangle.rectangle.Right).ToString();

                    textBoxFromY.Text = ctf.transformImageYToFunctionY(rectangle.rectangle.Bottom).ToString();
                    textBoxToY.Text = ctf.transformImageYToFunctionY(rectangle.rectangle.Top).ToString();
                }

                buttonGraph_Click(this, EventArgs.Empty);
            }
        }
        
        private void pictureBoxWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveOperation)
            {
                deltaX = (firstPosition.X - Cursor.Position.X);
                deltaY = (Cursor.Position.Y - firstPosition.Y);

                // меняем диапазон построения

                /*
                xMin = xMin + ctf.transformPixelXRangeToFunctionXRange(deltaX);
                xMax = xMax + ctf.transformPixelXRangeToFunctionXRange(deltaX);
                yMin = yMin + ctf.transformPixelYRangeToFunctionYRange(deltaY);
                yMax = yMax + ctf.transformPixelYRangeToFunctionYRange(deltaY);
                */
                
                xMin = xMin + ctf.TransformerAxisX.transformRangeLengthImageToFunction(deltaX);
                xMax = xMax + ctf.TransformerAxisX.transformRangeLengthImageToFunction(deltaX);
                yMin = yMin + ctf.TransformerAxisY.transformRangeLengthImageToFunction(deltaY);
                yMax = yMax + ctf.TransformerAxisY.transformRangeLengthImageToFunction(deltaY);

                textBoxFromX.Text = xMin.ToString();
                textBoxToX.Text = xMax.ToString();
                textBoxFromY.Text = yMin.ToString();
                textBoxToY.Text = yMax.ToString();

                // восстанофил

                if (Math.Abs(deltaX) > 1 || Math.Abs(deltaY) > 1)
                {
                    firstPosition = Cursor.Position;
                    deltaX = deltaY = 0;
                }

                // перестройка

                buttonGraph_Click(this, Do_Not_Check_Correctness);
            }
            else if (hoverOperation)
            {
                deltaX = (Cursor.Position.X - firstPosition.X);
                deltaY = (Cursor.Position.Y - firstPosition.Y);

                // со знаком минус - потому что движемся в противоположную сторону.

                xImageCoordinate = xImageCoordinate - deltaX;
                yImageCoordinate = yImageCoordinate - deltaY;

                if (Math.Abs(deltaX) > 1 || Math.Abs(deltaY) > 1)
                {
                    firstPosition = Cursor.Position;
                    deltaX = deltaY = 0;
                }

                ShowImageInPictureBox();
            }
            else if (scaleOperation)
            {
                Point newLocation = ctf.getInBoundPoint(e.Location);

                deltaX = (newLocation.X - firstPosition.X);
                deltaY = (newLocation.Y - firstPosition.Y);

                Point upperLeft = new Point(deltaX < 0 ? (firstPosition.X + deltaX) : firstPosition.X, deltaY < 0 ? firstPosition.Y + deltaY : firstPosition.Y);

                if (deltaX < 0) deltaX = -deltaX;
                if (deltaY < 0) deltaY = -deltaY;

                if (ctf.transformImageXtoFunctionX(upperLeft.X + deltaX) > xMax)
                    deltaX = (int)Math.Round(ctf.transformFunctionXToPixelX(xMax));

                if (upperLeft.Y + deltaY > pictureBoxWindow.Height)
                    deltaY = (int)Math.Round(ctf.transformFunctionYToPixelY(yMax));

                rectangle.rectangle = new Rectangle(upperLeft, new Size(deltaX, deltaY));

                pictureBoxWindow.Refresh();
            }
        }
    
        #endregion

        #region Functionality

        public void fillFunctionRectangle(Color color, int alpha, double x1, double y1, double width, double height)
        {
            if (!somethingIsDrawn) return;            
            PointF point = (PointF)ctf.transformFunctionToPixel(x1, y1);
            
            fillRectangle(color, alpha, new RectangleF(
                point, 
                new SizeF(
                    (float)(width / ctf.TransformerAxisX.ScaleFactor), 
                    (float)(height / ctf.TransformerAxisY.ScaleFactor))));

            this.Refresh();
        }

        public void fillRectangle(Color color, int alpha, RectangleF region)
        {
            if (!somethingIsDrawn) return;

            Graphics gr = Graphics.FromImage(drawnImage);
            
            Color transp = Color.FromArgb(alpha, color);
            Brush br = new SolidBrush(transp);

            gr.FillRectangle(br, region);
            this.Refresh();
        }

        #endregion

    // ---------------- Graph button clicking dummies

        private void numericUpDownShift_ValueChanged(object sender, EventArgs e)
        {
            buttonGraph_Click(this, EventArgs.Empty);
        }

        private void numericUpDownEX_ValueChanged(object sender, EventArgs e)
        {
            buttonGraph_Click(this, EventArgs.Empty);
        }

        private void numericUpDownEX2_ValueChanged(object sender, EventArgs e)
        {
            buttonGraph_Click(this, EventArgs.Empty);
        }

        private void numericUpDownParts_ValueChanged(object sender, EventArgs e)
        {
            buttonGraph_Click(this, EventArgs.Empty);
        }

        private void numericUpDownCW_ValueChanged(object sender, EventArgs e)
        {
            buttonGraph_Click(this, EventArgs.Empty);
        }

        private void numericUpDownAW_ValueChanged(object sender, EventArgs e)
        {
            buttonGraph_Click(this, EventArgs.Empty);
        }

        // ---------------------------------------------------------------------
        // ------------------------- paint event - should be redrawn -----------
        // ---------------------------------------------------------------------

        private void GraphicDrawer_Resize(object sender, EventArgs e)
        {
            buttonGraph_Click(this, EventArgs.Empty);
        }

        private void pictureBoxWindow_Paint(object sender, PaintEventArgs e)
        {
            if (pictureBoxWindow.Image != null)
                e.Graphics.drawDecals(this.decals);    // now draw all the decals
        }

        // ---------------------------------------------------------------------
        // ------------------------- API ---------------------------------------
        // ---------------------------------------------------------------------

        public List<IDecal> Decals
        {
            get { return decals; }
            set { decals = value; }
        }

        // ---------------------------------------------------------------------
        // ------------------------- Special eventargs -------------------------
        // ---------------------------------------------------------------------

        /// <summary>
        /// Used to avoid parsing text values while graphing.
        /// </summary>
        private static EventArgs_NoCheck Do_Not_Check_Correctness = new EventArgs_NoCheck();

        private class EventArgs_NoCheck : EventArgs
        { }
    }

}

#endif