using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace EGE
{
    public class Graphics
    {
        public static bool Initialized = false;
        private static float AspectRatio;
        public static Environment.Node PointerLocation = new Environment.Node();
        public static string EditMesh;
        public static bool StaticView;
        private static Matrix4 ProjectionMatrix;
        private static float Width, Height;
        private static float DetailDistance1 = (float)Math.Pow(20,2), DetailDistance2 = (float)Math.Pow(100,2);

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
            Graphics.Width = Width;
            Graphics.Height = Height;
            AspectRatio = Width / Height;
            GL.Viewport(0, 0, (int)Width, (int)Height);
        }

        public static void SetProjection()
        {
            GL.MatrixMode(MatrixMode.Projection);
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70), AspectRatio, 0.1f, 1000);
            GL.LoadMatrix(ref ProjectionMatrix);
        }

        /// <summary>
        /// Determines if provided point in 3D space is visible on screen
        /// </summary>
        /// <param name="point">3D point in space</param>
        /// <param name="margin">Screen margin for detection: 0 - center, 1 - border</param>
        /// <returns></returns>
        public static bool PointVisible(Vector3 point, float margin)
        {
            bool v = false;
            var vrc = Graphics.GetViewCoordinates(point, ref v);
            return v && Math.Abs(vrc.X) < margin && Math.Abs(vrc.Y) < margin;
        }

        /// <summary>
        /// Determine level of detail for specified object
        /// </summary>
        /// <param name="point">Object center</param>
        /// <returns>0 - Don't draw, 1 - Low poly (solid), 2 - low poly (textured), 3 - normal</returns>
        public static int GetDetailLevel(Vector3 point, float size)
        {
            Vector4
                obj = new Vector4(point, 1.0f),
                eye = Vector4.Transform(obj, World.WorldMatrix),
                clip = Vector4.Transform(eye, ProjectionMatrix);
            Vector3
                ndc = new Vector3(clip.X / clip.W, clip.Y / clip.W, clip.Z / clip.W);
            size *= 1.2f;
            float distance = eye.LengthSquared;
            if (clip.W < -size || (distance > (size * size) && Math.Abs(ndc.X) > 1.2f && Math.Abs(ndc.Y) > 1.2f)) return 0;
            else if (distance > DetailDistance2) return 1;
            else if (distance > DetailDistance1) return 2;
            else return 3;
        }

        public static Vector2 GetViewCoordinates(Vector3 ObjectCoordinate, ref bool visible)
        {
            // ref: http://www.songho.ca/opengl/gl_transform.html
            Vector4
                obj = new Vector4(ObjectCoordinate.X, ObjectCoordinate.Y, ObjectCoordinate.Z, 1.0f),
                eye = Vector4.Transform(obj, World.WorldMatrix),
                clip = Vector4.Transform(eye, ProjectionMatrix);
            Vector3
                ndc = new Vector3(clip.X / clip.W, clip.Y / clip.W, clip.Z / clip.W);
            visible = clip.W > 0;
            return ndc.Xy;
        }

        public static Vector2 GetScreenCoordinates(Vector3 ObjectCoordinate, ref bool visible)
        {
            Vector4 viewPort = new Vector4(0, 0, Width, Height);
            Vector2 ndc = GetViewCoordinates(ObjectCoordinate, ref visible);
            Vector2
                w = new Vector2(viewPort.Z / 2 * ndc.X + viewPort.X + viewPort.Z / 2,
                                viewPort.W / 2 * ndc.Y + viewPort.Y + viewPort.W / 2);
            return w;
        }
    }
}
