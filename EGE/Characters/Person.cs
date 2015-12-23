using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using OpenTK;

namespace EGE.Characters
{
    class Person : Character
    {
        static CameraDefinition defaultCameraDefinition = new CameraDefinition()
        {
            Distance = 10,
            FPV = true,
            Offset = Vector3.Zero,
            ViewAngle = Vector2.One,
            Style = DrawingStyle.Normal
        };

        public override void Draw()
        {
            
        }

        public override void Update(float elaspedTime)
        {
            
        }
    }
}
