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
        public Game()
        {
            this.Title = "Eternal";
            WindowState = WindowState.Fullscreen;
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
            currentScreen.Focuesed = Focused;
            currentScreen.Draw();
            base.OnRenderFrame(e);
            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            currentScreen.Resize(Width, Height);
            base.OnResize(e);
        }
    }
}
