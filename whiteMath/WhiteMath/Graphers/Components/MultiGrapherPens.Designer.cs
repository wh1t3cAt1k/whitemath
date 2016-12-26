#if (!NOWINFORMS)
namespace WhiteMath.Graphers.Components
{
    partial class MultiGrapherPens
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBoxGraphers = new System.Windows.Forms.ComboBox();
            this.buttonChooseAC = new System.Windows.Forms.Button();
            this.buttonChooseCC = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonChooseBC = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxGraphers
            // 
            this.comboBoxGraphers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGraphers.FormattingEnabled = true;
            this.comboBoxGraphers.Location = new System.Drawing.Point(58, 7);
            this.comboBoxGraphers.Name = "comboBoxGraphers";
            this.comboBoxGraphers.Size = new System.Drawing.Size(149, 21);
            this.comboBoxGraphers.TabIndex = 22;
            this.comboBoxGraphers.SelectedIndexChanged += new System.EventHandler(this.comboBoxGraphers_SelectedIndexChanged);
            // 
            // buttonChooseAC
            // 
            this.buttonChooseAC.Location = new System.Drawing.Point(95, 34);
            this.buttonChooseAC.Name = "buttonChooseAC";
            this.buttonChooseAC.Size = new System.Drawing.Size(27, 22);
            this.buttonChooseAC.TabIndex = 21;
            this.buttonChooseAC.UseVisualStyleBackColor = true;
            this.buttonChooseAC.Click += new System.EventHandler(this.buttonChooseAC_Click);
            // 
            // buttonChooseCC
            // 
            this.buttonChooseCC.Location = new System.Drawing.Point(215, 6);
            this.buttonChooseCC.Name = "buttonChooseCC";
            this.buttonChooseCC.Size = new System.Drawing.Size(27, 22);
            this.buttonChooseCC.TabIndex = 20;
            this.buttonChooseCC.UseVisualStyleBackColor = true;
            this.buttonChooseCC.Click += new System.EventHandler(this.buttonChooseCC_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Оси координат:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Кривые:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonChooseBC
            // 
            this.buttonChooseBC.Location = new System.Drawing.Point(215, 34);
            this.buttonChooseBC.Name = "buttonChooseBC";
            this.buttonChooseBC.Size = new System.Drawing.Size(27, 22);
            this.buttonChooseBC.TabIndex = 24;
            this.buttonChooseBC.UseVisualStyleBackColor = true;
            this.buttonChooseBC.Click += new System.EventHandler(this.buttonChooseBC_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(128, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Фон графика:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // MultiGrapherPens
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonChooseBC);
            this.Controls.Add(this.comboBoxGraphers);
            this.Controls.Add(this.buttonChooseAC);
            this.Controls.Add(this.buttonChooseCC);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Name = "MultiGrapherPens";
            this.Size = new System.Drawing.Size(255, 60);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxGraphers;
        private System.Windows.Forms.Button buttonChooseAC;
        private System.Windows.Forms.Button buttonChooseCC;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonChooseBC;
        private System.Windows.Forms.Label label1;

    }
}
#endif