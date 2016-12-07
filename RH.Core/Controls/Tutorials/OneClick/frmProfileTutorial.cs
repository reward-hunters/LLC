using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using RH.Core.Helpers;
using RH.Core.IO;

namespace RH.Core.Controls.Tutorials.OneClick
{
    public partial class frmProfileTutorial : FormEx
    {
        public frmProfileTutorial()
        {
            InitializeComponent();
            linkLabel1.Text = UserConfig.ByName("Tutorials")["Links", "Profile", "http://youtu.be/Olc7oeQUmWk"];
            Text = ProgramCore.ProgramCaption;
            linkLabel1.BackColor = Color.FromArgb(211, 211, 211);

            var directoryPath = Path.Combine(Application.StartupPath, "Tutorials");
            var filePath = string.Empty;
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShopOneClick:
                    filePath = Path.Combine(directoryPath, "TutProfile_OneClick.jpg");
                    break;
                default:
                    filePath = Path.Combine(directoryPath, "TutProfile.jpg");
                    break;
            }

            if (File.Exists(filePath))
                pictureBox1.ImageLocation = filePath;
        }

        private void frmProfileTutorial_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;            // this cancels the close event.
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var link = UserConfig.ByName("Tutorials")["Links", "Profile", "http://youtu.be/Olc7oeQUmWk"];
            Process.Start(link);
        }

        private void cbShow_CheckedChanged(object sender, EventArgs e)
        {
            UserConfig.ByName("Options")["Tutorials", "Profile"] = cbShow.Checked ? "0" : "1";
        }
    }
}
