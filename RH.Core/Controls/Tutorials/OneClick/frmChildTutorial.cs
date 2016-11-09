using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
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

            var directoryPath = Path.Combine(Application.StartupPath, "Tutorials");
            var filePath = Path.Combine(directoryPath, "TutChild_OneClick.jpg");

            if (File.Exists(filePath))
                BackgroundImage = Image.FromFile(filePath);
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
