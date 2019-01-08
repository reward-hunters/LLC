using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using RH.Core.Helpers;
using RH.Core.IO;
using RH.Core.Render.Controllers;
using RH.Core.Render.Helpers;
using RH.Core.Render.Meshes;
using RH.Core.Render.Obj;
using RH.MeshUtils.Data;
using RH.MeshUtils;

namespace RH.Core
{
    /// <summary> HeadShop project </summary>
    public class Project
    {
        /// <summary> Название проекта </summary>
        public string ProjectName;
        /// <summary> Путь к проекта </summary>
        public string ProjectPath;

        public string HeadModelPath;
        public bool IsOpenSmile = false;

        /// <summary> Распознанные ключевые точки с изображения (глаза-нос и т.п., нормированные) </summary>
        public List<Vector3> FacialFeatures;
        /// <summary> Тоже самое, но не нормированные, требуются для определения угла поворота. </summary>
        public List<Vector2> ImageRealPoints;

        /// <summary> Относительный путь до левой картинке (шаблона)</summary>
        private string frontImagePath = string.Empty;
        public string FrontImagePath
        {
            get
            {
                return frontImagePath;
            }
            set
            {
                frontImagePath = value;
                if (!string.IsNullOrEmpty(frontImagePath))
                {
                    var fileName = Path.Combine(ProjectPath, frontImagePath);
                    var fi = new FileInfo(fileName);
                    if (fi.Exists)
                    {
                        FrontImage = new Bitmap(fi.FullName);
                    }
                    else
                        frontImagePath = string.Empty;
                }

            }
        }
        public Bitmap FrontImage;

        /// <summary> Путь до картинки исходника. Так как в frontImagePath мы храним ссылку на скопированную и с  измененным размером имагу. </summary>
        public string RealTemplateImage;

        public Bitmap ProfileImage;

        /// <summary> Было ли назначение автоточек </summary>
        public bool AutodotsUsed;
        /// <summary> Отражение головы включено </summary>
        public bool MirrorUsed;

        public FlipType TextureFlip = FlipType.None;        // отражено ли или нет
        public FlipType ShapeFlip = FlipType.None;

        public float AgeCoefficient = 0;            // коэффициенты features для сохранения возраста и толщины.
        public float FatCoefficient = 0;
        public float SmileCoefficient = 0;

        public float MorphingScale = float.NaN;

        public Vector2 ProfileEyeLocation = Vector2.Zero;
        public Vector2 ProfileMouthLocation = Vector2.Zero;

        /// <summary> Размер текстур, выбранный при создании проекта </summary>
        public int TextureSize;

        #region Все нужное, для работы с моделью головы

        public HeadPoints<HeadPoint> BaseDots; // опорные точки для произвольной башки. по дефолту - женские точки 
        public HeadPoints<HeadPoint> ProfileBaseDots;

        public bool CustomHeadNeedProfileSetup; // в случае произвольной башки, нужно ли вызывать настройку точек или нет

        #endregion

        #region Face recognized position

        public double RotatedAngle = 0;

        /// <summary> Прямоугольник лица, рассчитанный по точкам глаз и рта, определенных распознаванием. Относительные координаты.</summary>
        public RectangleF FaceRectRelative;
        public RectangleF nextHeadRectF = new RectangleF();
        public Vector2 TopPoint = Vector2.Zero;

        /// <summary> Центр рта, опредееленный распознаванием лица. Относительные координаты </summary>
        private Vector2 mouthCenter;
        public Vector2 MouthCenter
        {
            get
            {
                return mouthCenter;
            }
            set
            {
                mouthCenter = value;
                if (MouthUserCenter == Vector2.Zero)
                    MouthUserCenter = mouthCenter;

                //RecalcRect();
            }
        }

        /// <summary> Центр левого глаза, опредееленный распознаванием лица. Относительные координаты </summary>
        private Vector2 leftEyeCenter;
        public Vector2 LeftEyeCenter
        {
            get
            {
                return leftEyeCenter;
            }
            set
            {
                leftEyeCenter = value;
                if (LeftEyeUserCenter == Vector2.Zero)
                    LeftEyeUserCenter = leftEyeCenter;

                //RecalcRect();
            }
        }

        public Vector4 FaceColor { get; set; }

        public readonly RenderMainHelper RenderMainHelper = new RenderMainHelper();

        /// <summary> Центр правого глаза, опредееленный распознаванием лица. Относительные координаты </summary>
        private Vector2 rightEyeCenter;
        public Vector2 RightEyeCenter
        {
            get
            {
                return rightEyeCenter;
            }
            set
            {
                rightEyeCenter = value;
                if (RightEyeUserCenter == Vector2.Zero)
                    RightEyeUserCenter = rightEyeCenter;

                //RecalcRect();
            }
        }

        public List<Vector3> DetectedTopPoints = new List<Vector3>();
        public List<Vector3> DetectedBottomPoints = new List<Vector3>();
        public List<Vector3> DetectedNosePoints = new List<Vector3>();
        public List<Vector3> DetectedLipsPoints = new List<Vector3>();
        public List<Vector3> DetectedLeftEyePoints = new List<Vector3>();
        public List<Vector3> DetectedRightEyePoints = new List<Vector3>();

        #endregion

        #region User face position

        /// <summary> Поцизии рта и глаз, определенные пользователем в автоточках (учитывая изменения положения точек). Относительные координаты </summary>
        public Vector2 MouthUserCenter;
        public Vector2 LeftEyeUserCenter;
        public Vector2 RightEyeUserCenter;
        public Vector2 NoseUserCenter = new Vector2(0, 0);       // так мы его никак не определим. ждем первых автоточек, Тогда будет пересчет

        #endregion


        /// <summary> Тип лица (мужское, женское, ребенок)</summary>
        public ManType ManType;
        /// <summary> Тип модели, используемой по умолчанию </summary>
        public GenesisType GenesisType = GenesisType.Genesis2;

        /// <summary> используются сглаженные текстуры </summary>
        public bool SmoothedTextures;

        /// <summary> Создание нового проекта </summary>
        /// <param name="projectName"></param>
        /// <param name="projectPath"></param>
        /// <param name="templateImageName"></param>
        /// <param name="manType"></param>
        /// <param name="headModelPath">Указываем путь до модели головы (в случае если выбрали import OBJ). Иначе - пустая строка</param>
        /// <param name="needCopy"></param>
        public Project(string projectName, string projectPath, string templateImageName, GenesisType genesisType, ManType manType, string headModelPath, bool needCopy, int selectedSize, bool isOpenSmile)
        {
            ProjectName = projectName;
            ProjectPath = projectPath;
            TextureSize = selectedSize;

            ManType = manType;
            IsOpenSmile = isOpenSmile;
            GenesisType = genesisType;
            switch (manType)
            {
                case ManType.Male:
                case ManType.Female:        // если это обычные модели - копируем их из папки с прогой - в папку с проектом
                case ManType.Child:
                    if (!ProgramCore.PluginMode)        //тогда хед модел пас оставляем какой был! пиздец важно!
                    {                                   // я хз почему это важно, но сейчас получается что при загрузке из Даз всегда берется одна модель, независимо от того, что выбрал пользователь (13.02.2018)
#if WEB_APP
                        headModelPath = "ftp://108.167.164.209/public_html/printahead.online/PrintAhead_DefaultModels/" +  manType.GetObjPath();
                        headModelPath = headModelPath.Replace(@"\", "/");
#else
                        headModelPath = Path.Combine(Application.StartupPath, "Models", "Model", GenesisType.GetGenesisPath(), manType.GetObjPath(isOpenSmile));
#endif
                    }

                    break;
                case ManType.Custom:
                    {
                        BaseDots = new HeadPoints<HeadPoint>();
                        foreach (var vector in HeadController.GetBaseDots(ManType.Female))
                            BaseDots.Add(new HeadPoint(vector));

                        ProfileBaseDots = new HeadPoints<HeadPoint>();
                        foreach (var vector in HeadController.GetProfileBaseDots(ManType.Female))
                            ProfileBaseDots.Add(new HeadPoint(vector));
                    }
                    break;
            }

            if (needCopy)
            {
                try
                {
                    var di = new DirectoryInfo(projectPath);
                    if (!di.Exists)
                        di.Create();

                    if (!string.IsNullOrEmpty(templateImageName))
                    {
                        var fi = new FileInfo(templateImageName);
                        if (fi.Exists)
                        {
                            var newImagePath = Path.Combine(projectPath, fi.Name);
                            File.Copy(templateImageName, newImagePath, true);
                            FrontImagePath = fi.Name;
                        }
                    }

                    #region Копируем модель

                    var directoryPath = Path.Combine(ProjectPath, "Model");
                    FolderEx.CreateDirectory(directoryPath);
                    var oldFileName = Path.GetFileNameWithoutExtension(headModelPath);
                    var newFileName = oldFileName;
                    var filePath = Path.Combine(directoryPath, newFileName + ".obj");

                    File.Copy(headModelPath, filePath, true); // сама модель
                    HeadModelPath = filePath;

                    #region Обрабатываем mtl файл и папку с текстурами

                    var mtl = oldFileName + ".mtl";
                    using (var ms = new StreamReader(headModelPath))
                    {
                        for (var i = 0; i < 10; i++)
                        {
                            if (ms.EndOfStream)
                                break;
                            var line = ms.ReadLine();
                            if (line.ToLower().Contains("mtllib"))
                            // ищем ссылку в obj файле на mtl файл (у них могут быть разные названия, но всегда в одной папке
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

                    ObjLoader.CopyMtl(mtl, mtl, Path.GetDirectoryName(headModelPath), "", directoryPath, selectedSize);

                    #endregion

                    #endregion
                }
                catch
                {
                    FrontImagePath = templateImageName;
                    HeadModelPath = headModelPath;
                }
            }
            else
            {
                FrontImagePath = templateImageName;
                HeadModelPath = headModelPath;
            }
        }

        public void LoadMeshes()
        {
#if WEB_APP
            var modelPath = Path.Combine(ProjectPath, "OBJ", "hair.obj");

            RenderMainHelper.LoadModel(modelPath, true, ManType, MeshType.Hair);

            var acDirPath = Path.GetDirectoryName(modelPath);
            var acName = Path.GetFileNameWithoutExtension(modelPath) + "_accessory.obj";
            var accessoryPath = Path.Combine(acDirPath, acName);
            if (File.Exists(accessoryPath))
                RenderMainHelper.LoadModel(accessoryPath, false, ManType, MeshType.Accessory);

#else
            var modelPath = Path.Combine(ProjectPath, "OBJ", "hair.obj");
            ProgramCore.MainForm.ctrlRenderControl.LoadModel(modelPath, true, GenesisType, ManType, MeshType.Hair, false);

            var acDirPath = Path.GetDirectoryName(modelPath);
            var acName = Path.GetFileNameWithoutExtension(modelPath) + "_accessory.obj";
            var accessoryPath = Path.Combine(acDirPath, acName);
            if (File.Exists(accessoryPath))
                ProgramCore.MainForm.ctrlRenderControl.LoadModel(accessoryPath, false, GenesisType, ManType, MeshType.Accessory, false);
#endif
        }

        /// <summary> Save current project to file </summary>
        public void ToStream()
        {
            try
            {
                if (ProgramCore.Project.AutodotsUsed)
                {
                    ProgramCore.MainForm.ctrlRenderControl.SaveSmoothedTextures(false);
                    ProgramCore.MainForm.ctrlRenderControl.SaveBrushTextures();
                }

                var path = Path.Combine(ProjectPath, string.Format("{0}.hds", ProjectName));
                using (var bw = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
                {
                    bw.Write(ProjectName);
                    bw.Write(FrontImagePath);

                    bw.Write(HeadModelPath);

                    bw.Write((int)GenesisType);
                    bw.Write((int)ManType);
                    bw.Write((int)TextureFlip);
                    bw.Write((int)ShapeFlip);

                    bw.Write(IsOpenSmile);

                    var fiName = Path.Combine(ProjectPath, "OBJ", "hair.obj");
                    FolderEx.CreateDirectory(Path.GetDirectoryName(fiName));
                    if (ProgramCore.MainForm.ctrlRenderControl.pickingController.HairMeshes.Count > 0) // save hair file
                        ObjSaver.SaveObjFile(fiName, ProgramCore.MainForm.ctrlRenderControl.pickingController.HairMeshes,
                            MeshType.Hair, 1.0f, ManType, ProgramCore.Project.ProjectName);
                    else
                        FolderEx.DeleteSafety(fiName);      // если раньше были волосы , а сейчас удалили - надо их грохнуть из папки тоже

                    var acDirPath = Path.GetDirectoryName(fiName);
                    var acName = Path.GetFileNameWithoutExtension(fiName) + "_accessory.obj";
                    var accessoryPath = Path.Combine(acDirPath, acName);
                    if (ProgramCore.MainForm.ctrlRenderControl.pickingController.AccesoryMeshes.Count > 0)
                        // save accessory file
                        ObjSaver.SaveObjFile(accessoryPath,
                            ProgramCore.MainForm.ctrlRenderControl.pickingController.AccesoryMeshes, MeshType.Accessory, 1.0f, ManType, ProgramCore.Project.ProjectName);
                    else
                        FolderEx.DeleteSafety(accessoryPath);

                    var partsLibraryPath = Path.Combine(ProjectPath, "Parts Library");
                    FolderEx.CreateDirectory(partsLibraryPath);

                    bw.Write(ProgramCore.MainForm.ctrlRenderControl.PartsLibraryMeshes.Count(x => x.Value.Count > 0)); // save list of meshes to part's library
                    foreach (var part in ProgramCore.MainForm.ctrlRenderControl.PartsLibraryMeshes)
                    {
                        if (part.Value.Count == 0)
                            continue;

                        bw.Write(part.Key);
                        bw.Write(part.Value[0].meshType == MeshType.Accessory);
                        bw.Write(part.Value[0].IsVisible);
                        bw.Write(part.Value[0].Path);

                        bw.Write(part.Value.Count);
                        foreach (var selMesh in part.Value)
                            bw.Write(selMesh.Title);

                        var fileName = part.Key + ".obj";
                        ObjSaver.SaveObjFile(Path.Combine(partsLibraryPath, fileName), part.Value, part.Value[0].meshType, 1.0f, ManType, ProgramCore.Project.ProjectName);
                    }

                    bw.Write(AutodotsUsed);

                    bw.Write(RotatedAngle);

                    // сохраняем прямоугольник лица
                    bw.Write(FaceRectRelative.X);
                    bw.Write(FaceRectRelative.Y);
                    bw.Write(FaceRectRelative.Width);
                    bw.Write(FaceRectRelative.Height);

                    // сохраняем центры рта и глаз распознаные
                    bw.Write(MouthCenter.X);
                    bw.Write(MouthCenter.Y);

                    bw.Write(LeftEyeCenter.X);
                    bw.Write(LeftEyeCenter.Y);
                    bw.Write(RightEyeCenter.X);
                    bw.Write(RightEyeCenter.Y);

                    // сохраняем центры рта и глаз пользовательские (после таскания и прочего)
                    bw.Write(NoseUserCenter.X);
                    bw.Write(NoseUserCenter.Y);
                    bw.Write(MouthUserCenter.X);
                    bw.Write(MouthUserCenter.Y);

                    bw.Write(LeftEyeUserCenter.X);
                    bw.Write(LeftEyeUserCenter.Y);
                    bw.Write(RightEyeUserCenter.X);
                    bw.Write(RightEyeUserCenter.Y);

                    bw.Write(AgeCoefficient);
                    bw.Write(FatCoefficient);

                    //Сохраняем цвет головы
                    bw.Write(FaceColor.X);
                    bw.Write(FaceColor.Y);
                    bw.Write(FaceColor.Z);

                    #region Информация о модели головы

                    var rmPath = Path.Combine(ProjectPath, "Model", "MeshParts.rm");

                    #region Сохранение RenderMesh

                    if (ManType != ManType.Custom)
                    {
                        foreach (var m in ProgramCore.MainForm.ctrlRenderControl.OldMorphing) // перед сохранением сбрасываем морфинги на 0. 
                            m.Value.Delta = 0;
                        foreach (var m in ProgramCore.MainForm.ctrlRenderControl.FatMorphing)
                            m.Value.Delta = 0;
                        //     ProgramCore.MainForm.ctrlRenderControl.DoMorth();
                    }

                    ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.Save(rmPath);

                    if (ManType != ManType.Custom)
                    {
                        foreach (var m in ProgramCore.MainForm.ctrlRenderControl.OldMorphing) // перед сохранением сбрасываем морфинги на 0. 
                            m.Value.Delta = AgeCoefficient;
                        foreach (var m in ProgramCore.MainForm.ctrlRenderControl.FatMorphing)
                            m.Value.Delta = FatCoefficient;
                        //   ProgramCore.MainForm.ctrlRenderControl.DoMorth();
                    }

                    #endregion

                    if (BaseDots != null)
                    {
                        bw.Write(BaseDots.Count);
                        foreach (var point in BaseDots)
                            point.ToStream(bw);
                    }
                    else
                        bw.Write(0);

                    if (ProfileBaseDots != null)
                    {
                        bw.Write(ProfileBaseDots.Count);
                        foreach (var point in ProfileBaseDots)
                            point.ToStream(bw);
                    }
                    else
                        bw.Write(0);

                    RenderMainHelper.autodotsShapeHelper.ShapeInfo.ToStream(bw);
                    RenderMainHelper.autodotsShapeHelper.ShapeProfileInfo.ToStream(bw);

                    ProgramCore.Project.RenderMainHelper.headMeshesController.TexturingInfo.ToStream(bw);


                    bw.Write(ProgramCore.Project.RenderMainHelper.headController.ShapeDots.Count);
                    foreach (var dot in ProgramCore.Project.RenderMainHelper.headController.ShapeDots)
                        dot.ToStreamM(bw);
                    bw.Write(ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2.Count);
                    foreach (var dot in ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2)
                        dot.ToStream(bw);

                    bw.Write(CustomHeadNeedProfileSetup);

                    #endregion

                    bw.Write(ProfileEyeLocation.X);
                    bw.Write(ProfileEyeLocation.Y);
                    bw.Write(ProfileMouthLocation.X);
                    bw.Write(ProfileMouthLocation.Y);

                    if (!string.IsNullOrEmpty(ProgramCore.MainForm.ctrlRenderControl.BackgroundTexture))
                    {
                        bw.Write(true);
                        bw.Write(ProgramCore.MainForm.ctrlRenderControl.BackgroundTexture);
                    }
                    else bw.Write(false);
                    bw.Write(ProgramCore.MainForm.activePanel);

                    bw.Write(ProgramCore.MainForm.ctrlRenderControl.camera.Scale);
                    bw.Write(ProgramCore.MainForm.ctrlRenderControl.camera.beta);
                    bw.Write(ProgramCore.MainForm.ctrlRenderControl.camera._dy);

                    bw.Write(MorphingScale);

                    bw.Write(TextureSize);
                }
            }
            catch (Exception e)
            {
                ProgramCore.EchoToLog(e, true);
            }
        }

        public Camera projectCamera;
        /// <summary> Load project from path </summary>
        public static Project FromStream(string path)
        {
            Project result;
            using (var br = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                var projectFi = new FileInfo(path);

                var projectName = br.ReadString();

                #region template image

                var templateImagePath = br.ReadString();
                if (!string.IsNullOrEmpty(templateImagePath))
                {
                    var fiName = Path.Combine(projectFi.DirectoryName, templateImagePath);
                    var fi = new FileInfo(fiName);
                    if (!fi.Exists)
                    {
                        ProgramCore.EchoToLog("Can't find template image in project.", EchoMessageType.Warning);
                        templateImagePath = string.Empty;
                    }
                }

                #endregion

                var headModelPath = br.ReadString();

                var genesisType = (GenesisType)br.ReadInt32();
                var manType = (ManType)br.ReadInt32();
                var textureFlip = (FlipType)br.ReadInt32();
                var shapeFlip = (FlipType)br.ReadInt32();

                var isOpenSmile = br.ReadBoolean();

                var textureSize = 1024;
                switch (ProgramCore.CurrentProgram)
                {
                    case ProgramCore.ProgramMode.HeadShop_OneClick:
                    case ProgramCore.ProgramMode.HeadShop_v11:
                    case ProgramCore.ProgramMode.FaceAge2_Partial:
                    case ProgramCore.ProgramMode.HeadShop_OneClick_v2:
                    case ProgramCore.ProgramMode.HeadShop_Rotator:
                        textureSize = 2048;          // если поставит ьу нас в проге 4096 - то все крашится к хуям. Пусть уж только на экспорте будет.
                        break;

                        /*   textureSize = 4096;
                           break;*/
                }

                result = new Project(projectName, projectFi.DirectoryName, templateImagePath, genesisType,  manType, headModelPath, false, textureSize, isOpenSmile);
                result.LoadMeshes();
                result.TextureFlip = textureFlip;
                result.ShapeFlip = shapeFlip;

                var partsCount = br.ReadInt32(); //part's library
                for (var i = 0; i < partsCount; i++)
                {
                    var title = br.ReadString();
                    var meshType = br.ReadBoolean() ? MeshType.Accessory : MeshType.Hair;
                    var meshVisible = br.ReadBoolean();
                    var meshPath = br.ReadString();

                    var meshCounts = br.ReadInt32();
                    for (var j = 0; j < meshCounts; j++)
                    {
                        var meshTitle = br.ReadString();
                        DynamicRenderMesh mesh;
                        if (meshType == MeshType.Accessory)
                            mesh = ProgramCore.MainForm.ctrlRenderControl.pickingController.AccesoryMeshes[meshTitle];
                        else
                            mesh = ProgramCore.MainForm.ctrlRenderControl.pickingController.HairMeshes[meshTitle];
                        if (mesh == null)
                            continue;

                        mesh.Title = title + "_" + j;
                        mesh.IsVisible = meshVisible;
                        mesh.Path = meshPath;

                        if (!ProgramCore.MainForm.ctrlRenderControl.PartsLibraryMeshes.ContainsKey(title))
                            ProgramCore.MainForm.ctrlRenderControl.PartsLibraryMeshes.Add(title, new DynamicRenderMeshes());
                        ProgramCore.MainForm.ctrlRenderControl.PartsLibraryMeshes[title].Add(mesh);
                    }
                }

                result.AutodotsUsed = br.ReadBoolean();

                result.RotatedAngle = br.ReadDouble();

                // загружаем прямоугольник лица (фронт)
                result.FaceRectRelative = new RectangleF(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                result.MouthCenter = new Vector2(br.ReadSingle(), br.ReadSingle());
                result.LeftEyeCenter = new Vector2(br.ReadSingle(), br.ReadSingle());
                result.RightEyeCenter = new Vector2(br.ReadSingle(), br.ReadSingle());

                result.NoseUserCenter = new Vector2(br.ReadSingle(), br.ReadSingle());
                result.MouthUserCenter = new Vector2(br.ReadSingle(), br.ReadSingle());
                result.LeftEyeUserCenter = new Vector2(br.ReadSingle(), br.ReadSingle());
                result.RightEyeUserCenter = new Vector2(br.ReadSingle(), br.ReadSingle());

                result.AgeCoefficient = br.ReadSingle();
                result.FatCoefficient = br.ReadSingle();

                //Сохраняем цвет головы
                result.FaceColor = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), 1.0f);

                #region Информация о модели головы

                var rmPath = Path.Combine(projectFi.DirectoryName, "Model", "MeshParts.rm");
                result.RenderMainHelper.headMeshesController.RenderMesh.Load(rmPath);
                foreach (var part in result.RenderMainHelper.headMeshesController.RenderMesh.Parts)
                {
                    if (!string.IsNullOrEmpty(part.TextureName))
                        part.Texture = ProgramCore.MainForm.ctrlRenderControl.GetTexture(part.TextureName);
                    if (!string.IsNullOrEmpty(part.TransparentTextureName))
                        part.TransparentTexture = ProgramCore.MainForm.ctrlRenderControl.GetTexture(part.TransparentTextureName);
                }

                var baseDotsCount = br.ReadInt32();
                for (var i = 0; i < baseDotsCount; i++)
                    result.BaseDots.Add(HeadPoint.FromStream(br));

                var profileBaseDotsCount = br.ReadInt32();
                for (var i = 0; i < profileBaseDotsCount; i++)
                    result.ProfileBaseDots.Add(HeadPoint.FromStream(br));

                result.RenderMainHelper.autodotsShapeHelper.ShapeInfo = TexturingInfo.FromStream(br);
                result.RenderMainHelper.autodotsShapeHelper.ShapeProfileInfo = TexturingInfo.FromStream(br);

                result.RenderMainHelper.headMeshesController.TexturingInfo = TexturingInfo.FromStream(br);

                result.RenderMainHelper.headController.AutoDotsv2.Clear();
                result.RenderMainHelper.headController.ShapeDots.Clear();
                var cnt = br.ReadInt32();
                for (var i = 0; i < cnt; i++)
                    result.RenderMainHelper.headController.ShapeDots.Add(MirroredHeadPoint.FromStreamW(br));
                cnt = br.ReadInt32();
                for (var i = 0; i < cnt; i++)
                    result.RenderMainHelper.headController.AutoDotsv2.Add(HeadPoint.FromStream(br));

                result.CustomHeadNeedProfileSetup = br.ReadBoolean();

                #endregion

                result.ProfileEyeLocation = new Vector2(br.ReadSingle(), br.ReadSingle());
                result.ProfileMouthLocation = new Vector2(br.ReadSingle(), br.ReadSingle());

                var fi1 = new FileInfo(Path.Combine(projectFi.DirectoryName, "ProfileImage.jpg"));
                if (fi1.Exists)
                {
                    using (var fs = new FileStream(fi1.FullName, FileMode.Open))
                    {
                        using (var bmp = new Bitmap(fs))
                            result.ProfileImage = (Bitmap)bmp.Clone();
                    }
                }

                try
                {
                    var hasStage = br.ReadBoolean();
                    if (hasStage)
                        ProgramCore.MainForm.ctrlRenderControl.BackgroundTexture = br.ReadString();

                    ProgramCore.MainForm.activePanel = br.ReadInt32();

                    result.projectCamera = new Camera();
                    result.projectCamera.Scale = br.ReadSingle();
                    result.projectCamera.beta = br.ReadDouble();
                    result.projectCamera._dy = br.ReadSingle();
                }
                catch
                {
                }

                try
                {
                    result.MorphingScale = br.ReadSingle();
                }
                catch
                {
                }

                try
                {
                    result.TextureSize = br.ReadInt32();
                }
                catch
                {
                }
            }

            return result;
        }

        /// <summary> Simple function, that parce project file and return path to template image (require for preview when loading project) </summary>
        public static string LoadTempaltePath(string projectPath)
        {
            using (var br = new BinaryReader(File.Open(projectPath, FileMode.Open)))
            {
                var projectFi = new FileInfo(projectPath);

                var projectName = br.ReadString();
                var templateImagePath = br.ReadString();
                return Path.Combine(projectFi.DirectoryName, templateImagePath);
            }
        }

        private void RecalcRect()
        {
            var leftTop = new Vector2(LeftEyeCenter.X, Math.Max(LeftEyeCenter.Y, RightEyeCenter.Y));
            var rightBottom = new Vector2(RightEyeCenter.X, MouthCenter.Y);
            FaceRectRelative = new RectangleF(leftTop.X, leftTop.Y, rightBottom.X - leftTop.X, rightBottom.Y - leftTop.Y);
        }

        public static string CopyImgToProject(string imagePath, string imageType)
        {
            var fi = new FileInfo(imagePath);
            if (fi.Exists)
            {
                var newImagePath = Path.Combine(ProgramCore.Project.ProjectPath, imageType + Path.GetExtension(fi.Name));
                if (newImagePath != imagePath)
                    File.Copy(imagePath, newImagePath, true);
                return newImagePath;
            }
            return string.Empty;
        }
        public static string CopyImgToProject(Image image, string imageType)
        {
            var newImagePath = Path.Combine(ProgramCore.Project.ProjectPath, imageType + ".jpg");

            if (File.Exists(newImagePath))
                File.Delete(newImagePath);

            image.Save(newImagePath);
            return newImagePath;
        }
    }

    public enum ManType
    {
        Male = 1,
        Female = 0,
        Child = 2,
        Custom = 3         // произвольный тип ебала
    }



    public static class ManTypeEx
    {
        public static string GetCaption(this ManType manType)
        {
            switch (manType)
            {
                case ManType.Male:
                    return "Man";
                case ManType.Female:
                    return "Woman";
                case ManType.Child:
                    return "Child";
                default:
                    return "Custom";
            }
        }

        public static string GetObjDirPath(this ManType manType, bool isOpenSmile)
        {
            switch (manType)
            {
                case ManType.Male:
                    return isOpenSmile ? "MaleWithSmilePlugin" : "Male";
                case ManType.Female:
                    return isOpenSmile ? "FemWithSmilePlugin" : "Fem";
                case ManType.Child:
                    return isOpenSmile ? "ChildWithSmilePlugin" : "Child";
                default:
                    return "Fem";
            }
        }

        public static string GetObjDirPathSmilePlugin(this ManType manType)
        {
            switch (manType)
            {
                case ManType.Male:
                    return "MaleWithSmilePlugin";
                case ManType.Female:
                    return "FemWithSmilePlugin";
                case ManType.Child:
                    return "ChildWithSmilePlugin";
                default:
                    return string.Empty;
            }
        }

        public static string GetObjPath(this ManType manType, bool isOpenSmile)
        {
            switch (manType)
            {
                case ManType.Male:
                    return Path.Combine(manType.GetObjDirPath(isOpenSmile), "Male.obj");
                case ManType.Female:
                    return Path.Combine(manType.GetObjDirPath(isOpenSmile), "Fem.obj");
                case ManType.Child:
                    return Path.Combine(manType.GetObjDirPath(isOpenSmile), "Child.obj");
                default:
                    return string.Empty;
            }
        }

        public static string GetObjPathSmilePlugin(this ManType manType)
        {
            switch (manType)
            {
                case ManType.Male:
                    return Path.Combine(manType.GetObjDirPathSmilePlugin(), "Male.obj");
                case ManType.Female:
                    return Path.Combine(manType.GetObjDirPathSmilePlugin(), "Fem.obj");
                case ManType.Child:
                    return Path.Combine(manType.GetObjDirPathSmilePlugin(), "Child.obj");
                default:
                    return string.Empty;
            }
        }

        public static string GetGenesisPath(this GenesisType genesisType)
        {
            switch (genesisType)
            {
                case GenesisType.Genesis3:
                    return "Genesis 3";
                case GenesisType.Genesis8:
                    return "Genesis 8";
                default:
                    return "Genesis 2";
            }
        }
    }

    public enum FlipType
    {
        LeftToRight,
        RightToLeft,
        None
    }
}
