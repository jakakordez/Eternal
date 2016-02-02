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
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthClamp);
            //GL.Enable(EnableCap.CullFace);
            //GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Normalize);
            GL.Enable(EnableCap.RescaleNormal);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
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
            Matrix4 ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70), AspectRatio, 0.1f, 1000);
            GL.LoadMatrix(ref ProjectionMatrix);
        }
    }
}
