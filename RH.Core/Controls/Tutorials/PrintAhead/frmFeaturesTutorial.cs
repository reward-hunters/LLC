using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using RH.Core.Helpers;
using RH.Core.IO;

namespace RH.Core.Controls.Tutorials.PrintAhead
{
    public partial class frmFeaturesTutorial : FormEx
    {
        public frmFeaturesTutorial()
        {
            InitializeComponent();
            linkLabel1.Text = UserConfig.ByName("Tutorials")["Links", "Features", "https://youtu.be/_hADE739X9w"];
            Text = ProgramCore.ProgramCaption;

            var directoryPath = Path.Combine(Application.StartupPath, "Tutorials");
            var filePath = Path.Combine(directoryPath, "TutFeatures.jpg");
            if (File.Exists(filePath))
                BackgroundImage = Image.FromFile(filePath);
        }

        private void frmStartTutorial_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;            // this cancels the close event.
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var link = UserConfig.ByName("Tutorials")["Links", "Features", "https://youtu.be/_hADE739X9w"];
            Process.Start(link);
        }

        private void cbShow_CheckedChanged(object sender, EventArgs e)
        {
            UserConfig.ByName("Options")["Tutorials", "Features"] = cbShow.Checked ? "0" : "1";
        }
    }
}
