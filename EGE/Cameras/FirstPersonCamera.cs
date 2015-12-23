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
            MouseState mouseState = Mouse.GetState();
            X = (Mouse.GetCursorState().X - 200);
            Y = (Mouse.GetCursorState().Y - 200);

            Mouse.SetPosition(200, 200);
            Orientation.Y -= X / 500f;
            Orientation.X -= Y / 500f;
            base.Update();
        }
    }
}
