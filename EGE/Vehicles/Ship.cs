using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using BulletSharp;
using OpenTK.Graphics.OpenGL;

namespace EGE.Vehicles
{
    public class Ship:Vehicle
    {
        protected float Thrust, Steering;
        public float SteeringClamp { get; set; }
        public override void Load(Vector3 Location)
        {
            CollisionShape m = Resources.GetMeshCollisionShape(lowPolyVehicleMesh);

            vehicleBody = World.CreateRigidBody(Mass, Matrix4.CreateTranslation(Location), (m != null)? m : (CollisionShape)new BoxShape(5));
            vehicleBody.LinearFactor = new Vector3(1, 0, 1);
            vehicleBody.AngularFactor = new Vector3(0, 1, 0);
            vehicleBody.ActivationState = ActivationState.DisableDeactivation;
            World.DynamicsWorld.AddRigidBody(vehicleBody);
        }

        public override void Draw(Vector3 eye)
        {
            Matrix4 trans = World.WorldMatrix;

            trans = vehicleBody.CenterOfMassTransform * World.WorldMatrix;
            GL.LoadMatrix(ref trans);
            Resources.DrawMesh(vehicleMesh);
        }

        public override void Update()
        {
            base.Update();
            vehicleBody.ApplyCentralForce(new Vector3(50, 0, 0));
            /*raycastVehicle.ApplyEngineForce(Thrust * 5000, 2);
            raycastVehicle.ApplyEngineForce(Thrust * 5000, 3);
            raycastVehicle.SetSteeringValue(Steering, 0);
            raycastVehicle.SetSteeringValue(Steering, 1);
            //raycastVehicle.SetBrake(Brake*100, 0);
            //raycastVehicle.SetBrake(Brake*100, 1);
            raycastVehicle.SetBrake(Brake * 400, 2);
            raycastVehicle.SetBrake(Brake * 400, 3);

            SteeringWheel.MeshRotation = new Vector3(SteeringWheel.MeshRotation.X, SteeringWheel.MeshRotation.Y, -5 * Steering);
            VelocityNeedle.SetZ(raycastVehicle.CurrentSpeedKmHour / 50);
            RPM = (float)((Math.Sin(Clutch * MathHelper.PiOver2) * 1000) + ((1 - Math.Sin(Clutch * MathHelper.PiOver2)) * 0));
            RPMNeedle.SetZ(RPM);*/
        }

        public override void HandleInput()
        {
            if (Controller.In(Func.Acceleration) && Thrust < 1)
            {
                Thrust += 0.1f;
            }
            else Thrust *= 0.25f;
            if (Controller.In(Func.Brake) && Thrust > -1)
            {
                Thrust -= 0.1f;
            }
            else Thrust *= 0.25f;
            if (Controller.In(Func.Left) && Steering < SteeringClamp)
            {
                Steering += 0.01f;
                if (Steering > SteeringClamp) Steering = SteeringClamp;
            }
            else if (Controller.In(Func.Right) && Steering > -SteeringClamp)
            {
                Steering -= 0.01f;
                if (Steering < -SteeringClamp) Steering = -SteeringClamp;
            }
            if (!Controller.In(Func.Right) && !Controller.In(Func.Left)) Steering *= 0.8f;
        }
    }
}
