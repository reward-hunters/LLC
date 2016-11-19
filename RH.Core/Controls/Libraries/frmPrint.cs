using System;
using System.IO;
using System.Windows.Forms;
using RH.Core.Helpers;
using RH.Core.IO;
using RH.ImageListView;

namespace RH.Core.Controls.Libraries
{
    public partial class frmPrint : FormEx
    {
        private string currentPose = string.Empty;

        public frmPrint()
        {
            InitializeComponent();

            if (!float.IsNaN(ProgramCore.Project.MorphingScale))
                trackBarPose.Value = (int)(ProgramCore.Project.MorphingScale * 100);

            Sizeble = false;
        }

        #region Supported void's

        private void SetPose(ImageListViewItem sel)
        {
            var animFileName = Path.GetFileNameWithoutExtension(sel.Text) + ".obj";
            var animPath = Path.Combine(Application.StartupPath, "Stages", "Poses", ManType.Child.GetCaption(), animFileName);

            if (currentPose == animPath)
                return;

            currentPose = animPath;

            ProgramCore.MainForm.ctrlRenderControl.PoseMorphing = ProgramCore.MainForm.ctrlRenderControl.pickingController.LoadPartsMorphInfo(currentPose, ProgramCore.MainForm.ctrlRenderControl.headMeshesController.RenderMesh);

            trackBarPose.Enabled = true;
            trackBarPose.Value = 100;
            SetPosePosition();
        }
        private void SetPosePosition()
        {
            //if(ProgramCore.MainForm.ctrlRenderControl.PoseMorphing == null)
            //    return;
            var delta = trackBarPose.Value / 100f;

            ProgramCore.Project.MorphingScale = delta;      // для сохранения в проект
            //foreach (var m in ProgramCore.MainForm.ctrlRenderControl.PoseMorphing)
            //    m.Value.Delta = delta;

            ProgramCore.MainForm.ctrlRenderControl.DoMorth(delta);
        }

        #endregion

        #region Form's event

        private void trackBarPose_Scroll(object sender, EventArgs e)
        {
            SetPosePosition();
        }
        private void trackSize_Scroll(object sender, EventArgs e)
        {
            if (IsUpdating)
                return;

            ProgramCore.MainForm.ctrlRenderControl.headMeshesController.RenderMesh.MorphScale = trackSize.Value;
            ProgramCore.MainForm.ctrlRenderControl.UpdateCameraPosition(-0.54f);
        }

        private void frmStages_Activated(object sender, EventArgs e)
        {
            ProgramCore.MainForm.ctrlRenderControl.StagesActivate(false);

            BeginUpdate();
            trackSize.Value = (int)ProgramCore.MainForm.ctrlRenderControl.headMeshesController.RenderMesh.MorphScale;
            EndUpdate();
        }
        private void frmStages_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;            // this cancels the close event.
        }

        private void btn3DPrint_Click(object sender, EventArgs e)
        {
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop:
                case ProgramCore.ProgramMode.PrintAhead:
                case ProgramCore.ProgramMode.PrintAheadPayPal:
                    if (UserConfig.ByName("Options")["Tutorials", "3DPrinting", "1"] == "1")
                        ProgramCore.MainForm.frmTut3dPrint.ShowDialog(this);
                    break;
            }

            if (ProgramCore.paypalHelper == null)
                ProgramCore.MainForm.ExportSTL();
            else ProgramCore.paypalHelper.MakePayment("5", "Payment for PrintAhead stl print", frmMain.PrintType.STL);
        }
        private void btnColor3DPrint_Click(object sender, EventArgs e)
        {
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop:
                case ProgramCore.ProgramMode.PrintAhead:
                case ProgramCore.ProgramMode.PrintAheadPayPal:
                    if (UserConfig.ByName("Options")["Tutorials", "3DPrinting", "1"] == "1")
                        ProgramCore.MainForm.frmTut3dPrint.ShowDialog(this);
                    break;
            }

            if (ProgramCore.paypalHelper == null)
                ProgramCore.MainForm.ExportDAE();
            else ProgramCore.paypalHelper.MakePayment("8", "Payment for PrintAhead collada print", frmMain.PrintType.Collada);
        }

        #endregion

    }
}
