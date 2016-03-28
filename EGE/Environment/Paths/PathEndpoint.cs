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
        public string ObjectID { get; set; }
        public int NodeID { get; set; }

        public PathEndpoint() { }

        public PathEndpoint(string objectID, int node)
        {
            ObjectID = objectID;
            NodeID = node;
        }

        public Node getPosition(ObjectManager objects)
        {
            if (objects.ObjectReferences.Contains(ObjectID)){
                Object o = (Object)objects.Objects.Get(((ObjectReference)objects.ObjectReferences.Get(ObjectID)).Object);
                if(o.RoadEndpoints.Length > NodeID)
                {
                    Node relative = o.RoadEndpoints[NodeID];
                    Node bs = ((ObjectReference)objects.ObjectReferences.Get(ObjectID)).Position;
                    Node result = new Node();
                    Matrix4 r = relative.CreateTransform() * bs.CreateTransform();
                    result.Location = r.ExtractTranslation();
                    Quaternion a = r.ExtractRotation();
                    result.Rotation = new Vector3(0, (float)Math.Asin(-2.0 * (a.X * a.Z - a.W * a.Y)), 0);
                    return result;
                }
            }

            return null;
        }
    }
}
