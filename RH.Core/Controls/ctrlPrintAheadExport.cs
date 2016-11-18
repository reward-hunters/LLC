using System;
using System.Windows.Forms;
using RH.Core.Helpers;
using RH.Core.IO;

namespace RH.Core.Controls
{
    public partial class ctrlPrintAheadExport : UserControlEx
    {
        #region Var

        private IntPtr handle;

        public string ModelName
        {
            get { return textModelName.Text; }
        }
        public string Path
        {
            get
            {
                return textExportFolder.Text;
            }
        }


        #endregion

        public ctrlPrintAheadExport(IntPtr handle)
        {
            InitializeComponent();

            this.handle = handle;
        }

        private void UpdateApply()
        {
            btnApply.Enabled = !string.IsNullOrEmpty(textExportFolder.Text) && !string.IsNullOrEmpty(textModelName.Text);

            if (textExportFolder.Text == ProgramCore.Project.ProjectPath)
            {
                MessageBox.Show("Can't export file to project directory.", "Warning");
                btnApply.Enabled = false;
                return;
            }
        }

        private void btnOpenFolderDlg_Click(object sender, EventArgs e)
        {
            using (var ofd = new FolderDialogEx())
            {
                if (ofd.ShowDialog(handle) != DialogResult.OK)
                {
                    textExportFolder.Text = string.Empty;
                    UpdateApply();
                    return;
                }
                textExportFolder.Text = ofd.SelectedFolder[0];
                UpdateApply();
            }
        }
        private void textModelName_TextChanged(object sender, EventArgs e)
        {
            UpdateApply();
        }
    }
}
