namespace RH.Core.Controls.Libraries
{
    partial class frmFaceAge
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnFlipRight = new System.Windows.Forms.Button();
            this.btnFlipLeft = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPhotoshop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.trackAge = new RH.Core.Controls.TrackBar.TrackBarEx();
            this.label2 = new System.Windows.Forms.Label();
            this.trackFat = new RH.Core.Controls.TrackBar.TrackBarEx();
            this.labelSmile = new System.Windows.Forms.Label();
            this.trackBarSmile = new RH.Core.Controls.TrackBar.TrackBarEx();
            this.SuspendLayout();
            // 
            // btnFlipRight
            // 
            this.btnFlipRight.Image = global::RH.Core.Properties.Resources.btnToLeftNormal;
            this.btnFlipRight.Location = new System.Drawing.Point(55, 172);
            this.btnFlipRight.Name = "btnFlipRight";
            this.btnFlipRight.Size = new System.Drawing.Size(30, 30);
            this.btnFlipRight.TabIndex = 13;
            this.btnFlipRight.Tag = "2";
            this.btnFlipRight.UseVisualStyleBackColor = true;
            this.btnFlipRight.Click += new System.EventHandler(this.btnFlipRight_Click);
            // 
            // btnFlipLeft
            // 
            this.btnFlipLeft.Image = global::RH.Core.Properties.Resources.btnToRightNormal;
            this.btnFlipLeft.Location = new System.Drawing.Point(15, 172);
            this.btnFlipLeft.Name = "btnFlipLeft";
            this.btnFlipLeft.Size = new System.Drawing.Size(30, 30);
            this.btnFlipLeft.TabIndex = 12;
            this.btnFlipLeft.Tag = "2";
            this.btnFlipLeft.UseVisualStyleBackColor = true;
            this.btnFlipLeft.Click += new System.EventHandler(this.btnFlipLeft_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.Control;
            this.btnClose.Location = new System.Drawing.Point(120, 176);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(73, 23);
            this.btnClose.TabIndex = 14;
            this.btnClose.Tag = "2";
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnPhotoshop
            // 
            this.btnPhotoshop.BackColor = System.Drawing.SystemColors.Control;
            this.btnPhotoshop.Location = new System.Drawing.Point(199, 176);
            this.btnPhotoshop.Name = "btnPhotoshop";
            this.btnPhotoshop.Size = new System.Drawing.Size(73, 23);
            this.btnPhotoshop.TabIndex = 15;
            this.btnPhotoshop.Tag = "2";
            this.btnPhotoshop.Text = "Photoshop";
            this.btnPhotoshop.UseVisualStyleBackColor = false;
            this.btnPhotoshop.Click += new System.EventHandler(this.btnPhotoshop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(6, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 15);
            this.label1.TabIndex = 17;
            this.label1.Text = "Age:";
            // 
            // trackAge
            // 
            this.trackAge.BackColor = System.Drawing.Color.Transparent;
            this.trackAge.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.trackAge.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackAge.ForeColor = System.Drawing.Color.Black;
            this.trackAge.IndentHeight = 6;
            this.trackAge.Location = new System.Drawing.Point(65, 7);
            this.trackAge.Maximum = 80;
            this.trackAge.Minimum = 20;
            this.trackAge.Name = "trackAge";
            this.trackAge.Size = new System.Drawing.Size(212, 47);
            this.trackAge.TabIndex = 16;
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(6, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 15);
            this.label2.TabIndex = 19;
            this.label2.Text = "Weight:";
            // 
            // trackFat
            // 
            this.trackFat.BackColor = System.Drawing.Color.Transparent;
            this.trackFat.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.trackFat.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackFat.ForeColor = System.Drawing.Color.Black;
            this.trackFat.IndentHeight = 6;
            this.trackFat.Location = new System.Drawing.Point(67, 60);
            this.trackFat.Maximum = 30;
            this.trackFat.Minimum = -30;
            this.trackFat.Name = "trackFat";
            this.trackFat.Size = new System.Drawing.Size(212, 47);
            this.trackFat.TabIndex = 18;
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
            this.trackFat.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackFat_MouseUp);
            // 
            // labelSmile
            // 
            this.labelSmile.AutoSize = true;
            this.labelSmile.BackColor = System.Drawing.Color.Transparent;
            this.labelSmile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelSmile.ForeColor = System.Drawing.Color.Black;
            this.labelSmile.Location = new System.Drawing.Point(6, 132);
            this.labelSmile.Name = "labelSmile";
            this.labelSmile.Size = new System.Drawing.Size(48, 15);
            this.labelSmile.TabIndex = 22;
            this.labelSmile.Text = "Smile:";
            // 
            // trackBarSmile
            // 
            this.trackBarSmile.BackColor = System.Drawing.Color.Transparent;
            this.trackBarSmile.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.trackBarSmile.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackBarSmile.ForeColor = System.Drawing.Color.Black;
            this.trackBarSmile.IndentHeight = 6;
            this.trackBarSmile.Location = new System.Drawing.Point(67, 113);
            this.trackBarSmile.Maximum = 100;
            this.trackBarSmile.Minimum = 0;
            this.trackBarSmile.Name = "trackBarSmile";
            this.trackBarSmile.Size = new System.Drawing.Size(212, 47);
            this.trackBarSmile.TabIndex = 21;
            this.trackBarSmile.TextTickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBarSmile.TickColor = System.Drawing.Color.Gray;
            this.trackBarSmile.TickFrequency = 20;
            this.trackBarSmile.TickHeight = 4;
            this.trackBarSmile.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBarSmile.TrackerColor = System.Drawing.Color.Silver;
            this.trackBarSmile.TrackerSize = new System.Drawing.Size(16, 16);
            this.trackBarSmile.TrackLineColor = System.Drawing.Color.DimGray;
            this.trackBarSmile.TrackLineHeight = 3;
            this.trackBarSmile.Value = 100;
            this.trackBarSmile.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBarSmile_MouseUp);
            // 
            // frmFaceAge
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 211);
            this.Controls.Add(this.labelSmile);
            this.Controls.Add(this.trackBarSmile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.trackFat);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackAge);
            this.Controls.Add(this.btnPhotoshop);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnFlipRight);
            this.Controls.Add(this.btnFlipLeft);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFaceAge";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FaceAge";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmFaceAge_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFlipRight;
        private System.Windows.Forms.Button btnFlipLeft;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnPhotoshop;
        private System.Windows.Forms.Label label1;
        private TrackBar.TrackBarEx trackAge;
        private System.Windows.Forms.Label label2;
        private TrackBar.TrackBarEx trackFat;
        public System.Windows.Forms.Label labelSmile;
        public TrackBar.TrackBarEx trackBarSmile;
    }
}