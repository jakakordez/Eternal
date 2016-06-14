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
using EGE.Vehicles;
using EGE.Meshes;

namespace EGE
{
    public class Map
    {
        public Road[] Roads { get; set; }

        public Field[] Forests { get; set; }

        public ObjectManager ObjectCollection { get; set; }

        public VehicleManager VehicleCollection { get; set; }

        public Heightfield TerrainHeightfield { get; set; }

        string FilePath;

        public Map()
        {
            Roads = new Road[0];
            Forests = new Field[0];
            TerrainHeightfield = new Heightfield();
            ObjectCollection = new ObjectManager();
            VehicleCollection = new VehicleManager();
        }

        public void Draw(Vector3 eye)
        {
            Matrix4 trans = World.WorldMatrix;
            foreach (var r in Roads) r.Draw();
            foreach (var f in Forests) f.Draw();
        
            TerrainHeightfield.Draw();
            ObjectCollection.Draw();
            VehicleCollection.Draw(eye);
        }

        public void Load(string filePath)
        {
            FilePath = filePath;
            if (File.Exists(FilePath))
            {
                // Open map file
                using (ZipArchive archive = ZipFile.Open(FilePath, ZipArchiveMode.Read, Global.Encoding))
                {
                    // Load map descriptor
                    Stream entryStream = archive.GetEntry("MapDescriptor.json").Open();
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
            // Open map file
            using (ZipArchive archive = ZipFile.Open(FilePath, ZipArchiveMode.Create, Global.Encoding))
            {
                // Save map descriptor
                Stream entryStream = archive.CreateEntry("MapDescriptor.json").Open();
                byte[] entryBytes = Global.Encoding.GetBytes(JsonConvert.SerializeObject(this, Global.SerializerSettings));
                entryStream.Write(entryBytes, 0, entryBytes.Length);
                entryStream.Close();
            }
            if (File.Exists(filePath + "bkp")) File.Delete(filePath + "bkp");
        }

        public override string ToString()
        {
            return "Map";
        }
    }
}
