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
                    Vehicle v = new Vehicle();
                    Stream entryStream = e.Open();
                    JsonConvert.PopulateObject(Misc.StreamToString(entryStream), v, Global.SerializerSettings);
                    entryStream.Close();
                    VehicleCollection.Add(e.FullName, v);
                }
            }
        }
    }
}
