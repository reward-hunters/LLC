using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using RH.MeshUtils.Data;

namespace RH.MeshUtils.Helpers
{
    public class RenderHelper
    {
        static public void DrawLine(Vector3 a, Vector3 b)
        {
            GL.Vertex3(a);
            GL.Vertex3(b);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex3d
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;
        public Vector4 Color;
        public Vector4 AutodotsTexCoord;
        public Vector3 OriginalPosition;
        public float BlendWeight;
        public Vector3 OldPosition;
        public int PointIndex;
        public static readonly int Stride = Marshal.SizeOf(default(Vertex3d));

        /* public Vector3 Position;
         public Vector3 Normal;
         public Vector2 TexCoord;
         public Vector4 Color;
         public Vector3 AutodotsTexCoord;
         public Vector3 OriginalPosition;
         public Vector3 OldPosition;
         public int PointIndex;
         public static readonly int Stride = Marshal.SizeOf(default(Vertex3d));*/

        public void ToStream(BinaryWriter bw)
        {
            Position.ToStream(bw);
            Normal.ToStream(bw);

            bw.Write(TexCoord.X);
            bw.Write(TexCoord.Y);

            bw.Write(Color.X);
            bw.Write(Color.Y);
            bw.Write(Color.Z);
            bw.Write(Color.W);

            AutodotsTexCoord.ToStream(bw);
            OriginalPosition.ToStream(bw);
        }
        public static Vertex3d FromStream(BinaryReader br)
        {
            var result = new Vertex3d();
            result.Position = Vector3Ex.FromStream(br);
            result.Normal = Vector3Ex.FromStream(br);

            result.TexCoord = new Vector2(br.ReadSingle(), br.ReadSingle());
            result.Color = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

            result.AutodotsTexCoord = Vector4Ex.FromStream(br);
            result.OriginalPosition = Vector3Ex.FromStream(br);

            return result;
        }
    }

    public class TrinagleInfo
    {
        public Int32 TrinagleIndex = -1;
        public float U, V, W;
        public float ScaleX = 1;

        public TrinagleInfo(float scaleX = 1.0f)
        {
            ScaleX = scaleX;
        }

        public void ToStream(BinaryWriter bw)
        {
            bw.Write(TrinagleIndex);
            bw.Write(U);
            bw.Write(V);
            bw.Write(W);
        }
        public static TrinagleInfo FromStream(BinaryReader br)
        {
            var result = new TrinagleInfo();
            result.TrinagleIndex = br.ReadInt32();
            result.U = br.ReadSingle();
            result.V = br.ReadSingle();
            result.W = br.ReadSingle();

            return result;
        }
    }

    public class Point3d
    {
        public Dictionary<int, float> Weights = new Dictionary<int, float>();
        public Vector3 Position;
        public List<uint> Indices = null;
        public List<int> Nearests = new List<int>();
        public bool? IsFixed = null;
        public bool? IsFixedLocal = null;
        public TrinagleInfo TextureTrinagleInfo = new TrinagleInfo();
        public TrinagleInfo ShapeTrinagleInfo = new TrinagleInfo();

        public TrinagleInfo ProfileShapeTrinagleInfo = new TrinagleInfo();

        public bool isFixedLocalBroken = false;

        public void ToStream(BinaryWriter bw)
        {
            bw.Write(Weights.Count);
            foreach (var weight in Weights)
            {
                bw.Write(weight.Key);
                bw.Write(weight.Value);
            }
            Position.ToStream(bw);

            bw.Write(Indices.Count);
            foreach (var ind in Indices)
                bw.Write(ind);
            bw.Write(Nearests.Count);
            foreach (var near in Nearests)
                bw.Write(near);
            bw.Write(IsFixed.HasValue && IsFixed.Value);

            bw.Write(1341);         // для открытия старых проектов
            bw.Write(IsFixedLocal.HasValue && IsFixedLocal.Value);

            TextureTrinagleInfo.ToStream(bw);
            ShapeTrinagleInfo.ToStream(bw);
            ProfileShapeTrinagleInfo.ToStream(bw);
        }

        public static Point3d FromStream(BinaryReader br, ref bool oldVersion)
        {
            var result = new Point3d();

            var cnt = br.ReadInt32();
            for (var i = 0; i < cnt; i++)
                result.Weights.Add(br.ReadInt32(), br.ReadSingle());
            result.Position = Vector3Ex.FromStream(br);

            cnt = br.ReadInt32();
            if (cnt != 0)
            {
                result.Indices = new List<uint>();
                for (var i = 0; i < cnt; i++)
                    result.Indices.Add(br.ReadUInt32());
            }
            cnt = br.ReadInt32();
            for (var i = 0; i < cnt; i++)
                result.Nearests.Add(br.ReadInt32());
            result.IsFixed = br.ReadBoolean();

            if (!oldVersion)
            {
                var pos = br.BaseStream.Position;
                try
                {
                    var version = br.ReadInt32();
                    if (version != 1341)
                        throw new Exception();
                    result.IsFixedLocal = br.ReadBoolean();

                }
                catch
                {
                    result.isFixedLocalBroken = true;
                    br.BaseStream.Seek(pos, SeekOrigin.Begin);
                    oldVersion = true;
                }
            }
            else result.isFixedLocalBroken = true;

            result.TextureTrinagleInfo = TrinagleInfo.FromStream(br);
            result.ShapeTrinagleInfo = TrinagleInfo.FromStream(br);
            result.ProfileShapeTrinagleInfo = TrinagleInfo.FromStream(br);

            return result;
        }
    }

    public class MeshUndoInfo
    {
        public Dictionary<int, Vector3> Points = new Dictionary<int, Vector3>();

        public MeshUndoInfo Clone()
        {
            var result = new MeshUndoInfo();

            foreach (var v in Points)
                result.Points.Add(v.Key, v.Value);
            return result;
        }
    }

    public enum HeadMeshType
    {
        Eyes,
        Torso,
        Lip,
        Face,
        Head
    }

    public class RenderMeshPart
    {
        #region Var
        public Guid Guid;
        public String Name
        {
            get;
            set;
        }
#if (WEB_APP)
#else
        public int IndexBuffer, VertexBuffer = 0;
#endif
        public int NumIndices;
        public int CountIndices
        {
            get;
            set;
        }

        public List<uint> TempIndices = null;
        public Vertex3d[] TempVertices = null;

        public List<uint> Indices = new List<uint>();
        public Vertex3d[] Vertices = null;
        public List<Point3d> Points = new List<Point3d>();
        public List<MorphingPoint> MorphPoints = new List<MorphingPoint>();
        public int Texture = 0;
        public int TransparentTexture = 0;

        public string DefaultTextureName = string.Empty;
        public string TextureName = string.Empty;
        public string TransparentTextureName = string.Empty;

        public Vector4 Color = Vector4.One;
        public HeadMeshType Type = HeadMeshType.Head;
        public Dictionary<Point3d, int> PointsIndices = new Dictionary<Point3d, int>(new VectorEqualityComparer());
        public Vertex3d[] BaseVertices = null;
        private readonly List<uint> baseIndices = new List<uint>();

        public bool IsLeftToRight = false;
        public bool IsBaseTexture = false;
        public bool IsShaped
        {
            get
            {
                return Type == HeadMeshType.Face || Type == HeadMeshType.Lip || Type == HeadMeshType.Head;
            }
        }
        #endregion

        #region Public

        public bool IsVisible = true;

        public bool IsMirrored
        {
            get
            {
                return BaseVertices != null;
            }
        }

        public Vector2 GetCenter(bool? isLeft = null)
        {
            var center = Vector2.Zero;
            var count = 0.0f;
            if (isLeft == null)
            {
                foreach (var p in Points)
                {
                    count++;
                    center += p.Position.Xy;
                }
            }
            else
            {
                foreach (var p in Points)
                {
                    if ((isLeft.Value && p.Position.X < 0.0f) || (!isLeft.Value && p.Position.X > 0.0f))
                    {
                        count++;
                        center += p.Position.Xy;
                    }
                }
            }
            return center / count;
        }

        public void AttachShapePoints(List<ShapePoint> shapePoints)
        {
            if (!IsShaped)
                return;
            float k;
            var keys = new List<int>();
            for (var i = 0; i < Points.Count; i++)
            {
                var p = Points[i];
                p.Weights.Clear();
                keys.Clear();
                var summ = 0.0f;
                for (var j = 0; j < shapePoints.Count; j++)
                {
                    var sp = shapePoints[j];
                    if (sp.CheckContains(ref p.Position, out k))
                    {
                        summ += k;
                        p.Weights.Add(j, k);
                        sp.InsertPoint(Guid, i);
                        keys.Add(j);
                    }
                }
                if (summ > 1.0f)
                {
                    foreach (var key in keys)
                        p.Weights[key] /= summ;
                }
            }
        }

        public void UpdateNormals()
        {
            var normals = Normal.CalculateNormals(Vertices.Select(v => v.Position).ToList(), Indices);
            for (var i = 0; i < normals.Count; i++)
            {
                var v = Vertices[i];
                v.Normal = normals[i];
                Vertices[i] = v;
            }
#if (WEB_APP)
#else
            UpdateVertexBuffer();
#endif
        }

        public void UpdateProfileShape(ref TexturingInfo s, float percent = 0.6f)
        {
            foreach (var p in Points)
            {
                if (p.ProfileShapeTrinagleInfo.TrinagleIndex < 0 || (p.IsFixed != null && p.IsFixed.Value))
                    continue;
                var ti = p.ProfileShapeTrinagleInfo.TrinagleIndex * 3;
                var v1 = s.Points[s.Indices[ti]].Value;
                var v2 = s.Points[s.Indices[ti + 1]].Value;
                var v3 = s.Points[s.Indices[ti + 2]].Value;
                var position = p.Position;
                position.Z = p.ProfileShapeTrinagleInfo.U * v1.X + p.ProfileShapeTrinagleInfo.V * v2.X + p.ProfileShapeTrinagleInfo.W * v3.X;
                position.Y = p.ProfileShapeTrinagleInfo.U * v1.Y + p.ProfileShapeTrinagleInfo.V * v2.Y + p.ProfileShapeTrinagleInfo.W * v3.Y;

                var delta = (position - p.Position) * percent;
                p.Position += delta;

                foreach (var i in p.Indices)
                {
                    var v = Vertices[i];
                    v.Position = p.Position;
                    Vertices[i] = v;
                }
            }
        }

        public void BeginMorph()
        {
            for (var i = 0; i < Vertices.Length; ++i)
                Vertices[i].OldPosition = Vertices[i].Position;
        }

        public void EndMorph()
        {
            for (var i = 0; i < Vertices.Length; ++i)
                Vertices[i].Position = Vertices[i].OldPosition;
        }

        public void DoMorph(float k)
        {
            for (var i = 0; i < Vertices.Length; ++i)
            {
                var vertex = Vertices[i];
                Vertices[i].Position = vertex.OriginalPosition + (vertex.OldPosition - vertex.OriginalPosition) * k;
            }
            UpdateNormals();
        }

        private bool CheckFixed(Point3d p)
        {
            var result = Type == HeadMeshType.Torso || (p.IsFixed == true && p.Position.Y < -3.0f);
            return result;
        }

        public void UpdateShape(ref TexturingInfo t)
        {
            /* bool isMirrored = IsMirrored;
             if(isMirrored)
             {
                 UndoMirror(false);
             }*/

            foreach (var p in Points)
            {
                TrinagleInfo ShapeTrinagleInfo = GetTriangleInfo(ref t, p, false);
                if (ShapeTrinagleInfo.TrinagleIndex < 0 || CheckFixed(p))
                    continue;
                var ti = ShapeTrinagleInfo.TrinagleIndex * 3;
                var v1 = t.Points[t.Indices[ti]].Value;
                var v2 = t.Points[t.Indices[ti + 1]].Value;
                var v3 = t.Points[t.Indices[ti + 2]].Value;
                p.Position.X = (ShapeTrinagleInfo.U * v1.X + ShapeTrinagleInfo.V * v2.X + ShapeTrinagleInfo.W * v3.X) * ShapeTrinagleInfo.ScaleX;
                p.Position.Y = ShapeTrinagleInfo.U * v1.Y + ShapeTrinagleInfo.V * v2.Y + ShapeTrinagleInfo.W * v3.Y;
                foreach (var i in p.Indices)
                {
                    var v = Vertices[i];
                    v.Position = p.Position;
                    Vertices[i] = v;
                }
            }
#if (WEB_APP)
#else
            /*if(isMirrored)
            {
                Mirror(IsLeftToRight, 0.0f);
            }
            else
            {*/
            UpdateVertexBuffer();
            //}
#endif
        }

        private TrinagleInfo GetTriangleInfo(ref TexturingInfo t, Point3d point, bool isTexture)
        {
            /* var p = point.Position;// Vertices[point.Indices[0]].OriginalPosition;
             if((t.MirrorType == EMirrorType.Left && p.X < 0.0f) ||(t.MirrorType == EMirrorType.Right && p.X > 0.0f))
             {
                 return isTexture ? point.TextureTrinagleInfoMirror : point.ShapeTrinagleInfoMirror;
             }*/
            return isTexture ? point.TextureTrinagleInfo : point.ShapeTrinagleInfo;
        }

        public void UpdateTexCoords(ref TexturingInfo t)
        {
            bool isMirrored = IsMirrored;
            if (isMirrored)
            {
                UndoMirror(false);
            }

            foreach (var p in Points)
            {
                TrinagleInfo TextureTrinagleInfo = GetTriangleInfo(ref t, p, true);
                if (TextureTrinagleInfo.TrinagleIndex < 0)
                    continue;
                var ti = TextureTrinagleInfo.TrinagleIndex * 3;
                var v1 = t.TexCoords[t.Indices[ti]];
                var v2 = t.TexCoords[t.Indices[ti + 1]];
                var v3 = t.TexCoords[t.Indices[ti + 2]];
                foreach (var i in p.Indices)
                {
                    var v = Vertices[i];
                    v.AutodotsTexCoord.X = TextureTrinagleInfo.U * v1.X + TextureTrinagleInfo.V * v2.X + TextureTrinagleInfo.W * v3.X;
                    v.AutodotsTexCoord.Y = TextureTrinagleInfo.U * v1.Y + TextureTrinagleInfo.V * v2.Y + TextureTrinagleInfo.W * v3.Y;
                    if (IsBaseTexture)
                        v.TexCoord = v.AutodotsTexCoord.Xy;

                    Vertices[i] = v;
                }
            }
#if (WEB_APP)
#else
            if (isMirrored)
            {
                //Mirror(IsLeftToRight, 0.0f);
                UpdateBuffers(true);
            }
            else
            {
                UpdateVertexBuffer();
            }
#endif
        }

        public void FillBlendingData(List<BlendingInfo> blendingInfos)
        {
            foreach (var p in Points)
            {
                var k = 0.0f;
                foreach (var b in blendingInfos)
                {
                    var length = (b.Position - p.Position.Xy).Length;
                    if (length < b.Radius)
                        k = 1.0f;
                    else if (length < (b.Radius + b.HalfRadius))
                        k = Math.Max(k, (1.0f - ((length - b.Radius) / b.HalfRadius)));
                }

                foreach (var i in p.Indices)
                    Vertices[i].AutodotsTexCoord.Z = k;
            }
        }

        //private Vector2 GetPerpendicularPoint(Vector2 point0, Vector2 line0, Vector2 line1)
        //{
        //    var vector = line1 - line0;
        //    var perpendicular = new Vector2(vector.Y, -vector.X);
        //    var point1 = point0 + perpendicular;
        //    float ua, ub;
        //    if (AutodotsShapeHelper.GetUaUb(ref point0, ref point1, ref line0, ref line1, out ua, out ub))
        //        return point0 + (point1 - point0) * ua;
        //    return Vector2.Zero;
        //}

        public void FillPointInfo(ref TrinagleInfo triangle, ref TexturingInfo t, Vector2 point, ref Vector2 a, ref Vector2 b, ref Vector2 c, int triangleIndex)
        {
            if (TexturingInfo.PointInTriangle(ref a, ref b, ref c, point))
            {
                triangle.TrinagleIndex = triangleIndex;
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
            }
        }

        public void FillPointsInfo(ref TexturingInfo t, bool isShape, bool isProfile)
        {
            for (var i = 0; i < t.Indices.Length; i += 3)
            {
                var a = t.Points[t.Indices[i]].OriginalValue;
                var b = t.Points[t.Indices[i + 1]].OriginalValue;
                var c = t.Points[t.Indices[i + 2]].OriginalValue;
                for (var index = 0; index < Points.Count; index++)
                {
                    var point = Points[index];
                    var triangle = isShape ?
                        (isProfile ? point.ProfileShapeTrinagleInfo : point.ShapeTrinagleInfo)
                        : point.TextureTrinagleInfo;

                    var p = Vertices[point.Indices[0]].OriginalPosition;
                    if (triangle.TrinagleIndex < 0)
                    {
                        FillPointInfo(ref triangle, ref t, isProfile ? p.Zy : p.Xy, ref a, ref b, ref c, i / 3);
                    }

                    /* if (!isProfile)
                     {
                         var triangleMirror = isShape ? point.ShapeTrinagleInfoMirror : point.TextureTrinagleInfoMirror;
                         if(triangleMirror.TrinagleIndex < 0)
                         {
                             FillPointInfo(ref triangleMirror, ref t, new Vector2(-p.X, p.Y), ref a, ref b, ref c, i / 3);
                         }                        
                     }*/
                    Points[index] = point;
                }
            }
        }

        public void UndoMirror(bool needUpdateBuffer = true)
        {
            if (BaseVertices == null)
                return;
            Vertices = new Vertex3d[BaseVertices.Length];
            BaseVertices.CopyTo(Vertices, 0);
            BaseVertices = null;
            Indices.Clear();
            Indices.AddRange(baseIndices);
            CountIndices = Indices.Count;
            baseIndices.Clear();
#if (WEB_APP)
#else
            if (needUpdateBuffer)
            {
                UpdateBuffers();
            }
#endif
        }

        public void FindLipsEdges()
        {
            var verticesDictionary = new Dictionary<Vector3, int>(new VectorEqualityComparer());
            var points = new List<Vector3>();
            var edgesDictionary = new Dictionary<Line, int>(new VectorEqualityComparer());
            var triangle = new int[3];

            for (var i = 0; i < Indices.Count; i += 3)
            {
                for (var j = 0; j < 3; ++j)
                {
                    var point = Points[Vertices[(int)Indices[i + j]].PointIndex];
                    int index;
                    if (!verticesDictionary.TryGetValue(point.Position, out index))
                    {
                        index = points.Count;
                        points.Add(point.Position);
                        verticesDictionary.Add(point.Position, index);
                    }
                    //points[index].Add(point);
                    triangle[j] = index;
                }
                for (int j = 0, l = 2; j < 3; l = j, ++j)
                {
                    var edge = new Line(triangle[j], triangle[l]);
                    if (!edgesDictionary.ContainsKey(edge))
                        edgesDictionary.Add(edge, 1);
                    else
                        edgesDictionary[edge]++;
                }
            }

            var edges = edgesDictionary.Where(e => e.Value == 1).Select(e => e.Key).ToList();
            var edges0 = new List<Line>();
            if (edges.Count > 0)
            {
                var currentEdge = edges[0];
                while (currentEdge != null)
                {
                    if (currentEdge != null)
                    {
                        edges.Remove(currentEdge);
                        edges0.Add(currentEdge);
                    }
                    currentEdge = edges.FirstOrDefault(e => e.A == currentEdge.A || e.B == currentEdge.A || e.A == currentEdge.B || e.B == currentEdge.B);
                }
                if (edges.Count > 0 && edges0.Count > 0)
                {
                    Vector2 A = new Vector2(float.MaxValue, float.MaxValue), B = new Vector2(float.MinValue, float.MinValue);
                    Vector2 A0 = A, B0 = B;
                    foreach (var edge in edges)
                    {
                        var a = points[edge.A];
                        var b = points[edge.B];

                        A.X = Math.Min(A.X, Math.Min(a.X, b.X));
                        B.X = Math.Max(B.X, Math.Max(a.X, b.X));
                        A.Y = Math.Min(A.Y, Math.Min(a.Y, b.Y));
                        B.Y = Math.Max(B.Y, Math.Max(a.Y, b.Y));
                    }

                    foreach (var edge in edges0)
                    {
                        var a = points[edge.A];
                        var b = points[edge.B];

                        A0.X = Math.Min(A0.X, Math.Min(a.X, b.X));
                        B0.X = Math.Max(B0.X, Math.Max(a.X, b.X));
                        A0.Y = Math.Min(A0.Y, Math.Min(a.Y, b.Y));
                        B0.Y = Math.Max(B0.Y, Math.Max(a.Y, b.Y));
                    }
                }
            }
        }

        public void FindFixedPoints()
        {
            var verticesDictionary = new Dictionary<Vector3, int>(new VectorEqualityComparer());
            var points = new List<List<Point3d>>();
            var edgesDictionary = new Dictionary<Line, int>(new VectorEqualityComparer());
            var triangle = new int[3];

            var isEyelash = Name.ToLower().Contains("eyelash");
            for (var i = 0; i < Indices.Count; i += 3)
            {
                for (var j = 0; j < 3; ++j)
                {
                    var point = Points[Vertices[(int)Indices[i + j]].PointIndex];
                    if (isEyelash)
                        point.IsFixedLocal = false;
                    int index;
                    if (!verticesDictionary.TryGetValue(point.Position, out index))
                    {
                        index = points.Count;
                        points.Add(new List<Point3d>());
                        verticesDictionary.Add(point.Position, index);
                    }
                    points[index].Add(point);
                    triangle[j] = index;
                }
                for (int j = 0, l = 2; j < 3; l = j, ++j)
                {
                    var edge = new Line(triangle[j], triangle[l]);
                    if (!edgesDictionary.ContainsKey(edge))
                        edgesDictionary.Add(edge, 1);
                    else
                        edgesDictionary[edge]++;
                }
            }

            foreach (var edge in edgesDictionary.Where(e => e.Value == 1))
            {
                foreach (var point in points[edge.Key.A])
                    if (!point.IsFixedLocal.HasValue)
                        point.IsFixedLocal = true;
                foreach (var point in points[edge.Key.B])
                    if (!point.IsFixedLocal.HasValue)
                        point.IsFixedLocal = true;
            }
        }

        public void Smooth()
        {
            if (!IsShaped)
                return;

            var vertices = Vertices.Select(v => v.Position).ToArray();
            for (var i = 0; i < 3; ++i)
            {
                vertices = SmoothFilter.laplacianFilter(vertices, Indices.Select(index => (int)index).ToArray());
            }

            foreach (var p in Points)
            {
                if (p.IsFixedLocal != true)
                {
                    foreach (var i in p.Indices)
                    {
                        p.Position = vertices[i];
                        var v = Vertices[i];
                        v.Position = p.Position;
                        Vertices[i] = v;
                    }
                }
            }

        }

        public void Mirror(bool leftToRight, float axis)
        {
            IsLeftToRight = leftToRight;
            UndoMirror();
            BaseVertices = new Vertex3d[Vertices.Length];
            Vertices.CopyTo(BaseVertices, 0);
            baseIndices.Clear();
            baseIndices.AddRange(Indices);

            var mirroredPoints = new SortedList<int, int>();
            var pointsMapping = new SortedList<int, uint>();
            var vertices = new List<Vertex3d>();
            var positions = new List<Vector3>();
            var delta = leftToRight ? 0.00001f : -0.00001f;

            for (var i = 0; i < Vertices.Length; i++)
            {
                var vertex = Vertices[i];
                if (vertex.Position.X < axis + delta == leftToRight)
                {
                    var index = vertices.Count;
                    pointsMapping.Add(i, (uint)index);
                    vertices.Add(new Vertex3d
                    {
                        Position = vertex.Position,
                        TexCoord = vertex.TexCoord,
                        OriginalPosition = new Vector3(i + 2, 0.0f, 0.0f),
                        Color = Vector4.One
                    });
                    positions.Add(vertices.Last().Position);

                    if (vertex.Position.X > axis - delta == leftToRight)
                        mirroredPoints.Add(index, index);
                    else
                    {
                        mirroredPoints.Add(index, vertices.Count);

                        var texCoords = vertex.TexCoord;

                        /*if (Type == HeadMeshType.Eyes)
                        {
                            texCoords.X = texCoords.X > 0.5f ? texCoords.X - 0.5f : texCoords.X + 0.5f;
                            var originalPosition = new Vector3(-vertex.OriginalPosition.X, vertex.OriginalPosition.Y, vertex.OriginalPosition.Z);
                            var originalVertexs = Vertices.Where(v => VectorEqualityComparer.EqualsVector3(v.OriginalPosition, originalPosition)).ToArray();
                            if(originalVertexs.Length > 0)
                            {
                                //texCoords = originalVertexs[0].TexCoord;
                            }
                        }*/

                        vertices.Add(new Vertex3d
                        {
                            Position = new Vector3(axis - (vertex.Position.X - axis), vertex.Position.Y, vertex.Position.Z),
                            TexCoord = texCoords,
                            OriginalPosition = new Vector3(-(i + 2), 0.0f, 0.0f),
                            Color = Vector4.One
                        });
                        positions.Add(vertices.Last().Position);
                    }
                }
            }

            /*float maxU = float.MinValue;
            float minU = float.MaxValue;

            foreach (var vertex in vertices)
            {
                if (vertex.Position.X <= axis - delta == leftToRight)
                {
                    maxU = Math.Max(maxU, vertex.TexCoord.X);
                    minU = Math.Min(minU, vertex.TexCoord.X);
                }
            }

            if (maxU > minU)
            {
                float distance = maxU - minU;

                for(int i = 0; i < vertices.Count; ++i)
                {
                    var vertex = vertices[i];
                    if (vertex.Position.X > axis - delta == leftToRight)
                    {
                        float u = minU + (1.0f - (vertex.TexCoord.X - minU) / distance) * distance;
                        vertex.TexCoord.X = u;
                        vertices[i] = vertex;
                    }
                }*/

            var indices = new List<uint>();
            var linesMapping = new Dictionary<Line, int>(new VectorEqualityComparer());
            var lines = new[] { new Line(0, 0), new Line(0, 0) };
            var k = 0.0f;
            var idx = new int[2];
            for (var i = 0; i < Indices.Count; i += 3)
            {
                var triangle = Indices.GetRange(i, 3).Select(p => (int)p).ToArray();
                var count = triangle.Count(pointsMapping.ContainsKey);
                var centerIndex = -1;
                switch (count)
                {
                    case 3:
                        indices.AddRange(triangle.Select(t => pointsMapping[t]));
                        continue;
                    case 1:
                    case 2:
                        var c = count;
                        foreach (var t in triangle.Where(pointsMapping.ContainsKey))
                        {
                            var key = (int)pointsMapping[t];
                            if (mirroredPoints[key] == key)
                            {
                                centerIndex = t;
                                c--;
                            }
                        }
                        if (c == 0)
                            continue;
                        break;
                    case 0:
                        continue;
                }
                for (var j = 0; j < 3; j++)
                    if (pointsMapping.ContainsKey(triangle[j]) == (count == 1))
                    {
                        lines[0].A = triangle[j];
                        lines[0].B = triangle[(j + 1) % 3];
                        lines[1].A = triangle[(j + 2) % 3];
                        lines[1].B = triangle[j];
                    }
                idx[0] = linesMapping.ContainsKey(lines[0]) ? linesMapping[lines[0]] : -1;
                idx[1] = linesMapping.ContainsKey(lines[1]) ? linesMapping[lines[1]] : -1;
                for (var j = 0; j < idx.Length; j++)
                    if (idx[j] < 0)
                    {
                        var line = lines[j];
                        var v0 = Vertices[line.A];
                        var v1 = Vertices[line.B];
                        if (line.A == centerIndex || line.B == centerIndex)
                        {
                            count = 1;
                            centerIndex = (j + 1) % 2;
                        }
                        else
                        {
                            idx[j] = vertices.Count;
                            linesMapping.Add(line, vertices.Count);
                            mirroredPoints.Add(vertices.Count, vertices.Count);
                            vertices.Add(new Vertex3d
                            {
                                Position = Line.GetPoint(v0.Position, v1.Position, axis, ref k),
                                OriginalPosition = new Vector3(k, line.A, line.B),
                                TexCoord = v0.TexCoord + (v1.TexCoord - v0.TexCoord) * k,
                                Color = Vector4.One
                            });
                            positions.Add(vertices.Last().Position);
                        }
                    }
                if (count == 2)
                {
                    indices.Add((uint)idx[0]);
                    indices.Add(pointsMapping[lines[0].B]);
                    indices.Add(pointsMapping[lines[1].A]);

                    indices.Add(pointsMapping[lines[1].A]);
                    indices.Add((uint)idx[1]);
                    indices.Add((uint)idx[0]);
                }
                else
                {
                    switch (centerIndex)
                    {
                        case -1:
                            indices.Add(pointsMapping[lines[0].A]);
                            indices.Add((uint)idx[0]);
                            indices.Add((uint)idx[1]);
                            break;
                        default:
                            indices.Add(pointsMapping[lines[0].B]);
                            indices.Add(pointsMapping[lines[1].A]);
                            indices.Add((uint)idx[centerIndex]);
                            break;
                    }
                }
            }
            var cnt = indices.Count;
            for (var i = 0; i < cnt; i += 3)
            {
                var triangle = indices.GetRange(i, 3).ToArray();
                for (var j = 2; j >= 0; j--)
                    indices.Add((uint)mirroredPoints[(int)triangle[j]]);
            }

            var normals = Normal.CalculateNormals(positions, indices);
            for (var i = 0; i < normals.Count; i++)
            {
                var v = vertices[i];
                v.Normal = normals[i];
                vertices[i] = v;
            }

            Indices = indices;
            CountIndices = indices.Count;
            Vertices = vertices.ToArray();
#if (WEB_APP)
#else
            UpdateBuffers(true);
#endif
        }

#if (WEB_APP)
#else
        private void TempUpdateVertexBuffer()
        {
            for (var i = 0; i < Indices.Count / 3; i++)
            {
                var index = i * 3;
                var p0 = Vertices[(int)Indices[index]];
                var p1 = Vertices[(int)Indices[index + 1]];
                var p2 = Vertices[(int)Indices[index + 2]];
                var n = Normal.GetNormal(ref p0.Position, ref p1.Position, ref p2.Position);

                TempVertices[index].Position = p0.Position;
                TempVertices[index].TexCoord = p0.TexCoord;
                TempVertices[index].AutodotsTexCoord = p0.AutodotsTexCoord;
                TempVertices[index].Normal = n;

                TempVertices[index + 1].Position = p1.Position;
                TempVertices[index + 1].TexCoord = p1.TexCoord;
                TempVertices[index + 1].AutodotsTexCoord = p1.AutodotsTexCoord;
                TempVertices[index + 1].Normal = n;

                TempVertices[index + 2].Position = p2.Position;
                TempVertices[index + 2].TexCoord = p2.TexCoord;
                TempVertices[index + 2].AutodotsTexCoord = p2.AutodotsTexCoord;
                TempVertices[index + 2].Normal = n;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(TempVertices.Length * Vertex3d.Stride), TempVertices, BufferUsageHint.StreamDraw);
            OpenGlHelper.CheckErrors();
        }


        private void TempUpdateIndexBuffer()
        {
            UpdateIndexBuffer(TempIndices);
        }
#endif
        public void UpdateBuffers(bool firstTime = false)
        {
            UpdateVertexBuffer(firstTime);
            UpdateIndexBuffer(firstTime);
        }

        public void Destroy()
        {
#if (WEB_APP)
#else
            if (VertexBuffer != 0)
            {
                GL.DeleteBuffers(1, ref VertexBuffer);
                VertexBuffer = 0;
            }

            if (IndexBuffer != 0)
            {
                GL.DeleteBuffers(1, ref IndexBuffer);
                IndexBuffer = 0;
            }
#endif
        }

        private void UpdateVertexBuffer(bool firstTime = false)
        {
#if (WEB_APP)
#else
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);
            if (firstTime)
            {
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * Vertex3d.Stride), Vertices, BufferUsageHint.StreamDraw);
            }
            else
            {
                GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, (IntPtr)(Vertices.Length * Vertex3d.Stride), Vertices);
            }
            OpenGlHelper.CheckErrors();
#endif
        }

        public void UpdateIndexBuffer(bool firstTime = false)
        {
            UpdateIndexBuffer(Indices, firstTime);
        }

        public void UpdateIndexBuffer(List<uint> indices, bool firstTime = false)
        {
#if (WEB_APP)
#else
            CountIndices = indices.Count;
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBuffer);
            if (firstTime)
            {
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(CountIndices * sizeof(uint)), indices.ToArray(), BufferUsageHint.DynamicDraw);
            }
            else
            {
                GL.BufferSubData(BufferTarget.ElementArrayBuffer, (IntPtr)0, (IntPtr)(CountIndices * sizeof(uint)), indices.ToArray());
            }

            OpenGlHelper.CheckErrors();
#endif
        }

        public void Undo(MeshUndoInfo info)
        {
            foreach (var p in info.Points)
            {
                var point = Points[p.Key];
                point.Position = p.Value;
                foreach (var idx in point.Indices)
                {
                    var vertex = Vertices[idx];
                    vertex.Position = point.Position;
                    Vertices[idx] = vertex;
                }
            }
            UpdateNormals();
        }

        public MeshUndoInfo GetUndoInfo()
        {
            var info = new MeshUndoInfo();
            for (var i = 0; i < Points.Count; i++)
                info.Points.Add(i, Points[i].Position);
            return info;
        }

        public bool Create(MeshPartInfo info)
        {
            if (info.VertexPositions.Count == 0)
                return false;
            Guid = Guid.NewGuid();
            Color = info.Color;
            Texture = info.Texture;
            TransparentTexture = info.TransparentTexture;
            DefaultTextureName = TextureName = info.TextureName;
            TransparentTextureName = info.TransparentTextureName;

            Name = info.PartName;
            if (Name.Contains("Pupil") || Name.Contains("Eye"))// || Name.Contains("Irises") || Name.Contains("Eye") || Name.Contains("Cornea") || Name.Contains("Sclera") || Name.Contains("Pulpis"))
                Type = HeadMeshType.Eyes;
            else if (Name.Contains("SkinFace"))
                Type = HeadMeshType.Face;
            else if (Name.Contains("Lip"))
                Type = HeadMeshType.Lip;
            else if (Name.Contains("Torso"))
                Type = HeadMeshType.Torso;

            if(Name.Contains("Tongue"))
            {
                IsVisible = false;
            }

            Indices.Clear();
            var positions = new List<Vector3>();
            var texCoords = new List<Vector2>();

            var positionsDict = new Dictionary<VertexInfo, uint>(new VectorEqualityComparer());
            var pointnsDict = new Dictionary<Vector3, int>(new VectorEqualityComparer());
            var pointsIndicesDict = new Dictionary<int, int>();
            Points.Clear();
            for (var i = 0; i < info.VertexPositions.Count; i++)
            {
                var vertexInfo = new VertexInfo
                {
                    Position = info.VertexPositions[i],
                    TexCoords = info.TextureCoords[i]
                };
                if (!positionsDict.ContainsKey(vertexInfo))
                {
                    var index = (uint)positions.Count;
                    positionsDict.Add(vertexInfo, index);
                    Indices.Add(index);
                    positions.Add(vertexInfo.Position);
                    texCoords.Add(vertexInfo.TexCoords);

                    if (!pointnsDict.ContainsKey(vertexInfo.Position))
                    {
                        pointnsDict.Add(vertexInfo.Position, Points.Count);
                        pointsIndicesDict.Add((int)index, Points.Count);
                        Points.Add(new Point3d
                        {
                            Indices = new List<uint> { index },
                            Position = vertexInfo.Position
                        });

                        MorphPoints.Add(new MorphingPoint
                        {
                            Indices = new List<int> { (int)index },
                            Position = vertexInfo.Position
                        });
                    }
                    else
                    {
                        var id = pointnsDict[vertexInfo.Position];
                        Points[id].Indices.Add(index);
                        MorphPoints[id].Indices.Add((int)index);
                        pointsIndicesDict.Add((int)index, id);
                    }
                }
                else
                    Indices.Add(positionsDict[vertexInfo]);
            }

            CountIndices = Indices.Count;
            Vertices = new Vertex3d[positions.Count];

            var normals = Normal.CalculateNormals(positions, Indices);
            for (var i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Position = positions[i];
                Vertices[i].OriginalPosition = positions[i];
                Vertices[i].Normal = normals[i];
                Vertices[i].TexCoord = texCoords[i];
                Vertices[i].AutodotsTexCoord = new Vector4(texCoords[i].X, texCoords[i].Y, 1.0f, 1.0f);
                Vertices[i].Color = Vector4.One;
            }

            //temp
            TempIndices = new List<uint>();
            TempVertices = new Vertex3d[CountIndices];
            for (uint i = 0; i < CountIndices; i++)
            {
                TempIndices.Add(i);
                var v = Vertices[Indices[(int)i]];
                TempVertices[i].Position = v.Position;
                TempVertices[i].OriginalPosition = v.OriginalPosition;
                TempVertices[i].Normal = v.Normal;
                TempVertices[i].TexCoord = v.TexCoord;
                TempVertices[i].AutodotsTexCoord = v.AutodotsTexCoord;
                TempVertices[i].Color = v.Color;
            }
            //temp

            FillPoints(pointsIndicesDict);
#if (WEB_APP)
#else
            Destroy();
            GL.GenBuffers(1, out VertexBuffer);
            GL.GenBuffers(1, out IndexBuffer);
#endif
            return true;
        }


        public void ToStream(BinaryWriter bw)
        {
            bw.Write(Guid.ToString());
            bw.Write(Name);

            bw.Write(Indices.Count);
            foreach (var ind in Indices)
                bw.Write(ind);

            bw.Write(Vertices.Length);
            foreach (var vert in Vertices)
                vert.ToStream(bw);

            bw.Write(Points.Count);
            foreach (var point in Points)
                point.ToStream(bw);

            bw.Write(DefaultTextureName ?? string.Empty);
            bw.Write(TextureName ?? string.Empty);              // если isBase - нужно будет фотку текстуры подсунуть
            bw.Write(TransparentTextureName ?? string.Empty);

            Color.ToStream(bw);
            bw.Write((int)Type);
            bw.Write(IsBaseTexture);
        }
        public static RenderMeshPart FromStream(BinaryReader br)
        {
            var result = new RenderMeshPart();
            result.Guid = new Guid(br.ReadString());
            result.Name = br.ReadString();

            var cnt = br.ReadInt32();
            result.CountIndices = cnt;
            for (var i = 0; i < cnt; i++)
                result.Indices.Add(br.ReadUInt32());

            cnt = br.ReadInt32();
            if (cnt != 0)
            {
                result.Vertices = new Vertex3d[cnt];
                for (var i = 0; i < cnt; i++)
                    result.Vertices[i] = Vertex3d.FromStream(br);
            }

            cnt = br.ReadInt32();
            var oldVersion = false;
            for (var i = 0; i < cnt; i++)
                result.Points.Add(Point3d.FromStream(br, ref oldVersion));

            result.DefaultTextureName = br.ReadString();
            result.TextureName = br.ReadString();
            result.TransparentTextureName = br.ReadString();

            result.Color = Vector4Ex.FromStream(br);
            result.Type = (HeadMeshType)br.ReadInt32();
            result.IsBaseTexture = br.ReadBoolean();

#if (WEB_APP)
#else
            result.Destroy();
            GL.GenBuffers(1, out result.VertexBuffer);
            GL.GenBuffers(1, out result.IndexBuffer);
#endif

            //temp
            result.TempIndices = new List<uint>();
            result.TempVertices = new Vertex3d[result.CountIndices];
            for (uint i = 0; i < result.CountIndices; ++i)
            {
                result.TempIndices.Add(i);
                var v = result.Vertices[result.Indices[(int)i]];
                result.TempVertices[i].Position = v.Position;
                result.TempVertices[i].OriginalPosition = v.OriginalPosition;
                result.TempVertices[i].Normal = v.Normal;
                result.TempVertices[i].TexCoord = v.TexCoord;
                result.TempVertices[i].AutodotsTexCoord = v.AutodotsTexCoord;
                result.TempVertices[i].Color = v.Color;
            }
            //temp

            if (result.Points.FirstOrDefault(x => x.isFixedLocalBroken) != null)
                result.FindFixedPoints();

            return result;
        }

        #endregion

        #region Private

        private void FillPoints(Dictionary<int, int> dictionary)
        {
            PointsIndices.Clear();
            for (var p = 0; p < Points.Count; p++)
            {
                var point = Points[p];
                PointsIndices.Add(point, p);
                var triangles = new List<int>();
                foreach (var i in point.Indices)
                {
                    Vertices[i].PointIndex = p;
                    for (var t = 0; t < Indices.Count; t++)
                        if (Indices[t] == i)
                            triangles.Add(t - t % 3);
                }
                point.Nearests.Clear();
                //ищем все соседние точки
                foreach (var t in triangles)
                {
                    for (var i = t; i < t + 3; i++)
                    {
                        var ti = (int)Indices[i];
                        var pt = dictionary[ti];
                        if (pt != p && !point.Nearests.Contains(pt))
                            point.Nearests.Add(pt);
                    }
                }
            }
        }

        #endregion

        public void SetMorphPercent(float percent)
        {
            foreach (var point in MorphPoints)
            {
                foreach (var index in point.Indices)
                {
                    var vertex = Vertices[index];
                    vertex.Position = point.Position * percent + vertex.OriginalPosition * (1.0f - percent);
                    Vertices[index] = vertex;
                }
            }
            UpdateVertexBuffer();
        }
    }

    public class RenderMeshParts : List<RenderMeshPart>
    {
        public bool Contains(Guid id)
        {
            foreach (var item in this)
            {
                if (item.Guid == id)
                    return true;
            }
            return false;
        }

        public RenderMeshPart this[Guid id]
        {
            get
            {
                return this.FirstOrDefault(item => item.Guid == id);
            }
        }
    }
}
