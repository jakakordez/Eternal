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
        Vector3 centerPoint = new Vector3(0, 5, 0);//new Vector3(800, 5, 200);
        Vector3 orientation;
        bool inn = false;
        static CameraDefinition defaultCameraDefinition = new CameraDefinition()
        {
            Distance = 10,
            FPV = true,
            Offset = Vector3.Zero,
            ViewAngle = Vector2.One,
            Style = DrawingStyle.Wireframe
        };

        float X = 200, Y;

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
            float sp = (Controller.In(Func.FastMode) * 1) + 0.5f;
            Movement *= Matrix4.CreateTranslation(new Vector3(Controller.In(Func.Acceleration) * sp, 0, 0));
            Movement *= Matrix4.CreateTranslation(new Vector3(Controller.In(Func.Brake) * -sp, 0, 0));
            Movement *= Matrix4.CreateTranslation(new Vector3(0, 0, Controller.In(Func.Left) * -sp));
            Movement *= Matrix4.CreateTranslation(new Vector3(0, 0, Controller.In(Func.Right) * sp));
            Movement *= Matrix4.CreateTranslation(new Vector3(0, Controller.In(Func.Up) * sp, 0));
            Movement *= Matrix4.CreateTranslation(new Vector3(0, Controller.In(Func.Down) * -sp, 0));
            Movement *= Matrix4.CreateRotationY(orientation.Y);
            centerPoint += Movement.ExtractTranslation();

            MouseState mouseState = Mouse.GetState();
            X = (Mouse.GetCursorState().X - 200);
            Y = (Mouse.GetCursorState().Y - 200);
            
            Mouse.SetPosition(200, 200);
            orientation.Y -= X/500f;
            orientation.X -= Y / 500f;


            if (Controller.In(Func.View)==1 && !inn)
            {
                inn = true;
                if (Settings.CurrentDrawingMode == Settings.DrawingModes.Textured) Settings.CurrentDrawingMode = Settings.DrawingModes.Wireframe;
                else Settings.CurrentDrawingMode = Settings.DrawingModes.Textured;
            }
            else if(Controller.In(Func.View) == 0) inn = false;
        }

        public override void Draw()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            World.WorldMatrix = Camera.GenerateLookAt(centerPoint, orientation, CurrentCameraDefinition);
            GL.LoadMatrix(ref World.WorldMatrix);
        }
    }
}
