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

        private Vector3 rootPointWorld;
        private float scale;
        private Vector3 rootPointPhoto;

        public void Initialize(List<Vector3> facialFeatures)
        {
            Points.Clear();

            foreach (var point in facialFeatures)
            {
                var p = point - rootPointPhoto;
                p.X *= scale;
                p.Y *= -scale;
                p += rootPointWorld;
                Points.Add(p.Xy);
            }
        }

        public void Initialize( HeadPoints headPoints, List<Vector3> facialFeatures)
        {
            if (facialFeatures.Count < headPoints.Points.Count)
                return;

            if (headPoints.Points.Count <= RootIndex)
                return;

            if (headPoints.Points.Count <= HelperIndex)
                return;

            rootPointWorld = headPoints.GetWorldPoint(RootIndex);
            var helperPointWorld = headPoints.GetWorldPoint(HelperIndex);

            rootPointPhoto = facialFeatures[RootIndex];
            var helperPointPhoto = facialFeatures[HelperIndex];

            float distWorld = rootPointWorld.X - helperPointWorld.X;
            float distPhoto = rootPointPhoto.X - helperPointPhoto.X;

            scale = distWorld / distPhoto;

            Initialize(facialFeatures);
        }
    }
}
