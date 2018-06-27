using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Koxel.Modding
{
    public class TileType : IModComponent
    {
        public string tag { get; private set; }
        public string display { get; private set; }

        public Color color;

        public TileType(string tag, string display, float[] color)
        {
            this.tag = tag;
            this.display = display;
            this.color = new Color(color[0], color[1], color[2], color[3]);
        }
    }
}
