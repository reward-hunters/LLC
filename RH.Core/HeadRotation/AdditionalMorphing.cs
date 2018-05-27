using OpenTK;
using RH.MeshUtils.Data;
using RH.MeshUtils.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RH.Core.HeadRotation
{
    public class AdditionalMorphing
    {
        public MorphTriangleType Type = MorphTriangleType.Left;
        public List<Vector2> Convex = new List<Vector2>();
        public List<uint> Indices = new List<uint>();
        public bool IsReversed = false;

        public int LastIndex = 0;
        public int FirstIndex = 0;

        public void Initialize(ProjectedDots dots, HeadMorphing headMorphing)
        {
            MorphTriangleType realType = Type;
            if (IsReversed)
            {
                realType = Type == MorphTriangleType.Left ? MorphTriangleType.Right : MorphTriangleType.Left;
            }

            List<Vector2> points = new List<Vector2>();
            foreach (var part in ProgramCore.MainForm.ctrlRenderControl.headMeshesController.RenderMesh.Parts)
            {
                foreach (var point in part.MorphPoints)
                {
                    if (point.TriangleType != realType || point.Position.Z < 0.0f)
                        continue;
                    if (IsReversed)
                        points.Add(point.ReversedWorldPosition.Xy);
                    else
                        points.Add(point.WorldPosition.Xy);
                }
            }

            Convex = Triangulate.ComputeConvexHull(points, (Type == MorphTriangleType.Left) == IsReversed);

            LastIndex = Convex.Count - 1;

            float prevX = Convex[LastIndex].X;
            float prevY = Convex[LastIndex].Y;

            for (int i = LastIndex - 1; i >= 0; --i)
            {
                float y = Convex[i].Y;
                float x = Convex[i].X;
                if (x > prevX == (Type == MorphTriangleType.Left))
                    continue;
                if (y < prevY)
                    break;
                prevX = x;
                prevY = y;
                FirstIndex = i;
            }
            Convex.RemoveRange(0, FirstIndex);
            if (Type == MorphTriangleType.Left)
            {
                Convex.Insert(0, GetPoint(dots.Points[52]));
                Convex.Insert(0, GetPoint(dots.Points[3]));
                Convex.Insert(0, GetPoint(dots.Points[58]));
                Convex.Insert(0, GetPoint(headMorphing.headPoints.Points[72].Xy));
                Convex.Insert(0, GetPoint(headMorphing.headPoints.Points[73].Xy));
                Convex.Insert(0, GetPoint(headMorphing.headPoints.Points[74].Xy));
                Convex.Insert(0, GetPoint(headMorphing.headPoints.Points[75].Xy));
            }
            else
            {
                Convex.Insert(0, GetPoint(dots.Points[53]));
                Convex.Insert(0, GetPoint(dots.Points[4]));
                Convex.Insert(0, GetPoint(dots.Points[59]));
                Convex.Insert(0, GetPoint(headMorphing.headPoints.Points[70].Xy));
                Convex.Insert(0, GetPoint(headMorphing.headPoints.Points[77].Xy));
                Convex.Insert(0, GetPoint(headMorphing.headPoints.Points[76].Xy));
                Convex.Insert(0, GetPoint(headMorphing.headPoints.Points[75].Xy));
            }
            FirstIndex = 7;
            LastIndex = Convex.Count - 1;

            var tempPoints = new List<Point>();
            for (int index = 0; index < Convex.Count; ++index)
            {
                var position = Convex[index];
                tempPoints.Add(new Point((uint)index, position.X, position.Y));
            }
            Indices.Clear();
            Indices.AddRange(Triangulate.Delaunay(tempPoints));
        }

        private Vector2 GetPoint(Vector2 point)
        {
            if (IsReversed)
                point.X = -point.X;
            return point;
        }

        public Vector3 A;
        public Vector3 B;

        public void ProcessPoints(ProjectedDots dots)
        {
            int[] dotIndices = Type == MorphTriangleType.Right ?
                new int[] { 67, 69, 6, 8, 10, 11 } :
                new int[] { 66, 68, 5, 7, 9, 11 };

            var right = new Vector3(1.0f, 0.0f, 0.0f);
            right = ProgramCore.MainForm.ctrlRenderControl.headMeshesController.RenderMesh.GetWorldPoint(right);

            var a = dots.Points[dotIndices.First()];
            var b = Convex[FirstIndex];

            var sa = Vector3.Dot(new Vector3(a.X, a.Y, 0.0f), right);
            var sb = Vector3.Dot(new Vector3(b.X, b.Y, 0.0f), right);

            A = new Vector3(a.X, a.Y, 0.0f);
            B = new Vector3(b.X, b.Y, 0.0f);

            var ds = Math.Abs(sa / sb);

            foreach (var part in ProgramCore.MainForm.ctrlRenderControl.headMeshesController.RenderMesh.Parts)
            {
                foreach (var point in part.MorphPoints)
                {
                    point.Position.X *= ds;

                    foreach (var index in point.Indices)
                    {
                        part.Vertices[index].Position = point.Position;
                    }
                }

                part.UpdateBuffers();
            }
        }


        private void MorphPoints()
        {
            var start = Convex[FirstIndex];
            var end = Convex[LastIndex];
            var centerY = (start.Y + end.Y) * 0.5f;
            var height = Math.Abs((start.Y - end.Y)) * 0.5f;

            foreach (var part in ProgramCore.MainForm.ctrlRenderControl.headMeshesController.RenderMesh.Parts)
            {
                foreach (var point in part.MorphPoints)
                {
                    if (point.AdditionalTriangle.TriangleIndex > -1)
                    {
                        var index = point.AdditionalTriangle.TriangleIndex * 3;
                        var i0 = (int)Indices[index];
                        var i1 = (int)Indices[index + 1];
                        var i2 = (int)Indices[index + 2];

                        var a = Convex[i0];
                        var b = Convex[i1];
                        var c = Convex[i2];

                        var worldPosition = point.AdditionalMorph(ref a, ref b, ref c, IsReversed);
                        var ky = (point.WorldPosition.Y - centerY) / height;
                        ky = 1.0f - Math.Min(ky * ky, 1.0f);
                        var kz = point.Position.Z / 2.0f;
                        kz = kz * kz;
                        var k = Math.Min(Math.Min(ky, kz), 1.0f);
                        var originalPoint = IsReversed ? ProgramCore.MainForm.ctrlRenderControl.headMeshesController.RenderMesh.GetReversePositionFromWorld(worldPosition) : ProgramCore.MainForm.ctrlRenderControl.headMeshesController.RenderMesh.GetPositionFromWorld(worldPosition);

                        point.Position = k * originalPoint + point.Position * (1.0f - k); // = point.Position;//
                    }

                    foreach (var index in point.Indices)
                    {
                        part.Vertices[index].Position = point.Position;
                    }
                }
                part.UpdateBuffers();
            }
        }
    }
}
