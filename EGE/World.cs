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
using EGE.Vehicles;

namespace EGE
{
    public class World
    {
        public Map CurrentMap { get; set;}
        public Characters.Character MainCharacter;
        public static Matrix4 WorldMatrix;
        
        public static DiscreteDynamicsWorld DynamicsWorld;

        CollisionDispatcher dispatcher;
        DbvtBroadphase broadphase;
        CollisionConfiguration collisionConf;

        public World(bool StaticView)
        {
            Graphics.StaticView = StaticView;
            CurrentMap = new Map();

            if (StaticView) MainCharacter = new Characters.DebugView();
            else
            {
                // collision configuration contains default setup for memory, collision setup
                collisionConf = new DefaultCollisionConfiguration();
                dispatcher = new CollisionDispatcher(collisionConf);

                broadphase = new DbvtBroadphase();
                DynamicsWorld = new DiscreteDynamicsWorld(dispatcher, broadphase, null, collisionConf);
                DynamicsWorld.Gravity = new Vector3(0, -9.81f, 0);

                MainCharacter = new Characters.Person(new Vector3(403, 5, 274));
                DynamicsWorld.DebugDrawer = new Tools.PhysicsDebugDrawer();
                
            }
        }

        public void Init()
        {
            Graphics.Init();
            Controller.InitController();
        }

        public void LoadData(string Path)
        {
            Resources.LoadResources(Path + "\\Map");
            Tools.Contruction.Load(Path + "\\Map", CurrentMap);
            if (Graphics.StaticView) ((Characters.DebugView)MainCharacter).Load();
        }
        public void Build()
        {
            //CurrentMap.CurrentTerrain.Roads.AsParallel().ForAll(r => r.Build());
            
            foreach (Environment.Paths.Road r in CurrentMap.Roads)
            {
                if(r.Points.Length > 1) r.Build(CurrentMap.ObjectCollection);
            }
            foreach (var item in CurrentMap.StaticModels)
            {
                item.Load();
            }
            CurrentMap.TerrainHeightfield.Load();
            foreach (var f in CurrentMap.Forests)
            {
                f.Build(CurrentMap.TerrainHeightfield);
            }

            if (!Graphics.StaticView)
            {
                CurrentMap.ObjectCollection.Load();
                /*var car = Vehicles.Vehicles.getKey("Car/Volkswagen/Polo"); 
                car.Load(new Vector3(693, 10, 284));
                VehicleList.Add(car);
                /*car = Vehicles.Vehicles.getKey("Car/BMW/M3 E90");
                (car as Vehicles.Car).Load(new Vector3(693, 15, 294));
                VehicleList.Add(car);
                car = Vehicles.Vehicles.getKey("Ship/Ferry/Guarda");
                (car as Vehicles.Ship).Load(new Vector3(670, 5, 200));
                VehicleList.Add(car);*/
                CurrentMap.VehicleCollection.spawnVehicle("Polo", new Vector3(693, 10, 284));
                //CurrentMap.VehicleCollection.spawnVehicle("BMW", new Vector3(693, 15, 294));
                /*for(int i = 0; i < 2; i++)
                {
                    CurrentMap.VehicleCollection.spawnVehicle("Polo", new Vector3(650-i*5, 10, 284));
                }*/
            }
        }
        
        public void SaveData(string Path)
        {
            if (!Directory.Exists(Path + "\\Map")) Directory.CreateDirectory(Path + "\\Map");
            Tools.Contruction.Save(Path + "\\Map", CurrentMap);
        }

        public void Update(bool focused, float elaspedTime)
        {
            if (focused)
            {
                if (!Graphics.StaticView)
                {
                    foreach (var item in CurrentMap.VehicleCollection.VehicleInstances.GetNodes())
                    {
                        ((Vehicle)item.Value).Update();
                    }
                    DynamicsWorld.StepSimulation(elaspedTime);
                }
                Controller.Update();
                KeyboardState keyboardState = Keyboard.GetState();
                MainCharacter.Update(elaspedTime, CurrentMap);
            }
        }

        public void Resize(float Width, float Height)
        {
            Graphics.Resize(Width, Height);
        }

        public void Draw(bool Focused)
        {
            GL.ClearColor(Color.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Graphics.SetProjection();

            MainCharacter.Draw();
            if (Graphics.EditMesh == null)
            {
                if (DynamicsWorld != null) DynamicsWorld.DebugDrawWorld();
                CurrentMap.Draw(MainCharacter.GetEye());
            }
            else Resources.DrawMesh(Graphics.EditMesh);
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
