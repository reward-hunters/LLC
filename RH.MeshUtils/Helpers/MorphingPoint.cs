﻿using OpenTK;
using RH.MeshUtils.Helpers;
using System.Collections.Generic;

namespace RH.MeshUtils.Helpers
{
    public enum MorphTriangleType
    {
        Default,
        Left,
        Right
    }

    public class MorphTrinagleInfo
    {
        public int TriangleIndex = -1;
        public float U, V, W;
    }

    public class MorphingPoint
    {
        public static bool PointInTriangle(ref Vector2 a, ref Vector2 b, ref Vector2 c, Vector2 p)
        {
            var n1 = (b.Y - a.Y) * (p.X - a.X) - (b.X - a.X) * (p.Y - a.Y);
            var n2 = (c.Y - b.Y) * (p.X - b.X) - (c.X - b.X) * (p.Y - b.Y);
            var n3 = (a.Y - c.Y) * (p.X - c.X) - (a.X - c.X) * (p.Y - c.Y);
            return (n1 <= 0.0f && n2 <= 0.0f && n3 <= 0.0f) || (n1 >= 0.0f && n2 >= 0.0f && n3 >= 0.0f);
        }

        public Vector3 Position;
        public Vector3 WorldPosition;
        public Vector3 ReversedWorldPosition;
        public List<int> Indices = new List<int>();
        public MorphTrinagleInfo FrontTriangle = new MorphTrinagleInfo();
        public MorphTrinagleInfo RightTriangle = new MorphTrinagleInfo();
        public MorphTrinagleInfo AdditionalTriangle = new MorphTrinagleInfo();
        public MorphTriangleType TriangleType = MorphTriangleType.Default;

        public Vector3 MorphFront(ref Vector2 v1, ref Vector2 v2, ref Vector2 v3)
        {
            Vector3 result = Position;
            result.X = FrontTriangle.U * v1.X + FrontTriangle.V * v2.X + FrontTriangle.W * v3.X;
            result.Y = FrontTriangle.U * v1.Y + FrontTriangle.V * v2.Y + FrontTriangle.W * v3.Y;
            return result;
        }

        public Vector3 MorphRight(ref Vector2 v1, ref Vector2 v2, ref Vector2 v3)
        {
            Vector3 result = Position;
            result.Z = RightTriangle.U * v1.X + RightTriangle.V * v2.X + RightTriangle.W * v3.X;
            result.Y = RightTriangle.U * v1.Y + RightTriangle.V * v2.Y + RightTriangle.W * v3.Y;
            return result;
        }

        public Vector3 AdditionalMorph(ref Vector2 v1, ref Vector2 v2, ref Vector2 v3, bool isReversed)
        {
            Vector3 result = isReversed ? ReversedWorldPosition : WorldPosition;
            result.X = AdditionalTriangle.U * v1.X + AdditionalTriangle.V * v2.X + AdditionalTriangle.W * v3.X;
            result.Y = AdditionalTriangle.U * v1.Y + AdditionalTriangle.V * v2.Y + AdditionalTriangle.W * v3.Y;
            return result;
        }

        public bool Initialize(ref Vector2 a, ref Vector2 b, ref Vector2 c, int triangleIndex, bool isFront)
        {
            var point = isFront ? Position.Xy : Position.Zy;

            if (PointInTriangle(ref a, ref b, ref c, point))
            {
                var triangle = isFront ? FrontTriangle : RightTriangle;

                var uv = Vector3.Cross(
                    new Vector3(c.X - a.X, b.X - a.X, a.X - point.X),
                    new Vector3(c.Y - a.Y, b.Y - a.Y, a.Y - point.Y));
                if (uv.Z == 0.0f)
                    triangle.U = triangle.V = triangle.W = 0.0f;
                else
                {
                    triangle.U = 1.0f - (uv.X + uv.Y) / uv.Z;
                    triangle.V = uv.Y / uv.Z;
                    triangle.W = uv.X / uv.Z;
                }
                triangle.TriangleIndex = triangleIndex;
                return true;
            }
            return false;
        }

        public bool AdditionalInitialize(ref Vector2 a, ref Vector2 b, ref Vector2 c, int triangleIndex, bool isReversed)
        {
            var point = isReversed ? ReversedWorldPosition.Xy : WorldPosition.Xy;

            if (PointInTriangle(ref a, ref b, ref c, point))
            {
                var uv = Vector3.Cross(
                    new Vector3(c.X - a.X, b.X - a.X, a.X - point.X),
                    new Vector3(c.Y - a.Y, b.Y - a.Y, a.Y - point.Y));
                if (uv.Z == 0.0f)
                    AdditionalTriangle.U = AdditionalTriangle.V = AdditionalTriangle.W = 0.0f;
                else
                {
                    AdditionalTriangle.U = 1.0f - (uv.X + uv.Y) / uv.Z;
                    AdditionalTriangle.V = uv.Y / uv.Z;
                    AdditionalTriangle.W = uv.X / uv.Z;
                }
                AdditionalTriangle.TriangleIndex = triangleIndex;
                return true;
            }
            return false;
        }

        public void InitializeTexCoords(ref Vector2 v1, ref Vector2 v2, ref Vector2 v3,
            ref Vector2 vm1, ref Vector2 vm2, ref Vector2 vm3,
            RenderMeshPart meshPart)
        {
            foreach (var i in Indices)
            {
                var v = meshPart.Vertices[i];

                v.AutodotsTexCoord.X = FrontTriangle.U * v1.X + FrontTriangle.V * v2.X + FrontTriangle.W * v3.X;
                v.AutodotsTexCoord.Y = FrontTriangle.U * v1.Y + FrontTriangle.V * v2.Y + FrontTriangle.W * v3.Y;

                v.AutodotsTexCoord.Z = FrontTriangle.U * vm1.X + FrontTriangle.V * vm2.X + FrontTriangle.W * vm3.X;
                v.AutodotsTexCoord.W = FrontTriangle.U * vm1.Y + FrontTriangle.V * vm2.Y + FrontTriangle.W * vm3.Y;

                meshPart.Vertices[i] = v;
            }
        }
    }
}
