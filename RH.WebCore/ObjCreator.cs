using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using OpenTK;
using RH.Core;
using RH.Core.Helpers;
using RH.Core.IO;
using RH.Core.Render;
using RH.MeshUtils.Data;
using RH.MeshUtils.Helpers;
using RH.Core.Render.Controllers;
using RH.MeshUtils;
using Luxand;
using System.Windows.Forms;
using RH.Core.Render.Obj;
using System.Collections.Generic;

namespace RH.WebCore
{
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
        #endregion

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
            var pointFeature = GetFeaturePoints(sourceImage);
            var facialFeatures = new List<Vector2>();
            foreach (var point in pointFeature)
                facialFeatures.Add(new Vector2(point.x / (sourceImage.Width * 1f), point.y / (sourceImage.Height * 1f)));

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


            var distance = facialFeatures[2].Y - facialFeatures[11].Y;
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
            ProgramCore.Project.DetectedLipsPoints.Add(facialFeatures[61]);

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
            ProgramCore.Project.DetectedBottomPoints.Add(new Vector2((p11.X + facialFeatures[9].X) * 0.5f, p11.Y));
            ProgramCore.Project.DetectedBottomPoints.Add(new Vector2((p11.X + facialFeatures[10].X) * 0.5f, p11.Y));
            ProgramCore.Project.DetectedBottomPoints.Add(facialFeatures[8]);// * 0.75f + facialFeatures[10] * 0.25f);
            ProgramCore.Project.DetectedBottomPoints.Add(facialFeatures[6]);

            ProgramCore.Project.DetectedBottomPoints.Add(facialFeatures[66]);
            ProgramCore.Project.DetectedBottomPoints.Add(facialFeatures[68]);
            ProgramCore.Project.DetectedBottomPoints.Add(facialFeatures[69]);
            ProgramCore.Project.DetectedBottomPoints.Add(facialFeatures[67]);

            ProgramCore.Project.DetectedTopPoints.Add(facialFeatures[66]);
            ProgramCore.Project.DetectedTopPoints.Add(facialFeatures[67]);
        }

        public void CreateObj(int manTypeInt, string imagePath, string sessionID)
        {
            var manType = ManType.Male;
            switch (manTypeInt)
            {
                case 1:
                    manType = ManType.Female;
                    break;
                case 2:
                    manType = ManType.Child;
                    break;
            }

            #region Создание проекта

            //var path = Application.ExecutablePath; // UserConfig.AppDataDir;
            //path = Path.Combine(path, "Temp", sessionID);
            //FolderEx.CreateDirectory(path, true);

            var templateImage = default(Bitmap);
            using (WebClient client = new WebClient())
            {
                byte[] imageBytes = client.DownloadData(imagePath);

                using (var ms = new MemoryStream(imageBytes))
                    templateImage = new Bitmap(ms);
            }

            ProgramCore.Project = new Project(sessionID, null, null, manType, null, false, 1024);
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
            ProgramCore.Project.RenderMainHelper.LoadProject(true, aabb, imagePath);

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

            //string fiName = Path.Combine(path, ProgramCore.Project.ProjectName + ".obj");
            
            ProgramCore.Project.RenderMainHelper.SaveHead("");
            ProgramCore.Project.RenderMainHelper.SaveSmoothedTextures();

            //FTPHelper ftpHelper = new FTPHelper(@"ftp://108.167.164.209/public_ftp/PrintAhead_models/" + sessionID, "i2q1d8b1", "B45B2nnFv$!j6V");
            //foreach (var file in Directory.GetFiles(path))              // сохраняем все папки
            //    ftpHelper.Upload(file, Path.GetFileName(file));

            //foreach (var directory in Directory.GetDirectories(path))
            //{
            //    var fullPath = Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar);
            //    var lastDirectory = Path.GetFileName(fullPath);

            //    ftpHelper.Address = @"ftp://108.167.164.209/public_ftp/PrintAhead_models/" + sessionID + "/" + lastDirectory;
            //    foreach (var file in Directory.GetFiles(directory)) // и все папки, вложенностью = 1
            //        ftpHelper.Upload(file, Path.GetFileName(file));
            //}
        }
    }
}
