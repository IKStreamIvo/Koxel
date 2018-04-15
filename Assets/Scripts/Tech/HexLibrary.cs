using System;
using System.Collections.Generic;

namespace Koxel.Tech
{
    public class HexData
    {
        public float Size { get; private set; }
        public float Height { get; private set; }
        public float Width { get; private set; }

        public HexData(float hexSize)
        {
            Size = hexSize;

            Height = Size * 2f;
            Width = (float)Math.Sqrt(3f) / 2f * Height;
        }
    }
}