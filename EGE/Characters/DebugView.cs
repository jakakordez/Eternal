using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using System.IO.Compression;

namespace EGE.Characters
{
    public class DebugView:Character
    {
        Vector3 centerPoint = new Vector3(673, 5, 274);
        bool inn = false;
        static CameraDefinition defaultCameraDefinition = new CameraDefinition()
        {
            Distance = 10,
            FPV = true,
            Offset = Vector3.Zero,
            ViewAngle = Vector2.One,
            Style = DrawingStyle.Wireframe
        };

        public Vector3 PointerLocation = new Vector3();
        Meshes.Mesh PointerMesh;

        public DebugView()
        {
            CameraList = new Camera[] { new FirstPersonCamera(defaultCameraDefinition), new TopDownCamera(defaultCameraDefinition)};
            CurrentCamera = 0;
        }

        public void Load()
        {
            PointerMesh = new Meshes.Mesh();
            PointerMesh.LoadMTL(new System.IO.MemoryStream(EGEResources.pointermtl));
            PointerMesh.LoadMesh(new ZipArchive(new System.IO.MemoryStream(EGEResources.pointermesh), ZipArchiveMode.Read));
        }

        public override void Update(float elaspedTime, Map map)
        {
            CameraList[0].Update();

            Matrix4 Movement = Matrix4.Identity;
            
            float sp = (Controller.Val(Func.FastMode) * 1) + 0.5f;
            Vector3 forward = Vector3.UnitX;
            Vector3 right = Vector3.UnitZ;
            if(CurrentCamera > 0)
            {
                forward = -Vector3.UnitZ;
                right = Vector3.UnitX;
            }
            Movement *= Matrix4.CreateTranslation(forward * Controller.Val(Func.Forward) * sp);
            Movement *= Matrix4.CreateTranslation(forward * Controller.Val(Func.Backward) * -sp);
            Movement *= Matrix4.CreateTranslation(right * Controller.Val(Func.Left) * -sp);
            Movement *= Matrix4.CreateTranslation(right * Controller.Val(Func.Right) * sp);
            Movement *= Matrix4.CreateTranslation(new Vector3(0, Controller.Val(Func.Up) * sp, 0));
            Movement *= Matrix4.CreateTranslation(new Vector3(0, Controller.Val(Func.Down) * -sp, 0));
            if (CurrentCamera == 0) Movement *= Matrix4.CreateRotationY(CameraList[0].Orientation.Y);
            centerPoint += Movement.ExtractTranslation();

            if (Controller.In(Func.View) && !inn)
            {
                inn = true;
                if (Settings.CurrentDrawingMode == Settings.DrawingModes.Textured) Settings.CurrentDrawingMode = Settings.DrawingModes.Wireframe;
                else Settings.CurrentDrawingMode = Settings.DrawingModes.Textured;
            }
            else if(!Controller.In(Func.View)) inn = false;
        }

        public override void Draw()
        {
            
            GL.MatrixMode(MatrixMode.Modelview);

            if (Controller.Pressed(Func.SwitchView)) CurrentCamera = (CurrentCamera + 1) % CameraList.Length;

            CameraList[CurrentCamera].GenerateLookAt(centerPoint);
            
            Matrix4 trans = Matrix4.CreateTranslation(PointerLocation) * World.WorldMatrix;
            GL.LoadMatrix(ref trans);
            PointerMesh.Draw();
            GL.LoadMatrix(ref World.WorldMatrix);
        }

        public void Navigate(Vector3 Point)
        {
            PointerLocation = Point;
            if(CurrentCamera == 0)
            {
                centerPoint = Point + new Vector3(-2, 2, 2);
                CameraList[0].Orientation = new Vector3(-MathHelper.PiOver4, MathHelper.PiOver4, -MathHelper.PiOver4);
            }
            else
            {
                centerPoint = Point+new Vector3(0, 10, 0);
            }
        }

        public override Vector3 GetEye()
        {
            return centerPoint;
        }
    }
}
