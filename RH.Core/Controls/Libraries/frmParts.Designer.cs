using System.ComponentModel;
using System.Windows.Forms;

namespace RH.Core.Controls.Libraries
{
    partial class frmParts
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
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Node0");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Node1");
            this.labelEmpty = new System.Windows.Forms.Label();
            this.tlParts = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnLoadLibrary = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBarSize = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.trackBarXSize = new System.Windows.Forms.TrackBar();
            this.trackBarYSize = new System.Windows.Forms.TrackBar();
            this.trackBarZSize = new System.Windows.Forms.TrackBar();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarXSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarYSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarZSize)).BeginInit();
            this.SuspendLayout();
            // 
            // labelEmpty
            // 
            this.labelEmpty.AutoSize = true;
            this.labelEmpty.BackColor = System.Drawing.Color.White;
            this.labelEmpty.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelEmpty.Location = new System.Drawing.Point(12, 9);
            this.labelEmpty.Name = "labelEmpty";
            this.labelEmpty.Size = new System.Drawing.Size(217, 15);
            this.labelEmpty.TabIndex = 1;
            this.labelEmpty.Text = "There are no items to show in this view.";
            // 
            // tlParts
            // 
            this.tlParts.CheckBoxes = true;
            this.tlParts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlParts.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tlParts.Location = new System.Drawing.Point(0, 0);
            this.tlParts.Name = "tlParts";
            treeNode9.Name = "Node0";
            treeNode9.Text = "Node0";
            treeNode10.Name = "Node1";
            treeNode10.Text = "Node1";
            this.tlParts.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode9,
            treeNode10});
            this.tlParts.ShowLines = false;
            this.tlParts.ShowRootLines = false;
            this.tlParts.Size = new System.Drawing.Size(220, 259);
            this.tlParts.TabIndex = 2;
            this.tlParts.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tlParts_AfterCheck);
            this.tlParts.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tlParts_AfterSelect);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.trackBarZSize);
            this.panel1.Controls.Add(this.trackBarYSize);
            this.panel1.Controls.Add(this.trackBarXSize);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btnLoadLibrary);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.trackBarSize);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(220, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(223, 259);
            this.panel1.TabIndex = 3;
            // 
            // btnLoadLibrary
            // 
            this.btnLoadLibrary.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnLoadLibrary.Location = new System.Drawing.Point(2, 224);
            this.btnLoadLibrary.Name = "btnLoadLibrary";
            this.btnLoadLibrary.Size = new System.Drawing.Size(87, 30);
            this.btnLoadLibrary.TabIndex = 13;
            this.btnLoadLibrary.Text = "Load library";
            this.btnLoadLibrary.UseVisualStyleBackColor = true;
            this.btnLoadLibrary.Click += new System.EventHandler(this.btnLoadLibrary_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(5, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Scale";
            // 
            // trackBarSize
            // 
            this.trackBarSize.Location = new System.Drawing.Point(20, 33);
            this.trackBarSize.Maximum = 100;
            this.trackBarSize.Minimum = 1;
            this.trackBarSize.Name = "trackBarSize";
            this.trackBarSize.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarSize.Size = new System.Drawing.Size(45, 163);
            this.trackBarSize.TabIndex = 0;
            this.trackBarSize.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarSize.Value = 50;
            this.trackBarSize.Scroll += new System.EventHandler(this.trackBarSize_Scroll);
            this.trackBarSize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trackBarSize_MouseDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(50, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "X Scale";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(107, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Y Scale";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(164, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Z Scale";
            // 
            // trackBarXSize
            // 
            this.trackBarXSize.Location = new System.Drawing.Point(71, 33);
            this.trackBarXSize.Maximum = 100;
            this.trackBarXSize.Minimum = 1;
            this.trackBarXSize.Name = "trackBarXSize";
            this.trackBarXSize.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarXSize.Size = new System.Drawing.Size(45, 163);
            this.trackBarXSize.TabIndex = 17;
            this.trackBarXSize.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarXSize.Value = 50;
            this.trackBarXSize.Scroll += new System.EventHandler(this.trackBarXSize_Scroll);
            this.trackBarXSize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trackBarSize_MouseDown);
            // 
            // trackBarYSize
            // 
            this.trackBarYSize.Location = new System.Drawing.Point(122, 33);
            this.trackBarYSize.Maximum = 100;
            this.trackBarYSize.Minimum = 1;
            this.trackBarYSize.Name = "trackBarYSize";
            this.trackBarYSize.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarYSize.Size = new System.Drawing.Size(45, 163);
            this.trackBarYSize.TabIndex = 18;
            this.trackBarYSize.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarYSize.Value = 50;
            this.trackBarYSize.Scroll += new System.EventHandler(this.trackBarYSize_Scroll);
            this.trackBarYSize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trackBarSize_MouseDown);
            // 
            // trackBarZSize
            // 
            this.trackBarZSize.Location = new System.Drawing.Point(178, 33);
            this.trackBarZSize.Maximum = 100;
            this.trackBarZSize.Minimum = 1;
            this.trackBarZSize.Name = "trackBarZSize";
            this.trackBarZSize.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarZSize.Size = new System.Drawing.Size(45, 163);
            this.trackBarZSize.TabIndex = 19;
            this.trackBarZSize.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarZSize.Value = 50;
            this.trackBarZSize.Scroll += new System.EventHandler(this.trackBarZSize_Scroll);
            this.trackBarZSize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trackBarSize_MouseDown);
            // 
            // frmParts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 259);
            this.Controls.Add(this.tlParts);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelEmpty);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmParts";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Parts library";
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.frmParts_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmParts_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarXSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarYSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarZSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label labelEmpty;
        private TreeView tlParts;
        private Panel panel1;
        private Label label1;
        private System.Windows.Forms.TrackBar trackBarSize;
        private Button btnLoadLibrary;
        private System.Windows.Forms.TrackBar trackBarZSize;
        private System.Windows.Forms.TrackBar trackBarYSize;
        private System.Windows.Forms.TrackBar trackBarXSize;
        private Label label4;
        private Label label3;
        private Label label2;
    }
}