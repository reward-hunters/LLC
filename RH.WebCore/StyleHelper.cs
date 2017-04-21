using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Core.Helpers;

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
        public static void AttachHair(int manType, string sessionID, string hairPath)
        {
            if (string.IsNullOrEmpty(hairPath.Trim()))
                return;

            var paths = hairPath.Trim().Split(new string[] {"\""}, StringSplitOptions.RemoveEmptyEntries);
            if (paths.Length == 0)
                return;
            var hairObjPath = paths[1].Trim();
            hairObjPath = Path.GetDirectoryName(hairObjPath) + "/" + Path.GetFileNameWithoutExtension(hairObjPath) + ".obj";
            hairObjPath = hairObjPath.Replace(@"\", "/");
            if (hairObjPath.StartsWith(@"http:/printahead.net/"))
                hairObjPath = hairObjPath.Replace(@"http:/printahead.net/", @"ftp://108.167.164.209/public_html/");
            if (!FTPHelper.IsFileExists(hairObjPath))
                return;

            var newHairObjPath = "ftp://108.167.164.209/public_html/printahead.online/PrintAhead_models/" + sessionID;
            FTPHelper.CopyFromFtpToFtp(hairObjPath, newHairObjPath, "Hair.obj", sessionID);













            var objPath = Path.Combine(fi.DirectoryName, Path.GetFileNameWithoutExtension(fi.Name) + ".obj");
            var meshes = pickingController.AddMehes(objPath, meshType, true, ProgramCore.Project.ManType, false);

            if (float.IsNaN(meshSize) && meshes.Count > 0 && UserConfig.ByName("Parts").Contains(meshes[0].Path))
            {
                var mesh = meshes[0];
                meshSize = StringConverter.ToFloat(UserConfig.ByName("Parts")[mesh.Path, "Size"]);
                meshPosition = Vector3Ex.FromString(UserConfig.ByName("Parts")[mesh.Path, "Position"]);
            }

            for (var i = 0; i < meshes.Count; i++)
            {
                var mesh = meshes[i];
                if (mesh == null || mesh.vertexArray.Length == 0) //ТУТ!
                    continue;

                Vector3 s = meshPosition;

                mesh.Position = new Vector3(s[0], s[1], s[2]);
                mesh.Transform[3, 0] += s[0];
                mesh.Transform[3, 1] += s[1];
                mesh.Transform[3, 2] += s[2];

                if (!float.IsNaN(meshSize))
                {
                    if (meshType == MeshType.Accessory)
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
                    else mesh.InterpolateMesh(meshSize);
                }

                mesh.Title = title + "_" + i;
            }
        }
    }
}
