using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;

namespace EGE.Cameras
{
    class FirstPersonCamera:Camera
    {
        public FirstPersonCamera()
        {

        }

        public override void Update()
        {
            MouseState mouseState = Mouse.GetCursorState();
            
            X = mouseState.X - (ScreenWidth/2);
            Y = mouseState.Y - (ScreenHeight / 2);

            ResetView();
            Orientation.Y -= X / 500f;
            Orientation.X -= Y / 500f;
            if (Orientation.X < -1.23f) Orientation.X += Y / 500f;
            if (Orientation.X > 0.94f) Orientation.X += Y / 500f;
            base.Update();
        }
    }
}
