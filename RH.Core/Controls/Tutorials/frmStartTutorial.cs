﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using RH.Core.Helpers;
using RH.Core.IO;

namespace RH.Core.Controls.Tutorials
{
    public partial class frmStartTutorial : FormEx
    {
        public frmStartTutorial()
        {
            InitializeComponent();
            linkLabel1.Text = "website"; //UserConfig.ByName("Tutorials")["Links", "Start", "https://printahead.net/wp-content/uploads/2018/09/HeadShop11manual.pdf "];
            Text = ProgramCore.ProgramCaption;
            linkLabel1.BackColor = Color.FromArgb(211, 211, 211);

            var filePath = FolderEx.GetTutorialImagePath("StartTutorial");
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
            var link = "https://printahead.net/wp-content/uploads/2018/09/HeadShop11manual.pdf"; //UserConfig.ByName("Tutorials")["Links", "Start", "https://printahead.net/wp-content/uploads/2018/09/HeadShop11manual.pdf "];
            Process.Start(link);
        }

        private void cbShow_CheckedChanged(object sender, EventArgs e)
        {
            UserConfig.ByName("Options")["Tutorials", "Start"] = cbShow.Checked ? "0" : "1";
        }
    }
}
