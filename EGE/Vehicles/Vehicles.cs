using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;

namespace EGE.Vehicles
{
    public class Vehicles
    {
        public static Dictionary<string, Vehicle> VehicleCollection;
        static string ArchivePath;

        public static void LoadVehicles(string FilePath)
        {
            VehicleCollection = new Dictionary<string, Vehicle>();
            ArchivePath = FilePath + "\\Vehicles.ege";
            Misc.CheckArchive(ArchivePath);
            using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Read, Global.Encoding))
            {
                VehicleCollection.Clear();
                foreach (var e in archive.Entries)
                {
                    
                    Stream entryStream = e.Open();
                    switch (e.FullName.Split('/')[0])
                    {
                        case "Car":
                            Car c = new Car();
                            JsonConvert.PopulateObject(Misc.StreamToString(entryStream), c, Global.SerializerSettings);
                            VehicleCollection.Add(e.FullName, c);
                            break;
                        default:
                            Vehicle v = new Vehicle();
                            JsonConvert.PopulateObject(Misc.StreamToString(entryStream), v, Global.SerializerSettings);
                            VehicleCollection.Add(e.FullName, v);
                            break;
                    }
                    entryStream.Close();
                    
                }
            }
        }

        public static void SaveVehicles()
        {
            if (File.Exists(ArchivePath + "bkp")) File.Delete(ArchivePath + "bkp");
            if (File.Exists(ArchivePath)) File.Move(ArchivePath, ArchivePath + "bkp");
            using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Create, Global.Encoding))
            {
                foreach (var item in VehicleCollection)
                {
                    Stream entryStream = archive.CreateEntry(item.Key).Open();
                    byte[] entryBytes = Global.Encoding.GetBytes(JsonConvert.SerializeObject(item.Value, Global.SerializerSettings));
                    entryStream.Write(entryBytes, 0, entryBytes.Length);
                    entryStream.Close();
                }
            }
            if (File.Exists(ArchivePath + "bkp")) File.Delete(ArchivePath + "bkp");
        }

        public static void RemoveVehicle(string key)
        {
            VehicleCollection.Remove(key);
            using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Update, Global.Encoding))
            {
                archive.GetEntry(key).Delete();
            }
        }
        public static void RenameVehicle(string key, string newKey)
        {
            var a = VehicleCollection[key];
            RemoveVehicle(key);
            VehicleCollection.Add(newKey, a);
            using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Update, Global.Encoding))
            {
                Stream entryStream = archive.CreateEntry(newKey).Open();
                byte[] entryBytes = Global.Encoding.GetBytes(JsonConvert.SerializeObject(a, Global.SerializerSettings));
                entryStream.Write(entryBytes, 0, entryBytes.Length);
                entryStream.Close();
            }
        }

        public static Vehicle getKey(string key)
        {
            return (Vehicle)VehicleCollection[key].Clone();
        }
    }
}
