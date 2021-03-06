﻿using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using RH.MeshUtils.Data;
using RH.MeshUtils.Helpers;

namespace RH.MeshUtils
{
    public class HeadMeshesController
    {
        #region Var

        private RenderMesh rMesh = new RenderMesh();
        public RenderMesh RenderMesh
        {
            get { return rMesh; }
            set { rMesh = value; }
        }
        public TexturingInfo TexturingInfo = new TexturingInfo();
        #endregion

        #region Public
        public void Destroy()
        {
            RenderMesh.Destroy();
        }

        public void UpdateShape(ref TexturingInfo shapeInfo)
        {
            foreach (var p in RenderMesh.Parts)
                p.UpdateShape(ref shapeInfo);
        }

        public void UpdateProfileShape(ref TexturingInfo shapeInfo)
        {
            foreach (var p in RenderMesh.Parts)
                p.UpdateProfileShape(ref shapeInfo);
        }

        public void UpdateTexCoors(IEnumerable<Vector2> texCoords)
        {

            TexturingInfo.TexCoords = texCoords.ToArray();
            foreach (var p in RenderMesh.Parts)
                p.UpdateTexCoords(ref TexturingInfo);
        }

        public void InitializeShapingProfile(ref TexturingInfo shapeInfo)
        {
            foreach (var p in RenderMesh.Parts)
                p.FillPointsInfo(ref shapeInfo, true, true);
        }

        public void InitializeShaping(ref TexturingInfo shapeInfo)
        {
            foreach (var p in RenderMesh.Parts)
            {
                p.FillPointsInfo(ref shapeInfo, true, false);
            }
        }

        public void InitializeTexturing(IEnumerable<HeadPoint> points, IEnumerable<Int32> indices)
        {
            TexturingInfo.Points = new HeadPoints<HeadPoint>();
            TexturingInfo.Points.AddRange(points);

            //TexturingInfo.UpdatePointsInfo(RenderMesh.Scale, RenderMesh.AABB.Center.Xy);

            TexturingInfo.Indices = indices.ToArray();
            foreach (var p in RenderMesh.Parts)
            {
                p.FillPointsInfo(ref TexturingInfo, false, false);
            }
        }

        public void FinishCreating()
        {
            UpdateBuffers(true);
            RenderMesh.FindFixedPoints();
        }

        public void ScaleWidth(float k, float centerX)
        {
            RenderMesh.ScaleWidth(k, centerX);
            UpdateBuffers();
        }

        public void Resize(float scale)
        {
            RenderMesh.Resize(scale);
            UpdateBuffers();
        }

        public float SetSize(float diagonal)
        {
            var scale = RenderMesh.SetSize(diagonal);
            UpdateBuffers();
            return scale;
        }

        public float FinishCreating(float widthToHeight, RectangleAABB aabb)
        {
            var result = RenderMesh.Transform(widthToHeight, aabb);
            UpdateBuffers();
            return result;
        }

        public void UpdateBuffers(bool firstTime = false)
        {
            foreach (var part in RenderMesh.Parts)
                part.UpdateBuffers(firstTime);
        }

        public void Smooth()
        {
            foreach (var p in RenderMesh.Parts)
                p.Smooth();
        }

        public bool CreateMeshPart(GenesisType genesis, MeshPartInfo info)
        {
            var part = new RenderMeshPart();

            if (part.Create(genesis, info))
            {
                RenderMesh.AddPart(part);
                return true;
            }
            return false;
        }

        public void Draw(bool debug)
        {
            RenderMesh.Draw(debug);
        }

        public void Mirror(bool leftToRight, float axis)
        {
            foreach (var p in RenderMesh.Parts)
            {
                p.Mirror(leftToRight, axis);
            }

        }

        public void UndoMirror()
        {
            foreach (var p in RenderMesh.Parts)
                p.UndoMirror();
        }

        public void GetUndoInfo(out Dictionary<Guid, MeshUndoInfo> undoInfo)
        {
            undoInfo = RenderMesh.Parts.ToDictionary(part => part.Guid, part => part.GetUndoInfo());
        }

        public void Undo(Dictionary<Guid, MeshUndoInfo> undoInfo)
        {
            MeshUndoInfo info = null;
            foreach (var part in RenderMesh.Parts.Where(part => undoInfo.TryGetValue(part.Guid, out info)))
            {
                part.Undo(info);
            }
        }

        public void UpdateNormals()
        {
            foreach (var p in RenderMesh.Parts)
                p.UpdateNormals();
        }

        #endregion
    }
}
