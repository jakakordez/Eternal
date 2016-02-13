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
    public class Road : Path, Buildable
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

        public void Build()
        {
            List<Vector3> BezierCurve = new List<Vector3>();
            float Sharpness = 2;

            Vector3[] BezierControlPoints = new Vector3[4];
            float angle = (Misc.getAngle(Points[1].Ref.Location.Xz - Points[0].Ref.Location.Xz));
            Vector2 l = Misc.getCartesian(angle) * Sharpness;

            for (int i = 0; i < Points.Length - 1; i++)
            {
                BezierControlPoints[0] = Points[i].Ref.Location;
                BezierControlPoints[1] = BezierControlPoints[0] + new Vector3(l.X, 0, l.Y);

                angle = (Misc.getAngle(Points[i + 1].Ref.Location.Xz - Points[i].Ref.Location.Xz));
                if (i < Points.Length - 2)
                {
                    float nextAngle = (Misc.getAngle(Points[i + 2].Ref.Location.Xz - Points[i + 1].Ref.Location.Xz));
                    if (angle > MathHelper.Pi && nextAngle < MathHelper.PiOver2) angle -= MathHelper.TwoPi;
                    angle = ((angle + nextAngle) / 2);

                }

                float segments = (Points[i + 1].Ref.Location.Xz - Points[i].Ref.Location.Xz).Length * 0.5f;
                l = Misc.getCartesian(angle) * (segments / 2);
                BezierControlPoints[2] = Points[i + 1].Ref.Location - new Vector3(l.X, 0, l.Y);
                BezierControlPoints[3] = Points[i + 1].Ref.Location;

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
            RoadMesh.GenerateCollisionShape = true;
            RoadMesh.Load(BezierCurve.ToArray(), Indices, TextureName, TextureCoordinates);

            if (!World.StaticView)
            {
                RoadSurface = World.CreateRigidBody(0, Matrix4.Identity, RoadMesh.CollisionShape);
            }
        }
    }
}
