using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace EGE.Characters
{
    class DebugView:Character
    {
        Vector3 centerPoint = new Vector3(25, 2, 10);
        Vector3 orientation;
        static CameraDefinition defaultCameraDefinition = new CameraDefinition()
        {
            Distance = 10,
            FPV = true,
            Offset = Vector3.Zero,
            ViewAngle = Vector2.One,
            Style = DrawingStyle.Wireframe
        };

        float X = 200, Y;
        Matrix4 position = Matrix4.CreateTranslation(new Vector3(1, 1, 1));

        public override CameraDefinition CurrentCameraDefinition
        {
            get
            {
                return defaultCameraDefinition;
            }
            set { }
        }

        public override void Update(float elaspedTime, KeyboardState keyboardState)
        {
            Matrix4 Movement = Matrix4.Identity;
            Movement *= Matrix4.CreateTranslation(new Vector3(Controller.In(Func.Acceleration) * 0.1f, 0, 0));
            Movement *= Matrix4.CreateTranslation(new Vector3(Controller.In(Func.Brake) * -0.1f, 0, 0));
            Movement *= Matrix4.CreateTranslation(new Vector3(0, 0, Controller.In(Func.Left) * -0.1f));
            Movement *= Matrix4.CreateTranslation(new Vector3(0, 0, Controller.In(Func.Right) * 0.1f));
            Movement *= Matrix4.CreateRotationY(orientation.Y);
            centerPoint += Movement.ExtractTranslation();

            MouseState mouseState = Mouse.GetState();
            X = (Mouse.GetCursorState().X - 200);
            Y = (Mouse.GetCursorState().Y - 200);
            
            Mouse.SetPosition(200, 200);
            orientation.Y -= X/500f;
            orientation.X -= Y / 500f;
        }

        public override void Draw()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 WorldMatrix = Camera.GenerateLookAt(centerPoint, orientation, CurrentCameraDefinition);
            GL.LoadMatrix(ref WorldMatrix);
        }
    }
}
