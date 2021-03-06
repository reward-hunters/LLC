﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using RH.Core.Helpers;
using RH.Core.IO;

namespace RH.Core.Controls.Tutorials.OneClick
{
    public partial class frmChildTutorial : Form
    {
        public frmChildTutorial()
        {
            InitializeComponent();
            linkLabel1.Text = UserConfig.ByName("Tutorials")["Links", "Profile", "http://youtu.be/Olc7oeQUmWk"];
            Text = ProgramCore.ProgramCaption;
            linkLabel1.BackColor = Color.FromArgb(211, 211, 211);

            var filePath = FolderEx.GetTutorialImagePath("TutChild_OneClick");
            if (!string.IsNullOrEmpty(filePath))
                pictureBox1.ImageLocation = filePath;
        }

        private void frmProfileTutorial_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;            // this cancels the close event.
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var link = UserConfig.ByName("Tutorials")["Links", "Child", "http://youtu.be/Olc7oeQUmWk"];
            Process.Start(link);
        }

        private void cbShow_CheckedChanged(object sender, EventArgs e)
        {
            UserConfig.ByName("Options")["Tutorials", "Child"] = cbShow.Checked ? "0" : "1";
        }
    }
}
