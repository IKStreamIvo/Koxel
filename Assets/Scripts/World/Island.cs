using Koxel.Modding;

namespace Koxel.World{
    public class Island{
        public int seed;
        public Biome biome;
        public int width, height;
        public float stepSize;

        public Island(int seed, Biome biome, int width, int height, float stepSize = 0.25f){
            this.seed = seed;
            this.biome = biome;
            this.width = width;
            this.height = height;
            this.stepSize = stepSize;
        }
    }
}