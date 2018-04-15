using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Koxel.World;

namespace Koxel.World
{
    public class TileData
    {
        public string biome;
        public string tileType;

        public TileData(HexTile tile)
        {
            if (tile != null)
            {
                biome = tile.biome;
                tileType = tile.tileType;
            }
        }
    }
}
