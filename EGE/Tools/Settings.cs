using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE
{
    static class Settings
    {
        public static DrawingModes CurrentDrawingMode = DrawingModes.Debug;

        public enum DrawingModes
        {
            Debug,
            Wireframe,
            Full,
            Textured
        }
    }
}
