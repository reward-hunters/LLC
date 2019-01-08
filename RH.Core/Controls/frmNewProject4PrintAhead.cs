using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using RH.Core.Helpers;
using RH.Core.IO;
using RH.Core.Properties;
using RH.Core.Render;
using RH.Core.Render.Helpers;
using RH.MeshUtils;

namespace RH.Core.Controls
{
    public partial class frmNewProject4PrintAhead : Form
    {
        #region Var

        public string OpenProjectPath { get; private set; }

        public ManType ManType
        {
            get
            {
                if (btnMale.Tag.ToString() == "1")
                    return ManType.Male;
                if (btnFemale.Tag.ToString() == "1")
                    return ManType.Female;
                if (btnChild.Tag.ToString() == "1")
                    return ManType.Child;
                return ManType.Custom;
            }
        }
        public GenesisType GenType
        {
            get
            {
                if (rbGenesis3.Checked)
                    return GenesisType.Genesis3;
                if (rbGenesis8.Checked)
                    return GenesisType.Genesis8;

                return GenesisType.Genesis2;
            }
            set
            {
                switch (value)
                {
                    case GenesisType.Genesis3:
                        rbGenesis3.Checked = true;
                        break;
                    case GenesisType.Genesis8:
                        rbGenesis8.Checked = true;
                        break;
                    default:
                        rbGenesis2.Checked = true;
                        break;
                }
            }
        }

        public string CustomModelPath;

        private string templateImage;


        public DialogResult dialogResult = DialogResult.Cancel;
        private readonly bool atStartup;

        private LuxandFaceRecognition fcr;

        public int SelectedSize
        {
            get
            {
                switch (ProgramCore.CurrentProgram)
                {
                    case ProgramCore.ProgramMode.HeadShop_OneClick:
                        return 2048;
                    case ProgramCore.ProgramMode.HeadShop_v11:
                    case ProgramCore.ProgramMode.HeadShop_OneClick_v2:
                    case ProgramCore.ProgramMode.HeadShop_Rotator:
                    case ProgramCore.ProgramMode.FaceAge2_Partial:
                        int videoCardSize;
                        GL.GetInteger(GetPName.MaxTextureSize, out videoCardSize);
                        //   return videoCardSize > 2048 ? 4096 : 2048;
                        return 2048;            // если поставит ьу нас в проге 4096 - то все крашится к хуям. Пусть уж только на экспорте будет.
                    default:
                        return 1024;
                }
            }
        }
        private readonly Pen edgePen;
        private readonly Pen arrowPen;

        public int ImageTemplateWidth;
        public int ImageTemplateHeight;
        public int ImageTemplateOffsetX;
        public int ImageTemplateOffsetY;

        public List<PointF> facialFeaturesTransformed = new List<PointF>();

        private readonly float eWidth;
        public RectangleF TopEdgeTransformed;

        private RectangleF startCenterFaceRect;
        private float centerX(RectangleF rect)
        {
            return rect.Left + rect.Width / 2;
        }

        private bool leftMousePressed;
        private Point startMousePoint;
        private RectangleF startEdgeRect;
        private Vector2 headHandPoint = Vector2.Zero;

        #endregion

        public frmNewProject4PrintAhead(bool atStartup)
        {
            InitializeComponent();
            AllowDrop = true;

            this.atStartup = atStartup;

            edgePen = (Pen)DrawingTools.GreenPen.Clone();
            arrowPen = (Pen)DrawingTools.GreenPen.Clone();
            arrowPen.EndCap = LineCap.ArrowAnchor;

            eWidth = pictureTemplate.Width - 100;
            TopEdgeTransformed = new RectangleF(pictureTemplate.Width / 2f - eWidth / 2f, 50, eWidth, eWidth + 20);

            ShowInTaskbar = atStartup;

            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_OneClick:
                    rbImportObj.Visible = btnChild.Visible = label8.Visible = label11.Visible = labelNotes.Visible = labelNotes1.Visible = false;
                    break;
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                    labelNotes.Visible = labelNotes1.Visible = !ProgramCore.IsFreeVersion;
                    label11.Visible = rbImportObj.Visible = ProgramCore.PluginMode;
                    break;
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.HeadShop_OneClick_v2:
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                case ProgramCore.ProgramMode.FaceAge2_Partial:
                    pictureExample.Visible = label1.Visible = label5.Visible = textTemplateImage.Visible = false;
                    groupBox1.Visible = rbNewProject.Visible = rbOpenProject.Visible = groupBoxOpen.Visible = lblNewProject.Visible = textNewProjectFolder.Visible = btnNewProjectFolder.Visible =
                         textNewProjectName.Visible = false;

                    labelPrintAhead11.Visible = true;
                    labelNotes.Visible = labelNotes1.Visible = false;
                    rbImportObj.Visible = label11.Visible = false;
                    labelHelp.Text = "Browse or drag jpg file here";
                    label8.Visible = btnChild.Visible = false;

                    rbGenesis2.Visible = rbGenesis3.Visible = rbGenesis8.Visible = true;
                    break;
                default:
                    label11.Visible = rbImportObj.Visible = ProgramCore.PluginMode;
                    labelNotes.Visible = labelNotes1.Visible = false;
                    break;
            }
        }
        public void ApplyNewImage(string fileName)
        {
            labelHelp.Visible = false;
            textTemplateImage.Text = fileName;

            templateImage = fileName;
            fcr = new LuxandFaceRecognition();
            if (!fcr.Recognize(ref templateImage, true))
            {
                textTemplateImage.Text = templateImage = string.Empty;
                pictureTemplate.Image = null;
                labelHelp.Visible = true;

                return;                     // это ОЧЕНЬ! важно. потому что мы во время распознавания можем создать обрезанную фотку и использовать ее как основную в проекте.
            }
            if (fcr.IsMale)
                btnMale_Click(null, null);
            else btnFemale_Click(null, null);

            using (var ms = new MemoryStream(File.ReadAllBytes(templateImage))) // Don't use using!!
            {
                var img = (Bitmap)Image.FromStream(ms);
                pictureTemplate.Image = (Bitmap)img.Clone();
                img.Dispose();
            }

            RecalcRealTemplateImagePosition();

            Single distance;
            if (fcr.IsMale)
                distance = facialFeaturesTransformed[22].Y - facialFeaturesTransformed[11].Y;           // раньше использовалась 2 точка.но согласно ТЗ от 27.3.2017 используем теперь эту точку
            else
                distance = facialFeaturesTransformed[2].Y - facialFeaturesTransformed[11].Y;

            TopEdgeTransformed.Y = facialFeaturesTransformed[16].Y + distance;          // определение высоты по алгоритму старикана
            TopEdgeTransformed.Y = TopEdgeTransformed.Y < 0 ? 10 : TopEdgeTransformed.Y;

            RenderTimer.Start();

            if (ProgramCore.PluginMode)
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var dazPath = Path.Combine(appDataPath, @"DAZ 3D\Studio4\temp\FaceShop\", "fs3d.obj");
                if (File.Exists(dazPath))
                {
                    if (ProgramCore.CurrentProgram != ProgramCore.ProgramMode.HeadShop_OneClick && ProgramCore.CurrentProgram != ProgramCore.ProgramMode.HeadShop_v11 && ProgramCore.CurrentProgram != ProgramCore.ProgramMode.HeadShop_OneClick_v2)
                        rbImportObj.Checked = true;

                    CustomModelPath = dazPath;
                }
                else
                    MessageBox.Show(@"Daz model not found.", @"HeadShop", MessageBoxButtons.OK);
            }

            if (Math.Abs(fcr.RotatedAngle) > 25)
                MessageBox.Show("The head rotated more than 20 degrees. Please select an other photo...");
            else
                btnApply.Enabled = true;

        }

        #region Form's event

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (!atStartup)
            {
                if (pictureTemplate.Image == null)
                {
                    MessageBox.Show("Select Template Image !", "HeadShop", MessageBoxButtons.OK);
                    return;
                }
            }

            dialogResult = DialogResult.OK;
            Close();
        }
        private void frmNewProject4PrintAhead_Resize(object sender, EventArgs e)
        {
            RecalcRealTemplateImagePosition();
        }
        private void rbImportObj_CheckedChanged(object sender, EventArgs e)
        {
            /*   if (rbImportObj.Checked)
               {
                   btnFemale.Tag = btnChild.Tag = btnMale.Tag = "2";
                   btnChild.Image = Resources.btnChildGray;
                   btnMale.Image = Resources.btnMaleGray;
                   btnFemale.Image = Resources.btnFemaleGray;*/
            /*
                            if (!ProgramCore.PluginMode)
                            {
                                using (var ofd = new OpenFileDialogEx("Select obj file", "OBJ Files|*.obj"))
                                {
                                    ofd.Multiselect = false;
                                    if (ofd.ShowDialog() != DialogResult.OK)
                                    {
                                        btnMale_Click(this, new EventArgs());
                                        return;
                                    }

                                    //btnNext.Enabled = true;
                                    CustomModelPath = ofd.FileName;
                                }
                            }*/
            //     }
        }

        private void pictureTemplate_Paint(object sender, PaintEventArgs e)
        {
            if (string.IsNullOrEmpty(templateImage))
                return;

            foreach (var point in facialFeaturesTransformed)
                e.Graphics.FillEllipse(DrawingTools.BlueSolidBrush, point.X - 2, point.Y - 2, 4, 4);

            e.Graphics.DrawArc(edgePen, TopEdgeTransformed, 220, 100);
            e.Graphics.DrawLine(arrowPen, centerX(TopEdgeTransformed), TopEdgeTransformed.Top, centerX(TopEdgeTransformed), TopEdgeTransformed.Top + 40);
        }
        private void pictureTemplate_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                leftMousePressed = true;

                headHandPoint.X = (ImageTemplateOffsetX + e.X) / (ImageTemplateWidth * 1f);
                headHandPoint.Y = (ImageTemplateOffsetY + e.Y) / (ImageTemplateHeight * 1f);

                if (e.X >= TopEdgeTransformed.Left && e.X <= TopEdgeTransformed.Right && e.Y >= TopEdgeTransformed.Y && e.Y <= TopEdgeTransformed.Y + 40)
                {
                    currentSelection = Selection.TopEdge;
                    startEdgeRect = TopEdgeTransformed;
                    startMousePoint = new Point(e.X, e.Y);

                    Cursor = ProgramCore.MainForm.GrabbingCursor;
                }

            }
        }
        private void pictureTemplate_MouseMove(object sender, MouseEventArgs e)
        {

            if (startMousePoint == Point.Empty)
                startMousePoint = new Point(e.X, e.Y);

            if (leftMousePressed && currentSelection != Selection.Empty)
            {
                Vector2 newPoint;
                Vector2 delta2;
                newPoint.X = (ImageTemplateOffsetX + e.X) / (ImageTemplateWidth * 1f);
                newPoint.Y = (ImageTemplateOffsetY + e.Y) / (ImageTemplateHeight * 1f);

                delta2 = newPoint - headHandPoint;
                switch (currentSelection)
                {

                    case Selection.TopEdge:
                        TopEdgeTransformed.Y = startEdgeRect.Y + (e.Y - startMousePoint.Y);
                        RecalcRealTemplateImagePosition();
                        break;

                }
            }
            else
            {
                if (e.X >= TopEdgeTransformed.Left && e.X <= TopEdgeTransformed.Right && e.Y >= TopEdgeTransformed.Y && e.Y <= TopEdgeTransformed.Y + 40)
                    Cursor = ProgramCore.MainForm.GrabCursor;
                else
                    Cursor = Cursors.Arrow;
            }

        }
        private void pictureTemplate_MouseUp(object sender, MouseEventArgs e)
        {
            if (leftMousePressed && currentSelection != Selection.Empty)
                RecalcRealTemplateImagePosition();

            startMousePoint = Point.Empty;
            currentSelection = Selection.Empty;
            leftMousePressed = false;

            headHandPoint = Vector2.Zero;
            Cursor = Cursors.Arrow;
        }
        private void pictureTemplate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textTemplateImage.Text))
                return;

            using (var ofd = new OpenFileDialogEx("Select template file", "Image Files|*.jpg;*.png;*.jpeg;*.bmp"))
            {
                ofd.Multiselect = false;
                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    btnApply.Enabled = false;
                    return;
                }

                ApplyNewImage(ofd.FileName);
            }
        }
        private void pictureTemplate_DoubleClick(object sender, EventArgs e)
        {
            textTemplateImage.Text = string.Empty;
            pictureTemplate_Click(sender, e);
        }

        private void btnQuestion_MouseDown(object sender, MouseEventArgs e)
        {
            btnQuestion.Image = Resources.btnQuestionPressed;
        }
        private void btnQuestion_MouseUp(object sender, MouseEventArgs e)
        {
            ProgramCore.MainForm.ShowTutorial();
            btnQuestion.Image = Resources.btnQuestionNormal;
        }
        private void btnPlay_MouseDown(object sender, MouseEventArgs e)
        {
            btnPlay.Image = Resources.btnPlayPressed;
        }
        private void btnPlay_MouseUp(object sender, MouseEventArgs e)
        {
            ProgramCore.MainForm.ShowVideo();
            btnPlay.Image = Resources.btnPlayNormal;
        }
        private void btnInfo_MouseDown(object sender, MouseEventArgs e)
        {
            btnInfo.Image = Resources.btnInfoPressed;
        }
        private void btnInfo_MouseUp(object sender, MouseEventArgs e)
        {
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.HeadShop_OneClick_v2:
                case ProgramCore.ProgramMode.FaceAge2_Partial:
                    ProgramCore.Splash.ShowDialog(this);
                    break;
                default:
                    ProgramCore.MainForm.ShowSiteInfo();
                    break;
            }

            btnInfo.Image = Resources.btnInfoNormal;
        }

        private void btnMale_Click(object sender, EventArgs e)
        {
            if (btnMale.Tag.ToString() == "2")
            {
                btnMale.Tag = "1";
                btnMale.Image = Resources.btnMaleNormal;


                btnChild.Tag = btnFemale.Tag = "2";
                btnChild.Image = Resources.btnChildGray;
                btnFemale.Image = Resources.btnFemaleGray;
                rbImportObj.Checked = false;

            }
        }
        private void btnFemale_Click(object sender, EventArgs e)
        {
            if (btnFemale.Tag.ToString() == "2")
            {
                btnFemale.Tag = "1";
                btnFemale.Image = Resources.btnFemaleNormal;

                btnChild.Tag = btnMale.Tag = "2";
                btnChild.Image = Resources.btnChildGray;
                btnMale.Image = Resources.btnMaleGray;
                rbImportObj.Checked = false;
            }
        }
        private void btnChild_Click(object sender, EventArgs e)
        {
            if (btnChild.Tag.ToString() == "2")
            {
                btnChild.Tag = "1";
                btnChild.Image = Resources.btnChildNormal;

                btnMale.Tag = btnFemale.Tag = "2";
                btnMale.Image = Resources.btnMaleGray;
                btnFemale.Image = Resources.btnFemaleGray;
                rbImportObj.Checked = false;
            }
        }

        #endregion

        public void CreateProject()
        {
            #region Корректируем размер фотки

            var realTemplateImage = templateImage;
            using (var ms = new MemoryStream(File.ReadAllBytes(templateImage))) // Don't use using!!
            {
                var img = (Bitmap)Image.FromStream(ms);
                var max = (float)Math.Max(img.Width, img.Height);
                if (max != SelectedSize)
                {
                    var k = SelectedSize / max;
                    var newImg = ImageEx.ResizeImage(img, new Size((int)Math.Round(img.Width * k), (int)Math.Round((img.Height * k))));

                    templateImage = UserConfig.AppDataDir;
                    FolderEx.CreateDirectory(templateImage);
                    templateImage = Path.Combine(templateImage, "tempProjectImage.jpg");

                    newImg.Save(templateImage, ImageFormat.Jpeg);
                }
            }

            #endregion

            var path = string.Empty;
            if (string.IsNullOrEmpty(textNewProjectFolder.Text))
            {
                path = UserConfig.AppDataDir;
                path = Path.Combine(path, "TempProject");
                FolderEx.CreateDirectory(path, true);
            }
            else
            {
                path = textNewProjectFolder.Text;
            }

            var projectName = string.IsNullOrEmpty(textNewProjectName.Text) ? "HeadShop project" : textNewProjectName.Text;
            ProgramCore.Project = new Project(projectName, path, templateImage, GenType, ManType, CustomModelPath, true, SelectedSize, fcr.IsOpenSmile);
            ProgramCore.Project.RealTemplateImage = realTemplateImage;

            ProgramCore.Project.FacialFeatures = fcr.FacialFeatures;
            ProgramCore.Project.ImageRealPoints = fcr.RealPoints;

            ProgramCore.Project.LoadMeshes();

            var minX = fcr.GetMinX();
            var topPoint = (TopEdgeTransformed.Y - ImageTemplateOffsetY) / ImageTemplateHeight;

            ProgramCore.Project.TopPoint.X = 0.0f;
            ProgramCore.Project.TopPoint.Y = topPoint;

            ProgramCore.Project.FaceRectRelative = new RectangleF(minX, topPoint, fcr.GetMaxX() - minX, fcr.BottomFace.Y - topPoint);
            ProgramCore.Project.MouthCenter = new Vector2(fcr.LeftMouth.X + (fcr.RightMouth.X - fcr.LeftMouth.X) * 0.5f, fcr.LeftMouth.Y + (fcr.RightMouth.Y - fcr.LeftMouth.Y) * 0.5f);

            ProgramCore.Project.LeftEyeCenter = fcr.LeftEyeCenter;
            ProgramCore.Project.RightEyeCenter = fcr.RightEyeCenter;
            ProgramCore.Project.FaceColor = fcr.FaceColor;

            ProgramCore.Project.DetectedLipsPoints.Clear();
            ProgramCore.Project.DetectedNosePoints.Clear();
            ProgramCore.Project.DetectedBottomPoints.Clear();
            ProgramCore.Project.DetectedTopPoints.Clear();

            ProgramCore.Project.DetectedLipsPoints.Add(fcr.FacialFeatures[3]);            // точки рта
            ProgramCore.Project.DetectedLipsPoints.Add(fcr.FacialFeatures[58]);
            ProgramCore.Project.DetectedLipsPoints.Add(fcr.FacialFeatures[55]);
            ProgramCore.Project.DetectedLipsPoints.Add(fcr.FacialFeatures[59]);
            ProgramCore.Project.DetectedLipsPoints.Add(fcr.FacialFeatures[4]);
            ProgramCore.Project.DetectedLipsPoints.Add(fcr.FacialFeatures[57]);
            ProgramCore.Project.DetectedLipsPoints.Add(fcr.FacialFeatures[56]);

            ProgramCore.Project.DetectedLipsPoints.Add(fcr.FacialFeatures[61]);//Центр рта верх
            ProgramCore.Project.DetectedLipsPoints.Add(fcr.FacialFeatures[64]);//Центр рта низ

            ProgramCore.Project.DetectedLipsPoints.Add(fcr.FacialFeatures[60]);  //9
            ProgramCore.Project.DetectedLipsPoints.Add(fcr.FacialFeatures[62]);  //10

            ProgramCore.Project.DetectedLipsPoints.Add(fcr.FacialFeatures[63]);  //11
            ProgramCore.Project.DetectedLipsPoints.Add(fcr.FacialFeatures[65]);  //12


            ProgramCore.Project.DetectedNosePoints.Add(fcr.FacialFeatures[45]);           // точки носа
            ProgramCore.Project.DetectedNosePoints.Add(fcr.FacialFeatures[46]);
            ProgramCore.Project.DetectedNosePoints.Add(fcr.FacialFeatures[2]);
            ProgramCore.Project.DetectedNosePoints.Add(fcr.FacialFeatures[22]);
            ProgramCore.Project.DetectedNosePoints.Add(fcr.FacialFeatures[49]);

            ProgramCore.Project.DetectedLeftEyePoints.Add(fcr.FacialFeatures[23]); //Точки левого глаза
            ProgramCore.Project.DetectedLeftEyePoints.Add(fcr.FacialFeatures[28]);
            ProgramCore.Project.DetectedLeftEyePoints.Add(fcr.FacialFeatures[24]);
            ProgramCore.Project.DetectedLeftEyePoints.Add(fcr.FacialFeatures[27]);

            ProgramCore.Project.DetectedRightEyePoints.Add(fcr.FacialFeatures[25]); //Точки правого глаза
            ProgramCore.Project.DetectedRightEyePoints.Add(fcr.FacialFeatures[32]);
            ProgramCore.Project.DetectedRightEyePoints.Add(fcr.FacialFeatures[26]);
            ProgramCore.Project.DetectedRightEyePoints.Add(fcr.FacialFeatures[31]);

            ProgramCore.Project.DetectedBottomPoints.Add(fcr.FacialFeatures[5]); //точки нижней части лица
            ProgramCore.Project.DetectedBottomPoints.Add(fcr.FacialFeatures[7]);// * 0.75f + fcr.FacialFeatures[9] * 0.25f);
            var p11 = fcr.FacialFeatures[11];
            ProgramCore.Project.DetectedBottomPoints.Add(new Vector3((p11.X + fcr.FacialFeatures[9].X) * 0.5f, p11.Y, fcr.FacialFeatures[9].Z));
            ProgramCore.Project.DetectedBottomPoints.Add(new Vector3((p11.X + fcr.FacialFeatures[10].X) * 0.5f, p11.Y, fcr.FacialFeatures[10].Z));
            ProgramCore.Project.DetectedBottomPoints.Add(fcr.FacialFeatures[8]);// * 0.75f + fcr.FacialFeatures[10] * 0.25f);
            ProgramCore.Project.DetectedBottomPoints.Add(fcr.FacialFeatures[6]);

            ProgramCore.Project.DetectedBottomPoints.Add(fcr.FacialFeatures[66]);
            ProgramCore.Project.DetectedBottomPoints.Add(fcr.FacialFeatures[68]);
            ProgramCore.Project.DetectedBottomPoints.Add(fcr.FacialFeatures[69]);
            ProgramCore.Project.DetectedBottomPoints.Add(fcr.FacialFeatures[67]);

            ProgramCore.Project.DetectedTopPoints.Add(fcr.FacialFeatures[66]);
            ProgramCore.Project.DetectedTopPoints.Add(fcr.FacialFeatures[67]);



            ProgramCore.Project.RotatedAngle = fcr.RotatedAngle;

            var aabb = ProgramCore.MainForm.ctrlRenderControl.InitializeShapedotsHelper(true);         // инициализация точек головы. эта инфа тоже сохранится в проект
                                                                                                       //     ProgramCore.MainForm.ctrlRenderControl.ImportPoints();
            ProgramCore.MainForm.UpdateProjectControls(true, aabb);

            //  ProgramCore.Project.ToStream();
            // ProgramCore.MainForm.ctrlRenderControl.UpdateMeshProportions();

            if (ProgramCore.Project.ManType == ManType.Custom)
            {
                ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.SetCustomControlPoints;
                ProgramCore.MainForm.ctrlRenderControl.InitializeCustomControlSpritesPosition();

                var exampleImgPath = Path.Combine(Application.StartupPath, "Plugin", "ControlBaseDotsExample.jpg");
                using (var ms = new MemoryStream(File.ReadAllBytes(exampleImgPath))) // Don't use using!!
                    ProgramCore.MainForm.ctrlTemplateImage.SetTemplateImage((Bitmap)Image.FromStream(ms), false);          // устанавливаем картинку помощь для юзера
            }
        }

        /// <summary> Пересчитать положение прямоугольника в зависимост от размера картинки на picturetemplate </summary>
        private void RecalcRealTemplateImagePosition()
        {
            var pb = pictureTemplate;
            if (pb.Image == null)
            {
                ImageTemplateWidth = ImageTemplateHeight = 0;
                ImageTemplateOffsetX = ImageTemplateOffsetY = -1;
                return;
            }

            if (pb.Width / (double)pb.Height < pb.Image.Width / (double)pb.Image.Height)
            {
                ImageTemplateWidth = pb.Width;
                ImageTemplateHeight = pb.Image.Height * ImageTemplateWidth / pb.Image.Width;
            }
            else if (pb.Width / (double)pb.Height > pb.Image.Width / (double)pb.Image.Height)
            {
                ImageTemplateHeight = pb.Height;
                ImageTemplateWidth = pb.Image.Width * ImageTemplateHeight / pb.Image.Height;
            }
            else // if ((double)pb.Width / (double)pb.Height == (double)pb.Image.Width / (double)pb.Image.Height)
            {
                ImageTemplateWidth = pb.Width;
                ImageTemplateHeight = pb.Height;
            }

            ImageTemplateOffsetX = (pb.Width - ImageTemplateWidth) / 2;
            ImageTemplateOffsetY = (pb.Height - ImageTemplateHeight) / 2;

            facialFeaturesTransformed.Clear();
            foreach (var point in fcr.FacialFeatures)
            {
                var pointTransformed = new PointF(point.X * ImageTemplateWidth + ImageTemplateOffsetX,
                                          point.Y * ImageTemplateHeight + ImageTemplateOffsetY);
                facialFeaturesTransformed.Add(pointTransformed);
            }

            if (TopEdgeTransformed.Y < 0)
                TopEdgeTransformed.Y = 0;

        }
        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            pictureTemplate.Refresh();
        }

        public enum Selection
        {
            TopEdge,
            BottomEdge,
            Empty
        }
        private Selection currentSelection = Selection.Empty;

        private void btnOpenProject_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "HeadShop files|*.hds";
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                textOpenProject.Text = ofd.FileName;
                OpenProjectPath = ofd.FileName;
                btnApply.Enabled = true;
            }
        }

        private void btnNewProjectFolder_Click(object sender, EventArgs e)
        {
            using (var ofd = new FolderDialogEx())
            {
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                textNewProjectFolder.Text = ofd.SelectedFolder[0];
            }
        }

        private void rbOpenProject_CheckedChanged(object sender, EventArgs e)
        {
            rbNewProject.Checked = !rbOpenProject.Checked;
            groupBoxOpen.Enabled = rbOpenProject.Checked;
            groupBox1.Enabled = pictureTemplate.Enabled = !rbOpenProject.Checked;
        }

        private void rbGenesis2_CheckedChanged(object sender, EventArgs e)
        {
            if (rbGenesis2.Checked)
                rbGenesis3.Checked = rbGenesis8.Checked = false;
        }
        private void rbGenesis3_CheckedChanged(object sender, EventArgs e)
        {
            if (rbGenesis3.Checked)
                rbGenesis2.Checked = rbGenesis8.Checked = false;
        }
        private void rbGenesis8_CheckedChanged(object sender, EventArgs e)
        {
            if (rbGenesis8.Checked)
                rbGenesis2.Checked = rbGenesis3.Checked = false;
        }

        private void frmNewProject4PrintAhead_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void frmNewProject4PrintAhead_DragDrop(object sender, DragEventArgs e)
        {
            int x = this.PointToClient(new Point(e.X, e.Y)).X;
            int y = this.PointToClient(new Point(e.X, e.Y)).Y;
            if (x >= pictureTemplate.Location.X && x <= pictureTemplate.Location.X + pictureTemplate.Width && y >= pictureTemplate.Location.Y && y <= pictureTemplate.Location.Y + pictureTemplate.Height)
            {
                string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                ApplyNewImage(FileList[0]);
            }
        }


    }
}
