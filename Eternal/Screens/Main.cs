using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using EGE;

namespace Eternal
{
    class Main : Screen
    {
        World currentWorld;

        public Main()
        {
            currentWorld = new World(false);
        }

        public override void Load()
        {
            currentWorld.LoadData("C:\\Users\\jakak\\Desktop\\mapa");
            currentWorld.Init();
            currentWorld.Build();
        }
        public override void Draw()
        {
            currentWorld.Draw(true);
        }

        public override void Update(FrameEventArgs e)
        {
            currentWorld.Update(Focused, (float)e.Time);
        }

        public override void Resize(float Width, float Height)
        {
            currentWorld.Resize(Width, Height);
        }
    }
}
