using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;

namespace EGE.Environment.Paths
{
    public class Junction
    {
        public string ObjectMesh { set; get; }

        public NodeReference[] RoadEndpoints { set; get; }
        
        public Junction()
        {
            ObjectMesh = "";
        }

        public void Load()
        {

        }
    }
}
