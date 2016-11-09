using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using RH.Core.Helpers;
using RH.Core.IO;

namespace RH.Core.Controls.Tutorials.PrintAhead
{
    public partial class frmShapeTutorial : FormEx
    {
        public frmShapeTutorial()
        {
            InitializeComponent();
            linkLabel1.Text = UserConfig.ByName("Tutorials")["Links", "Shape", "https://www.youtube.com/watch?v=AjG09RGgHvw"];
            Text = ProgramCore.ProgramCaption;

            var directoryPath = Path.Combine(Application.StartupPath, "Tutorials");
            var filePath = Path.Combine(directoryPath, "ShapeTutorial.jpg");
            if (File.Exists(filePath))
                BackgroundImage = Image.FromFile(filePath);
        }

        private void frmShapeTutorial_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;            // this cancels the close event.
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var link = UserConfig.ByName("Tutorials")["Links", "Shape", "https://www.youtube.com/watch?v=AjG09RGgHvw"];
            Process.Start(link);
        }

        private void cbShow_CheckedChanged(object sender, EventArgs e)
        {
            UserConfig.ByName("Options")["Tutorials", "Shape"] = cbShow.Checked ? "0" : "1";
        }
    }
}
