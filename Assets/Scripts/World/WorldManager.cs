using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Koxel.Tech;
using SimplexNoise;

namespace Koxel.World
{
    public class WorldManager : MonoBehaviour
    {
        public IslandGenerator Generator; 
        public HexData HexData { get; private set; }

        private void Awake()
        {
            Generator = new IslandGenerator();
            HexData = new HexData(Game.GameConfig.hexSize);
        }

        public void CreateIsland(){
            int seed = Random.Range(1, 1000000);
            Island startIsland = new Island(seed, 128, 128);
            Generator.Generate(startIsland);
        }

        private void Update()
        {
            //Generator.Update();
        }
    }
}
