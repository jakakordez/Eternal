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
using EGE.Meshes;

namespace EGE
{
    public class Terrain
    {
        public Road[] Roads { get; set; }

        public Model[] StaticModels { get; set; }

        public Heightfield TerrainHeightfield { get; set; }

        string FilePath;

        public Terrain()
        {
            Roads = new Road[0];
            StaticModels = new Model[0];
            TerrainHeightfield = new Heightfield();
        }

        public void Draw()
        {
            Matrix4 trans = World.WorldMatrix;
            foreach (Road road in Roads)
            {
                road.Draw();
            }
            
            foreach (var m in StaticModels)
            {
                m.Draw();
            }
            trans = World.WorldMatrix;
            GL.LoadMatrix(ref trans);
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
            }
            if (File.Exists(filePath+"bkp")) File.Delete(filePath+"bkp");
        }
    }
}
