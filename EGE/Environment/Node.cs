using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace EGE.Environment
{
    public class Node
    {
        public Vector3 NodeLocation { get; set; }

        public Node() { NodeLocation = Vector3.Zero; }
        public Node(Vector3 Location) { NodeLocation = Location; }
    }
}
