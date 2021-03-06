﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using RH.Core.Helpers;
using RH.Core.IO;

namespace RH.Core.Controls.Tutorials.OneClick
{
    public partial class frmAutodotsTutorial : FormEx
    {
        public frmAutodotsTutorial()
        {
            InitializeComponent();
            linkLabel1.Text = UserConfig.ByName("Tutorials")["Links", "Autodots", GetDefaultLink()];
            Text = ProgramCore.ProgramCaption;
            linkLabel1.BackColor = Color.FromArgb(211, 211, 211);

            var directoryPath = Path.Combine(Application.StartupPath, "Tutorials");
            var filePath = string.Empty;
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_OneClick:
                    filePath = FolderEx.GetTutorialImagePath("TutAutodots_OneClick");
                    break;
                default:
                    filePath = FolderEx.GetTutorialImagePath("TutAutodots");
                    break;
            }

            if (!string.IsNullOrEmpty(filePath))
                pictureBox1.ImageLocation = filePath;
        }
        private string GetDefaultLink()
        {
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.HeadShop_OneClick_v2:
                case ProgramCore.ProgramMode.FaceAge2_Partial:
                    return "https://www.youtube.com/watch?v=X-8Gho1YUIc&t=345s";
                default:
                    return "http://youtu.be/JC5z64YP1xA";
            }
        }

        private void frmAutodotsTutorial_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;            // this cancels the close event.
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var link = UserConfig.ByName("Tutorials")["Links", "Autodots", GetDefaultLink()];
            Process.Start(link);
        }

        private void cbShow_CheckedChanged(object sender, EventArgs e)
        {
            UserConfig.ByName("Options")["Tutorials", "Autodots"] = cbShow.Checked ? "0" : "1";
        }
    }
}
