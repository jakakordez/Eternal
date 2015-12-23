using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Drawing;

namespace EGE
{
    public class World
    {
        private MeshCollector MeshCollection;

        public Map CurrentMap { get; set;}
        public Camera PrimaryCamera;
        Characters.Character MainCharacter;
        public static Matrix4 WorldMatrix;

        public static bool StaticView;
        
        private static DiscreteDynamicsWorld DynamicsWorld;

        CollisionDispatcher dispatcher;
        DbvtBroadphase broadphase;
        CollisionConfiguration collisionConf;

        public World(bool StaticView)
        {
            World.StaticView = StaticView;
            CurrentMap = new Map();
            PrimaryCamera = new Camera();
            MeshCollection = new MeshCollector();
            MainCharacter = new Characters.DebugView();

            if (!StaticView)
            {
                // collision configuration contains default setup for memory, collision setup
                collisionConf = new DefaultCollisionConfiguration();
                dispatcher = new CollisionDispatcher(collisionConf);

                broadphase = new DbvtBroadphase();
                DynamicsWorld = new DiscreteDynamicsWorld(dispatcher, broadphase, null, collisionConf);
                DynamicsWorld.Gravity = new Vector3(0, -9.81f, 0);
            }
        }

        public void Init()
        {
            Tools.Graphics.Init();
            Controller.InitController();
        }

        public void LoadData(string Path)
        {
            Tools.TextureManager.LoadTextures(Path + "\\Map");
            Tools.ResourceManager.LoadResources(Path + "\\Map");
            Tools.MeshManager.LoadMeshes(Path + "\\Map");
            Tools.Contruction.Load(Path + "\\Map", CurrentMap);
        }
        RigidBody kr;
        public void Build()
        {
            //CurrentMap.CurrentTerrain.Roads.AsParallel().ForAll(r => r.Build());
            if (!StaticView)
            {
                SphereShape krogla = new SphereShape(3);
                kr = CreateRigidBody(50, Matrix4.CreateTranslation(new Vector3(20, 100, 20)), krogla);
            }
            
            foreach (Environment.Paths.Road r in CurrentMap.CurrentTerrain.Roads)
            {
                if(r.RoadPath.Length > 1) r.Build();
            }
            foreach (var item in CurrentMap.CurrentTerrain.StaticModels)
            {
                item.Load();
            }
            CurrentMap.CurrentTerrain.TerrainHeightfield.Load();
        }

        public void SaveData(string Path)
        {
            if (!Directory.Exists(Path + "\\Map")) Directory.CreateDirectory(Path + "\\Map");
            Tools.Contruction.Save(Path + "\\Map", CurrentMap);
        }

        public void Update(bool focused, float elaspedTime)
        {
            /*if (k[OpenTK.Input.Key.Enter] && !e) Addball();
            e = k[OpenTK.Input.Key.Enter];
            
            Player.Update(elaspedTime, new Controller(k), CurrentMap, Player);
            for (int i = 0; i < Vehicles.Length; i++)
            {
                Vehicles[i].Update(elaspedTime, null, CurrentMap, Player);
            }*/
            if (focused)
            {
                if (!StaticView) DynamicsWorld.StepSimulation(elaspedTime);
                Controller.Update();
                KeyboardState keyboardState = Keyboard.GetState();
                MainCharacter.Update(elaspedTime);
            }
        }

        public void Resize(float Width, float Height)
        {

            Tools.Graphics.Resize(Width, Height);
        }

        public void Draw(bool Focused)
        {
            GL.ClearColor(Color.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Tools.Graphics.SetProjection();

            if(!StaticView) CurrentMap.CurrentTerrain.StaticModels[0].Center.Ref.Location = kr.CenterOfMassTransform.ExtractTranslation();

            MainCharacter.Draw();


            CurrentMap.CurrentTerrain.Draw();

            //    if (Focused) camera.Update(mouse);

            //    /*GL.LoadMatrix(ref lookat);
            //    Matrix4 t = CurrentMap.CurrentTerrain.ground.CenterOfMassTransform * lookat;
            //    GL.LoadMatrix(ref t);*/
            //    /*GL.Color4(Color.White);
            //    GL.BindTexture(TextureTarget.Texture2D, grass);
            //    GL.Begin(PrimitiveType.Quads);
            //    GL.TexCoord2(new Vector2(0, 0));
            //    float d = 64f;//5000f;
            //    GL.Vertex3(new Vector3(-d, 0, -d));
            //    GL.TexCoord2(new Vector2(0, 10000));
            //    GL.Vertex3(new Vector3(-d, 0, d));
            //    GL.TexCoord2(new Vector2(10000, 10000));
            //    GL.Vertex3(new Vector3(d, 0, d));
            //    GL.TexCoord2(new Vector2(10000, 0));
            //    GL.Vertex3(new Vector3(d, 0, -d));
            //    GL.End();*/
            //    //GL.LoadMatrix(ref WorldMatrix);

            //    /*GL.Begin(PrimitiveType.Lines);
            //    GL.Color4(OpenTK.Graphics.Color4.Red);
            //    DynamicsWorld.DebugDrawObject(Matrix4.Identity, CurrentMap.CurrentTerrain.ground.CollisionShape, OpenTK.Graphics.Color4.Red);
            //    GL.End();*/
            //    CurrentMap.Draw(ref MeshCollection, WorldMatrix, Player.body.CenterOfMassPosition);
            //    GL.Begin(PrimitiveType.Triangles);
            //    GL.Color4(OpenTK.Graphics.Color4.Red);
            //    for (int i = 0; i < ball.Count; i++)
            //    {
            //        Vector3 pos = ball[i].CenterOfMassPosition;
            //        GL.Vertex3(pos + new Vector3(0, 0.5f, 0));
            //        GL.Vertex3(pos + new Vector3(0, 0.5f, 0.5f));
            //        GL.Vertex3(pos + new Vector3(0, -0.5f, 0));

            //        GL.Vertex3(pos + new Vector3(0.5f, 0.5f, 0));
            //        GL.Vertex3(pos + new Vector3(0, 0.5f, 0.5f));
            //        GL.Vertex3(pos + new Vector3(0, 0.5f, 0));
            //    }
            //    GL.End();

            //    Player.Draw(WorldMatrix, ref MeshCollection);
            //    for (int i = 0; i < Vehicles.Length; i++)
            //    {
            //        Vehicles[i].Draw(WorldMatrix, ref MeshCollection);
            //    }
            //    GL.LoadMatrix(ref WorldMatrix);
            
        }

        public static RigidBody CreateRigidBody(float mass, Matrix4 startTransform, CollisionShape shape)
        {
            bool isDynamic = (mass != 0.0f);

            Vector3 localInertia = Vector3.Zero;
            if (isDynamic)
                shape.CalculateLocalInertia(mass, out localInertia);

            DefaultMotionState myMotionState = new DefaultMotionState(startTransform);

            RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape, localInertia);
            RigidBody body = new RigidBody(rbInfo);
            DynamicsWorld.AddRigidBody(body);
            return body;
        }
        /*public void ExitPhysics()
        {
            //remove/dispose constraints
            int i;
            for (i = DynamicsWorld.NumConstraints - 1; i >= 0; i--)
            {
                TypedConstraint constraint = DynamicsWorld.GetConstraint(i);
                DynamicsWorld.RemoveConstraint(constraint);
                constraint.Dispose();
            }

            //remove the rigidbodies from the dynamics world and delete them
            for (i = DynamicsWorld.NumCollisionObjects - 1; i >= 0; i--)
            {
                CollisionObject obj = DynamicsWorld.CollisionObjectArray[i];
                RigidBody body = obj as RigidBody;
                if (body != null && body.MotionState != null)
                {
                    body.MotionState.Dispose();
                }
                DynamicsWorld.RemoveCollisionObject(obj);
                obj.Dispose();
            }

            DynamicsWorld.Dispose();
            broadphase.Dispose();
            if (dispatcher != null) dispatcher.Dispose();
            collisionConf.Dispose();
        }*/
    }
}
