using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE
{
    public class Map
    {
        public Terrain CurrentTerrain { get; set; }
        string MapPath;

        public Map()
        {
            CurrentTerrain = new Terrain();
        }

        public void Load(World world, string mapPath)
        {
            MapPath = mapPath;
            CurrentTerrain.Load(world, mapPath + "\\Terrain.ege");
        }

        public void Save(string mapPath)
        {
            MapPath = mapPath;
            CurrentTerrain.Save(mapPath + "\\Terrain.ege");
        }

        public override string ToString()
        {
            return "Map";
        }
    }
}
