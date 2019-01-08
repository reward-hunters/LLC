using System;
using System.Windows.Forms;
using RH.Core.Render;
using RH.Core.Controls.Panels;
using RH.Core.Helpers;
using System.IO;
using RH.Core.IO;
using System.Drawing;

namespace RH.Core.Controls.Libraries
{
    public partial class frmFaceAge : FormEx
    {
        public frmFaceAge()
        {
            InitializeComponent();
        }

        private void btnFlipLeft_Click(object sender, EventArgs e)
        {
            if (btnFlipLeft.Tag.ToString() == "2")
            {
                btnFlipLeft.Tag = "1";
                btnFlipRight.Tag = "2";

                btnFlipLeft.Image = Properties.Resources.btnToRightPressed;
                btnFlipRight.Image = Properties.Resources.btnToLeftNormal;

                switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
                {
                    case Mode.HeadLine:
                        ProgramCore.Project.RenderMainHelper.headMeshesController.Mirror(true, 0);
                        ProgramCore.Project.ShapeFlip = FlipType.LeftToRight;

                        ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2.ClearSelection();
                        ProgramCore.Project.RenderMainHelper.headController.ShapeDots.ClearSelection();
                        ProgramCore.MainForm.ctrlTemplateImage.RectTransformMode = false;
                        break;
                    case Mode.None:
                        ProgramCore.MainForm.ctrlRenderControl.LeftToRightReflection = true;
                        ProgramCore.MainForm.ctrlRenderControl.ApplySmoothedTextures();
                        break;
                }
            }
            else
            {
                btnFlipLeft.Tag = "2";
                btnFlipLeft.Image = Properties.Resources.btnToRightNormal;

                switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
                {
                    case Mode.HeadLine:
                        ProgramCore.Project.RenderMainHelper.headMeshesController.UndoMirror();
                        ProgramCore.Project.ShapeFlip = FlipType.None;
                        break;
                    case Mode.None:
                        ProgramCore.MainForm.ctrlRenderControl.LeftToRightReflection = null;
                        ProgramCore.MainForm.ctrlRenderControl.ApplySmoothedTextures();
                        break;
                }
            }
        }

        private void btnFlipRight_Click(object sender, EventArgs e)
        {
            if (btnFlipRight.Tag.ToString() == "2")
            {
                btnFlipRight.Tag = "1";
                btnFlipLeft.Tag = "2";

                btnFlipRight.Image = Properties.Resources.btnToLeftPressed;
                btnFlipLeft.Image = Properties.Resources.btnToRightNormal;

                switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
                {
                    case Mode.HeadLine:
                        ProgramCore.Project.RenderMainHelper.headMeshesController.Mirror(false, 0);
                        ProgramCore.Project.ShapeFlip = FlipType.RightToLeft;

                        ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2.ClearSelection();
                        ProgramCore.Project.RenderMainHelper.headController.ShapeDots.ClearSelection();
                        ProgramCore.MainForm.ctrlTemplateImage.RectTransformMode = false;
                        break;
                    case Mode.None:
                        ProgramCore.MainForm.ctrlRenderControl.LeftToRightReflection = false;
                        ProgramCore.MainForm.ctrlRenderControl.ApplySmoothedTextures();
                        break;
                }

            }
            else
            {
                btnFlipRight.Tag = "2";
                btnFlipRight.Image = Properties.Resources.btnToLeftNormal;

                switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
                {
                    case Mode.HeadLine:
                        ProgramCore.Project.RenderMainHelper.headMeshesController.UndoMirror();
                        ProgramCore.Project.ShapeFlip = FlipType.None;
                        break;

                    case Mode.None:
                        ProgramCore.MainForm.ctrlRenderControl.LeftToRightReflection = null;
                        ProgramCore.MainForm.ctrlRenderControl.ApplySmoothedTextures();
                        break;
                }

            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
            Application.Exit();
        }
        private void frmFaceAge_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void trackAge_MouseUp(object sender, MouseEventArgs e)
        {
            var delta = trackAge.Value == trackAge.Minimum ? 0 : trackAge.Value / (trackAge.Maximum * 1f);
            PanelFeatures.UpdateAge(delta);
        }

        private void trackFat_MouseUp(object sender, MouseEventArgs e)
        {
            var delta = trackFat.Value == 0 ? 0 : trackFat.Value / (trackFat.Maximum * 1f);
            PanelFeatures.UpdateWeight(delta);
        }

        private void trackBarSmile_MouseUp(object sender, MouseEventArgs e)
        {
            var delta = trackBarSmile.Value == 0 ? 0 : trackBarSmile.Value / (trackBarSmile.Maximum * 1f);
            PanelFeatures.UpdateSmile(delta);
        }

        private void btnPhotoshop_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "FaceAge");
            FolderEx.CreateDirectory(fileName);
            fileName = Path.Combine(fileName, "tempFaceAge.png");

            var templateImage = UserConfig.AppDataDir;
            templateImage = Path.Combine(templateImage, "faceAgeTempImage.jpg");
            var bmp = new Bitmap(templateImage);

            ProgramCore.MainForm.ctrlRenderControl.SaveToPng(fileName, bmp.Width, bmp.Height);
            MessageBox.Show(@"Image successfully exported!", @"Done", MessageBoxButtons.OK);
            Application.Exit();
        }
    }
}
