using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Koxel.Tech;
using SimplexNoise;

namespace Koxel.World
{
    public class _WorldManager : MonoBehaviour
    {
        public WorldGenerator Generator; 
        public Dictionary<Vector3, Chunk> Chunks;
        //public Dictionary<Vector3, HexTile> Tiles;
        public HexData HexData { get; private set; }
        public Simplex simplex;

        int octaves = 4;
        float lacunarity = 6f;
        float persistance = 0.5f;
        float scale = 25f;
        Vector2[] octaveOffsets;

        private void Awake()
        {
            HexData = new HexData(Game.GameConfig.hexSize);
            Chunks = new Dictionary<Vector3, Chunk>();
            //Tiles = new Dictionary<Vector3, HexTile>();
            Generator = new WorldGenerator(Chunks);
            
            int seed = Random.Range(1, 1000000);
            System.Random random = new System.Random(seed);
            octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = random.Next(-100000, 100000);
                float offsetY = random.Next(-100000, 100000);
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }
            simplex = new Simplex(seed);
        }

        private void Update()
        {
            Generator.Update();
        }

        public float HeightMap(HexTile tile)
        {
            float height = 0;

            float x = (tile.coords.x);
            float y = (tile.coords.y);

            for (int octave = 0; octave < octaves; octave++)
            {
                float frequency = Mathf.Pow(lacunarity, octave);
                float amplitude = Mathf.Pow(persistance, octave);

                float sampleX = x / scale * frequency + octaveOffsets[octave].x;
                float sampleY = y / scale * frequency + octaveOffsets[octave].y;

                float noise = (float)simplex.Evaluate(x, y) * 2f - 1f;
                height += noise * amplitude;
            }


            /*float noise100 = (float)simplex.Evaluate(x / 100.0F, y / 100.0F);
            float noise1000 = (float)simplex.Evaluate(x / 1000.0F, y / 1000.0F);
            float noise10000 = (float)simplex.Evaluate(x / 10000.0F, y / 10000.0F);

            /*float ground = noise100 * 0.5F + noise1000 * 0.3F + noise10000 * 0.2F;

            float noiseM1 = (float)simplex.Evaluate(0.25F + x / 80.0F, 0.25F + y / 80.0F);
            float noiseM2 = (float)simplex.Evaluate(0.25F + x / 80.0F, 0.25F + y / 80.0F);
            float noiseM = Mathf.Sqrt(noiseM1) * Mathf.Sqrt(noiseM2);
            noiseM *= 2;
            float mountain = 0;
            if (noiseM > 0.4)
            {
                mountain = (noiseM - 0.4f) * 2f;
            }

            float noiseN10 = (float)simplex.Evaluate(x / 5.0F, y / 5.0F);
            float noiseN = (float)simplex.Evaluate(0.25F + x / 25.0F, 0.25F + y / 25.0F);
            float noise = 0;
            if (noiseN > 0.55)
            {
                noise = Mathf.Max(0, noiseN10 - 0.5f) * 0.1f;
            }


            float noiseF = (float)simplex.Evaluate(0.85F + x / 40.0F, 0.85F + y / 40.0F);
            float ratioNoise = 1;
            if (noiseF > 0.5)
            {
                ratioNoise = 1 - Mathf.Max(0, (noiseF - 0.5f) * 0.5f);
            }
            
            height = ratioNoise * (ground + mountain) + noise;*/

            //height = noise100 + noise1000 + noise10000;
            //height *= 100f;
            //height -= 100f;
            //Debug.Log(height);
            return height;
        }

        public float HeightMap2(HexTile tile)
        {
            int x = (int)tile.coords.x;
            int y = (int)tile.coords.y;

            float noise = 0;

            ///freq: noise(freq * x, freq * y) [zooming]
            ///redistribution: noise = Math.Pow(noise, redis) [valleys]
            ///ocataves: noise at different frequencies

            noise += (float)simplex.Evaluate(x / 5f, y / 5f) / 20f;
            noise += (float)simplex.Evaluate(x / 25f, y / 25f) / 6f;
            noise += (float)simplex.Evaluate(x / 50f, y / 50f);
            noise += (float)simplex.Evaluate(x / 100f, y / 100f);
            //noise += simplex.Evaluate(x / 750f, y / 750f)*1;

            noise = noise / 4f;
            noise = noise - 0.8f;

            if (noise >= -0.5f)
                noise = (noise * 1.75f) + 0.38f; //increases height above -0.5, set it to * 10 to see where it is

            if (noise >= 0.1)
                noise = (noise * 1.75f) - 0.07f;

            noise = Mathf.Pow(noise, 2f);

            return (float)noise * 50f + 50f;
        }

    }
}
