using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.IO.Compression;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace EGE.Tools
{
    public static class TextureManager
    {
        static Dictionary<string, int> TextureMap = new Dictionary<string, int>();
        static string ArchivePath;

        public static void LoadTextures(string FilePath)
        {
            ArchivePath = FilePath+"\\Textures.ege";
            Create(ArchivePath);
            using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Read, Global.Encoding))
            {
                foreach (var e in archive.Entries)
                {
                    AddImage(e.Name, (Bitmap)Image.FromStream(e.Open()));
                }
            }
        }

        static void AddImage(string name, Bitmap image)
        {
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
            TextureMap.Add(name, id);
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

        public static void AddTexture(string textureName, Bitmap data)
        {
            using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Update, Global.Encoding))
            {
                ZipArchiveEntry e = archive.CreateEntry(textureName);
                Stream s = e.Open();
                data.Save(s, ImageFormat.Bmp);
            }
            AddImage(textureName, data);
        }

        public static void RemoveTexture(string textureName)
        {
            GL.DeleteTexture(TextureMap[textureName]);
            using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Update, Global.Encoding))
            {
                archive.GetEntry(textureName).Delete();
            }
        }

        public static void BindTexture(string textureName)
        {
            GL.Color4(Color.White);
            GL.BindTexture(TextureTarget.Texture2D, TextureMap[textureName]);
        }

        public static Dictionary<string, Bitmap> GetTextures()
        {
            Create(ArchivePath);
            Dictionary<string, Bitmap> images;
            using (ZipArchive archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Read, Global.Encoding))
            {
                images = new Dictionary<string, Bitmap>();
                foreach (var e in archive.Entries)
                {
                    images.Add(e.Name, (Bitmap)Image.FromStream(e.Open()));
                }
            }
            return images;
        }
    }
}
