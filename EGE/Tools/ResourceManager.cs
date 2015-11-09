using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace EGE.Tools
{
    public static class ResourceManager
    {
        public static Dictionary<string, long> Files = new Dictionary<string, long>();
        static string ArchivePath;
        public static void LoadResources(string FilePath)
        {
            ArchivePath = FilePath + "\\Resources.ege";
            Create(ArchivePath);
            using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Read, Global.Encoding))
            {
                foreach (var item in archive.Entries)
                {
                    Files.Add(item.Name, item.Length);
                }
            }
        }

        static void Create(string path)
        {
            if (!File.Exists(path))
            {
                using (ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Create, Global.Encoding))
                {
                }
            }
        }

        public static byte[] GetResource(string Name)
        {
            try {
                byte[] data;
                using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Read, Global.Encoding))
                {
                    ZipArchiveEntry e = archive.GetEntry(Name);
                    Stream s = e.Open();
                    data = new byte[e.Length];
                    s.Read(data, 0, data.Length);
                }
                return data;
            }
            catch { return new byte[0]; }
        }

        public static Stream GetStream(string Name)
        {
            try {
                ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Read, Global.Encoding);
                ZipArchiveEntry e = archive.GetEntry(Name);
                Stream s = e.Open();
                return s;
            }
            catch { return null; }
        }

        public static void AddFile(string path)
        {
            using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Update, Global.Encoding))
            {
                ZipArchiveEntry e = archive.CreateEntry(Misc.pathName(path));
                Stream s = e.Open();
                File.OpenRead(path).CopyTo(s);
                Files.Add(Misc.pathName(path), s.Length);
            }
        }

        public static void RemoveFile(string name)
        {
            Files.Remove(name);
            using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Update, Global.Encoding))
            {
                archive.GetEntry(name).Delete();
            }
        }
    }
}
