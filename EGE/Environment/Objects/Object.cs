using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Environment
{
    public class Object : Indexed
    {
        public string ObjectMesh { set; get; }
        public string CollisionMesh { set; get; }

        public Node[] RoadEndpoints { set; get; }

        public Object()
        {
            ObjectMesh = "";
            CollisionMesh = "";
            RoadEndpoints = new Node[0];
        }
    }
}
