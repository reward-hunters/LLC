using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using RH.MeshUtils.Helpers;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;

namespace RH.MeshUtils.Data
{
    public class RenderMesh
    {
        public delegate void BeforePartDrawHandler(RenderMeshPart part);
        public event BeforePartDrawHandler OnBeforePartDraw;
        public List<BlendingInfo> BlendingInfos = new List<BlendingInfo>();

        public Matrix4 RotationMatrix
        {
            get { return rotationMatrix; }
            set
            {
                rotationMatrix = value;
                Matrix4.Invert(ref rotationMatrix, out invRotationMatrix);
            }
        }

        private Matrix4 rotationMatrix = Matrix4.Identity;
        private Matrix4 invRotationMatrix = Matrix4.Identity;

        //Reversed rotation
        public Matrix4 ReverseRotationMatrix
        {
            get { return reverseRotationMatrix; }
            set
            {
                reverseRotationMatrix = value;
                Matrix4.Invert(ref reverseRotationMatrix, out invReverseRotationMatrix);
            }
        }

        private Matrix4 reverseRotationMatrix = Matrix4.Identity;
        private Matrix4 invReverseRotationMatrix = Matrix4.Identity;

        public Quaternion MeshQuaternion = Quaternion.Identity;

        public RenderMeshParts Parts
        {
            get;
            set;
        }

        //Угол поворота головы
        public float HeadAngle
        {
            get;
            set;
        }

        public float FaceCenterX
        {
            get;
            set;
        }

        public float NoseDepth
        {
            get;
            set;
        }

        public RectangleAABB AABB = new RectangleAABB();
        public Vector2 Scale = Vector2.One;
        public Vector2 Center = Vector2.Zero;        
        private static float MORPH_SCALE = 10.0f;
        private static float MORPH_SCALE_MAX = 0.7f;
        private static float MORPH_SCALE_MIN = 0.15f;
        public float MorphScale = 8.0f;

        public float RealScale
        {
            get
            {
                return (MorphScale * 0.125f/*Уменьшаем в 8 раз как сказал старик*/) / 4.2f;
            }
        }

        public RenderMesh()
        {
            Parts = new RenderMeshParts();
        }

        private bool morphing = false;

        public void BeginMorph()
        {
            if (morphing)
                return;
            foreach (var part in Parts)
                part.BeginMorph();
            morphing = true;
        }

        public void EndMorph()
        {
            if (!morphing)
                return;
            foreach (var part in Parts)
                part.EndMorph();
            morphing = false;
        }

        public void DoMorph(float k)
        {
            if (!morphing)
                return;
            foreach (var part in Parts)
                part.DoMorph(k);
        }

        public void ScaleWidth(float k, float centerX)
        {
            foreach (var part in Parts)
            {
                for (var i = 0; i < part.Vertices.Length; i++)
                {
                    var vertex = part.Vertices[i];
                    vertex.Position.X = centerX + (vertex.Position.X - centerX) * k;
                    vertex.OriginalPosition.X = centerX + (vertex.OriginalPosition.X - centerX) * k;
                    part.Vertices[i] = vertex;
                }
                foreach (var p in part.Points)
                {
                    var pos = p.Position;
                    pos.X = centerX + (pos.X - centerX) * k;
                    p.Position = pos;
                }
            }
        }

        public float SetSize(float diagonal)
        {
            Vector2 a = new Vector2(99999.0f, 99999.0f), b = new Vector2(-99999.0f, -99999.0f);
            foreach (var part in Parts)
                foreach (var vertex in part.Vertices)
                {
                    a.X = Math.Min(vertex.Position.X, a.X);
                    a.Y = Math.Min(vertex.Position.Y, a.Y);
                    b.X = Math.Max(vertex.Position.X, b.X);
                    b.Y = Math.Max(vertex.Position.Y, b.Y);
                }
            var d = (b - a).Length;
            if (d == 0.0f)
                return 0.0f;
            var scale = diagonal / d;
            foreach (var part in Parts)
            {
                for (var i = 0; i < part.Vertices.Length; i++)
                {
                    var vertex = part.Vertices[i];
                    vertex.OriginalPosition = vertex.Position;
                    vertex.Position.X *= scale;
                    vertex.Position.Y *= scale;
                    vertex.Position.Z *= scale;
                    part.Vertices[i] = vertex;
                }
                foreach (var p in part.Points)
                {
                    var pos = p.Position;
                    pos.X *= scale;
                    pos.Y *= scale;
                    pos.Z *= scale;
                    p.Position = pos;
                }
            }
            return 1.0f / scale;
        }

        public float Transform(float k, RectangleAABB aabb)
        {
            if (Parts.Count == 0)
                return 0.0f;
            AABB = aabb;
            var newWidht = aabb.Height * k;
            var scaleX = newWidht / aabb.Width;
            //Scale.X = scaleX;
            //Scale.X = 1.0f;//(float)Math.Sqrt(Math.Abs(k * AABB.Size.Y / AABB.Size.X));
            //Scale.Y = 1.0f / Scale.X;
            var centerX = (aabb.B.X + aabb.A.X) * 0.5f;
            //var count = 0.0f;
            //foreach (var part in Parts)
            //    foreach (var v in part.Vertices)
            //    {
            //        count += 1.0f;
            //        center += v.Position;
            //    }
            //center /= count;
            //Center = center.Xy;

            foreach (var part in Parts)
            {
                for (var i = 0; i < part.Vertices.Length; i++)
                {
                    var vertex = part.Vertices[i];
                    vertex.OriginalPosition = vertex.Position;
                    vertex.Position.X -= centerX;
                    vertex.Position.X *= scaleX;
                    //vertex.Position.Y *= Scale.Y;
                    vertex.Position.X += centerX;
                    part.Vertices[i] = vertex;    
                }
                foreach (var p in part.Points)
                {
                    var pos = p.Position;
                    pos.X -= centerX;
                    pos.X *= scaleX;
                    //pos.Y *= Scale.Y;
                    pos.X += centerX;
                    p.Position = pos;
                }
            }

            AABB.A = new Vector3((AABB.A.X - centerX) * scaleX + centerX, AABB.A.Y, AABB.A.Z);
            AABB.B = new Vector3((AABB.B.X - centerX) * scaleX + centerX, AABB.B.Y, AABB.B.Z);
            return scaleX;
        }

        public void SetBlendingInfo(Vector2 leye, Vector2 reye, Vector2 lip, Vector2 face)
        {
            var aabb = new RectangleAABB();

            var a = aabb.A;
            var b = aabb.B;
            a.Y = Math.Min(Math.Min(leye.Y, lip.Y), reye.Y);
            b.Y = Math.Max(Math.Max(leye.Y, lip.Y), reye.Y);
            a.X = Math.Min(Math.Min(leye.X, lip.X), reye.X);
            b.X = Math.Max(Math.Max(leye.X, lip.X), reye.X);
            aabb.A = a;
            aabb.B = b;

            BlendingInfos.Clear();
            var radius = Math.Abs(leye.X - reye.X) * 0.35f;
            BlendingInfos.Add(new BlendingInfo
            {
                Position = leye,
                Radius = radius
            });
            BlendingInfos.Add(new BlendingInfo
            {
                Position = reye,
                Radius = radius
            });
            BlendingInfos.Add(new BlendingInfo
            {
                Position = aabb.Center.Xy,
                Radius = (aabb.Center.Xy - aabb.B.Xy).Length * 0.8f
            });
            BlendingInfos.Add(new BlendingInfo
            {
                Position = lip,
                Radius = (aabb.Center.Xy - lip).Length * 0.75f
            });
            BlendingInfos.Add(new BlendingInfo
            {
                Position = (leye + reye) * 0.5f,
                Radius = (aabb.B.Xy - face).Length * 0.75f
            });
            foreach (var part in Parts)
                part.FillBlendingData(BlendingInfos);
        }

        public void UpdateAABB(RenderMeshPart part, ref Vector3 a, ref Vector3 b)
        {
            foreach (var vertex in part.Vertices)
            {
                a.Z = Math.Min(vertex.Position.Z, a.Z);
                b.Z = Math.Max(vertex.Position.Z, b.Z);
            }
        }

        public void AddPart(RenderMeshPart part)
        {
            Parts.Add(part);
            var a = AABB.A;
            var b = AABB.B;
            UpdateAABB(part, ref a, ref b);
            AABB.A = a;
            AABB.B = b;
        }

        public void FindFixedPoints()
        {
            return;

            var verticesDictionary = new Dictionary<Vector3, int>(new VectorEqualityComparer());
            var points = new List<List<Point3d>>();
            var edgesDictionary = new Dictionary<Line, int>(new VectorEqualityComparer());
            var triangle = new int[3];
            foreach (var part in Parts)
            {
                if (part.Type == HeadMeshType.Torso || part.Type == HeadMeshType.Lip)
                    continue;
                //if (part.Type == HeadMeshType.Lip)
                //{
                //    part.FindLipsEdges();
                //}
                part.FindFixedPoints();
                var isEyelash = part.Name.ToLower().Contains("eyelash");
                for (var i = 0; i < part.Indices.Count; i += 3)
                {
                    for (var j = 0; j < 3; ++j)
                    {
                        var point = part.Points[part.Vertices[(int)part.Indices[i + j]].PointIndex];
                        if (isEyelash)
                            point.IsFixed = false;
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
            }
            foreach (var edge in edgesDictionary.Where(e => e.Value == 1))
            {
                foreach (var point in points[edge.Key.A])
                    if(!point.IsFixed.HasValue)
                        point.IsFixed = true;
                foreach (var point in points[edge.Key.B])
                    if (!point.IsFixed.HasValue)
                        point.IsFixed = true;
            }
        }

        public Vector3 GetWorldPoint(Vector3 point)
        {
            var point4 = new Vector4(point);
            return Vector4.Transform(point4, RotationMatrix).Xyz;
        }

        public Vector3 GetPositionFromWorld(Vector3 point)
        {
            var point4 = new Vector4(point);
            return Vector4.Transform(point4, invRotationMatrix).Xyz;
        }

        public Vector3 GetReverseWorldPoint(Vector3 point)
        {
            var point4 = new Vector4(point);
            return Vector4.Transform(point4, ReverseRotationMatrix).Xyz;
        }

        public Vector3 GetReversePositionFromWorld(Vector3 point)
        {
            var point4 = new Vector4(point);
            return Vector4.Transform(point4, invReverseRotationMatrix).Xyz;
        }

        public void DetectFaceRotationEmgu(int ImageWidth, int ImageHeight, List<Vector2> RealPoints, List<Vector3> HeadPoints)
        {
            var imagePoints = new List<PointF>();
            imagePoints.Add(new PointF(RealPoints[66].X, RealPoints[66].Y));        // уши
            imagePoints.Add(new PointF(RealPoints[67].X, RealPoints[67].Y));
            imagePoints.Add(new PointF(RealPoints[0].X, RealPoints[0].Y));       // глаза центры
            imagePoints.Add(new PointF(RealPoints[1].X, RealPoints[1].Y));
            imagePoints.Add(new PointF(RealPoints[3].X, RealPoints[3].Y));       // левый-правый угол рта
            imagePoints.Add(new PointF(RealPoints[4].X, RealPoints[4].Y));
            imagePoints.Add(new PointF(RealPoints[2].X, RealPoints[2].Y));       // центр носа

            var modelPoints = new List<MCvPoint3D32f>();
            modelPoints.Add(new MCvPoint3D32f(HeadPoints[66].X, HeadPoints[66].Y, HeadPoints[66].Z));
            modelPoints.Add(new MCvPoint3D32f(HeadPoints[67].X, HeadPoints[67].Y, HeadPoints[67].Z));
            modelPoints.Add(new MCvPoint3D32f(HeadPoints[0].X, HeadPoints[0].Y, HeadPoints[0].Z));
            modelPoints.Add(new MCvPoint3D32f(HeadPoints[1].X, HeadPoints[1].Y, HeadPoints[1].Z));
            modelPoints.Add(new MCvPoint3D32f(HeadPoints[3].X, HeadPoints[3].Y, HeadPoints[3].Z));
            modelPoints.Add(new MCvPoint3D32f(HeadPoints[4].X, HeadPoints[4].Y, HeadPoints[4].Z));
            modelPoints.Add(new MCvPoint3D32f(HeadPoints[2].X, HeadPoints[2].Y, HeadPoints[2].Z));

            #region CamMatrix

            //var img = CvInvoke.Imread(ProgramCore.MainForm.PhotoControl.TemplateImage);
            //float imageWidth = img.Cols;
            // float imageHeight = img.Rows;
            float imageWidth = ImageWidth;
            float imageHeight = ImageHeight;
            var max_d = Math.Max(imageWidth, imageHeight);
            var camMatrix = new Emgu.CV.Matrix<double>(3, 3);
            camMatrix[0, 0] = max_d;
            camMatrix[0, 1] = 0;
            camMatrix[0, 2] = imageWidth / 2.0;
            camMatrix[1, 0] = 0;
            camMatrix[1, 1] = max_d;
            camMatrix[1, 2] = imageHeight / 2.0;
            camMatrix[2, 0] = 0;
            camMatrix[2, 1] = 0;
            camMatrix[2, 2] = 1.0;

            /*
            float max_d = Mathf.Max (imageHeight, imageWidth);
            camMatrix = new Mat (3, 3, CvType.CV_64FC1);
            camMatrix.put (0, 0, max_d);
            camMatrix.put (0, 1, 0);
            camMatrix.put (0, 2, imageWidth / 2.0f);
            camMatrix.put (1, 0, 0);
            camMatrix.put (1, 1, max_d);
            camMatrix.put (1, 2, imageHeight / 2.0f);
            camMatrix.put (2, 0, 0);
            camMatrix.put (2, 1, 0);
            camMatrix.put (2, 2, 1.0f);
             */

            #endregion

            var distArray = new double[] { 0, 0, 0, 0 };
            var distMatrix = new Matrix<double>(distArray);      // не используемый коэф.

            var rv = new double[] { 0, 0, 0 };
            var rvec = new Matrix<double>(rv);

            var tv = new double[] { 0, 0, 1 };
            var tvec = new Matrix<double>(tv);

            Emgu.CV.CvInvoke.SolvePnP(modelPoints.ToArray(), imagePoints.ToArray(), camMatrix, distMatrix, rvec, tvec, false, Emgu.CV.CvEnum.SolvePnpMethod.EPnP);      // решаем проблему PNP

            double tvec_z = tvec[2, 0];
            var rotM = new Matrix<double>(3, 3);

            if (!double.IsNaN(tvec_z))
            {
                CvInvoke.Rodrigues(rvec, rotM);
                Matrix4 transformationM = new Matrix4();
                transformationM.Row0 = new Vector4((float)rotM[0, 0], (float)rotM[0, 1], (float)rotM[0, 2], (float)tvec[0, 0]);
                transformationM.Row1 = new Vector4((float)rotM[1, 0], (float)rotM[1, 1], (float)rotM[1, 2], (float)tvec[1, 0]);
                transformationM.Row2 = new Vector4((float)rotM[2, 0], (float)rotM[2, 1], (float)rotM[2, 2], (float)tvec[2, 0]);
                transformationM.Row3 = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

                var quaternion = MathHelpers.ExtractRotationFromMatrix(ref transformationM);

                //quaternion.Y = -quaternion.Y;

                // RotationMatrix = CreateRotationMatrix(quaternion);
                // RotationMatrix = Matrix4.CreateFromQuaternion(quaternion);
                // RotationMatrix = invertYM * RotationMatrix * invertZM;

                quaternion.X = -quaternion.X;
                quaternion.Y = -quaternion.Y;
                quaternion.Z = -quaternion.Z;

                MeshQuaternion = quaternion;

                var angles = MathHelpers.ToEulerRad(MeshQuaternion);
                if (angles.X > -5.0f && angles.X < 5.0f)
                    angles.X = 0.0f;

                HeadAngle = angles.Y;

                MeshQuaternion = quaternion = MathHelpers.ToQ(angles);

                RotationMatrix = MathHelpers.CreateRotationMatrix(quaternion);

                quaternion.Z = -MeshQuaternion.Z;
                ReverseRotationMatrix = MathHelpers.CreateRotationMatrix(quaternion);
            }
            else
            {
                MeshQuaternion = Quaternion.Identity;
                HeadAngle = 0.0f;
            }

        }

        public void Draw(bool debug)
        {
#if (WEB_APP)
#else
            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.ColorArray);

            foreach (var part in Parts)
            {
                if (OnBeforePartDraw != null)
                    OnBeforePartDraw(part);

                GL.BindBuffer(BufferTarget.ArrayBuffer, part.VertexBuffer);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, part.IndexBuffer);

                GL.VertexPointer(3, VertexPointerType.Float, Vertex3d.Stride, new IntPtr(0));
                GL.NormalPointer(NormalPointerType.Float, Vertex3d.Stride, new IntPtr(Vector3.SizeInBytes));
                // GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex3d.Stride, new IntPtr(2 * Vector3.SizeInBytes + Vector2.SizeInBytes + Vector4.SizeInBytes));
                GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex3d.Stride, new IntPtr(2 * Vector3.SizeInBytes));
                GL.ColorPointer(4, ColorPointerType.Float, Vertex3d.Stride, new IntPtr(2 * Vector3.SizeInBytes + Vector2.SizeInBytes));

                GL.DrawRangeElements(PrimitiveType.Triangles, 0, part.CountIndices, part.CountIndices, DrawElementsType.UnsignedInt, new IntPtr(0));
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.ColorArray);

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);

            if (debug)
                OpenGlHelper.DrawAABB(AABB.A, AABB.B);
#endif
        }

        public void DrawToTexture(IEnumerable<RenderMeshPart> parts)
        {
#if (WEB_APP)
#else
            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            foreach (var part in parts)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, part.VertexBuffer);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, part.IndexBuffer);

                GL.VertexPointer(2, VertexPointerType.Float, Vertex3d.Stride, new IntPtr(2 * Vector3.SizeInBytes)); //Как позицию используем основные текстурные координаты
                GL.NormalPointer(NormalPointerType.Float, Vertex3d.Stride, new IntPtr(0));//Как нормаль используем позиции (координата Z потребуется для вычисления смешивания 
                GL.TexCoordPointer(3, TexCoordPointerType.Float, Vertex3d.Stride, new IntPtr(2 * Vector3.SizeInBytes + Vector2.SizeInBytes + Vector4.SizeInBytes));//Как текстурные координаты берем дополнительные текстурные координаты

                GL.DrawRangeElements(PrimitiveType.Triangles, 0, part.CountIndices, part.CountIndices, DrawElementsType.UnsignedInt, new IntPtr(0));
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
#endif
        }

        public void Destroy()
        {
            foreach (var part in Parts)
            {
                part.Destroy();
            }
        }

        public void Save(string path)
        {
            using (var bw = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
            {
                AABB.ToStream(bw);
                Scale.ToStream(bw);
                Center.ToStream(bw);
                bw.Write(MorphScale);

                bw.Write(Parts.Count);
                foreach (var part in Parts)
                    part.ToStream(bw);
            }
        }

        public void Load(string path)
        {
            if (!File.Exists(path))
                return;

            Parts.Clear();
            using (var br = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                AABB = RectangleAABB.FromStream(br);
                Scale = Vector2Ex.FromStream(br);
                Center = Vector2Ex.FromStream(br);
                MorphScale = br.ReadSingle();

                var cnt = br.ReadInt32();
                for (var i = 0; i < cnt; i++)
                    Parts.Add(RenderMeshPart.FromStream(br));
            }
        }
    }
}
