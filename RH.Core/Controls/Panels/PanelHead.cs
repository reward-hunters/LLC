using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using RH.Core.Controls.Tutorials.OneClick;
using RH.Core.Helpers;
using RH.Core.IO;
using RH.Core.Properties;
using RH.Core.Render;
using RH.Core.Render.Controllers;
using RH.MeshUtils.Helpers;

namespace RH.Core.Controls.Panels
{
    public partial class PanelHead : UserControlEx
    {
        #region Var

        public EventHandler OnDelete;
        public EventHandler OnDuplicate;
        public EventHandler OnSave;
        public EventHandler OnUndo;

        public EventHandler OnShapeTool;

        private bool frontTab;

        #endregion

        public PanelHead()
        {
            InitializeComponent();
            frontTab = true;

            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                    btnSave.Visible = false;
                    break;
                default:
                    btnSave.Visible = true;
                    break;
            }


            if (ProgramCore.CurrentProgram == ProgramCore.ProgramMode.HeadShop_OneClick)
                btnShapeTool.Visible = btnPolyLine.Visible = false;

            if (ProgramCore.Project != null)
                ResetButtons();

            ReInitializeControl(frontTab);
        }

        private void ReInitializeControl(bool isFrontTab)
        {
            btnAutodots.Visible = isFrontTab;// && ProgramCore.CurrentProgram != ProgramCore.ProgramMode.HeadShop_Rotator;

            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_OneClick:
                    btnPolyLine.Visible = !isFrontTab;
                    break;
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                    btnMirror_Click(null, EventArgs.Empty);
                    break;
            }
        }


        #region Supported void's

        public void ResetModeTools(bool resetMirror = true)
        {
            if (btnMirror.Tag.ToString() == "1" && resetMirror && ProgramCore.CurrentProgram != ProgramCore.ProgramMode.HeadShop_Rotator)
                btnMirror_Click(null, EventArgs.Empty);

            if (btnShapeTool.Tag.ToString() == "1")
                btnShapeTool_Click(null, EventArgs.Empty);

            if (btnLasso.Tag.ToString() == "1")
                btnLasso_Click(null, EventArgs.Empty);

            if (btnAutodots.Tag.ToString() == "1")
                btnAutodots_Click(null, EventArgs.Empty);

            if (btnPolyLine.Tag.ToString() == "1")
                btnPolyLine_Click(null, EventArgs.Empty);

            ResetButtons();
        }
        public void ResetButtons()
        {
            if (ProgramCore.Project.AutodotsUsed)
            {
                if (ProgramCore.CurrentProgram != ProgramCore.ProgramMode.HeadShop_Rotator)
                    btnMirror.Enabled = false;
                btnAutodots.Enabled = true;

                btnLasso.Enabled = false;

                btnDots.Enabled = true;
                btnPolyLine.Enabled = true;
                btnShapeTool.Enabled = true;

                /*         btnFlipLeft.Enabled = false;
                         btnFlipRight.Enabled = false;*/

                if (ProgramCore.CurrentProgram == ProgramCore.ProgramMode.HeadShop_v11)
                {
                    btnProfile.Visible = false;
                    btnPolyLine.Visible = false;
                }

                btnProfile.Enabled = true;
            }
            else
            {
                if (ProgramCore.CurrentProgram != ProgramCore.ProgramMode.HeadShop_Rotator)
                    btnMirror.Enabled = false;
                btnAutodots.Enabled = true;

                btnLasso.Enabled = false;

                btnDots.Enabled = false;
                btnPolyLine.Enabled = false;
                btnShapeTool.Enabled = false;

                /*    btnFlipLeft.Enabled = false;
                    btnFlipRight.Enabled = false;*/

                btnProfile.Enabled = false;
            }
        }

        public void SetPanelLogic()
        {
            switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
            {
                case Mode.HeadShapeFirstTime:
                case Mode.HeadShape:
                    {
                        if (ProgramCore.CurrentProgram != ProgramCore.ProgramMode.HeadShop_Rotator)
                            btnMirror.Enabled = true;
                        btnAutodots.Enabled = false;

                        btnLasso.Enabled = false;

                        btnDots.Enabled = false;
                        btnPolyLine.Enabled = false;
                        btnShapeTool.Enabled = true;

                        /*btnFlipLeft.Enabled = true;
                        btnFlipRight.Enabled = true;*/
                    }
                    break;
                case Mode.HeadAutodots:
                case Mode.HeadAutodotsFirstTime:
                    {
                        if (ProgramCore.CurrentProgram != ProgramCore.ProgramMode.HeadShop_Rotator)
                            btnMirror.Enabled = false;
                        btnAutodots.Enabled = true;

                        btnLasso.Enabled = true;

                        btnDots.Enabled = false;
                        btnPolyLine.Enabled = false;
                        btnShapeTool.Enabled = false;

                        /* btnFlipLeft.Enabled = ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.HeadAutodots;
                         btnFlipRight.Enabled = ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.HeadAutodots;*/
                    }
                    break;
                case Mode.HeadFlipLeft:
                case Mode.HeadFlipRight:
                    break;
                /*        case Mode.HeadShapedots:
                            btnMirror.Enabled = false;
                            btnAutodots.Enabled = false;

                            btnLasso.Enabled = true;

                            btnDots.Enabled = ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.HeadShapedots;
                            btnPolyLine.Enabled = ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.HeadLine;
                            btnShapeTool.Enabled = false;

                            btnFlipLeft.Enabled = true;
                            btnFlipRight.Enabled = true;
                            break;*/
                case Mode.HeadLine:
                    if (ProgramCore.CurrentProgram != ProgramCore.ProgramMode.HeadShop_Rotator)
                        btnMirror.Enabled = false;
                    btnAutodots.Enabled = false;

                    btnLasso.Enabled = false;

                    btnDots.Enabled = false;
                    btnPolyLine.Enabled = ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.HeadLine;
                    btnShapeTool.Enabled = false;

                    /*     btnFlipLeft.Enabled = true;
                         btnFlipRight.Enabled = true;*/
                    break;
                case Mode.HeadAutodotsLassoStart:
                case Mode.HeadAutodotsLassoActive:
                //   case Mode.HeadShapedotsLassoStart:
                //   case Mode.HeadShapedotsLassoActive:
                case Mode.LassoStart:
                case Mode.LassoActive:
                    break;
                default:
                    ResetButtons();
                    break;
            }
        }
        public void DisableShape()
        {
            btnShapeTool_Click(null, EventArgs.Empty);
        }

        private void SetDefaultHeadRotation()
        {
            if (ProgramCore.MainForm.HeadFront)         // поворот головы по дефолту для данных режимов
                ProgramCore.MainForm.ctrlRenderControl.OrtoTop();
            else
                ProgramCore.MainForm.ctrlRenderControl.OrtoRight();
        }
        private void UpdateNormals()
        {
            foreach (var part in ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.Parts)
                part.UpdateNormals();
        }

        /* private void UpdateFlipEnable(FlipType flip)
         {
             switch (flip)
             {
                 case FlipType.LeftToRight:
                     EnableFlipLeftToRight();
                     break;
                 case FlipType.RightToLeft:
                     EnableFlipRightToLeft();
                     break;
             }
         }*/
        /*    private void EnableFlipLeftToRight()
            {
                btnFlipLeft.Tag = "1";
                btnFlipRight.Tag = "2";

                btnFlipLeft.Image = Properties.Resources.btnToRightPressed;
                btnFlipRight.Image = Properties.Resources.btnToRightNormal;
            }
            private void EnableFlipRightToLeft()
            {
                btnFlipRight.Tag = "1";
                btnFlipLeft.Tag = "2";

                btnFlipRight.Image = Properties.Resources.btnToLeftPressed;
                btnFlipLeft.Image = Properties.Resources.btnToRightNormal;
            }
            private void DisableFlip()
            {
                btnFlipRight.Tag = "2";
                btnFlipLeft.Tag = "2";

                btnFlipRight.Image = Properties.Resources.btnToLeftNormal;
                btnFlipLeft.Image = Properties.Resources.btnToRightNormal;
            }*/

        public void UpdateProfileSmoothing(bool isSmoothing)
        {
            lblProfileSmoothing.Visible = trackProfileSmoothing.Visible = isSmoothing;
            labelSmooth.Visible = trackBarSmooth.Visible = !isSmoothing;

            if (isSmoothing)                    // "After finished calculations, the default position of smoothen should be at 50% "
                ProgramCore.MainForm.ctrlTemplateImage.ProfileSmoothing.Smooth(trackProfileSmoothing.Value / (float)(trackProfileSmoothing.Maximum));
        }

        #endregion

        #region Form's event

        public void btnMirror_Click(object sender, EventArgs e)
        {
            if (btnMirror.Tag.ToString() == "2")
            {
                btnMirror.Tag = "1";

                btnMirror.BackColor = SystemColors.ControlDarkDark;
                btnMirror.ForeColor = Color.White;

                switch (ProgramCore.CurrentProgram)
                {
                    case ProgramCore.ProgramMode.HeadShop_Rotator:
                        if (sender != null)
                            ProgramCore.Project.RenderMainHelper.headMeshesController.Mirror(ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.HeadAngle > 0.0f, 0.0f);
                        break;
                    default:
                        {
                            ProgramCore.MainForm.ctrlRenderControl.ToolMirrored = true;
                            ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.HeadShape;
                        }
                        break;
                }
                ProgramCore.Project.MirrorUsed = true;
            }
            else
            {
                btnMirror.Tag = "2";

                btnMirror.BackColor = SystemColors.Control;
                btnMirror.ForeColor = Color.Black;

                switch (ProgramCore.CurrentProgram)
                {
                    case ProgramCore.ProgramMode.HeadShop_Rotator:
                        if (sender != null)
                            ProgramCore.Project.RenderMainHelper.headMeshesController.UndoMirror();
                        break;
                    default:
                        {
                            ProgramCore.MainForm.ctrlRenderControl.ToolMirrored = false;
                            ProgramCore.Project.RenderMainHelper.headController.ClearPointsSelection();
                        }
                        break;
                }
                ProgramCore.Project.MirrorUsed = false;
            }
            SetPanelLogic();
        }

        private void btnSave_MouseDown(object sender, MouseEventArgs e)
        {
            btnSave.BackColor = SystemColors.ControlDarkDark;
            btnSave.ForeColor = Color.White;
        }
        private void btnSave_MouseUp(object sender, MouseEventArgs e)
        {
            btnSave.BackColor = SystemColors.Control;
            btnSave.ForeColor = Color.Black;

            OnSave?.Invoke(this, EventArgs.Empty);
        }

        private void btnDelete_MouseDown(object sender, MouseEventArgs e)
        {
            btnDelete.BackColor = SystemColors.ControlDarkDark;
            btnDelete.ForeColor = Color.White;
        }
        private void btnDelete_MouseUp(object sender, MouseEventArgs e)
        {
            btnDelete.BackColor = SystemColors.Control;
            btnDelete.ForeColor = Color.Black;

            OnDelete?.Invoke(this, EventArgs.Empty);
        }

        private void btnUndo_MouseDown(object sender, MouseEventArgs e)
        {
            btnUndo.BackColor = SystemColors.ControlDarkDark;
            btnUndo.ForeColor = Color.White;
        }
        private void btnUndo_MouseUp(object sender, MouseEventArgs e)
        {
            btnUndo.BackColor = SystemColors.Control;
            btnUndo.ForeColor = Color.Black;

            OnUndo?.Invoke(this, EventArgs.Empty);
        }

        public void btnLasso_Click(object sender, EventArgs e)
        {
            if (btnLasso.Tag.ToString() == "2")
            {
                btnLasso.Tag = "1";

                btnLasso.BackColor = SystemColors.ControlDarkDark;
                btnLasso.ForeColor = Color.White;

                switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
                {
                    case Mode.HeadAutodots:
                    case Mode.HeadAutodotsLassoStart:
                    case Mode.HeadAutodotsLassoActive:
                        break;
                    default:
                        ProgramCore.Project.RenderMainHelper.headController.ClearPointsSelection();
                        break;
                }

                if (ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.HeadAutodots || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.HeadAutodotsFirstTime)
                    ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.HeadAutodotsLassoStart;
                /*  else if (ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.HeadShapedots)
                      ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.HeadShapedotsLassoStart;*/

                SetPanelLogic();
            }
            else
            {
                btnLasso.Tag = "2";

                btnLasso.BackColor = SystemColors.Control;
                btnLasso.ForeColor = Color.Black;

                if (ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.HeadAutodotsLassoStart || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.HeadAutodotsLassoActive)
                {
                    ProgramCore.MainForm.ctrlTemplateImage.SelectAutodotsByLasso();
                    ProgramCore.MainForm.ctrlRenderControl.Mode = ProgramCore.Project.AutodotsUsed ? Mode.HeadAutodots : Mode.HeadAutodotsFirstTime;
                }
                /*     else if (ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.HeadShapedotsLassoStart || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.HeadShapedotsLassoActive)
                     {
                         ProgramCore.MainForm.ctrlTemplateImage.SelectShapedotsByLasso();
                         ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.HeadShapedots;
                     }
                     */
                SetPanelLogic();
            }
        }

        public void btnShapeTool_Click(object sender, EventArgs e)
        {
            if (btnShapeTool.Tag.ToString() == "2")
            {
                btnShapeTool.Tag = "1";
                btnShapeTool.Image = Resources.btnHandPressed1;

                ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.HeadShapeFirstTime;
                ProgramCore.MainForm.EnableRotating();

                ProgramCore.MainForm.frmFreeHand.cbMirror.Enabled = false;
                ProgramCore.MainForm.ctrlTemplateImage.UpdateUserCenterPositions(false, true);

                //        UpdateFlipEnable(ProgramCore.Project.ShapeFlip);
                SetPanelLogic();

                if (OnShapeTool != null && sender != null)
                    OnShapeTool(this, EventArgs.Empty);

                switch (ProgramCore.CurrentProgram)
                {
                    case ProgramCore.ProgramMode.HeadShop_v10_2:
                    case ProgramCore.ProgramMode.HeadShop_v11:
                    case ProgramCore.ProgramMode.HeadShop_Rotator:
                    case ProgramCore.ProgramMode.PrintAhead:
                    case ProgramCore.ProgramMode.PrintAhead_PayPal:
                    case ProgramCore.ProgramMode.PrintAhead_Online:
                        if (frontTab && UserConfig.ByName("Options")["Tutorials", "Freehand", "1"] == "1")
                            ProgramCore.MainForm.frmTutFreehand.ShowDialog(this);
                        break;
                }
            }
            else
            {
                btnShapeTool.Tag = "2";
                btnShapeTool.Image = Resources.btnHandNormal1;

                btnDots.Enabled = btnPolyLine.Enabled = btnShapeTool.Enabled = false;
                ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.None;
                ProgramCore.MainForm.DisableRotating();
                ProgramCore.Project.RenderMainHelper.HeadShapeController.EndShape();

                ProgramCore.MainForm.frmFreeHand.BeginUpdate();
                ProgramCore.MainForm.frmFreeHand.cbMirror.Enabled = false;
                ProgramCore.MainForm.frmFreeHand.cbMirror.Checked = false;
                ProgramCore.MainForm.frmFreeHand.EndUpdate();

                SetPanelLogic();
                SetDefaultHeadRotation();
                UpdateNormals();

                if (OnShapeTool != null && sender != null)
                    OnShapeTool(this, EventArgs.Empty);
            }
        }

        public void btnAutodots_Click(object sender, EventArgs e)
        {
            if (IsUpdating)
                return;

            BeginUpdate();
            if (btnAutodots.Tag.ToString() == "2")
            {
                btnAutodots.Tag = "1";

                SetDefaultHeadRotation();
                ProgramCore.MainForm.DisableRotating();

                btnAutodots.BackColor = SystemColors.ControlDarkDark;
                btnAutodots.ForeColor = Color.White;

                if (ProgramCore.Project.AutodotsUsed)
                    ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.HeadAutodots;
                else
                {
                    ProgramCore.Project.RenderMainHelper.headMeshesController.InitializeTexturing(ctrlRenderControl.autodotsShapeHelper.GetBaseDots(), HeadController.GetIndices());
                    ctrlRenderControl.autodotsShapeHelper.Transform(ProgramCore.Project.RenderMainHelper.headMeshesController.TexturingInfo.Points.ToArray());
                    ProgramCore.Project.RenderMainHelper.headController.StartAutodots();

                    ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.HeadAutodotsFirstTime;

                }
                ProgramCore.MainForm.ctrlTemplateImage.UpdateUserCenterPositions(false, true);


                //         btnFlipLeft.Visible = true;
                //       btnFlipRight.Visible = true;
                //     UpdateFlipEnable(ProgramCore.Project.TextureFlip);
                SetPanelLogic();

                if (frontTab && UserConfig.ByName("Options")["Tutorials", "Autodots", "1"] == "1")
                    ProgramCore.MainForm.frmTutAutodots.ShowDialog(this);
            }
            else
            {
                btnAutodots.Tag = "2";

                btnAutodots.BackColor = SystemColors.Control;
                btnAutodots.ForeColor = Color.Black;

                if (!ProgramCore.Project.AutodotsUsed)
                {
                    //заменть все шейп-точки на новые
                    //    ProgramCore.Project.RenderMainHelper.headController.UpdateAllShapedotsFromAutodots();

                    for (var i = 0; i < ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.Parts.Count; i++)
                    {
                        var part = ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.Parts[i];
                        if (part.Texture == 0)
                        {
                            part.Texture = ProgramCore.MainForm.ctrlRenderControl.HeadTextureId;
                            part.TextureName = ProgramCore.MainForm.ctrlRenderControl.GetTexturePath(part.Texture);
                        }
                    }

                    ProgramCore.Project.AutodotsUsed = true;
                    btnProfile.Enabled = true;
                    ProgramCore.Project.RenderMainHelper.headController.EndAutodots();
                    ProgramCore.MainForm.ctrlRenderControl.ApplySmoothedTextures();

                    /*  for (var i = 0; i < ProgramCore.Project.RenderMainHelper.headController.AutoDots.Count; i++)      // TODO АЛГОРИТМЫ АВТОДОТСОВ 29.05.2018
                      {
                          var p = ProgramCore.Project.RenderMainHelper.headController.AutoDots[i];
                          ctrlRenderControl.autodotsShapeHelper.Transform(p.Value, i); // точка в мировых координатах
                      }*/
                }

                //        btnFlipLeft.Visible = false;
                //        btnFlipRight.Visible = false;
                ProgramCore.MainForm.ctrlRenderControl.CalcReflectedBitmaps();
                ProgramCore.MainForm.ctrlTemplateImage.RectTransformMode = false;
                ProgramCore.MainForm.EnableRotating();
                ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.None;
                SetPanelLogic();
                //      DisableFlip();
            }
            EndUpdate();
        }
        private void btnDots_Click(object sender, EventArgs e)
        {
            /*        if (btnDots.Tag.ToString() == "2")
                    {
                        btnDots.Tag = "1";
                        btnPolyLine.Tag = btnShapeTool.Tag = "2";

                        btnDots.Image = Properties.Resources.btnDotsPressed;
                        btnPolyLine.Image = Properties.Resources.btnPolyLineNormal;
                        btnShapeTool.Image = Properties.Resources.btnHandNormal1;

                        ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.HeadShapedots;
                        ProgramCore.MainForm.ctrlTemplateImage.UpdateUserCenterPositions(false, true);
                        ProgramCore.MainForm.DisableRotating();

                        UpdateFlipEnable(ProgramCore.Project.ShapeFlip);
                        SetPanelLogic();

                        if (frontTab && UserConfig.ByName("Options")["Tutorials", "Shapedots", "1"] == "1")
                            ProgramCore.MainForm.frmTutShapedots.ShowDialog(this);
                    }
                    else
                    {
                        btnDots.Tag = "2";
                        btnDots.Image = Properties.Resources.btnDotsNormal;
                        UpdateNormals();

                        ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.None;
                        ProgramCore.MainForm.EnableRotating();
                        DisableFlip();
                        SetPanelLogic();
                    }*/
        }

        public readonly frmLineToolTutorial frmTutLineTool = new frmLineToolTutorial();
        public void btnPolyLine_Click(object sender, EventArgs e)
        {
            if (btnPolyLine.Tag.ToString() == "2")
            {
                if (ProgramCore.MainForm.HeadProfile && ProgramCore.MainForm.ctrlTemplateImage.ControlPointsMode != ProfileControlPointsMode.None)
                {
                    MessageBox.Show("Set Control Points !", "HeadShop", MessageBoxButtons.OK);
                    return; // значит загрузили картинку, но не назначили ей опорные точки. нельзя ниче делатЬ!
                }

                if (UserConfig.ByName("Options")["Tutorials", "LineTool", "1"] == "1")
                    frmTutLineTool.ShowDialog(this);

                ++ProgramCore.MainForm.ctrlRenderControl.historyController.currentGroup;
                btnPolyLine.Tag = "1";
                btnDots.Tag = btnShapeTool.Tag = "2";

                btnPolyLine.Image = Resources.btnPolyLinePressed;
                btnDots.Image = Resources.btnDotsNormal;
                btnShapeTool.Image = Resources.btnHandNormal1;

                SetDefaultHeadRotation();
                ProgramCore.MainForm.DisableRotating();


                ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.HeadLine;
                ProgramCore.MainForm.ctrlTemplateImage.UpdateUserCenterPositions(false, true);

                //  UpdateFlipEnable(ProgramCore.Project.ShapeFlip);
                SetPanelLogic();
                if (ProgramCore.MainForm.HeadProfile)
                {
                    ProgramCore.MainForm.ctrlRenderControl.HeadLineMode = MeshPartType.ProfileTop;
                    ProgramCore.MainForm.ctrlTemplateImage.UpdateProfileLocation();
                }
            }
            else
            {
                btnPolyLine.Tag = "2";
                btnPolyLine.Image = Resources.btnPolyLineNormal;

                var userPoints = ProgramCore.Project.RenderMainHelper.headController.AllPoints.Select(x => x.Value).ToList();
                if (userPoints.Count >= 3)          // если твое условие - просто не выполняем ничего. но интерфейсно все равно отключить надо!
                {
                    #region История (undo)

                    Dictionary<Guid, MeshUndoInfo> undoInfo;
                    ProgramCore.Project.RenderMainHelper.headMeshesController.GetUndoInfo(out undoInfo);
                    var isProfile = ProgramCore.MainForm.HeadProfile;
                    var teInfo = isProfile ? ctrlRenderControl.autodotsShapeHelper.ShapeProfileInfo : ctrlRenderControl.autodotsShapeHelper.ShapeInfo;
                    var historyElem = new HistoryHeadShapeLines(undoInfo, ProgramCore.Project.RenderMainHelper.headController.Lines, teInfo, isProfile);
                    historyElem.Group = ProgramCore.MainForm.ctrlRenderControl.historyController.currentGroup;
                    ProgramCore.MainForm.ctrlRenderControl.historyController.Add(historyElem);

                    #endregion

                    ProgramCore.MainForm.ctrlTemplateImage.FinishLine();
                    switch (ProgramCore.MainForm.ctrlRenderControl.HeadLineMode)
                    {
                        case MeshPartType.LEye:
                        case MeshPartType.REye:
                            userPoints.RemoveAt(userPoints.Count - 1);
                            var center = ProgramCore.MainForm.ctrlRenderControl.HeadLineMode == MeshPartType.LEye ? ProgramCore.Project.LeftEyeUserCenter : ProgramCore.Project.RightEyeCenter;
                            center = MirroredHeadPoint.UpdateWorldPoint(center);

                            ctrlRenderControl.autodotsShapeHelper.Transform(ProgramCore.MainForm.ctrlRenderControl.HeadLineMode, userPoints, center);
                            break;
                        case MeshPartType.Nose: //тут центр не нужен,сказал воваш.. 
                            ctrlRenderControl.autodotsShapeHelper.Transform(ProgramCore.MainForm.ctrlRenderControl.HeadLineMode, userPoints, Vector2.Zero);
                            break;
                        case MeshPartType.Lip: // ТУТ ЦЕНТР???
                            ctrlRenderControl.autodotsShapeHelper.Transform(ProgramCore.MainForm.ctrlRenderControl.HeadLineMode, userPoints, Vector2.Zero);
                            break;
                        case MeshPartType.Head:
                            userPoints.RemoveAt(userPoints.Count - 1);
                            var leftTop = new Vector2(ProgramCore.Project.LeftEyeUserCenter.X, Math.Max(ProgramCore.Project.LeftEyeUserCenter.Y, ProgramCore.Project.RightEyeUserCenter.Y));
                            var rightBottom = new Vector2(ProgramCore.Project.RightEyeUserCenter.X, ProgramCore.Project.MouthUserCenter.Y);

                            var eyesMouthRect = new RectangleF(leftTop.X, leftTop.Y, rightBottom.X - leftTop.X, rightBottom.Y - leftTop.Y);
                            center = new Vector2(eyesMouthRect.X + eyesMouthRect.Width * 0.5f, eyesMouthRect.Y + eyesMouthRect.Height * 0.5f);
                            center = MirroredHeadPoint.UpdateWorldPoint(center);

                            ctrlRenderControl.autodotsShapeHelper.Transform(ProgramCore.MainForm.ctrlRenderControl.HeadLineMode, userPoints, center);
                            break;
                    }
                }

                ProgramCore.Project.RenderMainHelper.headController.Lines.Clear();

                ProgramCore.MainForm.EnableRotating();
                UpdateNormals();

                ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.None;
                ProgramCore.MainForm.ctrlRenderControl.HeadLineMode = MeshPartType.None;

                ProgramCore.MainForm.ctrlTemplateImage.LineSelectionMode = false;
                ProgramCore.MainForm.ctrlTemplateImage.ResetProfileRects();
                ProgramCore.MainForm.ctrlRenderControl.ProfileFaceRect = RectangleF.Empty;

                //    DisableFlip();
                SetPanelLogic();
            }
        }

        public void btnFlipLeft_Click(object sender, EventArgs e)
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
                        //    case Mode.HeadShapedots:
                        ProgramCore.Project.RenderMainHelper.headMeshesController.Mirror(true, 0);
                        ProgramCore.Project.ShapeFlip = FlipType.LeftToRight;

                        ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2.ClearSelection();
                        ProgramCore.Project.RenderMainHelper.headController.ShapeDots.ClearSelection();
                        ProgramCore.MainForm.ctrlTemplateImage.RectTransformMode = false;
                        break;
                    /* case Mode.HeadAutodotsFirstTime:
                     case Mode.HeadAutodots:
                         ProgramCore.MainForm.ctrlRenderControl.FlipLeft(true);
                         ProgramCore.Project.RenderMainHelper.headMeshesController.Mirror(true, 0);                // добавлено после слияниея с shapedots!

                         ProgramCore.Project.TextureFlip = FlipType.LeftToRight;

                         ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2.ClearSelection();            // добавлено после слияниея с shapedots!
                         ProgramCore.Project.RenderMainHelper.headController.ShapeDots.ClearSelection();
                         ProgramCore.MainForm.ctrlTemplateImage.RectTransformMode = false;
                         break;*/
                    case Mode.None:
                        ProgramCore.MainForm.ctrlRenderControl.LeftToRightReflection = true;
                        ProgramCore.MainForm.ctrlRenderControl.ApplySmoothedTextures();
                        break;
                }

                SetPanelLogic();

                if (frontTab && UserConfig.ByName("Options")["Tutorials", "Mirror", "1"] == "1")
                    ProgramCore.MainForm.frmTutMirror.ShowDialog(this);
            }
            else
            {
                btnFlipLeft.Tag = "2";
                btnFlipLeft.Image = Properties.Resources.btnToRightNormal;

                switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
                {
                    case Mode.HeadLine:
                        //     case Mode.HeadShapedots:
                        ProgramCore.Project.RenderMainHelper.headMeshesController.UndoMirror();
                        ProgramCore.Project.ShapeFlip = FlipType.None;
                        break;
                    /* case Mode.HeadAutodotsFirstTime:
                     case Mode.HeadAutodots:
                         ProgramCore.Project.RenderMainHelper.headMeshesController.UndoMirror();     // после слияние с ShapeDots. Проверить!
                         ProgramCore.MainForm.ctrlRenderControl.FlipLeft(false);
                         ProgramCore.Project.TextureFlip = FlipType.None;
                         break;*/
                    case Mode.None:
                        ProgramCore.MainForm.ctrlRenderControl.LeftToRightReflection = null;
                        ProgramCore.MainForm.ctrlRenderControl.ApplySmoothedTextures();
                        break;
                }
            }
        }
        public void btnFlipRight_Click(object sender, EventArgs e)
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
                        //       case Mode.HeadShapedots:
                        ProgramCore.Project.RenderMainHelper.headMeshesController.Mirror(false, 0);
                        ProgramCore.Project.ShapeFlip = FlipType.RightToLeft;

                        ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2.ClearSelection();
                        ProgramCore.Project.RenderMainHelper.headController.ShapeDots.ClearSelection();
                        ProgramCore.MainForm.ctrlTemplateImage.RectTransformMode = false;
                        break;
                    /* case Mode.HeadAutodotsFirstTime:
                     case Mode.HeadAutodots:
                         ProgramCore.Project.RenderMainHelper.headMeshesController.Mirror(false, 0);       // после слияние с ShapeDots. Проверить!

                         ProgramCore.MainForm.ctrlRenderControl.FlipRight(true);
                         ProgramCore.Project.TextureFlip = FlipType.RightToLeft;

                         ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2.ClearSelection();        // после слияние с ShapeDots. Проверить!
                         ProgramCore.Project.RenderMainHelper.headController.ShapeDots.ClearSelection();
                         ProgramCore.MainForm.ctrlTemplateImage.RectTransformMode = false;
                         break;*/
                    case Mode.None:
                        ProgramCore.MainForm.ctrlRenderControl.LeftToRightReflection = false;
                        ProgramCore.MainForm.ctrlRenderControl.ApplySmoothedTextures();
                        break;
                }

                if (frontTab && UserConfig.ByName("Options")["Tutorials", "Mirror", "1"] == "1")
                    ProgramCore.MainForm.frmTutMirror.ShowDialog(this);
            }
            else
            {
                btnFlipRight.Tag = "2";
                btnFlipRight.Image = Properties.Resources.btnToLeftNormal;

                switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
                {
                    case Mode.HeadLine:
                        //    case Mode.HeadShapedots:
                        ProgramCore.Project.RenderMainHelper.headMeshesController.UndoMirror();
                        ProgramCore.Project.ShapeFlip = FlipType.None;
                        break;
                    /* case Mode.HeadAutodotsFirstTime:
                     case Mode.HeadAutodots:
                         ProgramCore.Project.RenderMainHelper.headMeshesController.UndoMirror();           // после слияние с ShapeDots. Проверить!
                         ProgramCore.MainForm.ctrlRenderControl.FlipRight(false);
                         ProgramCore.Project.TextureFlip = FlipType.None;
                         break;*/
                    case Mode.None:
                        ProgramCore.MainForm.ctrlRenderControl.LeftToRightReflection = null;
                        ProgramCore.MainForm.ctrlRenderControl.ApplySmoothedTextures();
                        break;
                }

                SetPanelLogic();
            }
        }

        #endregion

        private void btnProfile_Click(object sender, EventArgs e)
        {
            if (IsUpdating)
                return;

            if (ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomControlPoints || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomPoints || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomProfilePoints)
                return;

            BeginUpdate();
            if (btnProfile.Tag.ToString() == "2")
            {
                btnProfile.Tag = "1";
                frontTab = false;

                btnProfile.BackColor = SystemColors.ControlDarkDark;
                btnProfile.ForeColor = Color.White;

                ProgramCore.MainForm.ctrlRenderControl.StagesDeactivate(-1);

                ProgramCore.MainForm.HeadMode = true;
                ProgramCore.MainForm.HeadProfile = true;
                ProgramCore.MainForm.HeadFront = ProgramCore.MainForm.HeadFeature = false;
                ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.None;
                ProgramCore.MainForm.ctrlTemplateImage.btnCopyProfileImg.Visible = true;
                ProgramCore.MainForm.ctrlTemplateImage.btnNewProfilePict.Visible = true;

                ProgramCore.MainForm.ctrlRenderControl.camera.ResetCamera(true);
                ProgramCore.MainForm.ctrlRenderControl.OrtoRight();          // поворачиваем морду как надо

                ProgramCore.MainForm.EnableRotating();

                ctrlRenderControl.autodotsShapeHelper.UpdateProfileLines();
                ProgramCore.MainForm.ctrlTemplateImage.InitializeProfileControlPoints();

                if (ProgramCore.Project.ProfileImage == null) // если нет профиля - просто копируем ебало справа
                {
                    ProgramCore.MainForm.ctrlRenderControl.Render();             // специально два раза, чтобы переинициализировать буфер. хз с чем это связано.
                    ProgramCore.MainForm.ctrlRenderControl.Render();
                    ProgramCore.MainForm.ctrlTemplateImage.btnCopyProfileImg_MouseUp(null, null);
                }
                else
                {
                    ProgramCore.MainForm.ctrlTemplateImage.SetTemplateImage(ProgramCore.Project.ProfileImage);
                    ProgramCore.MainForm.ctrlTemplateImage.RecalcProfilePoints();
                }

                if (ProgramCore.Project.CustomHeadNeedProfileSetup)     // произвольная модель. при первом включении нужно произвести первоначальную настройку.
                    ProgramCore.MainForm.ctrlRenderControl.SetCustomProfileSetup();

                ProgramCore.MainForm.ctrlTemplateImage.UpdateProfileLocation();

                if (UserConfig.ByName("Options")["Tutorials", "Profile", "1"] == "1")
                    ProgramCore.MainForm.frmTutProfile.ShowDialog(this);
            }
            else
            {
                btnProfile.Tag = "2";

                btnProfile.BackColor = SystemColors.Control;
                btnProfile.ForeColor = Color.Black;

                frontTab = true;

                ProgramCore.MainForm.HeadMode = true;
                ProgramCore.MainForm.HeadFront = true;
                ProgramCore.MainForm.HeadProfile = ProgramCore.MainForm.HeadFeature = false;

                if (sender != null)         // иначе это во время загрузки программы. и не надо менять мод!
                    ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.None;

                ProgramCore.MainForm.ctrlTemplateImage.btnCopyProfileImg.Visible = false;
                ProgramCore.MainForm.ctrlTemplateImage.btnNewProfilePict.Visible = false;
                lblProfileSmoothing.Visible = trackProfileSmoothing.Visible = false;
                ProgramCore.MainForm.ctrlTemplateImage.isProfileSmoothing = false;
                ProgramCore.MainForm.ctrlRenderControl.OrtoTop();            // поворачиваем морду как надо
                ProgramCore.MainForm.EnableRotating();
                ProgramCore.MainForm.ctrlTemplateImage.SetTemplateImage(ProgramCore.Project.FrontImage);       // возвращаем как было, после изменения профиля лица

                //      if (UserConfig.ByName("Options")["Tutorials", "Autodots", "1"] == "1")
                //            ProgramCore.MainForm.frmTutAutodots.ShowDialog(this);
            }

            ReInitializeControl(frontTab);
            EndUpdate();
        }

        private void trackProfileSmoothing_MouseUp(object sender, MouseEventArgs e)
        {
            ProgramCore.MainForm.ctrlTemplateImage.ProfileSmoothing.Smooth(trackProfileSmoothing.Value / (float)(trackProfileSmoothing.Maximum));
        }

        private void trackBarSmooth_MouseUp(object sender, MouseEventArgs e)
        {
            var value = trackBarSmooth.Value / 100f;
            ProgramCore.MainForm.ctrlRenderControl.headMeshesController.RenderMesh.SetMorphPercent(value);
        }
    }
}
