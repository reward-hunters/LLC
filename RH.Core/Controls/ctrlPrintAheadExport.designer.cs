using System.ComponentModel;
using System.Windows.Forms;

namespace RH.Core.Controls
{
    partial class ctrlPrintAheadExport
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
            this.btnOpenFolderDlg = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textExportFolder = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textModelName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOpenFolderDlg
            // 
            this.btnOpenFolderDlg.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnOpenFolderDlg.Location = new System.Drawing.Point(420, 65);
            this.btnOpenFolderDlg.Name = "btnOpenFolderDlg";
            this.btnOpenFolderDlg.Size = new System.Drawing.Size(33, 24);
            this.btnOpenFolderDlg.TabIndex = 24;
            this.btnOpenFolderDlg.Text = "...";
            this.btnOpenFolderDlg.UseVisualStyleBackColor = true;
            this.btnOpenFolderDlg.Click += new System.EventHandler(this.btnOpenFolderDlg_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(17, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 17);
            this.label4.TabIndex = 22;
            this.label4.Text = "Export folder";
            // 
            // textExportFolder
            // 
            this.textExportFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.textExportFolder.Location = new System.Drawing.Point(117, 65);
            this.textExportFolder.Name = "textExportFolder";
            this.textExportFolder.ReadOnly = true;
            this.textExportFolder.Size = new System.Drawing.Size(297, 24);
            this.textExportFolder.TabIndex = 23;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(316, 17);
            this.label1.TabIndex = 19;
            this.label1.Text = "Select export folder and type your model name.";
            // 
            // textModelName
            // 
            this.textModelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.textModelName.Location = new System.Drawing.Point(117, 35);
            this.textModelName.Name = "textModelName";
            this.textModelName.Size = new System.Drawing.Size(297, 24);
            this.textModelName.TabIndex = 21;
            this.textModelName.TextChanged += new System.EventHandler(this.textModelName_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(17, 39);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 17);
            this.label5.TabIndex = 20;
            this.label5.Text = "Model name:";
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Enabled = false;
            this.btnApply.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnApply.Location = new System.Drawing.Point(300, 95);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(104, 35);
            this.btnApply.TabIndex = 8;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            // 
            // ctrlPrintAheadExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnOpenFolderDlg);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textExportFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textModelName);
            this.Controls.Add(this.label5);
            this.Name = "ctrlPrintAheadExport";
            this.Size = new System.Drawing.Size(468, 103);
            this.ResumeLayout(false);
            this.PerformLayout();

        }        

        #endregion
        private Button btnOpenFolderDlg;
        private Label label4;
        private TextBox textExportFolder;
        private Label label1;
        private TextBox textModelName;
        private Label label5;
        public Button btnApply;
    }
}