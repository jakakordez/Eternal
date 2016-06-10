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
            Scale = Vector3.One;
            HeightfieldName = "";
            HeightfieldMesh = new Mesh();
            Maximum = float.MinValue;
            Minimum = float.MaxValue;
        }

        public void Load()
        {
            
            Stream entryStream = new MemoryStream(Resources.GetFile(HeightfieldName));
            if (entryStream  != null)
            {
                
                // Generate heightfield mesh
                Vector3[] points = new Vector3[Size * Size];
                entryStream.Position = 0;
                byte[] b = new byte[4];
                Vector2[] texturecoords = new Vector2[Size * Size];
                for (long i = 0; i < entryStream.Length; i += 4)
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
                if (!Graphics.StaticView)
                {
                    // Generate rigid body
                    entryStream.Position = 0;
                    float Extreme = (Math.Abs(Minimum) > Math.Abs(Maximum)) ? Math.Abs(Minimum) : Maximum;
                    HeightfieldTerrainShape heightfield = new HeightfieldTerrainShape(Size, Size, entryStream, 1, -Extreme, Extreme, 1, PhyScalarType.PhyFloat, true);
                    heightfield.LocalScaling = Scale;
                    GroundBody = World.CreateRigidBody(0, Matrix4.CreateTranslation(new Vector3(Size / 2, 0, Size / 2)), heightfield);
                    GroundBody.CollisionFlags |= CollisionFlags.DisableVisualizeObject;
                }

                entryStream.Close();
            }
        }

        public void PopulateHeights(ref List<Vector3> pos)
        {
            Stream entryStream = new MemoryStream(Resources.GetFile(HeightfieldName));
            if (entryStream != null)
            {
                for (int i = 0; i < pos.Count; i++)
                {
                    if (pos[i].X < 0 || pos[i].Z > Size || pos[i].Z < 0 || pos[i].X > Size) continue;

                    // we'll use integer division to figure out where in the "heights" array
                    // positionOnHeightmap is. Remember that integer division always rounds
                    // down, so that the result of these divisions is the indices of the "upper
                    // left" of the 4 corners of that cell.
                    int left, top;
                    left = (int)pos[i].X / (int)Scale.X;
                    top = (int)pos[i].Z / (int)Scale.Z;

                    // next, we'll use modulus to find out how far away we are from the upper
                    // left corner of the cell. Mod will give us a value from 0 to terrainScale,
                    // which we then divide by terrainScale to normalize 0 to 1.
                    float xNormalized = (pos[i].X % Scale.X) / Scale.X;
                    float zNormalized = (pos[i].Z % Scale.Z) / Scale.Z;

                    // Now that we've calculated the indices of the corners of our cell, and
                    // where we are in that cell, we'll use bilinear interpolation to calculuate
                    // our height. This process is best explained with a diagram, so please see
                    // the accompanying doc for more information.
                    // First, calculate the heights on the bottom and top edge of our cell by
                    // interpolating from the left and right sides.
                    
                    float topHeight = Misc.lerp(
                        GetPoint(ref entryStream, left, top),//heightmap[left, top],
                        GetPoint(ref entryStream, left+1, top),//heightmap[left + 1, top],
                        xNormalized);

                    float bottomHeight = Misc.lerp(
                        GetPoint(ref entryStream, left, top+1),//heightmap[left, top + 1],
                        GetPoint(ref entryStream, left+1, top+1),//heightmap[left + 1, top + 1],
                        xNormalized);

                    // next, interpolate between those two values to calculate the height at our
                    // position.
                    pos[i] = new Vector3(pos[i].X, Misc.lerp(topHeight, bottomHeight, zNormalized), pos[i].Z);
                }
            }
        }

        float GetPoint(ref Stream entryStream, int i, int j)
        {
            byte[] b = new byte[4];
            entryStream.Seek((i*4) + (j * 4*Size), SeekOrigin.Begin);
            entryStream.Read(b, 0, 4);
            return BitConverter.ToSingle(b, 0);
        }
        
        public Bitmap GetBitmap()
        {
            Bitmap result = new Bitmap(Size, Size);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(result);
            g.Clear(Color.Black);
            Stream entryStream = new MemoryStream(Resources.GetFile(HeightfieldName));
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
            Matrix4 trans = World.WorldMatrix;
            GL.LoadMatrix(ref trans);

            Resources.BindTexture(TextureName);
            HeightfieldMesh.Draw();

            GL.Begin(BeginMode.Quads);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Color4(Color.FromArgb(200, Color.Navy));
            GL.Vertex3(new Vector3(-Size, 0, -Size));
            GL.Vertex3(new Vector3(-Size, 0, 2*Size));
            GL.Vertex3(new Vector3(2*Size, 0, 2*Size));
            GL.Vertex3(new Vector3(2*Size, 0, -Size));
            GL.End();
        }
    }
}
