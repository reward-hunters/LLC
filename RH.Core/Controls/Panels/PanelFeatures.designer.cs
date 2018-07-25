﻿using System.ComponentModel;
using System.Windows.Forms;
using RH.Core.Controls.TrackBar;

namespace RH.Core.Controls.Panels
{
    partial class PanelFeatures
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.trackAge = new RH.Core.Controls.TrackBar.TrackBarEx();
            this.trackFat = new RH.Core.Controls.TrackBar.TrackBarEx();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelSmooth = new System.Windows.Forms.Label();
            this.trackBarSmooth = new RH.Core.Controls.TrackBar.TrackBarEx();
            this.SuspendLayout();
            // 
            // btnUndo
            // 
            this.btnUndo.BackColor = System.Drawing.SystemColors.Control;
            this.btnUndo.Location = new System.Drawing.Point(173, 14);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(63, 23);
            this.btnUndo.TabIndex = 5;
            this.btnUndo.Tag = "2";
            this.btnUndo.Text = "Undo";
            this.btnUndo.UseVisualStyleBackColor = false;
            this.btnUndo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnUndo_MouseDown);
            this.btnUndo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnUndo_MouseUp);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.Control;
            this.btnDelete.Location = new System.Drawing.Point(93, 14);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(63, 23);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Tag = "2";
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnDelete_MouseDown);
            this.btnDelete.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnDelete_MouseUp);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.Control;
            this.btnSave.Location = new System.Drawing.Point(13, 14);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(63, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Tag = "2";
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnSave_MouseDown);
            this.btnSave.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnSave_MouseUp);
            // 
            // trackAge
            // 
            this.trackAge.BackColor = System.Drawing.Color.Transparent;
            this.trackAge.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.trackAge.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackAge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.trackAge.IndentHeight = 6;
            this.trackAge.Location = new System.Drawing.Point(309, 0);
            this.trackAge.Maximum = 80;
            this.trackAge.Minimum = 20;
            this.trackAge.Name = "trackAge";
            this.trackAge.Size = new System.Drawing.Size(212, 47);
            this.trackAge.TabIndex = 6;
            this.trackAge.TextTickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackAge.TickColor = System.Drawing.Color.Gray;
            this.trackAge.TickFrequency = 10;
            this.trackAge.TickHeight = 4;
            this.trackAge.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackAge.TrackerColor = System.Drawing.Color.Silver;
            this.trackAge.TrackerSize = new System.Drawing.Size(16, 16);
            this.trackAge.TrackLineColor = System.Drawing.Color.DimGray;
            this.trackAge.TrackLineHeight = 3;
            this.trackAge.Value = 20;
            this.trackAge.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackAge_MouseUp);
            // 
            // trackFat
            // 
            this.trackFat.BackColor = System.Drawing.Color.Transparent;
            this.trackFat.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.trackFat.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackFat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.trackFat.IndentHeight = 6;
            this.trackFat.Location = new System.Drawing.Point(601, 2);
            this.trackFat.Maximum = 30;
            this.trackFat.Minimum = -30;
            this.trackFat.Name = "trackFat";
            this.trackFat.Size = new System.Drawing.Size(212, 47);
            this.trackFat.TabIndex = 7;
            this.trackFat.TextTickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackFat.TickColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(146)))), ((int)(((byte)(148)))));
            this.trackFat.TickFrequency = 10;
            this.trackFat.TickHeight = 4;
            this.trackFat.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackFat.TrackerColor = System.Drawing.Color.Silver;
            this.trackFat.TrackerSize = new System.Drawing.Size(16, 16);
            this.trackFat.TrackLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(93)))), ((int)(((byte)(90)))));
            this.trackFat.TrackLineHeight = 3;
            this.trackFat.Value = 0;
            this.trackFat.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackWeight_MouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label1.Location = new System.Drawing.Point(268, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "Age:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label2.Location = new System.Drawing.Point(540, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "Weight:";
            // 
            // labelSmooth
            // 
            this.labelSmooth.AutoSize = true;
            this.labelSmooth.BackColor = System.Drawing.Color.Transparent;
            this.labelSmooth.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelSmooth.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.labelSmooth.Location = new System.Drawing.Point(836, 21);
            this.labelSmooth.Name = "labelSmooth";
            this.labelSmooth.Size = new System.Drawing.Size(60, 15);
            this.labelSmooth.TabIndex = 11;
            this.labelSmooth.Text = "Smooth;";
            // 
            // trackBarSmooth
            // 
            this.trackBarSmooth.BackColor = System.Drawing.Color.Transparent;
            this.trackBarSmooth.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.trackBarSmooth.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackBarSmooth.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.trackBarSmooth.IndentHeight = 6;
            this.trackBarSmooth.Location = new System.Drawing.Point(902, -1);
            this.trackBarSmooth.Maximum = 100;
            this.trackBarSmooth.Minimum = 0;
            this.trackBarSmooth.Name = "trackBarSmooth";
            this.trackBarSmooth.Size = new System.Drawing.Size(212, 47);
            this.trackBarSmooth.TabIndex = 10;
            this.trackBarSmooth.TextTickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBarSmooth.TickColor = System.Drawing.Color.Gray;
            this.trackBarSmooth.TickFrequency = 20;
            this.trackBarSmooth.TickHeight = 4;
            this.trackBarSmooth.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBarSmooth.TrackerColor = System.Drawing.Color.Silver;
            this.trackBarSmooth.TrackerSize = new System.Drawing.Size(16, 16);
            this.trackBarSmooth.TrackLineColor = System.Drawing.Color.DimGray;
            this.trackBarSmooth.TrackLineHeight = 3;
            this.trackBarSmooth.Value = 100;
            this.trackBarSmooth.ValueChanged += new RH.Core.Controls.TrackBar.ValueChangedHandler(this.trackBarSmooth_ValueChanged);
            this.trackBarSmooth.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBarSmooth_MouseUp);
            // 
            // PanelFeatures
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::RH.Core.Properties.Resources.menuBackground;
            this.Controls.Add(this.labelSmooth);
            this.Controls.Add(this.trackBarSmooth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackFat);
            this.Controls.Add(this.trackAge);
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnSave);
            this.Name = "PanelFeatures";
            this.Size = new System.Drawing.Size(1267, 49);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnUndo;
        private Button btnDelete;
        private Button btnSave;
        private Label label1;
        private Label label2;
        private TrackBarEx trackAge;
        private TrackBarEx trackFat;
        private TrackBarEx trackBarSmooth;
        public Label labelSmooth;
    }
}
