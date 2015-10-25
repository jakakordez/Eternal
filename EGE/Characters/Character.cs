using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Characters
{
    abstract class Character
    {
        public abstract CameraDefinition CurrentCameraDefinition { get; set; }

        public abstract void Update(float elaspedTime, OpenTK.Input.KeyboardState keyboardState);

        public abstract void Draw();
    }
}
