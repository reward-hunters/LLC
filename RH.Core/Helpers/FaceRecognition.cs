using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Luxand;
using OpenTK;
using RH.Core.IO;
using RH.Core.Render.Helpers;

namespace RH.Core.Helpers
{

    public class LuxandFaceRecognition
    {
        public static float[] GetPointDepths()
        {
            //Гулбины точек (1 = длина носа)
            float[] PointDepths =  {
                0.0f, // глаз
                0.0f, // глаз
                -1.0f, // кончик носа
                0.0f, // левый край губы
                0.0f, // правый край губы

                1.5f, // 5 Подбородок
                1.5f, // 6
                1.4f, // 7
                1.4f, // 8
                1.3f, // 9
                1.3f, // 10
                1.2f, // 11 Подбородок

                0.0f, // 12 Брови
                0.0f, // 13
                0.0f, // 14
                0.0f, // 15
                0.0f, // 16
                0.0f, // 17
                0.0f, // 18
                0.0f, // 19
                0.0f, // 20
                0.0f, // 21 Брови

                0.0f, // 22 Верх центра носа

                1.2f, // 23 Глаза
                1.2f, // 24
                1.2f, // 25
                1.2f, // 26
                1.2f, // 27
                1.2f, // 28
                1.2f, // 29
                1.2f, // 30
                1.2f, // 31
                1.2f, // 32
                1.2f, // 33
                1.2f, // 34
                1.2f, // 35
                1.2f, // 36
                1.2f, // 37
                1.2f, // 38
                1.2f, // 39
                1.2f, // 40
                1.2f, // 41
                1.2f, // 42 Глаза

                0.0f, // 43 Нос
                0.0f, // 44
                0.0f, // 45
                0.0f, // 46
                -0.5f, // 47
                -0.5f, // 48
                0.0f, // 49 Нос

                0.0f, // 50 Щеки
                0.0f, // 51 
                0.0f, // 52
                0.0f, // 53 Щеки

                0.0f, // 54 Губы
                0.0f, // 55
                0.0f, // 56
                0.0f, // 57
                0.0f, // 58
                0.0f, // 59
                0.0f, // 60
                0.0f, // 61
                0.0f, // 62
                0.0f, // 63
                0.0f, // 64
                0.0f, // 65 Губы

                2.0f, // 66 Уши
                2.0f, // 67
                1.5f, // 68
                1.5f, // 69 Уши
            };

            return PointDepths;
        }

        /// <summary> Есть ли улыбка с открытым ртом на фото. </summary>
        public bool IsOpenSmile { get; private set; }

        public RectangleF FaceRectRelative;
        public Vector4 FaceColor;

        public Vector2 LeftEyeCenter;
        public Vector2 RightEyeCenter;

        public Vector2 LeftMouth;
        public Vector2 RightMouth;

        public Vector2 LeftNose;
        public Vector2 RightNose;

        public Vector2 TopFace;
        public Vector2 MiddleFace1;
        public Vector2 MiddleFace2;
        public Vector2 BottomFace;

        private Vector2 RightMiddleFace1;
        private Vector2 RightMiddleFace2;

        public float GetMinX()
        {
            var minX = float.MaxValue;
            minX = Math.Min(TopFace.X, minX);
            minX = Math.Min(MiddleFace1.X, minX);
            minX = Math.Min(MiddleFace2.X, minX);
            return minX;
        }
        public float GetMaxX()
        {
            var maxX = float.MinValue;
            maxX = Math.Max(TopFace.X, maxX);
            maxX = Math.Max(RightMiddleFace1.X, maxX);
            maxX = Math.Max(RightMiddleFace2.X, maxX);
            return maxX;
        }

        /// <summary> Не отнормированные </summary>
        public List<Vector2> RealPoints;
        public List<Vector3> FacialFeatures;
        public bool IsMale;

        /// <summary> Угол, на который повернута голова. </summary>
        public double RotatedAngle;

        private int angleCount = 0;

        public static Vector4 GetFaceColor(Bitmap bmpImage, FSDK.TPoint[] pointFeature)
        {
            var distance = pointFeature[2].y - pointFeature[11].y;
            var top = pointFeature[16].y + distance - 15;          // определение высоты по алгоритму старикана
            top = top < 0 ? 0 : top;

            var colorPoints = new List<PointF>();
            var forehead = new PointF(pointFeature[22].x, pointFeature[16].y + (pointFeature[16].y - top) * 0.5f);
            colorPoints.Add(forehead);
            colorPoints.Add(new PointF(pointFeature[22].x, pointFeature[22].y));

            colorPoints.Add(new PointF(pointFeature[42].x, pointFeature[42].y));
            colorPoints.Add(new PointF(pointFeature[43].x, pointFeature[43].y));
            colorPoints.Add(new PointF(pointFeature[44].x, pointFeature[44].y));
            colorPoints.Add(new PointF(pointFeature[46].x, pointFeature[46].y));
            colorPoints.Add(new PointF(pointFeature[50].x, pointFeature[50].y));
            colorPoints.Add(new PointF(pointFeature[52].x, pointFeature[52].y));
            colorPoints.Add(new PointF(pointFeature[51].x, pointFeature[51].y));
            colorPoints.Add(new PointF(pointFeature[53].x, pointFeature[53].y));
            colorPoints.Add(new PointF(pointFeature[2].x, pointFeature[2].y));

            int r = 0;
            int g = 0;
            int b = 0;
            foreach (var point in colorPoints)
            {
                var colorH = bmpImage.GetPixel((int)point.X, (int)point.Y);
                r += colorH.R;
                g += colorH.G;
                b += colorH.B;
            }
            r /= colorPoints.Count;
            g /= colorPoints.Count;
            b /= colorPoints.Count;

            var color = Color.FromArgb(r, g, b);
            return new Vector4((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, 1.0f);
        }

        public bool Recognize(ref string path, bool needCrop, bool needRotation = true)
        {
            FaceRectRelative = RectangleF.Empty;
            LeftEyeCenter = RightEyeCenter = LeftMouth = LeftNose = RightNose = RightMouth = Vector2.Zero;

            var executablePath = Path.GetDirectoryName(Application.ExecutablePath);
            FSDK.TPoint[] pointFeature;
            var image = new FSDK.CImage(path);

            var faceRectangle = Rectangle.Empty;
            var mouthRectangle = Rectangle.Empty;

            FSDK.SetFaceDetectionThreshold(5);
            FSDK.SetFaceDetectionParameters(true, true, 512);
            var facePosition = image.DetectFace();

            if (0 == facePosition.w)
            {
                MessageBox.Show("No faces detected", "Face Detection");
                return false;
            }

            if (needCrop)
                RotatedAngle = facePosition.angle;      // угол, на который повернута голова.

            pointFeature = image.DetectFacialFeaturesInRegion(ref facePosition);

            String AttributeValues;         // определение пола
            FSDK.DetectFacialAttributeUsingFeatures(image.ImageHandle, ref pointFeature, "Gender", out AttributeValues, 1024);
            var ConfidenceMale = 0.0f;
            var ConfidenceFemale = 0.0f;
            var Age = 0.0f;             // в этой версии распознавалки не работает.
            FSDK.GetValueConfidence(AttributeValues, "Male", ref ConfidenceMale);
            FSDK.GetValueConfidence(AttributeValues, "Female", ref ConfidenceFemale);
            IsMale = ConfidenceMale > ConfidenceFemale;

            FSDK.DetectFacialAttributeUsingFeatures(image.ImageHandle, ref pointFeature, "Age", out AttributeValues, 1024);

            var left = facePosition.xc - (int)(facePosition.w * 0.6f);
            left = left < 0 ? 0 : left;
            //   int top = facePosition.yc - (int)(facePosition.w * 0.5f);             // верхушку определяет неправильлно. поэтому просто не будем обрезать :)
            BottomFace = new Vector2(pointFeature[11].x, pointFeature[11].y);

            var distance = pointFeature[2].y - pointFeature[11].y;
            var top = pointFeature[16].y + distance - 15;          // определение высоты по алгоритму старикана
            top = top < 0 ? 0 : top;

            var newWidth = (int)(facePosition.w * 1.2);
            newWidth = newWidth > image.Width || newWidth == 0 ? image.Width : newWidth;

            faceRectangle = new Rectangle(left, top, newWidth, BottomFace.Y + 15 < image.Height ? (int)(BottomFace.Y + 15) - top : image.Height - top - 1);
            if (needCrop)       // если это создание проекта - то нужно обрезать фотку и оставить только голову
            {
                var bmpImage = new Bitmap(path);
                FaceColor = GetFaceColor(bmpImage, pointFeature);

                var croppedImage = ImageEx.Crop(bmpImage, faceRectangle);
                path = UserConfig.AppDataDir;
                FolderEx.CreateDirectory(path);
                path = Path.Combine(path, "tempHaarImage.jpg");
                croppedImage.Save(path, ImageFormat.Jpeg);
                croppedImage.Dispose();

                return Recognize(ref path, false);
            }

            LeftEyeCenter = new Vector2(pointFeature[0].x, pointFeature[0].y);
            RightEyeCenter = new Vector2(pointFeature[1].x, pointFeature[1].y);

            LeftMouth = new Vector2(pointFeature[3].x, pointFeature[3].y);
            RightMouth = new Vector2(pointFeature[4].x, pointFeature[4].y);

            LeftNose = new Vector2(pointFeature[45].x, pointFeature[45].y);
            RightNose = new Vector2(pointFeature[46].x, pointFeature[46].y);

            TopFace = new Vector2(pointFeature[66].x, pointFeature[66].y);
            MiddleFace1 = new Vector2(pointFeature[66].x, pointFeature[66].y);
            MiddleFace2 = new Vector2(pointFeature[5].x, pointFeature[5].y);


            RightMiddleFace1 = new Vector2(pointFeature[67].x, pointFeature[67].y);
            RightMiddleFace2 = new Vector2(pointFeature[6].x, pointFeature[6].y);

            #region Поворот фотки по глазам!

            if (needRotation)
            {
                var v = new Vector2(LeftEyeCenter.X - RightEyeCenter.X, LeftEyeCenter.Y - RightEyeCenter.Y);
                v.Normalize();      // ПД !
                var xVector = new Vector2(1, 0);

                var xDiff = xVector.X - v.X;
                var yDiff = xVector.Y - v.Y;
                var angle = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;

                if (Math.Abs(angle) > 1 && angleCount <= 5)                // поворачиваем наклоненные головы
                {
                    ++angleCount;

                    using (var ms = new MemoryStream(File.ReadAllBytes(path))) // Don't use using!!
                    {
                        var originalImg = (Bitmap)Image.FromStream(ms);

                        path = UserConfig.AppDataDir;
                        FolderEx.CreateDirectory(path);
                        path = Path.Combine(path, "tempHaarImage.jpg");

                        using (var ii = ImageEx.RotateImage(new Bitmap(originalImg), (float)-angle))
                            ii.Save(path, ImageFormat.Jpeg);
                    }

                    return Recognize(ref path, false);
                }
            }

            #endregion

            var upperUpperLip = pointFeature[54];       // вехняя точка верхней губы
            var lowerUpperLip = pointFeature[61];            // нижняя точка верхней губы
            var lowerLip = pointFeature[64];            // верхняя точка нижней губы

            var diff2 = Math.Abs(lowerUpperLip.y - upperUpperLip.y);
            var diffX = Math.Abs(lowerLip.y - lowerUpperLip.y);

            IsOpenSmile = diffX > diff2;

            #region Переводим в относительные координаты

            LeftMouth = new Vector2(LeftMouth.X / (image.Width * 1f), LeftMouth.Y / (image.Height * 1f));
            RightMouth = new Vector2(RightMouth.X / (image.Width * 1f), RightMouth.Y / (image.Height * 1f));

            LeftEyeCenter = new Vector2(LeftEyeCenter.X / (image.Width * 1f), LeftEyeCenter.Y / (image.Height * 1f));
            RightEyeCenter = new Vector2(RightEyeCenter.X / (image.Width * 1f), RightEyeCenter.Y / (image.Height * 1f));

            LeftNose = new Vector2(LeftNose.X / (image.Width * 1f), LeftNose.Y / (image.Height * 1f));
            RightNose = new Vector2(RightNose.X / (image.Width * 1f), RightNose.Y / (image.Height * 1f));

            TopFace = new Vector2(TopFace.X / (image.Width * 1f), TopFace.Y / (image.Height * 1f));
            MiddleFace1 = new Vector2(MiddleFace1.X / (image.Width * 1f), MiddleFace1.Y / (image.Height * 1f));
            MiddleFace2 = new Vector2(MiddleFace2.X / (image.Width * 1f), MiddleFace2.Y / (image.Height * 1f));
            BottomFace = new Vector2(BottomFace.X / (image.Width * 1f), BottomFace.Y / (image.Height * 1f));

            RightMiddleFace1 = new Vector2(RightMiddleFace1.X / (image.Width * 1f), RightMiddleFace1.Y / (image.Height * 1f));
            RightMiddleFace2 = new Vector2(RightMiddleFace2.X / (image.Width * 1f), RightMiddleFace2.Y / (image.Height * 1f));

            FacialFeatures = new List<Vector3>();
            RealPoints = new List<Vector2>();
            int index = 0;
            var pointDepths = GetPointDepths();
            foreach (var point in pointFeature)
            {
                FacialFeatures.Add(new Vector3(point.x / (image.Width * 1f), point.y / (image.Height * 1f), pointDepths[index++]));
                RealPoints.Add(new Vector2(point.x, point.y));
            }

            #endregion

            return true;
        }
    }

    public class Cheek
    {
        public PointF TopCheek;
        public PointF TopCheekTransformed;
        public bool TopVisible = true;
        public bool TopTouched;

        public PointF CenterCheek;
        public PointF CenterCheekTransformed;
        public bool CenterVisible = true;
        public bool CenterTouched;

        public PointF DownCheek;
        public PointF DownCheekTransformed;
        public bool DownVisible = true;
        public bool DownTouched;

        private const int HalfCircleRadius = 15;
        private const int CircleSmallRadius = 8;
        private const int HalfCircleSmallRadius = 4;

        public Cheek(float xPos, float centerY)
        {
            CenterCheek = new PointF(xPos, centerY);
            TopCheek = new PointF(xPos, centerY - centerY / 2f);
            DownCheek = new PointF(xPos, centerY + centerY / 2f);
        }

        public void Transform(float imageWidth, float imageHeight, float offsetX, float offsetY)
        {
            TopCheekTransformed = new PointF(TopCheek.X * imageWidth + offsetX, TopCheek.Y * imageHeight + offsetY);

            CenterCheekTransformed = new PointF(CenterCheek.X * imageWidth + offsetX, CenterCheek.Y * imageHeight + offsetY);

            DownCheekTransformed = new PointF(DownCheek.X * imageWidth + offsetX, DownCheek.Y * imageHeight + offsetY);
        }

        public int CheckGrab(float x, float y, bool mousePressed)
        {
            if (x >= TopCheekTransformed.X - HalfCircleSmallRadius && x <= TopCheekTransformed.X + HalfCircleSmallRadius &&
                y >= TopCheekTransformed.Y - HalfCircleSmallRadius && y <= TopCheekTransformed.Y + HalfCircleSmallRadius)
            {
                if (mousePressed)
                {
                    TopVisible = true;
                    TopTouched = true;
                }
                return 0;
            }

            if (x >= CenterCheekTransformed.X - HalfCircleSmallRadius &&
                x <= CenterCheekTransformed.X + HalfCircleSmallRadius &&
                y >= CenterCheekTransformed.Y - HalfCircleSmallRadius &&
                y <= CenterCheekTransformed.Y + HalfCircleSmallRadius)
            {
                if (mousePressed)
                {
                    CenterVisible = true;
                    CenterTouched = true;
                }
                return 1;
            }

            if (x >= DownCheekTransformed.X - HalfCircleSmallRadius &&
                x <= DownCheekTransformed.X + HalfCircleSmallRadius &&
                y >= DownCheekTransformed.Y - HalfCircleSmallRadius &&
                y <= DownCheekTransformed.Y + HalfCircleSmallRadius)
            {
                if (mousePressed)
                {
                    DownVisible = true;
                    DownTouched = true;
                }
                return 2;
            }

            return -1;
        }

        public void UpdateVisibility()
        {
            if (!TopTouched)
                TopVisible = !TopVisible;
            if (!CenterTouched)
                CenterVisible = !CenterVisible;
            if (!DownTouched)
                DownVisible = !DownVisible;
        }

        public float GetMaxX()
        {
            var max = float.MinValue;
            max = Math.Max(TopCheek.X, max);
            max = Math.Max(CenterCheek.X, max);
            max = Math.Max(DownCheek.X, max);
            return max;
        }
        public float GetMinX()
        {
            var min = float.MaxValue;
            min = Math.Min(TopCheek.X, min);
            min = Math.Min(CenterCheek.X, min);
            min = Math.Min(DownCheek.X, min);
            return min;
        }

        Pen arrowsPen = new Pen(Color.DarkOliveGreen, 2);
        public void DrawLeft(Graphics g)
        {
            if (TopVisible)
            {
                g.FillRectangle(Brushes.DarkOliveGreen, TopCheekTransformed.X - HalfCircleSmallRadius,
                    TopCheekTransformed.Y - HalfCircleSmallRadius, CircleSmallRadius, CircleSmallRadius);
                g.DrawLine(arrowsPen, TopCheekTransformed.X, TopCheekTransformed.Y,
                    TopCheekTransformed.X + HalfCircleRadius, TopCheekTransformed.Y + HalfCircleRadius);
            }

            if (CenterVisible)
            {
                g.FillRectangle(Brushes.DarkOliveGreen, CenterCheekTransformed.X - HalfCircleSmallRadius,
                    CenterCheekTransformed.Y - HalfCircleSmallRadius, CircleSmallRadius, CircleSmallRadius);
                g.DrawLine(arrowsPen, CenterCheekTransformed.X, CenterCheekTransformed.Y,
                    CenterCheekTransformed.X + HalfCircleRadius, CenterCheekTransformed.Y);
            }

            if (DownVisible)
            {
                g.FillRectangle(Brushes.DarkOliveGreen, DownCheekTransformed.X - HalfCircleSmallRadius,
                    DownCheekTransformed.Y - HalfCircleSmallRadius, CircleSmallRadius, CircleSmallRadius);
                g.DrawLine(arrowsPen, DownCheekTransformed.X, DownCheekTransformed.Y,
                    DownCheekTransformed.X + HalfCircleRadius, DownCheekTransformed.Y - HalfCircleRadius);
            }
        }
        public void DrawRight(Graphics g)
        {
            if (TopVisible)
            {
                g.FillRectangle(Brushes.DarkOliveGreen, TopCheekTransformed.X - HalfCircleSmallRadius,
                    TopCheekTransformed.Y - HalfCircleSmallRadius, CircleSmallRadius, CircleSmallRadius);
                g.DrawLine(arrowsPen, TopCheekTransformed.X, TopCheekTransformed.Y,
                    TopCheekTransformed.X - HalfCircleRadius, TopCheekTransformed.Y + HalfCircleRadius);
            }

            if (CenterVisible)
            {
                g.FillRectangle(Brushes.DarkOliveGreen, CenterCheekTransformed.X - HalfCircleSmallRadius,
                    CenterCheekTransformed.Y - HalfCircleSmallRadius, CircleSmallRadius, CircleSmallRadius);
                g.DrawLine(arrowsPen, CenterCheekTransformed.X, CenterCheekTransformed.Y,
                    CenterCheekTransformed.X - HalfCircleRadius, CenterCheekTransformed.Y);
            }

            if (DownVisible)
            {
                g.FillRectangle(Brushes.DarkOliveGreen, DownCheekTransformed.X - HalfCircleSmallRadius,
                    DownCheekTransformed.Y - HalfCircleSmallRadius, CircleSmallRadius, CircleSmallRadius);
                g.DrawLine(arrowsPen, DownCheekTransformed.X, DownCheekTransformed.Y,
                    DownCheekTransformed.X - HalfCircleRadius, DownCheekTransformed.Y - HalfCircleRadius);
            }
        }
    }
}