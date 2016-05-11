using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace EGE.Characters
{
    public abstract class Character
    {
        protected Camera[] CameraList;
        protected int CurrentCamera;

        public Character()
        {
            CameraList = new Camera[0];
            CurrentCamera = 0;
        }

        public abstract Vector3 GetEye();

        public abstract void Update(float elaspedTime, Map map);

        public abstract void Draw();
    }
}
