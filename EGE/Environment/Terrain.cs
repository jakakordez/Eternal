using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using System.Drawing;
using System.ComponentModel;
using EGE.Environment.Paths;
using EGE.Environment;

namespace EGE
{
    public class Terrain
    {
        public Road[] Roads { get; set; }

        public Heightfield TerrainHeightfield { get; set; }

        string FilePath;

        public Terrain()
        {
            Roads = new Road[0];
            TerrainHeightfield = new Heightfield();
        }

        public void Draw()
        {
            foreach (Road road in Roads)
            {
                road.Draw();
            }
            TerrainHeightfield.Draw();
        }

        public void Load(string filePath)
        {
            FilePath = filePath;
            if (File.Exists(FilePath))
            {
                // Open terrain file
                using (ZipArchive archive = ZipFile.Open(FilePath, ZipArchiveMode.Read, Global.Encoding))
                {
                    // Load terrain descriptor
                    Stream entryStream = archive.GetEntry("TerrainDescriptor.json").Open();
                    JsonConvert.PopulateObject(Misc.StreamToString(entryStream), this, Global.SerializerSettings);
                    entryStream.Close();

                    // Load ground texture
                    /*if (archive.GetEntry("GroundTexture.bmp") != null)
                    {
                        entryStream = archive.GetEntry("GroundTexture.bmp").Open();
                        GroundTexture = Misc.LoadTexture(new Bitmap(entryStream), 1);
                        entryStream.Close();
                    }*/

                    // Load heightfield data
                    /*if (archive.GetEntry("Heightfield.raw") != null)
                    {
                        TerrainHeightfield.Load(archive.GetEntry("Heightfield.raw").Open());
                    }*/
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
               /* if (File.Exists(GroundTextureName))
                {
                    entryStream = archive.CreateEntry("GroundTexture.bmp").Open();
                    entryBytes = File.ReadAllBytes(GroundTextureName);
                    entryStream.Write(entryBytes, 0, entryBytes.Length);
                    entryStream.Close();
                }*/


                // Load heightfield data
                /*if (File.Exists(HeightfieldName))
                {
                    entryStream = archive.CreateEntry("Heightfield.raw").Open();
                    entryBytes = File.ReadAllBytes(HeightfieldName);
                    entryStream.Write(entryBytes, 0, entryBytes.Length);
                    entryStream.Close();
                }*/
                
            }
            if (File.Exists(filePath+"bkp")) File.Delete(filePath+"bkp");
        }
    }
}
