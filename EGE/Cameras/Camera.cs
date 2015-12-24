using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;

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
        public float Distance;
        public bool FPV;
        public Vector2 ViewAngle;
        public Vector3 Offset;
        public DrawingStyle Style;
    }
    public class Camera
    {
        public Vector3 Orientation;

        protected int ScreenHeight, ScreenWidth, X, Y;
        public Camera()
        {
            ScreenHeight = Screen.PrimaryScreen.Bounds.Size.Height;
            ScreenWidth = Screen.PrimaryScreen.Bounds.Size.Width;
        }

        public virtual void Update()
        {

        }

        public void ResetView()
        {
            Cursor.Position = new System.Drawing.Point(ScreenWidth / 2, ScreenHeight / 2);
        }

        public static Matrix4 GenerateLookAt(Vector3 center, Vector3 orientation, CameraDefinition cameraConfiguration)
        {
            Matrix4 e = new Matrix4(), t = new Matrix4();
            //e = Matrix4.CreateTranslation(cameraConfiguration.Offset);
            //t = Matrix4.CreateTranslation(cameraConfiguration.Offset + new Vector3(0, 0, 2)) * Matrix4.CreateRotationX((/*Y*/0 * cameraConfiguration.ViewAngle.X) - (cameraConfiguration.ViewAngle.X / 2)) * Matrix4.CreateRotationY((-/*X*/0 * cameraConfiguration.ViewAngle.Y) + (cameraConfiguration.ViewAngle.Y / 2));

            //e *= Matrix4.CreateFromQuaternion(orientation);
            //t *= Matrix4.CreateFromQuaternion(orientation);
            //e *= Matrix4.CreateTranslation(centerPoint);
            //t *= Matrix4.CreateTranslation(centerPoint);



            /*e = Matrix4.CreateRotationY(orientation.Y);
            e *= Matrix4.CreateTranslation(1, 0, 0);
            e *= Matrix4.CreateRotationY(-orientation.Y);
            e *= Matrix4.CreateTranslation(-centerPoint);
            e = T(centerPoint) * R(-orientation.Y) * T(1, 0, 0) * R(orientation.Y);*/
            e = Matrix4.Identity* T(1, 0, 0) * Matrix4.RotateZ(orientation.X) * R(orientation.Y) * T(center);


            return Matrix4.LookAt(center, e.ExtractTranslation(), Vector3.UnitY);
            //return Matrix4.LookAt(center, Vector3.Zero, Vector3.UnitY);
        }

        private static Matrix4 R(float Y)
        {
            return Matrix4.CreateRotationY(Y);
        }

        private static Matrix4 T(float x, float y, float z)
        {
            return Matrix4.CreateTranslation(x, y, z);
        }

        private static Matrix4 T(Vector3 v)
        {
            return Matrix4.CreateTranslation(v);
        }


    }
}
