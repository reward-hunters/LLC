using System;
using System.IO;
using RH.Core;
using RH.Core.Helpers;
using RH.Core.WebHelpers;

namespace RH.WebCore
{
    public static class StyleHelper
    {
        /// <summary>
        /// 
        /// Путь приходит в виде ссылке на картинку, Там же с тем же названием должен лежать обж.
        /// Мне проще обработать такие ссылки тут, чем в яве
        ///   							<img src=\"http://printahead.net/printahead.online/Library/Hair/Standard/20.jpg\"
        /// </summary>
        /// <param name="manType"></param>
        /// <param name="sessionID"></param>
        /// <param name="hairPath"></param>
        public static void AttachHair(int manTypeInt, string sessionID, string hairPath, string materialPath)
        {
            if (string.IsNullOrEmpty(hairPath.Trim()))
                return;

            var paths = hairPath.Trim().Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);
            if (paths.Length == 0)
                return;
            var hairObjPath = paths[1].Trim();
            hairObjPath = Path.GetDirectoryName(hairObjPath) + "/" + Path.GetFileNameWithoutExtension(hairObjPath) + ".obj";
            hairObjPath = hairObjPath.Replace(@"\", "/");
            if (hairObjPath.StartsWith(@"http:/printahead.net/"))
                hairObjPath = hairObjPath.Replace(@"http:/printahead.net/", @"ftp://108.167.164.209/public_html/");
            if (!FTPHelper.IsFileExists(hairObjPath))
                return;

            materialPath = "ftp://108.167.164.209/public_html/printahead.online/Library/Hair/Materials/blondy.jpg";
            if (!string.IsNullOrEmpty(materialPath))
            {
                var temp = @"ftp://108.167.164.209/public_html/printahead.online/PrintAhead_models/" + sessionID + "/Textures";
                FTPHelper.CopyFromFtpToFtp(materialPath, temp, "blondy.jpg");
                materialPath = @"ftp://108.167.164.209/public_html/printahead.online/PrintAhead_models/" + sessionID + "/Textures/blondy.jpg";
            }

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

            var newHairObjPath = "Hair";
            WebStyleHelper.AttachHair(hairObjPath, newHairObjPath, materialPath, manType, sessionID);

        }
    }
}
