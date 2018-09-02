using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Assimp;
using Ionic.Zip;
using Luxand;
using RH.Core.Controls;
using RH.Core.Controls.Libraries;
using RH.Core.Controls.Panels;
using RH.Core.Controls.Progress;
using RH.Core.Controls.Tutorials;
using RH.Core.Controls.Tutorials.OneClick;
using RH.Core.Controls.Tutorials.PrintAhead;
using RH.Core.Helpers;
using RH.Core.IO;
using RH.Core.Properties;
using RH.Core.Render;
using RH.Core.Render.Helpers;
using RH.Core.Render.Meshes;
using RH.Core.Render.Obj;
using RH.MeshUtils.Data;
using RH.Core.Render.Controllers;
using static RH.Core.ProgramCore;
using System.Threading;

namespace RH.Core
{
    public partial class frmMain : Form
    {
        #region Var

        public readonly MRUManager mruManager = new MRUManager();

        public frmMaterials frmMaterial;
        public frmAccessories frmAccessories;
        public frmPrint frmPrint;
        public frmStages frmStages;     // пока не используется!
        public frmParts frmParts;
        public frmStyles frmStyles;
        public frmFreeHand frmFreeHand;

        private PanelCut panelCut;
        private PanelShape panelShape;
        private PanelLibrary panelAccessories;
        private PanelLibrary panelMaterials;
        private PanelLibrary panelStages;
        private PanelLibrary panelStyles;
        public PanelHead panelFront
        {
            get;
            private set;
        }
        private PanelFeatures panelFeatures;

        private readonly frmStartTutorial frmTutStart = new frmStartTutorial();
        public readonly frmRecognizeTutorial frmTutRecognize = new frmRecognizeTutorial();
        public readonly frmChildTutorial frmTutChild = new frmChildTutorial();

        public readonly frmRetouchTutorial frmTutRetouch = new frmRetouchTutorial();
        private readonly frmAccessoryTutorial frmTutAccessory = new frmAccessoryTutorial();
        private readonly frmCutTutorial frmTutCut = new frmCutTutorial();
        private readonly frmMaterialTutorial frmTutMaterial = new frmMaterialTutorial();
        private readonly frmShapeTutorial frmTutShape = new frmShapeTutorial();
        private readonly frmStageTutorial frmTutStage = new frmStageTutorial();
        private readonly frmStyleTutorial frmTutStyle = new frmStyleTutorial();

        public readonly frmCustomHeadsTutorial frmTutCustomHeads = new frmCustomHeadsTutorial();
        public readonly frmAutodotsTutorial frmTutAutodots = new frmAutodotsTutorial();                 // OneClick
        public readonly frmShapedotsTutorial frmTutShapedots = new frmShapedotsTutorial();
        public readonly frmMirrorTutorial frmTutMirror = new frmMirrorTutorial();
        public readonly frmFreehandTutorial frmTutFreehand = new frmFreehandTutorial();
        public readonly frmProfileTutorial frmTutProfile = new frmProfileTutorial();                     // OneClick
        public readonly frmLineToolTutorial frmTutLineTool = new frmLineToolTutorial();

        public readonly frmFeaturesTutorial frmTutFeatures = new frmFeaturesTutorial();
        public readonly frm3dPrintTutorial frmTut3dPrint = new frm3dPrintTutorial();
        public readonly frmExportTutorial frmTutExport = new frmExportTutorial();

        /// <summary> Флаг, означающий что мы находимся во вкладкам изменения ибала. </summary>
        public bool HeadMode
        {
            get;
            set;
        }

        public bool HeadFront
        {
            get;
            set;
        }
        public bool HeadProfile
        {
            get;
            set;
        }
        public bool HeadFeature
        {
            get;
            set;
        }

        public List<string> PluginUvGroups = new List<string>();

        private readonly string openProjectPath;

        public readonly Cursor GrabCursor;
        public readonly Cursor GrabbingCursor;

        #endregion

        public frmMain(string fn)
        {
            InitializeComponent();

            // Enable double duffering to stop flickering.
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, false);
            SetStyle(ControlStyles.Opaque, false);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            using (var memoryStream = new MemoryStream(Resources.grab))
                GrabCursor = new Cursor(memoryStream);
            using (var memoryStream = new MemoryStream(Resources.grabbing))
                GrabbingCursor = new Cursor(memoryStream);

            KeyPreview = true;
            ProgramCore.ProgressProc += ProgressProc;

            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.PrintAhead:
                    Text = @"PrintAhead";
                    aboutHeadShopProToolStripMenuItem.Text = @"About PrintAhead";
                    panelMenuStage.Image = Resources.btnMenuPrintNormal;
                    openToolStripMenuItem.Visible = saveAsToolStripMenuItem.Visible = saveToolStripMenuItem.Visible = false;
                    childHelpToolStripMenuItem.Visible = false;
                    break;
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                case ProgramCore.ProgramMode.PrintAhead_Online:
                    Text = ProgramCore.CurrentProgram == ProgramCore.ProgramMode.PrintAhead_PayPal ? "PrintAhead 2.0" : "PrintAhead Online";
                    aboutHeadShopProToolStripMenuItem.Text = ProgramCore.CurrentProgram == ProgramCore.ProgramMode.PrintAhead_PayPal ? "About PrintAhead 2.0" : "About PrintAhead Online";
                    ProgramCore.paypalHelper.InitializeCef();
                    panelMenuStage.Image = Resources.btnMenuPrintNormal;
                    openToolStripMenuItem.Visible = saveAsToolStripMenuItem.Visible = saveToolStripMenuItem.Visible = false;
                    childHelpToolStripMenuItem.Visible = false;

                    exportToolStripMenuItem.Visible = exportToolStripMenuItem1.Visible = exportToolStripMenuItem2.Visible = exportToolStripMenuItem3.Visible = false;       // запрет экспорта 
                    saveToolStripMenuItem5.Visible = saveToolStripMenuItem6.Visible = saveToolStripMenuItem8.Visible = toolStripMenuItem1.Visible = false;
                    showManualToolStripMenuItem.Visible = false;

                    panelMenuAccessories.Visible = accessoryTabToolStripMenuItem.Visible = accessoriesHelpToolStripMenuItem1.Visible = true;        // ТЗ PrintAhead 3.0 PRD - вернуть обратно аксессуары
                    exportToolStripMenuItem.Visible = exportToolStripMenuItem1.Visible = exportToolStripMenuItem2.Visible = exportToolStripMenuItem3.Visible = false;       // запрет экспорта 
                    saveToolStripMenuItem5.Visible = saveToolStripMenuItem6.Visible = saveToolStripMenuItem8.Visible = toolStripMenuItem1.Visible = false;
                    materialHelpToolStripMenuItem.Visible = materialtabToolStripMenuItem.Visible = false;
                    break;
                case ProgramCore.ProgramMode.HeadShop_v10_2:
                    Text = @"HeadShop 10";
                    aboutHeadShopProToolStripMenuItem.Text = @"About HeadShop 10.2";
                    panelMenuStage.Image = Resources.btnMenuStageNormal;
                    childHelpToolStripMenuItem.Visible = false;
                    break;
                case ProgramCore.ProgramMode.HeadShop_v11:
                    Text = @"HeadShop 11";
                    aboutHeadShopProToolStripMenuItem.Text = @"About HeadShop 11";
                    panelMenuStage.Image = Resources.btnMenuStageNormal;
                    panelMenuFeatures.Visible = featuresTabToolStripMenuItem.Visible = true;
                    panelMenuStage.Visible = stageHelpToolStripMenuItem.Visible = false;
                    profileTabToolStripMenuItem.Visible = stageLibraryToolStripMenuItem1.Visible = false;
                    childHelpToolStripMenuItem.Visible = false;
                    break;
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                    Text = @"HeadShop 11";
                    aboutHeadShopProToolStripMenuItem.Text = @"About HeadShop 11";
                    panelMenuStage.Image = Resources.btnMenuStageNormal;
                    childHelpToolStripMenuItem.Visible = false;
                    break;
                case ProgramCore.ProgramMode.HeadShop_OneClick:
                    Text = @"HeadShop OneClick";
                    aboutHeadShopProToolStripMenuItem.Text = @"About HeadShop OneClick";

                    editToolStripMenuItem.Visible = frontTabToolStripMenuItem.Visible = false;
                    openToolStripMenuItem.Visible = saveAsToolStripMenuItem.Visible = saveToolStripMenuItem.Visible = false;
                    panelMenuFeatures.Visible = panelMenuStyle.Visible = panelMenuAccessories.Visible = panelMenuMaterials.Visible = panelMenuStage.Visible = false;
                    profileTabToolStripMenuItem.Visible = featuresTabToolStripMenuItem.Visible = styleTabToolStripMenuItem.Visible = accessoryTabToolStripMenuItem.Visible = materialtabToolStripMenuItem.Visible = navigateToolStripMenuItem.Visible = stageLibraryToolStripMenuItem1.Visible = false;
                    startHelpToolStripMenuItem.Visible = freehandHelpToolStripMenuItem.Visible = mirrorHelpToolStripMenuItem.Visible = accessoriesHelpToolStripMenuItem.Visible =
                    colorHelpToolStripMenuItem.Visible = accessoriesHelpToolStripMenuItem1.Visible = styleHelpToolStripMenuItem.Visible = materialHelpToolStripMenuItem.Visible = stageHelpToolStripMenuItem.Visible =
                    videoTutorialPart2ToolStripMenuItem.Visible = showManualToolStripMenuItem.Visible = false;          // все хелпные кнопки
                    toolStripMenuItem5.Visible = false;
                    childHelpToolStripMenuItem.Visible = true;
                    break;
            }

            if (!UserConfig.ByName("Tutorials").HasAny())
                InitializeTutorialLinks();

            PluginUvGroups.AddRange(new[] { "1_lip", "1_skinface", "lips", "face" });

            //ProgramCore.PluginMode = true;

            #region активация охуенной распознавалки

            if (FSDK.FSDKE_OK != FSDK.ActivateLibrary("DWysHuomlBcczVM2MQfiz/3WraXb7r+fM0th71X5A9z+gsHn2kpGOgWrVh9D/9sQWlPXO00CFmGMvetl9A+VEr9Y5GVBIccyV32uaZutZjKYH5KB2k87NJAAw6NPkzK0DSQ5b5W7EO0yg2+x4HxpWzPogGyIIYcAHIYY11/YGsU="))
            {
                MessageBox.Show(@"Please run the License Key Wizard (Start - Luxand - FaceSDK - License Key Wizard)", @"Error activating FaceSDK", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            FSDK.InitializeLibrary();
            FSDK.SetFaceDetectionParameters(true, true, 384);

            #endregion

            var path = UserConfig.AppDataDir;
            path = Path.Combine(path, "TempProject");
            if (Directory.Exists(path))
                try
                {
                    Directory.Delete(path, true);
                }
                catch (Exception ex)
                {
                    ProgramCore.EchoToLog(ex);
                }

            //  ProgramCore.PluginMode = true;
            if (!string.IsNullOrEmpty(fn))
            {
                if (fn.StartsWith("fs"))
                {

                    ProgramCore.PluginMode = true;
                    var strs = fn.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower().Trim());
                    foreach (var str in strs)
                    {
                        if (str.StartsWith("uvgroups"))
                        {
                            PluginUvGroups.AddRange(str.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower().Trim()).ToList());
                            break;
                        }
                    }
                }
                else if ((fn.Contains(".hds") || fn.Contains(".hrs")) && File.Exists(fn)) // open associated files
                    openProjectPath = fn;
            }
        }

        #region Form's event

        private void frmMain_Load(object sender, EventArgs e)
        {
            ProgramCore.Splash.ShowDialog();

            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_v10_2:
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                case ProgramCore.ProgramMode.PrintAhead:
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                case ProgramCore.ProgramMode.PrintAhead_Online:
                    if (UserConfig.ByName("Options")["Tutorials", "Start", "1"] == "1")
                        frmTutStart.ShowDialog(this);
                    break;
            }

            ctrlRenderControl.Initialize();

            frmParts = new frmParts();
            frmParts.UpdateList();


            if (!string.IsNullOrEmpty(openProjectPath))
                OpenProject(openProjectPath);
            else
            {
                switch (ProgramCore.CurrentProgram)
                {
                    case ProgramCore.ProgramMode.HeadShop_v10_2:
                    case ProgramCore.ProgramMode.PrintAhead:
                    case ProgramCore.ProgramMode.PrintAhead_PayPal:
                    case ProgramCore.ProgramMode.PrintAhead_Online:
                    case ProgramCore.ProgramMode.HeadShop_OneClick:
                    case ProgramCore.ProgramMode.HeadShop_v11:
                    case ProgramCore.ProgramMode.HeadShop_Rotator:          // вроде для этой версии не нужно открывать загруженные проекты. Поэтому лучше использовать этот диалог, он уже с новой распознавалкой.
                        {
                            #region проект для PrintAhead

                            var newProjectDlg = new frmNewProject4PrintAhead(true);
                            newProjectDlg.ShowDialog(this);

                            if (newProjectDlg.dialogResult != DialogResult.OK)
                            {
                                Application.Exit();
                                return;
                            }

                            if (string.IsNullOrEmpty(newProjectDlg.OpenProjectPath))
                                newProjectDlg.CreateProject();
                            else
                                OpenProject(newProjectDlg.OpenProjectPath);

                            #endregion
                        }
                        break;
                }

            }

            if (ProgramCore.PluginMode)     // хотелка, что бы прога всегда была выше Daz'a
                TopMost = true;

            frmMaterial = new frmMaterials();
            frmMaterial.VisibleChanged += frmMaterial_VisibleChanged;

            frmAccessories = new frmAccessories();
            frmAccessories.VisibleChanged += frmAccessories_VisibleChanged;

            switch (ProgramCore.CurrentProgram)
            {
                /* case ProgramCore.ProgramMode.HeadShop:            // 31.10 - frmStages  использовалось в старой версии программы и старикан отказался от этого вроде как.
                 case ProgramCore.ProgramMode.HeadShop_Rotator:
                     {
                         frmStages = new frmStages();
                         frmStages.VisibleChanged += frmStages_VisibleChanged;
                     }
                     break;*/
                default:
                    frmPrint = new frmPrint();
                    frmPrint.VisibleChanged += frmStages_VisibleChanged;
                    break;
            }


            frmStyles = new frmStyles();
            frmStyles.VisibleChanged += frmStyles_VisibleChanged;

            frmFreeHand = new frmFreeHand();

            panelCut = new PanelCut
            {
                Dock = DockStyle.Fill
            };
            panelCut.OnDelete += hairLibraryOnDelete_Click;
            panelCut.OnSave += OnSavePart_Click;
            panelCut.OnUndo += OnUndo_Click;

            panelShape = new PanelShape
            {
                Dock = DockStyle.Fill
            };
            panelShape.OnSave += OnSavePart_Click;
            panelShape.OnUndo += OnUndo_Click;

            panelAccessories = new PanelLibrary(true, true)
            {
                Dock = DockStyle.Fill
            };
            panelAccessories.OnOpenLibrary += accessoryLibraryOnOpen_Click;
            panelAccessories.OnDelete += accessoryLibraryOnDelete_Click;
            panelAccessories.OnExport += OnExport_Click;
            panelAccessories.OnSave += OnSavePart_Click;

            panelStyles = new PanelLibrary(true, true)
            {
                Dock = DockStyle.Fill
            };
            panelStyles.OnOpenLibrary += styleLibraryOnOpen_Click;
            panelStyles.OnDelete += styleLibraryOnDelete_Click;
            panelStyles.OnExport += OnExport_Click;
            panelStyles.OnSave += OnSavePart_Click;

            panelShape.OnSave += OnSavePart_Click;

            panelMaterials = new PanelLibrary(true, true)
            {
                Dock = DockStyle.Fill
            };
            panelMaterials.OnOpenLibrary += materialLibraryOnOpen_Click;
            panelMaterials.OnDelete += materialLibraryOnDelete_Click;
            panelMaterials.OnExport += OnExport_Click;
            panelMaterials.OnSave += OnSavePart_Click;
            panelShape.OnSave += OnSavePart_Click;

            panelStages = new PanelLibrary(true, false)
            {
                Dock = DockStyle.Fill
            };
            panelStages.OnOpenLibrary += stagesLibraryOnOpen_Click;
            panelStages.OnExport += OnExport_Click;
            panelStages.OnDelete += stagesLibraryOnDelete_Click;

            panelFront = new PanelHead()
            {
                Dock = DockStyle.Fill
            };
            panelFront.OnDelete += OnDeleteHeadSelectedPoints_Click;
            panelFront.OnSave += OnSaveHead_Click;
            panelFront.OnUndo += OnUndo_Click;
            panelFront.OnShapeTool += OnShapeTool_Click;

            panelFeatures = new PanelFeatures
            {
                Dock = DockStyle.Fill
            };
            panelFeatures.OnDelete += OnDeleteHeadSelectedPoints_Click;
            panelFeatures.OnSave += OnSaveHead_Click;
            panelFeatures.OnUndo += OnUndo_Click;

            if (activePanel != -1)      // это загружено из проекта. открываем ту вкладку, на которой закрыли
            {
                switch (activePanel)
                {
                    case 0:
                        panelMenuCut_Click(null, EventArgs.Empty);
                        break;
                    case 1:
                        panelMenuShape_Click(null, EventArgs.Empty);
                        break;
                    case 2:
                        panelMenuAccessories_Click(null, EventArgs.Empty);
                        break;
                    case 3:
                        panelMenuMaterials_Click(null, EventArgs.Empty);
                        break;
                    case 4:
                        panelMenuStage_Click(null, EventArgs.Empty);

                        if (ProgramCore.Project.projectCamera != null) // сохраненная позиция камеры.
                        {
                            ProgramCore.MainForm.ctrlRenderControl.camera._dy = ProgramCore.Project.projectCamera._dy;
                            ProgramCore.MainForm.ctrlRenderControl.camera.beta = ProgramCore.Project.projectCamera.beta;
                            ProgramCore.MainForm.ctrlRenderControl.camera.Scale = ProgramCore.Project.projectCamera.Scale;
                            ProgramCore.MainForm.ctrlRenderControl.camera.AfterLoadVoid();
                        }
                        break;
                    case 5:
                        panelMenuStyle_Click(null, EventArgs.Empty);
                        break;
                    case 6:
                        panelMenuFront_Click(null, EventArgs.Empty);
                        break;
                    case 7:
                        panelMenuFeatures_Click(null, EventArgs.Empty);
                        break;
                }
            }
            else
            {
                if (ProgramCore.MainForm.ctrlRenderControl.pickingController.HairMeshes.Count == 0)
                    panelMenuFront_Click(null, EventArgs.Empty); // set opened by default
                else
                    panelMenuCut_Click(null, EventArgs.Empty);  // иначе это наш проект волосач и по дефолту мы работаем с волосами, а не с формой лица.
            }

            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.PrintAhead:
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                case ProgramCore.ProgramMode.PrintAhead_Online:
                case ProgramCore.ProgramMode.HeadShop_OneClick:
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                    {
                        ProgramCore.MainForm.HeadFront = true;
                        if (!ProgramCore.PluginMode)
                        {
                            //     ProgramCore.MainForm.panelFront.btnAutodots_Click(null, null);            //TODO 28.05.2018. это было раскомменчено.
                            //    ProgramCore.MainForm.panelFront.btnAutodots_Click(null, null);

                            if (ProgramCore.CurrentProgram == ProgramCore.ProgramMode.HeadShop_Rotator && Math.Abs(ProgramCore.Project.RotatedAngle) > 5)
                            {
                                var headMeshesController = ProgramCore.MainForm.ctrlRenderControl.headMeshesController;
                                headMeshesController.Mirror(headMeshesController.RenderMesh.HeadAngle > 0.0f, 0.0f);
                            }
                        }
                    }
                    break;
                default:
                    InitRecentItems();
                    break;
            }

            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_v10_2:
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                case ProgramCore.ProgramMode.PrintAhead:
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                case ProgramCore.ProgramMode.PrintAhead_Online:
                    if (ProgramCore.Project.ManType == ManType.Custom && UserConfig.ByName("Options")["Tutorials", "CustomHeads", "1"] == "1")
                        frmTutCustomHeads.ShowDialog(this);
                    break;
            }

            if (ProgramCore.CurrentProgram == ProgramMode.HeadShop_v11)
            {
                ProgramCore.MainForm.ctrlRenderControl.headMeshesController.RenderMesh.SetMorphPercent(0.5f);
            }

            panelFront.btnAutodots_Click(null, new EventArgs());
            panelFront.btnAutodots_Click(null, new EventArgs());
        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmMaterial = null;
            frmAccessories = null;
            frmStyles = null;
            frmPrint = null;
            e.Cancel = false;
        }
        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            ctrlTemplateImage.KeyDown(e);
        }

        private void OnSavePart_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.SaveSelectedHairToPartsLibrary();
        }
        private void OnSaveHead_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.SaveHeadToFile();
        }
        private void OnUndo_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.historyController.Undo();
        }

        private void OnDeleteHeadSelectedPoints_Click(object sender, EventArgs e)
        {
            ProgramCore.Project.RenderMainHelper.headController.RemoveSelectedPoints();
        }

        #region Menu

        private void exitBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private bool beginExport;
        private void OnExport_Click(object sender, EventArgs e)
        {
            if (beginExport)
                return;

            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_v10_2:
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                case ProgramCore.ProgramMode.PrintAhead:
                    if (UserConfig.ByName("Options")["Tutorials", "3DPrinting", "1"] == "1")
                        ProgramCore.MainForm.frmTut3dPrint.ShowDialog(this);
                    break;
            }

            beginExport = true;
            Export();
            beginExport = false;
        }
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnExport_Click(this, EventArgs.Empty);
        }
        private void exportToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            OnExport_Click(null, EventArgs.Empty);
        }
        private void exportToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OnExport_Click(null, EventArgs.Empty);
        }
        private void exportToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            OnExport_Click(null, EventArgs.Empty);
        }

        private void saveToolStripMenuItem7_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.SaveSelectedHairToPartsLibrary();
        }
        private void saveToolStripMenuItem8_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.SaveHeadToFile();
        }
        private void saveToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.SaveHeadToFile();
        }
        private void saveToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.SaveHeadToFile();
        }

        private void deleteToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.DeleteSelectedHair();
        }
        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (frmStyles.Visible)
                frmStyles.Hide();
            else
                frmStyles.Show(this);
        }
        private void undoToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.historyController.Undo();
        }
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProgramCore.MainForm.ctrlRenderControl.historyController.Undo();
        }

        private void photoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmStages.DoPhoto();
        }
        private void linesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            panelFront.btnPolyLine_Click(null, EventArgs.Empty);
        }
        private void handToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            panelFront.btnShapeTool_Click(null, EventArgs.Empty);
        }
        private void autodotsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelFront.btnAutodots_Click(null, EventArgs.Empty);
        }
        private void ponitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelFront.btnLasso_Click(null, EventArgs.Empty);
        }
        private void flipToLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelFront.btnFlipLeft_Click(null, EventArgs.Empty);
        }
        private void flipToRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelFront.btnFlipRight_Click(null, EventArgs.Empty);
        }

        #endregion

        #region Navigaion

        private void checkHand_Click(object sender, EventArgs e)
        {
            if (checkHand.Tag.ToString() == "2")
            {
                ResetScaleModeTools();
                ctrlRenderControl.ScaleMode = ScaleMode.Move;

                checkHand.Tag = "1";
                checkArrow.Tag = checkZoom.Tag = "2";

                checkHand.Image = Resources.btnHandPressed;
                checkArrow.Image = Resources.btnArrowNormal;
                checkZoom.Image = Resources.btnZoomNormal;
            }
            else
            {
                checkHand.Tag = "2";
                checkHand.Image = Resources.btnHandNormal;

                ctrlRenderControl.ScaleMode = ScaleMode.None;
            }
        }
        private void checkArrow_Click(object sender, EventArgs e)
        {
            if (checkArrow.Tag.ToString() == "2")
            {
                ResetScaleModeTools();
                ctrlRenderControl.ScaleMode = ScaleMode.Rotate;

                checkArrow.Tag = "1";
                checkHand.Tag = checkZoom.Tag = "2";

                checkArrow.Image = Resources.btnArrowPressed;
                checkHand.Image = Resources.btnHandNormal;
                checkZoom.Image = Resources.btnZoomNormal;
            }
            else
            {
                checkArrow.Tag = "2";
                checkArrow.Image = Resources.btnArrowNormal;

                ctrlRenderControl.ScaleMode = ScaleMode.None;
            }
        }
        private void checkZoom_Click(object sender, EventArgs e)
        {
            if (checkZoom.Tag.ToString() == "2")
            {
                ResetScaleModeTools();
                ctrlRenderControl.ScaleMode = ScaleMode.Zoom;

                checkZoom.Tag = "1";
                checkHand.Tag = checkArrow.Tag = "2";

                checkZoom.Image = Resources.btnZoomPressed;
                checkHand.Image = Resources.btnHandNormal;
                checkArrow.Image = Resources.btnArrowNormal;
            }
            else
            {
                checkZoom.Tag = "2";
                checkZoom.Image = Resources.btnZoomNormal;

                ctrlRenderControl.ScaleMode = ScaleMode.None;
            }
        }

        private void btnUnscale_MouseDown(object sender, MouseEventArgs e)
        {
            btnUnscale.Image = Resources.btnUnscalePressed;
        }
        private void btnUnscale_MouseUp(object sender, MouseEventArgs e)
        {
            btnUnscale.Image = Resources.btnUnscaleNormal;

            ctrlRenderControl.camera.ResetCamera(true);
            if (ProgramCore.MainForm.HeadProfile)
                ctrlRenderControl.OrtoRight();

            ctrlTemplateImage.RecalcRealTemplateImagePosition();

            checkHand.Tag = checkArrow.Tag = checkZoom.Tag = "2";
            checkHand.Image = Resources.btnHandNormal;
            checkArrow.Image = Resources.btnArrowNormal;
            checkZoom.Image = Resources.btnZoomNormal;

            //    ctrlRenderControl.Mode = Mode.None;
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.ZoomIn();
        }
        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.ZoomOut();
        }
        private void fitWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.camera.ResetCamera(true);
        }

        private void ortoBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.OrtoBack();
        }
        private void ortoTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.OrtoTop();
        }
        private void ortoLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.OrtoLeft();
        }
        private void ortoRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.OrtoRight();
        }

        private void turnRightoneStepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.TurnRight();
        }
        private void turnLeftoneStepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.TurnLeft();
        }

        private void stepToponeStepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.StepTop();
        }
        private void stepBottomoneStepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.StepBottom();
        }

        private void panTopcontinuousPanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.TimerMode = Mode.TimerStepTop;
            ctrlRenderControl.workTimer.Start();
        }
        private void panBottomcontinuousPanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.TimerMode = Mode.TimerStepBottom;
            ctrlRenderControl.workTimer.Start();
        }

        private void rotateLeftcontinuousRotateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.TimerMode = Mode.TimerTurnLeft;
            ctrlRenderControl.workTimer.Start();
        }
        private void rotateRightcontinuousRotateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.TimerMode = Mode.TimerTurnRight;
            ctrlRenderControl.workTimer.Start();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.TimerMode = Mode.None;
            ctrlRenderControl.workTimer.Stop();
        }

        #endregion

        #region Tutorials

        public void ShowVideo()
        {
            var quickStartLink = UserConfig.ByName("Tutorials")["Links", "QuickStart", "https://youtu.be/8cejdijABQY"];
            Process.Start(quickStartLink);
        }
        public void ShowTutorial()
        {
            Process.Start("http://ep.yimg.com/ty/cdn/yhst-48396527764316/HeadShop10.2manual.pdf");
        }
        public void ShowSiteInfo()
        {
            Process.Start("http://www.abalonellc.com/");
        }

        private void showManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTutorial();
        }
        private void aboutHeadShopProToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProgramCore.Splash.ShowDialog(this);
        }
        private void videoTutorialPart1CutAndShapeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowVideo();
        }
        private void videoTutorialPart2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var advancedLink = UserConfig.ByName("Tutorials")["Links", "AdvancedManual", "https://youtu.be/z8rnXteNnm0"];
            Process.Start(advancedLink);
        }

        private void startHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_v10_2:
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                case ProgramCore.ProgramMode.PrintAhead:
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                case ProgramCore.ProgramMode.PrintAhead_Online:
                    frmTutStart.ShowDialog(this);
                    break;
            }
        }
        private void accessoriesHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_v10_2:
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                case ProgramCore.ProgramMode.PrintAhead:
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                case ProgramCore.ProgramMode.PrintAhead_Online:
                    frmTutFeatures.ShowDialog(this);
                    break;
            }
        }
        private void materialHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_v10_2:
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                case ProgramCore.ProgramMode.PrintAhead:
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                case ProgramCore.ProgramMode.PrintAhead_Online:
                    frmTutExport.ShowDialog(this);
                    break;
            }
        }
        private void stageHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_v10_2:
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                case ProgramCore.ProgramMode.PrintAhead:
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                case ProgramCore.ProgramMode.PrintAhead_Online:
                    frmTut3dPrint.ShowDialog(this);
                    break;
            }
        }
        private void styleHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_v10_2:
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                case ProgramCore.ProgramMode.PrintAhead:
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                case ProgramCore.ProgramMode.PrintAhead_Online:
                    frmTutStyle.ShowDialog(this);
                    break;
            }
        }

        private void autodotsHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTutAutodots.ShowDialog(this);
        }
        private void mirrorHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_v10_2:
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                case ProgramCore.ProgramMode.PrintAhead:
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                case ProgramCore.ProgramMode.PrintAhead_Online:
                    frmTutLineTool.ShowDialog(this);
                    break;
            }
        }
        private void freehandHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_v10_2:
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                case ProgramCore.ProgramMode.PrintAhead:
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                case ProgramCore.ProgramMode.PrintAhead_Online:
                    frmTutFreehand.ShowDialog(this);
                    break;
            }
        }
        private void profileHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTutProfile.ShowDialog(this);
        }

        private void accessoriesHelpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_v10_2:
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                case ProgramCore.ProgramMode.PrintAhead:
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                case ProgramCore.ProgramMode.PrintAhead_Online:
                    frmTutAccessory.ShowDialog(this);
                    break;
            }
        }
        private void colorHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_v10_2:
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                case ProgramCore.ProgramMode.PrintAhead:
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                case ProgramCore.ProgramMode.PrintAhead_Online:
                    frmTutMaterial.ShowDialog(this);
                    break;
            }
        }
        private void childHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_OneClick:
                    frmTutChild.ShowDialog(this);
                    break;
            }
        }
        private void retouchHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTutRetouch.ShowDialog(this);
        }

        #endregion

        #region Libraries and panels

        private void frmAccessories_VisibleChanged(object sender, EventArgs e)
        {
            if (frmAccessories.Visible)
                panelAccessories.ShowControl();
            else
                panelAccessories.HideControl();
        }
        private void frmMaterial_VisibleChanged(object sender, EventArgs e)
        {
            if (frmMaterial.Visible)
                panelMaterials.ShowControl();
            else
                panelMaterials.HideControl();
        }
        private void frmStages_VisibleChanged(object sender, EventArgs e)
        {
            if (frmPrint.Visible)
                panelStages.ShowControl();
            else
                panelStages.HideControl();
        }
        private void frmStyles_VisibleChanged(object sender, EventArgs e)
        {
            if (frmStyles.Visible)
                panelStyles.ShowControl();
            else
                panelStyles.HideControl();
        }

        private void materialLibraryOnOpen_Click(object sender, EventArgs e)
        {
            if (frmMaterial.Visible)
                frmMaterial.HideUp();
            else
                frmMaterial.Show(this);
        }
        private void accessoryLibraryOnOpen_Click(object sender, EventArgs e)
        {
            if (frmAccessories.Visible)
                frmAccessories.Hide();
            else
                frmAccessories.Show(this);
        }
        private void styleLibraryOnOpen_Click(object sender, EventArgs e)
        {
            if (frmStyles.Visible)
                frmStyles.Hide();
            else
                frmStyles.Show(this);
        }
        private void OnShapeTool_Click(object sender, EventArgs e)
        {
            if (frmFreeHand.Visible)
                frmFreeHand.Hide();
            else
                frmFreeHand.Show(this);
        }
        private void stagesLibraryOnOpen_Click(object sender, EventArgs e)
        {
            if (frmPrint.Visible)
                frmPrint.Hide();
            else
            {
                panelMenuStage_Click(this, EventArgs.Empty);
                ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes.Clear();
                frmPrint.Show(this);
            }
        }
        private void partsLibraryOnOpen_Click(object sender, EventArgs e)
        {
            if (frmParts.Visible)
                frmParts.Hide();
            else
                frmParts.Show(this);
        }
        private void styleLibraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmStyles.Visible)
                frmStyles.Hide();
            else
            {
                if (ProgramCore.MainForm.ctrlRenderControl.pickingController.HairMeshes.Count > 0 || ProgramCore.MainForm.ctrlRenderControl.pickingController.AccesoryMeshes.Count > 0)         // mean that it's user action
                {
                    if (MessageBox.Show(@"This action will remove all changes. Are you sure?", @"Attention", MessageBoxButtons.OKCancel) != DialogResult.OK)
                        return;

                    ctrlRenderControl.CleanProjectMeshes();                     // clear all changes and reset position.
                    ctrlRenderControl.OrtoTop();
                }

                frmStyles.Show(this);
            }
        }

        private void stagesLibraryOnDelete_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.SetDefaultBackground();
        }
        private void materialLibraryOnDelete_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.DeleteSelectedTexture();
        }
        private void accessoryLibraryOnDelete_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.DeleteSelectedAccessory();
        }
        private void styleLibraryOnDelete_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.DeleteSelectedAccessory();
        }
        private void hairLibraryOnDelete_Click(object sender, EventArgs e)
        {
            ctrlRenderControl.DeleteSelectedHair();
        }

        public int activePanel = -1;
        public void panelMenuCut_Click(object sender, EventArgs e)
        {
            if (ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomControlPoints || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomPoints || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomProfilePoints)
                return;

            if (panelMenuCut.Tag.ToString() == "2")
            {
                activePanel = 0;
                panelMenuCut.Tag = "1";
                panelMenuCut.Image = Resources.btnMenuCutPressed;
                panelMenuControl.Controls.Clear();
                panelMenuControl.Controls.Add(panelCut);

                panelMenuStyle.Tag = panelMenuShape.Tag = panelMenuAccessories.Tag = panelMenuMaterials.Tag = panelMenuStage.Tag = panelMenuFeatures.Tag = "2";
                panelMenuStyle.Image = Resources.btnMenuStyleNormal;
                panelMenuShape.Image = Resources.btnMenuShapeNormal;
                panelMenuAccessories.Image = Resources.btnMenuAccessoriesNormal;
                panelMenuMaterials.Image = Resources.btnMenuColorNormal;

                switch (ProgramCore.CurrentProgram)
                {
                    case ProgramCore.ProgramMode.HeadShop_v10_2:
                    case ProgramCore.ProgramMode.HeadShop_v11:
                    case ProgramCore.ProgramMode.HeadShop_Rotator:
                    case ProgramCore.ProgramMode.HeadShop_OneClick:
                        panelMenuStage.Image = Resources.btnMenuStageNormal;
                        break;
                    case ProgramCore.ProgramMode.PrintAhead:
                    case ProgramCore.ProgramMode.PrintAhead_PayPal:
                    case ProgramCore.ProgramMode.PrintAhead_Online:
                        panelMenuStage.Image = Resources.btnMenuPrintNormal;
                        break;
                }

                panelMenuFront.Tag = panelMenuFeatures.Tag = "2";
                panelMenuFront.Image = Resources.btnMenuFrontNormal;
                panelMenuFeatures.Image = Resources.btnMenuFeaturesNormal;

                ProgramCore.MainForm.ctrlRenderControl.StagesDeactivate(-1);

                ctrlRenderControl.Mode = Mode.None;
                ctrlTemplateImage.btnCopyProfileImg.Visible = false;
                ctrlRenderControl.OrtoTop();
                EnableRotating();
                ProgramCore.MainForm.ctrlTemplateImage.SetTemplateImage(ProgramCore.Project.FrontImage);       // возвращаем как было, после изменения профиля лица
                HeadMode = HeadFront = HeadProfile = HeadFeature = false;
                ResetModeTools();

                if (UserConfig.ByName("Options")["Tutorials", "Cut", "1"] == "1")
                    frmTutCut.ShowDialog(this);
            }
        }
        public void panelMenuShape_Click(object sender, EventArgs e)
        {
            if (ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomControlPoints || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomPoints || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomProfilePoints)
                return;

            if (panelMenuShape.Tag.ToString() == "2")
            {
                activePanel = 1;
                panelMenuShape.Tag = "1";
                panelMenuShape.Image = Resources.btnMenuShapePressed;
                panelMenuControl.Controls.Clear();
                panelMenuControl.Controls.Add(panelShape);

                panelMenuStyle.Tag = panelMenuCut.Tag = panelMenuAccessories.Tag = panelMenuMaterials.Tag = panelMenuStage.Tag = panelMenuFeatures.Tag = "2";
                panelMenuStyle.Image = Resources.btnMenuStyleNormal;
                panelMenuCut.Image = Resources.btnMenuCutNormal;
                panelMenuAccessories.Image = Resources.btnMenuAccessoriesNormal;
                panelMenuMaterials.Image = Resources.btnMenuColorNormal;
                switch (ProgramCore.CurrentProgram)
                {
                    case ProgramCore.ProgramMode.HeadShop_v10_2:
                    case ProgramCore.ProgramMode.HeadShop_v11:
                    case ProgramCore.ProgramMode.HeadShop_Rotator:
                    case ProgramCore.ProgramMode.HeadShop_OneClick:
                        panelMenuStage.Image = Resources.btnMenuStageNormal;
                        break;
                    case ProgramCore.ProgramMode.PrintAhead:
                    case ProgramCore.ProgramMode.PrintAhead_PayPal:
                    case ProgramCore.ProgramMode.PrintAhead_Online:
                        panelMenuStage.Image = Resources.btnMenuPrintNormal;
                        break;
                }

                panelMenuFront.Tag = panelMenuFeatures.Tag = "2";
                panelMenuFront.Image = Resources.btnMenuFrontNormal;
                panelMenuFeatures.Image = Resources.btnMenuFeaturesNormal;

                ProgramCore.MainForm.ctrlRenderControl.StagesDeactivate(-1);

                ctrlRenderControl.Mode = Mode.None;
                ctrlTemplateImage.btnCopyProfileImg.Visible = false;
                ctrlRenderControl.OrtoTop();
                EnableRotating();
                ProgramCore.MainForm.ctrlTemplateImage.SetTemplateImage(ProgramCore.Project.FrontImage);       // возвращаем как было, после изменения профиля лица
                HeadMode = HeadFront = HeadProfile = HeadFeature = false;
                ResetModeTools();

                if (UserConfig.ByName("Options")["Tutorials", "Shape", "1"] == "1")
                    frmTutShape.ShowDialog(this);
            }
        }
        public void panelMenuAccessories_Click(object sender, EventArgs e)
        {
            if (ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomControlPoints || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomPoints || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomProfilePoints)
                return;

            if (panelMenuAccessories.Tag.ToString() == "2")
            {
                activePanel = 2;
                panelMenuAccessories.Tag = "1";
                panelMenuAccessories.Image = Resources.btnMenuAccessoriesPressed;
                panelMenuControl.Controls.Clear();
                panelMenuControl.Controls.Add(panelAccessories);

                panelMenuStyle.Tag = panelMenuCut.Tag = panelMenuShape.Tag = panelMenuMaterials.Tag = panelMenuStage.Tag = panelMenuFeatures.Tag = "2";
                panelMenuStyle.Image = Resources.btnMenuStyleNormal;
                panelMenuCut.Image = Resources.btnMenuCutNormal;
                panelMenuShape.Image = Resources.btnMenuShapeNormal;
                panelMenuMaterials.Image = Resources.btnMenuColorNormal;
                switch (ProgramCore.CurrentProgram)
                {
                    case ProgramCore.ProgramMode.HeadShop_v10_2:
                    case ProgramCore.ProgramMode.HeadShop_v11:
                    case ProgramCore.ProgramMode.HeadShop_Rotator:
                    case ProgramCore.ProgramMode.HeadShop_OneClick:
                        panelMenuStage.Image = Resources.btnMenuStageNormal;
                        break;
                    case ProgramCore.ProgramMode.PrintAhead:
                    case ProgramCore.ProgramMode.PrintAhead_PayPal:
                    case ProgramCore.ProgramMode.PrintAhead_Online:
                        panelMenuStage.Image = Resources.btnMenuPrintNormal;
                        break;
                }

                panelMenuFront.Tag = panelMenuFeatures.Tag = "2";
                panelMenuFront.Image = Resources.btnMenuFrontNormal;
                panelMenuFeatures.Image = Resources.btnMenuFeaturesNormal;

                ProgramCore.MainForm.ctrlRenderControl.StagesDeactivate(-1);

                ctrlRenderControl.Mode = Mode.None;
                ctrlTemplateImage.btnCopyProfileImg.Visible = false;
                ctrlRenderControl.OrtoTop();
                EnableRotating();
                ProgramCore.MainForm.ctrlTemplateImage.SetTemplateImage(ProgramCore.Project.FrontImage);       // возвращаем как было, после изменения профиля лица
                HeadMode = HeadFront = HeadProfile = HeadFeature = false;

                if (UserConfig.ByName("Options")["Tutorials", "Accessory", "1"] == "1")
                    frmTutAccessory.ShowDialog(this);
            }
        }
        public void panelMenuMaterials_Click(object sender, EventArgs e)
        {
            if (ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomControlPoints || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomPoints || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomProfilePoints)
                return;

            if (panelMenuMaterials.Tag.ToString() == "2")
            {
                activePanel = 3;
                panelMenuMaterials.Tag = "1";
                panelMenuMaterials.Image = Resources.btnMenuColorPressed;
                panelMenuControl.Controls.Clear();
                panelMenuControl.Controls.Add(panelMaterials);

                panelMenuStyle.Tag = panelMenuCut.Tag = panelMenuShape.Tag = panelMenuAccessories.Tag = panelMenuStage.Tag = panelMenuFeatures.Tag = "2";
                panelMenuStyle.Image = Resources.btnMenuStyleNormal;
                panelMenuCut.Image = Resources.btnMenuCutNormal;
                panelMenuShape.Image = Resources.btnMenuShapeNormal;
                panelMenuAccessories.Image = Resources.btnMenuAccessoriesNormal;
                switch (ProgramCore.CurrentProgram)
                {
                    case ProgramCore.ProgramMode.HeadShop_v10_2:
                    case ProgramCore.ProgramMode.HeadShop_v11:
                    case ProgramCore.ProgramMode.HeadShop_Rotator:
                    case ProgramCore.ProgramMode.HeadShop_OneClick:
                        panelMenuStage.Image = Resources.btnMenuStageNormal;
                        break;
                    case ProgramCore.ProgramMode.PrintAhead:
                    case ProgramCore.ProgramMode.PrintAhead_PayPal:
                    case ProgramCore.ProgramMode.PrintAhead_Online:
                        panelMenuStage.Image = Resources.btnMenuPrintNormal;
                        break;
                }

                panelMenuFront.Tag = panelMenuFeatures.Tag = "2";
                panelMenuFront.Image = Resources.btnMenuFrontNormal;
                panelMenuFeatures.Image = Resources.btnMenuFeaturesNormal;

                ProgramCore.MainForm.ctrlRenderControl.StagesDeactivate(-1);

                ctrlRenderControl.Mode = Mode.None;
                ctrlTemplateImage.btnCopyProfileImg.Visible = false;
                ctrlRenderControl.OrtoTop();
                EnableRotating();
                ProgramCore.MainForm.ctrlTemplateImage.SetTemplateImage(ProgramCore.Project.FrontImage);       // возвращаем как было, после изменения профиля лица
                HeadMode = HeadFront = HeadProfile = HeadFeature = false;

                if (UserConfig.ByName("Options")["Tutorials", "Material", "1"] == "1")
                    frmTutMaterial.ShowDialog(this);
            }
        }
        public void panelMenuStage_Click(object sender, EventArgs e)
        {
            if (ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomControlPoints || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomPoints || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomProfilePoints)
                return;

            if (panelMenuStage.Tag.ToString() == "2")
            {
                activePanel = 4;
                panelMenuStage.Tag = "1";
                switch (ProgramCore.CurrentProgram)
                {
                    case ProgramCore.ProgramMode.HeadShop_v10_2:
                    case ProgramCore.ProgramMode.HeadShop_v11:
                    case ProgramCore.ProgramMode.HeadShop_Rotator:
                    case ProgramCore.ProgramMode.HeadShop_OneClick:
                        panelMenuStage.Image = Resources.btnMenuStagePressed;
                        break;
                    case ProgramCore.ProgramMode.PrintAhead:
                    case ProgramCore.ProgramMode.PrintAhead_PayPal:
                    case ProgramCore.ProgramMode.PrintAhead_Online:
                        panelMenuStage.Image = Resources.btnMenuPrintPressed;
                        break;
                }
                panelMenuControl.Controls.Clear();
                panelMenuControl.Controls.Add(panelStages);

                panelMenuStyle.Tag = panelMenuCut.Tag = panelMenuShape.Tag = panelMenuAccessories.Tag = panelMenuMaterials.Tag = panelMenuFeatures.Tag = "2";
                panelMenuStyle.Image = Resources.btnMenuStyleNormal;
                panelMenuCut.Image = Resources.btnMenuCutNormal;
                panelMenuShape.Image = Resources.btnMenuShapeNormal;
                panelMenuAccessories.Image = Resources.btnMenuAccessoriesNormal;
                panelMenuMaterials.Image = Resources.btnMenuColorNormal;

                panelMenuFront.Tag = panelMenuFeatures.Tag = "2";
                panelMenuFront.Image = Resources.btnMenuFrontNormal;
                panelMenuFeatures.Image = Resources.btnMenuFeaturesNormal;

                ProgramCore.MainForm.ctrlRenderControl.StagesActivate();

                ctrlRenderControl.Mode = Mode.None;
                ctrlTemplateImage.btnCopyProfileImg.Visible = false;
                ctrlRenderControl.OrtoTop();
                EnableRotating();
                ProgramCore.MainForm.ctrlTemplateImage.SetTemplateImage(ProgramCore.Project.FrontImage, false);       // возвращаем как было, после изменения профиля лица
                HeadMode = HeadFront = HeadProfile = HeadFeature = false;

                if (ProgramCore.Project.projectCamera != null) // сохраненная позиция камеры.
                {
                    ProgramCore.MainForm.ctrlRenderControl.camera._dy = ProgramCore.Project.projectCamera._dy;
                    ProgramCore.MainForm.ctrlRenderControl.camera.beta = ProgramCore.Project.projectCamera.beta;
                    ProgramCore.MainForm.ctrlRenderControl.camera.Scale = ProgramCore.Project.projectCamera.Scale;
                    ProgramCore.MainForm.ctrlRenderControl.camera.AfterLoadVoid();
                }

                switch (ProgramCore.CurrentProgram)
                {
                    case ProgramCore.ProgramMode.PrintAhead_PayPal:
                    case ProgramCore.ProgramMode.PrintAhead_Online:
                        ctrlRenderControl.AddBase();
                        var backPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Libraries", "Stages", "Backgrounds", "fire1.jpg");
                        if (File.Exists(backPath))
                            ProgramCore.MainForm.ctrlRenderControl.BackgroundTexture = backPath;
                        break;
                }
                if (UserConfig.ByName("Options")["Tutorials", "Stage", "1"] == "1")
                    frmTutStage.ShowDialog(this);
            }
        }
        public void panelMenuStyle_Click(object sender, EventArgs e)
        {
            if (ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomControlPoints || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomPoints || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomProfilePoints)
                return;

            if (panelMenuStyle.Tag.ToString() == "2")
            {
                // 3.	There’s no reason why when revisiting “Hair Tab” it should remove all previous hair and accessory items. 
                /*  if (sender != null && (ProgramCore.MainForm.ctrlRenderControl.pickingController.HairMeshes.Count > 0 || ProgramCore.MainForm.ctrlRenderControl.pickingController.AccesoryMeshes.Count > 0))         // mean that it's user action
                  {
                      if (MessageBox.Show("This action will remove all changes. Are you sure?", "Attention", MessageBoxButtons.OKCancel) != DialogResult.OK)
                          return;

                      ctrlRenderControl.CleanProjectMeshes();                     // clear all changes and reset position.
                  }*/


                ctrlRenderControl.OrtoTop();

                activePanel = 5;
                panelMenuStyle.Tag = "1";
                panelMenuStyle.Image = Resources.btnMenuStylePressed;
                panelMenuControl.Controls.Clear();
                panelMenuControl.Controls.Add(panelStyles);

                panelMenuCut.Tag = panelMenuShape.Tag = panelMenuAccessories.Tag = panelMenuMaterials.Tag = panelMenuStage.Tag = panelMenuFeatures.Tag = "2";
                panelMenuCut.Image = Resources.btnMenuCutNormal;
                panelMenuShape.Image = Resources.btnMenuShapeNormal;
                panelMenuAccessories.Image = Resources.btnMenuAccessoriesNormal;
                panelMenuMaterials.Image = Resources.btnMenuColorNormal;
                switch (ProgramCore.CurrentProgram)
                {
                    case ProgramCore.ProgramMode.HeadShop_v10_2:
                    case ProgramCore.ProgramMode.HeadShop_v11:
                    case ProgramCore.ProgramMode.HeadShop_Rotator:
                    case ProgramCore.ProgramMode.HeadShop_OneClick:
                        panelMenuStage.Image = Resources.btnMenuStageNormal;
                        break;
                    case ProgramCore.ProgramMode.PrintAhead:
                    case ProgramCore.ProgramMode.PrintAhead_PayPal:
                    case ProgramCore.ProgramMode.PrintAhead_Online:
                        panelMenuStage.Image = Resources.btnMenuPrintNormal;
                        break;
                }

                panelMenuFront.Tag = panelMenuFeatures.Tag = "2";
                panelMenuFront.Image = Resources.btnMenuFrontNormal;
                panelMenuFeatures.Image = Resources.btnMenuFeaturesNormal;

                ProgramCore.MainForm.ctrlRenderControl.StagesDeactivate(-1);

                ctrlRenderControl.Mode = Mode.None;
                ctrlTemplateImage.btnCopyProfileImg.Visible = false;
                ctrlRenderControl.OrtoTop();
                EnableRotating();
                ProgramCore.MainForm.ctrlTemplateImage.SetTemplateImage(ProgramCore.Project.FrontImage);       // возвращаем как было, после изменения профиля лица
                HeadMode = HeadFront = HeadProfile = HeadFeature = false;
                ResetModeTools();

                //             if (frmStyles != null && !frmStyles.Visible)
                //                 styleLibraryOnOpen_Click(null, EventArgs.Empty);

                if (UserConfig.ByName("Options")["Tutorials", "Style", "1"] == "1")
                    frmTutStyle.ShowDialog(this);
            }
        }

        public void panelMenuFront_Click(object sender, EventArgs e)
        {
            if (ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomControlPoints || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomPoints || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomProfilePoints)
                return;

            if (panelMenuFront.Tag.ToString() == "2")
            {
                activePanel = 6;
                panelMenuFront.Tag = "1";
                panelMenuFront.Image = Resources.btnMenuFrontPressed;
                panelMenuControl.Controls.Clear();
                panelMenuControl.Controls.Add(panelFront);

                panelMenuStage.Tag = panelMenuStyle.Tag = panelMenuCut.Tag = panelMenuShape.Tag = panelMenuAccessories.Tag = panelMenuMaterials.Tag = panelMenuFeatures.Tag = "2";
                panelMenuStyle.Image = Resources.btnMenuStyleNormal;
                panelMenuCut.Image = Resources.btnMenuCutNormal;
                panelMenuShape.Image = Resources.btnMenuShapeNormal;
                panelMenuAccessories.Image = Resources.btnMenuAccessoriesNormal;
                panelMenuMaterials.Image = Resources.btnMenuColorNormal;
                switch (ProgramCore.CurrentProgram)
                {
                    case ProgramCore.ProgramMode.HeadShop_v10_2:
                    case ProgramCore.ProgramMode.HeadShop_v11:
                    case ProgramCore.ProgramMode.HeadShop_Rotator:
                    case ProgramCore.ProgramMode.HeadShop_OneClick:
                        panelMenuStage.Image = Resources.btnMenuStageNormal;
                        break;
                    case ProgramCore.ProgramMode.PrintAhead:
                    case ProgramCore.ProgramMode.PrintAhead_PayPal:
                    case ProgramCore.ProgramMode.PrintAhead_Online:
                        panelMenuStage.Image = Resources.btnMenuPrintNormal;
                        break;
                }

                panelMenuFeatures.Image = Resources.btnMenuFeaturesNormal;

                ProgramCore.MainForm.ctrlRenderControl.StagesDeactivate(-1);

                HeadMode = true;
                HeadFront = true;
                HeadProfile = HeadFeature = false;

                if (sender != null)         // иначе это во время загрузки программы. и не надо менять мод!
                    ctrlRenderControl.Mode = Mode.None;

                ctrlTemplateImage.btnCopyProfileImg.Visible = false;
                ctrlRenderControl.OrtoTop();            // поворачиваем морду как надо
                EnableRotating();
                ProgramCore.MainForm.ctrlTemplateImage.SetTemplateImage(ProgramCore.Project.FrontImage, false);       // возвращаем как было, после изменения профиля лица

                //         if (UserConfig.ByName("Options")["Tutorials", "Autodots", "1"] == "1")            // хз зачем тут. есть же кнопка автодотс. устаревшее похоже
                //             frmTutAutodots.ShowDialog(this);
            }
        }
        public void panelMenuFeatures_Click(object sender, EventArgs e)
        {
            if (ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomControlPoints || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomPoints || ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.SetCustomProfilePoints)
                return;

            if (ProgramCore.Project.ManType == ManType.Custom)
            {
                MessageBox.Show(@"Features not available for custom head!", @"HeadShop", MessageBoxButtons.OK);
                return;
            }

            if (panelMenuFeatures.Tag.ToString() == "2")
            {
                activePanel = 7;
                panelFeatures.SetAge(ProgramCore.Project.AgeCoefficient);
                panelFeatures.Setfat(ProgramCore.Project.FatCoefficient);
                panelFeatures.SetSmile(ProgramCore.Project.IsOpenSmile);

                panelMenuFeatures.Tag = "1";
                panelMenuFeatures.Image = Resources.btnMenuFeaturesPressed;
                panelMenuControl.Controls.Clear();
                panelMenuControl.Controls.Add(panelFeatures);

                panelMenuStage.Tag = panelMenuStyle.Tag = panelMenuCut.Tag = panelMenuShape.Tag = panelMenuAccessories.Tag = panelMenuMaterials.Tag = "2";
                panelMenuStyle.Image = Resources.btnMenuStyleNormal;
                panelMenuCut.Image = Resources.btnMenuCutNormal;
                panelMenuShape.Image = Resources.btnMenuShapeNormal;
                panelMenuAccessories.Image = Resources.btnMenuAccessoriesNormal;
                panelMenuMaterials.Image = Resources.btnMenuColorNormal;
                switch (ProgramCore.CurrentProgram)
                {
                    case ProgramCore.ProgramMode.HeadShop_v10_2:
                    case ProgramCore.ProgramMode.HeadShop_v11:
                    case ProgramCore.ProgramMode.HeadShop_Rotator:
                    case ProgramCore.ProgramMode.HeadShop_OneClick:
                        panelMenuStage.Image = Resources.btnMenuStageNormal;
                        break;
                    case ProgramCore.ProgramMode.PrintAhead:
                    case ProgramCore.ProgramMode.PrintAhead_PayPal:
                    case ProgramCore.ProgramMode.PrintAhead_Online:
                        panelMenuStage.Image = Resources.btnMenuPrintNormal;
                        break;
                }

                panelMenuFront.Tag = "2";
                panelMenuFront.Image = Resources.btnMenuFrontNormal;

                ProgramCore.MainForm.ctrlRenderControl.StagesDeactivate(-1);

                HeadMode = HeadFront = HeadProfile = HeadFeature = false;
                HeadFeature = true;
                HeadFront = HeadProfile = false;
                ctrlRenderControl.Mode = Mode.None;
                ctrlTemplateImage.btnCopyProfileImg.Visible = false;
                ctrlRenderControl.OrtoTop();
                EnableRotating();
                ProgramCore.MainForm.ctrlTemplateImage.SetTemplateImage(ProgramCore.Project.FrontImage);       // возвращаем как было, после изменения профиля лица

                if (UserConfig.ByName("Options")["Tutorials", "Features", "1"] == "1")
                    frmTutFeatures.ShowDialog(this);
            }
        }

        #endregion

        #endregion

        #region Supported void's

        private void InitRecentItems()
        {
            mruManager.Initialize(this, ProgramCore.RegistryPath);

            if (mruManager.mruList.Count == 0)
            {
                var recentFile = new ToolStripMenuItem
                {
                    Text = @"Recent File",
                    Enabled = false
                };
                fileToolStripMenuItem.DropDownItems.Add(recentFile);
            }
            else
            {
                foreach (string item in mruManager.mruList)
                {
                    var recentFile = new ToolStripMenuItem
                    {
                        Text = item
                    };
                    recentFile.Click += recentFile_Click;
                    fileToolStripMenuItem.DropDownItems.Add(recentFile);
                }
            }

            var separator = new ToolStripSeparator();
            fileToolStripMenuItem.DropDownItems.Add(separator);

            var exitBtn = new ToolStripMenuItem
            {
                Text = @"Exit"
            };
            exitBtn.Click += exitBtn_Click;
            fileToolStripMenuItem.DropDownItems.Add(exitBtn);
        }
        private void InitializeTutorialLinks()
        {
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.PrintAhead:
                case ProgramCore.ProgramMode.PrintAhead_Online:
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                    UserConfig.ByName("Tutorials")["Links", "Start"] = "http://youtu.be/JC5z64YP1xA";
                    UserConfig.ByName("Tutorials")["Links", "Recognize"] = "https://www.youtube.com/watch?v=AjG09RGgHvw";
                    UserConfig.ByName("Tutorials")["Links", "Shapedots"] = "http://youtu.be/pIlrJUByJj8";
                    UserConfig.ByName("Tutorials")["Links", "Profile"] = "http://youtu.be/Olc7oeQUmWk";
                    UserConfig.ByName("Tutorials")["Links", "Mirror"] = "http://youtu.be/JC5z64YP1xA";
                    UserConfig.ByName("Tutorials")["Links", "LineTool"] = "https://www.youtube.com/watch?v=c7YbRsm8m9I";
                    UserConfig.ByName("Tutorials")["Links", "Freehand"] = "http://youtu.be/c2Yvd2DaiDg";
                    UserConfig.ByName("Tutorials")["Links", "CustomHeads"] = "http://youtu.be/H9dqNF4HdMQ";
                    UserConfig.ByName("Tutorials")["Links", "Autodots"] = "http://youtu.be/JC5z64YP1xA";

                    UserConfig.ByName("Tutorials")["Links", "Style"] = "https://www.youtube.com/watch?v=AjG09RGgHvw";
                    UserConfig.ByName("Tutorials")["Links", "Stage"] = "https://www.youtube.com/watch?v=AjG09RGgHvw";
                    UserConfig.ByName("Tutorials")["Links", "Shape"] = "https://www.youtube.com/watch?v=AjG09RGgHvw";
                    UserConfig.ByName("Tutorials")["Links", "Material"] = "https://www.youtube.com/watch?v=AjG09RGgHvw";
                    UserConfig.ByName("Tutorials")["Links", "Cut"] = "https://www.youtube.com/watch?v=AjG09RGgHvw";
                    UserConfig.ByName("Tutorials")["Links", "Accessory"] = "https://www.youtube.com/watch?v=AjG09RGgHvw";

                    UserConfig.ByName("Tutorials")["Links", "AdvancedManual"] = "https://youtu.be/gWOkSUDbv0I";
                    UserConfig.ByName("Tutorials")["Links", "QuickStart"] = "https://youtu.be/8cejdijABQY";
                    break;
                case ProgramCore.ProgramMode.HeadShop_v10_2:
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.HeadShop_OneClick:
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                    UserConfig.ByName("Tutorials")["Links", "Start"] = "https://www.youtube.com/watch?v=0baUErHwngA";
                    UserConfig.ByName("Tutorials")["Links", "Recognize"] = "https://www.youtube.com/watch?v=AjG09RGgHvw";
                    UserConfig.ByName("Tutorials")["Links", "Shapedots"] = "http://youtu.be/pIlrJUByJj8";
                    UserConfig.ByName("Tutorials")["Links", "Profile"] = "https://youtu.be/IFvfVc-81M4";
                    UserConfig.ByName("Tutorials")["Links", "Mirror"] = "http://youtu.be/JC5z64YP1xA";
                    UserConfig.ByName("Tutorials")["Links", "LineTool"] = " https://youtu.be/dzLtPdsmtMY";
                    UserConfig.ByName("Tutorials")["Links", "Freehand"] = "https://youtu.be/-Lg6AWr17gU";
                    UserConfig.ByName("Tutorials")["Links", "CustomHeads"] = "http://youtu.be/H9dqNF4HdMQ";
                    UserConfig.ByName("Tutorials")["Links", "Autodots"] = "https://youtu.be/Cxi_eTyCh-Y";

                    UserConfig.ByName("Tutorials")["Links", "Style"] = "https://youtu.be/8_7CnBSW85M";
                    UserConfig.ByName("Tutorials")["Links", "Stage"] = "https://www.youtube.com/watch?v=AjG09RGgHvw";
                    UserConfig.ByName("Tutorials")["Links", "Shape"] = "https://www.youtube.com/watch?v=AjG09RGgHvw";
                    UserConfig.ByName("Tutorials")["Links", "Material"] = "https://youtu.be/zHA7_1ODIl0";
                    UserConfig.ByName("Tutorials")["Links", "Cut"] = "https://www.youtube.com/watch?v=AjG09RGgHvw";
                    UserConfig.ByName("Tutorials")["Links", "Accessory"] = "https://youtu.be/UeQljfKlNG8";

                    UserConfig.ByName("Tutorials")["Links", "AdvancedManual"] = "https://youtu.be/z8rnXteNnm0";
                    UserConfig.ByName("Tutorials")["Links", "QuickStart"] = "https://youtu.be/7sovzyHCnRY";

                    UserConfig.ByName("Tutorials")["Links", "Features"] = "https://youtu.be/_hADE739X9w";
                    UserConfig.ByName("Tutorials")["Links", "Export"] = "https://youtu.be/aPsJOD1Nroc";
                    UserConfig.ByName("Tutorials")["Links", "3DPrinting"] = "https://youtu.be/A_MQCNI4E8U";
                    break;
            }
        }

        public void SuccessPay(PrintType printType)
        {
            switch (printType)
            {
                case PrintType.STL:
                    ExportSTL();
                    break;
                case PrintType.Collada:
                    ExportDAE();
                    break;
            }
        }
        public void BadPay()
        {

            MessageBox.Show(@"Payment was failed!");
        }
        /// <summary> Формат stl 5$</summary>
        public void ExportSTL()
        {
            string fiName;
            string stlName;

            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.PrintAhead:
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                case ProgramCore.ProgramMode.PrintAhead_Online:
                    {
                        var ctrl = new ctrlPrintAheadExport(IntPtr.Zero);
                        if (ProgramCore.ShowDialog(ctrl, "STL print", MessageBoxButtons.OKCancel, false) != DialogResult.OK)
                            return;

                        stlName = Path.Combine(ctrl.Path, ctrl.ModelName + ".stl");
                        fiName = Path.Combine(ctrl.Path, ctrl.ModelName + ".obj");
                    }
                    break;
                default:
                    {
                        using (var ofd = new FolderDialogEx())
                        {
                            if (ofd.ShowDialog(Handle) != DialogResult.OK)
                                return;
                            stlName = Path.Combine(ofd.SelectedFolder[0], ProgramCore.Project.ProjectName + ".stl");
                            fiName = Path.Combine(ofd.SelectedFolder[0], ProgramCore.Project.ProjectName + ".obj");
                        }
                    }
                    break;
            }

            if (ProgramCore.Project != null)
            {
                ctrlRenderControl.pickingController.SelectedMeshes.Clear();
                ProgramCore.Project.ToStream();
            }

            var meshInfos = ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.Parts.Select(part => new MeshInfo(part)).ToList();

            ObjSaver.ExportMergedModel(fiName, ProgramCore.MainForm.ctrlRenderControl.pickingController.HairMeshes,
                ProgramCore.MainForm.ctrlRenderControl.pickingController.AccesoryMeshes, meshInfos,
               ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.RealScale, ProgramCore.Project.ProjectName);

            var importer = new AssimpImporter();
            importer.ConvertFromFileToFile(fiName, stlName, "stl");
        }
        /// <summary> Формат dae. (collada) </summary>
        public void ExportDAE()
        {
            string fiName;
            string daeName;
            string newDirectory;
            string modelName;

            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.PrintAhead:
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                case ProgramCore.ProgramMode.PrintAhead_Online:
                    {
                        var ctrl = new ctrlPrintAheadExport(Handle);
                        if (ProgramCore.ShowDialog(ctrl, "DAE print", MessageBoxButtons.OKCancel, false) != DialogResult.OK)
                            return;

                        modelName = ctrl.ModelName;

                        newDirectory = Path.Combine(ctrl.Path, "SmoothedModelTextures");
                        FolderEx.CreateDirectory(newDirectory);

                        daeName = Path.Combine(newDirectory, modelName + ".dae");
                        fiName = Path.Combine(newDirectory, modelName + ".obj");
                    }
                    break;
                default:
                    {
                        using (var ofd = new FolderDialogEx())
                        {
                            if (ofd.ShowDialog(Handle) != DialogResult.OK)
                                return;

                            if (ofd.SelectedFolder[0] == ProgramCore.Project.ProjectPath)
                            {
                                MessageBox.Show(@"Can't export file to project directory.", @"Warning");
                                return;
                            }

                            modelName = ProgramCore.Project.ProjectName;

                            newDirectory = Path.Combine(ofd.SelectedFolder[0], "SmoothedModelTextures");
                            FolderEx.CreateDirectory(newDirectory);

                            daeName = Path.Combine(newDirectory, modelName + ".dae");
                            fiName = Path.Combine(newDirectory, modelName + ".obj");
                        }
                    }
                    break;
            }

            //var tempScale = 5f;
            var realScale = ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.RealScale;
            if (ProgramCore.PluginMode)
            {
                var scale = 1f;
                switch (ProgramCore.Project.ManType)
                {
                    case ManType.Male:
                        scale = ProgramCore.Project.RenderMainHelper.headMeshesController.SetSize(29.9421043f); // подгонка размера 
                        break;
                    case ManType.Female:
                        scale = ProgramCore.Project.RenderMainHelper.headMeshesController.SetSize(29.3064537f); // подгонка размера 
                        break;
                    case ManType.Child:
                        scale = ProgramCore.Project.RenderMainHelper.headMeshesController.SetSize(25.6209984f); // подгонка размера 
                        break;
                }

                //tempScale = ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.MorphScale;
                //ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.MorphScale /= scale;
                realScale /= scale;
            }


            if (ProgramCore.Project != null)
            {
                ctrlRenderControl.pickingController.SelectedMeshes.Clear();
                ProgramCore.Project.ToStream();
            }
            var morphK = float.IsNaN(ProgramCore.Project.MorphingScale) ? 0.9f : ProgramCore.Project.MorphingScale;
            ProgramCore.MainForm.ctrlRenderControl.DoMorth(morphK);      // чтобы не потерять Smoothing

            Process.Start("http://www.shapeways.com/");

            var meshInfos = new List<MeshInfo>();
            foreach (var part in ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.Parts)
                meshInfos.Add(new MeshInfo(part));
            MeshInfo.FindCenter(meshInfos, "Меши бошки до экспорта frmMain::ExportCollada()");
            ProgramCore.EchoToLog($"realScale frmMain::ExportCollada(): {realScale}", EchoMessageType.Information);
            ObjSaver.ExportMergedModel(fiName, ProgramCore.MainForm.ctrlRenderControl.pickingController.HairMeshes,
                ProgramCore.MainForm.ctrlRenderControl.pickingController.AccesoryMeshes, meshInfos, realScale, ProgramCore.Project.ProjectName, true, true);

            var importer = new AssimpImporter();
            importer.ConvertFromFileToFile(fiName, daeName, "collada");


            if (ProgramCore.Project.FrontImage != null)
                ProgramCore.Project.FrontImage.Save(Path.Combine(newDirectory, "tempHaarImage.jpg"));
            if (ProgramCore.Project.ProfileImage != null)
                ProgramCore.Project.ProfileImage.Save(Path.Combine(newDirectory, "ProfileImage.jpg"));

            File.Delete(fiName);
            var mtlName = Path.Combine(newDirectory, modelName + ".mtl");
            if (File.Exists(mtlName))
                File.Delete(mtlName);

            using (var zip = new ZipFile())
            {
                zip.AddFiles(Directory.GetFiles(newDirectory), false, "");

                foreach (var dir in Directory.GetDirectories(newDirectory))
                {
                    var files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        if (!zip.ContainsEntry((new FileInfo(file)).Name))
                            zip.AddFile(file, "");
                    }
                }

                zip.Save(Path.Combine(newDirectory, modelName + ".zip"));
            }

            #region Удаление всего, кроме архива

            foreach (var file in Directory.GetFiles(newDirectory))
            {
                if (Path.GetExtension(file) == ".zip")
                    continue;

                if (File.Exists(file))
                    File.Delete(file);
            }

            foreach (var dir in Directory.GetDirectories(newDirectory))
            {
                var files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    if (Path.GetExtension(file) == ".zip")
                        continue;

                    if (File.Exists(file))
                        File.Delete(file);
                }
            }

            #endregion

            //if (ProgramCore.PluginMode)
            //    ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.MorphScale = tempScale;

            MessageBox.Show(@"Color 3D export finished!", @"Done");
        }
        /// <summary> Экспорт на кнопки плюс экспорт ПЛАГИНА! </summary>
        public void Export()
        {
            string fiName;
            var diName = string.Empty;
            var specialExportPath = string.Empty;
            var tempScale = 5f;
            if (ProgramCore.PluginMode)
            {
                tempScale = ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.MorphScale;
                ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.MorphScale = 1;          // чтобы скейл в проге не влиял на экспорт.

                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                diName = Path.Combine(appDataPath, @"DAZ 3D\Studio4\temp\FaceShop\");
                specialExportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "My DAZ 3D Library", "Runtime", "FaceShop", "fs");      // сделано специально. т.к. для плагина ПЛАГИН утаскивает только часть файлов. Остальное - придется вручную сохранять по этому пути.
                fiName = Path.Combine(diName, "fs3d.obj");
            }
            else
            {
                using (var ofd = new FolderDialogEx())
                {
                    if (ofd.ShowDialog(Handle) != DialogResult.OK)
                        return;
                    fiName = Path.Combine(ofd.SelectedFolder[0], ProgramCore.Project.ProjectName + ".obj");
                }
            }

            ctrlRenderControl.pickingController.SelectedMeshes.Clear();
            var acDirPath = Path.GetDirectoryName(fiName);

            #region Выгрузка волос и аксессуаров

            var exportDirectory = string.IsNullOrWhiteSpace(specialExportPath) ? ProgramCore.Project.ProjectPath : specialExportPath;

            var haPath = Path.GetFileNameWithoutExtension(fiName) + "hair.obj";
            var hairPath = Path.Combine(exportDirectory, haPath);
            var realScale = ProgramCore.PluginMode ? 1.0f : ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.RealScale;

            ObjSaver.SaveObjFile(hairPath, ctrlRenderControl.pickingController.HairMeshes, MeshType.Hair, realScale, ProgramCore.Project.ManType, ProgramCore.Project.ProjectName, true);

            if (ProgramCore.MainForm.ctrlRenderControl.pickingController.AccesoryMeshes.Count > 0)            // save accessories to separate file
            {
                var acName = Path.GetFileNameWithoutExtension(fiName) + "_accessory.obj";

                var accessoryPath = Path.Combine(exportDirectory, acName);
                ObjSaver.SaveObjFile(accessoryPath, ctrlRenderControl.pickingController.AccesoryMeshes, MeshType.Accessory, realScale, ProgramCore.Project.ManType, ProgramCore.Project.ProjectName, true);
            }


            #endregion


            ctrlRenderControl.SaveHead(fiName, true);
            if (PluginMode)
            {
                ProgramCore.Project.ProjectPath = specialExportPath;        //3.	Need to deposit all texture files into Users/Public/Documents/My DAZ Library/Runtime/Faceshop/fs folder, including smoothed eye, mouth and teeth. Right now only face (fs.bmp) is deposited.
                if (ProgramCore.Project.AutodotsUsed)
                   ctrlRenderControl.SaveBlendingTextures();
                ctrlRenderControl.SaveSmoothedTextures();
            }

            if (ProgramCore.PluginMode)
            {
                var dsxPath = Path.Combine(Application.StartupPath, "Plugin", "fs3d.dsx");
                File.Copy(dsxPath, Path.Combine(diName, "fs3d.dsx"), true);

                var fsbmPath = Path.Combine(Application.StartupPath, "Plugin", "fsbm.bmp");
                File.Copy(fsbmPath, Path.Combine(diName, "fsbm.bmp"), true);

                var mtlPath = Path.Combine(Application.StartupPath, "Plugin", "fs3d.mtl");
                File.Copy(mtlPath, Path.Combine(diName, "fs3d.mtl"), true);

                var iTexture = -1;

                foreach (var part in ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.Parts)
                {
                    if (ProgramCore.MainForm.PluginUvGroups.Contains(part.Name.ToLower().Trim()))
                    {
                        var smoothTexs = ctrlRenderControl.SmoothedTextures.Where(s => s.Key != 0 && s.Value == part.Texture);
                        if (smoothTexs.Any())
                        {
                            iTexture = smoothTexs.First().Value;
                            break;
                        }
                    }
                }
                if (iTexture == -1)
                {
                    iTexture = ctrlRenderControl.SmoothedTextures.Count > 0 ? ctrlRenderControl.SmoothedTextures.Values.ElementAt(0) : ctrlRenderControl.HeadTextureId;
                }

                var mapPath = ctrlRenderControl.GetTexturePath(iTexture);
                switch (ProgramCore.CurrentProgram)
                {
                    // case ProgramCore.ProgramMode.HeadShop_v11:
                    case ProgramCore.ProgramMode.HeadShop_Rotator:      // HeadShop 11 - выгрузка текстур с размерами 4096.
                    case ProgramCore.ProgramMode.HeadShop_OneClick:      // для этой программы должна быть выгрузка текстур с размерами 2048
                        {
                            var actualTextureSize = ProgramCore.CurrentProgram == ProgramCore.ProgramMode.HeadShop_OneClick ? 2048 : 4096;
                            if (ProgramCore.MainForm.ctrlRenderControl.brushTextures.ContainsKey(iTexture))            // применяем результаты кисточки
                            {
                                var brushTexture = ProgramCore.MainForm.ctrlRenderControl.brushTextures[iTexture];
                                using (var bitmap = new Bitmap(mapPath))
                                {
                                    var max = (float)Math.Max(bitmap.Width, bitmap.Height);
                                    if (max != actualTextureSize)
                                    {
                                        var k = actualTextureSize / max;
                                        var newImg = ImageEx.ResizeImage(bitmap, new Size((int)(bitmap.Width * k), (int)(bitmap.Height * k)));

                                        using (var grfx = Graphics.FromImage(newImg))
                                            grfx.DrawImage(brushTexture.TextureData, 0, 0, newImg.Width, newImg.Height);

                                        newImg.Save(Path.Combine(diName, "fs3d.bmp"), ImageFormat.Bmp);
                                    }
                                    else
                                    {
                                        using (var grfx = Graphics.FromImage(bitmap))
                                            grfx.DrawImage(brushTexture.TextureData, 0, 0);

                                        bitmap.Save(Path.Combine(diName, "fs3d.bmp"), ImageFormat.Bmp);
                                    }
                                }
                            }
                            else
                            {
                                using (var img = new Bitmap(mapPath)) // Don't use using!!
                                {
                                    var max = (float)Math.Max(img.Width, img.Height);
                                    if (max != actualTextureSize)
                                    {
                                        var k = actualTextureSize / max;
                                        var newImg = ImageEx.ResizeImage(img, new Size((int)(img.Width * k), (int)(img.Height * k)));
                                        newImg.Save(Path.Combine(diName, "fs3d.bmp"), ImageFormat.Bmp);
                                    }
                                    else
                                        img.Save(Path.Combine(diName, "fs3d.bmp"), ImageFormat.Bmp);
                                }
                            }
                        }
                        break;
                    default:
                        {
                            if (ProgramCore.MainForm.ctrlRenderControl.brushTextures.ContainsKey(iTexture))            // применяем результаты кисточки
                            {
                                var brushTexture = ProgramCore.MainForm.ctrlRenderControl.brushTextures[iTexture];
                                using (var bitmap = new Bitmap(mapPath))
                                {
                                    using (var grfx = Graphics.FromImage(bitmap))
                                        grfx.DrawImage(brushTexture.TextureData, 0, 0);
                                    bitmap.Save(Path.Combine(diName, "fs3d.bmp"), ImageFormat.Bmp);
                                }
                            }
                            else
                            {
                                using (var bmp = new Bitmap(mapPath))
                                    bmp.Save(Path.Combine(diName, "fs3d.bmp"), ImageFormat.Bmp);
                            }
                        }
                        break;
                }

                var di = new DirectoryInfo(acDirPath);
                foreach (var file in di.GetFiles())
                {
                    var now = DateTime.Now;
                    file.CreationTime = now;
                    file.LastAccessTime = now;
                    file.LastWriteTime = now;
                }

                ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.MorphScale = tempScale;
            }

            MessageBox.Show(ProgramCore.ProgramCaption + @" project successfully exported!", @"Done", MessageBoxButtons.OK);
            if (ProgramCore.PluginMode)
                Environment.Exit(0);
        }

        public void ChangeCursors(Cursor cursor)
        {
            Cursor = cursor;
            ctrlTemplateImage.Cursor = cursor;
            ctrlRenderControl.Cursor = cursor;
        }
        public void EnableRotating()
        {
            checkArrow.Enabled = true;
            rotateLeftcontinuousRotateToolStripMenuItem.Enabled = true;
            rotateRightcontinuousRotateToolStripMenuItem.Enabled = true;
            turnLeftoneStepToolStripMenuItem.Enabled = true;
            turnRightoneStepToolStripMenuItem.Enabled = true;

            ctrlRenderControl.panelOrtoLeft.Enabled = ctrlRenderControl.panelOrtoRight.Enabled = true;
        }
        public void DisableRotating()
        {
            if (checkArrow.Tag.ToString() == "1")
                checkArrow_Click(this, EventArgs.Empty);
            if (ctrlRenderControl.TimerMode == Mode.TimerTurnLeft || ctrlRenderControl.TimerMode == Mode.TimerTurnRight)
            {
                ctrlRenderControl.workTimer.Stop();
                ctrlRenderControl.TimerMode = Mode.None;
            }

            checkArrow.Enabled = false;
            rotateLeftcontinuousRotateToolStripMenuItem.Enabled = false;
            rotateRightcontinuousRotateToolStripMenuItem.Enabled = false;
            turnLeftoneStepToolStripMenuItem.Enabled = false;
            turnRightoneStepToolStripMenuItem.Enabled = false;

            ctrlRenderControl.panelOrtoLeft.Enabled = ctrlRenderControl.panelOrtoRight.Enabled = false;
        }
        internal void ResetModeTools()
        {
            panelShape?.ResetModeTools();
            panelCut?.ResetModeTools();
            panelFront?.ResetModeTools();

            ctrlRenderControl.ResetModeTools();
        }
        private void ResetScaleModeTools()
        {
            if (checkHand.Tag.ToString() == "1")
                checkHand_Click(this, EventArgs.Empty);
            if (checkZoom.Tag.ToString() == "1")
                checkZoom_Click(this, EventArgs.Empty);
            if (checkArrow.Tag.ToString() == "1")
                checkArrow_Click(this, EventArgs.Empty);
        }

        #region ProgressProc

        private bool isProgress;
        private DateTime lastUpdateDateTime = DateTime.MinValue;
        private void ProgressProc(object sender, ProgressProcEventArgs e)
        {
            var now = DateTime.Now;
            if (!isProgress)
            {
                StartProgress();
            }
            if (now - lastUpdateDateTime > TimeSpan.FromMilliseconds(40))
            {
                Application.DoEvents();
                lastUpdateDateTime = now;
            }
            ProgressManager.ProgressProc(sender, e);
        }
        private readonly ProgressMessageFilter progressMessageFilter = new ProgressMessageFilter();
        private void StartProgress()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                Application.AddMessageFilter(progressMessageFilter);
                ProgramCore.AddCallStackReleasedProc(_stopProgress);
                isProgress = true;
            }
            catch (Exception ex)
            {
                ProgramCore.EchoToLog(ex);
            }
        }
        private void StopProgress()
        {
            try
            {
                Application.RemoveMessageFilter(progressMessageFilter);
                Cursor = Cursors.Default;
                isProgress = false;
            }
            catch (Exception ex)
            {
                ProgramCore.EchoToLog(ex);
            }
        }
        private void _stopProgress(object sender, EventArgs e)
        {
            StopProgress();
        }
        class ProgressMessageFilter : IMessageFilter
        {
            public bool PreFilterMessage(ref Message m)
            {
                if (m.HWnd == ProgressManager.ProgressHWnd || m.HWnd == ProgressManager.ProgressCancelButtonHWnd)
                    return false;
                return (m.Msg != WMConsts.WM_TIMER && (m.Msg >= WMConsts.WM_KEYFIRST && m.Msg <= WMConsts.WM_KEYLAST || m.Msg >= WMConsts.WM_MOUSEFIRST && m.Msg <= WMConsts.WM_MOUSELAST));
            }
        }

        #endregion

        #endregion

        #region Project

        private static void DirectoryCopy(string sourceDirName, string destDirName)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            var dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            foreach (var subdir in dirs)
            {
                var temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.HeadShop_v10_2:
                case ProgramCore.ProgramMode.HeadShop_v11:
                case ProgramCore.ProgramMode.HeadShop_Rotator:
                case ProgramCore.ProgramMode.HeadShop_OneClick:
                case ProgramCore.ProgramMode.PrintAhead:
                    {
                        var frm = new frmNewProject4PrintAhead(false);
                        frm.ShowDialog();
                        if (frm.dialogResult != DialogResult.OK)
                            return;

                        frm.CreateProject();
                    }
                    break;
            }
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialogEx("Open HeadShop/HairShop project", "HeadShop projects|*.hds|HairShop projects|*.hs"))
            {
                if (ofd.ShowDialog(false) != DialogResult.OK)
                    return;

                OpenProject(ofd.FileName);
            }
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ProgramCore.Project != null)
            {
                ctrlRenderControl.pickingController.SelectedMeshes.Clear();
                ProgramCore.Project.ToStream();
                MessageBox.Show(@"Project successfully saved!", @"Done", MessageBoxButtons.OK);
            }
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ProgramCore.Project == null)
                return;

            using (var sfd = new SaveFileDialogEx("Save as HeadShop project", "HeadShop projects|*.hs|OBJ Hair|*.obj|DAE model|*.dae"))
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    switch (sfd.FilterIndex)
                    {
                        case 1:
                            #region All project

                            var fullPath = sfd.FileName;
                            var projectName = Path.GetFileNameWithoutExtension(fullPath);
                            var projectPath = Path.Combine(Path.GetDirectoryName(fullPath), projectName);

                            var newDirectoryPath = Path.Combine(projectPath, "Model");
                            var directoryPath = Path.Combine(ProgramCore.Project.ProjectPath, "Model");

                            DirectoryCopy(directoryPath, newDirectoryPath);

                            var frontImage = ProgramCore.Project.FrontImagePath;
                            var newFrontImage = Path.Combine(projectPath, Path.GetFileName(frontImage));
                            File.Copy(Path.Combine(ProgramCore.Project.ProjectPath, frontImage), newFrontImage);

                            ProgramCore.Project.ProjectName = projectName;
                            ProgramCore.Project.ProjectPath = projectPath;
                            ProgramCore.Project.HeadModelPath = Path.Combine(projectPath, "Model", Path.GetFileName(ProgramCore.Project.HeadModelPath));
                            ProgramCore.Project.ToStream();
                            MessageBox.Show(@"Project successfully saved!", @"Done", MessageBoxButtons.OK);

                            #endregion
                            break;
                        case 2:
                            Export();
                            break;
                        case 3:

                            #region Копируем модель

                            File.Copy(ProgramCore.Project.HeadModelPath, sfd.FileName, true);           // сама модель

                            #region Обрабатываем mtl файл и папку с текстурами

                            var oldFileName = Path.GetFileNameWithoutExtension(ProgramCore.Project.HeadModelPath);
                            var mtl = oldFileName + ".mtl";
                            using (var ms = new StreamReader(ProgramCore.Project.HeadModelPath))
                            {
                                for (var i = 0; i < 10; i++)
                                {
                                    if (ms.EndOfStream)
                                        break;
                                    var line = ms.ReadLine();
                                    if (line.ToLower().Contains("mtllib"))          // ищем ссылку в obj файле на mtl файл (у них могут быть разные названия, но всегда в одной папке
                                    {
                                        var lines = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                        if (lines.Length > 1)
                                        {
                                            mtl = lines[1];
                                            break;
                                        }
                                    }
                                }
                            }

                            ObjLoader.CopyMtl(mtl, mtl, Path.GetDirectoryName(ProgramCore.Project.HeadModelPath), "", Path.GetDirectoryName(sfd.FileName), ProgramCore.Project.TextureSize);

                            #endregion

                            MessageBox.Show(@"Model successfully exported!", @"Done", MessageBoxButtons.OK);

                            #endregion



                            break;
                    }
                }
            }
        }
        private void recentFile_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (item != null)
                OpenProject(item.Text);
        }


        public void UpdateProjectControls(bool newProject, RectangleAABB aabb = null)
        {
            if (ProgramCore.Project == null)
            {
                ProgramCore.MainForm.ctrlTemplateImage.SetTemplateImage(null);
            }
            else
            {
                ctrlRenderControl.LoadProject(newProject, aabb);
                ctrlRenderControl.camera.UpdateDy();

                if (panelCut != null && panelStyles != null)
                {
                    if (ProgramCore.MainForm.ctrlRenderControl.pickingController.HairMeshes.Count == 0)
                        panelMenuStyle_Click(null, EventArgs.Empty);
                    else
                        panelMenuCut_Click(null, EventArgs.Empty);
                }

                frmStages?.InitializeListView();

                if (ProgramCore.Project.FrontImage == null)
                    ProgramCore.MainForm.ctrlTemplateImage.SetTemplateImage(null);
                else
                {
                    using (var bmp = new Bitmap(ProgramCore.Project.FrontImage))
                        ProgramCore.MainForm.ctrlTemplateImage.SetTemplateImage((Bitmap)bmp.Clone(), false);
                }

                if (ProgramCore.Project.ManType != ManType.Custom)
                {

                    ctrlRenderControl.autodotsShapeHelper.TransformRects();
                    ctrlRenderControl.autodotsShapeHelper.InitializeShaping();

                    switch (ProgramCore.CurrentProgram)
                    {
                        case ProgramCore.ProgramMode.HeadShop_v10_2:
                            break;
                        default:
                            {
                                ProgramCore.MainForm.ctrlRenderControl.PhotoLoaded();
                            }
                            break;
                    }
                }
                else
                {
                    ProgramCore.MainForm.ctrlRenderControl.camera.ResetCamera(true);
                }



                if (newProject && ProgramCore.Project.ManType == ManType.Custom)
                    ctrlRenderControl.camera.ResetCamera(true);

            }

            if (frmPrint != null || frmStages != null)
            {
                ctrlRenderControl.StagesActivate();     // for recalc
                ctrlRenderControl.StagesDeactivate(0);
            }

            frmParts?.UpdateList();
        }
        private void OpenProject(string fileName)
        {
            ProgramCore.Project = Project.FromStream(fileName);
            UpdateProjectControls(false);

            ProgramCore.MainForm.ctrlRenderControl.InitializeShapedotsHelper();         // инициализация точек головы.
            ProgramCore.MainForm.ctrlTemplateImage.RecalcProfilePoints();            // пидоры сломали все. инициализируем профиль.

            if (ProgramCore.Project.AgeCoefficient != 0 || ProgramCore.Project.FatCoefficient != 0)  // восстанавливаем морфинги
            {
                foreach (var m in ProgramCore.MainForm.ctrlRenderControl.OldMorphing)
                    m.Value.Delta = ProgramCore.Project.AgeCoefficient;
                foreach (var m in ProgramCore.MainForm.ctrlRenderControl.FatMorphing)
                    m.Value.Delta = ProgramCore.Project.FatCoefficient;
                ProgramCore.MainForm.ctrlRenderControl.DoMorth();
            }

            if (!float.IsNaN(ProgramCore.Project.MorphingScale))
                ctrlRenderControl.DoMorth(ProgramCore.Project.MorphingScale);

            MessageBox.Show(@"Project successfully loaded!", @"Done");
            mruManager.Add(fileName);
        }

        #endregion

    }

    #region Enum's

    public enum PrintType
    {
        STL,            //5$
        Collada         // zip archive 8$
    }

    #endregion
}
