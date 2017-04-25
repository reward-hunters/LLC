using OpenTK;
using RH.Core.Render.Controllers;
using RH.Core.Render.Meshes;
using RH.Core.Render.Obj;

namespace RH.Core.WebHelpers
{
    public static class WebStyleHelper
    {
        public static void AttachHair(string objPath, string exportPath, string materialPath, ManType manType, string sessionID)
        {
            var objModel = ObjLoader.LoadObjFile(objPath, false);
            if (objModel == null)
                return;

            var temp = 0;
            var meshes = PickingController.LoadHairMeshes(objModel, null, true, manType, MeshType.Hair, ref temp);
     

            var meshSize = 1f;
            var meshPosition = new Vector3(0, 0, 0);
            /* if (float.IsNaN(meshSize) && meshes.Count > 0 && UserConfig.ByName("Parts").Contains(meshes[0].Path))
             {
                 var mesh = meshes[0];
                 meshSize = StringConverter.ToFloat(UserConfig.ByName("Parts")[mesh.Path, "Size"]);
                 meshPosition = Vector3Ex.FromString(UserConfig.ByName("Parts")[mesh.Path, "Position"]);
             }*/

            for (var i = 0; i < meshes.Count; i++)
            {
                var mesh = meshes[i];
                if (mesh == null || mesh.vertexArray.Length == 0) //ТУТ!
                    continue;


                mesh.Material.DiffuseTextureMap = materialPath;
                Vector3 s = meshPosition;

                mesh.Position = new Vector3(s[0], s[1], s[2]);
                mesh.Transform[3, 0] += s[0];
                mesh.Transform[3, 1] += s[1];
                mesh.Transform[3, 2] += s[2];

                /*if (!float.IsNaN(meshSize))
                    mesh.InterpolateMesh(meshSize);*/
            }

            var rMeshes = new DynamicRenderMeshes();
            rMeshes.AddRange(meshes);
      //      var realScale = 246 * ((8 * 0.125f/*Уменьшаем в 8 раз как сказал старик*/) / 4.2f);
            ObjSaver.SaveObjFile(exportPath, rMeshes, MeshType.Hair, 246, manType, sessionID);
        }
    }
}
