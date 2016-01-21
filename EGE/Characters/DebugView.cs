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
        Vector3 centerPoint = new Vector3(673, 5, 274);
        bool inn = false;
        static CameraDefinition defaultCameraDefinition = new CameraDefinition()
        {
            Distance = 10,
            FPV = true,
            Offset = Vector3.Zero,
            ViewAngle = Vector2.One,
            Style = DrawingStyle.Wireframe
        };

        public DebugView()
        {
            CameraList = new Camera[] { new FirstPersonCamera(defaultCameraDefinition)};
            CurrentCamera = 0;
        }

        public override void Update(float elaspedTime)
        {
            CameraList[0].Update();

            Matrix4 Movement = Matrix4.Identity;
            float sp = (Controller.Val(Func.FastMode) * 1) + 0.5f;
            Movement *= Matrix4.CreateTranslation(new Vector3(Controller.Val(Func.Forward) * sp, 0, 0));
            Movement *= Matrix4.CreateTranslation(new Vector3(Controller.Val(Func.Backward) * -sp, 0, 0));
            Movement *= Matrix4.CreateTranslation(new Vector3(0, 0, Controller.Val(Func.Left) * -sp));
            Movement *= Matrix4.CreateTranslation(new Vector3(0, 0, Controller.Val(Func.Right) * sp));
            Movement *= Matrix4.CreateTranslation(new Vector3(0, Controller.Val(Func.Up) * sp, 0));
            Movement *= Matrix4.CreateTranslation(new Vector3(0, Controller.Val(Func.Down) * -sp, 0));
            Movement *= Matrix4.CreateRotationY(CameraList[0].Orientation.Y);
            centerPoint += Movement.ExtractTranslation();   

            if (Controller.In(Func.View) && !inn)
            {
                inn = true;
                if (Settings.CurrentDrawingMode == Settings.DrawingModes.Textured) Settings.CurrentDrawingMode = Settings.DrawingModes.Wireframe;
                else Settings.CurrentDrawingMode = Settings.DrawingModes.Textured;
            }
            else if(!Controller.In(Func.View)) inn = false;
        }

        public override void Draw()
        {
            
            GL.MatrixMode(MatrixMode.Modelview);

            CameraList[CurrentCamera].GenerateLookAt(centerPoint);
            GL.LoadMatrix(ref World.WorldMatrix);
        }
    }
}
