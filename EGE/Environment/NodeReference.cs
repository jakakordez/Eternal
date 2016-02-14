﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenTK;

namespace EGE.Environment
{
    public class NodeReference
    {
        [JsonIgnore]
        public Node Ref {
            get
            {
                return Nodes.GetNode(ID);
            }
            set
            {
                Nodes.SetNode(ID, value);
            }
        }

        
        public ulong ID
        {
            get; set;
        }

        public NodeReference()
        {
        }

        public NodeReference(OpenTK.Vector3 FromLocation)
        {
            ID = Nodes.AddNode(new Node(FromLocation));
        }

        public NodeReference(Node n)
        {
            ID = Nodes.AddNode(n);
        }

        public NodeReference(ulong id)
        {
            ID = id;
        }

        public Vector3 AbsPosition()
        {
            return Nodes.GetNodeLocation(ID);
        }

        public Vector3 AbsRotation()
        {
            return Nodes.GetNodeRotation(ID);
        }
    }
}
