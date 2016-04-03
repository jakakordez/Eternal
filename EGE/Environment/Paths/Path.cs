using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace EGE.Environment.Paths
{
    public class Path
    {
        public PathEndpoint FirstEndpoint { get; set; }
        public PathEndpoint LastEndpoint { get; set; }
        public Node[] Points { get; set; }

        public Path()
        {
            Points = new Node[0];
            FirstEndpoint = new PathEndpoint();
            LastEndpoint = new PathEndpoint();
        }

        public static Vector3[] CreateCurve(Vector2[] roadLine, int Segments, float Offset, float Height, bool Invert)
        {
            Vector2[] roadCurve = Misc.GetBezierApproximation(roadLine, Segments);
            Vector3[] PathNodes = new Vector3[Segments + 1];
            float kot = 0;
            for (int i = 0; i < Segments + 1; i++)
            {
                if (i > 1 && i < roadLine.Length - 2)
                {
                    float kot1 = (float)Math.Atan((roadCurve[i - 1].Y - roadCurve[i].Y) / (roadCurve[i - 1].X - roadCurve[i].X)) - MathHelper.PiOver2;
                    float kot2 = (float)Math.Atan((roadCurve[i + 1].Y - roadCurve[i].Y) / (roadCurve[i + 1].X - roadCurve[i].X)) - MathHelper.PiOver2;
                    kot = (kot1 + kot2) / 2;
                }
                else if (i < 2) kot = (float)Math.Atan((roadCurve[i + 1].Y - roadCurve[i].Y) / (roadCurve[i + 1].X - roadCurve[i].X)) - MathHelper.PiOver2;
                else kot = (float)Math.Atan((roadCurve[i - 1].Y - roadCurve[i].Y) / (roadCurve[i - 1].X - roadCurve[i].X)) - MathHelper.PiOver2;
                float y = Offset * (float)Math.Sin(kot);
                float x = Offset * (float)Math.Cos(kot);
                PathNodes[(Invert) ? Segments - i : i] = new Vector3(roadCurve[i].X + x, Height, roadCurve[i].Y + y);
            }
            return PathNodes;
        }

        public static Vector3[] CreateCurve(Vector3[] roadLine, int Segments, float Offset, bool Invert)
        {
            Vector2[] roadCurve = Misc.GetBezierApproximation(roadLine.Select(p=>p.Xz).ToArray(), Segments);
            Vector2[] heightLine = new Vector2[roadLine.Length];
            Vector2 prevPoint = roadLine[0].Xz;
            for (int i = 0; i < heightLine.Length; i++)
            {
                heightLine[i] = new Vector2((roadLine[i].Xz - prevPoint).Length, roadLine[i].Y);
                prevPoint = roadLine[i].Xz;
            }
            Vector2[] heightCurve = Misc.GetBezierApproximation(heightLine, Segments);
            Vector3[] PathNodes = new Vector3[Segments + 1];
            for (int i = 0; i < Segments + 1; i++)
            {
                Vector2 dir = new Vector2();
                if (i== 0) dir = (roadCurve[i] - roadCurve[i+1]);
                else if (i == Segments) dir = (roadCurve[i - 1] - roadCurve[i]);
                else dir = (roadCurve[i - 1] - roadCurve[i]) + (roadCurve[i] - roadCurve[i + 1]);
                dir.Normalize();
                dir *= Offset;
                PathNodes[(Invert) ? Segments - i : i] = new Vector3(roadCurve[i].X + dir.Y, heightCurve[i].Y, roadCurve[i].Y - dir.X);
            }
            return PathNodes;
        }
    }
}
