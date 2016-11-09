using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using RH.Core.Helpers;
using RH.Core.IO;

namespace RH.Core.Controls.Tutorials.OneClick
{
    public partial class frmShapedotsTutorial : FormEx
    {
        public frmShapedotsTutorial()
        {
            InitializeComponent();
            linkLabel1.Text = UserConfig.ByName("Tutorials")["Links", "Shapedots", "http://youtu.be/pIlrJUByJj8"];
            Text = ProgramCore.ProgramCaption;

            var directoryPath = Path.Combine(Application.StartupPath, "Tutorials");
            var filePath = Path.Combine(directoryPath, "TutShapedots.jpg");
            if (File.Exists(filePath))
                BackgroundImage = Image.FromFile(filePath);
        }

        private void frmShapedotsTutorial_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;            // this cancels the close event.
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var link = UserConfig.ByName("Tutorials")["Links", "Shapedots", "http://youtu.be/pIlrJUByJj8"];
            Process.Start(link);
        }

        private void cbShow_CheckedChanged(object sender, EventArgs e)
        {
            UserConfig.ByName("Options")["Tutorials", "Shapedots"] = cbShow.Checked ? "0" : "1";
        }
    }
}
