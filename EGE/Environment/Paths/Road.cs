using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using EGE.Meshes;
using BulletSharp;

namespace EGE.Environment.Paths
{
    public class Road : Path
    {
        public string TextureName { get; set; }

        public float RoadWidth { get; set; }

        public Lane[] OutgoingLanes { get; set; }
        public Lane[] IngoingLanes { get; set; }

        Mesh RoadMesh;
        RigidBody RoadSurface;

        public Road():base()
        {
            OutgoingLanes = new Lane[] { new Lane(1.5f) };
            IngoingLanes = new Lane[] { new Lane(1.5f) };
            RoadMesh = new Mesh();
            TextureName = "";
            RoadWidth = 6;
        }

        public void Draw()
        {
            Resources.BindTexture(TextureName);
            RoadMesh.Draw();
        }

        public void Build(ObjectManager objects)
        {
            List<Vector3> BezierCurve = new List<Vector3>();
            List<Node> PointsForPath = new List<Node>();
            Vector3[] BezierControlPoints = new Vector3[4];

            Vector2 dir = new Vector2();
            Node first = FirstEndpoint.getPosition(objects);
            if (first != null)
            {
                dir = Misc.getCartesian(first.Rotation.Y);
                first.Rotation = new Vector3(dir.X, 0, dir.Y);
                PointsForPath.Add(first); // First endpoint exists, so add it to the list
            }
            PointsForPath.AddRange(Points);
            Node last = LastEndpoint.getPosition(objects);
            if (last != null)
            {
                // Set rotation for the last endpoint
                dir = Misc.getCartesian(last.Rotation.Y);
                last.Rotation = new Vector3(-dir.X, 0, -dir.Y);
                PointsForPath.Add(last); // Last endpoint exists, so add it to the list
            }

            for (int i = 0; i < PointsForPath.Count; i++)
            {
                if((i > 0 || first == null)&&(i < PointsForPath.Count-1 || last == null))
                {
                    if (i == 0) dir = PointsForPath[i + 1].Location.Xz - PointsForPath[i].Location.Xz;
                    else if(i == PointsForPath.Count - 1) dir = PointsForPath[i].Location.Xz - PointsForPath[i - 1].Location.Xz;
                    else
                    {
                        dir = (PointsForPath[i].Location.Xz - PointsForPath[i - 1].Location.Xz);
                        dir += (PointsForPath[i + 1].Location.Xz - PointsForPath[i].Location.Xz);
                    }
                    dir.Normalize();
                    PointsForPath[i].Rotation = new Vector3(dir.X, 0, dir.Y);
                }
            }
            for (int i = 0; i < PointsForPath.Count - 1; i++)
            {
                float segments = (PointsForPath[i + 1].Location.Xz - PointsForPath[i].Location.Xz).Length * 0.25f;
                BezierControlPoints[0] = PointsForPath[i].Location;
                BezierControlPoints[1] = PointsForPath[i].Location + (PointsForPath[i].Rotation*segments);
                BezierControlPoints[2] = PointsForPath[i + 1].Location - (PointsForPath[i + 1].Rotation*segments);
                BezierControlPoints[3] = PointsForPath[i + 1].Location;
                
                Vector3[] roadEdgeRight = CreateCurve(BezierControlPoints, (int)segments, RoadWidth/2, false);
                Vector3[] roadEdgeLeft = CreateCurve(BezierControlPoints, (int)segments, -RoadWidth/2, false);
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
            RoadMesh.GenerateCollisionShape = !World.StaticView;
            RoadMesh.Load(BezierCurve.ToArray(), Indices, TextureName, TextureCoordinates);

            if (!World.StaticView)
            {
                RoadSurface = World.CreateRigidBody(0, Matrix4.Identity, RoadMesh.CollisionShape);
            }
        }

        public static string GeogebraPoints(Vector3[] ps)
        {
            string result = "{";
            for (int i = 0; i < ps.Length; i++)
            {
                result += "(" + ps[i].X.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + ps[i].Z.ToString(System.Globalization.CultureInfo.InvariantCulture) + ")";
                if (i < ps.Length - 1) result += ",";
            }
            return result + "}";
        }
    }
}
