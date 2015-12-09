using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using EGE.Meshes;

namespace EGE.Environment.Paths
{
    public class Road : Buildable
    {
        public NodeReference[] RoadPath{ get; set; }

        public string TextureName { get; set; }

        public float RoadWidth { get; set; }

        List<ulong[]> Lanes;

        Mesh RoadMesh;

        public Road()
        {
            RoadPath = new NodeReference[0];
            Lanes = new List<ulong[]>();
            RoadMesh = new Mesh();
            TextureName = "";
        }

        public void Draw()
        {
            if (Settings.CurrentDrawingMode == Settings.DrawingModes.Debug)
            {
                GL.Begin(PrimitiveType.LineStrip);
                for (int i = 0; i < RoadPath.Length; i++)
                {
                    //GL.Vertex3(RoadPath[i]);
                }
                GL.End();
            }
            else
            {
                Tools.TextureManager.BindTexture(TextureName);
                RoadMesh.Draw();
            }
        }

        public void Build()
        {
            List<Vector3> BezierCurve = new List<Vector3>();
            float Sharpness = 2;

            Vector3[] BezierControlPoints = new Vector3[4];
            float angle = (Misc.getAngle(RoadPath[1].Ref.Location.Xz - RoadPath[0].Ref.Location.Xz));
            Vector2 l = Misc.getCartesian(angle) * Sharpness;

            for (int i = 0; i < RoadPath.Length - 1; i++)
            {
                BezierControlPoints[0] = RoadPath[i].Ref.Location;
                BezierControlPoints[1] = BezierControlPoints[0] + new Vector3(l.X, 0, l.Y);

                angle = (Misc.getAngle(RoadPath[i + 1].Ref.Location.Xz - RoadPath[i].Ref.Location.Xz));
                if (i < RoadPath.Length - 2)
                {
                    float nextAngle = (Misc.getAngle(RoadPath[i + 2].Ref.Location.Xz - RoadPath[i + 1].Ref.Location.Xz));

                    angle = ((angle + nextAngle) / 2);
                }
                float segments = (RoadPath[i + 1].Ref.Location.Xz - RoadPath[i].Ref.Location.Xz).Length * 1;
                l = Misc.getCartesian(angle) * (segments / 2);
                BezierControlPoints[2] = RoadPath[i + 1].Ref.Location - new Vector3(l.X, 0, l.Y);
                BezierControlPoints[3] = RoadPath[i + 1].Ref.Location;

                Vector3[] roadEdgeRight = Curve.CreateCurve(BezierControlPoints, (int)segments, RoadWidth/2, false);
                Vector3[] roadEdgeLeft = Curve.CreateCurve(BezierControlPoints, (int)segments, -RoadWidth/2, false);
                for (int j = 0; j < roadEdgeLeft.Length; j++)
                {
                    BezierCurve.Add(roadEdgeLeft[j]);
                    BezierCurve.Add(roadEdgeRight[j]);
                }
            }

            Vector3[] Vertices = new Vector3[BezierCurve.Count * 2];
            int[] Indices = new int[(BezierCurve.Count) * 3];
            Vector2[] TextureCoordinates = new Vector2[BezierCurve.Count * 2];
            for (int i = 0; i < (BezierCurve.Count/2)-1; i++)
            {
                Indices[(i * 6) + 0] = (i * 2) + 0;
                Indices[(i * 6) + 1] = (i * 2) + 1;
                Indices[(i * 6) + 2] = (i * 2) + 2;
                Indices[(i * 6) + 3] = (i * 2) + 2;
                Indices[(i * 6) + 4] = (i * 2) + 1;
                Indices[(i * 6) + 5] = (i * 2) + 3;

                Vertices[(i * 2) + 0] = BezierCurve[(i * 2) + 0];
                Vertices[(i * 2) + 1] = BezierCurve[(i * 2) + 1];
                Vertices[(i * 2) + 2] = BezierCurve[(i * 2) + 2];
                Vertices[(i * 2) + 3] = BezierCurve[(i * 2) + 3];

                TextureCoordinates[(i * 4) + 0] = new Vector2(1, 0);
                TextureCoordinates[(i * 4) + 1] = new Vector2(0, 0);
                TextureCoordinates[(i * 4) + 2] = new Vector2(1, 0.2f);
                TextureCoordinates[(i * 4) + 3] = new Vector2(0, 0.2f);
            }
            RoadMesh.Load(BezierCurve.ToArray(), Indices, TextureName, TextureCoordinates);
        }
    }
}
