﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;
using Luxand;
using OpenTK;
using RH.Core;
using RH.Core.Helpers;
using RH.Core.Render.Controllers;
using RH.MeshUtils;
using RH.MeshUtils.Data;
using RH.MeshUtils.Helpers;

namespace RH.WebCore
{
    /// <summary> ДЛЯ правильной работы с сайтом, требуется выставить директиву WEB_APP у сборок MeshUtils и Core в свойствах. </summary>
    public class ObjCreator
    {

        #region Fields
        private HeadShapeController HeadShapeController
        {
            get
            {
                return ProgramCore.Project.RenderMainHelper.HeadShapeController;
            }
        }
        private HeadController headController
        {
            get
            {
                return ProgramCore.Project.RenderMainHelper.headController;
            }
        }
        private HeadMeshesController headMeshesController
        {
            get
            {
                return ProgramCore.Project.RenderMainHelper.headMeshesController;
            }
        }
        public static AutodotsShapeHelper autodotsShapeHelper
        {
            get
            {
                return ProgramCore.Project.RenderMainHelper.autodotsShapeHelper;
            }
        }

        public Dictionary<Guid, PartMorphInfo> OldMorphing = null;
        public Dictionary<Guid, PartMorphInfo> FatMorphing = null;
        #endregion

        public ObjCreator()
        {
        }

        private static FSDK.TPoint[] GetFeaturePoints(Image sourceImage)
        {
            FSDK.TPoint[] pointFeature = null;
            var image = new FSDK.CImage(new Bitmap(sourceImage));

            var faceRectangle = Rectangle.Empty;
            var facePosition = image.DetectFace();
            if (0 == facePosition.w)
                return pointFeature;

            pointFeature = image.DetectFacialFeaturesInRegion(ref facePosition);
            return pointFeature;
        }

        public float GetMinX(Vector2 TopFace, Vector2 MiddleFace1, Vector2 MiddleFace2)
        {
            var minX = float.MaxValue;
            minX = Math.Min(TopFace.X, minX);
            minX = Math.Min(MiddleFace1.X, minX);
            minX = Math.Min(MiddleFace2.X, minX);
            return minX;
        }

        public float GetMaxX(Vector2 TopFace, Vector2 RightMiddleFace1, Vector2 RightMiddleFace2)
        {
            var maxX = float.MinValue;
            maxX = Math.Max(TopFace.X, maxX);
            maxX = Math.Max(RightMiddleFace1.X, maxX);
            maxX = Math.Max(RightMiddleFace2.X, maxX);
            return maxX;
        }


        private void Recognize(Bitmap sourceImage)
        {
           var facialFeatures = new List<Vector3>();
            var pointDepths = LuxandFaceRecognition.GetPointDepths();
            int index = 0;
            var pointFeature = GetFeaturePoints(sourceImage);
            foreach (var point in pointFeature)
                facialFeatures.Add(new Vector3(point.x / (sourceImage.Width * 1f), point.y / (sourceImage.Height * 1f), pointDepths[index++]));

            var LeftEyeCenter = new Vector2(pointFeature[0].x, pointFeature[0].y);
            LeftEyeCenter = new Vector2(LeftEyeCenter.X / (sourceImage.Width * 1f), LeftEyeCenter.Y / (sourceImage.Height * 1f));

            var RightEyeCenter = new Vector2(pointFeature[1].x, pointFeature[1].y);
            RightEyeCenter = new Vector2(RightEyeCenter.X / (sourceImage.Width * 1f), RightEyeCenter.Y / (sourceImage.Height * 1f));

            var LeftMouth = new Vector2(pointFeature[3].x, pointFeature[3].y);
            var RightMouth = new Vector2(pointFeature[4].x, pointFeature[4].y);
            LeftMouth = new Vector2(LeftMouth.X / (sourceImage.Width * 1f), LeftMouth.Y / (sourceImage.Height * 1f));
            RightMouth = new Vector2(RightMouth.X / (sourceImage.Width * 1f), RightMouth.Y / (sourceImage.Height * 1f));

            var BottomFace = new Vector2(pointFeature[11].x, pointFeature[11].y);
            BottomFace = new Vector2(BottomFace.X / (sourceImage.Width * 1f), BottomFace.Y / (sourceImage.Height * 1f));

            var TopFace = new Vector2(pointFeature[66].x, pointFeature[66].y);
            var MiddleFace1 = new Vector2(pointFeature[66].x, pointFeature[66].y);
            var MiddleFace2 = new Vector2(pointFeature[5].x, pointFeature[5].y);
            TopFace = new Vector2(TopFace.X / (sourceImage.Width * 1f), TopFace.Y / (sourceImage.Height * 1f));
            MiddleFace1 = new Vector2(MiddleFace1.X / (sourceImage.Width * 1f), MiddleFace1.Y / (sourceImage.Height * 1f));
            MiddleFace2 = new Vector2(MiddleFace2.X / (sourceImage.Width * 1f), MiddleFace2.Y / (sourceImage.Height * 1f));

            var RightMiddleFace1 = new Vector2(pointFeature[67].x, pointFeature[67].y);
            var RightMiddleFace2 = new Vector2(pointFeature[6].x, pointFeature[6].y);

            RightMiddleFace1 = new Vector2(RightMiddleFace1.X / (sourceImage.Width * 1f), RightMiddleFace1.Y / (sourceImage.Height * 1f));
            RightMiddleFace2 = new Vector2(RightMiddleFace2.X / (sourceImage.Width * 1f), RightMiddleFace2.Y / (sourceImage.Height * 1f));


            var distance = facialFeatures[2].Y - facialFeatures[11].Y;     // а хз. старикан попросил чуть ниже брать, но не объяснил как.
            var topPoint = facialFeatures[16].Y + distance;

            var minX = GetMinX(TopFace, MiddleFace1, MiddleFace2);

            ProgramCore.Project.FaceRectRelative = new RectangleF(minX, topPoint, GetMaxX(TopFace, RightMiddleFace1, RightMiddleFace2) - minX, BottomFace.Y - topPoint);
            ProgramCore.Project.MouthCenter = new Vector2(LeftMouth.X + (RightMouth.X - LeftMouth.X) * 0.5f, LeftMouth.Y + (RightMouth.Y - LeftMouth.Y) * 0.5f);

            ProgramCore.Project.LeftEyeCenter = LeftEyeCenter;
            ProgramCore.Project.RightEyeCenter = RightEyeCenter;
            ProgramCore.Project.FaceColor = LuxandFaceRecognition.GetFaceColor(sourceImage, pointFeature);

            ProgramCore.Project.DetectedLipsPoints.Clear();
            ProgramCore.Project.DetectedNosePoints.Clear();
            ProgramCore.Project.DetectedBottomPoints.Clear();
            ProgramCore.Project.DetectedTopPoints.Clear();

            ProgramCore.Project.DetectedLipsPoints.Add(facialFeatures[3]);            // точки рта
            ProgramCore.Project.DetectedLipsPoints.Add(facialFeatures[58]);
            ProgramCore.Project.DetectedLipsPoints.Add(facialFeatures[55]);
            ProgramCore.Project.DetectedLipsPoints.Add(facialFeatures[59]);
            ProgramCore.Project.DetectedLipsPoints.Add(facialFeatures[4]);
            ProgramCore.Project.DetectedLipsPoints.Add(facialFeatures[57]);
            ProgramCore.Project.DetectedLipsPoints.Add(facialFeatures[56]);

            ProgramCore.Project.DetectedLipsPoints.Add(facialFeatures[61]);//Центр рта верх
            ProgramCore.Project.DetectedLipsPoints.Add(facialFeatures[64]);//Центр рта низ

            ProgramCore.Project.DetectedLipsPoints.Add(facialFeatures[60]);  //9
            ProgramCore.Project.DetectedLipsPoints.Add(facialFeatures[62]);  //10

            ProgramCore.Project.DetectedLipsPoints.Add(facialFeatures[63]);  //11
            ProgramCore.Project.DetectedLipsPoints.Add(facialFeatures[65]);  //12

            ProgramCore.Project.DetectedNosePoints.Add(facialFeatures[45]);           // точки носа
            ProgramCore.Project.DetectedNosePoints.Add(facialFeatures[46]);
            ProgramCore.Project.DetectedNosePoints.Add(facialFeatures[2]);
            ProgramCore.Project.DetectedNosePoints.Add(facialFeatures[22]);
            ProgramCore.Project.DetectedNosePoints.Add(facialFeatures[49]);

            ProgramCore.Project.DetectedLeftEyePoints.Add(facialFeatures[23]); //Точки левого глаза
            ProgramCore.Project.DetectedLeftEyePoints.Add(facialFeatures[28]);
            ProgramCore.Project.DetectedLeftEyePoints.Add(facialFeatures[24]);
            ProgramCore.Project.DetectedLeftEyePoints.Add(facialFeatures[27]);

            ProgramCore.Project.DetectedRightEyePoints.Add(facialFeatures[25]); //Точки правого глаза
            ProgramCore.Project.DetectedRightEyePoints.Add(facialFeatures[32]);
            ProgramCore.Project.DetectedRightEyePoints.Add(facialFeatures[26]);
            ProgramCore.Project.DetectedRightEyePoints.Add(facialFeatures[31]);

            ProgramCore.Project.DetectedBottomPoints.Add(facialFeatures[5]); //точки нижней части лица
            ProgramCore.Project.DetectedBottomPoints.Add(facialFeatures[7]);// * 0.75f + facialFeatures[9] * 0.25f);
            var p11 = facialFeatures[11];
            ProgramCore.Project.DetectedBottomPoints.Add(new Vector3((p11.X + facialFeatures[9].X) * 0.5f, p11.Y, facialFeatures[9].Z));
            ProgramCore.Project.DetectedBottomPoints.Add(new Vector3((p11.X + facialFeatures[10].X) * 0.5f, p11.Y, facialFeatures[10].Z));
            ProgramCore.Project.DetectedBottomPoints.Add(facialFeatures[8]);// * 0.75f + facialFeatures[10] * 0.25f);
            ProgramCore.Project.DetectedBottomPoints.Add(facialFeatures[6]);

            ProgramCore.Project.DetectedBottomPoints.Add(facialFeatures[66]);
            ProgramCore.Project.DetectedBottomPoints.Add(facialFeatures[68]);
            ProgramCore.Project.DetectedBottomPoints.Add(facialFeatures[69]);
            ProgramCore.Project.DetectedBottomPoints.Add(facialFeatures[67]);

            ProgramCore.Project.DetectedTopPoints.Add(facialFeatures[66]);
            ProgramCore.Project.DetectedTopPoints.Add(facialFeatures[67]);
        }

        ///  Путь на волосы и аксессуары приходит в виде ссылке на картинку, Там же с тем же названием должен лежать обж.
        /// Мне проще обработать такие ссылки тут, чем в яве
        ///   							<img src=\"http://printahead.net/printahead.online/Library/Hair/Standard/20.jpg\" </summary>
        private string GetParcedHairAccessoriesLink(string uri, string extension)
        {
            if (string.IsNullOrEmpty(uri))
                return string.Empty;

            if (uri.Trim().StartsWith(@"ftp://108.167.164.209/public_html/printahead.online/"))
                return uri.Trim();

            var paths = uri.Trim().Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);
            if (paths.Length != 0)
            {
                var hairObjPath = paths[1].Trim();
                hairObjPath = Path.GetDirectoryName(hairObjPath) + "/" + Path.GetFileNameWithoutExtension(hairObjPath) + extension;
                hairObjPath = hairObjPath.Replace(@"\", "/");
                if (hairObjPath.StartsWith(@"http:/printahead.net/"))
                    hairObjPath = hairObjPath.Replace(@"http:/printahead.net/", @"ftp://108.167.164.209/public_html/");
                if (hairObjPath.StartsWith(@"https:/printahead.net/"))
                    hairObjPath = hairObjPath.Replace(@"https:/printahead.net/", @"ftp://108.167.164.209/public_html/");

                return hairObjPath;
            }
            return string.Empty;
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

        public string Test()
        {
            string result = "";

            var path = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "bin");

            foreach (var file in Directory.GetDirectories(path))
                result += file + Environment.NewLine;
            result += "_____";

            foreach (var file in Directory.GetFiles(path))
                result += file + Environment.NewLine;

            return Environment.GetEnvironmentVariable("Path");
            //  return  Environment.GetEnvironmentVariable("Path"); //System.Reflection.Assembly.GetExecutingAssembly().Location;
        }
        /// <summary>
        ///  Путь на волосы и аксессуары приходит в виде ссылке на картинку, Там же с тем же названием должен лежать обж.
        /// Мне проще обработать такие ссылки тут, чем в яве
        ///   							<img src=\"http://printahead.net/printahead.online/Library/Hair/Standard/20.jpg\" </summary>
        /// <param name="manTypeInt"></param>
        /// <param name="sessionID"></param>
        /// <param name="hairPath"></param>
        /// <param name="hairMaterialPath"></param>
        /// <param name="accessoryPath"></param>
        /// <param name="accessoryMaterialPath"></param>
        /// <param name="size">96% (3.2"), 113%(3.8"), 134%(4.5") ( 1 это 3.2, 2 - 3.8 дюйма и т.п.</param>
        public void CreateObj(int manTypeInt, string sessionID, string hairPath, string hairMaterialPath, string accessoryPath, string accessoryMaterialPath, string basePath, string baseMaterialPath, string addonPath1, string addonPath2, string addonPath3, string addonPath4, string addonMaterialPath, int oldMorphingValue, int fatMorphingValue, int smoothingValue, int size, string ftpOutputName)
        {
            var manType = ManType.Male;
            switch (manTypeInt)
            {
                case 1:
                    manType = ManType.Female;
                    break;
                case 2:
                case 3:
                    manType = ManType.Child;
                    break;
            }

            #region Создание проекта

            WebClient client = new WebClient();

            var imagePath = "https://printahead.net/printahead.online/PrintAhead_images/" + sessionID + ".jpeg";
            byte[] imageBytes = client.DownloadData(imagePath);

            var ms = new MemoryStream(imageBytes);
            var templateImage = new Bitmap(ms);


            ProgramCore.Project = new Project(sessionID, null, null, GenesisType.Genesis2, manType, null, false, 1024, false);
            ProgramCore.Project.FrontImage = templateImage;
            //ProgramCore.Project.LoadMeshes();
            #endregion

            #region активация охуенной распознавалки

            if (FSDK.FSDKE_OK != FSDK.ActivateLibrary("DWysHuomlBcczVM2MQfiz/3WraXb7r+fM0th71X5A9z+gsHn2kpGOgWrVh9D/9sQWlPXO00CFmGMvetl9A+VEr9Y5GVBIccyV32uaZutZjKYH5KB2k87NJAAw6NPkzK0DSQ5b5W7EO0yg2+x4HxpWzPogGyIIYcAHIYY11/YGsU="))
            {
                MessageBox.Show("Please run the License Key Wizard (Start - Luxand - FaceSDK - License Key Wizard)", "Error activating FaceSDK", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            FSDK.InitializeLibrary();
            FSDK.SetFaceDetectionParameters(true, true, 384);

            #endregion

            Recognize(templateImage);
            var aabb = ProgramCore.Project.RenderMainHelper.InitializeShapedotsHelper(true);
            ProgramCore.Project.RenderMainHelper.LoadProject(true, aabb, imagePath, false);

            headMeshesController.InitializeTexturing(autodotsShapeHelper.GetBaseDots(), HeadController.GetIndices());
            autodotsShapeHelper.Transform(headMeshesController.TexturingInfo.Points.ToArray());
            headController.StartAutodots();
            ProgramCore.Project.RenderMainHelper.UpdateUserCenterPositions();

            for (var i = 0; i < headMeshesController.RenderMesh.Parts.Count; i++)
            {
                var part = headMeshesController.RenderMesh.Parts[i];
                if (part.Texture == 0)
                {
                    part.Texture = ProgramCore.Project.RenderMainHelper.HeadTextureId;
                    part.TextureName = ProgramCore.Project.RenderMainHelper.GetTexturePath(part.Texture);
                }
            }

            headController.EndAutodots();

            #region Загружаем и применяем морфинги

            #region Старость

            if (oldMorphingValue != 20)          // назачем морфить, если по дефолту итак так. чтобы время на загрузку не тратить
            {
                var intTemp = 0;

                var oldMorphingPath = "ftp://108.167.164.209/public_html/printahead.online/PrintAhead_DefaultModels/" + manType.GetObjDirPath(false) + "/Old.obj";       // загружаем трансформации для старения
                oldMorphingPath = oldMorphingPath.Replace(@"\", "/");
                if (FTPHelper.IsFileExists(oldMorphingPath))
                    OldMorphing = ProgramCore.Project.RenderMainHelper.pickingController.LoadPartsMorphInfo(oldMorphingPath, headMeshesController.RenderMesh, ref intTemp);

                if (OldMorphing != null)
                {
                    var delta = oldMorphingValue == 20 ? 0 : oldMorphingValue / 80f;
                    foreach (var m in OldMorphing)
                        m.Value.Delta = delta;
                    ProgramCore.Project.AgeCoefficient = delta;
                }
            }

            #endregion

            #region Толстость

            if (fatMorphingValue != 0)
            {
                var intTemp = 0;
                var fatMorphingPath = "ftp://108.167.164.209/public_html/printahead.online/PrintAhead_DefaultModels/" + manType.GetObjDirPath(false) + "/Fat.obj";  // загружаем трансформации для толстения
                fatMorphingPath = fatMorphingPath.Replace(@"\", "/");
                if (FTPHelper.IsFileExists(fatMorphingPath))
                    FatMorphing = ProgramCore.Project.RenderMainHelper.pickingController.LoadPartsMorphInfo(fatMorphingPath, headMeshesController.RenderMesh, ref intTemp);

                if (FatMorphing != null)
                {
                    var delta = fatMorphingValue == 0 ? 0 : fatMorphingValue / 30f;
                    foreach (var m in FatMorphing)
                        m.Value.Delta = delta;
                    ProgramCore.Project.FatCoefficient = delta;
                }
            }

            #endregion

            float? k = null;
            if (smoothingValue != 0)
            {
                var delta = (100 - smoothingValue) / 100f;

                ProgramCore.Project.MorphingScale = delta;
                k = delta;
            }

            DoMorth(k);

            #endregion

            FTPHelper.UpdateAddress(@"ftp://108.167.164.209/public_html/printahead.online/PrintAhead_models/" + sessionID);

            ProgramCore.Project.RenderMainHelper.InitializeHairPositions();
            ProgramCore.Project.RenderMainHelper.InitializeAccessoryPositions();

            ZipOutputStream zipStream = null;

            MemoryStream outputMemStream = new MemoryStream();
            if (!string.IsNullOrEmpty(ftpOutputName))
            {
                zipStream = new ZipOutputStream(outputMemStream); // заодно все будем паковать в архивчик
                zipStream.SetLevel(3);
            }

            #region Attach hair

            var hairObjPath = string.Empty;
            if (string.IsNullOrEmpty(hairPath))
            {
                switch (manTypeInt)
                {
                    case 0:
                        hairObjPath = @"ftp://108.167.164.209/public_html/printahead.online/Library/Hair/Standard/20.obj";      // мужик
                        break;
                    case 1:
                        hairObjPath = @"ftp://108.167.164.209/public_html/printahead.online/Library/Hair/Standard/3.obj";       // баба
                        break;
                    case 2:
                        hairObjPath = @"ftp://108.167.164.209/public_html/printahead.online/Library/Hair/Standard/10C.obj";     // мальчик
                        break;
                    case 3:
                        hairObjPath = @"ftp://108.167.164.209/public_html/printahead.online/Library/Hair/Standard/12C.obj";     // девочка
                        break;
                }
            }
            else
                hairObjPath = GetParcedHairAccessoriesLink(hairPath, manType == ManType.Child ? "C.obj" : ".obj");

            if (!string.IsNullOrEmpty(hairObjPath) && FTPHelper.IsFileExists(hairObjPath))
            {
                hairMaterialPath = GetParcedHairAccessoriesLink(hairMaterialPath, "T.jpg").Replace("/Icons/", "/Materials/");
                if (string.IsNullOrEmpty(hairMaterialPath))
                    hairMaterialPath = "ftp://108.167.164.209/public_html/printahead.online/Library/Hair/Materials/H7T.jpg";

                var temp = @"ftp://108.167.164.209/public_html/printahead.online/PrintAhead_models/" + sessionID + "/Textures";
                var fileName = Path.GetFileNameWithoutExtension(hairMaterialPath) + ".jpg";

                FTPHelper.CopyFromFtpToFtp(hairMaterialPath, temp, fileName, zipStream, fileName);
                hairMaterialPath = @"ftp://108.167.164.209/public_html/printahead.online/PrintAhead_models/" + sessionID + "/Textures/" + fileName;

                ProgramCore.Project.RenderMainHelper.AttachHair(hairObjPath, hairMaterialPath, manType);
            }

            #endregion

            #region Attach accessories

            var accessoryObjPath = string.Empty;
            if (string.IsNullOrEmpty(accessoryPath))
            {
                switch (manType)
                {
                    case ManType.Child:
                        accessoryObjPath = @"ftp://108.167.164.209/public_html/printahead.online/Library/Accessory/Standard/I.obj";
                        break;
                    case ManType.Female:
                        accessoryObjPath = @"ftp://108.167.164.209/public_html/printahead.online/Library/Accessory/Standard/HF.obj";
                        break;
                    default:
                        accessoryObjPath = @"ftp://108.167.164.209/public_html/printahead.online/Library/Accessory/Standard/HM.obj";
                        break;
                }
            }
            else
                accessoryObjPath = GetParcedHairAccessoriesLink(accessoryPath, manType == ManType.Child ? "C.obj" : ".obj");

            if (!string.IsNullOrEmpty(accessoryObjPath) && FTPHelper.IsFileExists(accessoryObjPath))
            {
                accessoryMaterialPath = GetParcedHairAccessoriesLink(accessoryMaterialPath, "T.jpg").Replace("/Icons/", "/Materials/"); ;
                if (string.IsNullOrEmpty(accessoryMaterialPath))
                    accessoryMaterialPath = "ftp://108.167.164.209/public_html/printahead.online/Library/Accessory/Materials/lgreenT.jpg";

                var temp = @"ftp://108.167.164.209/public_html/printahead.online/PrintAhead_models/" + sessionID + "/Textures";
                var fileName = Path.GetFileNameWithoutExtension(accessoryMaterialPath) + ".jpg";

                FTPHelper.CopyFromFtpToFtp(accessoryMaterialPath, temp, fileName, zipStream, fileName);
                accessoryMaterialPath = @"ftp://108.167.164.209/public_html/printahead.online/PrintAhead_models/" + sessionID + "/Textures/" + fileName;

                ProgramCore.Project.RenderMainHelper.AttachAccessory(accessoryObjPath, accessoryMaterialPath, manType);
            }

            #region Base

            var baseObjPath = string.Empty;
            if (!string.IsNullOrEmpty(basePath))
                baseObjPath = GetParcedHairAccessoriesLink(basePath, manType == ManType.Child ? "C.obj" : ".obj");

            if (!string.IsNullOrEmpty(baseObjPath) && FTPHelper.IsFileExists(baseObjPath))
            {
                baseMaterialPath = GetParcedHairAccessoriesLink(baseMaterialPath, "T.jpg").Replace("/Icons/", "/Materials/");
                if (string.IsNullOrEmpty(baseMaterialPath))
                    baseMaterialPath = "ftp://108.167.164.209/public_html/printahead.online/Library/Accessory/Materials/lgreenT.jpg";

                var temp = @"ftp://108.167.164.209/public_html/printahead.online/PrintAhead_models/" + sessionID + "/Textures";
                var fileName = Path.GetFileNameWithoutExtension(baseMaterialPath) + ".jpg";

                FTPHelper.CopyFromFtpToFtp(baseMaterialPath, temp, fileName, zipStream, fileName);
                baseMaterialPath = @"ftp://108.167.164.209/public_html/printahead.online/PrintAhead_models/" + sessionID + "/Textures/" + fileName;

                ProgramCore.Project.RenderMainHelper.AttachAccessory(baseObjPath, baseMaterialPath, manType);
            }

            #endregion

            #region Addons

            var addonObjPath = GetParcedHairAccessoriesLink(addonPath1, manType == ManType.Child ? "C.obj" : ".obj");
            AttachAddon(addonObjPath, ref addonMaterialPath, sessionID, manType, ref zipStream);

            addonObjPath = GetParcedHairAccessoriesLink(addonPath2, manType == ManType.Child ? "C.obj" : ".obj");
            AttachAddon(addonObjPath, ref addonMaterialPath, sessionID, manType, ref zipStream);

            addonObjPath = GetParcedHairAccessoriesLink(addonPath3, manType == ManType.Child ? "C.obj" : ".obj");
            AttachAddon(addonObjPath, ref addonMaterialPath, sessionID, manType, ref zipStream);

            addonObjPath = GetParcedHairAccessoriesLink(addonPath4, manType == ManType.Child ? "C.obj" : ".obj");
            AttachAddon(addonObjPath, ref addonMaterialPath, sessionID, manType, ref zipStream);

            #endregion

            #endregion


            ProgramCore.Project.RenderMainHelper.SaveMergedHead(sessionID, zipStream, size);
            ProgramCore.Project.RenderMainHelper.SaveSmoothedTextures(zipStream);


            var address = "ftp://108.167.164.209/public_html/printahead.online/PrintAhead_models/" + ProgramCore.Project.ProjectName + "/Textures";
            var profileImgPath = sessionID + ".jpeg";

            var ftpHelper = new FTPHelper(address);
            var stream = new MemoryStream();
            templateImage.Save(stream, ImageFormat.Jpeg);
            ftpHelper.Upload(stream, profileImgPath);

            if (zipStream != null)
            {
                ms.Seek(0, SeekOrigin.Begin);
                var newEntry = new ZipEntry(profileImgPath);
                zipStream.PutNextEntry(newEntry);
                ms.CopyTo(zipStream);
                zipStream.CloseEntry();

                zipStream.IsStreamOwner = false;    // False stops the Close also Closing the underlying stream.
                zipStream.Close();          // Must finish the ZipOutputStream before using outputMemStream.

                outputMemStream.Position = 0;
                address = "ftp://108.167.164.209/public_html/printahead.online/PrintAhead_output/";
                ftpHelper = new FTPHelper(address);
                ftpHelper.Upload(outputMemStream, ftpOutputName + ".zip");
            }
        }

        private bool isAddonMaterialActivated = false;
        private void AttachAddon(string addonObjPath, ref string addonMaterialPath, string sessionID, ManType manType, ref ZipOutputStream zipStream)
        {
            if (!string.IsNullOrEmpty(addonObjPath) && FTPHelper.IsFileExists(addonObjPath))
            {
                if (!isAddonMaterialActivated)
                {
                    addonMaterialPath = GetParcedHairAccessoriesLink(addonMaterialPath, "T.jpg").Replace("/Icons/", "/Materials/"); ;
                    if (!string.IsNullOrEmpty(addonMaterialPath))
                    {
                        var temp = @"ftp://108.167.164.209/public_html/printahead.online/PrintAhead_models/" + sessionID + "/Textures";
                        var fileName = Path.GetFileNameWithoutExtension(addonMaterialPath) + ".jpg";

                        FTPHelper.CopyFromFtpToFtp(addonMaterialPath, temp, fileName, zipStream, fileName);
                        addonMaterialPath = @"ftp://108.167.164.209/public_html/printahead.online/PrintAhead_models/" + sessionID + "/Textures/" + fileName;
                        isAddonMaterialActivated = true;
                    }
                }

                ProgramCore.Project.RenderMainHelper.AttachAccessory(addonObjPath, addonMaterialPath, manType);
            }
        }

        public static double GetFaceAngle(string sessionId)
        {
            using (WebClient client = new WebClient())
            {
                var imagePath = "https://printahead.net/printahead.online/PrintAhead_images/" + sessionId + ".jpeg";
                byte[] imageBytes = client.DownloadData(imagePath);

                using (var ms = new MemoryStream(imageBytes))
                {

                    var img = new Bitmap(ms);

                    CropHelper.ActivateRecognition();

                    var points = GetFeaturePoints(img);
                    var detectedNosePoints = new List<Vector2>();
                    detectedNosePoints.Add(new Vector2(points[45].x / (img.Width * 1f), points[45].y / (img.Height * 1f)));
                    detectedNosePoints.Add(new Vector2(points[46].x / (img.Width * 1f), points[46].y / (img.Height * 1f)));
                    detectedNosePoints.Add(new Vector2(points[2].x / (img.Width * 1f), points[2].y / (img.Height * 1f)));
                    detectedNosePoints.Add(new Vector2(points[22].x / (img.Width * 1f), points[22].y / (img.Height * 1f)));
                    detectedNosePoints.Add(new Vector2(points[49].x / (img.Width * 1f), points[49].y / (img.Height * 1f)));

                    /*    var noseTip = MirroredHeadPoint.GetFrontWorldPoint(detectedNosePoints[2], ProgramCore.CurrentProgram);

                        var noseTop = MirroredHeadPoint.GetFrontWorldPoint(detectedNosePoints[3], ProgramCore.CurrentProgram);*/
                    var noseTop = detectedNosePoints[3];
                    var noseTip = detectedNosePoints[2];
                    var noseLength = (noseTop.Y - noseTip.Y) * (float)Math.Tan(35.0 * Math.PI / 180.0);
                    var angle = Math.Asin(Math.Abs(noseTip.X - noseTop.X) / noseLength);

                    angle = angle * (180d / Math.PI);
                    return Math.Abs(angle);
                }
            }

        }
    }
}
