using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGE.Environment;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace EGE.Environment
{
    public class Field :IBuildable
    {
        public MeshReference[] Meshes { get; set; }
        public Node[] Polygon { get; set; }
        List<FieldNode> points;
        Random r = new Random();

        public Field()
        {
            Meshes = new MeshReference[0];
            Polygon = new Node[0];
        }

        public void Build()
        {
            points = new List<FieldNode>();
            if(Polygon.Length >= 3)
            {
                Vector3 l = Polygon[0].Location;
                float Top = l.Z, Left = l.X, Right = l.X, Bottom = l.Z;
                for (int i = 1; i < Polygon.Length; i++)
                {
                    if (Polygon[i].Location.Z > Top) Top = Polygon[i].Location.Z;
                    if (Polygon[i].Location.Z < Bottom) Bottom = Polygon[i].Location.Z;
                    if (Polygon[i].Location.X > Right) Right = Polygon[i].Location.X;
                    if (Polygon[i].Location.X < Left) Left = Polygon[i].Location.X;
                }
                Vector2[] poly = Polygon.Select(p => p.Location.Xz).ToArray();
                for (float i = Left; i < Right; i+=6)
                {
                    for(float j = Bottom; j < Top; j+=6)
                    {
                        if (Misc.PointInPolygon(poly, new Vector2(i, j)))
                        {
                            FieldNode n = new FieldNode();
                            n.Location = new Vector3(i, 10, j);
                            n.MeshIndex = r.Next(Meshes.Length - 1);
                            points.Add(n);
                        }
                    }
                }
            }
        }

        public void Draw(Vector3 eye)
        {
            foreach (var p in points)
            {
                if((eye-p.Location).LengthSquared < 900)  Meshes[p.MeshIndex].Draw(p.Location);
            }
        }
    }
}
