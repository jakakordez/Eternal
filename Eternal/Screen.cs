﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Eternal
{
    abstract class Screen
    {
        public bool Focused;
        public abstract void Load();
        public abstract void Draw();
        public abstract void Update(FrameEventArgs e);
        public abstract void Resize(float Width, float Height);
        public abstract void Close();
    }
}
