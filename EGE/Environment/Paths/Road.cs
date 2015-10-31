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

        BufferedObject RoadMesh;
        Vector3[] Vertices;

        public Road()
        {
            RoadPath = new Path();
            Lanes = new List<Path>();
            Points = new List<Vector3>();
            RoadMesh = new BufferedObject();
            Vertices = new Vector3[0];
        }

        public void Draw()
        {
            if (Settings.CurrentDrawingMode == Settings.DrawingModes.Debug)
            {
                /*GL.LineWidth(RoadPath.Width);
                GL.Begin(PrimitiveType.LineStrip);
                /*for (int i = 0; i < Points.Count; i++)
                {
                    GL.Vertex3(Points[i]);
                }*/
                //GL.End();

                GL.PointSize(5);
                GL.Begin(PrimitiveType.Points);
                /*for (int i = 0; i < RoadPath.PathNodes.Length; i++)
                {
                    GL.Vertex3(RoadPath.PathNodes[i].NodeLocation);
                }*/
                for (int i = 0; i < Vertices.Length; i++)
                {
                    GL.Vertex3(Vertices[i]);
                }
                GL.End();
            }
            else RoadMesh.Draw();
        }

        public void Build()
        {
            //try
            {
                Points.Clear();
                float Sharpness = 2;
                List<Vector2> BezierCurve = new List<Vector2>();
                Vector2[] BezierControlPoints = new Vector2[4];
                float angle = Misc.halfNormalizeAngle(Misc.getAngle(RoadPath.PathNodes[1].NodeLocation.Xz - RoadPath.PathNodes[0].NodeLocation.Xz));
                Vector2 l = Misc.getCartesian(angle) * Sharpness;

                for (int i = 0; i < RoadPath.PathNodes.Length - 1; i++)
                {
                    BezierControlPoints[0] = RoadPath.PathNodes[i].NodeLocation.Xz;
                    BezierControlPoints[1] = BezierControlPoints[0] + l;

                    angle = Misc.halfNormalizeAngle(Misc.getAngle(RoadPath.PathNodes[i + 1].NodeLocation.Xz - RoadPath.PathNodes[i].NodeLocation.Xz));
                    if (i < RoadPath.PathNodes.Length - 2)
                    {
                        float nextAngle = Misc.halfNormalizeAngle(Misc.getAngle(RoadPath.PathNodes[i + 2].NodeLocation.Xz - RoadPath.PathNodes[i + 1].NodeLocation.Xz));

                        angle = (Misc.halfNormalizeAngle(angle + nextAngle) / 2);
                    }
                    float segments = (RoadPath.PathNodes[i + 1].NodeLocation.Xz - RoadPath.PathNodes[i].NodeLocation.Xz).Length * 10;
                    l = Misc.getCartesian(angle) * (segments / 20);
                    BezierControlPoints[2] = RoadPath.PathNodes[i + 1].NodeLocation.Xz - l;
                    BezierControlPoints[3] = RoadPath.PathNodes[i + 1].NodeLocation.Xz;

                    Vector2[] bezierPoints = Misc.GetBezierApproximation(BezierControlPoints, (int)segments);
                    BezierCurve.AddRange(bezierPoints);
                }
                Vertices = new Vector3[BezierCurve.Count * 2];
                int[] Indices = new int[(BezierCurve.Count - 1) * 6];
                for (int i = 0; i < BezierCurve.Count; i++)
                {

                    if (i < BezierCurve.Count - 1)
                    {
                        l = Misc.getCartesian(Misc.getAngle(BezierCurve[i + 1] - BezierCurve[i]) + MathHelper.Pi);

                        Indices[(i * 6) + 0] = (i * 6) + 0;
                        Indices[(i * 6) + 1] = (i * 6) + 1;
                        Indices[(i * 6) + 2] = (i * 6) + 2;
                        Indices[(i * 6) + 3] = (i * 6) + 2;
                        Indices[(i * 6) + 4] = (i * 6) + 0;
                        Indices[(i * 6) + 5] = (i * 6) + 3;
                    }
                    else l = Misc.getCartesian(Misc.getAngle(BezierCurve[i] - BezierCurve[i - 1]) + MathHelper.Pi);
                    Vector2 v = BezierCurve[i] + (l*2);
                    Vertices[i * 2] = new Vector3(v.X, 0, v.Y);
                    v = BezierCurve[i] - (l*2);
                    Vertices[(i * 2) + 1] = new Vector3(v.X, 0, v.Y);
                }
                //RoadMesh.Load(Vertices, Indices, new Vector2[BezierCurve.Count * 6]);
            }
            //catch { }
        }
    }
}
