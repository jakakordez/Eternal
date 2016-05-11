using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace EGE.Tools
{
    public class PhysicsDebugDrawer : IDebugDraw
    {
        public static bool DrawRoads = false;
        DebugDrawModes currentMode = DebugDrawModes.None;//DebugDrawModes.DrawWireframe;
        public DebugDrawModes DebugMode
        {
            get
            {
                return currentMode;
            }

            set
            {
                currentMode = value;
            }
        }

        public void Draw3dText(ref Vector3 location, string textString)
        {
            throw new NotImplementedException();
        }

        public void DrawAabb(ref Vector3 from, ref Vector3 to, Color4 color)
        {
            throw new NotImplementedException();
        }

        public void DrawArc(ref Vector3 center, ref Vector3 normal, ref Vector3 axis, float radiusA, float radiusB, float minAngle, float maxAngle, Color4 color, bool drawSect)
        {
            throw new NotImplementedException();
        }

        public void DrawArc(ref Vector3 center, ref Vector3 normal, ref Vector3 axis, float radiusA, float radiusB, float minAngle, float maxAngle, Color4 color, bool drawSect, float stepDegrees)
        {
            throw new NotImplementedException();
        }

        public void DrawBox(ref Vector3 bbMin, ref Vector3 bbMax, Color4 color)
        {
            throw new NotImplementedException();
        }

        public void DrawBox(ref Vector3 bbMin, ref Vector3 bbMax, ref Matrix4 trans, Color4 color)
        {
            //throw new NotImplementedException();
            trans = trans * World.WorldMatrix;
            GL.LoadMatrix(ref trans);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(bbMin);
            GL.Vertex3(bbMin * new Vector3(1, -1, 1));
            GL.Vertex3(bbMin);
            GL.Vertex3(bbMin * new Vector3(-1, 1, 1));
            GL.Vertex3(bbMin);
            GL.Vertex3(bbMin * new Vector3(1, 1, -1));

            GL.Vertex3(bbMax);
            GL.Vertex3(bbMax * new Vector3(1, -1, 1));
            GL.Vertex3(bbMax);
            GL.Vertex3(bbMax * new Vector3(-1, 1, 1));
            GL.Vertex3(bbMax);
            GL.Vertex3(bbMax * new Vector3(1, 1, -1));

            GL.End();
        }

        public void DrawCapsule(float radius, float halfHeight, int upAxis, ref Matrix4 transform, Color4 color)
        {
            throw new NotImplementedException();
        }

        public void DrawCone(float radius, float height, int upAxis, ref Matrix4 transform, Color4 color)
        {
            throw new NotImplementedException();
        }

        public void DrawContactPoint(ref Vector3 pointOnB, ref Vector3 normalOnB, float distance, int lifeTime, Color4 color)
        {
            throw new NotImplementedException();
        }

        public void DrawCylinder(float radius, float halfHeight, int upAxis, ref Matrix4 transform, Color4 color)
        {
            throw new NotImplementedException();
        }

        public void DrawLine(ref Vector3 from, ref Vector3 to, Color4 color)
        {
            GL.LoadMatrix( ref World.WorldMatrix);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(from);
            GL.Vertex3(to);
            GL.End();
        }

        public void DrawLine(ref Vector3 from, ref Vector3 to, Color4 fromColor, Color4 toColor)
        {
            throw new NotImplementedException();
        }

        public void DrawPlane(ref Vector3 planeNormal, float planeConst, ref Matrix4 transform, Color4 color)
        {
            throw new NotImplementedException();
        }

        public void DrawSphere(ref Vector3 p, float radius, Color4 color)
        {
            throw new NotImplementedException();
        }

        public void DrawSphere(float radius, ref Matrix4 transform, Color4 color)
        {
            //throw new NotImplementedException();
        }

        public void DrawSpherePatch(ref Vector3 center, ref Vector3 up, ref Vector3 axis, float radius, float minTh, float maxTh, float minPs, float maxPs, Color4 color)
        {
            throw new NotImplementedException();
        }

        public void DrawSpherePatch(ref Vector3 center, ref Vector3 up, ref Vector3 axis, float radius, float minTh, float maxTh, float minPs, float maxPs, Color4 color, float stepDegrees)
        {
            throw new NotImplementedException();
        }

        public void DrawSpherePatch(ref Vector3 center, ref Vector3 up, ref Vector3 axis, float radius, float minTh, float maxTh, float minPs, float maxPs, Color4 color, float stepDegrees, bool drawSphere)
        {
            throw new NotImplementedException();
        }

        public void DrawTransform(ref Matrix4 transform, float orthoLen)
        {
            //throw new NotImplementedException();
        }

        public void DrawTriangle(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2, Color4 color, float __unnamed004)
        {
            throw new NotImplementedException();
        }

        public void DrawTriangle(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2, ref Vector3 __unnamed003, ref Vector3 __unnamed004, ref Vector3 __unnamed005, Color4 color, float alpha)
        {
            throw new NotImplementedException();
        }

        public void ReportErrorWarning(string warningString)
        {
            throw new NotImplementedException();
        }
    }
}
