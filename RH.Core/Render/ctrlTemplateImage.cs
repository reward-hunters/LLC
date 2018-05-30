﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using OpenTK;
using RH.Core.Helpers;
using RH.Core.Properties;
using RH.Core.Render.Controllers;
using RH.Core.Render.Helpers;
using RH.MeshUtils.Helpers;
using RH.MeshUtils.Data;

namespace RH.Core.Render
{
    public partial class ctrlTemplateImage : UserControlEx
    {
        #region Var

        public Vector2 ModelAdaptParam
        {
            get
            {
                var v = new Vector2(pictureTemplate.Width, pictureTemplate.Height);
                var p0 = new Vector2(EyesMouthRectTransformed.X, EyesMouthRectTransformed.Y);
                var p1 = new Vector2(EyesMouthRectTransformed.X + EyesMouthRectTransformed.Width, EyesMouthRectTransformed.Y + EyesMouthRectTransformed.Height);
                return new Vector2(v.Length / (p1 - p0).Length, 1.0f - ((p0.Y + p1.Y) * 0.5f) / v.Y);
            }
        }

        public Vector2 ModelAdaptParamProfile
        {
            get
            {
                var v = new Vector2(pictureTemplate.Width, pictureTemplate.Height);
                var point = ProgramCore.MainForm.ctrlTemplateImage.profileControlPoints[0].ValueMirrored;
                var p1 = new Vector2(point.X * ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateWidth + ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateOffsetX,
                                              point.Y * ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateHeight + ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateOffsetY);
                point = ProgramCore.MainForm.ctrlTemplateImage.profileControlPoints[3].ValueMirrored;
                var p0 = new Vector2(point.X * ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateWidth + ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateOffsetX,
                                              point.Y * ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateHeight + ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateOffsetY);
                return new Vector2(v.Length / (p1 - p0).Length, 1.0f - ((p0.Y + p1.Y) * 0.5f) / v.Y);
            }
        }

        /// <summary> Реальная позиция картинки на контроле </summary>
        private int OriginalImageTemplateWidth;
        private int OriginalImageTemplateHeight;
        private int ImageTemplateCenterX;
        private int ImageTemplateCenterY;
        private int ImageTemplateOldWidth;
        public int ImageTemplateWidth;
        public int ImageTemplateHeight;
        public int ImageTemplateOffsetX;
        public int ImageTemplateOffsetY;

        private bool startMove;
        private Vector2 startMousePoint;
        private bool leftMousePressed;
        private bool shiftKeyPressed;
        private bool dblClick;

        private float imageScale = 1.0f;
        private int moveRectIndex = -1;
        private Vector2 tempMoveRectCenter;         // старые значения прямоугольника, для сжимания-расжимания точек
        private float tempMoveRectWidth;
        private float tempMoveRectHeight;

        private Vector2 headLastPointRelative = Vector2.Zero;
        private Vector2 headLastPoint = Vector2.Zero;
        private Vector2 tempOffsetPoint;

        private readonly List<HeadPoint> headTempPoints = new List<HeadPoint>();

        /// <summary> прямоугольник глаза-рот, Подогнанный под контрол </summary>
        public RectangleF EyesMouthRectTransformed;

        public Vector2 MouthTransformed;
        public Vector2 LeftEyeTransformed;
        public Vector2 RightEyeTransformed;
        public Vector2 NoseTransformed;

        /// <summary> Прямоугольник, охватывающий все автоточки. Нужен для изменения всех точек сразу (сжатие/расжатие) </summary>
        public Rectangle FaceRectTransformed;
        /// <summary> Центральная точка на лбу. Нужна для выделения прямоугольинка автоточек. </summary>
        public Vector2 CentralFacePoint;

        public RectangleF ProfileFaceRect;
        public Rectangle ProfileFaceRectTransformed;

        /// <summary> Точки лассо для автоточек </summary>
        private readonly List<Vector2> headAutodotsLassoPoints = new List<Vector2>();
        /// <summary> Точки лассо для шейпа зеркального </summary>
        private readonly List<Vector2> headShapedotsLassoPoints = new List<Vector2>();

        public bool RectTransformMode;
        public bool LineSelectionMode;     // режим таскания точек при отрисовке линий


        private int profileControlPointIndex = 0;
        public List<MirroredHeadPoint> profileControlPoints = new List<MirroredHeadPoint>();

        public Vector2 ProfileScreenTopLocation = Vector2.Zero;
        public Vector2 ProfileScreenEyeLocation = Vector2.Zero;
        public Vector2 ProfileScreenMouthLocation = Vector2.Zero;
        public Vector2 ProfileScreenBottomLocation = Vector2.Zero;

        public ProfileControlPointsMode ControlPointsMode = ProfileControlPointsMode.None;

        public Image DrawingImage; //!!!

        private const float PointRectSize = 6;
        private const float HalfPointRectSize = 3f;

        internal ProfileSmoothing ProfileSmoothing;
        internal bool isProfileSmoothing;

        #endregion

        public ctrlTemplateImage()
        {
            InitializeComponent();

            PreviewKeyDown += ctrlTemplateImage_PreviewKeyDown;
        }

        /// <summary> Заполнить опорные точки для правой модели. Они константы </summary>
        public void InitializeProfileControlPoints()
        {
            var headController = ProgramCore.Project.RenderMainHelper.headController;
            profileControlPoints.Clear();

            if (headController.ShapeDots.Count == 0)
                return;

            var eyeX = -7.55f;
            var mouthX = -9.2f;
            if (ProgramCore.Project.ManType == ManType.Child)
            {
                eyeX = -7.2f;
                mouthX = -8.2f;
            }
            else if (ProgramCore.Project.ManType == ManType.Female)
            {
                eyeX = -7.8f;
                mouthX = -9.1f;
            }
            profileControlPoints.Add(new MirroredHeadPoint(new Vector2(0.0f, headController.ShapeDots[0].Value.Y), Vector2.Zero, false));       // верх
            profileControlPoints.Add(new MirroredHeadPoint(new Vector2(eyeX, (headController.ShapeDots[21].Value.Y + headController.ShapeDots[23].Value.Y) * 0.5f), Vector2.Zero, false)); // глаз
            profileControlPoints.Add(new MirroredHeadPoint(new Vector2(mouthX, headController.ShapeDots[51].Value.Y), Vector2.Zero, false));      //рот
            profileControlPoints.Add(new MirroredHeadPoint(new Vector2(-3.0f, (headController.ShapeDots[11].Value.Y + headController.ShapeDots[33].Value.Y) * 0.5f), Vector2.Zero, false));

            for (var i = 0; i < 4; i++)
            {
                var sprite = ProgramCore.MainForm.ctrlRenderControl.profilePointSprites[i];
                sprite.Position = new Vector2(-profileControlPoints[i].Value.X, profileControlPoints[i].Value.Y);
            }

        }

        #region Supported void's

        public List<PointF> facialFeaturesTransformed = new List<PointF>();
        /// <summary> Пересчитать положение прямоугольника в зависимост от размера картинки на picturetemplate </summary>
        public void RecalcRealTemplateImagePosition()
        {
            var pb = pictureTemplate;
            if (DrawingImage == null)
            {
                ImageTemplateWidth = ImageTemplateHeight = 0;
                ImageTemplateOffsetX = ImageTemplateOffsetY = -1;
                EyesMouthRectTransformed = RectangleF.Empty;
                return;
            }

            if (pb.Width / (double)pb.Height < DrawingImage.Width / (double)DrawingImage.Height)
            {
                ImageTemplateWidth = pb.Width;
                ImageTemplateHeight = DrawingImage.Height * ImageTemplateWidth / DrawingImage.Width;
            }
            else if (pb.Width / (double)pb.Height > DrawingImage.Width / (double)DrawingImage.Height)
            {
                ImageTemplateHeight = pb.Height;
                ImageTemplateWidth = DrawingImage.Width * ImageTemplateHeight / DrawingImage.Height;
            }
            else
            {
                ImageTemplateWidth = pb.Width;
                ImageTemplateHeight = pb.Height;
            }
            OriginalImageTemplateWidth = ImageTemplateWidth;
            OriginalImageTemplateHeight = ImageTemplateHeight;

            ImageTemplateOffsetX = (pb.Width - ImageTemplateWidth) / 2;
            ImageTemplateOffsetY = (pb.Height - ImageTemplateHeight) / 2;

            facialFeaturesTransformed.Clear();
            foreach (var point in ProgramCore.Project.FacialFeatures)
            {
                var pointTransformed = new PointF(point.X * ImageTemplateWidth + ImageTemplateOffsetX,
                                          point.Y * ImageTemplateHeight + ImageTemplateOffsetY);
                facialFeaturesTransformed.Add(pointTransformed);
            }

            imageScale = 1.0f;

            RecalcEyeMouthRect();
        }

        public void ApplyScale(float s)
        {
            imageScale = s;
            var w = OriginalImageTemplateWidth * imageScale;
            var h = OriginalImageTemplateHeight * imageScale;
            ImageTemplateWidth = (int)w;
            ImageTemplateHeight = (int)h;
            var k = ImageTemplateWidth * 1f / ImageTemplateOldWidth;
            var cx = (int)(ImageTemplateCenterX * k);
            var cy = (int)(ImageTemplateCenterY * k);
            ImageTemplateOffsetX = pictureTemplate.Width / 2 - cx;
            ImageTemplateOffsetY = pictureTemplate.Height / 2 - cy;
        }


        private void RecalcEyeMouthRect()
        {
            EyesMouthRectTransformed = new RectangleF(ProgramCore.Project.FaceRectRelative.X * ImageTemplateWidth + ImageTemplateOffsetX,
                                          ProgramCore.Project.FaceRectRelative.Y * ImageTemplateHeight + ImageTemplateOffsetY,
                                           ProgramCore.Project.FaceRectRelative.Width * ImageTemplateWidth,
                                           ProgramCore.Project.FaceRectRelative.Height * ImageTemplateHeight);

            RecalcUserCenters();
        }
        private void RecalcUserCenters()
        {
            MouthTransformed = new Vector2(ProgramCore.Project.MouthUserCenter.X * ImageTemplateWidth + ImageTemplateOffsetX,
                                         ProgramCore.Project.MouthUserCenter.Y * ImageTemplateHeight + ImageTemplateOffsetY);
            LeftEyeTransformed = new Vector2(ProgramCore.Project.LeftEyeUserCenter.X * ImageTemplateWidth + ImageTemplateOffsetX,
                                           ProgramCore.Project.LeftEyeUserCenter.Y * ImageTemplateHeight + ImageTemplateOffsetY);
            RightEyeTransformed = new Vector2(ProgramCore.Project.RightEyeUserCenter.X * ImageTemplateWidth + ImageTemplateOffsetX,
                                           ProgramCore.Project.RightEyeUserCenter.Y * ImageTemplateHeight + ImageTemplateOffsetY);
            NoseTransformed = new Vector2(ProgramCore.Project.NoseUserCenter.X * ImageTemplateWidth + ImageTemplateOffsetX,
                               ProgramCore.Project.NoseUserCenter.Y * ImageTemplateHeight + ImageTemplateOffsetY);
        }

        /// <summary> Смасштабировать и повернуть изображение профиля по опорным точкам </summary>
        private void UpdateProfileImageByControlPoints()
        {
            #region Обрезание

            var image = ProgramCore.Project.ProfileImage;
            var leftX = (int)(profileControlPoints.Min(x => x.ValueMirrored.X) * image.Width);
            var topY = (int)(profileControlPoints.Min(x => x.ValueMirrored.Y) * image.Height);
            var bottomY = (int)(profileControlPoints.Max(x => x.ValueMirrored.Y) * image.Height);

            leftX = leftX - 100 < 0 ? 0 : leftX - 100;          // ширину хз как определять, ой-вей
            topY = topY - 10 < 0 ? 0 : topY - 10;
            bottomY = bottomY + 10 > image.Height ? image.Height : bottomY + 10;
            var height = bottomY - topY;
            var faceRectangle = new Rectangle(leftX, topY, image.Width - leftX, height);

            var croppedImage = ImageEx.Crop(image, faceRectangle);
            for (var i = 0; i < profileControlPoints.Count; i++)            // смещаем все точки, чтобы учесть обрезанное
            {
                var point = profileControlPoints[i];
                var pointK = new Vector2(point.ValueMirrored.X * image.Width, point.ValueMirrored.Y * image.Height);        // по старой ширине-высоте

                pointK.X -= leftX;
                pointK.Y -= topY;

                profileControlPoints[i] = new MirroredHeadPoint(point.Value, new Vector2(pointK.X / (croppedImage.Width * 1f), pointK.Y / (croppedImage.Height * 1f)), false);    // и в новые
            }

            #endregion

            #region Поворот

            var xVector = new Vector2(1, 0);

            var vectorLeft = profileControlPoints[2].ValueMirrored - profileControlPoints[1].ValueMirrored; // из глаза рот
            vectorLeft = new Vector2(vectorLeft.X * croppedImage.Width, vectorLeft.Y * croppedImage.Height);
            vectorLeft.Normalize();
            var xDiff = xVector.X - vectorLeft.X;
            var yDiff = xVector.Y - vectorLeft.Y;
            var angleLeft = Math.Atan2(yDiff, xDiff);

            var vectorRight = profileControlPoints[2].Value - profileControlPoints[1].Value;
            vectorRight.Normalize();
            xDiff = xVector.X - vectorRight.X;
            yDiff = xVector.Y - vectorRight.Y;
            var angleRight = -Math.Atan2(yDiff, xDiff);

            var angleDiffRad = angleRight - angleLeft;
            var angleDiff = angleDiffRad * 180.0 / Math.PI;

            using (var ii = ImageEx.RotateImage(croppedImage, (float)angleDiff))
            {
                ProgramCore.Project.ProfileImage = new Bitmap(ii);
                SetTemplateImage(ProgramCore.Project.ProfileImage, false);
            }

            var center = new Vector2(ProgramCore.Project.ProfileImage.Width * 0.5f, ProgramCore.Project.ProfileImage.Height * 0.5f);
            var cosAngle = Math.Cos(angleDiffRad);
            var sinAngle = Math.Sin(angleDiffRad);
            for (var i = 0; i < profileControlPoints.Count; i++)            // смещаем все точки, чтобы учесть обрезанное
            {
                var point = profileControlPoints[i];
                var pointAbsolute = new Vector2(point.ValueMirrored.X * ProgramCore.Project.ProfileImage.Width, point.ValueMirrored.Y * ProgramCore.Project.ProfileImage.Height);        // по старой ширине-высоте

                var newPoint = pointAbsolute - center;
                newPoint = new Vector2((float)(newPoint.X * cosAngle - newPoint.Y * sinAngle),
                                           (float)(newPoint.Y * cosAngle + newPoint.X * sinAngle));
                newPoint += center;

                profileControlPoints[i] = new MirroredHeadPoint(point.Value, new Vector2(newPoint.X / (ProgramCore.Project.ProfileImage.Width * 1f), newPoint.Y / (ProgramCore.Project.ProfileImage.Height * 1f)), false);    // и в новые
            }

            ProgramCore.MainForm.ctrlRenderControl.InitializeProfileCamera(ModelAdaptParamProfile);

            #endregion

            var projectPath = Path.Combine(ProgramCore.Project.ProjectPath, "ProfileImage.jpg");
            ProgramCore.Project.ProfileImage.Save(projectPath);

            ControlPointsMode = ProfileControlPointsMode.UpdateRightLeft;
        }

        private void DrawAutodotsGroupPoints(Graphics g)
        {
            var pointRect = new RectangleF(MouthTransformed.X - HalfPointRectSize, MouthTransformed.Y - HalfPointRectSize, PointRectSize, PointRectSize);
            g.FillRectangle(ProgramCore.MainForm.ctrlRenderControl.HeadLineMode == MeshPartType.Lip ? DrawingTools.YellowSolidBrush : DrawingTools.BlueSolidBrush, pointRect);
            pointRect = new RectangleF(NoseTransformed.X - HalfPointRectSize, NoseTransformed.Y - HalfPointRectSize, PointRectSize, PointRectSize);
            g.FillRectangle(ProgramCore.MainForm.ctrlRenderControl.HeadLineMode == MeshPartType.Nose ? DrawingTools.YellowSolidBrush : DrawingTools.BlueSolidBrush, pointRect);

            pointRect = new RectangleF(LeftEyeTransformed.X - HalfPointRectSize, LeftEyeTransformed.Y - HalfPointRectSize, PointRectSize, PointRectSize);
            g.FillRectangle(ProgramCore.MainForm.ctrlRenderControl.HeadLineMode == MeshPartType.LEye ? DrawingTools.YellowSolidBrush : DrawingTools.BlueSolidBrush, pointRect);
            pointRect = new RectangleF(RightEyeTransformed.X - HalfPointRectSize, RightEyeTransformed.Y - HalfPointRectSize, PointRectSize, PointRectSize);
            g.FillRectangle(ProgramCore.MainForm.ctrlRenderControl.HeadLineMode == MeshPartType.REye ? DrawingTools.YellowSolidBrush : DrawingTools.BlueSolidBrush, pointRect);

            pointRect = new RectangleF(CentralFacePoint.X - HalfPointRectSize, CentralFacePoint.Y - HalfPointRectSize, PointRectSize, PointRectSize);
            g.FillRectangle(ProgramCore.MainForm.ctrlRenderControl.HeadLineMode == MeshPartType.Head ? DrawingTools.YellowSolidBrush : DrawingTools.BlueSolidBrush, pointRect);
        }
        public void DrawLassoOnPictureBox(Graphics g, bool autodots)
        {
            var points = autodots ? headAutodotsLassoPoints : headShapedotsLassoPoints;
            for (var i = points.Count - 2; i >= 0; i--)
            {
                var pointA = points[i];
                var pointB = points[i + 1];

                g.DrawLine(DrawingTools.GreenPen, pointA.X, pointA.Y, pointB.X, pointB.Y);
            }
            foreach (var point in points)
            {
                var pointRect = new RectangleF(point.X - 2.5f, point.Y - 2.5f, 5f, 5f);
                g.FillRectangle(DrawingTools.BlueSolidBrush, pointRect);
            }

        }
        public void SetTemplateImage(Bitmap image, bool needCameraInitialize = true)
        {
            DrawingImage = image;
            RecalcRealTemplateImagePosition();

            if (needCameraInitialize)
                ProgramCore.MainForm.ctrlRenderControl.InitialiseCamera(ModelAdaptParam);

            pictureTemplate.Refresh();
        }
        public void RefreshPictureBox()
        {
            pictureTemplate.Refresh();
        }

        public void SelectAutodotsByLasso()
        {
            ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2.ClearSelection();
            foreach (var point in ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2)
                point.CheckLassoSelection(headAutodotsLassoPoints);

            headAutodotsLassoPoints.Clear();
        }
        public void SelectShapedotsByLasso()
        {
            ProgramCore.Project.RenderMainHelper.headController.ShapeDots.ClearSelection();
            foreach (var point in ProgramCore.Project.RenderMainHelper.headController.ShapeDots)
                point.CheckLassoSelection(headShapedotsLassoPoints);

            headShapedotsLassoPoints.Clear();
        }

        public new void KeyDown(KeyEventArgs e)
        {           // приходится так делать, ибо у нас все перекрывается
            if (ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.HeadAutodots)
            {
                if (e.KeyData == (Keys.A))
                    ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2.SelectAll();
                else if (e.KeyData == (Keys.D))
                {
                    ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2.ClearSelection();
                }
            }
        }

        private void UpdateFaceRect()
        {
            var indicies = ProgramCore.Project.RenderMainHelper.headController.GetFaceIndexes();
            List<MirroredHeadPoint> faceDots = new List<MirroredHeadPoint>();
            switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
            {
                // case Mode.HeadShapedots:
                case Mode.HeadLine:
                case Mode.HeadShapeFirstTime:
                case Mode.HeadShape:
                    faceDots = ProgramCore.Project.RenderMainHelper.headController.GetSpecialShapedots(indicies);
                    break;
                default:
                   // faceDots = ProgramCore.Project.RenderMainHelper.headController.GetSpecialAutodots(indicies);
                    break;
            }

            if (faceDots.Count == 0)
                return;
            {
                var minX1 = faceDots.Min(point => point.ValueMirrored.X);
                var maxX1 = faceDots.Max(point => point.ValueMirrored.X);
                var minY1 = faceDots.Min(point => point.ValueMirrored.Y);
                var maxY1 = faceDots.Max(point => point.ValueMirrored.Y);

                var rrr = new RectangleF((float)minX1, (float)minY1, (float)(maxX1 - minX1), (float)(maxY1 - minY1));
            }

            var minX = faceDots.Min(point => point.ValueMirrored.X) * ImageTemplateWidth + ImageTemplateOffsetX;
            var maxX = faceDots.Max(point => point.ValueMirrored.X) * ImageTemplateWidth + ImageTemplateOffsetX;
            var minY = faceDots.Min(point => point.ValueMirrored.Y) * ImageTemplateHeight + ImageTemplateOffsetY;
            var maxY = faceDots.Max(point => point.ValueMirrored.Y) * ImageTemplateHeight + ImageTemplateOffsetY;

            FaceRectTransformed = new Rectangle((int)minX, (int)minY, (int)(maxX - minX), (int)(maxY - minY));

            CentralFacePoint = new Vector2(minX + (maxX - minX) * 0.5f, minY + (maxY - minY) / 3f);
        }
        public void UpdateUserCenterPositions(bool onlySelected, bool updateRect)
        {
            var center = UpdateUserCenterPositions(ProgramCore.Project.RenderMainHelper.headController.GetLeftEyeIndexes(), onlySelected);  // Left eye
            if (center != Vector2.Zero)
                ProgramCore.Project.LeftEyeUserCenter = center;

            center = UpdateUserCenterPositions(ProgramCore.Project.RenderMainHelper.headController.GetRightEyeIndexes(), onlySelected);  // Right eye
            if (center != Vector2.Zero)
                ProgramCore.Project.RightEyeUserCenter = center;

            center = UpdateUserCenterPositions(ProgramCore.Project.RenderMainHelper.headController.GetMouthIndexes(), onlySelected);  // Mouth
            if (center != Vector2.Zero)
                ProgramCore.Project.MouthUserCenter = center;

            center = UpdateUserCenterPositions(ProgramCore.Project.RenderMainHelper.headController.GetNoseIndexes(), onlySelected);  // Nose
            if (center != Vector2.Zero)
                ProgramCore.Project.NoseUserCenter = center;

            #region Определяем прямоугольник, охватывающий все автоточки

            if (updateRect)
                UpdateFaceRect();

            #endregion

            RecalcUserCenters();
        }
        private Vector2 UpdateUserCenterPositions(IEnumerable<int> indexes, bool onlySelected)
        {
            List<MirroredHeadPoint> sourcePoints = new List<MirroredHeadPoint>();
            switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
            {
                //       case Mode.HeadShapedots:
                case Mode.HeadLine:
                case Mode.HeadShapeFirstTime:
                case Mode.HeadShape:
                    sourcePoints = ProgramCore.Project.RenderMainHelper.headController.ShapeDots;
                    break;
                default:
                   sourcePoints = ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2.Select(x=>new MirroredHeadPoint(x.OriginalValue, x.OriginalValue)).ToList();   
                    break;
            }
            if (sourcePoints.Count == 0)
                return Vector2.Zero;

            var hasSelected = false;
            var dots = new List<MirroredHeadPoint>();
            foreach (var index in indexes)
            {
                var dot = sourcePoints[index];
                dots.Add(dot);

                if (!onlySelected || dot.Selected)
                    hasSelected = true;
            }

            if (!hasSelected)
                return Vector2.Zero;

            var minX = dots.Min(point => point.ValueMirrored.X);
            var maxX = dots.Max(point => point.ValueMirrored.X);
            var minY = dots.Min(point => point.ValueMirrored.Y);
            var maxY = dots.Max(point => point.ValueMirrored.Y);

            return new Vector2((maxX + minX) * 0.5f, (maxY + minY) * 0.5f);
        }

        private void UpdateProfileRectangle()
        {
            ProfileFaceRect = ProfileFaceRectTransformed = Rectangle.Empty;
            if (!ProgramCore.MainForm.HeadProfile || profileControlPoints.Count == 0)
                return;

            var pointUp = profileControlPoints[0];
            var pointBottom = profileControlPoints[3];

            var width = Math.Max(pointUp.ValueMirrored.X, pointBottom.ValueMirrored.X) - Math.Min(pointUp.ValueMirrored.X, pointBottom.ValueMirrored.X);
            var height = Math.Max(pointUp.ValueMirrored.Y, pointBottom.ValueMirrored.Y) - Math.Min(pointUp.ValueMirrored.Y, pointBottom.ValueMirrored.Y);

            var center = (pointUp.ValueMirrored + pointBottom.ValueMirrored) * 0.5f;


            ProfileFaceRect = new RectangleF(center.X - width * 0.5f, center.Y - height * 0.5f, width, height);
            ProfileFaceRectTransformed = new Rectangle((int)(ProfileFaceRect.X * ImageTemplateWidth + ImageTemplateOffsetX),
                                                       (int)(ProfileFaceRect.Y * ImageTemplateHeight + ImageTemplateOffsetY),
                                                        (int)(ProfileFaceRect.Width * ImageTemplateWidth),
                                                       (int)(ProfileFaceRect.Height * ImageTemplateHeight));

            if (ProgramCore.Project.RenderMainHelper.headController.Lines.Count == 2)
            {
                var currentLine = ProgramCore.Project.RenderMainHelper.headController.Lines[0];
                foreach (var point in currentLine)
                    point.UpdateWorldPoint();
            }
        }
        public void ResetProfileRects()
        {
            ProfileFaceRectTransformed = Rectangle.Empty;
            ProfileFaceRect = RectangleF.Empty;
        }

        public void FinishLine()
        {
            switch (ProgramCore.MainForm.ctrlRenderControl.HeadLineMode)
            {
                case MeshPartType.Nose:
                    break;
                case MeshPartType.LEye: // все, кроме носа. нос замыкать нельзя!
                case MeshPartType.REye:
                case MeshPartType.Head:
                case MeshPartType.ProfileTop:
                case MeshPartType.ProfileBottom:
                case MeshPartType.Lip:
                case MeshPartType.None:
                    if (ProgramCore.Project.RenderMainHelper.headController.Lines.Count != 0)
                    {
                        if (ProgramCore.MainForm.ctrlRenderControl.HeadLineMode == MeshPartType.Head && ProgramCore.Project.RenderMainHelper.headController.Lines.Count > 1)
                            break; // рисуем вторую линию, ее замыкать нельзя!

                        if (ProgramCore.MainForm.HeadProfile)
                        {
                            if (ProgramCore.Project.RenderMainHelper.headController.Lines.Count == 1)
                            {
                                var line = ProgramCore.Project.RenderMainHelper.headController.Lines.Last();
                                if (line.Count >= 1)
                                    ProgramCore.Project.RenderMainHelper.headController.Lines.Add(new HeadLine());
                            }

                            ProgramCore.MainForm.ctrlRenderControl.UpdateProfileRectangle();
                            UpdateProfileRectangle();
                        }
                        else
                        {
                            if (ProgramCore.Project.RenderMainHelper.headController.Lines.Count == 1)
                            {
                                var line = ProgramCore.Project.RenderMainHelper.headController.Lines.Last();
                                if (line.Count > 1)
                                {
                                    line.Add(line.First());

                                    ProgramCore.Project.RenderMainHelper.headController.Lines.Add(new HeadLine());
                                }
                            }
                        }
                    }
                    break;
            }
            LineSelectionMode = false;
        }

        private static Vector2 GetScreenPoint(Vector2 worldPoint)
        {
            var point = new Vector3(0.0f, worldPoint.Y, worldPoint.X);
            return ProgramCore.MainForm.ctrlRenderControl.camera.GetScreenPoint(point, ProgramCore.MainForm.ctrlRenderControl.Width, ProgramCore.MainForm.ctrlRenderControl.Height);
        }

        public void UpdateProfileLocation()
        {
            var topPoint = profileControlPoints[0].Value;
            var downPoint = profileControlPoints[3].Value;
            var eyePoint = profileControlPoints[1].Value;
            var mouthPoint = profileControlPoints[2].Value;
            var worldEyePoint = new Vector3(0.0f, eyePoint.Y, eyePoint.X);
            var worldMouthPoint = new Vector3(0.0f, mouthPoint.Y, mouthPoint.X);
            ProfileScreenTopLocation = GetScreenPoint(profileControlPoints[0].Value);
            var screenEyeLoaction = GetScreenPoint(profileControlPoints[1].Value);
            var screenMouthLocation = GetScreenPoint(profileControlPoints[2].Value);
            ProfileScreenBottomLocation = GetScreenPoint(profileControlPoints[3].Value);
            var leftLength = ProgramCore.Project.ProfileMouthLocation.X - ProgramCore.Project.ProfileEyeLocation.X;
            var rightLength = screenMouthLocation.X - screenEyeLoaction.X;
            var scale = rightLength / leftLength;
            var localOffset = ProgramCore.Project.ProfileEyeLocation * scale;
            ImageTemplateWidth = (int)(DrawingImage.Width * scale);
            ImageTemplateHeight = (int)(DrawingImage.Height * scale);
            ImageTemplateOffsetX = (int)(screenEyeLoaction.X - localOffset.X);
            ImageTemplateOffsetY = (int)(screenEyeLoaction.Y - localOffset.Y);
            ProfileScreenMouthLocation = screenMouthLocation;
            ProfileScreenEyeLocation = screenEyeLoaction;
        }

        #endregion

        #region Form's event
        private void ctrlTemplateImage_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == (Keys.A))
                ProgramCore.Project.RenderMainHelper.headController.SelectAll();
            else if (e.KeyData == (Keys.D))
                ProgramCore.Project.RenderMainHelper.headController.ClearPointsSelection();
            else if (e.KeyData == (Keys.ShiftKey | Keys.Shift))
                shiftKeyPressed = true;
            else if (e.KeyData == Keys.Enter)
            {
                switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
                {
                    case Mode.HeadLine:
                        if (ProgramCore.MainForm.HeadProfile)
                        {
                            if (isProfileSmoothing)
                            {
                                isProfileSmoothing = false;
                                ProgramCore.MainForm.panelFront.UpdateProfileSmoothing(isProfileSmoothing);
                            }
                            else
                            if (ProgramCore.Project.RenderMainHelper.headController.Lines.Count == 2)
                            {
                                if (ProgramCore.Project.RenderMainHelper.headController.AllPoints.Count > 3)
                                {
                                    foreach (var point in ProgramCore.Project.RenderMainHelper.headController.AllPoints)
                                        point.UpdateWorldPoint();

                                    #region История (undo)

                                    Dictionary<Guid, MeshUndoInfo> undoInfo;
                                    ProgramCore.Project.RenderMainHelper.headMeshesController.GetUndoInfo(out undoInfo);
                                    var isProfile = ProgramCore.MainForm.HeadProfile;
                                    var teInfo = isProfile ? ctrlRenderControl.autodotsShapeHelper.ShapeProfileInfo : ctrlRenderControl.autodotsShapeHelper.ShapeInfo;
                                    var historyElem = new HistoryHeadShapeLines(undoInfo, null, teInfo, isProfile);
                                    ProgramCore.MainForm.ctrlRenderControl.historyController.Add(historyElem);

                                    #endregion

                                    var userPoints = ProgramCore.Project.RenderMainHelper.headController.AllPoints.Select(x => x.ValueMirrored).ToList();
                                    var pointsTop = new List<Vector2>();
                                    List<Vector2> pointsBottom = null;
                                    var lipsY = ctrlRenderControl.autodotsShapeHelper.GetLipsTopY();
                                    var prevPoint = Vector2.Zero;
                                    for (var i = 0; i < userPoints.Count; ++i)
                                    {
                                        var p = userPoints[i];
                                        var x = p.X * ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateWidth + ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateOffsetX;
                                        var y = p.Y * ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateHeight + ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateOffsetY;
                                        var point = ProgramCore.MainForm.ctrlRenderControl.camera.GetWorldPoint((int)x, (int)y,
                                            ProgramCore.MainForm.ctrlRenderControl.Width, ProgramCore.MainForm.ctrlRenderControl.Height, 1.0f).Zy;

                                        if (point.Y < lipsY && i > 0)
                                        {
                                            if (pointsBottom == null)
                                            {
                                                var tempPoint = point - prevPoint;
                                                var d = (point.Y - prevPoint.Y) / (prevPoint.Y - lipsY);
                                                var center = prevPoint + tempPoint * d;
                                                pointsTop.Add(center);
                                                pointsBottom = new List<Vector2>();
                                                pointsBottom.Add(center);
                                            }
                                            pointsBottom.Add(point);
                                        }
                                        else
                                        {
                                            prevPoint = point;
                                            pointsTop.Add(point);
                                        }
                                    }


                                    ctrlRenderControl.autodotsShapeHelper.Transform(MeshPartType.ProfileTop, pointsTop, Vector2.Zero);
                                    ctrlRenderControl.autodotsShapeHelper.Transform(MeshPartType.ProfileBottom, pointsBottom, Vector2.Zero);

                                    var th = new Thread(() =>
                                    {
                                        Thread.CurrentThread.IsBackground = true;
                                        ProgramCore.Project.RenderMainHelper.headMeshesController.Smooth();
                                    });

                                    th.Start();
                                    while (th.IsAlive)
                                        ProgramCore.Progress("Please wait");

                                    ProfileSmoothing = new ProfileSmoothing(ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh, undoInfo);
                                }

                                foreach (var p in ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.Parts)
                                    p.UpdateNormals();

                                ProgramCore.Project.RenderMainHelper.headController.Lines.Clear();
                                //ProgramCore.MainForm.ctrlRenderControl.HeadLineMode = ProgramCore.MainForm.ctrlRenderControl.HeadLineMode == MeshPartType.ProfileTop ? MeshPartType.ProfileBottom : MeshPartType.ProfileTop;
                                ProgramCore.MainForm.ctrlRenderControl.UpdateProfileRectangle();
                                isProfileSmoothing = true;
                                ProgramCore.MainForm.panelFront.UpdateProfileSmoothing(isProfileSmoothing);
                            }
                            else
                                FinishLine();
                        }
                        else
                            FinishLine();
                        break;
                    case Mode.None:
                        if (ProgramCore.MainForm.HeadProfile)
                            switch (ControlPointsMode)
                            {
                                case ProfileControlPointsMode.MoveControlPoints:
                                    UpdateProfileImageByControlPoints();

                                    UpdateProfileRectangle();
                                    ProgramCore.MainForm.ctrlRenderControl.UpdateProfileRectangle();
                                    ControlPointsMode = ProfileControlPointsMode.UpdateRightLeft;
                                    break;
                                case ProfileControlPointsMode.UpdateRightLeft:
                                    UpdateProfileRectangle();
                                    foreach (var point in profileControlPoints)
                                        point.Selected = false;

                                    ControlPointsMode = ProfileControlPointsMode.None;
                                    break;
                            }

                        break;
                }
            }
        }

        private void ctrlTemplateImage_KeyUp(object sender, KeyEventArgs e)
        {
            shiftKeyPressed = false;
        }

        private void pictureTemplate_Resize(object sender, EventArgs e)
        {
            RecalcRealTemplateImagePosition();
            if (ProgramCore.Project != null)
            {
                UpdateUserCenterPositions(true, true);
                UpdateProfileRectangle();
            }
        }
        private void pictureTemplate_Paint(object sender, PaintEventArgs e)
        {
            if (ProgramCore.MainForm == null)
                return;

            e.Graphics.DrawImage(DrawingImage, ImageTemplateOffsetX, ImageTemplateOffsetY, ImageTemplateWidth, ImageTemplateHeight);

            if (ProgramCore.Debug && ProgramCore.MainForm.HeadFront)
                e.Graphics.DrawRectangle(DrawingTools.GreenPen, EyesMouthRectTransformed.X, EyesMouthRectTransformed.Y, EyesMouthRectTransformed.Width, EyesMouthRectTransformed.Height);

            switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
            {
                //      case Mode.HeadShapedots:
                case Mode.HeadAutodots:
                case Mode.HeadAutodotsFirstTime:
                 //   DrawAutodotsGroupPoints(e.Graphics);
                    if (RectTransformMode)
                    {
                        e.Graphics.DrawRectangle(DrawingTools.RedPen, FaceRectTransformed);

                        var pointRect = new RectangleF(FaceRectTransformed.X - HalfPointRectSize, FaceRectTransformed.Y - HalfPointRectSize, PointRectSize, PointRectSize);
                        e.Graphics.FillRectangle(DrawingTools.BlueSolidBrush, pointRect);

                        pointRect = new RectangleF(FaceRectTransformed.X + FaceRectTransformed.Width - HalfPointRectSize, FaceRectTransformed.Y - HalfPointRectSize, PointRectSize, PointRectSize);
                        e.Graphics.FillRectangle(DrawingTools.BlueSolidBrush, pointRect);

                        pointRect = new RectangleF(FaceRectTransformed.X + FaceRectTransformed.Width - HalfPointRectSize, FaceRectTransformed.Y + FaceRectTransformed.Height - HalfPointRectSize, PointRectSize, PointRectSize);
                        e.Graphics.FillRectangle(DrawingTools.BlueSolidBrush, pointRect);

                        pointRect = new RectangleF(FaceRectTransformed.X - HalfPointRectSize, FaceRectTransformed.Y + FaceRectTransformed.Height - HalfPointRectSize, PointRectSize, PointRectSize);
                        e.Graphics.FillRectangle(DrawingTools.BlueSolidBrush, pointRect);

                        if (ProgramCore.Project.TextureFlip != FlipType.None)
                        {
                            var centerX = FaceRectTransformed.X + (FaceRectTransformed.Width * 0.5f);
                            e.Graphics.DrawLine(DrawingTools.BluePen, centerX, FaceRectTransformed.Y, centerX, FaceRectTransformed.Bottom);
                        }
                    }

                    break;
                case Mode.HeadAutodotsLassoStart:
                case Mode.HeadAutodotsLassoActive:
                    DrawLassoOnPictureBox(e.Graphics, true);
                    break;
                    /*case Mode.HeadShapedotsLassoStart:
                    case Mode.HeadShapedotsLassoActive:
                        DrawLassoOnPictureBox(e.Graphics, false);*/
                    break;
                case Mode.HeadLine:
                    if (ProgramCore.MainForm.HeadFront)
                    {
                        #region вид спереди

                        DrawAutodotsGroupPoints(e.Graphics);

                        #endregion
                    }
                    else
                    {
                        #region Вид сбоку

                        if (ProgramCore.Debug)
                            e.Graphics.DrawRectangle(DrawingTools.RedPen, ProfileFaceRectTransformed);

                        #region Верхняя и нижняя точки
                        var points = new[] { ProfileScreenTopLocation, ProfileScreenEyeLocation, ProfileScreenMouthLocation, ProfileScreenBottomLocation };
                        for (var i = 0; i < points.Length; i += 3)
                        {
                            var point = points[i];

                            var pointRect = new RectangleF(point.X - HalfPointRectSize, point.Y - HalfPointRectSize, PointRectSize, PointRectSize);
                            e.Graphics.FillRectangle(profileControlPoints[i].Selected ? DrawingTools.RedSolidBrush : DrawingTools.BlueSolidBrush, pointRect);
                        }

                        #endregion

                        #endregion
                    }
                    break;
                case Mode.None:
                    switch (ControlPointsMode)
                    {
                        case ProfileControlPointsMode.SetControlPoints:
                        case ProfileControlPointsMode.MoveControlPoints:
                            {
                                foreach (var point in profileControlPoints)
                                {
                                    if (point.ValueMirrored == Vector2.Zero)
                                        continue;

                                    var pointK = new Vector2(point.ValueMirrored.X * ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateWidth + ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateOffsetX,
                                                             point.ValueMirrored.Y * ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateHeight + ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateOffsetY);
                                    var pointRect = new RectangleF(pointK.X - HalfPointRectSize, pointK.Y - HalfPointRectSize, PointRectSize, PointRectSize);
                                    e.Graphics.FillRectangle(point.Selected ? DrawingTools.RedSolidBrush : DrawingTools.BlueSolidBrush, pointRect);
                                }
                            }
                            break;
                        case ProfileControlPointsMode.UpdateRightLeft:

                            #region Верхняя и нижняя точки

                            for (var i = 0; i < profileControlPoints.Count; i += 3)
                            {
                                var point = profileControlPoints[i];
                                if (point.ValueMirrored == Vector2.Zero)
                                    continue;

                                var pointK = new Vector2(point.ValueMirrored.X * ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateWidth + ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateOffsetX,
                                                         point.ValueMirrored.Y * ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateHeight + ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateOffsetY);
                                var pointRect = new RectangleF(pointK.X - HalfPointRectSize, pointK.Y - HalfPointRectSize, PointRectSize, PointRectSize);
                                e.Graphics.FillRectangle(point.Selected ? DrawingTools.RedSolidBrush : DrawingTools.BlueSolidBrush, pointRect);
                            }

                            #endregion

                            #region Линии лица

                            foreach (var rect in ctrlRenderControl.autodotsShapeHelper.ProfileRects)
                                if (rect.LinkedShapeRect != null)
                                {
                                    for (var i = 1; i < rect.Points.Length; i++)
                                    {
                                        var point1 = new Vector2(-rect.Points[i - 1].X, rect.Points[i - 1].Y);
                                        var point2 = new Vector2(-rect.Points[i].X, rect.Points[i].Y);
                                        var pointM = new MirroredHeadPoint(point1, point1);
                                        var pointB = new MirroredHeadPoint(point2, point2);
                                        e.Graphics.DrawLine(DrawingTools.GreenPen,
                                            (pointM.ValueMirrored.X * ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateWidth + ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateOffsetX),
                                             pointM.ValueMirrored.Y * ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateHeight + ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateOffsetY,
                                            (pointB.ValueMirrored.X * ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateWidth + ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateOffsetX),
                                           pointB.ValueMirrored.Y * ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateHeight + ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateOffsetY);
                                    }
                                }

                            #endregion

                            break;
                    }
                    break;
            }

            if (ImageTemplateOffsetX != -1)
                ProgramCore.Project.RenderMainHelper.headController.DrawOnPictureBox(e.Graphics);
        }

        public void pictureTemplate_MouseDown(object sender, MouseEventArgs e)
        {
            Focus();

            dblClick = e.Clicks == 2;
            if (e.Button == MouseButtons.Left)
            {
                leftMousePressed = true;
                startMove = false;
                headLastPoint = new Vector2(e.X, e.Y);
                headLastPointRelative.X = (e.X - ImageTemplateOffsetX) / (ImageTemplateWidth * 1f);
                headLastPointRelative.Y = (e.Y - ImageTemplateOffsetY) / (ImageTemplateHeight * 1f);
                headTempPoints.Clear();

                switch (ProgramCore.MainForm.ctrlRenderControl.ScaleMode)
                {
                    case ScaleMode.Zoom:
                        switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
                        {
                            //      case Mode.HeadShapedots:
                            case Mode.HeadLine:                 // эти моды только для этих режимов!
                            case Mode.HeadAutodots:
                            case Mode.HeadAutodotsFirstTime:
                            case Mode.HeadAutodotsLassoStart:
                            case Mode.HeadAutodotsLassoActive:
                                {
                                    if (ProgramCore.MainForm.HeadProfile)
                                        break;

                                    ImageTemplateCenterX = pictureTemplate.Width / 2 - ImageTemplateOffsetX;
                                    ImageTemplateCenterY = pictureTemplate.Height / 2 - ImageTemplateOffsetY;
                                    ImageTemplateOldWidth = ImageTemplateWidth;
                                }
                                break;
                        }
                        break;
                    case ScaleMode.Move:
                        switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
                        {
                            // case Mode.HeadShapedots:
                            case Mode.HeadLine: // эти моды только для этих режимов!
                            case Mode.HeadAutodots:
                            case Mode.HeadAutodotsFirstTime:
                            case Mode.HeadAutodotsLassoStart:
                            case Mode.HeadAutodotsLassoActive:
                                {
                                    if (ProgramCore.MainForm.HeadProfile)
                                        break;

                                    tempOffsetPoint = new Vector2(ImageTemplateOffsetX, ImageTemplateOffsetY);
                                }
                                break;
                        }
                        break;
                    case ScaleMode.None:

                        #region Обычные режимы

                        switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
                        {
                            case Mode.ColorPicker:
                                {
                                    using (var bitmap = new Bitmap(pictureTemplate.Width, pictureTemplate.Height))
                                    using (var g = Graphics.FromImage(bitmap))
                                    {
                                        g.DrawImage(DrawingImage, ImageTemplateOffsetX, ImageTemplateOffsetY, ImageTemplateWidth, ImageTemplateHeight);
                                        var color = bitmap.GetPixel((int)headLastPoint.X, (int)headLastPoint.Y);
                                        ProgramCore.MainForm.frmMaterial.SetColorFromPicker(color);
                                    }
                                }
                                break;
                            case Mode.HeadAutodotsFirstTime:
                            case Mode.HeadAutodots:
                                {
                                    if (ProgramCore.Project.ShapeFlip != FlipType.None)
                                        return;

                                    if (dblClick)
                                    {
                                        RectTransformMode = false;
                                        ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2.ClearSelection();
                                    }

                                    #region Rectangle transforM ?

                                    moveRectIndex = -1;
                                    if (e.X >= FaceRectTransformed.X - HalfPointRectSize && e.X <= FaceRectTransformed.X + HalfPointRectSize && e.Y >= FaceRectTransformed.Y - HalfPointRectSize && e.Y <= FaceRectTransformed.Y + HalfPointRectSize)
                                        moveRectIndex = 1;
                                    else if (e.X >= FaceRectTransformed.X + FaceRectTransformed.Width - HalfPointRectSize && e.X <= FaceRectTransformed.X + FaceRectTransformed.Width + HalfPointRectSize
                                             && e.Y >= FaceRectTransformed.Y - HalfPointRectSize && e.Y <= FaceRectTransformed.Y + HalfPointRectSize)
                                        moveRectIndex = 2;
                                    else if (e.X >= FaceRectTransformed.X + FaceRectTransformed.Width - HalfPointRectSize && e.X <= FaceRectTransformed.X + FaceRectTransformed.Width + HalfPointRectSize
                                             && e.Y >= FaceRectTransformed.Y + FaceRectTransformed.Height - HalfPointRectSize && e.Y <= FaceRectTransformed.Y + FaceRectTransformed.Height + HalfPointRectSize)
                                        moveRectIndex = 3;
                                    else if (e.X >= FaceRectTransformed.X - HalfPointRectSize && e.X <= FaceRectTransformed.X + HalfPointRectSize && e.Y >= FaceRectTransformed.Y + FaceRectTransformed.Height - HalfPointRectSize
                                             && e.Y <= FaceRectTransformed.Y + FaceRectTransformed.Height + HalfPointRectSize)
                                        moveRectIndex = 4;

                                    #endregion

                                    if (moveRectIndex == -1) // если таскаем не прямоугольник, а точки
                                    {
                                        foreach (var item in ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2.SelectedPoints)
                                            headTempPoints.Add(item.Clone());
                                    }
                                    else
                                    {
                                        var temp = FaceRectTransformed.X + FaceRectTransformed.Width * 0.5f;
                                        tempMoveRectCenter.X = (temp - ImageTemplateOffsetX) / (ImageTemplateWidth * 1f);
                                        temp = FaceRectTransformed.Y + FaceRectTransformed.Height * 0.5f;
                                        tempMoveRectCenter.Y = (temp - ImageTemplateOffsetY) / (ImageTemplateHeight * 1f);

                                        tempMoveRectWidth = (FaceRectTransformed.Width) / (ImageTemplateWidth * 1f);
                                        tempMoveRectHeight = (FaceRectTransformed.Height) / (ImageTemplateHeight * 1f);
                                    }
                                }
                                break;
                            /*   case Mode.HeadShapedots:
                                   {
                                       if (ProgramCore.Project.ShapeFlip != FlipType.None)
                                           return;

                                       if (dblClick)
                                       {
                                           RectTransformMode = false;
                                           ProgramCore.Project.RenderMainHelper.headController.ShapeDots.ClearSelection();
                                       }

                                       #region Rectangle transforM ?

                                       moveRectIndex = -1;
                                       if (e.X >= FaceRectTransformed.X - HalfPointRectSize && e.X <= FaceRectTransformed.X + HalfPointRectSize && e.Y >= FaceRectTransformed.Y - HalfPointRectSize && e.Y <= FaceRectTransformed.Y + HalfPointRectSize)
                                           moveRectIndex = 1;
                                       else if (e.X >= FaceRectTransformed.X + FaceRectTransformed.Width - HalfPointRectSize && e.X <= FaceRectTransformed.X + FaceRectTransformed.Width + HalfPointRectSize
                                                && e.Y >= FaceRectTransformed.Y - HalfPointRectSize && e.Y <= FaceRectTransformed.Y + HalfPointRectSize)
                                           moveRectIndex = 2;
                                       else if (e.X >= FaceRectTransformed.X + FaceRectTransformed.Width - HalfPointRectSize && e.X <= FaceRectTransformed.X + FaceRectTransformed.Width + HalfPointRectSize
                                                && e.Y >= FaceRectTransformed.Y + FaceRectTransformed.Height - HalfPointRectSize && e.Y <= FaceRectTransformed.Y + FaceRectTransformed.Height + HalfPointRectSize)
                                           moveRectIndex = 3;
                                       else if (e.X >= FaceRectTransformed.X - HalfPointRectSize && e.X <= FaceRectTransformed.X + HalfPointRectSize && e.Y >= FaceRectTransformed.Y + FaceRectTransformed.Height - HalfPointRectSize
                                                && e.Y <= FaceRectTransformed.Y + FaceRectTransformed.Height + HalfPointRectSize)
                                           moveRectIndex = 4;

                                       #endregion

                                       if (moveRectIndex == -1) // если таскаем не прямоугольник, а точки
                                       {
                                           foreach (var item in ProgramCore.Project.RenderMainHelper.headController.ShapeDots.SelectedPoints)
                                               headTempPoints.Add(item.Clone() as MirroredHeadPoint);
                                       }
                                       else
                                       {
                                           var temp = FaceRectTransformed.X + FaceRectTransformed.Width * 0.5f;
                                           tempMoveRectCenter.X = (temp - ImageTemplateOffsetX) / (ImageTemplateWidth * 1f);
                                           temp = FaceRectTransformed.Y + FaceRectTransformed.Height * 0.5f;
                                           tempMoveRectCenter.Y = (temp - ImageTemplateOffsetY) / (ImageTemplateHeight * 1f);

                                           tempMoveRectWidth = (FaceRectTransformed.Width) / (ImageTemplateWidth * 1f);
                                           tempMoveRectHeight = (FaceRectTransformed.Height) / (ImageTemplateHeight * 1f);
                                       }
                                   }
                                   break;*/
                            case Mode.HeadAutodotsLassoStart:
                                if (dblClick)
                                {
                                    ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.HeadAutodotsLassoActive;
                                    headAutodotsLassoPoints.Add(headAutodotsLassoPoints.First());
                                }
                                break;
                            case Mode.HeadAutodotsLassoActive:
                                ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.HeadAutodotsLassoStart;
                                headAutodotsLassoPoints.Clear();
                                headAutodotsLassoPoints.Add(new Vector2(e.X, e.Y));
                                break;
                            /*     case Mode.HeadShapedotsLassoStart:
                                     if (dblClick)
                                     {
                                         ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.HeadShapedotsLassoActive;
                                         headShapedotsLassoPoints.Add(headShapedotsLassoPoints.First());
                                     }
                                     break;
                                 case Mode.HeadShapedotsLassoActive:
                                     ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.HeadShapedotsLassoStart;
                                     headShapedotsLassoPoints.Clear();
                                     headShapedotsLassoPoints.Add(new Vector2(e.X, e.Y));
                                     break;*/
                            case Mode.HeadLine:
                                if (ProgramCore.Project.ShapeFlip != FlipType.None)
                                    return;

                                if (ProgramCore.MainForm.HeadFront)
                                {
                                    #region вид спереди

                                    if (ProgramCore.MainForm.ctrlRenderControl.HeadLineMode == MeshPartType.None)
                                        return;

                                    if (dblClick)
                                        FinishLine();
                                    else
                                    {
                                        foreach (var item in ProgramCore.Project.RenderMainHelper.headController.SelectedPoints)
                                            headTempPoints.Add(item.Clone() as MirroredHeadPoint);
                                    }

                                    #endregion
                                }
                                else
                                {
                                    #region Вид сбоку

                                    if (isProfileSmoothing)
                                        return;

                                    if (dblClick)
                                        FinishLine();
                                    else
                                    {
                                        foreach (var item in ProgramCore.Project.RenderMainHelper.headController.SelectedPoints)
                                            headTempPoints.Add(item.Clone() as MirroredHeadPoint);
                                    }

                                    #endregion
                                }
                                break;
                            case Mode.None:
                                {
                                    switch (ControlPointsMode)
                                    {
                                        case ProfileControlPointsMode.MoveControlPoints:
                                        case ProfileControlPointsMode.UpdateRightLeft:
                                            foreach (var item in profileControlPoints)
                                                headTempPoints.Add(item.Clone() as MirroredHeadPoint);
                                            break;
                                    }
                                }
                                break;
                        }

                        #endregion

                        break;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
                {
                    case Mode.HeadLine:
                        if (ProgramCore.Project.ShapeFlip != FlipType.None)
                            return;

                        if (ProgramCore.MainForm.HeadFront)
                        {
                            #region вид спереди

                            if (ProgramCore.MainForm.ctrlRenderControl.HeadLineMode == MeshPartType.None)
                                return;

                            ProgramCore.Project.RenderMainHelper.headController.Lines.Add(new HeadLine());

                            #endregion
                        }
                        else
                        {
                            #region Вид сбоку

                            ProgramCore.Project.RenderMainHelper.headController.Lines.Add(new HeadLine());

                            #endregion
                        }

                        break;
                }
            }
        }

        public void pictureTemplate_MouseMove(object sender, MouseEventArgs e)
        {
            if (startMousePoint == Vector2.Zero)
                startMousePoint = new Vector2(e.X, e.Y);

            var firstMove = false;
            if (Math.Abs(startMousePoint.X - e.X) > 1 || Math.Abs(startMousePoint.Y - e.Y) > 1) // small exp
            {
                if (!startMove)
                    firstMove = true;

                startMove = true;
            }

            if (leftMousePressed)
            {
                Vector2 newPoint;
                Vector2 delta2;
                newPoint.X = (e.X - ImageTemplateOffsetX) / (ImageTemplateWidth * 1f);
                newPoint.Y = (e.Y - ImageTemplateOffsetY) / (ImageTemplateHeight * 1f);
                switch (ProgramCore.MainForm.ctrlRenderControl.ScaleMode)
                {
                    case ScaleMode.Move:
                        {
                            if (ProgramCore.MainForm.HeadProfile)
                                break;

                            switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
                            {
                                //      case Mode.HeadShapedots:
                                case Mode.HeadLine: // эти моды только для этих режимов!
                                case Mode.HeadAutodots:
                                case Mode.HeadAutodotsFirstTime:
                                case Mode.HeadAutodotsLassoStart:
                                case Mode.HeadAutodotsLassoActive:

                                    newPoint = new Vector2(e.X, e.Y);
                                    delta2 = newPoint - headLastPoint;
                                    ImageTemplateOffsetX = (int)(tempOffsetPoint.X + delta2.X);
                                    ImageTemplateOffsetY = (int)(tempOffsetPoint.Y + delta2.Y);
                                    RecalcEyeMouthRect();
                                    UpdateFaceRect();
                                    break;
                            }
                        }
                        break;
                    case ScaleMode.Zoom:
                        {
                            if (ProgramCore.MainForm.HeadProfile)
                                break;
                            switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
                            {
                                //   case Mode.HeadShapedots:
                                case Mode.HeadLine: // эти моды только для этих режимов!
                                case Mode.HeadAutodots:
                                case Mode.HeadAutodotsFirstTime:
                                case Mode.HeadAutodotsLassoStart:
                                case Mode.HeadAutodotsLassoActive:
                                    if (startMove)
                                    {
                                        var s = imageScale + (headLastPoint.Y - e.Y) * 0.01f;
                                        if (s < 1.0f)
                                            imageScale = 1.0f;
                                        else if (s > 5.0f)
                                            imageScale = 5.0f;
                                        else
                                        {
                                            ApplyScale(s);
                                            RefreshPictureBox();
                                        }
                                        headLastPoint = new Vector2(e.X, e.Y);
                                        RecalcEyeMouthRect();
                                        UpdateFaceRect();
                                    }
                                    break;
                            }
                        }
                        break;
                    case ScaleMode.None:

                        #region Если нет зума - обрабатываем обычные режимы

                        switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
                        {
                            case Mode.HeadAutodotsFirstTime:
                            case Mode.HeadAutodots:
                                if (ProgramCore.Project.ShapeFlip != FlipType.None)
                                    return;

                                if (firstMove &&
                                    ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2.SelectedPoints.Count >
                                    0)
                                {
                                    var history =
                                        new HistoryHeadAutoDots(
                                            ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2);
                                    ProgramCore.MainForm.ctrlRenderControl.historyController.Add(history);

                                    Dictionary<Guid, MeshUndoInfo> undoInfo;
                                    ProgramCore.Project.RenderMainHelper.headMeshesController.GetUndoInfo(
                                        out undoInfo);
                                    ProgramCore.MainForm.ctrlRenderControl.historyController.Add(
                                        new HistoryHeadShapeDots(undoInfo,
                                            ProgramCore.Project.RenderMainHelper.headController.ShapeDots));
                                }

                                if (startMove)
                                {
                                    if (firstMove)
                                        Cursor = ProgramCore.MainForm.GrabbingCursor;

                                    delta2 = newPoint - headLastPointRelative;
                                    if (moveRectIndex != -1) //таскаем прямоугольничек
                                    {
                                        var deltaX = (int)(e.X - headLastPoint.X);
                                        var deltaY = (int)(e.Y - headLastPoint.Y);
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
                                        headLastPoint = new Vector2(e.X, e.Y);

                                        Vector2 center;
                                        var temp = FaceRectTransformed.X + FaceRectTransformed.Width * 0.5f;
                                        center.X = ((temp - ImageTemplateOffsetX) / (ImageTemplateWidth * 1f));
                                        temp = FaceRectTransformed.Y + FaceRectTransformed.Height * 0.5f;
                                        center.Y = (temp - ImageTemplateOffsetY) / (ImageTemplateHeight * 1f);

                                        var newWidth = (FaceRectTransformed.Width) / (ImageTemplateWidth * 1f);
                                        var newHeight = (FaceRectTransformed.Height) / (ImageTemplateHeight * 1f);
                                        var kx = newWidth / tempMoveRectWidth;
                                        var ky = newHeight / tempMoveRectHeight;
                                        foreach ( var point in ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2.SelectedPoints)
                                        {
                                          var p = point.OriginalValue - tempMoveRectCenter;  
                                            p.X *= kx;
                                            p.Y *= ky;
                                            point.OriginalValue = p + center;
                                       //     point.UpdateWorldPoint();
                                        }
                                        tempMoveRectCenter = center;
                                        tempMoveRectWidth = newWidth;
                                        tempMoveRectHeight = newHeight;
                                        UpdateUserCenterPositions(false, true);
                                    }
                                    else // таскаем точки
                                    {
                                        var selectedPoints =
                                            ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2
                                                .SelectedPoints;
                                        for (var i = 0; i < selectedPoints.Count; i++)
                                        {
                                              var headPoint = selectedPoints[i];          
                                              headPoint.OriginalValue = headTempPoints[i].OriginalValue + delta2;

                                        }
                                        UpdateUserCenterPositions(true, true);
                                    }
                                }
                                break;
                            case Mode.HeadLine:
                                if (ProgramCore.Project.ShapeFlip != FlipType.None)
                                    return;

                                if (LineSelectionMode)
                                {
                                    if (firstMove &&
                                        ProgramCore.Project.RenderMainHelper.headController.SelectedPoints.Count > 0)
                                    {
                                        var isProfile = ProgramCore.MainForm.HeadProfile;
                                        var teInfo = isProfile
                                            ? ctrlRenderControl.autodotsShapeHelper
                                                .ShapeProfileInfo
                                            : ctrlRenderControl.autodotsShapeHelper.ShapeInfo;
                                        var historyElem = new HistoryHeadShapeLines(null,
                                            ProgramCore.Project.RenderMainHelper.headController.Lines, teInfo,
                                            isProfile);
                                        historyElem.Group =
                                            ProgramCore.MainForm.ctrlRenderControl.historyController.currentGroup;
                                        ProgramCore.MainForm.ctrlRenderControl.historyController.Add(historyElem);
                                    }

                                    delta2 = newPoint - headLastPointRelative;
                                    for (var i = 0;
                                        i < ProgramCore.Project.RenderMainHelper.headController.SelectedPoints.Count;
                                        i++)
                                    {
                                          var headPoint = ProgramCore.Project.RenderMainHelper.headController.SelectedPoints[i];
                                          headPoint.OriginalValue = headTempPoints[i].OriginalValue + delta2;

                                    }
                                }
                                break;
                            /*     case Mode.HeadShapedots:
                                     if (ProgramCore.Project.ShapeFlip != FlipType.None)
                                         return;

                                     if (startMove)
                                     {
                                         if (firstMove)
                                             Cursor = ProgramCore.MainForm.GrabbingCursor;

                                         if (firstMove &&
                                             ProgramCore.Project.RenderMainHelper.headController.ShapeDots.SelectedPoints
                                                 .Count > 0)
                                         {
                                             Dictionary<Guid, MeshUndoInfo> undoInfo;
                                             ProgramCore.Project.RenderMainHelper.headMeshesController.GetUndoInfo(
                                                 out undoInfo);
                                             ProgramCore.MainForm.ctrlRenderControl.historyController.Add(
                                                 new HistoryHeadShapeDots(undoInfo,
                                                     ProgramCore.Project.RenderMainHelper.headController.ShapeDots));
                                         }

                                         delta2 = newPoint - headLastPointRelative;
                                         if (moveRectIndex != -1) //таскаем прямоугольничек
                                         {
                                             var deltaX = (int)(e.X - headLastPoint.X);
                                             var deltaY = (int)(e.Y - headLastPoint.Y);
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
                                             headLastPoint = new Vector2(e.X, e.Y);

                                             Vector2 center;
                                             var temp = FaceRectTransformed.X + FaceRectTransformed.Width * 0.5f;
                                             center.X = ((temp - ImageTemplateOffsetX) / (ImageTemplateWidth * 1f));
                                             temp = FaceRectTransformed.Y + FaceRectTransformed.Height * 0.5f;
                                             center.Y = (temp - ImageTemplateOffsetY) / (ImageTemplateHeight * 1f);

                                             var newWidth = (FaceRectTransformed.Width) / (ImageTemplateWidth * 1f);
                                             var newHeight = (FaceRectTransformed.Height) / (ImageTemplateHeight * 1f);
                                             var kx = newWidth / tempMoveRectWidth;
                                             var ky = newHeight / tempMoveRectHeight;
                                             foreach (
                                                 var point in
                                                     ProgramCore.Project.RenderMainHelper.headController.ShapeDots
                                                         .SelectedPoints)
                                             {
                                                 var p = point.ValueMirrored - tempMoveRectCenter;
                                                 p.X *= kx;
                                                 p.Y *= ky;
                                                 point.ValueMirrored = p + center;
                                                 point.UpdateWorldPoint();
                                             }
                                             tempMoveRectCenter = center;
                                             tempMoveRectWidth = newWidth;
                                             tempMoveRectHeight = newHeight;
                                             UpdateUserCenterPositions(false, true);
                                         }
                                         else // таскаем точки
                                         {
                                             var selectedPoints =
                                                 ProgramCore.Project.RenderMainHelper.headController.ShapeDots
                                                     .SelectedPoints;
                                             for (var i = 0; i < selectedPoints.Count; i++)
                                             {
                                                 var headPoint = selectedPoints[i];
                                                 headPoint.ValueMirrored = headTempPoints[i].ValueMirrored + delta2;
                                                 headPoint.UpdateWorldPoint();
                                             }
                                             UpdateUserCenterPositions(true, true);
                                         }
                                     }
                                     else Cursor = ProgramCore.MainForm.GrabCursor;
                                     break;*/
                            case Mode.None:
                                {
                                    switch (ControlPointsMode)
                                    {
                                        case ProfileControlPointsMode.MoveControlPoints:
                                        case ProfileControlPointsMode.UpdateRightLeft:
                                            {
                                                delta2 = newPoint - headLastPointRelative;
                                                for (var i = 0; i < profileControlPoints.Count; i++)
                                                {
                                                      var headPoint = profileControlPoints[i];             
                                                      if (!headPoint.Selected)
                                                          continue;

                                                      headPoint.OriginalValue = headTempPoints[i].OriginalValue + delta2;
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;
                        }

                        #endregion

                        break;
                }

            }
            else
            {
                switch (ProgramCore.MainForm.ctrlRenderControl.ScaleMode)
                {
                    case ScaleMode.None:
                        {
                            switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
                            {
                                case Mode.HeadAutodotsFirstTime:
                                case Mode.HeadAutodots:
                                    if (ProgramCore.Project.ShapeFlip != FlipType.None)
                                        return;

                                    if ((e.X >= MouthTransformed.X - HalfPointRectSize && e.X <= MouthTransformed.X + HalfPointRectSize && e.Y >= MouthTransformed.Y - HalfPointRectSize && e.Y <= MouthTransformed.Y + HalfPointRectSize)       // рот
                                   || (e.X >= LeftEyeTransformed.X - HalfPointRectSize && e.X <= LeftEyeTransformed.X + HalfPointRectSize && e.Y >= LeftEyeTransformed.Y - HalfPointRectSize && e.Y <= LeftEyeTransformed.Y + HalfPointRectSize)  // левый глаз
                                   || (e.X >= RightEyeTransformed.X - HalfPointRectSize && e.X <= RightEyeTransformed.X + HalfPointRectSize && e.Y >= RightEyeTransformed.Y - HalfPointRectSize && e.Y <= RightEyeTransformed.Y + HalfPointRectSize)  // правый глаз
                                    || (e.X >= NoseTransformed.X - HalfPointRectSize && e.X <= NoseTransformed.X + HalfPointRectSize && e.Y >= NoseTransformed.Y - HalfPointRectSize && e.Y <= NoseTransformed.Y + HalfPointRectSize) // нос
                                    || (e.X >= CentralFacePoint.X - HalfPointRectSize && e.X <= CentralFacePoint.X + HalfPointRectSize && e.Y >= CentralFacePoint.Y - HalfPointRectSize && e.Y <= CentralFacePoint.Y + HalfPointRectSize) // прямоугольник и выделение всех точек
                                    || ProgramCore.Project.RenderMainHelper.headController.UpdateAutodotsPointSelection(e.X, e.Y, false))
                                        Cursor = ProgramCore.MainForm.GrabCursor;
                                    else
                                        Cursor = Cursors.Arrow;
                                    break;
                                    /*        case Mode.HeadShapedots:
                                                if (ProgramCore.Project.ShapeFlip != FlipType.None)
                                                    return;
                                                if ((e.X >= MouthTransformed.X - HalfPointRectSize &&
                                                     e.X <= MouthTransformed.X + HalfPointRectSize &&
                                                     e.Y >= MouthTransformed.Y - HalfPointRectSize &&
                                                     e.Y <= MouthTransformed.Y + HalfPointRectSize) // рот
                                                    ||
                                                    (e.X >= LeftEyeTransformed.X - HalfPointRectSize &&
                                                     e.X <= LeftEyeTransformed.X + HalfPointRectSize &&
                                                     e.Y >= LeftEyeTransformed.Y - HalfPointRectSize &&
                                                     e.Y <= LeftEyeTransformed.Y + HalfPointRectSize) // левый глаз
                                                    ||
                                                    (e.X >= RightEyeTransformed.X - HalfPointRectSize &&
                                                     e.X <= RightEyeTransformed.X + HalfPointRectSize &&
                                                     e.Y >= RightEyeTransformed.Y - HalfPointRectSize &&
                                                     e.Y <= RightEyeTransformed.Y + HalfPointRectSize) // правый глаз
                                                    ||
                                                    (e.X >= NoseTransformed.X - HalfPointRectSize &&
                                                     e.X <= NoseTransformed.X + HalfPointRectSize &&
                                                     e.Y >= NoseTransformed.Y - HalfPointRectSize &&
                                                     e.Y <= NoseTransformed.Y + HalfPointRectSize) // нос
                                                    ||
                                                    (e.X >= CentralFacePoint.X - HalfPointRectSize &&
                                                     e.X <= CentralFacePoint.X + HalfPointRectSize &&
                                                     e.Y >= CentralFacePoint.Y - HalfPointRectSize &&
                                                     e.Y <= CentralFacePoint.Y + HalfPointRectSize)
                                                    // прямоугольник и выделение всех точек
                                                    ||
                                                    ProgramCore.Project.RenderMainHelper.headController
                                                        .UpdateShapedotsPointSelection(e.X, e.Y, false))
                                                    Cursor = ProgramCore.MainForm.GrabCursor;
                                                else
                                                    Cursor = Cursors.Arrow;
                                                break;*/
                            }
                            break;

                        }
                }
            }
        }

        public void pictureTemplate_MouseUp(object sender, MouseEventArgs e)
        {
            startMousePoint = Vector2.Zero;
            if (e.Button == MouseButtons.Left)
            {
                headLastPoint = Vector2.Zero;
                switch (ProgramCore.MainForm.ctrlRenderControl.ScaleMode)
                {
                    case ScaleMode.Zoom:
                        tempOffsetPoint = Vector2.Zero;
                        break;
                    case ScaleMode.None:

                        #region Обычные режимы

                        switch (ProgramCore.MainForm.ctrlRenderControl.Mode)
                        {
                            case Mode.HeadAutodotsFirstTime:
                            case Mode.HeadAutodots:
                                {
                                    if (ProgramCore.Project.ShapeFlip != FlipType.None)
                                        return;

                                    if (!startMove && !dblClick)
                                    {
                                        if (!shiftKeyPressed)
                                            ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2.ClearSelection();

                                        if (e.X >= MouthTransformed.X - HalfPointRectSize && e.X <= MouthTransformed.X + HalfPointRectSize && e.Y >= MouthTransformed.Y - HalfPointRectSize && e.Y <= MouthTransformed.Y + HalfPointRectSize)       // рот
                                            ProgramCore.Project.RenderMainHelper.headController.SelectAutdotsMouth();
                                        else if (e.X >= LeftEyeTransformed.X - HalfPointRectSize && e.X <= LeftEyeTransformed.X + HalfPointRectSize && e.Y >= LeftEyeTransformed.Y - HalfPointRectSize && e.Y <= LeftEyeTransformed.Y + HalfPointRectSize)  // левый глаз
                                            ProgramCore.Project.RenderMainHelper.headController.SelectAutodotsLeftEye();
                                        else if (e.X >= RightEyeTransformed.X - HalfPointRectSize && e.X <= RightEyeTransformed.X + HalfPointRectSize && e.Y >= RightEyeTransformed.Y - HalfPointRectSize && e.Y <= RightEyeTransformed.Y + HalfPointRectSize)  // правый глаз
                                            ProgramCore.Project.RenderMainHelper.headController.SelectAutodotsRightEye();
                                        else if (e.X >= NoseTransformed.X - HalfPointRectSize && e.X <= NoseTransformed.X + HalfPointRectSize && e.Y >= NoseTransformed.Y - HalfPointRectSize && e.Y <= NoseTransformed.Y + HalfPointRectSize) // нос
                                            ProgramCore.Project.RenderMainHelper.headController.SelectAutodotsNose();
                                        else if (e.X >= CentralFacePoint.X - HalfPointRectSize && e.X <= CentralFacePoint.X + HalfPointRectSize && e.Y >= CentralFacePoint.Y - HalfPointRectSize && e.Y <= CentralFacePoint.Y + HalfPointRectSize) // прямоугольник и выделение всех точек
                                        {
                                            if (RectTransformMode)
                                            {
                                                RectTransformMode = false;
                                                ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2.ClearSelection();
                                            }
                                            else
                                            {
                                                RectTransformMode = true;
                                                UpdateUserCenterPositions(true, true);

                                                ProgramCore.Project.RenderMainHelper.headController.SelectAutodotsFaceEllipse();
                                            }
                                        }
                                        else
                                            ProgramCore.Project.RenderMainHelper.headController.UpdateAutodotsPointSelection(e.X, e.Y, true);
                                    }
                                    else
                                    {
                                        RecalcEyeMouthRect();

                                        if (ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.HeadAutodots)
                                        {
                                            ProgramCore.MainForm.ctrlRenderControl.CalcReflectedBitmaps();
                                            ProgramCore.Project.RenderMainHelper.headController.EndAutodots(false);
                                            ProgramCore.MainForm.ctrlRenderControl.ApplySmoothedTextures();

                                            for (var i = 0; i < ProgramCore.Project.RenderMainHelper.headController.AutoDotsv2.Count; i++)      // после слияние с ShapeDots. Проверить!
                                            {
                                          /*      var p = ProgramCore.Project.RenderMainHelper.headController.AutoDots[i];      //TODO АЛГОРИТМЫ АВТОДОТСОВ 29.05.2018

                                                if (p.Selected)
                                                    ctrlRenderControl.autodotsShapeHelper.Transform(p.Value, i); // точка в мировых координатах*/
                                            }

                                            switch (ProgramCore.CurrentProgram)
                                            {
                                                case ProgramCore.ProgramMode.HeadShop_Rotator:
                                                case ProgramCore.ProgramMode.HeadShop_v11:
                                                    if (ProgramCore.Project.MirrorUsed)
                                                    {
                                                        ProgramCore.Project.RenderMainHelper.headMeshesController.Mirror(ProgramCore.Project.RenderMainHelper.headMeshesController.RenderMesh.HeadAngle > 0.0f, 0.0f);
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                }
                                break;
                            /*      case Mode.HeadShapedots:
                                      if (ProgramCore.Project.ShapeFlip != FlipType.None)
                                          return;

                                      if (!startMove && !dblClick)
                                      {
                                          if (!shiftKeyPressed)
                                              ProgramCore.Project.RenderMainHelper.headController.ShapeDots.ClearSelection();

                                          if (e.X >= MouthTransformed.X - HalfPointRectSize && e.X <= MouthTransformed.X + HalfPointRectSize && e.Y >= MouthTransformed.Y - HalfPointRectSize && e.Y <= MouthTransformed.Y + HalfPointRectSize) // рот
                                              ProgramCore.Project.RenderMainHelper.headController.SelectShapedotsMouth();
                                          else if (e.X >= LeftEyeTransformed.X - HalfPointRectSize && e.X <= LeftEyeTransformed.X + HalfPointRectSize && e.Y >= LeftEyeTransformed.Y - HalfPointRectSize && e.Y <= LeftEyeTransformed.Y + HalfPointRectSize) // левый глаз
                                              ProgramCore.Project.RenderMainHelper.headController.SelectShapedotsLeftEye();
                                          else if (e.X >= RightEyeTransformed.X - HalfPointRectSize && e.X <= RightEyeTransformed.X + HalfPointRectSize && e.Y >= RightEyeTransformed.Y - HalfPointRectSize && e.Y <= RightEyeTransformed.Y + HalfPointRectSize) // правый глаз
                                              ProgramCore.Project.RenderMainHelper.headController.SelectShapedotsRightEye();
                                          else if (e.X >= NoseTransformed.X - HalfPointRectSize && e.X <= NoseTransformed.X + HalfPointRectSize && e.Y >= NoseTransformed.Y - HalfPointRectSize && e.Y <= NoseTransformed.Y + HalfPointRectSize) // нос
                                              ProgramCore.Project.RenderMainHelper.headController.SelectShapedotsNose();
                                          else if (e.X >= CentralFacePoint.X - HalfPointRectSize && e.X <= CentralFacePoint.X + HalfPointRectSize && e.Y >= CentralFacePoint.Y - HalfPointRectSize && e.Y <= CentralFacePoint.Y + HalfPointRectSize) // прямоугольник и выделение всех точек
                                          {
                                              if (RectTransformMode)
                                              {
                                                  RectTransformMode = false;
                                                  ProgramCore.Project.RenderMainHelper.headController.ShapeDots.ClearSelection();
                                              }
                                              else
                                              {
                                                  RectTransformMode = true;
                                                  UpdateUserCenterPositions(true, true);

                                                  ProgramCore.Project.RenderMainHelper.headController.SelectShapedotsFaceEllipse();
                                              }
                                          }
                                          else
                                              ProgramCore.Project.RenderMainHelper.headController.UpdateShapedotsPointSelection(e.X, e.Y, true);
                                      }
                                      else
                                      {
                                          for (var i = 0; i < ProgramCore.Project.RenderMainHelper.headController.ShapeDots.Count; i++)
                                          {
                                              var p = ProgramCore.Project.RenderMainHelper.headController.ShapeDots[i];

                                              if (p.Selected)
                                                  ProgramCore.MainForm.ctrlRenderControl.autodotsShapeHelper.Transform(p.Value, i); // точка в мировых координатах
                                          }
                                      }
                                      break;*/
                            case Mode.HeadAutodotsLassoStart:
                                headAutodotsLassoPoints.Add(new Vector2(e.X, e.Y));
                                break;
                            /*    case Mode.HeadShapedotsLassoStart:
                                    headShapedotsLassoPoints.Add(new Vector2(e.X, e.Y));
                                    break;*/
                            case Mode.HeadLine:
                                {
                                    if (ProgramCore.Project.ShapeFlip != FlipType.None)
                                        return;

                                    if (ProgramCore.MainForm.HeadFront)
                                    {
                                        #region вид спереди

                                        if (!startMove && !dblClick)
                                        {
                                            #region Проверяем, начали ли что-то обводить линиями

                                            var firstTime = false;
                                            if (e.X >= MouthTransformed.X - 2.5 && e.X <= MouthTransformed.X + 2.5 && e.Y >= MouthTransformed.Y - 2.5 && e.Y <= MouthTransformed.Y + 2.5) // рот
                                            {
                                                if (ProgramCore.MainForm.ctrlRenderControl.HeadLineMode != MeshPartType.Lip)
                                                {
                                                    firstTime = true;
                                                    ProgramCore.MainForm.ctrlRenderControl.HeadLineMode = MeshPartType.Lip;
                                                }
                                            }
                                            else if (e.X >= LeftEyeTransformed.X - 2.5 && e.X <= LeftEyeTransformed.X + 2.5 && e.Y >= LeftEyeTransformed.Y - 2.5 && e.Y <= LeftEyeTransformed.Y + 2.5) // левый глаз
                                            {
                                                firstTime = true;
                                                ProgramCore.MainForm.ctrlRenderControl.HeadLineMode = MeshPartType.LEye;
                                            }
                                            else if (e.X >= RightEyeTransformed.X - 2.5 && e.X <= RightEyeTransformed.X + 2.5 && e.Y >= RightEyeTransformed.Y - 2.5 && e.Y <= RightEyeTransformed.Y + 2.5) // правый глаз
                                            {
                                                firstTime = true;
                                                ProgramCore.MainForm.ctrlRenderControl.HeadLineMode = MeshPartType.REye;
                                            }
                                            else if (e.X >= NoseTransformed.X - 2.5 && e.X <= NoseTransformed.X + 2.5 && e.Y >= NoseTransformed.Y - 2.5 && e.Y <= NoseTransformed.Y + 2.5) // нос
                                            {
                                                firstTime = true;
                                                ProgramCore.MainForm.ctrlRenderControl.HeadLineMode = MeshPartType.Nose;
                                            }
                                            else if (e.X >= CentralFacePoint.X - 2.5 && e.X <= CentralFacePoint.X + 2.5 && e.Y >= CentralFacePoint.Y - 2.5 && e.Y <= CentralFacePoint.Y + 2.5)
                                            {
                                                firstTime = true;
                                                ProgramCore.MainForm.ctrlRenderControl.HeadLineMode = MeshPartType.Head;
                                            }

                                            #endregion

                                            if (firstTime)          // выбираем режим линии
                                            {
                                                ProgramCore.Project.RenderMainHelper.headController.Lines.Clear();
                                                ctrlRenderControl.autodotsShapeHelper.ResetPoints(ProgramCore.MainForm.ctrlRenderControl.HeadLineMode);
                                            }
                                            else if (ProgramCore.MainForm.ctrlRenderControl.HeadLineMode != MeshPartType.None)          // добавляем новые точки
                                            {
                                                var point = new MirroredHeadPoint(headLastPointRelative, headLastPointRelative, false);
                                                point.UpdateWorldPoint();

                                                #region Проверка на количество линий и режим выделения

                                                if (ProgramCore.Project.RenderMainHelper.headController.Lines.Count > 1)
                                                {
                                                    var condition = false;
                                                    switch (ProgramCore.MainForm.ctrlRenderControl.HeadLineMode)
                                                    {
                                                        case MeshPartType.Lip:
                                                            if (ProgramCore.Project.RenderMainHelper.headController.Lines.Count > 2)
                                                                condition = true;
                                                            break;
                                                        default:
                                                            if (ProgramCore.Project.RenderMainHelper.headController.Lines.Count > 1)

                                                                condition = true;
                                                            break;
                                                    }

                                                    if (condition) // если ничего не выделили - начинаем рисовать новую линию. иначе уходим в режим выделения и таскания точек
                                                    {
                                                        if (!shiftKeyPressed)
                                                            ProgramCore.Project.RenderMainHelper.headController.ClearPointsSelection();

                                                        if (ProgramCore.Project.RenderMainHelper.headController.UpdatePointSelection(point.Value.X, point.Value.Y))
                                                            LineSelectionMode = true;
                                                        else
                                                        {
                                                            if (LineSelectionMode)
                                                            {
                                                                LineSelectionMode = false;
                                                                ProgramCore.Project.RenderMainHelper.headController.ClearPointsSelection();
                                                                break;
                                                            }
                                                            else
                                                                ProgramCore.Project.RenderMainHelper.headController.Lines.Clear();
                                                        }
                                                    }
                                                }

                                                #endregion

                                                if (!LineSelectionMode)
                                                {
                                                    #region Добавляем новые точки линии

                                                    if (ProgramCore.Project.RenderMainHelper.headController.Lines.Count == 0)
                                                    {
                                                        var line = new HeadLine();
                                                        line.Add(point);
                                                        ProgramCore.Project.RenderMainHelper.headController.Lines.Add(line);
                                                    }
                                                    else
                                                    {
                                                        var currentLine = ProgramCore.Project.RenderMainHelper.headController.Lines.Last();
                                                        var hasIntersections = false;

                                                        if (currentLine.Count > 1) // проверка на пересечения линий
                                                        {
                                                            var lastPoint = currentLine.Last();

                                                            float ua, ub;
                                                            for (var i = currentLine.Count - 2; i >= 0; i--)
                                                            {
                                                                var pointA = currentLine[i];
                                                                var pointB = currentLine[i + 1];
                                                                if (AutodotsShapeHelper.GetUaUb(ref lastPoint.Value, ref point.Value, ref pointA.Value, ref pointB.Value, out ua, out ub))
                                                                {
                                                                    if (ua > 0 && ua < 1 && ub > 0 && ub < 1)
                                                                    {
                                                                        hasIntersections = true;
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        var inAnotherPoint = false;
                                                        if (ProgramCore.MainForm.ctrlRenderControl.HeadLineMode == MeshPartType.Lip && ProgramCore.Project.RenderMainHelper.headController.Lines.Count == 2)
                                                        {
                                                            // ЭТо вторая линия губ
                                                            foreach (var lPoint in ProgramCore.Project.RenderMainHelper.headController.Lines.First())
                                                                if (point.Value.X >= lPoint.Value.X - 0.25 && point.Value.X <= lPoint.Value.X + 0.25 && point.Value.Y >= lPoint.Value.Y - 0.25 && point.Value.Y <= lPoint.Value.Y + 0.25 && !currentLine.Contains(lPoint))
                                                                {
                                                                    if (currentLine.Count == 0)
                                                                        currentLine.Add(lPoint);
                                                                    else
                                                                    {
                                                                        currentLine.Add(lPoint);
                                                                        ProgramCore.Project.RenderMainHelper.headController.Lines.Add(new HeadLine());
                                                                    }
                                                                    inAnotherPoint = true;
                                                                    break;
                                                                }
                                                            if (currentLine.Count == 0) //первую точку добавляем всегда в пересечении с другой точкой.
                                                                inAnotherPoint = true;
                                                        }

                                                        // прочие случаи
                                                        if (!hasIntersections && !inAnotherPoint)
                                                        {
                                                            var firstPoint = currentLine.First();
                                                            if (point.Value.X >= firstPoint.Value.X - 0.25 && point.Value.X <= firstPoint.Value.X + 0.25 && point.Value.Y >= firstPoint.Value.Y - 0.25 && point.Value.Y <= firstPoint.Value.Y + 0.25)
                                                            {
                                                                currentLine.Add(firstPoint);
                                                                ProgramCore.Project.RenderMainHelper.headController.Lines.Add(new HeadLine());
                                                            }
                                                            else
                                                                currentLine.Add(point);
                                                        }
                                                    }

                                                    #endregion
                                                }
                                            }
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        #region Вид сбоку

                                        if (isProfileSmoothing)
                                            return;

                                        if (!startMove && !dblClick)
                                        {
                                            var point = new MirroredHeadPoint(headLastPointRelative, headLastPointRelative, false);
                                            point.UpdateWorldPoint();

                                            #region Проверка на количество линий и режим выделения
                                            if (ProgramCore.Project.RenderMainHelper.headController.Lines.Count > 1) // если ничего не выделили - начинаем рисовать новую линию. иначе уходим в режим выделения и таскания точек
                                            {
                                                if (!shiftKeyPressed)
                                                    ProgramCore.Project.RenderMainHelper.headController.ClearPointsSelection();

                                                if (ProgramCore.Project.RenderMainHelper.headController.UpdatePointSelection(point.Value.X, point.Value.Y))
                                                    LineSelectionMode = true;
                                                else
                                                {
                                                    if (LineSelectionMode)
                                                    {
                                                        LineSelectionMode = false;
                                                        ProgramCore.Project.RenderMainHelper.headController.ClearPointsSelection();
                                                        break;
                                                    }
                                                }
                                            }
                                            #endregion

                                            if (!LineSelectionMode)
                                            {
                                                #region Добавляем новые точки линии

                                                if (ProgramCore.Project.RenderMainHelper.headController.Lines.Count == 0)
                                                {
                                                    var line = new HeadLine();
                                                    line.Add(point);
                                                    ProgramCore.Project.RenderMainHelper.headController.Lines.Add(line);
                                                }
                                                else
                                                {
                                                    var currentLine = ProgramCore.Project.RenderMainHelper.headController.Lines.Last();
                                                    var hasIntersections = false;

                                                    if (currentLine.Count > 1) // проверка на пересечения линий
                                                    {
                                                        var lastPoint = currentLine.Last();

                                                        float ua, ub;
                                                        for (var i = currentLine.Count - 2; i >= 0; i--)
                                                        {
                                                            var pointA = currentLine[i];
                                                            var pointB = currentLine[i + 1];
                                                            if (AutodotsShapeHelper.GetUaUb(ref lastPoint.Value, ref point.Value, ref pointA.Value, ref pointB.Value, out ua, out ub))
                                                            {
                                                                if (ua > 0 && ua < 1 && ub > 0 && ub < 1)
                                                                {
                                                                    hasIntersections = true;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }

                                                    // прочие случаи
                                                    if (!hasIntersections)
                                                        currentLine.Add(point);
                                                }

                                                #endregion
                                            }
                                        }

                                        #endregion
                                    }

                                }
                                break;
                            case Mode.None:
                                {
                                    if (ProgramCore.MainForm.HeadProfile)
                                    {
                                        switch (ControlPointsMode)
                                        {
                                            case ProfileControlPointsMode.SetControlPoints:  // в профиле. расставляем опорные точки
                                                {
                                                    if (headLastPointRelative != Vector2.Zero)
                                                    {
                                                        profileControlPoints[profileControlPointIndex].ValueMirrored = headLastPointRelative;
                                                        ++profileControlPointIndex;

                                                        if (profileControlPointIndex == 4)
                                                        {
                                                            ControlPointsMode = ProfileControlPointsMode.MoveControlPoints;
                                                            profileControlPointIndex = 0;
                                                        }
                                                    }
                                                }
                                                break;
                                            case ProfileControlPointsMode.MoveControlPoints:  // выделяем и двигаем опорные точки
                                                {
                                                    if (!startMove && !dblClick)
                                                    {
                                                        if (!shiftKeyPressed)
                                                            foreach (var point in profileControlPoints)
                                                                point.Selected = false;

                                                        foreach (var point in profileControlPoints)
                                                        {
                                                            var pointK = new Vector2(point.ValueMirrored.X * ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateWidth + ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateOffsetX,
                                                                                     point.ValueMirrored.Y * ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateHeight + ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateOffsetY);
                                                            if (e.X >= pointK.X - 5 && e.X <= pointK.X + 5 && e.Y >= pointK.Y - 5 && e.Y <= pointK.Y + 5)
                                                            {
                                                                point.Selected = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            case ProfileControlPointsMode.UpdateRightLeft:  // выделяем и двигаем опорные точки
                                                {
                                                    if (!startMove && !dblClick)
                                                    {
                                                        if (!shiftKeyPressed)
                                                            foreach (var point in profileControlPoints)
                                                                point.Selected = false;

                                                        for (var i = 0; i < profileControlPoints.Count; i += 3)
                                                        {
                                                            var point = profileControlPoints[i];
                                                            var pointK = new Vector2(point.ValueMirrored.X * ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateWidth + ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateOffsetX,
                                                                                     point.ValueMirrored.Y * ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateHeight + ProgramCore.MainForm.ctrlTemplateImage.ImageTemplateOffsetY);
                                                            if (e.X >= pointK.X - 5 && e.X <= pointK.X + 5 && e.Y >= pointK.Y - 5 && e.Y <= pointK.Y + 5)
                                                            {
                                                                point.Selected = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    else if (startMove)
                                                    {
                                                        UpdateProfileRectangle();
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                                break;
                        }

                        #endregion

                        break;
                }
            }

            moveRectIndex = -1;

            startMove = false;
            leftMousePressed = false;
            dblClick = false;
            headLastPointRelative = Vector2.Zero;
            headTempPoints.Clear();
            Cursor = Cursors.Arrow;
        }

        private void btnCopyProfileImg_MouseDown(object sender, MouseEventArgs e)
        {
            btnCopyProfileImg.Image = Resources.copyArrowPressed;
        }
        public void btnCopyProfileImg_MouseUp(object sender, MouseEventArgs e)
        {
            btnCopyProfileImg.Image = Resources.copyArrowNormal;

            var projectPath = Path.Combine(ProgramCore.Project.ProjectPath, "ProfileImage.jpg");
            using (var img = ProgramCore.MainForm.ctrlRenderControl.GrabScreenshot(string.Empty, ProgramCore.MainForm.ctrlRenderControl.ClientSize.Width, ProgramCore.MainForm.ctrlRenderControl.ClientSize.Height))
            {
                img.Save(projectPath);
                ProgramCore.Project.ProfileImage = new Bitmap(img);
            }
            SetTemplateImage(ProgramCore.Project.ProfileImage, false);
            ProgramCore.Project.ProfileEyeLocation = Vector2.Zero;
            ProgramCore.Project.ProfileMouthLocation = Vector2.Zero;
            RecalcProfilePoints();
        }

        private void btnNewProfilePict_MouseDown(object sender, MouseEventArgs e)
        {
            btnNewProfilePict.Image = Resources.newProfilePictPressed;
        }
        private void btnNewProfilePict_MouseUp(object sender, MouseEventArgs e)
        {
            btnNewProfilePict.Image = Resources.newProfilePictNormal;

            ProgramCore.Project.RenderMainHelper.headController.LoadNewProfileImage();
        }

        public void RecalcProfilePoints()
        {

            if (profileControlPoints.Count > 0 && (ProgramCore.Project.ProfileEyeLocation == Vector2.Zero || ProgramCore.Project.ProfileMouthLocation == Vector2.Zero))
            {
                ProfileScreenTopLocation = GetScreenPoint(profileControlPoints[0].Value);
                ProgramCore.Project.ProfileEyeLocation = ProfileScreenEyeLocation = GetScreenPoint(profileControlPoints[1].Value);
                ProgramCore.Project.ProfileMouthLocation = ProfileScreenMouthLocation = GetScreenPoint(profileControlPoints[2].Value);
                ProfileScreenBottomLocation = GetScreenPoint(profileControlPoints[3].Value);
            }

            #region Пересчитываем точки справа на лево

            var width = ProgramCore.MainForm.ctrlRenderControl.camera.WindowWidth * ProgramCore.MainForm.ctrlRenderControl.camera.Scale;
            var height = ProgramCore.MainForm.ctrlRenderControl.camera.WindowHeight * ProgramCore.MainForm.ctrlRenderControl.camera.Scale;

            var centerPosition = new Vector2(0, ProgramCore.MainForm.ctrlRenderControl.camera.Position.Y + ProgramCore.MainForm.ctrlRenderControl.camera.dy);
            var offsetX = centerPosition.X - width * 0.5f;
            var offsetY = centerPosition.Y - height * 0.5f;

            foreach (var point in profileControlPoints)
                point.ValueMirrored = new Vector2((point.Value.X - offsetX) / width, 1 - ((point.Value.Y - offsetY) / height));

            #endregion
        }


        #endregion
    }

    public enum ProfileControlPointsMode
    {
        SetControlPoints,  // режим выставления опорных точек в профиле
        MoveControlPoints,     // опорные точки выставлены, можем таскать их
        UpdateRightLeft,    // сопоставляем правую и левую картинки
        None
    }
}
