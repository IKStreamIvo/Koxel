namespace Koxel.Modding
{
    public class Tile
    {
        TileSegment[] segments;

        public Tile(TileSegment[] segments){
            this.segments = segments;
        }
    }

    public class TileSegment
    {
        public TileType tileType;
        public float size;

        public TileSegment(TileType tileType, float size){
            this.tileType = tileType;
            this.size = size;
        }
    }
}