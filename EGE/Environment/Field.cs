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
    public class Field 
    {
        public MeshReference[] Meshes { get; set; }
        public MeshReference LowPolyMesh { get; set; }
        public Node[] Polygon { get; set; }
        List<FieldNode> points;

        public Field()
        {
            Meshes = new MeshReference[0];
            Polygon = new Node[0];
            LowPolyMesh = new MeshReference();
        }

        public void Build(Heightfield currentHeightfield)
        {
            var locations = new List<Vector3>();
            if (Polygon.Length >= 3)
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
                
                for (int i = (int)Left/20; i < Right/20; i++)
                {
                    for (int j = (int)Bottom/20; j < Top/20; j++)
                    {
                        var v = positions[Global.RNG.Next(positions.Length)];
                        for (int k = 0; k < v.Count; k++)
                        {
                            var vector = v[k] + (new Vector3(i * 20, 0, j * 20));
                            if (Misc.PointInPolygon(poly, vector.Xz)) locations.Add(vector);
                        }
                    }
                }
            }
            currentHeightfield.PopulateHeights(ref locations);
            points = locations.Select(l => new FieldNode() { Location = l, MeshIndex = Global.RNG.Next(Meshes.Length - 1) }).ToList();
        }

        public void Draw(Vector3 eye)
        {
            foreach (var p in points)
            {
                float l = (eye - p.Location).LengthSquared;
                if (l < 600) Meshes[p.MeshIndex].Draw(p.Location);
                else if(l < 250000) LowPolyMesh.Draw(p.Location);
            }
        }

        private static List<Vector3>[] positions =
            {
                new List<Vector3>(){
                    new Vector3(6.19f, 0, 17.81f),
                    new Vector3(16.99f, 0, 16.67f),
                    new Vector3(1.62f, 0, 12.45f),
                    new Vector3(8.04f, 0, 11.66f),
                    new Vector3(14.10f, 0, 11.13f),
                    new Vector3(4.08f, 0, 6.74f),
                    new Vector3(9.62f, 0, 2.88f),
                    new Vector3(17.08f, 0, 3.32f) },
                new List<Vector3>(){
                    new Vector3(2.97f, 0, 16.93f),
                    new Vector3(10.43f, 0, 15.79f),
                    new Vector3(16.75f, 0, 18.51f),
                    new Vector3(02.70f, 0, 9.11f),
                    new Vector3(09.64f, 0, 9.20f),
                    new Vector3(18.25f, 0, 11.31f),
                    new Vector3(04.11f, 0, 2.35f),
                    new Vector3(12.80f, 0, 2.88f),
                    new Vector3(18.42f, 0, 4.9f)},
                new List<Vector3>(){
                    new Vector3(01.67f, 0, 18.51f),
                    new Vector3(11.05f, 0, 18.33f),
                    new Vector3(05.89f, 0, 13.59f),
                    new Vector3(06.33f, 0, 7.71f),
                    new Vector3(12.56f, 0, 8.41f),
                    new Vector3(16.51f, 0, 11.31f),
                    new Vector3(02.46f, 0, 2.09f),
                    new Vector3(15.81f, 0, 3.67f), },
                new List<Vector3>(){
                    new Vector3(01.52f, 0, 14.47f),
                    new Vector3(07.84f, 0, 17.19f),
                    new Vector3(14.6f, 0, 18.86f),
                    new Vector3(05.38f, 0, 9.29f),
                    new Vector3(15.4f, 0, 13.86f),
                    new Vector3(02.75f, 0, 1.65f),
                    new Vector3(11.79f, 0, 1.91f),
                    new Vector3(18.12f, 0, 5.34f), }
            };
    }
}
