using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Drawing;
using EGE.Meshes;

namespace EGE.Environment
{
    public class Heightfield
    {
        public Vector3 Scale { get; set; }

        public Vector3 Offset { get; set; }

        public string HeightfieldName { get; set; }

        public string TextureName { get; set; }

        public int Size { get; set; }

        public Vector2 TextureScale { get;  set; }

        Mesh HeightfieldMesh;

        RigidBody GroundBody;

        float Maximum, Minimum;

        public Heightfield()
        {
            Scale = Vector3.Zero;
            HeightfieldName = "";
            HeightfieldMesh = new Mesh();
            Maximum = float.MinValue;
            Minimum = float.MaxValue;
        }

        public void Load()
        {
            Stream entryStream = new MemoryStream(Tools.ResourceManager.GetResource(HeightfieldName));
            if (entryStream  != null)
            {
                
                // Generate heightfield mesh
                Vector3[] points = new Vector3[Size * Size];
                entryStream.Position = 0;
                byte[] b = new byte[4];
                Vector2[] texturecoords = new Vector2[Size * Size];
                for (int i = 0; i < entryStream.Length; i += 4)
                {
                    entryStream.Read(b, 0, 4);
                    float h = BitConverter.ToSingle(b, 0);
                    points[i / 4] = new Vector3((i / 4) % Size, h, ((i / 4) / Size)) * Scale;
                    texturecoords[i / 4] = new Vector2((i / 4) % Size, ((i / 4) / Size)) * Scale.Xz;
                    texturecoords[i / 4].X /= TextureScale.X;
                    texturecoords[i / 4].Y /= TextureScale.Y;
                    if (h > Maximum) Maximum = h;
                    if (h < Minimum) Minimum = h;
                }
                int[] indicies = new int[(Size - 1) * (Size - 1) * 6];
                int c = 0;
                for (int i = 0; i < points.Length - Size; i++)
                {
                    if ((i % Size) < Size - 1)
                    {
                        indicies[c++] = i;
                        indicies[c++] = i + 1;
                        indicies[c++] = i + 1 + Size;
                        indicies[c++] = i + 1 + Size;
                        indicies[c++] = i + Size;
                        indicies[c++] = i;
                    }
                }
                HeightfieldMesh.Load(points, indicies, TextureName, texturecoords);
                if (!World.StaticView)
                {
                    // Generate rigid body

                    HeightfieldTerrainShape heightfield = new HeightfieldTerrainShape(Size, Size, entryStream, 1, -3000, 3000, 1, PhyScalarType.PhyFloat, false);
                    heightfield.LocalScaling = Scale;
                    World.CreateRigidBody(0, Matrix4.CreateTranslation(new Vector3(Size / 2, 0, Size / 2)), heightfield);
                }

                entryStream.Close();
            }
        }

        public Bitmap GetBitmap()
        {
            Bitmap result = new Bitmap(Size, Size);
            Graphics g = Graphics.FromImage(result);
            g.Clear(Color.Black);
            Stream entryStream = new MemoryStream(Tools.ResourceManager.GetResource(HeightfieldName));
            if (entryStream != null)
            {
                byte[] b = new byte[4];
                for (int i = 0; i < entryStream.Length; i += 4)
                {
                    entryStream.Read(b, 0, 4);
                    int argb = (int)(255*(BitConverter.ToSingle(b, 0) - Minimum)/ (Maximum - Minimum));
                    result.SetPixel((i/4) % Size, (int)((i/4) / Size), GetColor(BitConverter.ToSingle(b, 0)));
                }
            }
            return result;
        }

        private static Color GetColor(float Height)
        {
            int[,] ControlPoints = new int[,]
            {
                { -10, 32, 41, 159 },
                { -5, 66, 129, 63 },
                { 0, 56, 135, 133 },
                { 1, 223, 219, 55 },
                { 2, 105, 201, 78},
                { 500, 61, 130, 40},
            };
            for (int i = 0; i < ControlPoints.GetLength(0)-1; i++)
            {
                if(Height >= ControlPoints[i, 0] && Height < ControlPoints[i+1, 0])
                {
                    float controlPoint = (Height - ControlPoints[i, 0]) / (float)(ControlPoints[i + 1, 0] - ControlPoints[i, 0]);
                    int r = (int)Misc.lerp(ControlPoints[i, 1], ControlPoints[i + 1, 1], controlPoint);
                    int g = (int)Misc.lerp(ControlPoints[i, 2], ControlPoints[i + 1, 2], controlPoint);
                    int b = (int)Misc.lerp(ControlPoints[i, 3], ControlPoints[i + 1, 3], controlPoint);
                    return Color.FromArgb(r, g, b);
                }
            }
            return Color.Black;
        }

        public void Draw()
        {
            Tools.TextureManager.BindTexture(TextureName);
            HeightfieldMesh.Draw();
            
            GL.Begin(BeginMode.Quads);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Color3(Color.Blue);
            GL.Vertex3(new Vector3(0, 0, 0));
            GL.Vertex3(new Vector3(0, 0, Size));
            GL.Vertex3(new Vector3(Size, 0, Size));
            GL.Vertex3(new Vector3(Size, 0, 0));
            GL.End();
        }
    }
}
