using System;
using System.Windows.Forms;
using OpenTK;
using RH.Core.Helpers;
using RH.MeshUtils.Helpers;

namespace RH.Core.Controls.Libraries
{
    public partial class frmFreeHand : FormEx
    {
        public float Radius
        {
            get
            {
                return trackRadius.Value * 0.05f + 0.5f;
            }
        }
        public ShapeCoefType CoefType
        {
            get
            {
                if (rbHandBrush1.Checked)
                    return ShapeCoefType.Grade4;
                if (rbHandBrush2.Checked)
                    return ShapeCoefType.Qubed;
                if (rbHandBrush3.Checked)
                    return ShapeCoefType.Squared;
                return ShapeCoefType.SquarRoot;
            }
        }
        public bool UseMirror
        {
            get
            {
                return cbMirror.Checked;
            }
        }

        public frmFreeHand()
        {
            InitializeComponent();

             Sizeble = false;
        }

        private void frmFreeHand_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;            // this cancels the close event.
            ProgramCore.MainForm.panelFront.DisableShape();
        
        }

        private void trackRadius_MouseUp(object sender, MouseEventArgs e)
        {
            ProgramCore.Project.RenderMainHelper.HeadShapeController.UpdateRadius(Radius);
            handBrush_CheckedChanged(null, EventArgs.Empty);
        }
        private void handBrush_CheckedChanged(object sender, EventArgs e)
        {
            ProgramCore.Project.RenderMainHelper.HeadShapeController.UpdateCoef(CoefType, Radius);
        }

        private void cbMirror_CheckedChanged(object sender, EventArgs e)
        {
            if (IsUpdating)
                return;

            ProgramCore.Project.RenderMainHelper.HeadShapeController.StartShaping(new Vector3(ProgramCore.MainForm.ctrlRenderControl.HeadShapeP.X, ProgramCore.MainForm.ctrlRenderControl.HeadShapeP.Y, 0.0f),
                                                                                    ProgramCore.MainForm.ctrlRenderControl.camera.ViewMatrix, UseMirror, Radius, CoefType);
        }
    }
}
