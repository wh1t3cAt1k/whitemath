using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace whiteMath.Graphers.Components
{
    public partial class MultiGrapherPens : UserControl
    {
        private enum Rainbow
        {
            Red, Green, DarkBlue, Orange, DarkGreen, Blue, DarkOrange, Violet 
        }

        private Color[] curveColors;
        private Color axisColor;
        private Color bgColor;

        public event EventHandler ColorValueChanged;

        /// <summary>
        /// Returns the user-selected color for the curve with specified index in multigrapher list.
        /// </summary>
        /// <param name="MultiGrapherIndex"></param>
        /// <returns></returns>
        public Color getCurveColor(int multiGrapherIndex)
        {
            return curveColors[multiGrapherIndex];
        }

        /// <summary>
        /// Returns the user-selected color for the curve.
        /// </summary>
        /// <param name="MultiGrapherIndex"></param>
        /// <returns></returns>
        public Color getCurveColor()
        {
            return curveColors[comboBoxGraphers.SelectedIndex];
        }

        /// <summary>
        /// Returns the color array for the multigrapher.
        /// </summary>
        /// <returns></returns>
        public Color[] getCurveColors()
        {
            return curveColors.ToArray();
        }

        /// <summary>
        /// Returns the user-selected color for the axis.
        /// </summary>
        /// <returns></returns>
        public Color getAxisColor()
        {
            return axisColor;
        }

        public Color getBackgroundColor()
        {
            return bgColor;
        }

        public MultiGrapherPens()
        {
            InitializeComponent();

            this.axisColor = Color.Black;
            this.bgColor = Color.White;

            buttonChooseAC.BackColor = axisColor;
            buttonChooseBC.BackColor = bgColor;
        }

        /// <summary>
        /// Set grapher!
        /// </summary>
        /// <param name="grapher"></param>
        public void setGrapher(AbstractGrapher grapher)
        {
            if (grapher is MultiGrapher)
            {
                AbstractGrapher[] list = (grapher as MultiGrapher).getGrapherList();

                curveColors = new Color[list.Length];

                for (int i = 0; i < list.Length; i++)
                {
                    comboBoxGraphers.Items.Add(list[i].Name);
                    curveColors[i] = Color.FromName(((Rainbow)(i % 8)).ToString());
                }

                comboBoxGraphers.SelectedIndex = 0;
            }
            else
            {
                curveColors = new Color[1] { Color.Blue };

                comboBoxGraphers.Items.Add(grapher.Name);
                comboBoxGraphers.Enabled = false;
                comboBoxGraphers.SelectedIndex = 0;
            }
        }

        private void buttonChooseCC_Click(object sender, EventArgs e)
        {
            ColorDialog picker = new ColorDialog();

            if (picker.ShowDialog() != DialogResult.OK)
                return;

            curveColors[comboBoxGraphers.SelectedIndex] = picker.Color;
            buttonChooseCC.BackColor = picker.Color;

            if (ColorValueChanged != null)
                ColorValueChanged.Invoke(this, EventArgs.Empty);
        }

        private void buttonChooseAC_Click(object sender, EventArgs e)
        {
            ColorDialog picker = new ColorDialog();

            if (picker.ShowDialog() != DialogResult.OK)
                return;

            this.axisColor = picker.Color;
            buttonChooseAC.BackColor = picker.Color;

            if (ColorValueChanged != null)
                ColorValueChanged.Invoke(this, EventArgs.Empty);
        }

        private void buttonChooseBC_Click(object sender, EventArgs e)
        {
            ColorDialog picker = new ColorDialog();

            if (picker.ShowDialog() != DialogResult.OK)
                return;

            this.bgColor = picker.Color;
            buttonChooseBC.BackColor = picker.Color;

            if (ColorValueChanged != null)
                ColorValueChanged.Invoke(this, EventArgs.Empty);
        }

        private void comboBoxGraphers_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonChooseCC.BackColor = curveColors[comboBoxGraphers.SelectedIndex];
        }

    }
}
