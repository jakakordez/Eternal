﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using OpenTK;

namespace EGE.Environment
{
    public class ObjectReference
    {
        public RigidBody CollisionObject;
        public Node Position{ get; set; }
        public string Object { get; set; }

        public ObjectReference():base()
        {
            Position = new Node();
            Object = "";
        }
    }
}
