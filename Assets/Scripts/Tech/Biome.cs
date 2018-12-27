using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Koxel.Modding
{
    public class Biome : IModComponent
    {
        public string tag { get; private set; }
        public string display { get; private set; }

        public Elevation elevation;
        public Tile mainTile;

        public Biome(string tag, string display, Elevation elevation, Tile mainTile)
        {
            this.tag = tag;
            this.display = display;
            this.elevation = elevation;
        }
    }
}