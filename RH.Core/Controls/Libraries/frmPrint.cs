﻿using System;
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
            var temp = 0;
            ProgramCore.MainForm.ctrlRenderControl.PoseMorphing = 
                ProgramCore.MainForm.ctrlRenderControl.pickingController.LoadPartsMorphInfo(currentPose, ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh, ref temp);

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

            ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.MorphScale = trackSize.Value;
            ProgramCore.MainForm.ctrlRenderControl.UpdateCameraPosition(-0.54f);
        }

        private void frmStages_Activated(object sender, EventArgs e)
        {
            ProgramCore.MainForm.ctrlRenderControl.StagesActivate(false);

            BeginUpdate();
            trackSize.Value = (int)ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.MorphScale;
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
                case ProgramCore.ProgramMode.HeadShop_v10_2:
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.FaceAge2_Partial:
                case ProgramCore.ProgramMode.HeadShop_OneClick_v2:
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                case ProgramCore.ProgramMode.PrintAhead:
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                case ProgramCore.ProgramMode.PrintAhead_Online:
                    if (ProgramCore.IsTutorialVisible && UserConfig.ByName("Options")["Tutorials", "3DPrinting", "1"] == "1")
                        ProgramCore.MainForm.frmTut3dPrint.ShowDialog(this);
                    break;
            }

            if (ProgramCore.paypalHelper == null)
                ProgramCore.MainForm.ExportSTL();
            else
                ProgramCore.paypalHelper.MakePayment("5", "STL export", PrintType.STL);
        }
        private void btnColor3DPrint_Click(object sender, EventArgs e)
        {
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_v10_2:
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.FaceAge2_Partial:
                case ProgramCore.ProgramMode.HeadShop_OneClick_v2:
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                case ProgramCore.ProgramMode.PrintAhead:
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                case ProgramCore.ProgramMode.PrintAhead_Online:
                    if (ProgramCore.IsTutorialVisible && UserConfig.ByName("Options")["Tutorials", "3DPrinting", "1"] == "1")
                        ProgramCore.MainForm.frmTut3dPrint.ShowDialog(this);
                    break;
            }

            if (ProgramCore.paypalHelper == null)
                ProgramCore.MainForm.ExportDAE();
            else
                ProgramCore.paypalHelper.MakePayment("8", "DAE export", PrintType.Collada);
        }

        #endregion

    }
}
