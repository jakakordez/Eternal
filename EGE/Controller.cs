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
        Forward,
        Backward,
        Left,
        Right,
        Up,
        Down,
        CruiseControl,
        EngineStart,
        GearUp,
        GearDown,
        View,
        FastMode,
        Jump,
        Enter,
        SwitchView
    }
    class Controller
    {
        class ControlSource
        {
            public int DeviceIndex;
            public Type DeviceType;
            public int ControlIndex;
            public float Offset;
            public float Scale;
            public float NullZone;
            public float PrevState;
            public float CurrentState;
        }

        private static float MouseX, MouseY, MouseScroll;

        private static Dictionary<Func, ControlSource> ControllerMapping;

        public static void InitController() {
            ControllerMapping = new Dictionary<Func, ControlSource>();
            ControllerMapping.Add(Func.Acceleration, new ControlSource() { DeviceIndex = 0, DeviceType = typeof(Keyboard) , ControlIndex=(int)Key.W});
            ControllerMapping.Add(Func.Brake, new ControlSource() { DeviceIndex = 0, DeviceType = typeof(Keyboard), ControlIndex = (int)Key.S });
            ControllerMapping.Add(Func.Forward, new ControlSource() { DeviceIndex = 0, DeviceType = typeof(Keyboard), ControlIndex = (int)Key.W });
            ControllerMapping.Add(Func.Backward, new ControlSource() { DeviceIndex = 0, DeviceType = typeof(Keyboard), ControlIndex = (int)Key.S });
            ControllerMapping.Add(Func.Left, new ControlSource() { DeviceIndex = 0, DeviceType = typeof(Keyboard), ControlIndex = (int)Key.A });
            ControllerMapping.Add(Func.Right, new ControlSource() { DeviceIndex = 0, DeviceType = typeof(Keyboard), ControlIndex = (int)Key.D });
            ControllerMapping.Add(Func.Up, new ControlSource() { DeviceIndex = 0, DeviceType = typeof(Keyboard), ControlIndex = (int)Key.E });
            ControllerMapping.Add(Func.Down, new ControlSource() { DeviceIndex = 0, DeviceType = typeof(Keyboard), ControlIndex = (int)Key.Q });
            ControllerMapping.Add(Func.View, new ControlSource() { DeviceIndex = 0, DeviceType = typeof(Keyboard), ControlIndex = (int)Key.V });
            ControllerMapping.Add(Func.FastMode, new ControlSource() { DeviceIndex = 0, DeviceType = typeof(Keyboard), ControlIndex = (int)Key.LShift });
            ControllerMapping.Add(Func.Jump, new ControlSource() { DeviceIndex = 0, DeviceType = typeof(Keyboard), ControlIndex = (int)Key.Space });
            ControllerMapping.Add(Func.Enter, new ControlSource() { DeviceIndex = 0, DeviceType = typeof(Keyboard), ControlIndex = (int)Key.F });
            ControllerMapping.Add(Func.SwitchView, new ControlSource() { DeviceIndex = 0, DeviceType = typeof(Keyboard), ControlIndex = (int)Key.V });
        }

        public static void Update()
        {
            MouseState ms = Mouse.GetState();
            int Width = Screen.PrimaryScreen.Bounds.Width;
            int Height = Screen.PrimaryScreen.Bounds.Height;
            if (((Height / 2) - ms.Y) > 0 && ((Width / 2) - ms.X) > 0)
            {
                MouseX = (Width / 2) / ((Width / 2) - ms.X);
                MouseY = (Height / 2) / ((Height / 2) - ms.Y);
                MouseScroll = ms.WheelPrecise;
            }

            foreach (var key in Controller.ControllerMapping.Keys)
            {
                ControlSource source = ControllerMapping[key];
                source.PrevState = source.CurrentState;
                if (source.DeviceType == typeof(Keyboard))
                {
                    source.CurrentState = Keyboard.GetState(source.DeviceIndex).IsKeyDown((short)source.ControlIndex)?1:0;
                }
                else if (source.DeviceType == typeof(Mouse))
                {
                    MouseState deviceState = Mouse.GetState();
                    switch (source.ControlIndex)
                    {
                        case 0: source.CurrentState = MouseX; break;
                        case 1: source.CurrentState = MouseY; break;
                        case 2: source.CurrentState = deviceState.WheelPrecise; break;
                        case 3: source.CurrentState = deviceState.LeftButton == OpenTK.Input.ButtonState.Pressed ? 1 : 0; break;
                        case 4: source.CurrentState = deviceState.MiddleButton == OpenTK.Input.ButtonState.Pressed ? 1 : 0; break;
                        case 5: source.CurrentState = deviceState.RightButton == OpenTK.Input.ButtonState.Pressed ? 1 : 0; break;
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
            }
        }

        public static float Val(Func function)
        {
            return ControllerMapping[function].CurrentState;
        }

        public static bool In(Func function)
        {
            return ControllerMapping[function].CurrentState == 1;
        }

        public static bool Pressed(Func function)
        {
            return (ControllerMapping[function].PrevState == 0 && In(function));
        }
    }
}
