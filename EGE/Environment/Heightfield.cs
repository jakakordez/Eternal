using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using OpenTK;
using System.IO;

namespace EGE.Environment
{
    public class Heightfield
    {
        public Vector3 Scale { get; set; }

        public Vector3 Offset { get; set; }

        public string GroundTextureName { get; set; }

        public string HeightfieldName { get; set; }

        public int Size { get; set; }

        BufferedObject HeightfieldMesh;

        public Heightfield()
        {
            Scale = Vector3.Zero;
            GroundTextureName = "";
            HeightfieldName = "";
            HeightfieldMesh = new BufferedObject();
        }

        public void Load()
        {
            Stream entryStream = new MemoryStream(Tools.ResourceManager.GetResource(HeightfieldName));
            if (entryStream  != null)
            {
                HeightfieldTerrainShape heightfield = new HeightfieldTerrainShape(Size, Size, entryStream, 1, -3000, 3000, 1, PhyScalarType.PhyFloat, false);
                heightfield.LocalScaling = Scale;
                //currentWorld.CreateRigidBody(0, Matrix4.CreateTranslation(Vector3.Zero), heightfield); //TODO: uncomment

                // Generate heightfield mesh
                Vector3[] points = new Vector3[Size * Size];
                entryStream.Position = 0;
                byte[] b = new byte[4];
                Vector2[] texturecoords = new Vector2[Size * Size];
                for (int i = 0; i < entryStream.Length; i += 4)
                {
                    entryStream.Read(b, 0, 4);
                    points[i / 4] = new Vector3((i / 4) % Size, BitConverter.ToSingle(b, 0), ((i / 4) / Size)) * Scale;
                    texturecoords[i / 4] = new Vector2((i / 4) % Size, ((i / 4) / Size)) * Scale.Xz;
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
                HeightfieldMesh.Load(points, indicies, texturecoords);
                entryStream.Close();
            }
        }

        public void Draw()
        {
            Tools.TextureManager.BindTexture(GroundTextureName);
            HeightfieldMesh.Draw();
        }
    }
}
