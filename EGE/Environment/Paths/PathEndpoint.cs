using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace EGE.Environment.Paths
{
    public class PathEndpoint
    {
        public ulong ObjectID { get; set; }
        public int NodeID { get; set; }

        public PathEndpoint() { }

        public PathEndpoint(ulong objectID, int node)
        {
            ObjectID = objectID;
            NodeID = node;
        }

        public Node getPosition(ObjectManager objects)
        {
            /*if (objects.ObjectReferences.ContainsKey(ObjectID)){
                Object o = objects.Objects.Get(objects.ObjectReferences[ObjectID].Object);
                if(o.RoadEndpoints.Length > NodeID)
                {
                    Node relative = o.RoadEndpoints[NodeID];
                    Node bs = objects.ObjectReferences[ObjectID].Position;
                    Node result = new Node();
                    Matrix4 r = relative.CreateTransform() * bs.CreateTransform();
                    result.Location = r.ExtractTranslation();
                    Quaternion a = r.ExtractRotation();
                    result.Rotation = new Vector3(0, (float)Math.Asin(-2.0 * (a.X * a.Z - a.W * a.Y)), 0);
                    return result;
                }
            }*///todo

            return null;
        }
    }
}
