using System;
using whiteMath.Graphers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace whiteMath.Graphers
{
    /// <summary>
    /// Windows form for drawing graphs using Grapher objects.
    /// Suitable for bounded and unbounded graphers.
    /// 
    /// author: Pavel Kabir
    /// revised: 12.07.2010
    /// version: 1.2
    /// </summary>
    public class GraphicDrawerForm : Form
    {
        GraphicDrawer control;

        public GraphicDrawerForm(GraphicDrawer control)
        {
            this.control = control;
            this.Size = control.Size;

            control.Parent = this;
            control.Dock = DockStyle.Fill;

            this.PerformLayout();
            this.Refresh();
        }

        // -------------------------------- Required variables ----------------

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // GraphicDrawer
            // 
            this.ClientSize = new System.Drawing.Size(370, 262);
            this.Name = "GraphicDrawer";
            this.Text = "GraphicDrawer";
            this.ResumeLayout(false);

        }
    }
}
