﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Assimp;
using ICSharpCode.SharpZipLib.Zip;
using OpenTK;
using RH.Core.Helpers;
using RH.Core.IO;
using RH.Core.Render.Controllers;
using RH.Core.Render.Helpers;
using RH.Core.Render.Meshes;
using RH.MeshUtils.Data;

namespace RH.Core.Render.Obj
{
    public static class ObjSaver
    {
        public static void SaveObjFile(string filePath, RenderMesh mesh, MeshType type, string sessionId, bool saveBrushesToTexture = false)
        {
            SaveObjFile(filePath, mesh, type, null, sessionId, saveBrushesToTexture);
        }

        public static void SaveObjFile(string filePath, RenderMesh mesh, MeshType type, ObjExport objExport, string sessionId, bool saveBrushesToTexture = false, bool isCollada = false)
        {
            var meshInfos = new List<MeshInfo>();

            foreach (var part in mesh.Parts)
                meshInfos.Add(new MeshInfo(part, mesh.MorphScale));
            SaveObjFile(filePath, meshInfos, type, objExport, saveBrushesToTexture, isCollada, sessionId);
        }

        public static void SaveObjFile(string filePath, DynamicRenderMeshes meshes, MeshType type, float scale, ManType manType, string sessionId, bool saveBrushesToTexture = false, bool isCollada = false)
        {
            var meshInfos = new List<MeshInfo>();

            foreach (var mesh in meshes)
            {
                var meshInfo = mesh.GetMeshInfo(scale, manType);
                meshInfos.Add(meshInfo);
            }
            SaveObjFile(filePath, meshInfos, type, saveBrushesToTexture, isCollada, sessionId);
        }

        public static void SaveObjFile(string filePath, List<MeshInfo> meshInfos, MeshType type, ObjExport objExport, bool saveBrushesToTexture, bool isCollada, string sessionId)
        {
            if (objExport == null)
            {
                SaveObjFile(filePath, meshInfos, type, saveBrushesToTexture, isCollada, sessionId);
                return;
            }
            var fi = new FileInfo(filePath);
            if (fi.Exists)
                fi.Delete();

            if (fi.DirectoryName == null) return;
            var mtlPath = Path.Combine(fi.DirectoryName, Path.GetFileNameWithoutExtension(fi.Name) + ".mtl");
            var fiMtl = new FileInfo(mtlPath);
            //if (fiMtl.Exists)
            //    fiMtl.Delete();
            var groupIndex = 0;
            var materials = new Dictionary<string, ObjMaterial>();
            var startPositionIndex = 1;
            var startTexIndex = 1;
            var startNormalIndex = 1;
            var indicesPositions = new List<int>();
            var indicesTexCoords = new List<int>();
            var indicesNormals = new List<int>();
            var positions = new List<Vector3>();
            var normals = new List<Vector3>();
            var texCoords = new List<Vector2>();

            Vector3 A = new Vector3(99999.0f, 99999.0f, 99999.0f);
            Vector3 B = new Vector3(-99999.0f, -99999.0f, -99999.0f);
            foreach (var meshPartInfo in meshInfos)
            {
                PickingController.GetAABB(ref A, ref B, meshPartInfo.Positions);
            }
            objExport.SetObjectAABB(A, B);

            foreach (var meshInfo in meshInfos) // faces should write differently
            {
                if (meshInfo.IndicesNormals.Count == 0)
                    continue;

                String groupTitle;
                if (string.IsNullOrEmpty(meshInfo.Title))
                {
                    groupTitle = "Element_" + groupIndex;
                    while (materials.ContainsKey(groupTitle))
                    {
                        ++groupIndex;
                        groupTitle = "Element_" + groupIndex;
                    }
                    ++groupIndex;
                }
                else
                    groupTitle = meshInfo.Title;
                materials.Add(groupTitle, meshInfo.Material);

                for (var i = 0; i < meshInfo.IndicesTexCoords.Count; i++)
                {
                    indicesPositions.Add(startPositionIndex + meshInfo.IndicesPositions[i]);
                    indicesTexCoords.Add(startTexIndex + meshInfo.IndicesTexCoords[i]);
                    indicesNormals.Add(startNormalIndex + meshInfo.IndicesNormals[i]);
                }

                startPositionIndex += (meshInfo.IndicesPositions.Max() + 1);
                startTexIndex += (meshInfo.IndicesTexCoords.Max() + 1);
                startNormalIndex += (meshInfo.IndicesNormals.Max() + 1);

                positions.AddRange(meshInfo.Positions);
                normals.AddRange(meshInfo.Normals);
                texCoords.AddRange(meshInfo.TexCoords);
            }

            foreach (var face in objExport.Faces)
            {
                var index = face.TriangleIndex0 * 3;

                for (var l = 0; l < 3; l++)
                {
                    var v = positions[indicesPositions[index + l] - 1];
                    var vn = normals[indicesNormals[index + l] - 1];
                    var vt = texCoords[indicesTexCoords[index + l] - 1];
                    objExport.SetData(v, vt, vn, face, l);
                }
                if (face.VertexCount == 4)
                {
                    index = face.TriangleIndex1 * 3;
                    var v = positions[indicesPositions[index + 1] - 1];
                    var vn = normals[indicesNormals[index + 1] - 1];
                    var vt = texCoords[indicesTexCoords[index + 1] - 1];
                    objExport.SetData(v, vt, vn, face, 3);
                }
            }

            using (var sw = new StreamWriter(filePath, false, Encoding.Default))
            {
                sw.WriteLine("#Produced by HeadShop");
                sw.WriteLine(type == MeshType.Accessory ? "#Accessories" : "#HeadShop Model");
                sw.WriteLine("#" + DateTime.Now.ToShortDateString());

                sw.WriteLine("mtllib " + fiMtl.Name);
                sw.WriteLine();

                foreach (var v in objExport.Vertices)
                {
                    var resStr = "v " + v.X.ToString(ProgramCore.Nfi) + " " + v.Y.ToString(ProgramCore.Nfi) + " " +
                                 v.Z.ToString(ProgramCore.Nfi);
                    sw.WriteLine(resStr);
                }

                foreach (var vt in objExport.TexCoords)
                {
                    var resStr = "vt " + vt.X.ToString(ProgramCore.Nfi) + " " + vt.Y.ToString(ProgramCore.Nfi);
                    sw.WriteLine(resStr);
                }

                foreach (var vn in objExport.Normals)
                {
                    var resStr = "vn " + vn.X.ToString(ProgramCore.Nfi) + " " + vn.Y.ToString(ProgramCore.Nfi) + " " +
                                 vn.Z.ToString(ProgramCore.Nfi);
                    sw.WriteLine(resStr);
                }

                var prevGroup = String.Empty;
                foreach (var materialGroup in objExport.MaterialsGroups)
                {
                    if (materialGroup.Groups.Count == 0 || !materialGroup.Groups[0].IsValid)
                        continue;
                    for (var i = 0; i < materialGroup.Groups.Count; i++)
                    {
                        var group = materialGroup.Groups[i];
                        if (!group.IsValid)
                            continue;
                        if (!prevGroup.Equals(group.Group))
                        {
                            sw.WriteLine("g " + group.Group);
                            prevGroup = group.Group;
                        }
                        if (i == 0)
                            sw.WriteLine("usemtl " + materialGroup.Material);
                        for (var j = group.StartFaceIndex; j <= group.EndFaceIndex; j++)
                        {
                            var face = objExport.Faces[j];
                            var resStr = "f ";
                            foreach (var vertex in face.FaceVertices)
                            {
                                resStr += (vertex.VertexIndex).ToString(ProgramCore.Nfi);
                                if (objExport.TexCoords.Count > 0)
                                    resStr += "/" + (vertex.TextureIndex).ToString(ProgramCore.Nfi);
                                if (objExport.Normals.Count > 0)
                                    resStr += "/" + (vertex.NormalIndex).ToString(ProgramCore.Nfi);
                                resStr += " ";
                            }

                            sw.WriteLine(resStr.Remove(resStr.Length - 1));
                        }
                    }
                }
            }
            // SaveMaterial(mtlPath, materials, fi);
        }

        public static void SaveObjFile(string filePath, List<MeshInfo> meshInfos, MeshType type, bool saveBrushesToTexture, bool isCollada, string sessionId)
        {
            if (meshInfos.Count == 0)
                return;

            var materials = new Dictionary<string, ObjMaterial>();         // group title, diffuse color, texture path

#if WEB_APP

            var mtlPath = filePath + ".mtl";
            var mtlName = mtlPath;
            using (MemoryStream ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms, Encoding.Default))
#else
            var fi = new FileInfo(filePath);
            if (fi.Exists)
                fi.Delete();

            var mtlPath = Path.Combine(fi.DirectoryName, Path.GetFileNameWithoutExtension(fi.Name) + ".mtl");
            var fiMtl = new FileInfo(mtlPath);
            if (fiMtl.Exists)
                fiMtl.Delete();

            var mtlName = fiMtl.Name;
            using (var sw = new StreamWriter(filePath, false, Encoding.Default))
#endif

            {
                sw.WriteLine("#Produced by HeadShop");
                if (type == MeshType.Accessory)
                    sw.WriteLine("#Accessories");
                else
                    sw.WriteLine("#HeadShop Model");
                sw.WriteLine("#" + DateTime.Now.ToShortDateString());

                sw.WriteLine("mtllib " + mtlName);
                sw.WriteLine();

                SaveVerticles(meshInfos, sw, Matrix4.Zero);
                SaveTextureCoords(meshInfos, sw);
                SaveNormals(meshInfos, sw, Matrix4.Zero);

                var groupIndex = 0;
                var startPositionIndex = 1;
                var startTexIndex = 1;
                var startNormalIndex = 1;
                foreach (var meshInfo in meshInfos) // faces should write differently
                {
                    if (meshInfo.IndicesNormals.Count == 0)
                        continue;

                    var groupTitle = string.Empty;
                    if (string.IsNullOrEmpty(meshInfo.Title))
                    {
                        groupTitle = "Element_" + groupIndex;

                        while (materials.ContainsKey(groupTitle))
                        {
                            ++groupIndex;
                            groupTitle = "Element_" + groupIndex;
                        }

                        ++groupIndex;
                    }
                    else
                        groupTitle = meshInfo.Title;

                    if (!materials.ContainsKey(groupTitle))
                        materials.Add(groupTitle, meshInfo.Material);

                    sw.WriteLine("g " + groupTitle);
                    sw.WriteLine("usemtl " + groupTitle);

                    #region Faces

                    var resStr = "f ";
                    var index = 0;
                    for (var i = 0; i < meshInfo.IndicesTexCoords.Count; i++)
                    {
                        resStr +=
                            (startPositionIndex + meshInfo.IndicesPositions[i]).ToString(ProgramCore.Nfi) + "/";
                        resStr += (startTexIndex + meshInfo.IndicesTexCoords[i]).ToString(ProgramCore.Nfi) + "/";
                        resStr += (startNormalIndex + meshInfo.IndicesNormals[i]).ToString(ProgramCore.Nfi) +
                                  " ";
                        ++index;

                        if (index == 3)
                        {
                            index = 0;
                            sw.WriteLine(resStr.Remove(resStr.Length - 1));
                            resStr = "f ";
                        }
                    }

                    startPositionIndex += (meshInfo.IndicesPositions.Max() + 1);
                    startTexIndex += (meshInfo.IndicesTexCoords.Max() + 1);
                    startNormalIndex += (meshInfo.IndicesNormals.Max() + 1);

                    #endregion
                }


#if WEB_APP
                    sw.Flush();
                    ms.Position = 0;
                    ms.Flush();
                    var ftpHelper = new FTPHelper(@"ftp://108.167.164.209/public_html/printahead.online/PrintAhead_models/" + sessionId);
                    ftpHelper.Upload(ms, filePath + ".obj");
                }
            }
            SaveMaterial(mtlPath, materials, null, saveBrushesToTexture, isCollada, sessionId);
#else
            }
            SaveMaterial(mtlPath, materials, type, fi, saveBrushesToTexture, isCollada, string.Empty);
#endif
        }

        private static void TransformForPluginMode(DynamicRenderMesh mesh, MeshInfo meshInfo)
        {
            if (!ProgramCore.PluginMode)
                return;

            var scaleCoef = 1f;
            if (mesh.meshType == MeshType.Accessory)
                scaleCoef = 246f;
            else if (mesh.meshType == MeshType.Head)
                scaleCoef = PickingController.GetHeadScale(ProgramCore.Project.ManType);
            else
                scaleCoef = PickingController.GetHairScale(ProgramCore.Project.ManType);

            var d = ProgramCore.MainForm.ctrlRenderControl.pickingController.ObjExport.Delta;
            d.Y -= 0.0060975609f;
            var tempTransform = mesh.Transform * Matrix4.CreateTranslation(d * scaleCoef);
            var dp = new Vector3(mesh.Transform[3, 0], mesh.Transform[3, 1], mesh.Transform[3, 2]) -
                new Vector3(tempTransform[3, 0], tempTransform[3, 1], tempTransform[3, 2]);

            for (var i = 0; i < meshInfo.Positions.Count; ++i)
            {
                var position = meshInfo.Positions[i];
                position += dp;
                meshInfo.Positions[i] = position;
            }
        }

        /// <summary>  </summary>
        /// <param name="filePath"></param>
        /// <param name="HairMeshes"></param>
        /// <param name="AccesoryMeshes"></param>
        /// <param name="faceParts"></param>
        /// <param name="morphScale"></param>
        /// <param name="saveBrushesToTexture">При экспорте в ДАЗ или в колладу - нужно сохранять то что поправили кисточкой в туже текстуру </param>
        /// <param name="isCollada">Если коллада - то текстуры должны лежать в той же папке. Заказ старикана</param>
        public static void ExportMergedModel(string filePath, DynamicRenderMeshes HairMeshes, DynamicRenderMeshes AccesoryMeshes,
            List<MeshInfo> faceParts, float morphScale, string sessionId, bool saveBrushesToTexture = false, bool isCollada = false, ZipOutputStream zipStream = null)
        {
            //if (meshInfos.Count == 0)
            //    return;

            var materials = new Dictionary<string, ObjMaterial>();         // group title, diffuse color, texture path

#if WEB_APP
            var mtlPath = filePath + ".mtl";
            var mtlName = mtlPath;
            using (MemoryStream ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms, Encoding.Default))
#else
            var fi = new FileInfo(filePath);
            if (fi.Exists)
                fi.Delete();

            var mtlPath = Path.Combine(fi.DirectoryName, Path.GetFileNameWithoutExtension(fi.Name) + ".mtl");

            var fiMtl = new FileInfo(mtlPath);
            if (fiMtl.Exists)
                fiMtl.Delete();
            var mtlName = fiMtl.Name;



            using (var sw = new StreamWriter(filePath, false, Encoding.Default))
#endif
            {
                sw.WriteLine("#Produced by HeadShop");
                //if (type == MeshType.Accessory)
                //    sw.WriteLine("#Accessories");
                //else
                sw.WriteLine("#HeadShop Model");
                sw.WriteLine("#" + DateTime.Now.ToShortDateString());

                sw.WriteLine("mtllib " + mtlName);
                sw.WriteLine();


                var meshInfos = new List<MeshInfo>();
                foreach (var mesh in HairMeshes)
                {
                    var meshInfo = mesh.GetMeshInfo(morphScale, ProgramCore.Project.ManType);
                    MeshInfo.FindCenter(meshInfo.Positions, "Волосы до трансформации ObjSaver::ExportMergedModel()");

                    TransformForPluginMode(mesh, meshInfo);

                    meshInfos.Add(meshInfo);
                    MeshInfo.FindCenter(meshInfo.Positions, "Волосы после трансформации ObjSaver::ExportMergedModel()");
                }
                foreach (var mesh in AccesoryMeshes)
                {
                    var meshInfo = mesh.GetMeshInfo(morphScale, ProgramCore.Project.ManType);
                    MeshInfo.FindCenter(meshInfo.Positions, "Аксесуары до трансформации ObjSaver::ExportMergedModel()");
                    TransformForPluginMode(mesh, meshInfo);
                    meshInfos.Add(meshInfo);
                    MeshInfo.FindCenter(meshInfo.Positions, "Аксесуары после трансформации ObjSaver::ExportMergedModel()");
                }

                if (meshInfos.Count > 0)
                {
                    var scale = ProgramCore.PluginMode ? 1.0f : PickingController.GetHairScale(ProgramCore.Project.ManType);
                    ProgramCore.EchoToLog(String.Format("На это умножаем волосы или аксесуары при экспорте ObjSaver::ExportMergedModel(): {0}", scale), EchoMessageType.Information);
                    var transformMatrix = Matrix4.CreateScale(scale);

                    SaveVerticles(meshInfos, sw, transformMatrix);       //write only additional meshes first, with translated positions
                    MeshInfo.FindCenter(meshInfos, "Аксесуары и волосы, которые ушли на экспорт ObjSaver::ExportMergedModel()");
                    SaveTextureCoords(meshInfos, sw);
                    SaveNormals(meshInfos, sw, transformMatrix);
                }
                if (faceParts.Count > 0)
                {

                    ProgramCore.EchoToLog(String.Format("На это умножаем бошку при экспорте ObjSaver::ExportMergedModel(): {0}", morphScale), EchoMessageType.Information);
                    var transformMatrix = Matrix4.CreateScale(morphScale);
                    SaveVerticles(faceParts, sw, transformMatrix);
                    MeshInfo.FindCenter(faceParts, "Бошка, которая ушла на экспорт ObjSaver::ExportMergedModel()");
                    SaveTextureCoords(faceParts, sw);
                    SaveNormals(faceParts, sw, Matrix4.Zero);
                }
                meshInfos.AddRange(faceParts);
                MeshInfo.FindCenter(meshInfos, "Итоговая модель после экспорта ObjSaver::ExportMergedModel()");

                var groupIndex = 0;
                var startPositionIndex = 1;
                var startTexIndex = 1;
                var startNormalIndex = 1;
                foreach (var meshInfo in meshInfos)         // faces should write differently
                {
                    if (meshInfo.IndicesNormals.Count == 0)
                        continue;

                    var groupTitle = string.Empty;
                    if (string.IsNullOrEmpty(meshInfo.Title) || materials.ContainsKey(meshInfo.Title))
                    {
                        groupTitle = "Element_" + groupIndex;

                        while (materials.ContainsKey(groupTitle))
                        {
                            ++groupIndex;
                            groupTitle = "Element_" + groupIndex;
                        }

                        ++groupIndex;
                    }
                    else
                        groupTitle = meshInfo.Title;

                    materials.Add(groupTitle, meshInfo.Material);

                    sw.WriteLine("g " + groupTitle);
                    sw.WriteLine("usemtl " + groupTitle);

                    #region Faces

                    var resStr = "f ";
                    var index = 0;
                    for (var i = 0; i < meshInfo.IndicesTexCoords.Count; i++)
                    {
                        resStr += (startPositionIndex + meshInfo.IndicesPositions[i]).ToString(ProgramCore.Nfi) + "/";
                        resStr += (startTexIndex + meshInfo.IndicesTexCoords[i]).ToString(ProgramCore.Nfi) + "/";
                        resStr += (startNormalIndex + meshInfo.IndicesNormals[i]).ToString(ProgramCore.Nfi) + " ";
                        ++index;

                        if (index == 3)
                        {
                            index = 0;
                            sw.WriteLine(resStr.Remove(resStr.Length - 1));
                            resStr = "f ";
                        }
                    }

                    startPositionIndex += (meshInfo.IndicesPositions.Max() + 1);
                    startTexIndex += (meshInfo.IndicesTexCoords.Max() + 1);
                    startNormalIndex += (meshInfo.IndicesNormals.Max() + 1);

                    #endregion
                }
#if WEB_APP
                    sw.Flush();
                    ms.Position = 0;
                    ms.Flush();
                    var ftpHelper = new FTPHelper(@"ftp://108.167.164.209/public_html/printahead.online/PrintAhead_models/" + sessionId);
                    ftpHelper.Upload(ms, filePath + ".obj");


                    if (zipStream != null)
                    {
                        ms.Position = 0;
                        ms.Flush();
                        var newEntry = new ZipEntry(filePath + ".obj");
                        zipStream.PutNextEntry(newEntry);
                        ms.CopyTo(zipStream);
                        zipStream.CloseEntry();
                    }
                }
            }
            SaveMaterial(mtlPath, materials, null, saveBrushesToTexture, isCollada, sessionId, zipStream);

#else
            }
            SaveMaterial(mtlPath, materials, MeshType.Head, fi, saveBrushesToTexture, isCollada, sessionId);
#endif
        }

        /// <summary>  </summary>
        /// <param name="mtlPath"></param>
        /// <param name="materials"></param>
        /// <param name="fi"></param>
        /// <param name="saveBrushesToTexture">При экспорте в ДАЗ или в колладу - нужно сохранять то что поправили кисточкой в туже текстуру </param>
        /// <param name="isCollada">Если коллада - то текстуры должны лежать в той же папке. Заказ старикана</param>
        private static void SaveMaterial(string mtlPath, Dictionary<string, ObjMaterial> materials, MeshType type, FileInfo fi, bool saveBrushesToTexture, bool isCollada, string sessionId, ZipOutputStream zipStream = null)
        {
#if WEB_APP
            using (MemoryStream ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms, Encoding.Default))
#else
            using (var sw = new StreamWriter(mtlPath, false, Encoding.Default))
#endif
            {
                var res1 = string.Empty;
                foreach (var material in materials)
                {
                    res1 += "newmtl " + material.Key + "\n";

                    if (!float.IsNaN(material.Value.SpecularCoefficient))
                        res1 += "Ns " + material.Value.SpecularCoefficient.ToString(ProgramCore.Nfi) + "\n";
                    if (!float.IsNaN(material.Value.OpticalDensity))
                        res1 += "Ni " + material.Value.OpticalDensity.ToString(ProgramCore.Nfi) + "\n";

                    if (material.Value.AmbientColor != Vector3.Zero)
                        res1 += "Ka " + material.Value.AmbientColor.X.ToString(ProgramCore.Nfi) + " " +
                                material.Value.AmbientColor.Y.ToString(ProgramCore.Nfi) + " " +
                                material.Value.AmbientColor.Z.ToString(ProgramCore.Nfi) + "\n";
                    if (material.Value.DiffuseColor != Vector4.Zero)
                    {
                        res1 += "d " + material.Value.DiffuseColor.W.ToString(ProgramCore.Nfi) + "\n";
                        res1 += "Kd " + material.Value.DiffuseColor.X.ToString(ProgramCore.Nfi) + " " +
                                material.Value.DiffuseColor.Y.ToString(ProgramCore.Nfi) + " " +
                                material.Value.DiffuseColor.Z.ToString(ProgramCore.Nfi) + "\n";
                    }
                    if (material.Value.SpecularColor != Vector3.Zero)
                        res1 += "Ks " + material.Value.SpecularColor.X.ToString(ProgramCore.Nfi) + " " +
                                material.Value.SpecularColor.Y.ToString(ProgramCore.Nfi) + " " +
                                material.Value.SpecularColor.Z.ToString(ProgramCore.Nfi) + "\n";

                    SetTextureMap(material.Value.AmbientTextureMap, "map_Ka", type, fi, ref res1, saveBrushesToTexture,
                        isCollada);
                    SetTextureMap(material.Value.DiffuseTextureMap, "map_Kd", type, fi, ref res1, saveBrushesToTexture,
                        isCollada);
                    SetTextureMap(material.Value.SpecularTextureMap, "map_Ks", type, fi, ref res1, saveBrushesToTexture,
                        isCollada);
                    SetTextureMap(material.Value.SpecularHighlightTextureMap, "map_Ns", type, fi, ref res1,
                        saveBrushesToTexture, isCollada);
                    SetTextureMap(material.Value.TransparentTextureMap, "map_d", type, fi, ref res1, saveBrushesToTexture,
                        isCollada);
                    SetTextureMap(material.Value.BumpMap, "map_Bump", type, fi, ref res1, saveBrushesToTexture, isCollada);
                    SetTextureMap(material.Value.DisplacementMap, "disp", type, fi, ref res1, saveBrushesToTexture,
                        isCollada);
                    SetTextureMap(material.Value.StencilDecalMap, "decal", type, fi, ref res1, saveBrushesToTexture,
                        isCollada);
                }
                sw.Write(res1);


#if WEB_APP
                    sw.Flush();
                    ms.Position = 0;
                    ms.Flush();
                    var ftpHelper = new FTPHelper(@"ftp://108.167.164.209/public_html/printahead.online/PrintAhead_models/" + sessionId);
                    ftpHelper.Upload(ms, mtlPath);


                    if (zipStream != null)
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        var newEntry = new ZipEntry(mtlPath);
                        zipStream.PutNextEntry(newEntry);
                        ms.CopyTo(zipStream);
                        zipStream.CloseEntry();
                    }

                }
            }
#else
            }
#endif
        }

        private static void SetTextureMap(string mapPath, string mapTitle, MeshType type, FileInfo fi, ref string res, bool saveBrushesToTexture, bool isCollada)
        {
            if (!string.IsNullOrEmpty(mapPath))
            {
                var textureName = Path.GetFileName(mapPath);
#if WEB_APP

                //var ftp = new FTPHelper(@"ftp://108.167.164.209/public_html/printahead.online/PrintAhead_models/" + ProgramCore.Project.ProjectName + "/Textures");

                //if (mapPath.StartsWith("http"))
                //{
                //    using (WebClient client = new WebClient())
                //    {
                //        byte[] imageBytes = client.DownloadData(mapPath);

                //        using (var ms = new MemoryStream(imageBytes))
                //            ftp.Upload(ms, Path.GetFileName(mapPath));
                //    }
                //}
                //else
                //{

                //    var image = FTPHelper.DownloadImage(mapPath);
                //    using (var m = new MemoryStream())
                //    {
                //        image.Save(m, image.RawFormat);
                //        ftp.Upload(m, Path.GetFileName(mapPath));
                //    }
                //}
#else
                var newTexturePath = isCollada ? fi.DirectoryName : Path.Combine(fi.DirectoryName, "Textures");
                var newTextureFullPath = Path.Combine(newTexturePath, textureName);

                if (!File.Exists(newTextureFullPath))
                {
                    FolderEx.CreateDirectory(new DirectoryInfo(newTexturePath));

                    switch (ProgramCore.CurrentProgram)
                    {
                        case ProgramCore.ProgramMode.HeadShop_Rotator:
                        case ProgramCore.ProgramMode.HeadShop_v11:
                        case ProgramCore.ProgramMode.FaceAge2_Partial:
                        case ProgramCore.ProgramMode.HeadShop_OneClick_v2:
                            {
                                if (type != MeshType.Head)      // для волос и аксесуаров - не нужно скейлить текстуру.
                                {
                                    File.Copy(mapPath, newTextureFullPath, false);
                                }
                                else
                                {
                                    #region HeadShop rotator

                                    var actualTextureSize = 4096;
                                    var textureId = ProgramCore.MainForm.ctrlRenderControl.GetTexture(mapPath);
                                    if (ProgramCore.MainForm.ctrlRenderControl.brushTextures.ContainsKey(textureId))
                                    {
                                        var brushTexture = ProgramCore.MainForm.ctrlRenderControl.brushTextures[textureId];
                                        using (var bitmap = new Bitmap(mapPath))
                                        {
                                            var max = (float)Math.Max(bitmap.Width, bitmap.Height);
                                            if (max != actualTextureSize)
                                            {
                                                var k = actualTextureSize / max;
                                                var newImg = ImageEx.ResizeImage(bitmap, new Size((int)(bitmap.Width * k), (int)(bitmap.Height * k)));

                                                using (var grfx = Graphics.FromImage(newImg))
                                                    grfx.DrawImage(brushTexture.TextureData, 0, 0, newImg.Width, newImg.Height);

                                                newImg.Save(newTextureFullPath, ImageFormat.Bmp);
                                            }
                                            else
                                            {
                                                using (var grfx = Graphics.FromImage(bitmap))
                                                    grfx.DrawImage(brushTexture.TextureData, 0, 0);

                                                bitmap.Save(newTextureFullPath, ImageFormat.Bmp);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        using (var bitmap = new Bitmap(mapPath))
                                        {
                                            var max = (float)Math.Max(bitmap.Width, bitmap.Height);
                                            if (max != actualTextureSize)
                                            {
                                                var k = actualTextureSize / max;
                                                var newImg = ImageEx.ResizeImage(bitmap, new Size((int)(bitmap.Width * k), (int)(bitmap.Height * k)));
                                                newImg.Save(newTextureFullPath, ImageFormat.Bmp);
                                            }
                                            else
                                                File.Copy(mapPath, newTextureFullPath, false);
                                        }
                                    }

                                    #endregion
                                }
                            }
                            break;
                        default:
                            {
                                #region Для прочих версий

                                if (saveBrushesToTexture)
                                {
                                    var textureId = ProgramCore.MainForm.ctrlRenderControl.GetTexture(mapPath);
                                    if (ProgramCore.MainForm.ctrlRenderControl.brushTextures.ContainsKey(textureId))
                                    {
                                        var brushTexture = ProgramCore.MainForm.ctrlRenderControl.brushTextures[textureId];
                                        using (var bitmap = new Bitmap(mapPath))
                                        {
                                            using (var grfx = Graphics.FromImage(bitmap))
                                                grfx.DrawImage(brushTexture.TextureData, 0, 0);
                                            bitmap.Save(newTextureFullPath, ImageFormat.Jpeg);
                                        }
                                    }
                                    else
                                        File.Copy(mapPath, newTextureFullPath, false);
                                }
                                else
                                {
                                    File.Copy(mapPath, newTextureFullPath, false);
                                }

                                #endregion
                            }
                            break;

                    }
                }
#endif
                res += mapTitle + (isCollada ? " " : " /Textures/") + textureName + "\n";
            }
        }

        private static void SaveVerticles(IEnumerable<MeshInfo> meshInfos, StreamWriter sw, Matrix4 transformMatrix)
        {
            foreach (var meshInfo in meshInfos)
            {
                for (var i = 0; i < meshInfo.Positions.Count; ++i)
                {
                    var v = meshInfo.Positions[i];
                    if (transformMatrix != Matrix4.Zero)
                    {
                        var newV = Vector3.Transform(v, transformMatrix);
                        var resStr = "v " + newV.X.ToString(ProgramCore.Nfi) + " " + newV.Y.ToString(ProgramCore.Nfi) + " " + newV.Z.ToString(ProgramCore.Nfi);
                        meshInfo.Positions[i] = v;
                        sw.WriteLine(resStr);
                    }
                    else
                    {
                        var resStr = "v " + v.X.ToString(ProgramCore.Nfi) + " " + v.Y.ToString(ProgramCore.Nfi) + " " + v.Z.ToString(ProgramCore.Nfi);
                        sw.WriteLine(resStr);
                    }
                }
            }
        }
        private static void SaveTextureCoords(IEnumerable<MeshInfo> meshInfos, StreamWriter sw)
        {
            foreach (var meshInfo in meshInfos)
            {
                foreach (var vt in meshInfo.TexCoords)
                {
                    var resStr = "vt " + vt.X.ToString(ProgramCore.Nfi) + " " + vt.Y.ToString(ProgramCore.Nfi);
                    sw.WriteLine(resStr);
                }
            }
        }
        private static void SaveNormals(IEnumerable<MeshInfo> meshInfos, StreamWriter sw, Matrix4 transformMatrix)
        {
            foreach (var meshInfo in meshInfos)
            {
                foreach (var vn in meshInfo.Normals)
                {
                    var newVn = vn;
                    if (double.IsNaN(newVn.X))
                        newVn.X = 0;
                    if (double.IsNaN(newVn.Y))
                        newVn.Y = 0;
                    if (double.IsNaN(newVn.Z))
                        newVn.Z = 1;

                    if (transformMatrix != Matrix4.Zero)
                    {
                        newVn = Vector3.Transform(vn, transformMatrix);
                        if (double.IsNaN(newVn.X))
                            newVn.X = 0;
                        if (double.IsNaN(newVn.Y))
                            newVn.Y = 0;
                        if (double.IsNaN(newVn.Z))
                            newVn.Z = 1;

                        var resStr = "vn " + newVn.X.ToString(ProgramCore.Nfi) + " " + newVn.Y.ToString(ProgramCore.Nfi) + " " + newVn.Z.ToString(ProgramCore.Nfi);
                        sw.WriteLine(resStr);
                    }
                    else
                    {
                        var resStr = "vn " + vn.X.ToString(ProgramCore.Nfi) + " " + vn.Y.ToString(ProgramCore.Nfi) + " " + vn.Z.ToString(ProgramCore.Nfi);
                        sw.WriteLine(resStr);
                    }

                }
            }
        }
    }
}
