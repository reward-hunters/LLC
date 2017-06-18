using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using Luxand;
using OpenTK;
using RH.Core.Helpers;
using RH.Core.Render.Helpers;

namespace RH.WebCore
{
    public static class CropHelper
    {
        public static void WebCropImage(string path, string sessionID)
        {
            using (WebClient client = new WebClient())
            {
                byte[] imageBytes = client.DownloadData(path);

                using (var ms = new MemoryStream(imageBytes))
                {
                    ActivateRecognition();

                    var img = new Bitmap(ms);

                    //    var image = new FSDK.CImage(img);


                    if (ActivateRecognition())
                        CropImage(img, sessionID);
                    else
                        SaveToFTP(img, sessionID);
                }
            }
        }

        public static bool ActivateRecognition()
        {
            if (FSDK.FSDKE_OK != FSDK.ActivateLibrary("DWysHuomlBcczVM2MQfiz/3WraXb7r+fM0th71X5A9z+gsHn2kpGOgWrVh9D/9sQWlPXO00CFmGMvetl9A+VEr9Y5GVBIccyV32uaZutZjKYH5KB2k87NJAAw6NPkzK0DSQ5b5W7EO0yg2+x4HxpWzPogGyIIYcAHIYY11/YGsU="))
                return false;

            FSDK.InitializeLibrary();
            FSDK.SetFaceDetectionParameters(true, true, 384);

            return true;
        }


        public static void SaveToFTP(Image img, string name)
        {
            byte[] data;
            using (MemoryStream m = new MemoryStream())
            {
                img.Save(m, ImageFormat.Jpeg);
                m.Flush();

                data = m.ToArray();
            }

            FTPHelper ftpHelper = new FTPHelper(@"ftp://108.167.164.209/public_html/printahead.online/PrintAhead_images");
            ftpHelper.Upload(new MemoryStream(data), name + ".jpeg");
        }


        private static void CropImage(Image sourceImage, string imageName, bool needCrop = true, int angleCount = 0)
        {

            FSDK.TPoint[] pointFeature;

            var image = new FSDK.CImage(new Bitmap(sourceImage));



            var faceRectangle = Rectangle.Empty;

            var facePosition = image.DetectFace();
            if (0 == facePosition.w)
            {
                SaveToFTP(sourceImage, imageName);
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

            if (needCrop)
                sourceImage = ImageEx.Crop(new Bitmap(sourceImage), faceRectangle);


            // по новой картинке еще раз распознаемм все
            image = new FSDK.CImage(new Bitmap(sourceImage));
            facePosition = image.DetectFace();
            if (0 == facePosition.w)
            {
                SaveToFTP(sourceImage, imageName);
                return;
            }

            pointFeature = image.DetectFacialFeaturesInRegion(ref facePosition);


            var LeftEyeCenter = new Vector2(pointFeature[0].x, pointFeature[0].y);
            var RightEyeCenter = new Vector2(pointFeature[1].x, pointFeature[1].y);

            #region Поворот фотки по глазам!



            var v = new Vector2(LeftEyeCenter.X - RightEyeCenter.X, LeftEyeCenter.Y - RightEyeCenter.Y);
            v.Normalize(); // ПД !
            var xVector = new Vector2(1, 0);

            var xDiff = xVector.X - v.X;
            var yDiff = xVector.Y - v.Y;
            var angle = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;

            if (Math.Abs(angle) > 1 && angleCount <= 5) // поворачиваем наклоненные головы
            {
                ++angleCount;
                sourceImage = ImageEx.RotateImage(new Bitmap(sourceImage), (float)-angle);
                CropImage(sourceImage, imageName, false, angleCount);
                return;
            }

            #endregion

            #region Корректируем размер фотки

            const int selectedSize = 1024;              // вызывается уже при создании проекта
            var max = (float)Math.Max(sourceImage.Width, sourceImage.Height);
            if (max != selectedSize)
            {
                var k = selectedSize / max;
                sourceImage = ImageEx.ResizeImage(sourceImage, new Size((int)Math.Round(sourceImage.Width * k), (int)Math.Round((sourceImage.Height * k))));
            }

            #endregion

            SaveToFTP(sourceImage, imageName);
        }
    }
}