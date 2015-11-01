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
        public PathNode[] PathNodes { get; set; }
        public float Width { get; set; }

        public Path()
        {
            PathNodes = new PathNode[0];
        }

        public Path(Vector2[] roadLine, int Segments, float Offset, float Height, bool Invert)
        {
            Vector2[] roadCurve = Misc.GetBezierApproximation(roadLine, Segments);
            PathNodes = new PathNode[Segments + 1];
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
                float y = (Offset + Offset) * (float)Math.Sin(kot);
                float x = (Offset + Offset) * (float)Math.Cos(kot);
                PathNodes[(Invert) ? Segments - i : i] = new PathNode(new Vector3(roadCurve[i].X + x, Height, roadCurve[i].Y + y));
            }
        }
    }
}
