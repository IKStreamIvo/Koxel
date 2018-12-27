using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Koxel.Tech;
using SimplexNoise;
using Koxel.Modding;

namespace Koxel.World
{
    public class WorldManager : MonoBehaviour
    {
        public IslandGenerator Generator; 
        public HexData HexData { get; private set; }
        public Dictionary<Vector2Int, HexTile> islandMap;

        private void Awake()
        {
            Generator = new IslandGenerator();
            HexData = new HexData(Game.GameConfig.hexSize);
        }

        public void CreateIsland(){
            int seed = Random.Range(1, 1000000);
            Biome biome = (Biome)Game.ModManager.Components[ModManager.Component.Biome]["grassplains"];
            Island startIsland = new Island(13, biome, 64, 64);
            Generator.Generate(startIsland);
        }

        private void Update()
        {
            //Generator.Update();
        }
    }
}
