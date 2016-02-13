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
        bool Loaded;

        public Main()
        {
            currentWorld = new World(false);
        }

        public override void Load()
        {
            currentWorld.LoadData(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)+"\\mapa");
            currentWorld.Init();
            currentWorld.Build();
            Loaded = true;
        }
        public override void Draw()
        {
            if(Loaded) currentWorld.Draw(true);
        }

        public override void Update(FrameEventArgs e)
        {
            if(Loaded) currentWorld.Update(Focused, (float)e.Time);
        }

        public override void Resize(float Width, float Height)
        {
            currentWorld.Resize(Width, Height);
        }
    }
}
