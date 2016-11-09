﻿using System.ComponentModel;
using System.Windows.Forms;

namespace RH.Core.Render
{
    partial class ctrlTemplateImage
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
            this.btnNewProfilePict = new System.Windows.Forms.PictureBox();
            this.pictureTemplate = new System.Windows.Forms.PictureBox();
            this.btnCopyProfileImg = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewProfilePict)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTemplate)).BeginInit();
            this.pictureTemplate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnCopyProfileImg)).BeginInit();
            this.SuspendLayout();
            // 
            // btnNewProfilePict
            // 
            this.btnNewProfilePict.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnNewProfilePict.BackColor = System.Drawing.Color.Transparent;
            this.btnNewProfilePict.BackgroundImage =  global::RH.Core.Properties.Resources.newProfilePictPressed;
            this.btnNewProfilePict.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnNewProfilePict.Image =  global::RH.Core.Properties.Resources.newProfilePictNormal;
            this.btnNewProfilePict.Location = new System.Drawing.Point(3, 218);
            this.btnNewProfilePict.Name = "btnNewProfilePict";
            this.btnNewProfilePict.Size = new System.Drawing.Size(85, 49);
            this.btnNewProfilePict.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnNewProfilePict.TabIndex = 8;
            this.btnNewProfilePict.TabStop = false;
            this.btnNewProfilePict.Visible = false;
            this.btnNewProfilePict.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnNewProfilePict_MouseDown);
            this.btnNewProfilePict.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnNewProfilePict_MouseUp);
            // 
            // pictureTemplate
            // 
            this.pictureTemplate.Controls.Add(this.btnCopyProfileImg);
            this.pictureTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureTemplate.Location = new System.Drawing.Point(0, 0);
            this.pictureTemplate.Name = "pictureTemplate";
            this.pictureTemplate.Size = new System.Drawing.Size(664, 504);
            this.pictureTemplate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureTemplate.TabIndex = 7;
            this.pictureTemplate.TabStop = false;
            this.pictureTemplate.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureTemplate_Paint);
            this.pictureTemplate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureTemplate_MouseDown);
            this.pictureTemplate.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureTemplate_MouseMove);
            this.pictureTemplate.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureTemplate_MouseUp);
            this.pictureTemplate.Resize += new System.EventHandler(this.pictureTemplate_Resize);
            // 
            // btnCopyProfileImg
            // 
            this.btnCopyProfileImg.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnCopyProfileImg.BackColor = System.Drawing.Color.Transparent;
            this.btnCopyProfileImg.BackgroundImage =  global::RH.Core.Properties.Resources.copyArrowPressed;
            this.btnCopyProfileImg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCopyProfileImg.Image = global::RH.Core.Properties.Resources.copyArrowNormal;
            this.btnCopyProfileImg.Location = new System.Drawing.Point(578, 218);
            this.btnCopyProfileImg.Name = "btnCopyProfileImg";
            this.btnCopyProfileImg.Size = new System.Drawing.Size(85, 49);
            this.btnCopyProfileImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnCopyProfileImg.TabIndex = 5;
            this.btnCopyProfileImg.TabStop = false;
            this.btnCopyProfileImg.Visible = false;
            this.btnCopyProfileImg.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCopyProfileImg_MouseDown);
            this.btnCopyProfileImg.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnCopyProfileImg_MouseUp);
            // 
            // ctrlTemplateImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnNewProfilePict);
            this.Controls.Add(this.pictureTemplate);
            this.Name = "ctrlTemplateImage";
            this.Size = new System.Drawing.Size(664, 504);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureTemplate_Paint);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ctrlTemplateImage_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureTemplate_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureTemplate_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureTemplate_MouseUp);
            this.Resize += new System.EventHandler(this.pictureTemplate_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.btnNewProfilePict)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTemplate)).EndInit();
            this.pictureTemplate.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnCopyProfileImg)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public PictureBox btnCopyProfileImg;
        public PictureBox pictureTemplate;
        public PictureBox btnNewProfilePict;
    }
}
