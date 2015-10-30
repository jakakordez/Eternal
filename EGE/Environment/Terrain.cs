using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using System.Drawing;
using System.ComponentModel;
using EGE.Environment.Paths;

namespace EGE
{
    public class Terrain
    {
        public int Size { get; set; }
        public Vector3 Scale { get; set; }
        public string GroundTextureName { get; set; }
        public string HeightfieldName { get; set; }

        public Road[] Roads { get; set; }

        string FilePath;
        BufferedObject HeightfieldMesh;
        int GroundTexture;

        public Terrain()
        {
            GroundTextureName = "";
            HeightfieldName = "";
            Roads = new Road[0];
        }

        public void Draw()
        {
            foreach (Road road in Roads)
            {
                road.Draw();
            }
        }

        public void Load(World currentWorld, string filePath)
        {
            FilePath = filePath;
            // Open terrain file
            using (ZipArchive archive = ZipFile.Open(FilePath, ZipArchiveMode.Read, Global.Encoding))
            {
                // Load terrain descriptor
                Stream entryStream = archive.GetEntry("TerrainDescriptor.json").Open();
                JsonConvert.PopulateObject(Misc.StreamToString(entryStream), this, Global.SerializerSettings);
                entryStream.Close();

                // Load ground texture
                if (archive.GetEntry("GroundTexture.bmp") != null)
                {
                    entryStream = archive.GetEntry("GroundTexture.bmp").Open();
                    GroundTexture = Misc.LoadTexture(new Bitmap(entryStream), 1);
                    entryStream.Close();
                }

                // Load heightfield data
                if (archive.GetEntry("Heightfield.raw") != null)
                {
                    entryStream = archive.GetEntry("Heightfield.raw").Open();
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
                    //HeightfieldMesh = new BufferedObject(points, indicies, texturecoords);
                    entryStream.Close();
                }
            }
        }

        public void Save(string filePath)
        {
            FilePath = filePath;
            if (File.Exists(FilePath + "bkp")) File.Delete(FilePath + "bkp");
            if (File.Exists(filePath)) File.Move(filePath, filePath + "bkp");
            // Open terrain file
            using (ZipArchive archive = ZipFile.Open(FilePath, ZipArchiveMode.Create, Global.Encoding))
            {
                // Save terrain descriptor
                Stream entryStream = archive.CreateEntry("TerrainDescriptor.json").Open();
                byte[] entryBytes = Global.Encoding.GetBytes(JsonConvert.SerializeObject(this, Global.SerializerSettings));
                entryStream.Write(entryBytes, 0, entryBytes.Length);
                entryStream.Close();

                // Load ground texture
                if (File.Exists(GroundTextureName))
                {
                    entryStream = archive.CreateEntry("GroundTexture.bmp").Open();
                    entryBytes = File.ReadAllBytes(GroundTextureName);
                    entryStream.Write(entryBytes, 0, entryBytes.Length);
                    entryStream.Close();
                }


                // Load heightfield data
                if (File.Exists(HeightfieldName))
                {
                    entryStream = archive.CreateEntry("Heightfield.raw").Open();
                    entryBytes = File.ReadAllBytes(HeightfieldName);
                    entryStream.Write(entryBytes, 0, entryBytes.Length);
                    entryStream.Close();
                }
                
            }
            if (File.Exists(filePath+"bkp")) File.Delete(filePath+"bkp");
        }
    }
}
