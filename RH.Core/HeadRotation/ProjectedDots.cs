using OpenTK;
using OpenTK.Graphics.OpenGL;
using RH.Core.Helpers;
using System.Collections.Generic;

namespace RH.Core.HeadRotation
{
    public class ProjectedDots
    {
        public int RootIndex = 66;
        public int HelperIndex = 67;

        public List<Vector2> Points = new List<Vector2>();

        public void Draw()
        {
            GL.PointSize(5.0f);
            GL.Begin(PrimitiveType.Points);

            GL.Color3(0.0f, 0.0f, 1.0f);

            foreach (var point in Points)
            {
                GL.Vertex2(point);
            }
        
            GL.End();
            GL.PointSize(1.0f);
        }

        public void Initialize( HeadPoints headPoints, List<Vector3> facialFeatures)
        {
            if (facialFeatures.Count < headPoints.Points.Count)
                return;

            if (headPoints.Points.Count <= RootIndex)
                return;

            if (headPoints.Points.Count <= HelperIndex)
                return;

            Points.Clear();

            var rootPointWorld = headPoints.GetWorldPoint(RootIndex);
            var helperPointWorld = headPoints.GetWorldPoint(HelperIndex);

            var rootPointPhoto = facialFeatures[RootIndex];
            var helperPointPhoto = facialFeatures[HelperIndex];

            float distWorld = rootPointWorld.X - helperPointWorld.X;
            float distPhoto = rootPointPhoto.X - helperPointPhoto.X;

            float scale = distWorld / distPhoto;

            foreach (var point in facialFeatures)
            {
                var p = point - rootPointPhoto;
                p.X *= scale;
                p.Y *= -scale;
                p += rootPointWorld;
                Points.Add(p.Xy);
            }
        }
    }
}
