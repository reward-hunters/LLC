using RH.MeshUtils.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace RH.Core.HeadRotation
{
    class CustomHeadTriangles
    {
        public List<MorphTriangle> TrianglesFront = new List<MorphTriangle>();

        public CustomHeadTriangles()

        {
            TrianglesFront.Add(new MorphTriangle { A = 52, B = 5, C = 68, Type = MorphTriangleType.Left });
            TrianglesFront.Add(new MorphTriangle { A = 52, B = 66, C = 68, Type = MorphTriangleType.Left });
            //TrianglesFront.Add(new MorphTriangle { A = 5, B = 3, C = 68, Type = MorphTriangleType.Left });
            TrianglesFront.Add(new MorphTriangle { A = 5, B = 52, C = 3, Type = MorphTriangleType.Left });
            TrianglesFront.Add(new MorphTriangle { A = 5, B = 3, C = 7, Type = MorphTriangleType.Left });
            TrianglesFront.Add(new MorphTriangle { A = 3, B = 58, C = 7, Type = MorphTriangleType.Left });
            TrianglesFront.Add(new MorphTriangle { A = 9, B = 58, C = 7, Type = MorphTriangleType.Left });
            TrianglesFront.Add(new MorphTriangle { A = 9, B = 58, C = 55, Type = MorphTriangleType.Left });
            TrianglesFront.Add(new MorphTriangle { A = 9, B = 55, C = 11, Type = MorphTriangleType.Left });
            TrianglesFront.Add(new MorphTriangle { A = 45, B = 66, C = 43, Type = MorphTriangleType.Left });
            TrianglesFront.Add(new MorphTriangle { A = 66, B = 52, C = 45, Type = MorphTriangleType.Left });
            
            TrianglesFront.Add(new MorphTriangle { A = 23, B = 66, C = 43 });
            TrianglesFront.Add(new MorphTriangle { A = 66, B = 12, C = 23 });

            //right oval
            //TrianglesFront.Add(new MorphTriangle { A = 4, B = 53, C = 69, Type = MorphTriangleType.Right });
            TrianglesFront.Add(new MorphTriangle { A = 69, B = 53, C = 6, Type = MorphTriangleType.Right });
            TrianglesFront.Add(new MorphTriangle { A = 67, B = 53, C = 69, Type = MorphTriangleType.Right });
            //TrianglesFront.Add(new MorphTriangle { A = 6, B = 4, C = 69, Type = MorphTriangleType.Right });
            TrianglesFront.Add(new MorphTriangle { A = 53, B = 4, C = 6, Type = MorphTriangleType.Right });
            TrianglesFront.Add(new MorphTriangle { A = 6, B = 4, C = 8, Type = MorphTriangleType.Right });
            TrianglesFront.Add(new MorphTriangle { A = 4, B = 59, C = 8, Type = MorphTriangleType.Right });
            TrianglesFront.Add(new MorphTriangle { A = 10, B = 59, C = 8, Type = MorphTriangleType.Right });
            TrianglesFront.Add(new MorphTriangle { A = 10, B = 59, C = 55, Type = MorphTriangleType.Right });
            TrianglesFront.Add(new MorphTriangle { A = 10, B = 55, C = 11, Type = MorphTriangleType.Right });
            TrianglesFront.Add(new MorphTriangle { A = 46, B = 67, C = 44, Type = MorphTriangleType.Right });
            TrianglesFront.Add(new MorphTriangle { A = 46, B = 53, C = 67, Type = MorphTriangleType.Right });
            
            TrianglesFront.Add(new MorphTriangle { A = 26, B = 67, C = 44 });
            TrianglesFront.Add(new MorphTriangle { A = 26, B = 67, C = 15 });

            //left eye
            TrianglesFront.Add(new MorphTriangle { A = 23, B = 35, C = 37 });
            TrianglesFront.Add(new MorphTriangle { A = 0, B = 35, C = 37 });
            TrianglesFront.Add(new MorphTriangle { A = 28, B = 35, C = 0 });
            TrianglesFront.Add(new MorphTriangle { A = 28, B = 36, C = 0 });
            TrianglesFront.Add(new MorphTriangle { A = 36, B = 24, C = 38 });
            TrianglesFront.Add(new MorphTriangle { A = 36, B = 0, C = 38 });
            TrianglesFront.Add(new MorphTriangle { A = 27, B = 0, C = 38 });
            TrianglesFront.Add(new MorphTriangle { A = 27, B = 0, C = 37 });
            //left eyelash
            TrianglesFront.Add(new MorphTriangle { A = 23, B = 12, C = 18 });
            TrianglesFront.Add(new MorphTriangle { A = 18, B = 35, C = 23 });
            TrianglesFront.Add(new MorphTriangle { A = 18, B = 35, C = 16 });
            TrianglesFront.Add(new MorphTriangle { A = 28, B = 35, C = 16 });
            TrianglesFront.Add(new MorphTriangle { A = 28, B = 19, C = 16 });
            TrianglesFront.Add(new MorphTriangle { A = 28, B = 19, C = 36 });
            TrianglesFront.Add(new MorphTriangle { A = 13, B = 19, C = 36 });
            TrianglesFront.Add(new MorphTriangle { A = 24, B = 22, C = 13 });
            TrianglesFront.Add(new MorphTriangle { A = 13, B = 36, C = 24 });
            TrianglesFront.Add(new MorphTriangle { A = 38, B = 24, C = 43 });
            TrianglesFront.Add(new MorphTriangle { A = 37, B = 27, C = 43 });
            TrianglesFront.Add(new MorphTriangle { A = 37, B = 23, C = 43 });

            //right eye
            TrianglesFront.Add(new MorphTriangle { A = 26, B = 40, C = 42 });
            TrianglesFront.Add(new MorphTriangle { A = 1, B = 40, C = 42 });
            TrianglesFront.Add(new MorphTriangle { A = 32, B = 40, C = 1 });
            TrianglesFront.Add(new MorphTriangle { A = 32, B = 39, C = 1 });
            TrianglesFront.Add(new MorphTriangle { A = 39, B = 25, C = 41 });
            TrianglesFront.Add(new MorphTriangle { A = 39, B = 1, C = 41 });
            TrianglesFront.Add(new MorphTriangle { A = 31, B = 1, C = 41 });
            TrianglesFront.Add(new MorphTriangle { A = 31, B = 1, C = 42 });
            //right eyelash
            TrianglesFront.Add(new MorphTriangle { A = 26, B = 15, C = 21 });
            TrianglesFront.Add(new MorphTriangle { A = 21, B = 40, C = 26 });
            TrianglesFront.Add(new MorphTriangle { A = 21, B = 40, C = 17 });
            TrianglesFront.Add(new MorphTriangle { A = 32, B = 40, C = 17 });
            TrianglesFront.Add(new MorphTriangle { A = 32, B = 20, C = 17 });
            TrianglesFront.Add(new MorphTriangle { A = 32, B = 20, C = 39 });
            TrianglesFront.Add(new MorphTriangle { A = 14, B = 20, C = 39 });
            TrianglesFront.Add(new MorphTriangle { A = 25, B = 22, C = 14 });
            TrianglesFront.Add(new MorphTriangle { A = 14, B = 25, C = 39 });
            TrianglesFront.Add(new MorphTriangle { A = 25, B = 41, C = 44 });
            TrianglesFront.Add(new MorphTriangle { A = 31, B = 42, C = 44 });
            TrianglesFront.Add(new MorphTriangle { A = 26, B = 42, C = 44 });

            //middle face
            TrianglesFront.Add(new MorphTriangle { A = 14, B = 22, C = 13 });
            
            // nose
            TrianglesFront.Add(new MorphTriangle { A = 43, B = 22, C = 2 });
            TrianglesFront.Add(new MorphTriangle { A = 43, B = 45, C = 2 });
            TrianglesFront.Add(new MorphTriangle { A = 47, B = 45, C = 2 });
            TrianglesFront.Add(new MorphTriangle { A = 44, B = 22, C = 2 });
            TrianglesFront.Add(new MorphTriangle { A = 44, B = 46, C = 2 });
            TrianglesFront.Add(new MorphTriangle { A = 46, B = 48, C = 2 });
            TrianglesFront.Add(new MorphTriangle { A = 48, B = 49, C = 2 });
            TrianglesFront.Add(new MorphTriangle { A = 47, B = 49, C = 2 });
            // nose-eyes
            TrianglesFront.Add(new MorphTriangle { A = 25, B = 22, C = 44 });
            TrianglesFront.Add(new MorphTriangle { A = 44, B = 41, C = 31 });
            TrianglesFront.Add(new MorphTriangle { A = 24, B = 22, C = 43 });
            TrianglesFront.Add(new MorphTriangle { A = 43, B = 38, C = 27 });

            // left mouth
            TrianglesFront.Add(new MorphTriangle { A = 3, B = 52, C = 56 });
            TrianglesFront.Add(new MorphTriangle { A = 45, B = 52, C = 56 });
            TrianglesFront.Add(new MorphTriangle { A = 45, B = 47, C = 56 });
            TrianglesFront.Add(new MorphTriangle { A = 49, B = 47, C = 56 });
            TrianglesFront.Add(new MorphTriangle { A = 49, B = 54, C = 56 });
            TrianglesFront.Add(new MorphTriangle { A = 3, B = 60, C = 56 });
            TrianglesFront.Add(new MorphTriangle { A = 3, B = 60, C = 63 });
            TrianglesFront.Add(new MorphTriangle { A = 3, B = 58, C = 63 });
            TrianglesFront.Add(new MorphTriangle { A = 64, B = 58, C = 55 });
            TrianglesFront.Add(new MorphTriangle { A = 63, B = 61, C = 64 });
            TrianglesFront.Add(new MorphTriangle { A = 60, B = 54, C = 61 });
            TrianglesFront.Add(new MorphTriangle { A = 60, B = 54, C = 56 });
            TrianglesFront.Add(new MorphTriangle { A = 63, B = 64, C = 58 });
            TrianglesFront.Add(new MorphTriangle { A = 63, B = 60, C = 61 });
            // right mouth
            TrianglesFront.Add(new MorphTriangle { A = 4, B = 53, C = 57 });
            TrianglesFront.Add(new MorphTriangle { A = 46, B = 53, C = 57 });
            TrianglesFront.Add(new MorphTriangle { A = 46, B = 48, C = 57 });
            TrianglesFront.Add(new MorphTriangle { A = 49, B = 48, C = 57 });
            TrianglesFront.Add(new MorphTriangle { A = 49, B = 54, C = 57 });
            TrianglesFront.Add(new MorphTriangle { A = 4, B = 62, C = 57 });
            TrianglesFront.Add(new MorphTriangle { A = 4, B = 62, C = 65 });
            TrianglesFront.Add(new MorphTriangle { A = 4, B = 59, C = 65 });
            TrianglesFront.Add(new MorphTriangle { A = 64, B = 59, C = 55 });
            TrianglesFront.Add(new MorphTriangle { A = 65, B = 61, C = 64 });
            TrianglesFront.Add(new MorphTriangle { A = 62, B = 54, C = 61 });
            TrianglesFront.Add(new MorphTriangle { A = 62, B = 54, C = 57 });
            TrianglesFront.Add(new MorphTriangle { A = 65, B = 64, C = 59 });
            TrianglesFront.Add(new MorphTriangle { A = 65, B = 62, C = 61 });
        }

        public void DrawTriangles(HeadPoints headPoints)
        {
            GL.Begin(PrimitiveType.Triangles);
            GL.Color4(0.0f, 1.0f, 0.0f, 0.3f);
            
            foreach (var triangle in TrianglesFront)
            {
                var a = headPoints.GetWorldPoint(triangle.A);
                var b = headPoints.GetWorldPoint(triangle.B);
                var c = headPoints.GetWorldPoint(triangle.C);
                GL.Vertex3(a);
                GL.Vertex3(b);
                GL.Vertex3(c);
            }
            GL.End();
        }
    }
}
