using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;

namespace EGE.Environment
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Nodes
    {
        [JsonProperty]
        public static Dictionary<ulong, Node> NodeList;
        [JsonProperty]
        static ulong IdCounter = 0;
        static string FilePath;

        public void LoadNodes(string filePath)
        {
            FilePath = filePath;
            if (File.Exists(FilePath))
            {
                // Open nodes
                using (ZipArchive archive = ZipFile.Open(FilePath, ZipArchiveMode.Read, Global.Encoding))
                {
                    // Load node collection
                    Stream entryStream = archive.GetEntry("Nodes.json").Open();
                    JsonConvert.PopulateObject(Misc.StreamToString(entryStream), this, Global.SerializerSettings);
                    entryStream.Close();
                }
            }
        }

        public void SaveNodes(string filePath)
        {
            FilePath = filePath;
            if (File.Exists(FilePath + "bkp")) File.Delete(FilePath + "bkp");
            if (File.Exists(filePath)) File.Move(filePath, filePath + "bkp");
            // Open terrain file
            using (ZipArchive archive = ZipFile.Open(FilePath, ZipArchiveMode.Create, Global.Encoding))
            {
                // Save terrain descriptor
                Stream entryStream = archive.CreateEntry("Nodes.json").Open();
                byte[] entryBytes = Global.Encoding.GetBytes(JsonConvert.SerializeObject(this, Global.SerializerSettings));
                entryStream.Write(entryBytes, 0, entryBytes.Length);
                entryStream.Close();
            }
            if (File.Exists(filePath + "bkp")) File.Delete(filePath + "bkp");
        }

        public static ulong AddNode(Node n)
        {
            NodeList.Add(++IdCounter, n);
            return IdCounter;
        }

        public static void SetNode(ulong ID, Node n)
        {
            NodeList[ID] = n;
        }

        public static ulong Merge(ulong MergeInto, ulong MergeItem)
        {
            NodeList.Remove(MergeItem);
            return MergeInto;
        }

        public static Node GetNode(ulong id)
        {
            return NodeList[id];
        }
    }
}
