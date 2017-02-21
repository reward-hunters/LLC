using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using RH.Core;
using RH.Core.Helpers;
using RH.Core.IO;
using RH.Core.Render;
using RH.Core.Render.Controllers;

namespace RH.WebCore
{
    public static class ObjCreator
    {
        public static void CreateObj(ManType manType)
        {
            var render = new ctrlRenderControl();
            render.Initialize();

            #region Создание проекта

            var path = UserConfig.AppDataDir;
            path = Path.Combine(path, "TempProject");
            FolderEx.CreateDirectory(path, true);

            var templateImage = Path.Combine(path, "sourceImg.jpeg");
            using (WebClient client = new WebClient())
            {
                byte[] imageBytes = client.DownloadData(path);

                using (var ms = new MemoryStream(imageBytes))
                {
                    var img = new Bitmap(ms);
                    img.Save(templateImage);
                }
            }

            ProgramCore.Project = new Project("PrintAheadProject", path, templateImage, manType, string.Empty, true, 1024);

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

            var aabb = ProgramCore.MainForm.ctrlRenderControl.InitializeShapedotsHelper(true);         // инициализация точек головы. эта инфа тоже сохранится в проект
            ProgramCore.MainForm.UpdateProjectControls(true, aabb);

            ProgramCore.Project.ToStream();


            #endregion

            render.LoadProject(true, aabb);

            #region Texture

            // Start Autodots

            render.headMeshesController.InitializeTexturing(render.autodotsShapeHelper.GetBaseDots(), HeadController.GetIndices());
            render.autodotsShapeHelper.Transform(render.headMeshesController.TexturingInfo.Points.ToArray());
            render.headController.StartAutodots();

            // End  Autodots

            for (var i = 0; i < render.headMeshesController.RenderMesh.Parts.Count; i++)
            {
                var part = render.headMeshesController.RenderMesh.Parts[i];
                if (part.Texture == 0)
                {
                    part.Texture = render.HeadTextureId;
                    part.TextureName = render.GetTexturePath(part.Texture);
                }
            }

            render.headController.EndAutodots();
            render.ApplySmoothedTextures();

            for (var i = 0; i < render.headController.AutoDots.Count; i++)      // после слияние с ShapeDots. Проверить!
            {
                var p = render.headController.AutoDots[i];
                render.autodotsShapeHelper.Transform(p.Value, i); // точка в мировых координатах
            }
            render.CalcReflectedBitmaps();

            #endregion 
        }
    }
}
