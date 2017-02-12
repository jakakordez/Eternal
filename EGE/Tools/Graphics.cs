using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;

namespace EGE
{
    public class Graphics
    {
        public static bool Initialized = false;
        private static float AspectRatio = 1;
        public static Environment.Node PointerLocation = new Environment.Node();
        public static string EditMesh;
        public static bool StaticView;
        public static Matrix4 ProjectionMatrix;
        private static float Width, Height;
        private static float DetailDistance = (float)Math.Pow(20, 2);
        public static int ProgramID, ModelMatrixID, ViewMatrixID, ProjectionMatrixID, MeshColorID, LightPositionID;
        public static Matrix4 MeshColor;
        public static Matrix3 LightPosition_worldspace;

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

            GL.Enable(EnableCap.Light0);
            GL.Light(LightName.Light0, LightParameter.Diffuse, new OpenTK.Graphics.Color4(253, 184, 19, 255));
            GL.Light(LightName.Light0, LightParameter.Position, new float[] { 512, 512, 512, 0});

            ProgramID = Shaders.ShaderLoader.LoadShaders(EGEResources.vertex_shader, EGEResources.fragment_shader);
            ModelMatrixID = GL.GetUniformLocation(ProgramID, "ModelMatrix");
            ViewMatrixID = GL.GetUniformLocation(ProgramID, "ViewMatrix");
            ProjectionMatrixID = GL.GetUniformLocation(ProgramID, "ProjectionMatrix");
            MeshColorID = GL.GetUniformLocation(ProgramID, "MeshColor");
            LightPositionID = GL.GetUniformLocation(ProgramID, "LightPosition_worldspace");
            MeshColor = new Matrix4();
            UpdateProjection();

            Initialized = true;
        }

        public static void SetColor(Color4 color)
        {
            Graphics.MeshColor.Row0 = new Vector4(color.R, color.G, color.B, color.A/255f);
            GL.UniformMatrix4(Graphics.MeshColorID, false, ref Graphics.MeshColor);
        }

        public static void Resize(float Width, float Height)
        {
            Graphics.Width = Width;
            Graphics.Height = Height;
            AspectRatio = Width / Height;
            GL.Viewport(0, 0, (int)Width, (int)Height);
            UpdateProjection();
        }

        public static void UpdateProjection()
        {
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70), AspectRatio, 1f, 1000);
        }

        public static void SetLight(Vector3 position)
        {
            LightPosition_worldspace.Row0 = position;
            GL.UniformMatrix3(LightPositionID, false, ref LightPosition_worldspace);
        }

        public static void SetProjection()
        {
            GL.UniformMatrix4(Graphics.ProjectionMatrixID, false, ref ProjectionMatrix);
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
        /// Determine if specified object is visible
        /// </summary>
        /// <param name="point">Object center</param>
        /// <returns>0 - Don't draw, 1 - Low poly, 3 - normal</returns>
        public static int GetDetailLevel(Vector3 point, float size)
        {
            Vector4
                obj = new Vector4(point, 1.0f),
                eye = Vector4.Transform(obj, World.ViewMatrix),
                clip = Vector4.Transform(eye, ProjectionMatrix);
            Vector3
                ndc = new Vector3(clip.X / clip.W, clip.Y / clip.W, clip.Z / clip.W);
            size *= 1.2f;
            float distance = eye.LengthSquared;
            if (clip.W < -size || (distance > (size * size) && Math.Abs(ndc.X) > 1.2f && Math.Abs(ndc.Y) > 1.2f)) return 0;
            else if (distance > DetailDistance) return 1;
            else return 2;
            //return (Math.Abs(ndc.X) > 1.2f || Math.Abs(ndc.Y) > 1.2f)?0:3;
            //return 3;
        }

        public static Vector2 GetViewCoordinates(Vector3 ObjectCoordinate, ref bool visible)
        {
            // ref: http://www.songho.ca/opengl/gl_transform.html
            Vector4
                obj = new Vector4(ObjectCoordinate.X, ObjectCoordinate.Y, ObjectCoordinate.Z, 1.0f),
                eye = Vector4.Transform(obj, World.ViewMatrix),
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
