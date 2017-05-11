using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using OpenTK;
using RH.Core.Render.Controllers;
using RH.Core.Render.Helpers;
using RH.Core.Render.Meshes;
using RH.Core.Render.Obj;
using RH.MeshUtils;
using RH.MeshUtils.Data;
using RH.MeshUtils.Helpers;

namespace RH.Core.Helpers
{
    public class RenderMainHelper
    {
        public readonly HeadShapeController HeadShapeController = new HeadShapeController();
        public readonly HeadController headController = new HeadController();
        public readonly HeadMeshesController headMeshesController = new HeadMeshesController();
        public readonly AutodotsShapeHelper autodotsShapeHelper = new AutodotsShapeHelper();
        public readonly PickingController pickingController = new PickingController(null);

        public readonly Dictionary<string, TextureInfo> textures = new Dictionary<string, TextureInfo>();
        public readonly Dictionary<int, int> SmoothedTextures = new Dictionary<int, int>();

        public readonly Dictionary<string, Tuple<Vector3, float>> HairPositions = new Dictionary<string, Tuple<Vector3, float>>();
        public readonly Dictionary<string, Tuple<Vector3, float>> AccessoryPositions = new Dictionary<string, Tuple<Vector3, float>>();

        public RenderMainHelper()
        {
        }
        public void InitializeHairPositions()
        {
            HairPositions.Clear();

            var filePath = @"ftp://108.167.164.209/public_html/printahead.online/Library/Hair/parts.cfg";
            if (!FTPHelper.IsFileExists(filePath))
                return;

            var request = (FtpWebRequest)FtpWebRequest.Create(filePath);
            request.Credentials = new NetworkCredential(FTPHelper.Login, FTPHelper.Password);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            var ftpResponse = (FtpWebResponse)request.GetResponse();

            byte[] buffer = new byte[16 * 1024];
            using (var ftpStream = ftpResponse.GetResponseStream())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    ms.Position = 0;
                    ms.Flush();
                    using (var sr = new StreamReader(ms, Encoding.Default))
                    {
                        while (!sr.EndOfStream)
                        {
                            var path = sr.ReadLine();
                            var position = sr.ReadLine();
                            var size = sr.ReadLine();

                            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(size) || string.IsNullOrEmpty(position)) continue;

                            HairPositions.Add(Path.GetFileNameWithoutExtension(path.Remove(0, 1)), new Tuple<Vector3, float>(Vector3Ex.FromString(position.Split('=')[1]), StringConverter.ToFloat(size.Split('=')[1])));
                        }
                    }
                }
            }
        }

        public void InitializeAccessoryPositions()
        {
            AccessoryPositions.Clear();

            var filePath = @"ftp://108.167.164.209/public_html/printahead.online/Library/Accessory/parts.cfg";
            if (!FTPHelper.IsFileExists(filePath))
                return;

            var request = (FtpWebRequest)FtpWebRequest.Create(filePath);
            request.Credentials = new NetworkCredential(FTPHelper.Login, FTPHelper.Password);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            var ftpResponse = (FtpWebResponse)request.GetResponse();

            byte[] buffer = new byte[16 * 1024];
            using (var ftpStream = ftpResponse.GetResponseStream())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    ms.Position = 0;
                    ms.Flush();
                    using (var sr = new StreamReader(ms, Encoding.Default))
                    {
                        while (!sr.EndOfStream)
                        {
                            var path = sr.ReadLine();
                            var position = sr.ReadLine();
                            var size = sr.ReadLine();

                            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(size) || string.IsNullOrEmpty(position)) continue;

                            AccessoryPositions.Add(Path.GetFileNameWithoutExtension(path.Remove(0, 1)), new Tuple<Vector3, float>(Vector3Ex.FromString(position.Split('=')[1]), StringConverter.ToFloat(size.Split('=')[1])));
                        }
                    }
                }
            }
        }

        public int HeadTextureId;

        public void LoadModel(string path, bool needClean, ManType manType, MeshType type)
        {
            if (needClean)
                CleanProjectMeshes();

            pickingController.AddMehes(path, type, false, manType, false);
        }

        public void CleanProjectMeshes()
        {
            //  textures.Clear();             // сейчас это не надо :)
            pickingController.SelectedMeshes.Clear();
            pickingController.HairMeshes.Clear();
            pickingController.AccesoryMeshes.Clear();
        }

        public RectangleAABB InitializeShapedotsHelper(bool isNew = false)
        {
            autodotsShapeHelper.headMeshesController = headMeshesController;
            autodotsShapeHelper.SetType((int)ProgramCore.Project.ManType);
            autodotsShapeHelper.Initialise(HeadController.GetDots(ProgramCore.Project.ManType), isNew);

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

        public int GetTexture(string textureName)
        {
            var textureId = 0;
            if (!string.IsNullOrEmpty(textureName))//&& File.Exists(textureName))
            {

                if (textures.ContainsKey(textureName))
                    return textures[textureName].Texture;

                //Bitmap bitmap;
                //using (var ms = new MemoryStream(File.ReadAllBytes(textureName)))
                //    bitmap = (Bitmap)Image.FromStream(ms);

                textureId = textures.Count + 1; //GetTexture(bitmap);
                textures.Add(textureName, new TextureInfo
                {
                    Texture = textureId,
                    Width = 0,
                    Height = 0
                });
            }
            return textureId;
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

        public void LoadProject(bool newProject, RectangleAABB aabb, string headTexturePath)
        {
            //var headTexturePath = Path.Combine(ProgramCore.Project.ProjectPath, ProgramCore.Project.FrontImagePath);
            HeadTextureId = 0;
            if (!string.IsNullOrEmpty(headTexturePath))
            {
                HeadTextureId = GetTexture(headTexturePath);

                //if (ProgramCore.Project.FaceRectRelative == RectangleF.Empty)
                //{
                //    var fileName = Path.Combine(ProgramCore.Project.ProjectPath, ProgramCore.Project.FrontImagePath);

                //    var faceRecognition = new OpenCvFaceRecognition();
                //    faceRecognition.Recognize(ref fileName, false);

                //    ProgramCore.Project.FaceRectRelative = faceRecognition.FaceRectRelative;
                //    ProgramCore.Project.MouthCenter = faceRecognition.MouthCenter;
                //    ProgramCore.Project.LeftEyeCenter = faceRecognition.LeftEyeCenter;
                //    ProgramCore.Project.RightEyeCenter = faceRecognition.RightEyeCenter;
                //    ProgramCore.Project.FaceColor = faceRecognition.FaceColor;
                //}

            }

            if (newProject)
            {
                var modelPath = ProgramCore.Project.HeadModelPath;
                pickingController.AddMehes(modelPath, MeshType.Head, false, ProgramCore.Project.ManType, ProgramCore.PluginMode);

                //float scale = 0;
                //if (ProgramCore.Project.ManType == ManType.Custom)
                //{
                //    scale = headMeshesController.SetSize(29.3064537f); // подгонка размера для произвольной башки
                //}
                //else if (ProgramCore.PluginMode)
                //{
                //    switch (ProgramCore.Project.ManType)
                //    {
                //        case ManType.Male:
                //            scale = headMeshesController.SetSize(29.9421043f); // подгонка размера 
                //            break;
                //        case ManType.Female:
                //            scale = headMeshesController.SetSize(29.3064537f); // подгонка размера 
                //            break;
                //        case ManType.Child:
                //            scale = headMeshesController.SetSize(25.6209984f); // подгонка размера 
                //            break;
                //    }
                //}
                //if (pickingController.ObjExport != null)
                //    pickingController.ObjExport.Scale = scale;
            }
            HeadShapeController.Initialize(headMeshesController);
            //brushTool.InitializeBrush(headMeshesController);

            //if (ProgramCore.Project.ManType != ManType.Custom)
            //{
            //    var oldMorphingPath = Path.Combine(Application.StartupPath, "Models\\Morphing", ProgramCore.Project.ManType.GetCaption(), "Old.obj"); // загружаем трансформации для старения
            //    OldMorphing = pickingController.LoadPartsMorphInfo(oldMorphingPath, headMeshesController.RenderMesh);

            //    var fatMorphingPath = Path.Combine(Application.StartupPath, "Models\\Morphing", ProgramCore.Project.ManType.GetCaption(), "Fat.obj"); // загружаем трансформации для толстения
            //    FatMorphing = pickingController.LoadPartsMorphInfo(fatMorphingPath, headMeshesController.RenderMesh);
            //}

            var baseDots = HeadController.GetBaseDots(ProgramCore.Project.ManType);
            headMeshesController.RenderMesh.SetBlendingInfo(baseDots[0], baseDots[1], baseDots[2], baseDots[3]);

            #region Сглаживание текстур

            SmoothedTextures.Clear();
            var index = 1;
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
                        var newImagePath = @"ftp://108.167.164.209/public_html/printahead.online/PrintAhead_models/" + ProgramCore.Project.ProjectName + "/Textures/";
                        //var ftpHelper = new FTPHelper();
                        //var di = new DirectoryInfo(newImagePath);
                        // if (!di.Exists)
                        //    di.Create();

                        //var brushImagePath = Path.Combine(newImagePath, Path.GetFileNameWithoutExtension(path) + "_brush.png");
                        var smoothedImagePath = newImagePath + Path.GetFileNameWithoutExtension(path) + "_smoothed" + Path.GetExtension(path);
                        //if (!File.Exists(smoothedImagePath))
                        //File.Copy(path, smoothedImagePath, true);

                        //newImagePath = Path.Combine(newImagePath, Path.GetFileNameWithoutExtension(path) + Path.GetExtension(path));
                        //File.Copy(path, newImagePath, true);                        
                        var smoothedTexture = GetTexture(smoothedImagePath);// GetTexture(smoothedImagePath); // по старому пути у нас будут храниться сглаженные текстуры (что бы сохранение модельки сильно не менять)
                        part.Texture = oldTexture;
                        SmoothedTextures.Add(oldTexture, smoothedTexture); // связка - айди старой-новой текстур

                        //if (File.Exists(brushImagePath) && !brushTextures.ContainsKey(part.Texture))
                        //{
                        //    var texture = GetTexture(brushImagePath);
                        //    Bitmap bitmap;
                        //    using (var ms = new MemoryStream(File.ReadAllBytes(brushImagePath)))
                        //        bitmap = (Bitmap)Image.FromStream(ms);
                        //    brushTextures.Add(smoothedTexture, new BrushTextureInfo { Texture = texture, TextureData = bitmap, LinkedTextureName = smoothedImagePath });
                        //}
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

                    autodotsShapeHelper.TransformRects();
                    autodotsShapeHelper.InitializeShaping();

                    //switch (ProgramCore.CurrentProgram)
                    //{
                    //    case ProgramCore.ProgramMode.HeadShop_Rotator:
                    //        DetectFaceRotation();
                    //        break;
                    //}

                    var points = autodotsShapeHelper.GetBaseDots();

                    SpecialEyePointsUpdate(points, true);
                    SpecialEyePointsUpdate(points, false);

                    SpecialLipsPointsUpdate(points, ProgramCore.Project.MouthCenter);
                    SpecialNosePointsUpdate(points);

                    SpecialCenterUpdate(points, headController.GetNoseTopIndexes(), ProgramCore.Project.DetectedNosePoints[3]);
                    SpecialBottomPointsUpdate(points);
                    SpecialTopHaedWidth(points);
                }
            }
            else
            {
                autodotsShapeHelper.TransformRects();
                headMeshesController.UpdateBuffers();
            }
        }

        #region Transform mesh 
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

        private void SpecialTopHaedWidth(List<HeadPoint> points)
        {
            var topHeadIndices = new int[] { 6, 5, 4, 3, 0, 25, 26, 27, 28 };
            var a = MirroredHeadPoint.GetFrontWorldPoint(ProgramCore.Project.DetectedTopPoints[0], ProgramCore.CurrentProgram);
            var b = MirroredHeadPoint.GetFrontWorldPoint(ProgramCore.Project.DetectedTopPoints[1], ProgramCore.CurrentProgram);
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

        private void SpecialEyePointsUpdate(List<HeadPoint> points, bool isLeft)
        {
            var eyePoints = isLeft ? new[] { 21, 22, 23, 24 } : new[] { 45, 44, 43, 46 };

            for (var i = 0; i < eyePoints.Length; ++i)
            {
                var point = points[eyePoints[i]];
                var delta =
                    MirroredHeadPoint.GetFrontWorldPoint(isLeft ? ProgramCore.Project.DetectedLeftEyePoints[i] : ProgramCore.Project.DetectedRightEyePoints[i], ProgramCore.CurrentProgram) - point.Value;
                point.Value += delta;
                foreach (var l in point.LinkedPoints)
                {
                    var p = points[l];
                    p.Value += delta;
                    autodotsShapeHelper.Transform(p.Value, l);
                }
                autodotsShapeHelper.Transform(point.Value, eyePoints[i]);
            }
        }

        private void SpecialNosePointsUpdate(List<HeadPoint> points)
        {
            var bottomNosePoints = new int[] { 19, 41, 52 };

            for (var i = 0; i < bottomNosePoints.Length; ++i)
            {
                var point = points[bottomNosePoints[i]];
                var delta = MirroredHeadPoint.GetFrontWorldPoint(ProgramCore.Project.DetectedNosePoints[i], ProgramCore.CurrentProgram) - point.Value;
                if (bottomNosePoints[i] != 52)
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
            var rightPosUserSelected = MirroredHeadPoint.GetFrontWorldPoint(targetPoint, ProgramCore.CurrentProgram);          // перенояем координаты с левой картинки в правой
            var delta2 = rightPosUserSelected - center;

            var leftBorder = MirroredHeadPoint.GetFrontWorldPoint(borders[0], ProgramCore.CurrentProgram) - rightPosUserSelected;
            var rightBorder = MirroredHeadPoint.GetFrontWorldPoint(borders[1], ProgramCore.CurrentProgram) - rightPosUserSelected;

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
                p.Value = MirroredHeadPoint.GetFrontWorldPoint(ProgramCore.Project.DetectedLipsPoints[i++], ProgramCore.CurrentProgram);
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
                var delta = MirroredHeadPoint.GetFrontWorldPoint(ProgramCore.Project.DetectedBottomPoints[i], ProgramCore.CurrentProgram) - point.Value;
                point.Value += delta;
                autodotsShapeHelper.Transform(point.Value, bottomPoints[i]);
            }

            var bottomPointsX = new int[] { 7, 8, 30, 29 };
            for (var i = 0; i < bottomPointsX.Length; ++i)
            {
                var point = points[bottomPointsX[i]];
                var delta = MirroredHeadPoint.GetFrontWorldPoint(ProgramCore.Project.DetectedBottomPoints[i + 6], ProgramCore.CurrentProgram) - point.Value;
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

            var rightPosUserSelected = MirroredHeadPoint.GetFrontWorldPoint(targetPoint, ProgramCore.CurrentProgram);          // перенояем координаты с левой картинки в правой
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

        #endregion

        private void SaveObj(string fiName)
        {
            var haPath = Path.GetFileNameWithoutExtension(fiName) + "hair.obj";
            var hairPath = Path.Combine(ProgramCore.Project.ProjectPath, haPath);
            var realScale = ProgramCore.PluginMode ? 1.0f : ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.RealScale;

            ObjSaver.SaveObjFile(hairPath, pickingController.HairMeshes, MeshType.Hair, realScale, ProgramCore.Project.ManType, ProgramCore.Project.ProjectName, true);

            if (pickingController.AccesoryMeshes.Count > 0)            // save accessories to separate file
            {
                var acName = Path.GetFileNameWithoutExtension(fiName) + "_accessory.obj";

                var accessoryPath = Path.Combine(ProgramCore.Project.ProjectPath, acName);
                ObjSaver.SaveObjFile(accessoryPath, pickingController.AccesoryMeshes, MeshType.Accessory, realScale, ProgramCore.Project.ManType, ProgramCore.Project.ProjectName, true);
            }

            SaveHead(fiName, true);
        }

        public void SaveSmoothedTextures(ZipOutputStream zipStream)
        {
#if (WEB_APP)
            var frontTexture = ProgramCore.Project.FrontImage;
            var address = "ftp://108.167.164.209/public_html/printahead.online/PrintAhead_models/" + ProgramCore.Project.ProjectName + "/Textures";
            var ftpHelper = new FTPHelper(address);

            foreach (var smoothTex in SmoothedTextures.Where(s => s.Key != 0))
            {
                var oldTexturePath = GetTexturePath(smoothTex.Key);
                var bitmap = RenderToTexture(smoothTex.Key, smoothTex.Value, frontTexture);
                if (bitmap == null)
                    continue;

                var ms = new MemoryStream();
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                var fileName = Path.GetFileNameWithoutExtension(oldTexturePath) + "_smoothed" +
                               Path.GetExtension(oldTexturePath);

                ftpHelper.Upload(ms, fileName);

                if (zipStream != null)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    var newEntry = new ZipEntry(@"Textures\" + fileName);
                    zipStream.PutNextEntry(newEntry);
                    ms.CopyTo(zipStream);
                    zipStream.CloseEntry();
                }
            }
#else

            var newFolderPath = Path.Combine(ProgramCore.Project.ProjectPath, "Textures");
            var di = new DirectoryInfo(newFolderPath);
            if (!di.Exists)
                di.Create();

            var frontTexture = new Bitmap(ProgramCore.Project.FrontImagePath);

            foreach (var smoothTex in SmoothedTextures.Where(s => s.Key != 0))
            {
                var oldTexturePath = GetTexturePath(smoothTex.Key);
                var newImagePath = Path.Combine(newFolderPath, Path.GetFileNameWithoutExtension(oldTexturePath) + "_smoothed" + Path.GetExtension(oldTexturePath));
                var bitmap = RenderToTexture(smoothTex.Key, smoothTex.Value, frontTexture);
                bitmap.Save(newImagePath, ImageFormat.Jpeg);
            }
#endif
        }

        public Bitmap RenderToTexture(int oldTextureId, int textureId, Bitmap frontTexture)
        {
            var textureWidth = 0;
            var textureHeight = 0;
#if WEB_APP
            var texPath = GetTexturePath(oldTextureId);
            var img = FTPHelper.DownloadImage(texPath);
            if (img == null)
                return null;
#else
            var texPath = GetTexturePath(oldTextureId);
            var img = new Bitmap(texPath);
#endif
            textureWidth = img.Width;
            textureHeight = img.Height;
            PointF[] points = new[] { new PointF(), new PointF(), new PointF() };
            var faceColor = Color.FromArgb((int)(ProgramCore.Project.FaceColor.X * 255), (int)(ProgramCore.Project.FaceColor.Y * 255), (int)(ProgramCore.Project.FaceColor.Z * 255));

            using (var graphic = Graphics.FromImage(img))
            {
                graphic.FillRectangle(new SolidBrush(faceColor), 0, 0, textureWidth - 1, textureHeight - 1);
                var parts = headMeshesController.RenderMesh.Parts.Where(p => p.Texture == textureId);
                foreach (var part in parts)
                {
                    DrawTrianlges(graphic, textureWidth, textureHeight, part, frontTexture);
                }
            }
            return img;
            //return new Bitmap(texPath); 
            //RenderToTexture(oldTextureId, textureId, textureWidth, textureHeight, blendShader, DrawToTexture);
        }

        static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        private readonly Random Random = new Random();

        public Color RandomColor()
        {
            return Color.FromArgb(Random.Next(0, 255), Random.Next(0, 255), Random.Next(0, 255));
        }

        private float Clamp(float a, float min, float max)
        {
            if (a > max)
                return max;
            if (a < min)
                return min;
            return a;
        }

        private void DrawTrianlges(Graphics g, int width, int height, RenderMeshPart part, Bitmap newTexture)
        {
            Vector2i t0 = new Vector2i();
            Vector2i t1 = new Vector2i();
            Vector2i t2 = new Vector2i();

            Vector2 uv0 = new Vector2();
            Vector2 uv1 = new Vector2();
            Vector2 uv2 = new Vector2();

            var newWidth = newTexture.Width - 1;
            var newHeight = newTexture.Height - 1;

            var brush = new SolidBrush(RandomColor());

            const float u_BlendStartDepth = -0.5f;
            const float u_BlendDepth = 4f;


            for (int index = 0; index < part.Indices.Count; index += 3)
            {
                var v0 = part.Vertices[part.Indices[index]];
                var v1 = part.Vertices[part.Indices[index + 1]];
                var v2 = part.Vertices[part.Indices[index + 2]];

                if (v0.TexCoord.Y > v1.TexCoord.Y) Swap(ref v0, ref v1);
                if (v0.TexCoord.Y > v2.TexCoord.Y) Swap(ref v0, ref v2);
                if (v1.TexCoord.Y > v2.TexCoord.Y) Swap(ref v1, ref v2);

                t0.X = (int)Math.Round(v0.TexCoord.X * width);
                t0.Y = (int)Math.Round(v0.TexCoord.Y * height);
                t1.X = (int)Math.Round(v1.TexCoord.X * width);
                t1.Y = (int)Math.Round(v1.TexCoord.Y * height);
                t2.X = (int)Math.Round(v2.TexCoord.X * width);
                t2.Y = (int)Math.Round(v2.TexCoord.Y * height);

                uv0.X = Clamp(v0.AutodotsTexCoord.X, 0f, 1f) * newWidth;
                uv0.Y = Clamp(v0.AutodotsTexCoord.Y, 0f, 1f) * newHeight;
                uv1.X = Clamp(v1.AutodotsTexCoord.X, 0f, 1f) * newWidth;
                uv1.Y = Clamp(v1.AutodotsTexCoord.Y, 0f, 1f) * newHeight;
                uv2.X = Clamp(v2.AutodotsTexCoord.X, 0f, 1f) * newWidth;
                uv2.Y = Clamp(v2.AutodotsTexCoord.Y, 0f, 1f) * newHeight;

                var blend0 = Clamp(v0.AutodotsTexCoord.Z * (v0.Position.Z - u_BlendStartDepth) / u_BlendDepth, 0f, 1f);
                var blend1 = Clamp(v1.AutodotsTexCoord.Z * (v1.Position.Z - u_BlendStartDepth) / u_BlendDepth, 0f, 1f);
                var blend2 = Clamp(v2.AutodotsTexCoord.Z * (v2.Position.Z - u_BlendStartDepth) / u_BlendDepth, 0f, 1f);

                //if (blend0 == 0f && blend1 == 0f && blend2 == 0f)
                //    continue;

                if (t0.Y == t1.Y && t0.Y == t2.Y)
                {
                    continue;
                }


                var total_height = t2.Y - t0.Y;

                for (int i = 0; i < total_height; i++)
                {
                    bool second_half = i > (t1.Y - t0.Y) || t1.Y == t0.Y;
                    int segment_height = second_half ? (t2.Y - t1.Y) : (t1.Y - t0.Y);
                    float alpha = (float)i / total_height;
                    float beta = (float)(i - (second_half ? (t1.Y - t0.Y) : 0)) / segment_height;

                    var a = t0 + (t2 - t0) * alpha;
                    var b = second_half ? t1 + (t2 - t1) * beta : t0 + (t1 - t0) * beta;

                    var uvA = uv0 + (uv2 - uv0) * alpha;
                    var uvB = second_half ? uv1 + (uv2 - uv1) * beta : uv0 + (uv1 - uv0) * beta;

                    var blendA = blend0 + (blend2 - blend0) * alpha;
                    var blendB = second_half ? blend1 + (blend2 - blend1) * beta : blend0 + (blend1 - blend0) * beta;

                    if (a.X > b.X)
                    {
                        Swap(ref a, ref b);
                        Swap(ref uvA, ref uvB);
                    }

                    var ax = a.X;
                    var bx = b.X;
                    for (int j = ax; j <= bx; j++)
                    {
                        float phi = b.X == a.X ? 1f : (j - a.X) / (float)(b.X - a.X);
                        var uvP = uvA + (uvB - uvA) * phi;
                        var blend = blendA + (blendB - blendA) * phi;
                        var invBlend = 1f - blend;
                        var color = newTexture.GetPixel((int)uvP.X, (int)uvP.Y);
                        var resultColor = Color.FromArgb((int)(blend * 255), color.R, color.G, color.B);
                        g.FillRectangle(new SolidBrush(resultColor), j, t0.Y + i, 1, 1);
                    }
                }
            }
        }

        //private void DrawToTexture(int oldTextureId, int textureId)
        //{
        //    var parts = headMeshesController.RenderMesh.Parts.Where(p => p.Texture == textureId);
        //    foreach (var part in parts)
        //    {
        //        foreach(var v in part.Vertices)
        //    }
        //}

        /*
          public void DrawToTexture(IEnumerable<RenderMeshPart> parts)
        {
#if (WEB_APP)
#else
            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            foreach (var part in parts)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, part.VertexBuffer);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, part.IndexBuffer);

                GL.VertexPointer(2, VertexPointerType.Float, Vertex3d.Stride, new IntPtr(2 * Vector3.SizeInBytes)); //Как позицию используем основные текстурные координаты
                GL.NormalPointer(NormalPointerType.Float, Vertex3d.Stride, new IntPtr(0));//Как нормаль используем позиции (координата Z потребуется для вычисления смешивания 
                GL.TexCoordPointer(3, TexCoordPointerType.Float, Vertex3d.Stride, new IntPtr(2 * Vector3.SizeInBytes + Vector2.SizeInBytes + Vector4.SizeInBytes));//Как текстурные координаты берем дополнительные текстурные координаты

                GL.DrawRangeElements(PrimitiveType.Triangles, 0, part.CountIndices, part.CountIndices, DrawElementsType.UnsignedInt, new IntPtr(0));
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
#endif
        }
             */

        //private bool DrawToTexture(ShaderController shader, int oldTextureId, int textureId)
        //{
        //    //GL.BindTexture(TextureTarget.Texture2D, oldTextureId);
        //    DrawQuad(ProgramCore.Project.FaceColor.X, ProgramCore.Project.FaceColor.Y, ProgramCore.Project.FaceColor.Z, 1f);
        //    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        //    GL.Enable(EnableCap.Blend);

        //    shader.Begin();

        //    GL.ActiveTexture(TextureUnit.Texture0);
        //    GL.BindTexture(TextureTarget.Texture2D, HeadTextureId);
        //    shader.UpdateUniform("u_Texture", 0);
        //    shader.UpdateUniform("u_BlendStartDepth", -0.5f);
        //    shader.UpdateUniform("u_BlendDepth", 4f);

        //    headMeshesController.RenderMesh.DrawToTexture(headMeshesController.RenderMesh.Parts.Where(p => p.Texture == textureId));

        //    shader.End();
        //    GL.Disable(EnableCap.Blend);
        //    return true;
        //}

        //public Bitmap RenderToTexture(int oldTextureId, int textureId, int textureWidth, int textureHeight, ShaderController shader,
        //    Func<ShaderController, int, int, bool> renderFunc, bool useAlpha = false)
        //{
        //    graphicsContext.MakeCurrent(windowInfo);
        //    renderPanel.Size = new Size(textureWidth, textureHeight);
        //    GL.Viewport(0, 0, textureWidth, textureHeight);

        //    GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        //    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        //    GL.MatrixMode(MatrixMode.Projection);
        //    GL.PushMatrix();
        //    GL.LoadIdentity();
        //    GL.MatrixMode(MatrixMode.Modelview);
        //    GL.LoadIdentity();

        //    GL.Enable(EnableCap.Texture2D);
        //    GL.DepthMask(false);

        //    renderFunc(shader, oldTextureId, textureId);

        //    GL.DepthMask(true);
        //    GL.BindTexture(TextureTarget.Texture2D, 0);
        //    GL.MatrixMode(MatrixMode.Projection);
        //    GL.PopMatrix();

        //    var result = GrabScreenshot(String.Empty, textureWidth, textureHeight, useAlpha);
        //    glControl.Context.MakeCurrent(glControl.WindowInfo);
        //    SetupViewport(glControl);
        //    return result;
        //}

        //public Bitmap GrabScreenshot(string filePath, int width, int height, bool useAlpha = false)
        //{
        //    var bmp = new Bitmap(width, height);
        //    var rect = new Rectangle(0, 0, width, height);
        //    var data = bmp.LockBits(rect, ImageLockMode.WriteOnly, useAlpha ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb);
        //    GL.ReadPixels(0, 0, width, height, useAlpha ? OpenTK.Graphics.OpenGL.PixelFormat.Bgra : OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
        //    GL.Finish();
        //    bmp.UnlockBits(data);
        //    bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

        //    if (!string.IsNullOrEmpty(filePath))
        //        bmp.Save(filePath, ImageFormat.Jpeg);
        //    return bmp;
        //}


        public void SaveHead(string path, bool saveBrushesToTexture = false)
        {
            try
            {
                /*if (ProgramCore.Project.AutodotsUsed)
                    SaveBlendingTextures();*/

                ObjSaver.SaveObjFile(path, headMeshesController.RenderMesh, MeshType.Hair, pickingController.ObjExport, ProgramCore.Project.ProjectName, saveBrushesToTexture);
            }
            finally
            {
                //SaveSmoothedTextures();
            }
        }

        public void SaveMergedHead(string path, int size, ZipOutputStream zipStream)
        {
            var meshInfos = new List<MeshInfo>();

            foreach (var part in headMeshesController.RenderMesh.Parts)
                meshInfos.Add(new MeshInfo(part, headMeshesController.RenderMesh.MorphScale));


            ObjSaver.ExportMergedModel(path, pickingController.HairMeshes, pickingController.AccesoryMeshes, meshInfos, headMeshesController.RenderMesh.RealScale, ProgramCore.Project.ProjectName, false, false, size, zipStream);
        }


        public void UpdateUserCenterPositions()
        {
            var center = UpdateUserCenterPositions(headController.GetLeftEyeIndexes());  // Left eye
            if (center != Vector2.Zero)
                ProgramCore.Project.LeftEyeUserCenter = center;

            center = UpdateUserCenterPositions(headController.GetRightEyeIndexes());  // Right eye
            if (center != Vector2.Zero)
                ProgramCore.Project.RightEyeUserCenter = center;

            center = UpdateUserCenterPositions(headController.GetMouthIndexes());  // Mouth
            if (center != Vector2.Zero)
                ProgramCore.Project.MouthUserCenter = center;

            center = UpdateUserCenterPositions(headController.GetNoseIndexes());  // Nose
            if (center != Vector2.Zero)
                ProgramCore.Project.NoseUserCenter = center;

            #region Определяем прямоугольник, охватывающий все автоточки

            //if (updateRect)
            //    UpdateFaceRect();

            #endregion

            //RecalcUserCenters();
        }

        private Vector2 UpdateUserCenterPositions(IEnumerable<int> indexes)
        {
            List<MirroredHeadPoint> sourcePoints = ProgramCore.Project.RenderMainHelper.headController.AutoDots;
            if (sourcePoints.Count == 0)
                return Vector2.Zero;

            var dots = new List<MirroredHeadPoint>();
            foreach (var index in indexes)
            {
                var dot = sourcePoints[index];
                dots.Add(dot);
            }

            var minX = dots.Min(point => point.ValueMirrored.X);
            var maxX = dots.Max(point => point.ValueMirrored.X);
            var minY = dots.Min(point => point.ValueMirrored.Y);
            var maxY = dots.Max(point => point.ValueMirrored.Y);

            return new Vector2((maxX + minX) * 0.5f, (maxY + minY) * 0.5f);
        }

        //private void UpdateFaceRect()
        //{
        //    var indicies = ProgramCore.Project.RenderMainHelper.headController.GetFaceIndexes();
        //    List<MirroredHeadPoint> faceDots;
        //    switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
        //    {
        //        // case Mode.HeadShapedots:
        //        case Mode.HeadLine:
        //        case Mode.HeadShapeFirstTime:
        //        case Mode.HeadShape:
        //            faceDots = ProgramCore.Project.RenderMainHelper.headController.GetSpecialShapedots(indicies);
        //            break;
        //        default:
        //            faceDots = ProgramCore.Project.RenderMainHelper.headController.GetSpecialAutodots(indicies);
        //            break;
        //    }

        //    if (faceDots.Count == 0)
        //        return;
        //    {
        //        var minX1 = faceDots.Min(point => point.ValueMirrored.X);
        //        var maxX1 = faceDots.Max(point => point.ValueMirrored.X);
        //        var minY1 = faceDots.Min(point => point.ValueMirrored.Y);
        //        var maxY1 = faceDots.Max(point => point.ValueMirrored.Y);

        //        var rrr = new RectangleF((float)minX1, (float)minY1, (float)(maxX1 - minX1), (float)(maxY1 - minY1));
        //    }

        //    var minX = faceDots.Min(point => point.ValueMirrored.X) * ImageTemplateWidth + ImageTemplateOffsetX;
        //    var maxX = faceDots.Max(point => point.ValueMirrored.X) * ImageTemplateWidth + ImageTemplateOffsetX;
        //    var minY = faceDots.Min(point => point.ValueMirrored.Y) * ImageTemplateHeight + ImageTemplateOffsetY;
        //    var maxY = faceDots.Max(point => point.ValueMirrored.Y) * ImageTemplateHeight + ImageTemplateOffsetY;

        //    FaceRectTransformed = new Rectangle((int)minX, (int)minY, (int)(maxX - minX), (int)(maxY - minY));

        //    CentralFacePoint = new Vector2(minX + (maxX - minX) * 0.5f, minY + (maxY - minY) / 3f);
        //}

        //private void RecalcUserCenters()
        //{
        //    MouthTransformed = new Vector2(ProgramCore.Project.MouthUserCenter.X * ImageTemplateWidth + ImageTemplateOffsetX,
        //                                 ProgramCore.Project.MouthUserCenter.Y * ImageTemplateHeight + ImageTemplateOffsetY);
        //    LeftEyeTransformed = new Vector2(ProgramCore.Project.LeftEyeUserCenter.X * ImageTemplateWidth + ImageTemplateOffsetX,
        //                                   ProgramCore.Project.LeftEyeUserCenter.Y * ImageTemplateHeight + ImageTemplateOffsetY);
        //    RightEyeTransformed = new Vector2(ProgramCore.Project.RightEyeUserCenter.X * ImageTemplateWidth + ImageTemplateOffsetX,
        //                                   ProgramCore.Project.RightEyeUserCenter.Y * ImageTemplateHeight + ImageTemplateOffsetY);
        //    NoseTransformed = new Vector2(ProgramCore.Project.NoseUserCenter.X * ImageTemplateWidth + ImageTemplateOffsetX,
        //                       ProgramCore.Project.NoseUserCenter.Y * ImageTemplateHeight + ImageTemplateOffsetY);
        //}

        public void AttachHair(string hairObjPath, string materialPath, ManType manType, int size)
        {
            var objModel = ObjLoader.LoadObjFile(hairObjPath, false);
            if (objModel == null)
                return;

            var temp = 0;
            var meshes = PickingController.LoadHairMeshes(objModel, null, true, manType, MeshType.Hair,size, ref temp);
            foreach (var mesh in meshes)
            {
                if (mesh == null || mesh.vertexArray.Length == 0) //ТУТ!
                    continue;
                mesh.Material.DiffuseTextureMap = materialPath;
            }

            var objName = Path.GetFileNameWithoutExtension(hairObjPath);
            if (HairPositions.ContainsKey(objName))
            {
                var meshSize = HairPositions[objName].Item2;

                var s = HairPositions[objName].Item1 * ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.MorphScale;         // домножаем на 8 для веб версии. все на 8 домножаем! любим 8!
                for (var i = 0; i < meshes.Count; i++)
                {
                    var mesh = meshes[i];
                    if (mesh == null || mesh.vertexArray.Length == 0) //ТУТ!
                        continue;

                    mesh.Position += new Vector3(s[0], s[1], s[2]);
                    mesh.Transform[3, 0] += s[0];
                    mesh.Transform[3, 1] += s[1];
                    mesh.Transform[3, 2] += s[2];

                    if (!float.IsNaN(meshSize))
                        mesh.InterpolateMesh(meshSize);
                }
            }

            ProgramCore.Project.RenderMainHelper.pickingController.HairMeshes.Clear();
            ProgramCore.Project.RenderMainHelper.pickingController.HairMeshes.AddRange(meshes);
        }

        public void AttachAccessory(string accessoryObjPath, string accessoryMaterialPath, ManType manType, int size)
        {
            var objModel = ObjLoader.LoadObjFile(accessoryObjPath, false);
            if (objModel == null)
                return;

            var mesh = PickingController.LoadAccessoryMesh(objModel, size);
            if (string.IsNullOrEmpty(accessoryMaterialPath))
                mesh.Material.DiffuseColor = new Vector4(0.5f, 0.4f, 0.3f, 1);
            else
                mesh.Material.DiffuseTextureMap = accessoryMaterialPath;

            var objName = Path.GetFileNameWithoutExtension(accessoryObjPath);
            if (AccessoryPositions.ContainsKey(objName))
            {
                var meshSize = AccessoryPositions[objName].Item2;

                var s = AccessoryPositions[objName].Item1 * ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.MorphScale;         // домножаем на 8 для веб версии. все на 8 домножаем! любим 8!

                mesh.Position += new Vector3(s[0], s[1], s[2]);
                mesh.Transform[3, 0] += s[0];
                mesh.Transform[3, 1] += s[1];
                mesh.Transform[3, 2] += s[2];

                if (!float.IsNaN(meshSize))
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
            }

            ProgramCore.Project.RenderMainHelper.pickingController.AccesoryMeshes.Add(mesh);
        }
    }
}
