using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using OpenTK.Input;

namespace EGE
{
    public enum DrawingStyle
    {
        Wireframe,
        Planes,
        Normal
    }
    public struct CameraDefinition
    {
        public float Distance { get; set; }
        public bool FPV { get; set; }
        public Vector2 ViewAngle { get; set; }
        public Vector3 Offset { get; set; }
        public DrawingStyle Style { get; set; }
    }
    public class Camera
    {
        public Vector3 Orientation;
        public CameraDefinition CameraSettings { get; set; }

        protected int ScreenHeight, ScreenWidth, X, Y;
        public Camera(CameraDefinition definition)
        {
            CameraSettings = definition;
            ScreenHeight = Screen.PrimaryScreen.Bounds.Size.Height;
            ScreenWidth = Screen.PrimaryScreen.Bounds.Size.Width;
        }

        public virtual void Update()
        {
            MouseState mouseState = Mouse.GetCursorState();

            X = mouseState.X - (ScreenWidth / 2);
            Y = mouseState.Y - (ScreenHeight / 2);

            ResetView();
            Orientation.Y -= X / 500f;
            Orientation.X -= Y / 500f;
            if (Orientation.X < -1.23f) Orientation.X += Y / 500f;
            if (Orientation.X > 0.94f) Orientation.X += Y / 500f;
        }

        public void ResetView()
        {
            Cursor.Position = new System.Drawing.Point(ScreenWidth / 2, ScreenHeight / 2);
        }

        public virtual void GenerateLookAt(Vector3 center)
        {
            Matrix4 e = Matrix4.CreateTranslation(1, 0, 0) * Matrix4.CreateTranslation(CameraSettings.Offset) * Matrix4.CreateRotationZ(Orientation.X) * Matrix4.CreateRotationY(Orientation.Y) * Matrix4.CreateTranslation(center);
            Matrix4 v = Matrix4.CreateTranslation(CameraSettings.Offset) * Matrix4.CreateRotationY(Orientation.Y) * Matrix4.CreateTranslation(center);
            World.ViewMatrix = Matrix4.LookAt(v.ExtractTranslation(), e.ExtractTranslation(), Vector3.UnitY);
            GL.UniformMatrix4(Graphics.ViewMatrixID, false, ref World.ViewMatrix);
        }

        public virtual void GenerateLookAt(Matrix4 centerTransform)
        {
            Matrix4 e = Matrix4.CreateTranslation(1, 0, 0) * Matrix4.CreateRotationZ(Orientation.X) * Matrix4.CreateRotationY(Orientation.Y) * Matrix4.CreateTranslation(CameraSettings.Offset) *  centerTransform;
            Matrix4 v = Matrix4.CreateTranslation(CameraSettings.Offset) *  centerTransform;
            World.ViewMatrix = Matrix4.LookAt(v.ExtractTranslation(), e.ExtractTranslation(), Vector3.UnitY);
            GL.UniformMatrix4(Graphics.ViewMatrixID, false, ref World.ViewMatrix);
        }
    }
}
