using System.ComponentModel;
using System.Windows.Forms;
using RH.Core.Controls.TrackBar;
using RH.ImageListView;

namespace RH.Core.Controls.Libraries
{
    partial class frmPrint
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnColor3DPrint = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.trackSize = new RH.Core.Controls.TrackBar.TrackBarEx();
            this.btn3DPrint = new System.Windows.Forms.Button();
            this.trackBarPose = new System.Windows.Forms.TrackBar();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPose)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnColor3DPrint);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.trackSize);
            this.panel1.Controls.Add(this.btn3DPrint);
            this.panel1.Controls.Add(this.trackBarPose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(3, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(153, 266);
            this.panel1.TabIndex = 0;
            // 
            // btnColor3DPrint
            // 
            this.btnColor3DPrint.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnColor3DPrint.Location = new System.Drawing.Point(20, 234);
            this.btnColor3DPrint.Name = "btnColor3DPrint";
            this.btnColor3DPrint.Size = new System.Drawing.Size(110, 26);
            this.btnColor3DPrint.TabIndex = 15;
            this.btnColor3DPrint.Text = "Color 3D Print";
            this.btnColor3DPrint.UseVisualStyleBackColor = true;
            this.btnColor3DPrint.Click += new System.EventHandler(this.btnColor3DPrint_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 18);
            this.label1.TabIndex = 3;
            this.label1.Text = "Smoothing";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(98, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 18);
            this.label2.TabIndex = 14;
            this.label2.Text = "Size";
            // 
            // trackSize
            // 
            this.trackSize.BackColor = System.Drawing.Color.Transparent;
            this.trackSize.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.trackSize.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackSize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(125)))), ((int)(((byte)(123)))));
            this.trackSize.IndentHeight = 6;
            this.trackSize.Location = new System.Drawing.Point(94, 34);
            this.trackSize.Maximum = 10;
            this.trackSize.Minimum = 1;
            this.trackSize.Name = "trackSize";
            this.trackSize.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackSize.Size = new System.Drawing.Size(47, 151);
            this.trackSize.TabIndex = 13;
            this.trackSize.TickColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(146)))), ((int)(((byte)(148)))));
            this.trackSize.TickHeight = 4;
            this.trackSize.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackSize.TrackerColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(130)))), ((int)(((byte)(198)))));
            this.trackSize.TrackerSize = new System.Drawing.Size(16, 16);
            this.trackSize.TrackLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(93)))), ((int)(((byte)(90)))));
            this.trackSize.TrackLineHeight = 3;
            this.trackSize.Value = 1;
            this.trackSize.Scroll += new System.EventHandler(this.trackSize_Scroll);
            // 
            // btn3DPrint
            // 
            this.btn3DPrint.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn3DPrint.Location = new System.Drawing.Point(20, 202);
            this.btn3DPrint.Name = "btn3DPrint";
            this.btn3DPrint.Size = new System.Drawing.Size(110, 26);
            this.btn3DPrint.TabIndex = 12;
            this.btn3DPrint.Text = "3D Print";
            this.btn3DPrint.UseVisualStyleBackColor = true;
            this.btn3DPrint.Click += new System.EventHandler(this.btn3DPrint_Click);
            // 
            // trackBarPose
            // 
            this.trackBarPose.Location = new System.Drawing.Point(33, 34);
            this.trackBarPose.Maximum = 100;
            this.trackBarPose.Name = "trackBarPose";
            this.trackBarPose.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarPose.Size = new System.Drawing.Size(45, 151);
            this.trackBarPose.TabIndex = 2;
            this.trackBarPose.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarPose.Value = 90;
            this.trackBarPose.Scroll += new System.EventHandler(this.trackBarPose_Scroll);
            // 
            // frmPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(156, 266);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPrint";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Print";
            this.Activated += new System.EventHandler(this.frmStages_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmStages_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPose)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Label label1;
        public System.Windows.Forms.TrackBar trackBarPose;
        private Button btn3DPrint;
        private Label label2;
        public TrackBarEx trackSize;
        private Button btnColor3DPrint;
    }
}