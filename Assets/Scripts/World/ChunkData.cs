using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Koxel.World
{
    public class ChunkData
    {
        public int[] coords;
        public List<TileData> tiles;

        public ChunkData(Vector3 coords)
        {
            this.coords = new int[3];
            this.coords[0] = (int)coords.x;
            this.coords[1] = (int)coords.y;
            this.coords[2] = (int)coords.z;
        }
        public ChunkData(Chunk chunk)
        {
            this.coords = new int[3];
            this.coords[0] = (int)chunk.coords.x;
            this.coords[1] = (int)chunk.coords.y;
            this.coords[2] = (int)chunk.coords.z;
            this.tiles = new List<TileData>();
            foreach (HexTile tile in chunk.tiles.Values)
            {
                this.tiles.Add(new TileData(tile));
            }
        }
        public ChunkData(int[] coords, List<TileData> tiles)
        {
            this.coords = coords;
            this.tiles = tiles;
        }
        public ChunkData()
        {
            this.tiles = new List<TileData>(); ;
            this.coords = new int[3];
        }
    }
}
