﻿using System;
using System.Drawing;
using System.Windows.Forms;
using RH.Core.Helpers;
using RH.Core.IO;

namespace RH.Core.Controls.Tutorials
{
    public partial class frmRetouchTutorial : FormEx
    {
        public frmRetouchTutorial()
        {
            InitializeComponent();
            ///   linkLabel1.Text = UserConfig.ByName("Tutorials")["Links", "Start", "http://youtu.be/JC5z64YP1xA"];
            Text = ProgramCore.ProgramCaption;
            linkLabel1.BackColor = Color.FromArgb(211, 211, 211);
            linkLabel1.Visible = false;

            var filePath = FolderEx.GetTutorialImagePath("RetouchTutorial");
            if (!string.IsNullOrEmpty(filePath))
                pictureBox1.ImageLocation = filePath;
        }

        private void frmStartTutorial_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;            // this cancels the close event.
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
           // var link = UserConfig.ByName("Tutorials")["Links", "Start", "http://youtu.be/JC5z64YP1xA"];
        //    Process.Start(link);
        }

        private void cbShow_CheckedChanged(object sender, EventArgs e)
        {
            UserConfig.ByName("Options")["Tutorials", "Retouch"] = cbShow.Checked ? "0" : "1";
        }
    }
}
