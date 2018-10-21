using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using RH.Core.Helpers;
using RH.Core.IO;

namespace RH.Core.Controls.Tutorials.PrintAhead
{
    public partial class frmAccessoryTutorial : FormEx
    {
        public frmAccessoryTutorial()
        {
            InitializeComponent();
            linkLabel1.Text = UserConfig.ByName("Tutorials")["Links", "Accessory", GetDefaultLink()];
            Text = ProgramCore.ProgramCaption;
            linkLabel1.BackColor = Color.FromArgb(211, 211, 211);

            var filePath = FolderEx.GetTutorialImagePath("AccessoryTutorial");
            if (!string.IsNullOrEmpty(filePath))
                pictureBox1.ImageLocation = filePath;
        }
        private string GetDefaultLink()
        {
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.HeadShop_OneClick_v2:
                    return "https://www.youtube.com/watch?v=X-8Gho1YUIc&t=545s";
                default:
                    return "https://youtu.be/UeQljfKlNG8";
            }
        }

        private void frmAccessoryTutorial_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;            // this cancels the close event.
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var link = UserConfig.ByName("Tutorials")["Links", "Accessory", GetDefaultLink()];
            Process.Start(link);
        }

        private void cbShow_CheckedChanged(object sender, EventArgs e)
        {
            UserConfig.ByName("Options")["Tutorials", "Accessory"] = cbShow.Checked ? "0" : "1";
        }
    }
}
