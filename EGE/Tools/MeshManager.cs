using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using EGE.Meshes;

namespace EGE.Tools
{
    public static class MeshManager
    {
        static string ArchivePath;
        static Dictionary<string, Mesh> MeshCollection;

        public static void LoadMeshes(string archivePath)
        {
            ArchivePath = archivePath+"\\Meshes.ege";
            MeshCollection = new Dictionary<string, Mesh>();
        }

        public static void LoadMesh(string Name)
        {
            Create(ArchivePath);
            if (MeshCollection.ContainsKey(Name)) return;
                using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Update, Global.Encoding))
                {
                    //archive.GetEntry(Name+".mesh").Delete();
                    ZipArchiveEntry e = archive.GetEntry(Name+".mesh");
                    Mesh m = new Mesh(Name);
                    
                    if (e != null)
                    {
                        using (ZipArchive meshArchive = new ZipArchive(e.Open(), ZipArchiveMode.Read))
                        {
                            m.LoadMesh(meshArchive, Name+".mtl");
                        }
                    }
                    else {
                        using (ZipArchive meshArchive = new ZipArchive(archive.CreateEntry(Name + ".mesh").Open(), ZipArchiveMode.Update))
                        {
                            m.LoadOBJ(meshArchive);
                        }
                    }
                    MeshCollection.Add(Name, m);
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

        public static void DrawMesh(string Name)
        {
            MeshCollection[Name].Draw();
        }
    }
}
