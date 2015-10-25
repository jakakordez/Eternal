using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Environment.Paths
{
    public class Path
    {
        public PathNode[] PathNodes { get; set; }
        public float Width { get; set; }

        public Path()
        {
            PathNodes = new PathNode[0];
        }
    }
}
