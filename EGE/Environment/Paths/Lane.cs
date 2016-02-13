using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Environment.Paths
{
    public class Lane
    {
        public float LaneOffset { get; set; }
        public NodeReference[] LaneWaypoints;

        public Lane()
        {
            LaneWaypoints = new NodeReference[0];
        }

        public Lane(float Offset)
        {
            LaneOffset = Offset;

        }

    }
}
