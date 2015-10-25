using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using System.Windows.Forms;

namespace EGE
{
    public enum Func
    {
        Acceleration,
        Brake,
        Left,
        Right,
        CruiseControl,
        EngineStart,
        Forward,
        Reverse
    }
    class Controller
    {
        struct ControlSource
        {
            public int DeviceIndex;
            public Type DeviceType;
            public int ControlIndex;
            public float Offset;
            public float Scale;
            public float NullZone;
        }

        private static float MouseX, MouseY, MouseScroll;

        private static Dictionary<Func, ControlSource> ControllerMapping;

        public static void InitController() {
            ControllerMapping = new Dictionary<Func, ControlSource>();
            ControllerMapping.Add(Func.Acceleration, new ControlSource() { DeviceIndex = 0, DeviceType = typeof(Keyboard) , ControlIndex=(int)Key.W});
            ControllerMapping.Add(Func.Brake, new ControlSource() { DeviceIndex = 0, DeviceType = typeof(Keyboard), ControlIndex = (int)Key.S });
            ControllerMapping.Add(Func.Left, new ControlSource() { DeviceIndex = 0, DeviceType = typeof(Keyboard), ControlIndex = (int)Key.A });
            ControllerMapping.Add(Func.Right, new ControlSource() { DeviceIndex = 0, DeviceType = typeof(Keyboard), ControlIndex = (int)Key.D });
        }

        public static void Update()
        {
            int Width = Screen.PrimaryScreen.Bounds.Width;
            int Height = Screen.PrimaryScreen.Bounds.Height;
            MouseX = (Width / 2) / ((Width/2) - Mouse.GetState().X);
            MouseY = (Height / 2) / ((Height/2) - Mouse.GetState().Y);
            MouseScroll = Mouse.GetState().WheelPrecise;
        }


        public static float In(Func function)
        {
            ControlSource source = ControllerMapping[function];
            if (source.DeviceType == typeof(Keyboard))
            {
                return Keyboard.GetState(source.DeviceIndex).IsKeyDown((short)source.ControlIndex)?1:0;
            }
            else if(source.DeviceType == typeof(Mouse))
            {
                MouseState deviceState = Mouse.GetState();
                switch (source.ControlIndex)
                {
                    case 0: return MouseX;
                    case 1: return MouseY;
                    case 2: return deviceState.WheelPrecise;
                    case 3: return deviceState.LeftButton == OpenTK.Input.ButtonState.Pressed ? 1 : 0;
                    case 4: return deviceState.MiddleButton == OpenTK.Input.ButtonState.Pressed ? 1 : 0;
                    case 5: return deviceState.RightButton == OpenTK.Input.ButtonState.Pressed ? 1 : 0;
                }
            }
            /*else if(DeviceType == typeof(Joystick))
            {
                JoystickState deviceState = Joystick.GetState(deviceIndex);
                if (controlIndex <= 10) return deviceState.GetAxis((JoystickAxis)controlIndex);
                else if(controlIndex <= 14) return deviceState.GetHat((JoystickHat)(controlIndex-10)).
            }
            else if(DeviceType == typeof(GamePad))
            {
                
            }*/
            return 0;
        }
    }
}
