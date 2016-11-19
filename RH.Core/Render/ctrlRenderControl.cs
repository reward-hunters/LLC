﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using RH.Core.Controls;
using RH.Core.Helpers;
using RH.Core.IO;
using RH.Core.Render.Controllers;
using RH.Core.Render.Helpers;
using RH.Core.Render.Meshes;
using RH.Core.Render.Obj;
using RH.MeshUtils;
using RH.MeshUtils.Data;
using RH.MeshUtils.Helpers;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using StringConverter = RH.Core.Helpers.StringConverter;

namespace RH.Core.Render
{
    public partial class ctrlRenderControl : UserControlEx
    {
        #region Var

        private float time = 0;
        private int mX;
        private int mY;
        private bool leftMousePressed;
        private bool shiftPressed;
        private bool dblClick;

        private bool loaded;
        private bool startPress = true;

        public readonly Camera camera = new Camera();
        public readonly Dictionary<String, TextureInfo> textures = new Dictionary<String, TextureInfo>();
        private List<int> baseProfilePoints = new List<int>();
        internal BrushTool brushTool = new BrushTool();

        private readonly Panel renderPanel = new Panel();
        private GraphicsContext graphicsContext;
        private IWindowInfo windowInfo;

        public readonly Dictionary<string, DynamicRenderMeshes> PartsLibraryMeshes = new Dictionary<string, DynamicRenderMeshes>();         // потому что можем выбрать несколько мешей и запихать их как один в партс-лайбрари

        private bool playAnimation;
        public bool PlayAnimation
        {
            get
            {
                return playAnimation;
            }
            set
            {
                playAnimation = value;
                if (playAnimation && animationController.СurrentAnimation != null)
                    soundController.Play(animationController.СurrentAnimation.Sound);
                else
                    soundController.Stop();
            }
        }
        private ShaderController idleShader;
        private ShaderController blendShader;
        private ShaderController brushShader;

        public bool IsShapeChanged = false;
        public ShapeController shapeController = new ShapeController();
        public ShapeController shapeControllerMirror = new ShapeController();
        public readonly HeadShapeController HeadShapeController = new HeadShapeController();
        public AnimationController animationController = new AnimationController();
        public readonly SoundController soundController = new SoundController();
        public SliceController sliceController = new SliceController();
        public HistoryController historyController = new HistoryController();

        public readonly HeadController headController = new HeadController();
        public readonly HeadMeshesController headMeshesController = new HeadMeshesController();
        public readonly AutodotsShapeHelper autodotsShapeHelper = new AutodotsShapeHelper();

        private Mode mode;
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Mode Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
                if (mode == Mode.None && ProgramCore.MainForm != null)
                    ProgramCore.MainForm.ResetModeTools();
            }
        }

        public ScaleMode ScaleMode = ScaleMode.None;
        public Mode TimerMode = Mode.None;

        public ToolsMode ToolsMode = ToolsMode.HairLine;
        public bool ToolMirrored
        {
            get;
            set;
        }
        public bool ToolShapeMirrored;

        public PickingController pickingController
        {
            get;
            private set;
        }

        public bool SimpleDrawing;
        private bool profileTrianglesDrowing;

        private Vector3 meshPoint;
        private float HeadShapeZ;
        public Vector2 HeadShapeP;
        public bool UseTexture = true;
        public MeshPartType HeadLineMode = MeshPartType.None;

        internal Dictionary<DynamicRenderMesh, Tuple<Vector3, Matrix4>> meshTempItems = new Dictionary<DynamicRenderMesh, Tuple<Vector3, Matrix4>>();      // meshTempPosition && meshTempTransform

        private Vector2 accessoryRotateCenterCirclePoint;             // center circle of rotation
        private Vector2 accessoryRotateCircleMousePoint;              // cursor position on circle (for initial rotate)
        private Vector2 accessoryRotateMousePoint;                    // current cursor position during the rotation
        private Vector3 accessoryRotateCenterPoint;                   // rotation center
        private float accessoryRotateRadius;                          // circle radius

        private Vector2 hairRectPointA;                              // first point on rectangle selection
        private Vector2 hairRectPointB;                              // second point on rectangle selection
        public Dictionary<int, Vector2> HairRect = new Dictionary<int, Vector2>();
        private int hairSelectedPoint;

        private bool bHaveMouse;
        private Point startMousePoint;

        public List<Vector2> LassoPoints = new List<Vector2>();

        /// <summary> Дефолтовая текстура головы </summary>
        public int HeadTextureId;

        private Bitmap headTexture;
        private Bitmap HeadTexture
        {
            get
            {
                return headTexture;
            }
            set
            {
                headTexture = value;
                //       mixedTexture = new Bitmap(headTexture.Width, headTexture.Height);
            }
        }

        /// <summary> Текстура, отраженная слева направо </summary>
        private Bitmap reflectedLeft;
        /// <summary> Текстура, отраженная справа налево </summary>
        private Bitmap reflectedRight;

        public RectangleF ProfileFaceRect;

        private Dictionary<int, Bitmap> tempBitmaps = new Dictionary<int, Bitmap>();
        private List<Sprite2D> customBasePointsSprites = new List<Sprite2D>();
        public List<Sprite2D> profilePointSprites = new List<Sprite2D>();

        public Dictionary<int, int> SmoothedTextures = new Dictionary<int, int>();

        public Dictionary<Guid, PartMorphInfo> OldMorphing = null;
        public Dictionary<Guid, PartMorphInfo> FatMorphing = null;
        public Dictionary<Guid, PartMorphInfo> PoseMorphing = null;

        #region Var custom points

        private Vector2 customLastPoint;
        private Vector2 customLastRectPoint;
        private List<HeadPoint> customTempPoints = new List<HeadPoint>();
        public bool RectTransformMode;                  // прямоугольник, для изменения SetCustomPoints
        private int moveRectIndex = -1;
        private Vector2 tempMoveRectCenter;         // старые значения прямоугольника, для сжимания-расжимания точек
        private float tempMoveRectWidth;
        private float tempMoveRectHeight;

        /// <summary> Прямоугольник, охватывающий все автоточки. Нужен для изменения всех точек сразу (сжатие/расжатие) </summary>
        public RectangleF FaceRectTransformed;
        /// <summary> Центральная точка на лбу. Нужна для выделения прямоугольинка автоточек. </summary>
        public Vector2 CentralFacePoint;

        /// <summary> Поцизии рта и глаз для выбора определенных частей во фронте при настройке</summary>
        public Vector2 MouthUserCenter;
        public Vector2 LeftEyeUserCenter;
        public Vector2 RightEyeUserCenter;
        public Vector2 NoseUserCenter = new Vector2(0, 0);
        private static float customSelectionRadius = 0.15f;

        #endregion

        #endregion

        public ctrlRenderControl()
        {
            InitializeComponent();
            glControl.PreviewKeyDown += glControl_PreviewKeyDown;
            headMeshesController.RenderMesh.OnBeforePartDraw += RenderMesh_OnBeforePartDraw;
        }
        ~ctrlRenderControl()
        {
            if (!ProgramCore.PluginMode)
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
                foreach (var t in textures)
                    GL.DeleteTexture(t.Value.Texture);
            }
        }

        #region Initializing

        #region Graphic's

        /// <summary> Initialize control and setup GL settings </summary>
        public void Initialize()
        {
            loaded = true;
            PlayAnimation = false;
            pickingController = new PickingController(camera);

            idleShader = new ShaderController("idle.vs", "idle.fs");
            idleShader.SetUniformLocation("u_UseTexture");
            idleShader.SetUniformLocation("u_Color");
            idleShader.SetUniformLocation("u_Texture");
            idleShader.SetUniformLocation("u_BrushMap");
            idleShader.SetUniformLocation("u_TransparentMap");
            idleShader.SetUniformLocation("u_World");
            idleShader.SetUniformLocation("u_WorldView");
            idleShader.SetUniformLocation("u_ViewProjection");
            idleShader.SetUniformLocation("u_LightDirection");

            blendShader = new ShaderController(ProgramCore.PluginMode ? "blendingPl.vs" : "blending.vs", "blending.fs");
            blendShader.SetUniformLocation("u_Texture");
            blendShader.SetUniformLocation("u_BlendStartDepth");
            blendShader.SetUniformLocation("u_BlendDepth");

            brushShader = new ShaderController("brush.vs", "brush.fs");
            brushShader.SetUniformLocation("u_Texture");
            brushShader.SetUniformLocation("u_World");
            brushShader.SetUniformLocation("u_BrushColor");
            brushShader.SetUniformLocation("u_SphereCenter");
            brushShader.SetUniformLocation("u_SphereRadius");

            SetupViewport(glControl);

            windowInfo = Utilities.CreateWindowsWindowInfo(renderPanel.Handle);
            graphicsContext = new GraphicsContext(GraphicsMode.Default, windowInfo);
            renderPanel.Resize += (sender, args) => graphicsContext.Update(windowInfo);
            glControl.Context.MakeCurrent(glControl.WindowInfo);

            InitializeProfileSprites();
            InitializeCustomBaseSprites();
        }
        private void SetupViewport(GLControl c)
        {
            if (c.ClientSize.Height == 0)
                c.ClientSize = new Size(c.ClientSize.Width, 1);

            camera.UpdateViewport(c.ClientSize.Width, c.ClientSize.Height);
        }

        public void InitialiseCamera(Vector2 data)
        {
            var x = (headMeshesController.RenderMesh.AABB.B.Xy - headMeshesController.RenderMesh.AABB.A.Xy).Length;
            var y = (headMeshesController.RenderMesh.AABB.B.Y + headMeshesController.RenderMesh.AABB.A.Y) * 0.5f;
            camera.SetupCamera(data, new Vector2(x, y));
        }
        public void InitializeProfileCamera(Vector2 data)
        {
            var pointUp = ProgramCore.MainForm.ctrlTemplateImage.profileControlPoints[0];
            var pointBottom = ProgramCore.MainForm.ctrlTemplateImage.profileControlPoints[3];
            var x = (pointUp.Value - pointBottom.Value).Length;
            var y = (pointUp.Value.Y + pointBottom.Value.Y) * 0.5f;
            camera.SetupCamera(data, new Vector2(x, y));
        }

        private void InitializeProfileSprites()
        {
            profilePointSprites.Clear();

            var spriteTexturePath = Path.Combine(Application.StartupPath, "bin", "sprite.png");
            var spriteTexture = GetTexture(spriteTexturePath);

            for (var i = 0; i < 4; i++)
            {
                var sprite = new Sprite2D();
                sprite.Texture = spriteTexture;
                sprite.Size = new Vector2(1.5f, 1.5f);

                switch (i)
                {
                    case 0:         // это единичка)
                        sprite.TexCoordLeftBottom = new Vector2(0.5f, 0.5f);
                        sprite.TexCoordRightTop = Vector2.Zero;
                        break;
                    case 1:
                        sprite.TexCoordLeftBottom = new Vector2(1.0f, 0.5f);
                        sprite.TexCoordRightTop = new Vector2(0.5f, 0f);
                        break;
                    case 2:
                        sprite.TexCoordLeftBottom = new Vector2(0.5f, 1.0f);
                        sprite.TexCoordRightTop = new Vector2(0f, 0.5f);
                        break;
                    case 3:
                        sprite.TexCoordLeftBottom = new Vector2(1.0f, 1.0f);
                        sprite.TexCoordRightTop = new Vector2(0.5f, 0.5f);
                        break;
                }

                profilePointSprites.Add(sprite);
            }
        }

        private void InitializeCustomBaseSprites()
        {
            //customBasePointsSprites.Clear();
            if (customBasePointsSprites.Count > 0)
                return;

            var spriteTexturePath = Path.Combine(Application.StartupPath, "bin", "sprite.png");
            var spriteTexture = GetTexture(spriteTexturePath);

            for (var i = 0; i < 4; i++)
            {
                var sprite = new Sprite2D();
                sprite.Texture = spriteTexture;
                sprite.Size = new Vector2(1.5f, 1.5f);

                switch (i)
                {
                    case 0:
                        sprite.TexCoordLeftBottom = new Vector2(0.5f, 0.5f);
                        sprite.TexCoordRightTop = Vector2.Zero;
                        break;
                    case 1:
                        sprite.TexCoordLeftBottom = new Vector2(1.0f, 1.0f);
                        sprite.TexCoordRightTop = new Vector2(0.5f, 0.5f);
                        break;
                    case 2:
                        sprite.TexCoordLeftBottom = new Vector2(0.5f, 1.0f);
                        sprite.TexCoordRightTop = new Vector2(0f, 0.5f);
                        break;
                    case 3:
                        sprite.TexCoordLeftBottom = new Vector2(1.0f, 0.5f);
                        sprite.TexCoordRightTop = new Vector2(0.5f, 0f);
                        break;
                }

                customBasePointsSprites.Add(sprite);
            }
        }
        public void InitializeCustomControlSpritesPosition()
        {
            InitializeCustomBaseSprites();
            for (var i = 0; i < 4; i++)
            {
                var sprite = customBasePointsSprites[i];
                sprite.Position = new Vector2(ProgramCore.Project.BaseDots[i].Value.X, ProgramCore.Project.BaseDots[i].Value.Y);
            }
        }

        #endregion

        #region Project

        public void LoadProject(bool newProject, RectangleAABB aabb)
        {
            tempBitmaps.Clear();
            var headTexturePath = Path.Combine(ProgramCore.Project.ProjectPath, ProgramCore.Project.FrontImagePath);
            HeadTextureId = 0;
            if (!string.IsNullOrEmpty(headTexturePath))
            {
                using (var ms = new MemoryStream(File.ReadAllBytes(headTexturePath))) // Don't use using!!
                    HeadTexture = (Bitmap)Image.FromStream(ms);

                HeadTextureId = GetTexture(headTexturePath);

                if (ProgramCore.Project.FaceRectRelative == RectangleF.Empty)
                {
                    var fileName = Path.Combine(ProgramCore.Project.ProjectPath, ProgramCore.Project.FrontImagePath);

                    var faceRecognition = new OpenCvFaceRecognition();
                    faceRecognition.Recognize(ref fileName, false);

                    ProgramCore.Project.FaceRectRelative = faceRecognition.FaceRectRelative;
                    ProgramCore.Project.MouthCenter = faceRecognition.MouthCenter;
                    ProgramCore.Project.LeftEyeCenter = faceRecognition.LeftEyeCenter;
                    ProgramCore.Project.RightEyeCenter = faceRecognition.RightEyeCenter;
                    ProgramCore.Project.FaceColor = faceRecognition.FaceColor;
                }

            }
            baseProfilePoints.Clear();

            if (newProject)
            {
                var modelPath = ProgramCore.Project.HeadModelPath;
                pickingController.AddMehes(modelPath, MeshType.Head, false, ProgramCore.Project.ManType, ProgramCore.PluginMode);

                float scale = 0;
                if (ProgramCore.Project.ManType == ManType.Custom)
                {
                    scale = headMeshesController.SetSize(29.3064537f); // подгонка размера для произвольной башки
                    customBasePointsSprites.Clear();
                }
                else if (ProgramCore.PluginMode)
                {
                    switch (ProgramCore.Project.ManType)
                    {
                        case ManType.Male:
                            scale = headMeshesController.SetSize(29.9421043f); // подгонка размера 
                            break;
                        case ManType.Female:
                            scale = headMeshesController.SetSize(29.3064537f); // подгонка размера 
                            break;
                        case ManType.Child:
                            scale = headMeshesController.SetSize(25.6209984f); // подгонка размера 
                            break;
                    }
                }
                if (pickingController.ObjExport != null)
                    pickingController.ObjExport.Scale = scale;
            }
            HeadShapeController.Initialize(headMeshesController);
            brushTool.InitializeBrush(headMeshesController);

            if (ProgramCore.Project.ManType != ManType.Custom)
            {
                var oldMorphingPath = Path.Combine(Application.StartupPath, "Models\\Morphing", ProgramCore.Project.ManType.GetCaption(), "Old.obj"); // загружаем трансформации для старения
                OldMorphing = pickingController.LoadPartsMorphInfo(oldMorphingPath, headMeshesController.RenderMesh);

                var fatMorphingPath = Path.Combine(Application.StartupPath, "Models\\Morphing", ProgramCore.Project.ManType.GetCaption(), "Fat.obj"); // загружаем трансформации для толстения
                FatMorphing = pickingController.LoadPartsMorphInfo(fatMorphingPath, headMeshesController.RenderMesh);
            }

            var baseDots = HeadController.GetBaseDots(ProgramCore.Project.ManType);
            headMeshesController.RenderMesh.SetBlendingInfo(baseDots[0], baseDots[1], baseDots[2], baseDots[3]);

            #region Сглаживание текстур

            SmoothedTextures.Clear();
            for (var i = 0; i < headMeshesController.RenderMesh.Parts.Count; i++)
            {
                var part = headMeshesController.RenderMesh.Parts[i];
                if (part.Texture == -1)
                    continue;

                var oldTexture = GetTexture(part.DefaultTextureName);
                if (!SmoothedTextures.ContainsKey(oldTexture))
                {
                    if (part.Texture == 0 || part.IsBaseTexture)
                    {
                        part.IsBaseTexture = true;
                        part.Texture = 0;
                    }
                    else
                    {
                        var path = part.DefaultTextureName;//GetTexturePath(part.Texture);                        
                        var newImagePath = Path.Combine(ProgramCore.Project.ProjectPath, "SmoothedModelTextures");
                        var di = new DirectoryInfo(newImagePath);
                        if (!di.Exists)
                            di.Create();

                        var brushImagePath = Path.Combine(newImagePath, Path.GetFileNameWithoutExtension(path) + "_brush.png");
                        var smoothedImagePath = Path.Combine(newImagePath, Path.GetFileNameWithoutExtension(path) + "_smoothed" + Path.GetExtension(path));
                        if (!File.Exists(smoothedImagePath))
                            File.Copy(path, smoothedImagePath, true);

                        //newImagePath = Path.Combine(newImagePath, Path.GetFileNameWithoutExtension(path) + Path.GetExtension(path));
                        //File.Copy(path, newImagePath, true);                        
                        var smoothedTexture = GetTexture(smoothedImagePath); // по старому пути у нас будут храниться сглаженные текстуры (что бы сохранение модельки сильно не менять)
                        part.Texture = oldTexture;
                        SmoothedTextures.Add(oldTexture, smoothedTexture); // связка - айди старой-новой текстур

                        if (File.Exists(brushImagePath) && !brushTextures.ContainsKey(part.Texture))
                        {
                            var texture = GetTexture(brushImagePath);
                            Bitmap bitmap;
                            using (var ms = new MemoryStream(File.ReadAllBytes(brushImagePath)))
                                bitmap = (Bitmap)Image.FromStream(ms);
                            brushTextures.Add(smoothedTexture, new BrushTextureInfo { Texture = texture, TextureData = bitmap, LinkedTextureName = smoothedImagePath });
                        }
                    }
                }
                else
                {
                    part.Texture = oldTexture;
                }

                if (part.Texture != 0)      //все кроме отсутствующих. после первых автоточек - станет фоткой
                {
                    part.Texture = SmoothedTextures[part.Texture]; // переприсваиваем текстуры на сглаженные
                    part.TextureName = GetTexturePath(part.Texture);
                }
            }
            ProgramCore.Project.SmoothedTextures = true;

            #endregion

            if (newProject)
            {
                if (ProgramCore.Project.ManType != ManType.Custom)
                {
                    var scaleX = UpdateMeshProportions(aabb);
                    UpdatePointsProportion(scaleX, (aabb.A.X + aabb.B.X) * 0.5f);

                    //headMeshesController.InitializeTexturing(autodotsShapeHelper.GetBaseDots(), HeadController.GetIndices());
                    //autodotsShapeHelper.Transform(headMeshesController.TexturingInfo.Points.ToArray());

                    autodotsShapeHelper.TransformRects();
                    autodotsShapeHelper.InitializeShaping();

                    var points = autodotsShapeHelper.GetBaseDots();

                    SpecialCenterUpdate(points, headController.GetLeftEyeIndexes(), ProgramCore.Project.LeftEyeCenter);
                    SpecialCenterUpdate(points, headController.GetRightEyeIndexes(), ProgramCore.Project.RightEyeCenter);


                    //SpecialMouthEyesUpdate(points, headController.GetMouthIndexes(), ProgramCore.Project.MouthCenter, 
                    //    new List<Vector2> { ProgramCore.Project.DetectedPoints[0], ProgramCore.Project.DetectedPoints[1] });
                    //var noseBottomPoint = new Vector2(ProgramCore.Project.MouthCenter.X, ((eyesDiff.Y - ProgramCore.Project.MouthUserCenter.Y) * 0.4f) + ProgramCore.Project.MouthUserCenter.Y);
                    //SpecialMouthEyesUpdate(points, headController.GetNoseBottomIndexes(), noseBottomPoint,
                    //    new List<Vector2> { ProgramCore.Project.DetectedPoints[2], ProgramCore.Project.DetectedPoints[3] });
                    SpecialLipsPointsUpdate(points, ProgramCore.Project.MouthCenter);
                    SpecialNosePointsUpdate(points);

                    SpecialCenterUpdate(points, headController.GetNoseTopIndexes(), ProgramCore.Project.DetectedNosePoints[4]);
                    SpecialBottomPointsUpdate(points);
                    SpecialTopHaedWidth(points);
                    //autodotsShapeHelper.TransformRects();
                    //headMeshesController.UpdateBuffers();
                }
                else
                {
                    camera.ResetCamera(true);
                }
            }
            else
            {
                autodotsShapeHelper.TransformRects();
                headMeshesController.UpdateBuffers();
            }

            RenderTimer.Start();
        }

        private void SpecialTopHaedWidth(List<HeadPoint> points)
        {
            var topHeadIndices = new int[] { 6, 5, 4, 3, 0, 25, 26, 27, 28 };
            var a = MirroredHeadPoint.GetFrontWorldPoint(ProgramCore.Project.DetectedTopPoints[0]);
            var b = MirroredHeadPoint.GetFrontWorldPoint(ProgramCore.Project.DetectedTopPoints[1]);
            var width = b.X - a.X;
            var invOldWidth = 1.0f / (points[28].Value.X - points[6].Value.X);
            var minX = points[6].Value.X;

            foreach (var index in topHeadIndices)
            {
                var point = points[index];
                var x = (point.Value.X - minX) * invOldWidth * width + a.X;
                point.Value = new Vector2(x, point.Value.Y);
                autodotsShapeHelper.Transform(point.Value, index);
            }
        }

        private void SpecialNosePointsUpdate(List<HeadPoint> points)
        {
            var bottomNosePoints = new int[] { 19, 2, 41, 52 };

            for (var i = 0; i < bottomNosePoints.Length; ++i)
            {
                var point = points[bottomNosePoints[i]];
                var delta = MirroredHeadPoint.GetFrontWorldPoint(ProgramCore.Project.DetectedNosePoints[i]) - point.Value;
                if (i != 52)
                    delta.Y -= 0.2f;
                //delta = new Vector2(point.Value.X + delta.X, point.Value.Y) - point.Value;
                point.Value += delta;
                foreach (var l in point.LinkedPoints)
                {
                    var p = points[l];
                    p.Value += delta;
                    autodotsShapeHelper.Transform(p.Value, l);
                }
                autodotsShapeHelper.Transform(point.Value, bottomNosePoints[i]);
            }
        }

        private void SpecialLipsPointsUpdate(List<HeadPoint> points, Vector2 targetPoint)
        {
            var mouthIndices = headController.GetMouthIndexes();

            var borders = new Vector2[] { ProgramCore.Project.DetectedLipsPoints[0], ProgramCore.Project.DetectedLipsPoints[4] };

            float maxX, minX;
            var center = GetCenter(points, mouthIndices, out minX, out maxX);
            var rightPosUserSelected = MirroredHeadPoint.GetFrontWorldPoint(targetPoint);          // перенояем координаты с левой картинки в правой
            var delta2 = rightPosUserSelected - center;

            var leftBorder = MirroredHeadPoint.GetFrontWorldPoint(borders[0]) - rightPosUserSelected;
            var rightBorder = MirroredHeadPoint.GetFrontWorldPoint(borders[1]) - rightPosUserSelected;

            minX = minX - center.X;
            maxX = maxX - center.X;

            //Подгоняем все точки в центр и растягиваем по ширине
            foreach (var index in mouthIndices)
            {
                var p = points[index];
                var dx = p.Value.X - center.X;
                dx = dx < 0.0f ? (dx * leftBorder.X / minX) : (rightBorder.X * dx / maxX);
                p.Value = new Vector2(center.X + dx + delta2.X, p.Value.Y + delta2.Y);
            }
            //Проставляем фиксированные точки
            var indices = new int[] { 15, 17, 1, 39, 37, 38, 16, 51 };
            var i = 0;
            foreach (var index in indices)
            {
                var p = points[index];
                p.Value = MirroredHeadPoint.GetFrontWorldPoint(ProgramCore.Project.DetectedLipsPoints[i++]);
            }

            foreach (var index in mouthIndices)
            {
                var p = points[index];
                autodotsShapeHelper.Transform(p.Value, index);
            }
        }

        private void SpecialBottomPointsUpdate(List<HeadPoint> points)
        {
            var bottomPoints = new int[] { 9, 10, 11, 33, 32, 31 };
            for (var i = 0; i < bottomPoints.Length; ++i)
            {
                var point = points[bottomPoints[i]];
                var delta = MirroredHeadPoint.GetFrontWorldPoint(ProgramCore.Project.DetectedBottomPoints[i]) - point.Value;
                point.Value += delta;
                autodotsShapeHelper.Transform(point.Value, bottomPoints[i]);
            }

            var bottomPointsX = new int[] { 7, 8, 30, 29 };
            for (var i = 0; i < bottomPointsX.Length; ++i)
            {
                var point = points[bottomPointsX[i]];
                var delta = MirroredHeadPoint.GetFrontWorldPoint(ProgramCore.Project.DetectedBottomPoints[i + 6]) - point.Value;
                delta = new Vector2(point.Value.X + delta.X, point.Value.Y) - point.Value;
                point.Value += delta;
                foreach (var l in point.LinkedPoints)
                {
                    var p = points[l];
                    p.Value += delta;
                    autodotsShapeHelper.Transform(p.Value, l);
                }
                autodotsShapeHelper.Transform(point.Value, bottomPointsX[i]);
            }
        }

        private void SpecialCenterUpdate(List<HeadPoint> points, List<int> indexes, Vector2 targetPoint)
        {
            float maxX, minX;
            var center = GetCenter(points, indexes, out minX, out maxX);
            var rightPosUserSelected = MirroredHeadPoint.GetFrontWorldPoint(targetPoint);          // перенояем координаты с левой картинки в правой
            var delta2 = rightPosUserSelected - center;

            foreach (var index in indexes)
            {
                var p = points[index];
                p.Value += delta2;
                autodotsShapeHelper.Transform(p.Value, index);
            }
        }
        private Vector2 GetCenter(List<HeadPoint> points, List<int> indexes, out float xmin, out float xmax)
        {
            var dots = indexes.Select(index => points[index]).ToList();

            if (dots.Count == 0)
            {
                xmin = xmax = 0.0f;
                return Vector2.Zero;
            }


            var minX = xmin = dots.Min(point => point.Value.X);
            var maxX = xmax = dots.Max(point => point.Value.X);
            var minY = dots.Min(point => point.Value.Y);
            var maxY = dots.Max(point => point.Value.Y);

            return new Vector2((maxX + minX) * 0.5f, (maxY + minY) * 0.5f);
        }

        public void LoadModel(string path, bool needClean, ManType manType, MeshType type)
        {
            if (needClean)
                CleanProjectMeshes();

            pickingController.AddMehes(path, type, false, manType, false);
        }

        private void UpdatePointsProportion(float scaleX, float centerX)
        {
            foreach (var p in autodotsShapeHelper.ShapeInfo.Points)
            {
                var v = p.Value;
                v.X -= centerX;
                v.X *= scaleX;
                v.X += centerX;
                p.Value = v;
            }
        }

        public float UpdateMeshProportions(RectangleAABB aabb)
        {
            var widthToHeight = 0.669f; // подгоняем размер модели под размер еблища
            if (ProgramCore.Project.FaceRectRelative != RectangleF.Empty)
                widthToHeight = (ProgramCore.Project.FaceRectRelative.Width * ProgramCore.Project.FrontImage.Width) / (ProgramCore.Project.FaceRectRelative.Height * ProgramCore.Project.FrontImage.Height);
            return headMeshesController.FinishCreating(widthToHeight, aabb);
        }
        public void CleanProjectMeshes()
        {
            //  textures.Clear();             // сейчас это не надо :)
            pickingController.SelectedMeshes.Clear();
            pickingController.HairMeshes.Clear();
            pickingController.AccesoryMeshes.Clear();
            PartsLibraryMeshes.Clear();
        }
        public void CleanHairMeshes()
        {
            pickingController.SelectedMeshes.Clear();
            pickingController.HairMeshes.Clear();

            for (var i = PartsLibraryMeshes.Count - 1; i >= 0; i--)
            {
                var k = PartsLibraryMeshes.Values.ElementAt(i);
                var item = k.FirstOrDefault();
                if (item != null && item.meshType == MeshType.Hair)
                {
                    var key = PartsLibraryMeshes.Keys.ElementAt(i);
                    PartsLibraryMeshes.Remove(key);
                }
            }
        }


        #endregion

        #endregion

        #region Form's event

        private void glControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Modifiers != Keys.Control && e.Modifiers != Keys.Alt)
                e.IsInputKey = true;

            switch (Mode)
            {
                case Mode.SetCustomControlPoints:
                    {
                        switch (e.KeyData)
                        {
                            case Keys.A:
                                ProgramCore.Project.BaseDots.SelectAll();
                                break;
                            case Keys.D:
                                ProgramCore.Project.BaseDots.ClearSelection();
                                break;
                        }
                    }
                    break;
                case Mode.SetCustomPoints:
                    {
                        switch (e.KeyData)
                        {
                            case Keys.A:
                                autodotsShapeHelper.ShapeInfo.Points.SelectAll();
                                break;
                            case Keys.D:
                                autodotsShapeHelper.ShapeInfo.Points.ClearSelection();
                                break;
                        }
                    }
                    break;
                case Mode.SetCustomProfilePoints:
                    {
                        switch (e.KeyData)
                        {
                            case Keys.A:
                                autodotsShapeHelper.ShapeProfileInfo.Points.SelectAll();
                                break;
                            case Keys.D:
                                autodotsShapeHelper.ShapeProfileInfo.Points.ClearSelection();
                                break;
                        }
                    }
                    break;
                default:
                    if (ProgramCore.MainForm.HeadMode)   /* сделано специально. поскольку у нас так же должны обрабатываться хот кеи у менюшки. для этого нельзя полностью отдавать отлов клавиш этому контролу. но в случае с модификацией головы - необходимы эти простые команды, так как без них неудобно. */
                    {
                        switch (e.KeyData)
                        {
                            case Keys.A:
                                headController.SelectAll();
                                break;
                            case Keys.D:
                                headController.ClearPointsSelection();
                                break;
                        }
                    }
                    break;
            }
        }
        private void glControl_KeyDown(object sender, KeyEventArgs e)
        {
            shiftPressed = e.Shift;
            switch (e.KeyData)
            {
                case Keys.X:
                    ProgramCore.MainForm.ExportDAE();
                    break;
                case Keys.T:
                    showTriangles = !showTriangles;
                    break;
                case Keys.R:
                    showInfo = !showInfo;
                    break;
                case Keys.P:
                    profileTrianglesDrowing = !profileTrianglesDrowing;
                    break;
                case Keys.S:
                    SimpleDrawing = !SimpleDrawing;
                    break;
                case Keys.Left:
                    TimerMode = Mode.TimerTurnLeft;
                    keyTimer.Start();
                    break;
                case Keys.Right:
                    TimerMode = Mode.TimerTurnRight;
                    keyTimer.Start();
                    break;
                case Keys.Up:
                    TimerMode = Mode.TimerZoomIn;
                    keyTimer.Start();
                    break;
                case Keys.Down:
                    TimerMode = Mode.TimerZoomOut;
                    keyTimer.Start();
                    break;
                case Keys.Space:
                    //if (ProgramCore.MainForm.HeadMode)
                    UseTexture = !UseTexture;
                    //else
                    //    SimpleDrawing = !SimpleDrawing;
                    break;
                case Keys.Delete:
                    if (pickingController.SelectedMeshes.Count > 0)
                    {
                        if (pickingController.SelectedMeshes.Count > 1 || pickingController.SelectedMeshes[0].meshType == MeshType.Hair)
                            DeleteSelectedHair();
                        else
                            DeleteSelectedAccessory();
                    }
                    break;
            }
        }

        //temp
        private void SaveTmpPoints()
        {
            using (var sw = new StreamWriter("point.txt"))
            {
                sw.WriteLine("base points");
                sw.WriteLine();
                foreach (var item in ProgramCore.Project.BaseDots)
                    sw.WriteLine("new Vector2({0}f, {1}f),", string.Format("{0}", item.Value.X).Replace(',', '.'), string.Format("{0}", item.Value.Y).Replace(',', '.'));
                sw.WriteLine("main front points");
                sw.WriteLine();
                autodotsShapeHelper.TransformRects();
                var dots = autodotsShapeHelper.GetBaseDots();
                foreach (var d in dots)
                    sw.WriteLine("new HeadPoint({0}f, {1}f),", string.Format("{0}", d.Value.X).Replace(',', '.'), string.Format("{0}", d.Value.Y).Replace(',', '.'));

                sw.WriteLine("additional front points");
                sw.WriteLine();
                var fdots = autodotsShapeHelper.Dots;
                foreach (var d in fdots)
                    sw.WriteLine("new Vector2({0}f, {1}f),", string.Format("{0}", d.X).Replace(',', '.'), string.Format("{0}", d.Y).Replace(',', '.'));

                sw.WriteLine("profile points");
                sw.WriteLine();
                var pdots = autodotsShapeHelper.ProfileDots;
                foreach (var d in pdots)
                    sw.WriteLine("new Vector2({0}f, {1}f),", string.Format("{0}", d.X).Replace(',', '.'), string.Format("{0}", d.Y).Replace(',', '.'));

                var pbdots = autodotsShapeHelper.GetProfileBaseDots();
                sw.WriteLine("profile base points");
                foreach (var d in pbdots)
                    sw.WriteLine("new Vector2({0}f, {1}f),", string.Format("{0}", d.X).Replace(',', '.'), string.Format("{0}", d.Y).Replace(',', '.'));
            }
        }
        //temp

        private void InitializeCustomHead()
        {
            var aabb = new RectangleAABB();
            Vector3 min = aabb.A, max = aabb.B;
            for (var i = 8; i < autodotsShapeHelper.ShapeInfo.Points.Count; ++i)
            {
                var p = autodotsShapeHelper.ShapeInfo.Points[i];
                min.X = Math.Min(p.Value.X, min.X);
                min.Y = Math.Min(p.Value.Y, min.Y);
                max.X = Math.Max(p.Value.X, max.X);
                max.Y = Math.Max(p.Value.Y, max.Y);
            }
            aabb.A = min;
            aabb.B = max;

            var scaleX = UpdateMeshProportions(aabb);
            UpdatePointsProportion(scaleX, (aabb.A.X + aabb.B.X) * 0.5f);

            //headMeshesController.InitializeTexturing(autodotsShapeHelper.GetBaseDots(), HeadController.GetIndices());
            //autodotsShapeHelper.Transform(headMeshesController.TexturingInfo.Points.ToArray());

            autodotsShapeHelper.TransformRects();
            autodotsShapeHelper.InitializeShaping();

            var points = autodotsShapeHelper.GetBaseDots();
            SpecialCenterUpdate(points, headController.GetMouthIndexes(), ProgramCore.Project.MouthCenter);
            SpecialCenterUpdate(points, headController.GetLeftEyeIndexes(), ProgramCore.Project.LeftEyeCenter);
            SpecialCenterUpdate(points, headController.GetRightEyeIndexes(), ProgramCore.Project.RightEyeCenter);

            var eyesDiff = (ProgramCore.Project.LeftEyeCenter + ProgramCore.Project.RightEyeCenter) * 0.5f;
            var noseBottomPoint = new Vector2(ProgramCore.Project.MouthCenter.X, ((eyesDiff.Y - ProgramCore.Project.MouthUserCenter.Y) * 0.4f) + ProgramCore.Project.MouthUserCenter.Y);
            SpecialCenterUpdate(points, headController.GetNoseBottomIndexes(), noseBottomPoint);
            SpecialCenterUpdate(points, headController.GetNoseTopIndexes(), eyesDiff);
        }

        private void glControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                switch (Mode)
                {
                    case Mode.SetCustomControlPoints:
                        {
                            UpdateCustomFaceParts(false, true);
                            Mode = Mode.SetCustomPoints;
                        }
                        break;
                    case Mode.SetCustomPoints:
                        {
                            ProgramCore.Project.CustomHeadNeedProfileSetup = true;
                            RectTransformMode = false;
                            Mode = Mode.None;
                            ProgramCore.MainForm.panelMenuFront_Click(null, EventArgs.Empty); // set opened by default
                            InitializeCustomHead();
                            camera.ResetCamera(true);
                        }
                        break;
                    case Mode.SetCustomProfilePoints:
                        {
                            ProgramCore.Project.CustomHeadNeedProfileSetup = false;
                            RectTransformMode = false;
                            Mode = Mode.None;
                        }
                        break;
                    default:
                        {
                            if (pickingController.SelectedMeshes.Count > 1)
                            {
                                pickingController.SelectedMeshes[0].AttachMeshes(pickingController.SelectedMeshes);
                                pickingController.SelectedMeshes.RemoveAt(0);
                                DeleteSelectedHair();
                            }
                        }
                        break;
                }
            }
            if (e.KeyCode == Keys.ShiftKey)
                shiftPressed = false;
            keyTimer.Stop();
            workTimer.Stop();
        }

        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            dblClick = e.Clicks == 2;
            if (e.Button == MouseButtons.Right)
            {
                #region Правая кнопка с выделенными мешами волос или аксессуаров

                if (pickingController.SelectedMeshes != null)
                {
                    switch (Mode)
                    {
                        case Mode.AccessoryRotateSetCircle:
                        case Mode.AccessoryRotate:
                            {
                                Mode = Mode.None;
                                Cursor = DefaultCursor;

                                accessoryRotateCenterCirclePoint = Vector2.Zero;
                                accessoryRotateCircleMousePoint = Vector2.Zero;
                                accessoryRotateMousePoint = Vector2.Zero;
                                accessoryRotateCenterPoint = Vector3.Zero;
                                accessoryRotateRadius = 0f;
                            }
                            break;
                        case Mode.HairShapeSetRect:
                        case Mode.HairStretchSetRect:
                        case Mode.HairPleatSetRect:
                            if (hairRectPointB == Vector2.Zero)
                                hairRectPointA = Vector2.Zero;
                            else if (hairRectPointA != Vector2.Zero)
                            {
                                switch (Mode)
                                {
                                    case Mode.HairStretchSetRect:
                                        Mode = Mode.HairStretch;
                                        break;
                                    case Mode.HairShapeSetRect:
                                        Mode = Mode.HairShape;
                                        break;
                                    case Mode.HairPleatSetRect:
                                        Mode = Mode.HairPleat;
                                        break;
                                }

                                var vector = hairRectPointB - hairRectPointA;
                                var perpendicular = new Vector2(vector.Y, -vector.X);
                                perpendicular.Normalize();

                                var r1 = new Vector2(hairRectPointA.X - perpendicular.X * 50f, hairRectPointA.Y - perpendicular.Y * 50f);
                                var r2 = new Vector2(hairRectPointA.X + perpendicular.X * 50f, hairRectPointA.Y + perpendicular.Y * 50f);
                                var r3 = new Vector2(hairRectPointB.X + perpendicular.X * 50f, hairRectPointB.Y + perpendicular.Y * 50f);
                                var r4 = new Vector2(hairRectPointB.X - perpendicular.X * 50f, hairRectPointB.Y - perpendicular.Y * 50f);

                                HairRect.Clear();
                                HairRect.Add(0, r1);
                                HairRect.Add(1, r2);
                                HairRect.Add(2, r3);
                                HairRect.Add(3, r4);
                                IsShapeChanged = true;
                            }
                            break;
                        case Mode.HairStretch:
                        case Mode.HairShape:
                        case Mode.HairPleat:
                            IsShapeChanged = true;
                            hairRectPointA = Vector2.Zero;
                            hairRectPointB = Vector2.Zero;
                            Mode = Mode == Mode.HairStretch ? Mode.HairStretchSetRect : Mode.HairShapeSetRect;
                            break;
                        default:
                            if (pickingController.SelectedMeshes.Count == 1 && pickingController.SelectedMeshes[0].meshType == MeshType.Accessory)
                            {
                                Mode = Mode.AccessoryRotateSetCircle;
                                Cursor = Cursors.Cross;
                            }
                            break;
                    }
                }

                #endregion
            }
            else if (e.Button == MouseButtons.Left)
            {
                leftMousePressed = true;
                startPress = true;

                if (pickingController.SelectedMeshes.Count > 0 && Mode != Mode.ColorPicker)
                {
                    #region Левая кнопка с выделенными мешами волос или аксессуаров

                    var xy = glControl.PointToClient(new Point(e.X, e.Y));
                    meshPoint = camera.GetWorldPoint(xy.X, xy.Y, Width, Height, 0.0f);

                    meshTempItems.Clear();
                    foreach (var mesh in pickingController.SelectedMeshes)
                        meshTempItems.Add(mesh, new Tuple<Vector3, Matrix4>(mesh.Position, mesh.Transform));

                    switch (Mode)
                    {
                        case Mode.AccessoryRotateSetCircle:

                            var dir = new Vector3(camera.Position.X, 0.0f, camera.Position.Z);
                            var length = dir.Length;
                            dir /= length;
                            var distance = Vector3.Dot(dir, pickingController.SelectedMeshes[0].Position);
                            accessoryRotateCenterPoint = camera.GetWorldPoint(e.X, e.Y, Width, Height, length - distance);
                            accessoryRotateCenterCirclePoint = new Vector2(e.X, e.Y);

                            accessoryRotateCircleMousePoint = Vector2.Zero;
                            accessoryRotateMousePoint = Vector2.Zero;
                            accessoryRotateRadius = 0f;
                            break;
                        case Mode.AccessoryRotate:
                            break;
                        case Mode.HairStretchSetRect:
                        case Mode.HairShapeSetRect:
                        case Mode.HairPleatSetRect:
                            if (hairRectPointA == Vector2.Zero || hairRectPointB != Vector2.Zero)
                            {
                                hairRectPointA = new Vector2(e.X, e.Y);
                                hairRectPointB = Vector2.Zero;
                            }
                            else
                                hairRectPointB = new Vector2(e.X, e.Y);
                            break;
                        case Mode.HairShape:
                        case Mode.HairStretch:
                        case Mode.HairPleat:
                            bHaveMouse = hairSelectedPoint != -1;
                            if (bHaveMouse)
                                Cursor = Cursors.SizeAll;
                            break;
                        case Mode.HairCut:
                            if (sliceController.Lines.Count == 0 || (sliceController.Lines.Count == 2 && ToolsMode == ToolsMode.HairLine))
                                sliceController.BeginSlice(ToolsMode == ToolsMode.HairArc);

                            sliceController.MovePoint(new Vector2(e.X, e.Y), shiftPressed, ToolMirrored ? Width : 0);
                            break;
                        default:
                            accessoryRotateCenterCirclePoint = Vector2.Zero;
                            accessoryRotateCircleMousePoint = Vector2.Zero;
                            accessoryRotateMousePoint = Vector2.Zero;
                            accessoryRotateRadius = 0f;

                            hairRectPointA = Vector2.Zero;
                            hairRectPointB = Vector2.Zero;
                            HairRect.Clear();
                            break;
                    }

                    #endregion
                }
                else
                {
                    #region Обычная левая кнопка

                    switch (Mode)
                    {
                        case Mode.ColorPicker:
                            {
                                using (var img = GrabScreenshot(string.Empty, ClientSize.Width, ClientSize.Height))
                                {
                                    var color = img.GetPixel((int)e.X, (int)e.Y);
                                    ProgramCore.MainForm.frmMaterial.SetColorFromPicker(color);
                                }
                            }
                            break;
                        case Mode.LassoStart:
                            if (dblClick)
                            {
                                Mode = Mode.LassoActive;
                                LassoPoints.Add(LassoPoints.First());
                            }
                            break;
                        case Mode.LassoActive:
                            Mode = Mode.LassoStart;
                            LassoPoints.Clear();
                            LassoPoints.Add(new Vector2(e.X, e.Y));
                            break;
                        case Mode.HeadShape:
                            if (ProgramCore.Project.ShapeFlip != FlipType.None)
                                return;

                            if (!ToolMirrored)
                            {
                                if (dblClick)
                                    headController.ClearPointsSelection();

                                var point = camera.GetWorldPoint(e.X, e.Y, glControl.ClientSize.Width, glControl.ClientSize.Height, 15.0f).Xy;
                                headController.UpdatePointSelection(point.X, point.Y);
                            }
                            break;
                        case Mode.SetCustomControlPoints:
                            {
                                customLastPoint = camera.GetWorldPoint(e.X, e.Y, glControl.ClientSize.Width, glControl.ClientSize.Height, 15.0f).Xy;
                                customTempPoints.Clear();
                                foreach (var item in ProgramCore.Project.BaseDots)
                                    customTempPoints.Add(item.Clone());
                            }
                            break;
                        case Mode.SetCustomPoints:
                            {
                                customLastPoint = customLastRectPoint = camera.GetWorldPoint(e.X, e.Y, glControl.ClientSize.Width, glControl.ClientSize.Height, 15.0f).Xy;
                                if (dblClick)
                                {
                                    RectTransformMode = false;
                                    autodotsShapeHelper.ShapeInfo.Points.ClearSelection();
                                }

                                #region Rectangle transform

                                moveRectIndex = -1;
                                if (customLastPoint.X >= FaceRectTransformed.X - 1 && customLastPoint.X <= FaceRectTransformed.X + 1 && customLastPoint.Y >= FaceRectTransformed.Y - 1 && customLastPoint.Y <= FaceRectTransformed.Y + 1)
                                    moveRectIndex = 1;
                                else if (customLastPoint.X >= FaceRectTransformed.X + FaceRectTransformed.Width - 1 && customLastPoint.X <= FaceRectTransformed.X + FaceRectTransformed.Width + 1
                                         && customLastPoint.Y >= FaceRectTransformed.Y - 1 && customLastPoint.Y <= FaceRectTransformed.Y + 1)
                                    moveRectIndex = 2;
                                else if (customLastPoint.X >= FaceRectTransformed.X + FaceRectTransformed.Width - 1 && customLastPoint.X <= FaceRectTransformed.X + FaceRectTransformed.Width + 1
                                         && customLastPoint.Y >= FaceRectTransformed.Y + FaceRectTransformed.Height - 1 && customLastPoint.Y <= FaceRectTransformed.Y + FaceRectTransformed.Height + 1)
                                    moveRectIndex = 3;
                                else if (customLastPoint.X >= FaceRectTransformed.X - 1 && customLastPoint.X <= FaceRectTransformed.X + 1 && customLastPoint.Y >= FaceRectTransformed.Y + FaceRectTransformed.Height - 1
                                         && customLastPoint.Y <= FaceRectTransformed.Y + FaceRectTransformed.Height + 1)
                                    moveRectIndex = 4;

                                #endregion

                                if (moveRectIndex == -1) // если таскаем не прямоугольник, а точки
                                {
                                    customTempPoints.Clear();
                                    foreach (var item in autodotsShapeHelper.ShapeInfo.Points.SelectedPoints)
                                        customTempPoints.Add(item.Clone());
                                }
                                else
                                {
                                    tempMoveRectCenter.X = FaceRectTransformed.X + FaceRectTransformed.Width * 0.5f;
                                    tempMoveRectCenter.Y = FaceRectTransformed.Y + FaceRectTransformed.Height * 0.5f;

                                    tempMoveRectWidth = FaceRectTransformed.Width;
                                    tempMoveRectHeight = FaceRectTransformed.Height;
                                }
                            }
                            break;
                        case Mode.SetCustomProfilePoints:
                            {
                                customLastPoint = customLastRectPoint = camera.GetWorldPoint(e.X, e.Y, glControl.ClientSize.Width, glControl.ClientSize.Height, 15.0f).Zy;
                                customLastPoint.X *= -1;
                                customLastRectPoint.X *= -1;

                                if (dblClick)
                                {
                                    RectTransformMode = false;
                                    autodotsShapeHelper.ShapeProfileInfo.Points.ClearSelection();
                                }

                                #region Rectangle transform

                                moveRectIndex = -1;
                                if (customLastPoint.X >= FaceRectTransformed.X - 1 && customLastPoint.X <= FaceRectTransformed.X + 1 && customLastPoint.Y >= FaceRectTransformed.Y - 1 && customLastPoint.Y <= FaceRectTransformed.Y + 1)
                                    moveRectIndex = 1;
                                else if (customLastPoint.X >= FaceRectTransformed.X + FaceRectTransformed.Width - 1 && customLastPoint.X <= FaceRectTransformed.X + FaceRectTransformed.Width + 1
                                         && customLastPoint.Y >= FaceRectTransformed.Y - 1 && customLastPoint.Y <= FaceRectTransformed.Y + 1)
                                    moveRectIndex = 2;
                                else if (customLastPoint.X >= FaceRectTransformed.X + FaceRectTransformed.Width - 1 && customLastPoint.X <= FaceRectTransformed.X + FaceRectTransformed.Width + 1
                                         && customLastPoint.Y >= FaceRectTransformed.Y + FaceRectTransformed.Height - 1 && customLastPoint.Y <= FaceRectTransformed.Y + FaceRectTransformed.Height + 1)
                                    moveRectIndex = 3;
                                else if (customLastPoint.X >= FaceRectTransformed.X - 1 && customLastPoint.X <= FaceRectTransformed.X + 1 && customLastPoint.Y >= FaceRectTransformed.Y + FaceRectTransformed.Height - 1
                                         && customLastPoint.Y <= FaceRectTransformed.Y + FaceRectTransformed.Height + 1)
                                    moveRectIndex = 4;

                                #endregion

                                if (moveRectIndex == -1) // если таскаем не прямоугольник, а точки
                                {
                                    customTempPoints.Clear();
                                    foreach (var item in autodotsShapeHelper.ShapeProfileInfo.Points.SelectedPoints)
                                        customTempPoints.Add(item.Clone());
                                }
                                else
                                {
                                    tempMoveRectCenter.X = FaceRectTransformed.X + FaceRectTransformed.Width * 0.5f;
                                    tempMoveRectCenter.Y = FaceRectTransformed.Y + FaceRectTransformed.Height * 0.5f;

                                    tempMoveRectWidth = FaceRectTransformed.Width;
                                    tempMoveRectHeight = FaceRectTransformed.Height;
                                }
                            }
                            break;
                        case Mode.Brush:
                            {
                                historyController.Add(new HistoryBrush(brushTextures));
                                brushTool.StartBrush(camera.ViewMatrix);
                                var point = SliceController.UnprojectPoint(new Vector2(e.X, e.Y), camera.WindowWidth, camera.WindowHeight, camera.ProjectMatrix.Inverted());
                                brushTool.DrawBrush(point);
                                RenderBrush();
                            }
                            break;
                    }

                    #endregion
                }
            }
        }
        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (startMousePoint == Point.Empty)
                startMousePoint = new Point(e.X, e.Y);

            if (Math.Abs(startMousePoint.X - e.X) > 0.5 || Math.Abs(startMousePoint.Y - e.Y) > 0.5)   // small exp
                startPress = false;
            var newPoint = new Vector2(e.X, e.Y);
            if (leftMousePressed)
            {
                #region Левая кнопка нажата

                switch (ScaleMode)
                {
                    case ScaleMode.Rotate:
                        camera.LeftRight((e.Location.X - mX) * 1.0f / 150.0f);
                        break;
                    case ScaleMode.Move:
                        camera.dy -= (e.Location.Y - mY) * camera.Scale;

                        if (ProgramCore.MainForm.HeadProfile)
                            ProgramCore.MainForm.ctrlTemplateImage.UpdateProfileLocation();
                        break;
                    case ScaleMode.Zoom:
                        camera.Wheel((e.Location.Y - mY) / 150f * camera.Scale);

                        if (ProgramCore.MainForm.HeadProfile)
                            ProgramCore.MainForm.ctrlTemplateImage.UpdateProfileLocation();
                        break;
                    case ScaleMode.None:

                        #region Основные режимы

                        switch (Mode)
                        {
                            case Mode.Brush:
                                {
                                    var point = SliceController.UnprojectPoint(new Vector2(e.X, e.Y), camera.WindowWidth, camera.WindowHeight, camera.ProjectMatrix.Inverted());
                                    brushTool.DrawBrush(point);
                                    RenderBrush();
                                }
                                break;
                            case Mode.AccessoryRotateSetCircle:
                                {
                                    accessoryRotateCircleMousePoint = new Vector2(e.X, e.Y);
                                    accessoryRotateRadius = (float)Math.Sqrt(Math.Pow(accessoryRotateCircleMousePoint.X - accessoryRotateCenterCirclePoint.X, 2) + Math.Pow(accessoryRotateCircleMousePoint.Y - accessoryRotateCenterCirclePoint.Y, 2));
                                }
                                break;
                            case Mode.AccessoryRotate:
                                {
                                    accessoryRotateMousePoint = new Vector2(e.X, e.Y);

                                    var pointA = accessoryRotateCircleMousePoint - accessoryRotateCenterCirclePoint;
                                    var pointB = accessoryRotateMousePoint - accessoryRotateCenterCirclePoint;
                                    var angle = -(float)Math.Atan2(pointA.X * pointB.Y - pointB.X * pointA.Y, pointA.X * pointB.X + pointA.Y * pointB.Y);

                                    var accessoryMesh = pickingController.SelectedMeshes[0];        // always only one accessory can select.
                                    accessoryMesh.Rotate(angle, meshTempItems[accessoryMesh].Item2, true, accessoryRotateCenterPoint, meshTempItems[accessoryMesh].Item1);
                                }
                                break;
                            case Mode.HairStretchSetRect:
                            case Mode.HairShapeSetRect:
                            case Mode.HairPleatSetRect:
                                if (hairRectPointA != Vector2.Zero && hairRectPointB != Vector2.Zero)
                                {
                                    if (shiftPressed)
                                    {
                                        var d = newPoint - hairRectPointA;
                                        if (Math.Abs(d.X) > Math.Abs(d.Y))
                                            d.Y = 0.0f;
                                        else
                                            d.X = 0.0f;
                                        hairRectPointB = hairRectPointA + d;
                                    }
                                    else
                                        hairRectPointB = new Vector2(e.X, e.Y);
                                }
                                break;
                            case Mode.HairStretch:
                            case Mode.HairShape:
                            case Mode.HairPleat:
                                if (bHaveMouse)
                                {
                                    var ptCurrent = new Vector2(e.X, e.Y);
                                    MoveAPoint(hairSelectedPoint, ptCurrent);
                                }
                                break;
                            case Mode.HairCut:
                                sliceController.MovePoint(new Vector2(e.X, e.Y), shiftPressed, ToolMirrored ? Width : 0);
                                break;
                            case Mode.SetCustomControlPoints:
                                var mousePoint = camera.GetWorldPoint(e.X, e.Y, glControl.ClientSize.Width, glControl.ClientSize.Height, 15.0f).Xy;
                                var delta2 = mousePoint - customLastPoint;
                                for (var i = 0; i < ProgramCore.Project.BaseDots.Count; i++)
                                {
                                    var headPoint = ProgramCore.Project.BaseDots[i];
                                    if (!headPoint.Selected)
                                        continue;


                                    headPoint.Value = customTempPoints[i].Value + delta2;
                                }
                                break;
                            case Mode.SetCustomPoints:
                                {
                                    if (!startPress)
                                    {
                                        mousePoint = camera.GetWorldPoint(e.X, e.Y, glControl.ClientSize.Width, glControl.ClientSize.Height, 15.0f).Xy;
                                        if (moveRectIndex != -1)          //таскаем прямоугольничек
                                        {
                                            var deltaX = mousePoint.X - customLastRectPoint.X;
                                            var deltaY = mousePoint.Y - customLastRectPoint.Y;
                                            switch (moveRectIndex)
                                            {
                                                case 1:
                                                    FaceRectTransformed.X += deltaX;
                                                    FaceRectTransformed.Width -= deltaX;
                                                    FaceRectTransformed.Y += deltaY;
                                                    FaceRectTransformed.Height -= deltaY;
                                                    break;
                                                case 2:
                                                    FaceRectTransformed.Width += deltaX;
                                                    FaceRectTransformed.Y += deltaY;
                                                    FaceRectTransformed.Height -= deltaY;
                                                    break;
                                                case 3:
                                                    FaceRectTransformed.Width += deltaX;
                                                    FaceRectTransformed.Height += deltaY;
                                                    break;
                                                case 4:
                                                    FaceRectTransformed.Width -= deltaX;
                                                    FaceRectTransformed.X += deltaX;
                                                    FaceRectTransformed.Height += deltaY;
                                                    break;
                                            }

                                            customLastRectPoint = mousePoint;

                                            Vector2 center;
                                            center.X = FaceRectTransformed.X + FaceRectTransformed.Width * 0.5f;
                                            center.Y = FaceRectTransformed.Y + FaceRectTransformed.Height * 0.5f;

                                            var newWidth = FaceRectTransformed.Width;
                                            var newHeight = FaceRectTransformed.Height;
                                            var kx = newWidth / tempMoveRectWidth;
                                            var ky = newHeight / tempMoveRectHeight;
                                            foreach (var point in autodotsShapeHelper.ShapeInfo.Points.SelectedPoints)
                                            {
                                                var p = point.Value - tempMoveRectCenter;
                                                p.X *= kx;
                                                p.Y *= ky;
                                                point.Value = p + center;
                                            }
                                            tempMoveRectCenter = center;
                                            tempMoveRectWidth = newWidth;
                                            tempMoveRectHeight = newHeight;

                                            UpdateCustomFaceParts(false, false);
                                        }
                                        else            // таскаем точки
                                        {
                                            delta2 = mousePoint - customLastPoint;
                                            var selectedPoints = autodotsShapeHelper.ShapeInfo.Points.SelectedPoints;
                                            for (var i = 0; i < selectedPoints.Count; i++)
                                            {
                                                var headPoint = selectedPoints[i];
                                                headPoint.Value = customTempPoints[i].Value + delta2;
                                            }
                                            UpdateCustomFaceParts(true, true);
                                        }
                                    }
                                }
                                break;
                            case Mode.SetCustomProfilePoints:
                                {
                                    if (!startPress)
                                    {
                                        mousePoint = camera.GetWorldPoint(e.X, e.Y, glControl.ClientSize.Width, glControl.ClientSize.Height, 15.0f).Zy;
                                        mousePoint.X *= -1;

                                        if (moveRectIndex != -1)          //таскаем прямоугольничек
                                        {
                                            var deltaX = mousePoint.X - customLastRectPoint.X;
                                            var deltaY = mousePoint.Y - customLastRectPoint.Y;
                                            switch (moveRectIndex)
                                            {
                                                case 1:
                                                    FaceRectTransformed.X += deltaX;
                                                    FaceRectTransformed.Width -= deltaX;
                                                    FaceRectTransformed.Y += deltaY;
                                                    FaceRectTransformed.Height -= deltaY;
                                                    break;
                                                case 2:
                                                    FaceRectTransformed.Width += deltaX;
                                                    FaceRectTransformed.Y += deltaY;
                                                    FaceRectTransformed.Height -= deltaY;
                                                    break;
                                                case 3:
                                                    FaceRectTransformed.Width += deltaX;
                                                    FaceRectTransformed.Height += deltaY;
                                                    break;
                                                case 4:
                                                    FaceRectTransformed.Width -= deltaX;
                                                    FaceRectTransformed.X += deltaX;
                                                    FaceRectTransformed.Height += deltaY;
                                                    break;
                                            }

                                            customLastRectPoint = mousePoint;

                                            Vector2 center;
                                            center.X = FaceRectTransformed.X + FaceRectTransformed.Width * 0.5f;
                                            center.Y = FaceRectTransformed.Y + FaceRectTransformed.Height * 0.5f;

                                            var newWidth = FaceRectTransformed.Width;
                                            var newHeight = FaceRectTransformed.Height;
                                            var kx = newWidth / tempMoveRectWidth;
                                            var ky = newHeight / tempMoveRectHeight;
                                            for (var i = 0; i < autodotsShapeHelper.ShapeProfileInfo.Points.Count; i++)
                                            {
                                                var point = autodotsShapeHelper.ShapeProfileInfo.Points[i];
                                                if (point.Selected)
                                                {
                                                    var p = point.Value - tempMoveRectCenter;
                                                    p.X *= kx;
                                                    if (!baseProfilePoints.Contains(i))
                                                    {
                                                        p.Y *= ky;
                                                        point.Value = p + center;
                                                    }
                                                    else
                                                        point.Value = new Vector2(p.X + center.X, point.Value.Y);
                                                }
                                            }
                                            tempMoveRectCenter = center;
                                            tempMoveRectWidth = newWidth;
                                            tempMoveRectHeight = newHeight;

                                            UpdateCustomPointsFaceRect(false, true);
                                        }
                                        else            // таскаем точки
                                        {
                                            delta2 = mousePoint - customLastPoint;
                                            delta2.X *= -1;
                                            var delta3 = new Vector2(delta2.X, 0.0f);
                                            var index = 0;
                                            for (var i = 0; i < autodotsShapeHelper.ShapeProfileInfo.Points.Count; i++)
                                            {
                                                var headPoint = autodotsShapeHelper.ShapeProfileInfo.Points[i];
                                                if (headPoint.Selected)
                                                {
                                                    headPoint.Value = customTempPoints[index].Value + (baseProfilePoints.Contains(i) ? delta3 : delta2);
                                                    index++;
                                                }
                                            }
                                            UpdateCustomPointsFaceRect(true, true);
                                        }
                                    }
                                }
                                break;
                            case Mode.None:
                                {
                                    if (pickingController.SelectedMeshes.Count > 0)
                                    {
                                        var xy = glControl.PointToClient(new Point(e.X, e.Y));
                                        var point = camera.GetWorldPoint(xy.X, xy.Y, Width, Height, 0.0f);

                                        foreach (var mesh in pickingController.SelectedMeshes)
                                        {
                                            var meshTempInfo = meshTempItems[mesh];

                                            mesh.Transform = meshTempInfo.Item2;
                                            var delta = point - meshPoint;
                                            mesh.Transform[3, 0] += delta.X;
                                            mesh.Transform[3, 1] += delta.Y;
                                            mesh.Transform[3, 2] += delta.Z;

                                            mesh.Position = meshTempInfo.Item1 + delta;
                                        }
                                    }
                                }
                                break;
                        }

                        #endregion

                        break;
                }

                #endregion
            }
            else
            {
                switch (Mode)
                {
                    case Mode.HairStretch:
                    case Mode.HairShape:
                    case Mode.HairPleat:
                        var isSelected = false;
                        foreach (var point in HairRect)
                            if (e.X >= point.Value.X - 5 && e.X <= point.Value.X + 5 && e.Y >= point.Value.Y - 5 && e.Y <= point.Value.Y + 5)
                            {
                                hairSelectedPoint = point.Key;
                                isSelected = true;
                                break;
                            }

                        if (!isSelected)
                            hairSelectedPoint = -1;

                        break;
                }
            }

            mX = e.Location.X;
            mY = e.Location.Y;
        }
        private void glControl_MouseUp(object sender, MouseEventArgs e)
        {
            startMousePoint = Point.Empty;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    leftMousePressed = false;

                    switch (ScaleMode)
                    {
                        case ScaleMode.Move:
                        case ScaleMode.Rotate:
                        case ScaleMode.Zoom:
                            if (ProgramCore.MainForm.activePanel == 4)
                            {
                                if (ProgramCore.Project.projectCamera == null)
                                    // сохраненняем позицию камеры для окна Stage
                                    ProgramCore.Project.projectCamera = new Camera();



                                ProgramCore.Project.projectCamera._dy = camera._dy;
                                ProgramCore.Project.projectCamera.beta = camera.beta;
                                ProgramCore.Project.projectCamera.Scale = camera.Scale;
                                camera.AfterLoadVoid();
                            }
                            break;
                    }


                    switch (Mode)
                    {
                        case Mode.AccessoryRotateSetCircle:
                            {
                                Mode = Mode.AccessoryRotate;
                            }
                            break;
                        case Mode.AccessoryRotate:
                            {
                                accessoryRotateCenterCirclePoint = Vector2.Zero;
                                accessoryRotateCircleMousePoint = Vector2.Zero;
                                accessoryRotateMousePoint = Vector2.Zero;
                                accessoryRotateRadius = 0f;
                                Mode = Mode.AccessoryRotateSetCircle;
                            }
                            break;
                        case Mode.HairStretchSetRect:
                        case Mode.HairShapeSetRect:
                        case Mode.HairPleatSetRect:
                            break;
                        case Mode.HairStretch:
                        case Mode.HairShape:
                        case Mode.HairPleat:
                            bHaveMouse = false;
                            Cursor = DefaultCursor;
                            break;
                        case Mode.HairCut:
                            sliceController.AddPoint(new Vector2(e.X, e.Y), shiftPressed, ToolMirrored ? Width : 0);
                            break;
                        case Mode.LassoStart:
                            LassoPoints.Add(new Vector2(e.X, e.Y));
                            break;
                        case Mode.HeadShapeFirstTime:
                            if (ProgramCore.Project.ShapeFlip != FlipType.None)
                                return;

                            Mode = Mode.HeadShape;
                            ProgramCore.MainForm.frmFreeHand.cbMirror.Enabled = true;
                            HeadShapeP = SliceController.UnprojectPoint(new Vector2(e.X, e.Y), camera.WindowWidth, camera.WindowHeight, camera.ProjectMatrix.Inverted());
                            HeadShapeZ = HeadShapeController.StartShaping(new Vector3(HeadShapeP.X, HeadShapeP.Y, 0.0f), camera.ViewMatrix, ProgramCore.MainForm.frmFreeHand.UseMirror, ProgramCore.MainForm.frmFreeHand.Radius, ProgramCore.MainForm.frmFreeHand.CoefType);
                            break;
                        case Mode.HeadShape:
                            if (ProgramCore.Project.ShapeFlip != FlipType.None)
                                return;

                            ProgramCore.MainForm.frmFreeHand.cbMirror.Enabled = false;
                            HeadShapeP = SliceController.UnprojectPoint(new Vector2(e.X, e.Y), camera.WindowWidth, camera.WindowHeight, camera.ProjectMatrix.Inverted());
                            var p1 = new Vector3(HeadShapeP.X, HeadShapeP.Y, HeadShapeZ);
                            p1 = Vector3.Transform(p1, camera.ViewMatrix.Inverted());
                            Dictionary<Guid, MeshUndoInfo> undoInfo;
                            HeadShapeController.MoveShapePoint(p1, out undoInfo);

                            historyController.Add(new HistoryHeadShape(undoInfo));
                            break;
                        case Mode.SetCustomControlPoints:
                            {
                                if (!shiftPressed)
                                    foreach (var point in ProgramCore.Project.BaseDots)
                                        point.Selected = false;

                                var mousePoint = camera.GetWorldPoint(e.X, e.Y, glControl.ClientSize.Width, glControl.ClientSize.Height, 15.0f).Xy;
                                ProgramCore.Project.BaseDots.UpdatePointSelection(mousePoint, 0.5f);
                            }
                            break;
                        case Mode.SetCustomPoints:
                            {
                                if (startPress && !dblClick)
                                {
                                    var mousePoint = camera.GetWorldPoint(e.X, e.Y, glControl.ClientSize.Width, glControl.ClientSize.Height, 15.0f).Xy;
                                    if (!shiftPressed)
                                        autodotsShapeHelper.ShapeInfo.Points.ClearSelection();

                                    if (mousePoint.X >= MouthUserCenter.X - customSelectionRadius && mousePoint.X <= MouthUserCenter.X + customSelectionRadius && mousePoint.Y >= MouthUserCenter.Y - customSelectionRadius && mousePoint.Y <= MouthUserCenter.Y + customSelectionRadius)       // рот
                                        autodotsShapeHelper.ShapeInfo.Points.SelectPoints(autodotsShapeHelper.GetMouthIndexes());
                                    else if (mousePoint.X >= LeftEyeUserCenter.X - customSelectionRadius && mousePoint.X <= LeftEyeUserCenter.X + customSelectionRadius && mousePoint.Y >= LeftEyeUserCenter.Y - customSelectionRadius && mousePoint.Y <= LeftEyeUserCenter.Y + customSelectionRadius)  // левый глаз
                                        autodotsShapeHelper.ShapeInfo.Points.SelectPoints(autodotsShapeHelper.GetLeftEyeIndexes());
                                    else if (mousePoint.X >= RightEyeUserCenter.X - customSelectionRadius && mousePoint.X <= RightEyeUserCenter.X + customSelectionRadius && mousePoint.Y >= RightEyeUserCenter.Y - customSelectionRadius && mousePoint.Y <= RightEyeUserCenter.Y + customSelectionRadius)  // правый глаз
                                        autodotsShapeHelper.ShapeInfo.Points.SelectPoints(autodotsShapeHelper.GetRightEyeIndexes());
                                    else if (mousePoint.X >= NoseUserCenter.X - customSelectionRadius && mousePoint.X <= NoseUserCenter.X + customSelectionRadius && mousePoint.Y >= NoseUserCenter.Y - customSelectionRadius && mousePoint.Y <= NoseUserCenter.Y + customSelectionRadius) // нос
                                        autodotsShapeHelper.ShapeInfo.Points.SelectPoints(autodotsShapeHelper.GetNoseIndexes());
                                    else if (mousePoint.X >= CentralFacePoint.X - customSelectionRadius && mousePoint.X <= CentralFacePoint.X + customSelectionRadius && mousePoint.Y >= CentralFacePoint.Y - customSelectionRadius && mousePoint.Y <= CentralFacePoint.Y + customSelectionRadius) // прямоугольник и выделение всех точек
                                    {
                                        if (RectTransformMode)
                                        {
                                            RectTransformMode = false;
                                            autodotsShapeHelper.ShapeInfo.Points.ClearSelection();
                                        }
                                        else
                                        {
                                            RectTransformMode = true;
                                            UpdateCustomFaceParts(true, true);

                                            autodotsShapeHelper.ShapeInfo.Points.ClearSelection();
                                            foreach (var index in autodotsShapeHelper.GetFaceIndexes())
                                                autodotsShapeHelper.ShapeInfo.Points[index].Selected = true;
                                        }
                                    }
                                    else
                                        autodotsShapeHelper.ShapeInfo.Points.UpdatePointSelection(mousePoint);
                                }
                            }
                            break;
                        case Mode.SetCustomProfilePoints:
                            {
                                if (startPress && !dblClick)
                                {
                                    var mousePoint = camera.GetWorldPoint(e.X, e.Y, glControl.ClientSize.Width, glControl.ClientSize.Height, 15.0f).Zy;
                                    var reflectedMousePoint = new Vector2(-mousePoint.X, mousePoint.Y);

                                    if (!shiftPressed)
                                        autodotsShapeHelper.ShapeProfileInfo.Points.ClearSelection();

                                    if (reflectedMousePoint.X >= CentralFacePoint.X - customSelectionRadius && reflectedMousePoint.X <= CentralFacePoint.X + customSelectionRadius && reflectedMousePoint.Y >= CentralFacePoint.Y - customSelectionRadius && reflectedMousePoint.Y <= CentralFacePoint.Y + customSelectionRadius) // прямоугольник и выделение всех точек
                                    {
                                        if (RectTransformMode)
                                        {
                                            RectTransformMode = false;
                                            autodotsShapeHelper.ShapeProfileInfo.Points.ClearSelection();
                                        }
                                        else
                                        {
                                            RectTransformMode = true;
                                            UpdateCustomPointsFaceRect(true, true);

                                            autodotsShapeHelper.ShapeProfileInfo.Points.SelectAll();
                                        }
                                    }
                                    else
                                        autodotsShapeHelper.ShapeProfileInfo.Points.UpdatePointSelection(mousePoint);
                                }
                            }
                            break;

                        default:
                            {
                                if (startPress)
                                {
                                    pickingController.UpdateSelectedFace(e.X, e.Y);
                                    startPress = false;
                                }
                            }
                            break;
                    }
                    break;
            }
        }

        private void glControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }
        private void glControl_DragDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            var fi = new FileInfo((data as string[])[0]);

            var ctrl = new ctrlNewPart();
            if (ProgramCore.ShowDialog(this, ctrl, "New part", MessageBoxButtons.OKCancel, false) != DialogResult.OK)
                return;

            var meshType = fi.FullName.Contains("Style") ? MeshType.Hair : MeshType.Accessory;
            if (meshType == MeshType.Hair)                          // only one hair in time
                CleanHairMeshes();

            if (!PartsLibraryMeshes.ContainsKey(ctrl.Title))
                PartsLibraryMeshes.Add(ctrl.Title, new DynamicRenderMeshes());

            var objPath = Path.Combine(fi.DirectoryName, Path.GetFileNameWithoutExtension(fi.Name) + ".obj");
            var meshes = pickingController.AddMehes(objPath, meshType, true, ProgramCore.Project.ManType, false);

            var meshSize = float.NaN;
            var meshPosition = Vector3.Zero;
            if (meshes.Count > 0 && UserConfig.ByName("Parts").Contains(meshes[0].Path))
            {
                var mesh = meshes[0];
                meshSize = StringConverter.ToFloat(UserConfig.ByName("Parts")[mesh.Path, "Size"]);
                meshPosition = Vector3Ex.FromString(UserConfig.ByName("Parts")[mesh.Path, "Position"]);
            }

            for (var i = 0; i < meshes.Count; i++)
            {
                var mesh = meshes[i];
                if (mesh == null || mesh.vertexArray.Length == 0) //ТУТ!
                    continue;

                var xy = glControl.PointToClient(new Point(e.X, e.Y));

                Vector3 s;
                if (!float.IsNaN(meshSize))
                {
                    s = meshPosition;

                }
                else
                    s = camera.GetWorldPoint(xy.X, xy.Y, Width, Height, 15.0f);

                mesh.Position = new Vector3(s[0], s[1], s[2]);
                mesh.Transform[3, 0] += s[0];
                mesh.Transform[3, 1] += s[1];
                mesh.Transform[3, 2] += s[2];

                if (!float.IsNaN(meshSize))
                {
                    if (meshType == MeshType.Accessory)
                    {
                        mesh.Transform[3, 0] -= s[0]; // применяем изменение размера
                        mesh.Transform[3, 1] -= s[1];
                        mesh.Transform[3, 2] -= s[2];
                        mesh.Transform *= Matrix4.CreateScale(meshSize / mesh.MeshSize);
                        mesh.Transform[3, 0] += s[0];
                        mesh.Transform[3, 1] += s[1];
                        mesh.Transform[3, 2] += s[2];
                        mesh.IsChanged = true;
                        mesh.MeshSize = meshSize;

                    }
                    else mesh.InterpolateMesh(meshSize);
                }
                if (meshType == MeshType.Accessory)
                    mesh.Material.DiffuseColor = ProgramCore.Project.FaceColor;

                mesh.Title = ctrl.Title + "_" + i;
                PartsLibraryMeshes[ctrl.Title].Add(mesh);
            }
            ProgramCore.MainForm.frmParts.UpdateList();
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            if (!loaded)
                return;
            var c = sender as GLControl;
            SetupViewport(c);
            if (ProgramCore.MainForm != null)
            {
                if (ProgramCore.MainForm.HeadProfile)
                {
                    switch (ProgramCore.MainForm.ctrlTemplateImage.ControlPointsMode)
                    {
                        case ProfileControlPointsMode.None:         // опорные точки выставлены
                            {
                                UpdateProfileRectangle();
                                InitializeProfileCamera(ProgramCore.MainForm.ctrlTemplateImage.ModelAdaptParamProfile);
                            }
                            break;
                    }
                }
                else
                    InitialiseCamera(ProgramCore.MainForm.ctrlTemplateImage.ModelAdaptParam);

            }
        }

        #endregion

        #region Drawing

        public void Render()
        {
            if (!loaded)  // whlie context not create
                return;
            GL.ClearColor(Color.LightGray);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            DrawBackground();
            camera.PutCamera();

            GL.PushMatrix();
            GL.Enable(EnableCap.Texture2D);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Normalize);
            GL.Disable(EnableCap.CullFace);
            idleShader.Begin();
            DrawHead();

            if (ProgramCore.Debug)
            {
                if (SimpleDrawing)
                {
                    GL.LineWidth(1.0f);
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                }
                else
                {
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                }
            }

            var shader = idleShader;
            GL.Enable(EnableCap.DepthTest);
            DrawMeshes(pickingController.HairMeshes, ref shader, false);
            DrawMeshes(pickingController.AccesoryMeshes, ref shader, false);

            EnableTransparent();
            DrawMeshes(pickingController.HairMeshes, ref shader, true);
            DrawMeshes(pickingController.AccesoryMeshes, ref shader, true);
            DisableTransparent();
            idleShader.End();
            GL.PopMatrix();

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);
            if (ProgramCore.Debug)
            {
                DrawAxis();
                if (profileTrianglesDrowing)
                {
                    if (autodotsShapeHelper.ShapeProfileInfo.Points != null)
                    {
                        GL.Begin(PrimitiveType.Lines);
                        GL.Color4(0.0f, 1.0f, 0.0f, 1.0f);
                        for (var i = 0; i < autodotsShapeHelper.ShapeProfileInfo.Indices.Length; i += 3)
                        {
                            var a = autodotsShapeHelper.ShapeProfileInfo.Indices[i];
                            var b = autodotsShapeHelper.ShapeProfileInfo.Indices[i + 1];
                            var c = autodotsShapeHelper.ShapeProfileInfo.Indices[i + 2];

                            var p0 = autodotsShapeHelper.ShapeProfileInfo.Points[a];
                            var p1 = autodotsShapeHelper.ShapeProfileInfo.Points[b];
                            var p2 = autodotsShapeHelper.ShapeProfileInfo.Points[c];

                            GL.Vertex3(0.0f, p0.Value.Y, p0.Value.X);
                            GL.Vertex3(0.0f, p1.Value.Y, p1.Value.X);
                            GL.Vertex3(0.0f, p1.Value.Y, p1.Value.X);
                            GL.Vertex3(0.0f, p2.Value.Y, p2.Value.X);
                            GL.Vertex3(0.0f, p2.Value.Y, p2.Value.X);
                            GL.Vertex3(0.0f, p0.Value.Y, p0.Value.X);
                        }
                        GL.End();

                        GL.PointSize(5.0f);
                        GL.Begin(PrimitiveType.Points);
                        GL.Color4(0.0f, 0.75f, 0.25f, 1.0f);
                        foreach (var p in autodotsShapeHelper.ShapeProfileInfo.Points)
                            GL.Vertex3(0.0f, p.Value.Y, p.Value.X);
                        GL.End();
                    }
                }
            }

            if (pickingController.SelectedMeshes.Count == 1 && pickingController.SelectedMeshes[0].meshType == MeshType.Accessory)
            {
                GL.PointSize(5.0f);
                GL.Begin(PrimitiveType.Points);
                GL.Vertex3(pickingController.SelectedMeshes[0].Position);
                GL.End();
            }

            if (ProgramCore.MainForm.HeadMode)
            {
                DrawHeadTools();
                ProgramCore.MainForm.ctrlTemplateImage.pictureTemplate.Refresh();
            }

            if (ProgramCore.Debug && Mode == Mode.SetCustomProfilePoints)
            {
                EnableTransparent();
                GL.PointSize(5.0f);
                if (autodotsShapeHelper.ShapeProfileInfo.Points != null)
                {
                    GL.Begin(PrimitiveType.Triangles);
                    GL.Color4(0.0f, 1.0f, 0.0f, 0.3f);
                    for (var i = 0; i < autodotsShapeHelper.ShapeProfileInfo.Indices.Length; i += 3)
                    {
                        var a = autodotsShapeHelper.ShapeProfileInfo.Indices[i];
                        var b = autodotsShapeHelper.ShapeProfileInfo.Indices[i + 1];
                        var c = autodotsShapeHelper.ShapeProfileInfo.Indices[i + 2];
                        var p = autodotsShapeHelper.ShapeProfileInfo.Points[a];
                        GL.Vertex3(0.0f, p.Value.Y, p.Value.X);
                        p = autodotsShapeHelper.ShapeProfileInfo.Points[b];
                        GL.Vertex3(0.0f, p.Value.Y, p.Value.X);
                        p = autodotsShapeHelper.ShapeProfileInfo.Points[c];
                        GL.Vertex3(0.0f, p.Value.Y, p.Value.X);
                    }
                    GL.End();
                }
                DisableTransparent();
            }
            //            DrawProfileControlTmpPoints();
            if (ProgramCore.Debug)
            {
                DrawAABB();
                if (showTriangles)
                    DrawHelpTriangles();
                if (showInfo)
                    DrowPointsInfo();
            }

            switch (Mode)
            {
                case Mode.AccessoryRotate:
                case Mode.AccessoryRotateSetCircle:
                    DrawAccessoryRotatingCircle();
                    break;
                case Mode.HairShape:
                case Mode.HairStretch:
                case Mode.HairPleat:
                case Mode.HairShapeSetRect:
                case Mode.HairStretchSetRect:
                case Mode.HairPleatSetRect:
                    DrawHairRectangle();
                    break;
                case Mode.LassoStart:
                case Mode.LassoActive:
                    DrawLasso();
                    break;
                case Mode.HeadLine:
                    if (ProgramCore.MainForm.HeadFront)
                        DrawHeadLines();
                    else
                        DrawProfileHeadLines();
                    break;
                case Mode.SetCustomControlPoints:
                    DrawCustomControlPoints();
                    break;
                case Mode.SetCustomPoints:
                    DrawCustomPoints();

                    if (RectTransformMode)
                        DrawCustomFaceRect();

                    EnableTransparent();
                    GL.PointSize(5.0f);             // рисуем зеленый фон. удобнее настраивать
                    if (autodotsShapeHelper.ShapeProfileInfo.Points != null)
                    {
                        GL.Begin(PrimitiveType.Triangles);
                        GL.Color4(0.0f, 1.0f, 0.0f, 0.3f);
                        for (var i = 0; i < autodotsShapeHelper.ShapeInfo.Indices.Length; i += 3)
                        {
                            var a = autodotsShapeHelper.ShapeInfo.Indices[i];
                            var b = autodotsShapeHelper.ShapeInfo.Indices[i + 1];
                            var c = autodotsShapeHelper.ShapeInfo.Indices[i + 2];
                            var p = autodotsShapeHelper.ShapeInfo.Points[a];
                            GL.Vertex2(p.Value);
                            p = autodotsShapeHelper.ShapeInfo.Points[b];
                            GL.Vertex2(p.Value);
                            p = autodotsShapeHelper.ShapeInfo.Points[c];
                            GL.Vertex2(p.Value);
                        }
                        GL.End();
                    }
                    DisableTransparent();

                    break;
                case Mode.SetCustomProfilePoints:
                    DrawCustomProfilePoints();

                    if (RectTransformMode)
                        DrawCustomFaceRect();
                    break;
                case Mode.None:
                    if (ProgramCore.Debug)
                    {
                        switch (ProgramCore.MainForm.ctrlTemplateImage.ControlPointsMode)
                        {
                            case ProfileControlPointsMode.SetControlPoints:
                            case ProfileControlPointsMode.MoveControlPoints:
                                DrawProfileControlTmpPoints();
                                break;
                        }
                    }

                    switch (ProgramCore.MainForm.ctrlTemplateImage.ControlPointsMode)
                    {
                        case ProfileControlPointsMode.SetControlPoints:
                            DrawProfileControlPoints();
                            break;
                    }

                    break;
            }
            if (sliceController != null && sliceController.Lines.Count > 00)
                DrawSlice();
            glControl.SwapBuffers();
        }


        private bool isFullPointsInfo = true;
        private bool showInfo = true;
        private bool showTriangles = true;
        public bool IsFullPointsInfo
        {
            get
            {
                return isFullPointsInfo;
            }
            set
            {
                if (isFullPointsInfo != value)
                {
                    isFullPointsInfo = value;
                    TextRenderHelpers = null;
                }
            }
        }
        public List<TextRenderHelper> TextRenderHelpers = null;
        public Font TextFont = new Font(new FontFamily(GenericFontFamilies.SansSerif), 20, GraphicsUnit.Pixel);

        void DrawAABB()
        {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(1.0f, 0.0f, 0.0f);

            var p0 = headMeshesController.RenderMesh.AABB.A.Xy;
            var p1 = new Vector2(headMeshesController.RenderMesh.AABB.A.X, headMeshesController.RenderMesh.AABB.B.Y);
            var p2 = headMeshesController.RenderMesh.AABB.B.Xy;
            var p3 = new Vector2(headMeshesController.RenderMesh.AABB.B.X, headMeshesController.RenderMesh.AABB.A.Y);

            GL.Vertex2(p0);
            GL.Vertex2(p1);
            GL.Vertex2(p1);
            GL.Vertex2(p2);
            GL.Vertex2(p2);
            GL.Vertex2(p3);
            GL.Vertex2(p3);
            GL.Vertex2(p0);

            GL.End();
        }

        void DrawHelpTriangles()
        {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(0.0f, 0.0f, 1.0f);
            for (var i = 0; i < autodotsShapeHelper.ShapeInfo.Indices.Length; i += 3)
            {
                var p0 = autodotsShapeHelper.ShapeInfo.Points[autodotsShapeHelper.ShapeInfo.Indices[i]];
                var p1 = autodotsShapeHelper.ShapeInfo.Points[autodotsShapeHelper.ShapeInfo.Indices[i + 1]];
                var p2 = autodotsShapeHelper.ShapeInfo.Points[autodotsShapeHelper.ShapeInfo.Indices[i + 2]];

                GL.Vertex2(p0.Value);
                GL.Vertex2(p1.Value);
                GL.Vertex2(p1.Value);
                GL.Vertex2(p2.Value);
                GL.Vertex2(p2.Value);
                GL.Vertex2(p0.Value);
            }

            GL.End();

            EnableTransparent();

            GL.Begin(PrimitiveType.Triangles);
            GL.Color4(0.0f, 1.0f, 0.0f, 0.3f);
            for (var i = 0; i < autodotsShapeHelper.ShapeInfo.Indices.Length; i += 3)
            {
                var a = autodotsShapeHelper.ShapeInfo.Indices[i];
                var b = autodotsShapeHelper.ShapeInfo.Indices[i + 1];
                var c = autodotsShapeHelper.ShapeInfo.Indices[i + 2];
                var p = autodotsShapeHelper.ShapeInfo.Points[a];
                GL.Vertex2(p.Value);
                p = autodotsShapeHelper.ShapeInfo.Points[b];
                GL.Vertex2(p.Value);
                p = autodotsShapeHelper.ShapeInfo.Points[c];
                GL.Vertex2(p.Value);
            }
            GL.End();

            DisableTransparent();

            GL.PointSize(5.0f);
            GL.Begin(PrimitiveType.Points);

            foreach (var point in autodotsShapeHelper.ShapeInfo.Points)
            {
                if (!point.Visible)
                    continue;
                GL.Color3(1.0f, 0.0f, 0.0f);
                GL.Vertex2(point.Value);
            }

            GL.End();
            GL.PointSize(1.0f);
        }

        public void DrowPointsInfo()
        {
            const float scale = 0.015f;
            if (IsFullPointsInfo)
            {
                if (TextRenderHelpers == null)
                {
                    TextRenderHelpers = new List<TextRenderHelper>();
                    for (var i = 0; i < autodotsShapeHelper.ShapeInfo.Points.Count; i++)
                        TextRenderHelpers.Add(new TextRenderHelper(TextFont, Color4.Black,
                            i.ToString(CultureInfo.InvariantCulture))
                        {
                            Scale = scale,
                            Position = autodotsShapeHelper.ShapeInfo.Points[i].Value
                        });
                }
            }
            else
            {
                if (headController.ShapeDots.Count == 0)
                    return;
                if (TextRenderHelpers == null)
                {
                    TextRenderHelpers = new List<TextRenderHelper>();
                    for (var i = 0; i < headController.ShapeDots.Count; i++)
                        TextRenderHelpers.Add(new TextRenderHelper(TextFont, Color4.Black,
                            i.ToString(CultureInfo.InvariantCulture))
                        {
                            Scale = scale,
                            Position = headController.ShapeDots[i].Value
                        });
                }
            }

            EnableTransparent();

            foreach (var text in TextRenderHelpers)
            {
                GL.Translate(text.Position.X, text.Position.Y, 0);
                text.Render();
                GL.Translate(-text.Position.X, -text.Position.Y, 0);
            }

            GL.Disable(EnableCap.Texture2D);
            DisableTransparent();
        }

        public RectangleAABB InitializeShapedotsHelper(bool isNew = false)
        {
            autodotsShapeHelper.headMeshesController = headMeshesController;
            autodotsShapeHelper.SetType((int)ProgramCore.Project.ManType);
            autodotsShapeHelper.Initialise(HeadController.GetDots(ProgramCore.Project.ManType), isNew);
            baseProfilePoints = autodotsShapeHelper.InitializeProfile(HeadController.GetProfileBaseDots(ProgramCore.Project.ManType), isNew);

            if (!isNew)
                autodotsShapeHelper.TransformRects();

            var result = new RectangleAABB();

            if (isNew && ProgramCore.Project.ManType != ManType.Custom)
            {
                Vector3 min = result.A, max = result.B;
                for (var i = 8; i < autodotsShapeHelper.ShapeInfo.Points.Count; ++i)
                {
                    var p = autodotsShapeHelper.ShapeInfo.Points[i];
                    min.X = Math.Min(p.Value.X, min.X);
                    min.Y = Math.Min(p.Value.Y, min.Y);
                    max.X = Math.Max(p.Value.X, max.X);
                    max.Y = Math.Max(p.Value.Y, max.Y);
                }
                result.A = min;
                result.B = max;
            }

            return result;
        }

        private float GetMinY(IEnumerable<DynamicRenderMesh> meshes)
        {
            var minY = 99999.0f;
            var viewProjection = camera.ViewMatrix * camera.ProjectMatrix;

            foreach (var mesh in meshes)
            {
                var worldMatrix = mesh.Transform;//Если мы в режиме просмотра морфинга, то делаем это
                if (InStageMode)
                {
                    var position = new Vector3(worldMatrix[3, 0], worldMatrix[3, 1], worldMatrix[3, 2]);
                    worldMatrix[3, 0] = 0.0f;
                    worldMatrix[3, 1] = 0.0f;
                    worldMatrix[3, 2] = 0.0f;
                    worldMatrix *= Matrix4.CreateScale(headMeshesController.RenderMesh.RealScale);
                    worldMatrix[3, 0] = position.X * headMeshesController.RenderMesh.RealScale;
                    worldMatrix[3, 1] = position.Y * headMeshesController.RenderMesh.RealScale;
                    worldMatrix[3, 2] = position.Z * headMeshesController.RenderMesh.RealScale;
                }

                foreach (var vertex in mesh.vertexArray)
                {
                    var position = Vector3.Transform(vertex.Position, worldMatrix);
                    var y = Vector3.Transform(position, viewProjection).Y;
                    if (y < minY)
                        minY = y;
                }
            }
            return minY;
        }

        public void UpdateCameraPosition(float shelf)
        {
            var minY = GetMinY(pickingController.HairMeshes);
            var y = GetMinY(pickingController.AccesoryMeshes);
            if (y < minY)
                minY = y;
            var viewProjection = camera.ViewMatrix * camera.ProjectMatrix;
            var worldMatrix = //Если мы в режиме просмотра морфинга, то делаем это
                InStageMode ? Matrix4.CreateScale(headMeshesController.RenderMesh.RealScale) : Matrix4.Identity;
            foreach (var part in headMeshesController.RenderMesh.Parts)
            {
                foreach (var vertex in part.Vertices)
                {
                    var position = Vector3.Transform(vertex.Position, worldMatrix);
                    y = Vector3.Transform(position, viewProjection).Y;
                    if (y < minY)
                        minY = y;
                }
            }

            var dy = (shelf - minY) * camera.WindowHeight * camera.Scale * 0.5f;
            camera.dy -= dy;
            /*
             pickingController.HairMeshes, re
             pickingController.AccesoryMeshes
             * 
             vec4 position = u_World * vec4( gl_Vertex.xyz, 1.0 );
    gl_Position = u_ViewProjection * position;
             * 
             idleShader.UpdateUniform("u_LightDirection", Vector3.Normalize(camera.Position));
            var worldMatrix = //Если мы в режиме просмотра морфинга, то делаем это
                inStageMode ? Matrix4.CreateScale(headMeshesController.RenderMesh.RealScale) : Matrix4.Identity;
            //Иначе берем Matrix4.Identity
            idleShader.UpdateUniform("u_World", worldMatrix);
            idleShader.UpdateUniform("u_WorldView", worldMatrix * camera.ViewMatrix);
             */
        }

        private void DrawMeshes(IEnumerable<BaseRenderMesh> meshes, ref ShaderController shader, bool transparent)
        {
            foreach (var mesh in meshes)
            {
                if (mesh.Material.IsTransparent != transparent)
                    continue;

                var worldMatrix = mesh.Transform;//Если мы в режиме просмотра морфинга, то делаем это
                if (InStageMode)
                {
                    var position = new Vector3(worldMatrix[3, 0], worldMatrix[3, 1], worldMatrix[3, 2]);
                    worldMatrix[3, 0] = 0.0f;
                    worldMatrix[3, 1] = 0.0f;
                    worldMatrix[3, 2] = 0.0f;
                    worldMatrix *= Matrix4.CreateScale(headMeshesController.RenderMesh.RealScale);
                    worldMatrix[3, 0] = position.X * headMeshesController.RenderMesh.RealScale;
                    worldMatrix[3, 1] = position.Y * headMeshesController.RenderMesh.RealScale;
                    worldMatrix[3, 2] = position.Z * headMeshesController.RenderMesh.RealScale;
                }

                shader.UpdateUniform("u_World", worldMatrix);
                shader.UpdateUniform("u_WorldView", worldMatrix * camera.ViewMatrix);
                shader.UpdateUniform("u_ViewProjection", camera.ViewMatrix * camera.ProjectMatrix);
                mesh.Draw(shader);
            }
        }

        static public void DrawAxis()
        {
            GL.LineWidth(1.0f);
            GL.DepthMask(false);
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Red);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(100.0, 0.0, 0.0);
            GL.Color3(Color.Blue);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(0.0, 100.0, 0.0);
            GL.Color3(Color.Green);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(0.0, 0.0, 100.0);
            GL.End();

            GL.Enable(EnableCap.LineStipple);
            GL.LineStipple(1, 255);
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Red);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(-100.0, 0.0, 0.0);
            GL.Color3(Color.Blue);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(0.0, -100.0, 0.0);
            GL.Color3(Color.Green);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(0.0, 0.0, -100.0);
            GL.End();
            GL.Disable(EnableCap.LineStipple);

            GL.DepthMask(true);
        }

        private void DrawSlice()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            var orhto = Matrix4.CreateOrthographic(Width, -Height, 0.0f, 100.0f);
            GL.LoadMatrix(ref orhto);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(-Width * 0.5d, -Height * 0.5d, 0d);

            GL.DepthMask(false);

            sliceController.Draw();

            GL.DepthMask(true);
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
        }
        private void DrawAccessoryRotatingCircle()
        {
            if (accessoryRotateCenterCirclePoint == Vector2.Zero)
                return;

            const float numSegments = 36;
            const float theta = (float)(2 * 3.1415926 / numSegments);
            var c = (float)Math.Cos(theta);//precalculate the sine and cosine
            var s = (float)Math.Sin(theta);
            float t;

            var x = accessoryRotateRadius;//we start at angle = 0 
            float y = 0;

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            var orhto = Matrix4.CreateOrthographic(Width, -Height, 0.0f, 100.0f);
            GL.LoadMatrix(ref orhto);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(-Width * 0.5d, -Height * 0.5d, 0d);

            GL.DepthMask(false);

            GL.PointSize(5.0f);
            GL.Begin(PrimitiveType.Points);
            GL.Color3(Color.Blue);
            GL.Vertex2(accessoryRotateCenterCirclePoint);
            GL.Vertex2(accessoryRotateCircleMousePoint);

            if (accessoryRotateMousePoint != Vector2.Zero)
                GL.Vertex2(accessoryRotateMousePoint);
            GL.End();


            if (accessoryRotateCircleMousePoint != Vector2.Zero)
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Color3(Color.Blue);
                GL.Vertex2(accessoryRotateCenterCirclePoint);
                GL.Vertex2(accessoryRotateCircleMousePoint);
                GL.End();
            }
            if (accessoryRotateMousePoint != Vector2.Zero)
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Color3(Color.Green);
                GL.Vertex2(accessoryRotateCenterCirclePoint);
                GL.Vertex2(accessoryRotateMousePoint);
                GL.End();
            }

            GL.Begin(PrimitiveType.LineLoop);
            GL.Color3(Color.Red);

            for (var ii = 0; ii < numSegments; ii++)            // num_segments
            {
                GL.Vertex2(x + accessoryRotateCenterCirclePoint.X, y + accessoryRotateCenterCirclePoint.Y);//output vertex 

                //apply the rotation matrix
                t = x;
                x = c * x - s * y;
                y = s * t + c * y;
            }
            GL.End();

            GL.DepthMask(true);
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
        }
        private void DrawLasso()
        {
            if (LassoPoints.Count == 0)
                return;

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            var orhto = Matrix4.CreateOrthographic(Width, -Height, 0.0f, 100.0f);
            GL.LoadMatrix(ref orhto);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(-Width * 0.5d, -Height * 0.5d, 0d);

            GL.DepthMask(false);

            #region Drawing

            GL.Color3(1.0f, 0.0f, 0.0f);

            GL.Begin(PrimitiveType.Lines);
            if (LassoPoints.Count > 1)
                for (var i = 1; i < LassoPoints.Count; i++)
                {
                    GL.Vertex2(LassoPoints[i - 1]);
                    GL.Vertex2(LassoPoints[i]);
                }

            GL.End();

            GL.PointSize(5.0f);
            GL.Begin(PrimitiveType.Points);
            foreach (var line in LassoPoints)
                GL.Vertex2(line);
            if (LassoPoints.Count > 0)
                GL.Vertex2(LassoPoints.Last());
            GL.End();
            GL.PointSize(1.0f);

            #endregion

            GL.DepthMask(true);
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
        }

        /// <summary> Отрисовка головы в режиме работы с головой </summary>
        private void DrawHead()
        {
            idleShader.UpdateUniform("u_LightDirection", Vector3.Normalize(camera.Position));
            var worldMatrix = //Если мы в режиме просмотра морфинга, то делаем это
                InStageMode ? Matrix4.CreateScale(headMeshesController.RenderMesh.RealScale) : Matrix4.Identity;
            //Иначе берем Matrix4.Identity
            idleShader.UpdateUniform("u_World", worldMatrix);
            idleShader.UpdateUniform("u_WorldView", worldMatrix * camera.ViewMatrix);
            idleShader.UpdateUniform("u_ViewProjection", camera.ViewMatrix * camera.ProjectMatrix);

            headMeshesController.Draw(ProgramCore.Debug);
        }

        private void RenderMesh_OnBeforePartDraw(RenderMeshPart part)
        {
            var transparent = UseTexture ? (float)part.TransparentTexture : 0.0f;
            if (transparent > 0.0f)
                EnableTransparent();
            else
                DisableTransparent();

            var shader = idleShader;
            var useTextures = Vector3.Zero;

            if (brushTextures.ContainsKey(part.Texture))
            {
                GL.ActiveTexture(TextureUnit.Texture2);
                GL.BindTexture(TextureTarget.Texture2D, brushTextures[part.Texture].Texture);
                shader.UpdateUniform("u_BrushMap", 2);
                useTextures.Z = 1.0f;
            }

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, part.TransparentTexture);
            shader.UpdateUniform("u_TransparentMap", 1);
            //shader.UpdateUniform("u_UseTransparent", transparent);            
            useTextures.Y = transparent;
            if (!ProgramCore.PluginMode)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, part.Texture);
                shader.UpdateUniform("u_Texture", 0);
                useTextures.X = UseTexture ? part.Texture : 0.0f;
                //shader.UpdateUniform("u_UseTexture", UseTexture ? part.Texture : 0.0f);
                shader.UpdateUniform("u_Color", part.Color);
            }
            else
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                var pName = part.Name.ToLower();
                var texture = ProgramCore.Project.ManType != ManType.Custom || ProgramCore.MainForm.PluginUvGroups.Contains(pName) ? part.Texture : 0;
                GL.BindTexture(TextureTarget.Texture2D, texture);
                shader.UpdateUniform("u_Texture", 0);
                useTextures.X = UseTexture ? texture : 0.0f;
                //shader.UpdateUniform("u_UseTexture", UseTexture ? texture : 0.0f);
                shader.UpdateUniform("u_Color", part.Color);
            }

            shader.UpdateUniform("u_UseTexture", useTextures);
        }

        private void DrawHeadTools()
        {
            GL.DepthMask(false);

            headController.Draw();

            GL.DepthMask(true);
        }
        private void DrawHeadLines()
        {
            GL.Begin(PrimitiveType.Lines);
            var index = 0;
            foreach (var r in autodotsShapeHelper.Rects)
            {
                //if (r.Type == MeshPartType.None || r.Type == MeshPartType.LEar || r.Type == MeshPartType.REar)
                //    continue;

                if (r.Type == HeadLineMode)
                {
                    if (r.Type == MeshPartType.Lip)
                    {
                        ++index;
                        if ((index <= 7 && headController.Lines.Count < 2) || (index > 7 && headController.Lines.Count == 2))
                            GL.Color3(Color.Red);
                        else
                            GL.Color3(Color.Green);
                    }
                    else
                        GL.Color3(Color.Red);
                }
                else
                    GL.Color3(Color.Green);

                for (var i = 1; i < r.Points.Length; i++)
                {
                    GL.Vertex2(r.Points[i - 1]);
                    GL.Vertex2(r.Points[i]);
                }

            }
            GL.End();
            GL.DepthMask(true);
        }
        private void DrawProfileHeadLines()
        {
            GL.DepthMask(false);

            GL.Begin(PrimitiveType.Lines);
            foreach (var rect in autodotsShapeHelper.ProfileRects)
                if (rect.LinkedShapeRect != null)
                {
                    GL.Color3(Color.Red);

                    for (var i = 1; i < rect.Points.Length; i++)
                    {
                        GL.Vertex3(0.0f, rect.Points[i - 1].Y, rect.Points[i - 1].X);
                        GL.Vertex3(0.0f, rect.Points[i].Y, rect.Points[i].X);
                    }
                }

            GL.End();

            if (ProgramCore.Debug)
            {
                if (ProfileFaceRect != RectangleF.Empty)
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Color3(Color.Red);
                    GL.Vertex2(new Vector2(ProfileFaceRect.Left, ProfileFaceRect.Top));
                    GL.Vertex2(new Vector2(ProfileFaceRect.Right, ProfileFaceRect.Top));

                    GL.Vertex2(new Vector2(ProfileFaceRect.Right, ProfileFaceRect.Top));
                    GL.Vertex2(new Vector2(ProfileFaceRect.Right, ProfileFaceRect.Bottom));

                    GL.Vertex2(new Vector2(ProfileFaceRect.Right, ProfileFaceRect.Bottom));
                    GL.Vertex2(new Vector2(ProfileFaceRect.Left, ProfileFaceRect.Bottom));

                    GL.Vertex2(new Vector2(ProfileFaceRect.Left, ProfileFaceRect.Bottom));
                    GL.Vertex2(new Vector2(ProfileFaceRect.Left, ProfileFaceRect.Top));
                    GL.End();

                    GL.MatrixMode(MatrixMode.Modelview);
                    GL.LoadIdentity();
                    GL.Translate(0.0f, -camera.dy, 0.0f);

                    GL.PointSize(5.0f);
                    GL.Begin(PrimitiveType.Points);
                    foreach (var line in headController.Lines)
                        foreach (var point in line)
                        {
                            GL.Color3(0.0f, 1.0f, 0.0f);

                            GL.Vertex2(point.Value);
                        }
                    GL.End();
                }
            }

            GL.DepthMask(true);
        }

        private void DrawProfileControlTmpPoints()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Translate(0.0f, -camera.dy, 0.0f);

            GL.DepthMask(false);

            GL.PointSize(5.0f);
            GL.Begin(PrimitiveType.Points);

            foreach (var point in ProgramCore.MainForm.ctrlTemplateImage.profileControlPoints)
            {
                GL.Color3(0.0f, 1.0f, 0.0f);
                GL.Vertex2(point.Value);
            }
            GL.End();
            if (ProgramCore.MainForm.ctrlTemplateImage.profileControlPoints.Count > 0)
            {
                GL.LineWidth(2.0f);
                GL.Begin(PrimitiveType.Lines);
                for (var i = 1; i < 3; ++i)
                {
                    GL.Color3(0.0f, 1.0f, 0.0f);
                    var point = ProgramCore.MainForm.ctrlTemplateImage.profileControlPoints[i];
                    GL.Vertex2(point.Value);
                }
                GL.End();
            }

            GL.DepthMask(true);
            GL.PopMatrix();
        }
        private void DrawProfileControlPoints()
        {
            foreach (var sprite in profilePointSprites)
                sprite.Draw(true, true);
        }
        private void DrawHairRectangle()
        {
            var halfWidth = Width * 0.5f;
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            var orhto = Matrix4.CreateOrthographic(Width, -Height, 0.0f, 100.0f);
            GL.LoadMatrix(ref orhto);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(-Width * 0.5d, -Height * 0.5d, 0d);

            GL.DepthMask(false);

            GL.PointSize(5.0f);
            GL.Begin(PrimitiveType.Points);
            GL.Color3(Color.Red);
            if (hairRectPointA != Vector2.Zero)
            {
                GL.Vertex2(hairRectPointA);
                if (ToolShapeMirrored)
                    GL.Vertex2(new Vector2(halfWidth - (hairRectPointA.X - halfWidth), hairRectPointA.Y));
            }
            if (hairRectPointB != Vector2.Zero)
            {
                GL.Vertex2(hairRectPointB);
                if (ToolShapeMirrored)
                    GL.Vertex2(new Vector2(halfWidth - (hairRectPointB.X - halfWidth), hairRectPointB.Y));
            }

            GL.End();

            GL.Begin(PrimitiveType.Lines);
            if (hairRectPointA != Vector2.Zero && hairRectPointB != Vector2.Zero)
            {
                GL.Color3(Color.Red);
                GL.Vertex2(hairRectPointA);
                GL.Vertex2(hairRectPointB);

                if (ToolShapeMirrored)
                {
                    GL.Vertex2(new Vector2(halfWidth - (hairRectPointA.X - halfWidth), hairRectPointA.Y));
                    GL.Vertex2(new Vector2(halfWidth - (hairRectPointB.X - halfWidth), hairRectPointB.Y));
                }
            }

            if (Mode == Mode.HairShape || Mode == Mode.HairStretch || Mode == Mode.HairPleat)
            {
                for (int i = 0, j = HairRect.Count - 1; i < HairRect.Count; j = i, i++)
                {
                    var p0 = HairRect[i];
                    var p1 = HairRect[j];
                    GL.Vertex2(p0);
                    GL.Vertex2(p1);
                    if (ToolShapeMirrored)
                    {
                        p0.X = halfWidth - (p0.X - halfWidth);
                        p1.X = halfWidth - (p1.X - halfWidth);
                        GL.Vertex2(p0);
                        GL.Vertex2(p1);
                    }
                }

                if (hairSelectedPoint != -1)
                {
                    GL.Color3(Color.Green);

                    var s2 = new Vector2(HairRect[hairSelectedPoint].X - 5, HairRect[hairSelectedPoint].Y - 5);
                    var s3 = new Vector2(HairRect[hairSelectedPoint].X - 5, HairRect[hairSelectedPoint].Y + 5);
                    var s1 = new Vector2(HairRect[hairSelectedPoint].X + 5, HairRect[hairSelectedPoint].Y - 5);
                    var s4 = new Vector2(HairRect[hairSelectedPoint].X + 5, HairRect[hairSelectedPoint].Y + 5);

                    GL.Vertex2(s2);
                    GL.Vertex2(s3);

                    GL.Vertex2(s1);
                    GL.Vertex2(s2);

                    GL.Vertex2(s1);
                    GL.Vertex2(s4);

                    GL.Vertex2(s4);
                    GL.Vertex2(s3);
                }
            }
            GL.End();

            GL.DepthMask(true);
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
        }

        private void DrawCustomControlPoints()
        {
            InitializeCustomControlSpritesPosition();
            for (var i = 0; i < ProgramCore.Project.BaseDots.Count; i++)
            {
                var point = ProgramCore.Project.BaseDots[i];
                if (!point.Visible)
                    continue;

                var sprite = customBasePointsSprites[i];
                if (point.Selected)
                    sprite.Color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
                else
                    sprite.Color = Vector4.One;

                customBasePointsSprites[i].Draw(false, true);
            }

        }
        private void DrawCustomPoints()
        {
            GL.PointSize(5.0f);
            GL.Begin(PrimitiveType.Points);

            foreach (var point in autodotsShapeHelper.ShapeInfo.Points)
            {
                if (!point.Visible)
                    continue;

                if (point.Selected)
                    GL.Color3(1.0f, 0.0f, 0.0f);
                else
                    GL.Color3(0.0f, 1.0f, 0.0f);

                GL.Vertex2(point.Value);
            }

            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Vertex2(CentralFacePoint);

            GL.Vertex2(LeftEyeUserCenter);
            GL.Vertex2(RightEyeUserCenter);
            GL.Vertex2(MouthUserCenter);
            GL.Vertex2(NoseUserCenter);

            GL.End();
            GL.PointSize(1.0f);
        }
        private void DrawCustomFaceRect()
        {
            var s1 = new Vector2(FaceRectTransformed.X - 0.5f, FaceRectTransformed.Y - 0.5f);
            var s2 = new Vector2(FaceRectTransformed.X + FaceRectTransformed.Width - 0.5f, FaceRectTransformed.Y - 0.5f);
            var s3 = new Vector2(FaceRectTransformed.X + FaceRectTransformed.Width - 0.5f, FaceRectTransformed.Y + FaceRectTransformed.Height - 0.5f);
            var s4 = new Vector2(FaceRectTransformed.X - 0.5f, FaceRectTransformed.Y + FaceRectTransformed.Height - 0.5f);

            GL.PointSize(5.0f);
            GL.Begin(PrimitiveType.Points);
            GL.Color3(0.0f, 0.0f, 1.0f);

            GL.Vertex2(s1);
            GL.Vertex2(s2);
            GL.Vertex2(s3);
            GL.Vertex2(s4);

            GL.End();
            GL.PointSize(1.0f);

            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Red);
            GL.Vertex2(s1);
            GL.Vertex2(s2);

            GL.Vertex2(s2);
            GL.Vertex2(s3);

            GL.Vertex2(s3);
            GL.Vertex2(s4);

            GL.Vertex2(s4);
            GL.Vertex2(s1);
            GL.End();
        }
        private void DrawCustomProfilePoints()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(0.0f, -camera.dy, 0.0f);

            GL.PointSize(5.0f);
            GL.Begin(PrimitiveType.Points);
            for (var i = 0; i < autodotsShapeHelper.ShapeProfileInfo.Points.Count; i++)
            {
                var point = autodotsShapeHelper.ShapeProfileInfo.Points[i];

                if (!point.Visible)
                    continue;

                if (point.Selected)
                    GL.Color3(1.0f, 0.0f, 0.0f);
                else
                    if (baseProfilePoints.Contains(i))
                    GL.Color3(0.0f, 0.0f, 1.0f);
                else
                    GL.Color3(0.0f, 1.0f, 0.0f);

                GL.Vertex2(-point.Value.X, point.Value.Y);
            }

            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Vertex2(CentralFacePoint);

            GL.End();
            GL.PointSize(1.0f);
        }

        private void EnableTransparent()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.DepthMask(false);
        }
        private void DisableTransparent()
        {
            GL.Disable(EnableCap.Blend);
            GL.DepthMask(true);
        }

        private void btnColorMode_Click(object sender, EventArgs e)
        {
            UseTexture = true;
        }
        private void panelBlackWhiteMode_Click(object sender, EventArgs e)
        {
            UseTexture = false;
        }

        private int backgroundTexture;
        private string bTexture;
        public string BackgroundTexture
        {
            get { return bTexture; }
            set
            {
                bTexture = value;
                backgroundTexture = GetTexture(bTexture);
            }
        }
        private void DrawBackground()
        {
            if (backgroundTexture != 0)
            {
                GL.MatrixMode(MatrixMode.Projection);
                GL.PushMatrix();
                GL.LoadIdentity();
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                GL.Enable(EnableCap.Texture2D);
                GL.DepthMask(false);
                GL.BindTexture(TextureTarget.Texture2D, backgroundTexture);
                GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);
                GL.Begin(PrimitiveType.Quads);

                GL.TexCoord2(0.0f, 1.0f);
                GL.Vertex2(-1.0f, -1.0f);
                GL.TexCoord2(1.0f, 1.0f);
                GL.Vertex2(1.0f, -1.0f);
                GL.TexCoord2(1.0f, 0.0f);
                GL.Vertex2(1.0f, 1.0f);
                GL.TexCoord2(0.0f, 0.0f);
                GL.Vertex2(-1.0f, 1.0f);

                GL.End();

                GL.DepthMask(true);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.MatrixMode(MatrixMode.Projection);
                GL.PopMatrix();
            }
        }
        public void SetDefaultBackground()
        {
            backgroundTexture = 0;
            BackgroundTexture = string.Empty;
        }

        #endregion

        #region Navigation

        public void ZoomIn()
        {
            camera.Wheel(-0.01f);
        }
        public void ZoomOut()
        {
            camera.Wheel(0.01f);
        }

        public void TurnLeft()
        {
            camera.LeftRight(-0.05);
        }
        public void TurnRight()
        {
            camera.LeftRight(0.05);
        }

        public void StepTop()
        {
            camera.dy += 1f;
        }
        public void StepBottom()
        {
            camera.dy -= 1f;
        }

        public void OrtoTop()
        {
            camera.ResetCamera(false);
        }
        public void OrtoBack()
        {
            camera.InitCamera(4.710, 500);
        }
        public void OrtoLeft()
        {
            camera.InitCamera(3.150, 500);
        }
        public void OrtoRight()
        {
            camera.InitCamera(0.0009, 500);
        }

        private void panelOrtoTop_Click(object sender, EventArgs e)
        {
            OrtoTop();
        }
        private void panelOrtoBottom_Click(object sender, EventArgs e)
        {
            OrtoBack();
        }
        private void panelOrtoLeft_Click(object sender, EventArgs e)
        {
            OrtoLeft();
        }
        private void panelOrtoRight_Click(object sender, EventArgs e)
        {
            OrtoRight();
        }
        private void panelStop_Click(object sender, EventArgs e)
        {
            workTimer.Stop();
        }

        #endregion

        #region Supported void's

        /// <summary> Update selection on mouse move</summary>
        public void MoveAPoint(int id, Vector2 newPoint)
        {
            var vector = hairRectPointB - hairRectPointA;
            var perpendicular = new Vector2(vector.Y, -vector.X);
            perpendicular.Normalize();
            vector = newPoint - hairRectPointA;
            var dist = Vector2.Dot(perpendicular, vector);
            if (id == 0 || id == 3)
                dist = -dist;
            perpendicular *= dist;

            HairRect[0] = hairRectPointA - perpendicular;
            HairRect[1] = hairRectPointA + perpendicular;
            HairRect[2] = hairRectPointB + perpendicular;
            HairRect[3] = hairRectPointB - perpendicular;
            IsShapeChanged = true;
        }

        public class BrushTextureInfo
        {
            public int Texture = 0;
            public Bitmap TextureData = null;
            public String LinkedTextureName = String.Empty;
        }

        public Dictionary<int, BrushTextureInfo> brushTextures = new Dictionary<int, BrushTextureInfo>();
        private DateTime lastPaintTime;
        public void RenderBrush()
        {
            var time = DateTime.Now;
            if ((time - lastPaintTime).Milliseconds < 40)
                return;
            lastPaintTime = time;

            if (brushTool.ResultIndices != null && brushTool.ResultIndices.Any())
            {
                var parts = new Dictionary<int, List<RenderMeshPart>>();
                foreach (var part in headMeshesController.RenderMesh.Parts)
                {
                    List<uint> indices;
                    if (brushTool.ResultIndices.TryGetValue(part.Guid, out indices))
                    {
                        part.UpdateIndexBuffer(indices);
                        List<RenderMeshPart> prts;
                        if (!parts.TryGetValue(part.Texture, out prts))
                        {
                            prts = new List<RenderMeshPart>();
                            parts.Add(part.Texture, prts);
                        }
                        prts.Add(part);
                        if ((!brushTextures.ContainsKey(part.Texture)) && textures.Values.Any(p => p.Texture == part.Texture))
                        {
                            var partTexture = textures.FirstOrDefault(p => p.Value.Texture == part.Texture);
                            var bitmap = new Bitmap(partTexture.Value.Width, partTexture.Value.Height);
                            var texture = GetTexture(bitmap);
                            if (texture == 0)
                                return;
                            brushTextures.Add(part.Texture, new BrushTextureInfo { Texture = texture, TextureData = bitmap, LinkedTextureName = partTexture.Key });
                        }
                    }
                }

                foreach (var part in parts)
                {
                    if (!brushTextures.ContainsKey(part.Key))
                        continue;
                    var texture = brushTextures[part.Key];
                    var result = RenderToTexture(texture.Texture, part.Key, texture.TextureData.Width, texture.TextureData.Height, brushShader, DrawToBrushTexture, true);
                    texture.TextureData = result;
                    SetTexture(texture.Texture, result);
                    //result.Save(String.Format("C:\\{0}_brush.png", texture.Key));
                }

                foreach (var part in parts)
                    foreach (var p in part.Value)
                        p.UpdateIndexBuffer();
            }
        }

        public void SetBrushTextures(Dictionary<int, BrushTextureInfo> bt)
        {
            brushTextures = bt;
            foreach (var b in bt)
            {
                SetTexture(b.Value.Texture, b.Value.TextureData);
            }
        }

        public bool DrawToBrushTexture(ShaderController shader, int oldTextureId, int textureId)
        {
            GL.BindTexture(TextureTarget.Texture2D, oldTextureId);
            DrawQuad(1f, 1f, 1f, 1f);

            shader.Begin();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, oldTextureId);
            shader.UpdateUniform("u_Texture", 0);
            shader.UpdateUniform("u_World", Matrix4.Identity);
            shader.UpdateUniform("u_BrushColor", brushTool.Color);
            shader.UpdateUniform("u_SphereCenter", brushTool.SphereCenter);
            shader.UpdateUniform("u_SphereRadius", brushTool.Radius);

            headMeshesController.RenderMesh.DrawToTexture(headMeshesController.RenderMesh.Parts.Where(p => p.Texture == textureId));

            shader.End();

            return true;
        }

        public Bitmap RenderToTexture(int oldTextureId, int textureId)
        {
            var textureWidth = 0;
            var textureHeight = 0;
            var texPath = GetTexturePath(oldTextureId);
            using (var img = new Bitmap(texPath))
            {
                textureWidth = img.Width;
                textureHeight = img.Height;
            }
            return RenderToTexture(oldTextureId, textureId, textureWidth, textureHeight, blendShader, DrawToTexture);
        }

        private bool DrawToTexture(ShaderController shader, int oldTextureId, int textureId)
        {
            //GL.BindTexture(TextureTarget.Texture2D, oldTextureId);
            DrawQuad(ProgramCore.Project.FaceColor.X, ProgramCore.Project.FaceColor.Y, ProgramCore.Project.FaceColor.Z, 1f);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);

            shader.Begin();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, HeadTextureId);
            shader.UpdateUniform("u_Texture", 0);
            shader.UpdateUniform("u_BlendStartDepth", -0.5f);
            shader.UpdateUniform("u_BlendDepth", 4f);

            headMeshesController.RenderMesh.DrawToTexture(headMeshesController.RenderMesh.Parts.Where(p => p.Texture == textureId));

            shader.End();
            GL.Disable(EnableCap.Blend);
            return true;
        }

        private void DrawQuad(float r, float g, float b, float a)
        {
            GL.Color4(r, g, b, a);
            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex2(-1.0f, -1.0f);
            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex2(1.0f, -1.0f);
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex2(1.0f, 1.0f);
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex2(-1.0f, 1.0f);

            GL.End();
        }

        public Bitmap RenderToTexture(int oldTextureId, int textureId, int textureWidth, int textureHeight, ShaderController shader,
            Func<ShaderController, int, int, bool> renderFunc, bool useAlpha = false)
        {
            graphicsContext.MakeCurrent(windowInfo);
            renderPanel.Size = new Size(textureWidth, textureHeight);
            GL.Viewport(0, 0, textureWidth, textureHeight);

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Enable(EnableCap.Texture2D);
            GL.DepthMask(false);

            renderFunc(shader, oldTextureId, textureId);

            GL.DepthMask(true);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();

            var result = GrabScreenshot(String.Empty, textureWidth, textureHeight, useAlpha);
            glControl.Context.MakeCurrent(glControl.WindowInfo);
            SetupViewport(glControl);
            return result;
        }

        public Bitmap GrabScreenshot(string filePath, int width, int height, bool useAlpha = false)
        {
            var bmp = new Bitmap(width, height);
            var rect = new Rectangle(0, 0, width, height);
            var data = bmp.LockBits(rect, ImageLockMode.WriteOnly, useAlpha ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, width, height, useAlpha ? OpenTK.Graphics.OpenGL.PixelFormat.Bgra : OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            GL.Finish();
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            if (!string.IsNullOrEmpty(filePath))
                bmp.Save(filePath, ImageFormat.Jpeg);
            return bmp;
        }

        public void SetCustomProfileSetup()
        {
            OrtoRight();
            UpdateCustomPointsFaceRect(true, true);       // пересчитываем прямоугольник лица
            autodotsShapeHelper.UpdateProfileLines();
            Mode = Mode.SetCustomProfilePoints;
        }

        /// <summary> Get or load texture by filename </summary>
        /// <param name="textureName">Path to texture</param>
        /// <returns>Texture id</returns>
        public int GetTexture(String textureName)
        {
            var textureId = 0;
            if (textureName != String.Empty && File.Exists(textureName))
            {

                if (textures.ContainsKey(textureName))
                    return textures[textureName].Texture;

                Bitmap bitmap;
                using (var ms = new MemoryStream(File.ReadAllBytes(textureName)))
                    bitmap = (Bitmap)Image.FromStream(ms);

                textureId = GetTexture(bitmap);
                textures.Add(textureName, new TextureInfo
                {
                    Texture = textureId,
                    Width = bitmap.Width,
                    Height = bitmap.Height
                });
            }
            return textureId;
        }

        public int GetTexture(Bitmap bitmap)
        {
            try
            {
                int textureId;
                GL.GenTextures(1, out textureId);

                SetTexture(textureId, bitmap);
                return textureId;
            }
            catch
            {
                return 0;
            }
        }

        public string GetTexturePath(int id)
        {
            if (id == 0)
                return string.Empty;
            foreach (var t in textures)
                if (t.Value.Texture == id)
                    return t.Key;
            return string.Empty;
        }

        public void SetTexture(int textureId, Bitmap bitmap)
        {
            try
            {
                GL.BindTexture(TextureTarget.Texture2D, textureId);

                var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                bitmap.UnlockBits(data);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            }
            finally
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
        }

        private void timerExecute()
        {
            switch (TimerMode)
            {
                case Mode.TimerTurnLeft:
                    TurnLeft();
                    break;
                case Mode.TimerTurnRight:
                    TurnRight();
                    break;
                case Mode.TimerStepTop:
                    StepTop();
                    break;
                case Mode.TimerStepBottom:
                    StepBottom();
                    break;
                case Mode.TimerZoomIn:
                    ZoomIn();
                    break;
                case Mode.TimerZoomOut:
                    ZoomOut();
                    break;
            }
        }
        private void workTimer_Tick(object sender, EventArgs e)
        {
            timerExecute();
        }
        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            Render();
        }

        public void DeleteSelectedTexture()
        {
            foreach (var selectedMesh in pickingController.SelectedMeshes)
            {
                selectedMesh.Material.DiffuseTextureMap = string.Empty;
                selectedMesh.Material.TransparentTextureMap = string.Empty;
            }
        }
        internal void DeleteSelectedAccessory()
        {
            pickingController.SelectedMeshes.BeginInit();
            try
            {
                var title = string.Empty;
                for (var i = pickingController.SelectedMeshes.Count - 1; i >= 0; i--)
                {
                    var selectedMesh = pickingController.SelectedMeshes[i];
                    if (selectedMesh.meshType != MeshType.Accessory)
                        continue;

                    pickingController.AccesoryMeshes.Remove(selectedMesh);

                    title = selectedMesh.Title;
                    selectedMesh.Destroy();
                    pickingController.SelectedMeshes.RemoveAt(i);
                }
                RemoveMeshFromPartLibrary(title);
            }
            finally
            {
                pickingController.SelectedMeshes.EndInit(true);
            }
        }
        internal void DeleteSelectedHair()
        {
            pickingController.SelectedMeshes.BeginInit();
            try
            {
                var historyItems = new HistoryMeshes(pickingController.SelectedMeshes.Where(x => x.meshType == MeshType.Hair));
                historyController.Add(historyItems);

                var title = string.Empty;
                for (var i = pickingController.SelectedMeshes.Count - 1; i >= 0; i--)
                {
                    var selectedMesh = pickingController.SelectedMeshes[i];
                    if (selectedMesh.meshType != MeshType.Hair)
                        continue;

                    pickingController.HairMeshes.Remove(selectedMesh);

                    title = selectedMesh.Title;
                    selectedMesh.Destroy();
                    pickingController.SelectedMeshes.RemoveAt(i);
                }
                RemoveMeshFromPartLibrary(title);
            }
            finally
            {
                pickingController.SelectedMeshes.EndInit(true);
            }
        }

        private void RemoveMeshFromPartLibrary(string groupTitle, bool isVisible = false)
        {
            if (PartsLibraryMeshes.ContainsKey(groupTitle))
            {
                foreach (var mesh in PartsLibraryMeshes[groupTitle])
                    mesh.IsVisible = isVisible;

                PartsLibraryMeshes.Remove(groupTitle);
                ProgramCore.MainForm.frmParts.UpdateList();
            }
        }

        internal void SaveSelectedHairToPartsLibrary()
        {
            if (pickingController.SelectedMeshes.Count == 0)
                return;
            if (pickingController.SelectedMeshes.All(x => x.meshType != MeshType.Hair))
                return;

            pickingController.SelectedMeshes[0].AttachMeshes(pickingController.SelectedMeshes);
            var firstMesh = pickingController.SelectedMeshes.First();
            pickingController.SelectedMeshes.RemoveAt(0);
            DeleteSelectedHair();
            pickingController.SelectedMeshes.Add(firstMesh);

            using (var sfd = new SaveFileDialogEx("Save part", "OBJ files|*.obj"))
            {
                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                var meshes = new DynamicRenderMeshes();
                foreach (var mesh in pickingController.SelectedMeshes)
                    meshes.Add(mesh);

                pickingController.SelectedMeshes.Clear();
                ObjSaver.SaveObjFile(sfd.FileName, meshes, MeshType.Hair, headMeshesController.RenderMesh.RealScale);

                var fileName = Path.GetFileNameWithoutExtension(sfd.FileName);
                var title = fileName;
                var index = 0;
                while (PartsLibraryMeshes.ContainsKey(title))      // prevent duplicated names, because it's not occure save correct to obj
                {
                    title = fileName + "_" + index;
                    ++index;
                }

                if (!PartsLibraryMeshes.ContainsKey(title))
                    PartsLibraryMeshes.Add(title, new DynamicRenderMeshes());

                for (var i = 0; i < meshes.Count; i++)
                {
                    var mesh = meshes[i];
                    mesh.Title = title + "_" + i;
                    if (!PartsLibraryMeshes[title].Contains(mesh))
                        PartsLibraryMeshes[title].Add(mesh);
                }

                ProgramCore.MainForm.frmParts.UpdateList();
            }
        }
        internal void SaveHeadToFile()
        {
            using (var sfd = new SaveFileDialogEx("Save part", "OBJ files|*.obj"))          // не думаю, что стоит добавлять голову в библиотеку. смысл?
            {
                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                SaveHead(sfd.FileName);
            }
        }
        public void SaveHead(string path, bool saveBrushesToTexture = false)
        {
            try
            {
                if (ProgramCore.Project.AutodotsUsed)
                    SaveBlendingTextures();

                ObjSaver.SaveObjFile(path, headMeshesController.RenderMesh, MeshType.Hair, pickingController.ObjExport, saveBrushesToTexture);

                #region Сохраняем отраженную текстуру

                if (ProgramCore.Project.TextureFlip != FlipType.None && !ProgramCore.PluginMode)
                {
                    var newTexturePath = Path.Combine(Path.GetDirectoryName(path), "Textures");
                    var newTextureFullPath = Path.Combine(newTexturePath, Path.GetFileName(ProgramCore.Project.FrontImagePath));

                    FolderEx.CreateDirectory(new DirectoryInfo(newTexturePath));

                    switch (ProgramCore.Project.TextureFlip)
                    {
                        case FlipType.LeftToRight:
                            reflectedLeft.Save(newTextureFullPath, ImageFormat.Jpeg);
                            break;
                        case FlipType.RightToLeft:
                            reflectedRight.Save(newTextureFullPath, ImageFormat.Jpeg);
                            break;
                    }

                }
            }
            finally
            {
                SaveSmoothedTextures();
            }
            #endregion
        }

        private void keyTimer_Tick(object sender, EventArgs e)
        {
            timerExecute();
        }

        private bool inStageMode = false;
        public bool InStageMode
        {
            set
            {
                if (value != inStageMode)
                {
                    inStageMode = value;
                    if (inStageMode)
                        UpdateCameraPosition(-0.54f);
                }
            }
            get { return inStageMode; }
        }

        public void StagesActivate(bool isChanged = false)
        {
            InStageMode = true;
            PlayAnimation = true;
            pickingController.UpdateSelectedFace(int.MinValue, int.MinValue);

            ProgramCore.MainForm.frmParts.Hide();
            ProgramCore.MainForm.frmMaterial.Hide();
            ProgramCore.MainForm.frmAccessories.Hide();
            ProgramCore.MainForm.frmStyles.Hide();

            foreach (var mesh in animationController.BodyMeshes)
                mesh.BeginAnimate();

            foreach (var mesh in pickingController.HairMeshes)
            {
                if (isChanged)
                    mesh.IsChanged = true;
                mesh.BeginAnimate();
            }
            foreach (var mesh in pickingController.AccesoryMeshes)
            {
                if (isChanged)
                    mesh.IsChanged = true;
                mesh.BeginAnimate();
            }

            headMeshesController.RenderMesh.BeginMorph();
        }
        public void StagesDeactivate(int pos)
        {
            headMeshesController.RenderMesh.EndMorph();
            InStageMode = false;
            if (pos != -1)
            {
                switch (pos)
                {
                    case 0:
                        ProgramCore.MainForm.panelMenuCut_Click(ProgramCore.MainForm, EventArgs.Empty);
                        break;
                    case 1:
                        ProgramCore.MainForm.panelMenuMaterials_Click(ProgramCore.MainForm, EventArgs.Empty);
                        break;
                    case 2:
                        ProgramCore.MainForm.panelMenuAccessories_Click(ProgramCore.MainForm, EventArgs.Empty);
                        break;
                    case 3:
                        ProgramCore.MainForm.panelMenuStyle_Click(ProgramCore.MainForm, EventArgs.Empty);
                        break;
                }

            }

            if (ProgramCore.MainForm.frmStyles != null && pos != 3)
                ProgramCore.MainForm.frmStyles.Hide();
            if (ProgramCore.MainForm.frmStages != null)
                ProgramCore.MainForm.frmStages.Hide();
            if (ProgramCore.MainForm.frmMaterial != null && pos != 1)
                ProgramCore.MainForm.frmMaterial.Hide();
            if (ProgramCore.MainForm.frmFreeHand != null)
                ProgramCore.MainForm.frmFreeHand.Hide();
            PlayAnimation = false;
            foreach (var mesh in animationController.BodyMeshes)
                mesh.EndAnimate();
            foreach (var mesh in pickingController.HairMeshes)
                mesh.EndAnimate();
            foreach (var mesh in pickingController.AccesoryMeshes)
                mesh.EndAnimate();
        }

        public void EndSlicing()
        {
            if (pickingController.SelectedMeshes.Count > 0 && pickingController.SelectedMeshes.All(x => x.meshType == MeshType.Hair))
            {
                if (!sliceController.IsEmpty())
                {
                    sliceController.EndSlice(Width, Height, camera);
                    var historyItem = new HistoryMeshes();
                    foreach (var mesh in pickingController.SelectedMeshes)
                    {
                        var historyMesh = historyItem.AddMesh(mesh);
                        historyMesh.ToDelete.Add(mesh.Id);

                        sliceController.Slice(mesh, camera);

                        var color = pickingController.SelectedColor.ContainsKey(mesh) ? pickingController.SelectedColor[mesh] : Vector4.Zero;
                        for (var i = 0; i < sliceController.ResultMeshes.Count; i++)
                        {
                            var resMesh = sliceController.ResultMeshes[i];
                            resMesh.Title = mesh.Title + "_" + i;
                            resMesh.Material.DiffuseColor = color;
                            pickingController.HairMeshes.Add(resMesh);
                            historyMesh.ToDelete.Add(resMesh.Id);
                        }
                    }
                    historyController.Add(historyItem);
                }

                sliceController.BeginSlice(ToolsMode == ToolsMode.HairArc);
            }
        }

        internal void ResetModeTools()
        {
            accessoryRotateCenterCirclePoint = Vector2.Zero;
            accessoryRotateCircleMousePoint = Vector2.Zero;
            accessoryRotateMousePoint = Vector2.Zero;
            accessoryRotateCenterPoint = Vector3.Zero;
            accessoryRotateRadius = 0f;

            hairRectPointA = Vector2.Zero;
            hairRectPointB = Vector2.Zero;
            HairRect.Clear();

            LassoPoints.Clear();

            sliceController.BeginSlice(ToolsMode == ToolsMode.HairArc);
        }

        public void SelectHairByLasso()
        {
            pickingController.SelectedMeshes.BeginUpdate();
            try
            {
                pickingController.SelectedMeshes.Clear();
                var invProjection = camera.ProjectMatrix.Inverted();
                var lassoPoints = new List<Vector2>();
                foreach (var p in LassoPoints)
                    lassoPoints.Add(SliceController.UnprojectPoint(p, camera.WindowWidth, camera.WindowHeight, invProjection));
                foreach (var mesh in pickingController.HairMeshes)
                {
                    if (mesh.IsSelected(lassoPoints, camera))
                        pickingController.SelectedMeshes.Add(mesh);
                }
            }
            finally
            {
                pickingController.SelectedMeshes.EndUpdate();
            }
        }

        public void UpdateProfileRectangle()
        {
            ProfileFaceRect = Rectangle.Empty;
            if (!ProgramCore.MainForm.HeadProfile || headController.ShapeDots.Count == 0)
                return;

            var pointUp = ProgramCore.MainForm.ctrlTemplateImage.profileControlPoints[0];
            var pointBottom = ProgramCore.MainForm.ctrlTemplateImage.profileControlPoints[3];

            var width = Math.Max(pointUp.Value.X, pointBottom.Value.X) - Math.Min(pointUp.Value.X, pointBottom.Value.X);
            var height = Math.Max(pointUp.Value.Y, pointBottom.Value.Y) - Math.Min(pointUp.Value.Y, pointBottom.Value.Y);

            var center = (pointUp.Value + pointBottom.Value) * 0.5f;

            ProfileFaceRect = new RectangleF(center.X - width * 0.5f, center.Y - height * 0.5f, width, height);
        }

        public void ApplySmoothedTextures()
        {
            foreach (var smoothTex in SmoothedTextures)
            {
                if (smoothTex.Key == 0)
                    continue;

                var bitmap = RenderToTexture(smoothTex.Key, smoothTex.Value);
                SetTexture(smoothTex.Value, bitmap);
            }
        }

        public void SaveBlendingTextures()
        {
            var newFolderPath = Path.Combine(ProgramCore.Project.ProjectPath, "SmoothedModelTextures");
            var di = new DirectoryInfo(newFolderPath);
            if (!di.Exists)
                di.Create();

            foreach (var smoothTex in SmoothedTextures.Where(s => s.Key != 0))
            {
                var oldTexturePath = GetTexturePath(smoothTex.Key);
                var newImagePath = Path.Combine(newFolderPath, Path.GetFileNameWithoutExtension(oldTexturePath) + "_smoothed" + Path.GetExtension(oldTexturePath));
                var bitmap = RenderToTexture(smoothTex.Key, smoothTex.Value);

                var brush = brushTextures.Select(b => b.Value).FirstOrDefault(b => b.LinkedTextureName.Equals(newImagePath));
                if (brush != null)
                {
                    using (var grfx = Graphics.FromImage(bitmap))
                    {
                        grfx.DrawImage(brush.TextureData, 0, 0);
                    }
                }

                bitmap.Save(newImagePath, ImageFormat.Jpeg);
            }
        }

        public void SaveBrushTextures()
        {
            var newFolderPath = Path.Combine(ProgramCore.Project.ProjectPath, "SmoothedModelTextures");
            var di = new DirectoryInfo(newFolderPath);
            if (!di.Exists)
                di.Create();

            foreach (var texture in brushTextures)
            {
                var texturePath = texture.Value.LinkedTextureName.Contains("_smoothed.") ?
                    texture.Value.LinkedTextureName.Replace("_smoothed" + Path.GetExtension(texture.Value.LinkedTextureName), String.Empty) :
                    texture.Value.LinkedTextureName;

                texturePath = Path.Combine(newFolderPath, Path.GetFileNameWithoutExtension(texturePath) + "_brush.png");
                texture.Value.TextureData.Save(texturePath, ImageFormat.Png);
            }
        }

        public void SaveSmoothedTextures()
        {
            var newFolderPath = Path.Combine(ProgramCore.Project.ProjectPath, "SmoothedModelTextures");
            var di = new DirectoryInfo(newFolderPath);
            if (!di.Exists)
                di.Create();

            foreach (var smoothTex in SmoothedTextures.Where(s => s.Key != 0))
            {
                var oldTexturePath = GetTexturePath(smoothTex.Key);
                var newImagePath = Path.Combine(newFolderPath, Path.GetFileNameWithoutExtension(oldTexturePath) + "_smoothed" + Path.GetExtension(oldTexturePath));
                using (var bitmap = RenderToTexture(smoothTex.Key, smoothTex.Value))
                    bitmap.Save(newImagePath, ImageFormat.Jpeg);
            }
        }

        public void DoMorth(float? k = null)
        {
            var morphs = new List<Dictionary<Guid, PartMorphInfo>>();
            if (OldMorphing != null)
                morphs.Add(OldMorphing);
            if (FatMorphing != null)
                morphs.Add(FatMorphing);
            //if (PoseMorphing != null)
            //    morphs.Add(PoseMorphing);
            if (k != null)
                headMeshesController.RenderMesh.EndMorph();

            Morphing.Morph(morphs, headMeshesController.RenderMesh);

            if (k != null)
            {
                headMeshesController.RenderMesh.BeginMorph();
                headMeshesController.RenderMesh.DoMorph(k.Value);
            }
        }

        /// <summary> Пересчет прямоугольника лица для таскания точек настройки головы (при выборе режима загрузки custom) </summary>
        private void UpdateCustomPointsFaceRect(bool needUpdateRect, bool isProfile)
        {
            var source = isProfile ? autodotsShapeHelper.ShapeProfileInfo : autodotsShapeHelper.ShapeInfo;
            var minX = source.Points.Min(point => point.Value.X);
            var maxX = source.Points.Max(point => point.Value.X);
            var minY = source.Points.Min(point => point.Value.Y);
            var maxY = source.Points.Max(point => point.Value.Y);

            if (isProfile)
            {
                minX *= -1;
                maxX *= -1;
            }

            CentralFacePoint = new Vector2(minX + (maxX - minX) * 0.5f, minY + (maxY - minY) / 1.5f);

            if (needUpdateRect)
                FaceRectTransformed = new Rectangle((int)minX, (int)minY, (int)(maxX - minX), (int)(maxY - minY));
        }
        private void UpdateCustomFaceParts(bool onlySelected, bool updateRect)
        {
            var center = UpdateUserCenterPositions(autodotsShapeHelper.GetLeftEyeIndexes(), onlySelected);  // Left eye
            if (center != Vector2.Zero)
                LeftEyeUserCenter = center;

            center = UpdateUserCenterPositions(autodotsShapeHelper.GetRightEyeIndexes(), onlySelected);  // Right eye
            if (center != Vector2.Zero)
                RightEyeUserCenter = center;

            center = UpdateUserCenterPositions(autodotsShapeHelper.GetMouthIndexes(), onlySelected);  // Mouth
            if (center != Vector2.Zero)
                MouthUserCenter = center;

            center = UpdateUserCenterPositions(autodotsShapeHelper.GetNoseIndexes(), onlySelected);  // Nose
            if (center != Vector2.Zero)
                NoseUserCenter = center;

            #region Определяем прямоугольник, охватывающий все автоточки

            if (updateRect)
                UpdateCustomPointsFaceRect(updateRect, false);

            #endregion
        }
        private Vector2 UpdateUserCenterPositions(IEnumerable<int> indexes, bool onlySelected)
        {
            var hasSelected = false;
            var dots = new List<HeadPoint>();
            foreach (var index in indexes)
            {
                var dot = autodotsShapeHelper.ShapeInfo.Points[index];
                dots.Add(dot);

                if (!onlySelected || dot.Selected)
                    hasSelected = true;
            }

            if (!hasSelected)
                return Vector2.Zero;

            var minX = dots.Min(point => point.Value.X);
            var maxX = dots.Max(point => point.Value.X);
            var minY = dots.Min(point => point.Value.Y);
            var maxY = dots.Max(point => point.Value.Y);

            return new Vector2((maxX + minX) * 0.5f, (maxY + minY) * 0.5f);
        }


        #region Отражение

        /// <summary> Определить целиком отраженные слева и справа изображения </summary>
        public void CalcReflectedBitmaps()
        {
            var originalImg = ProgramCore.Project.FrontImage;

            var indicies = headController.GetFaceIndexes();
            var faceDots = headController.GetSpecialAutodots(indicies);

            var minX = faceDots.Min(point => point.ValueMirrored.X) * originalImg.Width;
            var maxX = faceDots.Max(point => point.ValueMirrored.X) * originalImg.Width;

            var faceCenter = minX + (maxX - minX) * 0.5f;

            var width = Math.Min(faceCenter, originalImg.Width - faceCenter);

            var leftRect = new Rectangle((int)(faceCenter - width), 0, (int)width, originalImg.Height);
            var rightRect = new Rectangle((int)faceCenter, 0, (int)width, originalImg.Height);

            var leftCroppedImage = ImageEx.Crop(originalImg, leftRect);            // получаем левый и правый кусочки изображений
            var rightCroppedImage = ImageEx.Crop(originalImg, rightRect);

            leftCroppedImage.RotateFlip(RotateFlipType.RotateNoneFlipX);         // отражаем по горизонту
            rightCroppedImage.RotateFlip(RotateFlipType.RotateNoneFlipX);

            var originalFlipLeftImg = new Bitmap(originalImg);                                                                   // и встраиваем обратно уже отраженным
            var originalFlipRightImg = new Bitmap(originalImg);
            ImageEx.CopyRegionIntoImage(leftCroppedImage, ref originalFlipLeftImg, rightRect);
            ImageEx.CopyRegionIntoImage(rightCroppedImage, ref originalFlipRightImg, leftRect);

            reflectedLeft = originalFlipLeftImg;
            reflectedRight = originalFlipRightImg;
        }

        /// <summary> Отражение слева направо </summary>
        /// <param name="trackPos">Положение бегунка</param>
        public void FlipLeft(bool flip)
        {
            SetTexture(HeadTextureId, flip ? reflectedLeft : headTexture);
            ApplySmoothedTextures();
        }

        /// <summary> Отражение справа налево </summary>
        /// <param name="trackPos">Положение бегунка</param>
        public void FlipRight(bool flip)
        {
            SetTexture(HeadTextureId, flip ? reflectedRight : headTexture);
            ApplySmoothedTextures();
        }

        #endregion

        #endregion

    }

    #region enum's

    public enum Mode
    {
        None = 0,
        AccessoryRotateSetCircle,
        AccessoryRotate,
        HairStretchSetRect,
        HairStretch,
        HairShapeSetRect,
        HairShape,
        HairPleatSetRect,
        HairPleat,
        HairCut,
        LassoStart,
        LassoActive,
        TimerTurnLeft,
        TimerTurnRight,
        TimerStepTop,
        TimerStepBottom,
        TimerZoomIn,
        TimerZoomOut,
        HeadLine,
        //HeadShapedots,
        HeadShapeFirstTime,
        HeadShape,
        HeadAutodots,
        HeadAutodotsFirstTime,
        HeadAutodotsLassoStart,
        HeadAutodotsLassoActive,
        // HeadShapedotsLassoStart,
        //  HeadShapedotsLassoActive,
        HeadFlipLeft,           // отражение слева направо
        HeadFlipRight,
        SetCustomControlPoints,     // произвольная модель. этап 1. расставить 4 главные опорные точки
        SetCustomPoints,            // расставить остальные точки во фронте. этап 2
        SetCustomProfilePoints,      // расставить остальные точки в профиле. этап 3
        ColorPicker,   // пипетка
        Brush
    }

    public enum ScaleMode
    {
        Rotate,
        Zoom,
        Move,
        None
    }

    public enum ToolsMode
    {
        HairLine,
        HairPolyLine,
        HairArc,
    }


    #endregion
}
