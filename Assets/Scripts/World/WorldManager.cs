using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Koxel.Tech;

namespace Koxel.World
{
    public class WorldManager : MonoBehaviour
    {
        public WorldGenerator Generator; 
        public Dictionary<Vector3, Chunk> Chunks;
        public HexData HexData { get; private set; }


        private void Awake()
        {
            HexData = new HexData(Game.GameConfig.hexSize);
            Chunks = new Dictionary<Vector3, Chunk>();
            Generator = new WorldGenerator(Chunks);
        }

        private void Update()
        {
            Generator.Update();
        }
    }
}
