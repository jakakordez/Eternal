﻿using System;
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

        Vehicles.Vehicle car;

        public static bool StaticView;
        
        public static DiscreteDynamicsWorld DynamicsWorld;

        CollisionDispatcher dispatcher;
        DbvtBroadphase broadphase;
        CollisionConfiguration collisionConf;

        public World(bool StaticView)
        {
            World.StaticView = StaticView;
            CurrentMap = new Map();
            PrimaryCamera = new Camera();
            MeshCollection = new MeshCollector();

            if (StaticView) MainCharacter = new Characters.DebugView();
            else
            {
                // collision configuration contains default setup for memory, collision setup
                collisionConf = new DefaultCollisionConfiguration();
                dispatcher = new CollisionDispatcher(collisionConf);

                broadphase = new DbvtBroadphase();
                DynamicsWorld = new DiscreteDynamicsWorld(dispatcher, broadphase, null, collisionConf);
                DynamicsWorld.Gravity = new Vector3(0, -9.81f, 0);

                MainCharacter = new Characters.Person(new Vector3(683, 10, 274));

                car = new Vehicles.Car();
                (car as Vehicles.Car).Load();
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
        public void Build()
        {
            //CurrentMap.CurrentTerrain.Roads.AsParallel().ForAll(r => r.Build());
            
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

            MainCharacter.Draw();
            

            CurrentMap.CurrentTerrain.Draw();

            (car as Vehicles.Car).Draw();
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
