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

        private void Recognize(string templateImage)
        {
            var fcr = new LuxandFaceRecognition();
            fcr.Recognize(ref templateImage, true);

            var distance = fcr.FacialFeatures[2].Y - fcr.FacialFeatures[11].Y;
            var topPoint = fcr.FacialFeatures[16].Y + distance;
            var minX = fcr.GetMinX();

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
            ProgramCore.Project.DetectedLipsPoints.Add(fcr.FacialFeatures[61]);

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
            ProgramCore.Project.DetectedBottomPoints.Add(new Vector2((p11.X + fcr.FacialFeatures[9].X) * 0.5f, p11.Y));
            ProgramCore.Project.DetectedBottomPoints.Add(new Vector2((p11.X + fcr.FacialFeatures[10].X) * 0.5f, p11.Y));
            ProgramCore.Project.DetectedBottomPoints.Add(fcr.FacialFeatures[8]);// * 0.75f + fcr.FacialFeatures[10] * 0.25f);
            ProgramCore.Project.DetectedBottomPoints.Add(fcr.FacialFeatures[6]);

            ProgramCore.Project.DetectedBottomPoints.Add(fcr.FacialFeatures[66]);
            ProgramCore.Project.DetectedBottomPoints.Add(fcr.FacialFeatures[68]);
            ProgramCore.Project.DetectedBottomPoints.Add(fcr.FacialFeatures[69]);
            ProgramCore.Project.DetectedBottomPoints.Add(fcr.FacialFeatures[67]);

            ProgramCore.Project.DetectedTopPoints.Add(fcr.FacialFeatures[66]);
            ProgramCore.Project.DetectedTopPoints.Add(fcr.FacialFeatures[67]);
        }

        public void UpdateProjectControls(bool newProject, RectangleAABB aabb = null)
        {
            ProgramCore.Project.RenderMainHelper.LoadProject(newProject, aabb);

            //if (ProgramCore.Project == null)
            //{
            //    ProgramCore.MainForm.ctrlTemplateImage.SetTemplateImage(null);
            //}
            //else
            //{



            //    if (ProgramCore.Project.FrontImage == null)
            //        ProgramCore.MainForm.ctrlTemplateImage.SetTemplateImage(null);
            //    else
            //    {
            //        using (var bmp = new Bitmap(ProgramCore.Project.FrontImage))
            //            ProgramCore.MainForm.ctrlTemplateImage.SetTemplateImage((Bitmap)bmp.Clone());
            //    }
            //}
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

            var path = Application.ExecutablePath; // UserConfig.AppDataDir;
            path = Path.Combine(path, "Temp", sessionID);
            FolderEx.CreateDirectory(path, true);

            var templateImage = default(Bitmap);
            using (WebClient client = new WebClient())
            {
                byte[] imageBytes = client.DownloadData(imagePath);

                using (var ms = new MemoryStream(imageBytes))
                    templateImage = new Bitmap(ms);
            }

            ProgramCore.Project = new Project(sessionID, path, string.Empty, manType, string.Empty, false, 1024);
            ProgramCore.Project.LoadMeshes();
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
            ProgramCore.Project.RenderMainHelper.LoadProject(true, aabb);
            string fiName = Path.Combine(path, ProgramCore.Project.ProjectName + ".obj");

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

            ProgramCore.Project.RenderMainHelper.SaveHead(fiName);
            ProgramCore.Project.RenderMainHelper.SaveSmoothedTextures();

            FTPHelper ftpHelper = new FTPHelper(@"ftp://108.167.164.209/public_ftp/PrintAhead_models/" + sessionID, "i2q1d8b1", "B45B2nnFv$!j6V");
            foreach (var file in Directory.GetFiles(path))              // сохраняем все папки
                ftpHelper.Upload(file, Path.GetFileName(file));

            foreach (var directory in Directory.GetDirectories(path))
            {
                var fullPath = Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar);
                var lastDirectory = Path.GetFileName(fullPath);

                ftpHelper.Address = @"ftp://108.167.164.209/public_ftp/PrintAhead_models/" + sessionID + "/" + lastDirectory;
                foreach (var file in Directory.GetFiles(directory)) // и все папки, вложенностью = 1
                    ftpHelper.Upload(file, Path.GetFileName(file));
            }
        }
    }
}
