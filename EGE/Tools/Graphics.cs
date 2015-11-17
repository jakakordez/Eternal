using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace EGE.Tools
{
    class Graphics
    {
        public static bool Initialized = false;
        static float AspectRatio;

        public static void Init()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthClamp);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Light0);
            //GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.AlphaTest);
            Initialized = true;
        }

        public static void Resize(float Width, float Height)
        {
            AspectRatio = Width / Height;
            GL.Viewport(0, 0, (int)Width, (int)Height);
        }

        public static void SetProjection()
        {
            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70), AspectRatio, 0.1f, 500);
            GL.LoadMatrix(ref ProjectionMatrix);
        }
    }
}
