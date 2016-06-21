using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Environment
{
    public class Object : Indexed
    {
        public MeshReference ObjectMesh { get; set; }

        public Node[] RoadEndpoints { set; get; }

        public Object()
        {
            ObjectMesh = new MeshReference();
            RoadEndpoints = new Node[0];
        }
    }
}
