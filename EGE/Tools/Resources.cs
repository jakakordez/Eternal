using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.IO.Compression;
using EGE.Meshes;

namespace EGE
{
    public struct RFile{
        public string Folder, Name;
        private string extension;
        public string Extension {
            set
            {
                string[] textureTypes = new string[] { "jpg", "bmp", "png" };
                if (textureTypes.Count(f => value == f) > 0) Type = RFileType.Texture;
                else if (value == "mesh") Type = RFileType.Mesh;
                else Type = RFileType.File;
                extension = value;
            }
            get
            {
                return extension;
            }
        }

        public long Size;
        public int Id;
        public object obj;
        public RFileType Type;
        public enum RFileType
        {
            File,
            Mesh,
            Texture
        }

        public string FullName {
            get { return Folder + Name + "." + Extension; }
            set
            {
                string[] p = Misc.pathName(value).Split('.');
                Name = p[0];
                Extension = p[1];
                Folder = Misc.pathUp(value)+"/";
                if (Folder == "/") Folder = "";
            }
        }

        public RFile(string fullName)
        {
            Folder = "";
            Name = "";
            extension = "";
            Type = RFileType.File;
            Size = 0;
            Id = 0;
            obj = null;
            FullName = fullName;
        }
    }

    public class Resources
    {
        public static string ArchivePath;

        static List<RFile> Files;

        public static void LoadResources(string FilePath)
        {
            Files = new List<RFile>();
            ArchivePath = FilePath + "\\Resources.ege";
            CheckArchive(ArchivePath);
            using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Read, Global.Encoding))
            {
                Files.Clear();
                foreach (var e in archive.Entries)
                {
                    if (e.Name == "") continue;
                    
                    RFile f = new RFile(e.FullName);
                    f.Size = e.Length;
                    if(f.Type == RFile.RFileType.Texture) f.Id = LoadImage(e);
                    else if (f.Type == RFile.RFileType.Mesh) f.obj = LoadMesh(e, f.Folder);
                    
                    Files.Add(f);
                }
            }
        }

        public static void FillTreeview(System.Windows.Forms.TreeNodeCollection rootNodes)
        {
            CheckArchive(ArchivePath);
            using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Update, Global.Encoding))
            {
                foreach (var e in archive.Entries)
                {
                    if (e.FullName.Contains('/'))
                    {
                        System.Windows.Forms.TreeNodeCollection nodes = rootNodes;
                        string[] parts = e.FullName.Split('/');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (parts[i].Contains('.') || parts[i] == "") continue;
                            bool found = false;
                            
                            foreach (System.Windows.Forms.TreeNode item in nodes)
                            {
                                if(item.Text == parts[i])
                                {
                                    nodes = item.Nodes;
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                var n = new System.Windows.Forms.TreeNode(parts[i]);
                                nodes.Add(n);
                                nodes = n.Nodes;
                            }
                        }
                    }
                }
            }
        }

        public static List<RFile> GetFolderFiles(string location)
        {
            return Files.Where(f => f.Folder == location).ToList();
        }

        private static int LoadImage(ZipArchiveEntry e)
        {
            Bitmap image = (Bitmap)Image.FromStream(e.Open());
            float scale = 1;
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            BitmapData bmp_data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)(bmp_data.Width / scale), (int)(bmp_data.Height / scale), 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            image.UnlockBits(bmp_data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            return id;
        }

        public static Mesh LoadMesh(ZipArchiveEntry e, string folder)
        {
            Mesh m = new Mesh(e.Name);
            m.LoadMTL(folder+e.Name.Split('.')[0] + ".mtl");
            using (ZipArchive meshArchive = new ZipArchive(e.Open(), ZipArchiveMode.Read))
            {
                m.LoadMesh(meshArchive);
            }
            return m;
        }

        public static Image textureToBitmap(string TextureName)
        {
            Bitmap result;
            using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Read, Global.Encoding))
            {
                result = (Bitmap)Image.FromStream(archive.GetEntry(TextureName).Open());
            }
            return result;
        }

        static void CheckArchive(string path)
        {
            if (!File.Exists(path))
            {
                using (ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Create, Global.Encoding)){ }
            }
        }

        static void CreateFile(string Name)
        {
            using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Update, Global.Encoding))
            {
                Stream s = archive.CreateEntry(Name).Open();
            }
        }

        public static byte[] GetFile(string Name)
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

        public static void AddFile(string path, string toFolder)
        {
            using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Update, Global.Encoding))
            {
                ZipArchiveEntry e = archive.CreateEntry(toFolder+Misc.pathName(path));
                Stream s = e.Open();
                File.OpenRead(path).CopyTo(s);
                RFile f = new RFile(e.FullName);
                f.Size = s.Length;
                s.Close();
                if (f.Type == RFile.RFileType.Texture) f.Id = LoadImage(e);
                else if (f.Type == RFile.RFileType.Mesh) f.obj = LoadMesh(e, f.Folder);
                
                f.Folder = toFolder;
                Files.Add(f);
            }
        }

        public static void BindTexture(string textureName)
        {
            GL.Color4(Color.White);
            GL.BindTexture(TextureTarget.Texture2D, findFile(textureName, RFile.RFileType.Texture).Id);
        }

        public static void RemoveFile(string name)
        {
            Files.RemoveAll(f => f.FullName == name);
            using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Update, Global.Encoding))
            {
                archive.GetEntry(name).Delete();
            }
        }

        public static void RemoveFolder(string folder)
        {
            using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Update, Global.Encoding))
            {
                archive.Entries.Where(e => e.FullName.StartsWith(folder)).AsParallel().ForAll(e => e.Delete());
            }
        }

        public static void DrawMesh(string name)
        {
            Mesh m = (Mesh)findFile(name + ".mesh", RFile.RFileType.Mesh).obj;
            if(m != null)m.Draw();
        }

        public static void BuildMesh(RFile fromFile)
        {
            CheckArchive(ArchivePath);
            // if (ResourceManager.ContainsKey(Name)) return;

            Mesh m = new Mesh(fromFile.Folder+fromFile.Name);
            m.LoadMTL(fromFile.Folder+fromFile.Name + ".mtl");
            m.LoadOBJ(fromFile.Folder+fromFile.Name);
        }

        public static RFile findFile(string name, RFile.RFileType type)
        {
            foreach (var item in Files)
            {
                if (item.FullName == name && item.Type == type) return item;
            }
            return new RFile();
        }
    }
}
