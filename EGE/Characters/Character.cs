using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Characters
{
    abstract class Character
    {
        protected Camera[] CameraList;
        protected int CurrentCamera;

        public Character()
        {
            CameraList = new Camera[0];
            CurrentCamera = 0;
        }

        public abstract void Update(float elaspedTime);

        public abstract void Draw();
    }
}
