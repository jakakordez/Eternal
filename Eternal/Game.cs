using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using EGE;

namespace Eternal
{
    class Game:GameWindow
    {
        Screen currentScreen;
        double frames, time;

        public Game()
        {
            this.Title = "Eternal";
            WindowState = WindowState.Maximized;
            VSync = VSyncMode.On;
            System.Windows.Forms.Cursor.Hide();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            currentScreen = new Main();
            currentScreen.Load();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (Keyboard[OpenTK.Input.Key.Delete]) System.Diagnostics.Debugger.Break();
            if (Keyboard[OpenTK.Input.Key.Escape]) Exit();
            currentScreen.Update(e);
            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            
            currentScreen.Focused = Focused;
            currentScreen.Draw();
            base.OnRenderFrame(e);
            SwapBuffers();
            time += e.Time;
            frames++;
            if (time > 1){
                Title = Math.Round(frames / time, 2) + " FPS";
                frames = 0;
                time = 0.0001;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            currentScreen.Resize(Width, Height);
            base.OnResize(e);
        }
    }
}
