using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.IO.Compression;

namespace EGE
{
    public static class Misc
    {
        public static float toFloat(string data)
        {
            float result = 0;
            float.TryParse(data, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result);
            return result;
        }

        public static string pathUp(string path)
        {
            string[] p = path.Replace("\\", "/").Split('/');
            return string.Join("/", p, 0, p.Length - 1);
        }

        public static string pathName(string path)
        {
            string[] p = path.Replace("\\", "/").Split('/');
            return p[p.Length-1];
        }

        public static void parseFloat(string data, out float result)
        {
            float.TryParse(data, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result);
        }

        public static int toInt(string data)
        {
            int result = 0;
            int.TryParse(data, out result);
            return result;
        }

        public static int Push<T>(T value, ref T[] values)
        {
            Array.Resize<T>(ref values, values.Length + 1);
            values[values.Length - 1] = value;
            return values.Length - 1;
        }

        public static void Push<T>(T[] value, ref T[] values)
        {
            Array.Resize<T>(ref values, values.Length + value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                values[values.Length-value.Length+i] = value[i];
            }
        }

        public static int LoadTexture(Bitmap image, float scale)
        {
            /*if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);*/

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

        public static Vector2[] GetBezierApproximation(Vector2[] controlPoints, int outputSegmentCount)
        {
            Vector2[] points = new Vector2[outputSegmentCount + 1];
            for (int i = 0; i <= outputSegmentCount; i++)
            {
                double t = (double)i / outputSegmentCount;
                points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
            }
            return points;
        }

        public static Vector2 GetBezierPoint(double t, Vector2[] controlPoints, int index, int count)
        {
            if (count == 1)
                return controlPoints[index];
            var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
            var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
            return new Vector2((float)((1 - t) * P0.X + t * P1.X), (float)((1 - t) * P0.Y + t * P1.Y));
        }
        public static float normalizeAngle(float angle)
        {
            angle += MathHelper.TwoPi;
            angle %= MathHelper.TwoPi;
            return angle;
        }

        public static float halfNormalizeAngle(float angle)
        {
            angle = normalizeAngle(angle);
            if (angle > MathHelper.Pi) angle -= MathHelper.TwoPi;
            return angle;
        }

        public static double getAngleD(Vector2 vector)
        {
            if (vector.Y >= 0 && vector.X >= 0) return Math.Asin(vector.Y / vector.Length);
            if (vector.Y >= 0 && vector.X <= 0) return Math.PI-Math.Asin(vector.Y / vector.Length);
            if (vector.Y <= 0 && vector.X >= 0) return (Math.PI*2) + Math.Asin(vector.Y / vector.Length);
            if (vector.Y <= 0 && vector.X <= 0) return Math.PI - Math.Asin(vector.Y / vector.Length);
            return 0;
        }

        public static float getAngle(Vector2 vector)
        {
            return (float)getAngleD(vector);
        }

        public static Vector2 getCartesian(float angle)
        {
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);
            //float cos = (float)Math.Sqrt(1 - (sin * sin));
            return new Vector2(cos, sin);
        }

        public static string StreamToString(Stream stream)
        {
            //stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.Default))
            {
                return reader.ReadToEnd();
            }
        }

        public static string sizeToString(long bytes)
        {
            if (bytes > 1000000000) return Math.Round(bytes / 1000000000f, 2) + " GB";
            if (bytes > 1000000) return Math.Round(bytes / 1000000f, 2) + " MB";
            if (bytes > 1000) return Math.Round(bytes / 1000f, 2) + " kB";
            else return bytes + " b";
        }

        public static float lerp(float v0, float v1, float t)
        {
            return (1 - t) * v0 + t * v1;
        }

        public static void CheckArchive(string path)
        {
            if (!File.Exists(path))
            {
                using (ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Create, Global.Encoding)) { }
            }
        }

        public static bool PointIntPolygon(Vector2[] vertices, Vector2 point)
        {
            int j = vertices.Length - 1;
            bool c = false;
            for (int i = 0; i < vertices.Length; j = i++)
            {
                if (((vertices[i].Y > point.Y) != (vertices[j].Y > point.Y)) &&
                 (point.X < (vertices[j].X - vertices[i].X) * (point.Y - vertices[i].Y) / (vertices[j].Y - vertices[i].Y) + vertices[i].X))
                    c = !c;
            }
            return c;
        }
    }
}
