using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using Luxand;
using OpenTK;
using RH.Core.Helpers;
using RH.Core.IO;
using RH.Core.Render.Helpers;

namespace RH.WebCore
{
    public static class CropHelper
    {
        public static void Pass1(string path)
        {
            using (WebClient client = new WebClient())
            {
                byte[] imageBytes = client.DownloadData(path);

                using (var ms = new MemoryStream(imageBytes))
                {
                    var img = Image.FromStream(ms);
                }
            }
        }

        public static void Pass(string path)
        {
            using (WebClient client = new WebClient())
            {
                byte[] imageBytes = client.DownloadData(path);

                using (var ms = new MemoryStream(imageBytes))
                {
                    var img = Image.FromStream(ms);
                    using (var croppedImage = ImageEx.Crop(img, new Rectangle(0, 0, 20, 20)))
                    {

                        byte[] data;
                        using (MemoryStream m = new MemoryStream())
                        {
                            croppedImage.Save(m, ImageFormat.Jpeg);
                            data = m.ToArray();
                        }

                        FTPHelper ftpHelper = new FTPHelper(@"ftp://108.167.164.209", "i2q1d8b1", "B45B2nnFv$!j6V");
                        ftpHelper.Upload(new MemoryStream(data), "test1.jpeg");
                    }
                }
            }
        }

        public static void SaveToFTP(Image img)
        {
            byte[] data;
            using (MemoryStream m = new MemoryStream())
            {
                img.Save(m, ImageFormat.Jpeg);
                data = m.ToArray();
            }

            FTPHelper ftpHelper = new FTPHelper(@"ftp://108.167.164.209", "i2q1d8b1", "B45B2nnFv$!j6V");
            ftpHelper.Upload(new MemoryStream(data), "test1.jpeg");
        }


        public static void WebCropImage(Image sourceImage, bool needCrop)
        {
            var FaceRectRelative = RectangleF.Empty;

            FSDK.TPoint[] pointFeature;
            var image = new FSDK.CImage(sourceImage);


            var faceRectangle = Rectangle.Empty;
            var mouthRectangle = Rectangle.Empty;

            var facePosition = image.DetectFace();
            if (0 == facePosition.w)
            {
                SaveToFTP(sourceImage);
                return;
            }

            pointFeature = image.DetectFacialFeaturesInRegion(ref facePosition);

            var left = facePosition.xc - (int)(facePosition.w * 0.6f);
            left = left < 0 ? 0 : left;
            //   int top = facePosition.yc - (int)(facePosition.w * 0.5f);             // верхушку определяет неправильлно. поэтому просто не будем обрезать :)
            var BottomFace = new Vector2(pointFeature[11].x, pointFeature[11].y);

            var distance = pointFeature[2].y - pointFeature[11].y;
            var top = pointFeature[16].y + distance - 15; // определение высоты по алгоритму старикана
            top = top < 0 ? 0 : top;

            var newWidth = (int)(facePosition.w * 1.2);
            newWidth = newWidth > image.Width || newWidth == 0 ? image.Width : newWidth;

            faceRectangle = new Rectangle(left, top, newWidth,
                BottomFace.Y + 15 < image.Height ? (int)(BottomFace.Y + 15) - top : image.Height - top - 1);

            if (needCrop) // если это создание проекта - то нужно обрезать фотку и оставить только голову
            {
                using (var croppedImage = ImageEx.Crop(sourceImage, faceRectangle))
                    WebCropImage(croppedImage, false);
            }

            var LeftEyeCenter = new Vector2(pointFeature[0].x, pointFeature[0].y);
            var RightEyeCenter = new Vector2(pointFeature[1].x, pointFeature[1].y);

            var LeftMouth = new Vector2(pointFeature[3].x, pointFeature[3].y);
            var RightMouth = new Vector2(pointFeature[4].x, pointFeature[4].y);

            var LeftNose = new Vector2(pointFeature[45].x, pointFeature[45].y);
            var RightNose = new Vector2(pointFeature[46].x, pointFeature[46].y);

            var TopFace = new Vector2(pointFeature[66].x, pointFeature[66].y);
            var MiddleFace1 = new Vector2(pointFeature[66].x, pointFeature[66].y);
            var MiddleFace2 = new Vector2(pointFeature[5].x, pointFeature[5].y);


            var RightMiddleFace1 = new Vector2(pointFeature[67].x, pointFeature[67].y);
            var RightMiddleFace2 = new Vector2(pointFeature[6].x, pointFeature[6].y);

            #region Поворот фотки по глазам!

            var v = new Vector2(LeftEyeCenter.X - RightEyeCenter.X, LeftEyeCenter.Y - RightEyeCenter.Y);
            v.Normalize(); // ПД !
            var xVector = new Vector2(1, 0);

            var xDiff = xVector.X - v.X;
            var yDiff = xVector.Y - v.Y;
            var angle = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;

            if (Math.Abs(angle) > 1) // поворачиваем наклоненные головы
                sourceImage = ImageEx.RotateImage(new Bitmap(sourceImage), (float) -angle);

            #endregion

            SaveToFTP(sourceImage);
        }
    }
}