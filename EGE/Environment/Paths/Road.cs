using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace EGE.Environment.Paths
{
    public class Road
    {
        public Path RoadPath { get; set; }

        List<Path> Lanes;

        List<Vector3> Points;

        public Road()
        {
            RoadPath = new Path();
            Lanes = new List<Path>();
            Points = new List<Vector3>();
        }

        public void Draw()
        {
            GL.LineWidth(RoadPath.Width);
            GL.Begin(BeginMode.LineStrip);
            /*foreach (Node n in RoadPath.PathNodes)
            {
                GL.Vertex3(n.NodeLocation);
            }*/
            if (Points.Count == 0) Build();
            for (int i = 0; i < Points.Count; i++)
            {
                GL.Vertex3(Points[i]);
            }
            GL.End();
            
        }

        public void Build()
        {
            float Sharpness = 5;
            Vector2[] BezierControlPoints = new Vector2[4];
            float angle = Misc.getAngle(RoadPath.PathNodes[1].NodeLocation.Xz - RoadPath.PathNodes[0].NodeLocation.Xz);
            Vector2 l = Misc.getCartesian(angle) * Sharpness;

            for (int i = 0; i < RoadPath.PathNodes.Length-1; i++)
            {
                BezierControlPoints[0] = RoadPath.PathNodes[i].NodeLocation.Xz;
                BezierControlPoints[1] = BezierControlPoints[0] + l;

                angle = Misc.getAngle(RoadPath.PathNodes[i + 1].NodeLocation.Xz - RoadPath.PathNodes[i].NodeLocation.Xz);
                if(i < RoadPath.PathNodes.Length - 2)
                {
                    float nextAngle = Misc.getAngle(RoadPath.PathNodes[i + 2].NodeLocation.Xz - RoadPath.PathNodes[i+1].NodeLocation.Xz);
                    angle = (angle + nextAngle) / 2;
                }
                l = Misc.getCartesian(angle) * Sharpness;
                BezierControlPoints[2] = RoadPath.PathNodes[i + 1].NodeLocation.Xz - l;
                BezierControlPoints[3] = RoadPath.PathNodes[i + 1].NodeLocation.Xz;
                float segments = (RoadPath.PathNodes[i + 1].NodeLocation.Xz - RoadPath.PathNodes[i].NodeLocation.Xz).Length;
                Vector2[] bezierPoints = Misc.GetBezierApproximation(BezierControlPoints, (int)segments);
                Points.AddRange(bezierPoints.Select(p => new Vector3(p.X, 0, p.Y)));
            }
        }
    }
}
