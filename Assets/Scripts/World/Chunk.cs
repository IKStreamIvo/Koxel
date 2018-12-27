using Koxel;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Koxel.World
{
    public class Chunk : MonoBehaviour
    {
        bool hasGenerated;
        public GameObject tilePrefab;
        public Vector3 coords;
        public Dictionary<Vector3, HexTile> tiles;

        [Header("Debug")]
        public List<Vector3> tileCoordsDEBUG;
        public List<HexTile> tileDEBUG;

        void Awake()
        {
            if (!hasGenerated)
            {
                tileCoordsDEBUG = new List<Vector3>();
                tileDEBUG = new List<HexTile>();
                GenerateNew();
            }
        }

        private void Start()
        {

        }

        public void Generate()
        {
            tileDEBUG.AddRange(tiles.Values);
            tileCoordsDEBUG.AddRange(tiles.Keys);

            for (int r = -Game.GameConfig.chunkSize / 2; r <= Game.GameConfig.chunkSize / 2; r++)
            {
                for (int q = -Game.GameConfig.chunkSize / 2; q <= Game.GameConfig.chunkSize / 2; q++)
                {
                    GameObject tileGO = tiles[new Vector3(r, q, -r - q)].gameObject;
                    int xr = (int)(r + Game.GameConfig.chunkSize * coords.x);
                    int yq = (int)(q + Game.GameConfig.chunkSize * coords.y);

                    HexTile tile = tileGO.GetComponent<HexTile>();
                    tile.coords = new Vector3(xr, yq, -xr - yq);
                    tile.chunk = this;
                    tile.biome = "randomBiome";
                    tile.tileType = "randomTileType";

                    Vector3 pos = new Vector3(
                        tileGO.transform.localPosition.x,
                        0,//Game.World.HeightMap(tile),
                        tileGO.transform.localPosition.z
                    );
                    tileGO.transform.localPosition = pos;
                    tileGO.name = "Tile (" + xr + ", " + yq + ")";
                    
                    /*if (pos.y < Game.World.waterThreshold)
                    {
                        tile.SetColor(Game.World.water);
                    }
                    else if (pos.y < Game.World.grassThreshold)
                    {
                        tile.SetColor(Game.World.grass);
                        if (Random.Range(0, Game.World.generalTileAssetChance) == 0)
                        {
                            List<TileAsset> tileAssets = Game.World.tileAssets;
                            List<int> usedRots = new List<int>();
                            usedRots.Add(-1);
                            for (int i = 0; i < Random.Range(0, tileAssets.Count) || i < Game.World.maxAssetsPerTile; i++)
                            {
                                TileAsset asset = tileAssets[Random.Range(0, tileAssets.Count)];
                                if (Random.Range(0, asset.chance) == 0)
                                {
                                    GameObject assetGO = Instantiate(asset.gameObject, tile.transform);
                                    assetGO.name.Replace("(Clone)", "");
                                    float scale = Random.Range(asset.sizeRange.x, asset.sizeRange.y);
                                    assetGO.transform.localScale = new Vector3(scale, scale, scale);
                                    int rotation = -1;
                                    while (usedRots.Contains(rotation))
                                        rotation = Random.Range(0, 5);
                                    usedRots.Add(rotation);
                                    assetGO.transform.Rotate(new Vector3(0f, rotation * 60f, 0f));
                                    assetGO.GetComponent<TileAsset>().Setup(asset.name, asset.prefab, asset.chance, asset.sizeRange, asset.assetInteractions, asset.actionData);
                                }
                            }
                        }
                    }
                    else
                    {
                        tile.SetColor(Game.World.stone);
                    }*/
                }
            }
        }
        public void Generate(List<TileData> tileDatas)
        {
            //Debug.Log("Generate with tileDatas");

            //tiles = new Dictionary<Vector3, HexTile>();
            for (int r = -Game.GameConfig.chunkSize / 2; r <= Game.GameConfig.chunkSize / 2; r++)
            {
                for (int q = -Game.GameConfig.chunkSize / 2; q <= Game.GameConfig.chunkSize / 2; q++)
                {
                    GameObject tileGO = tiles[new Vector3(r, q, -r - q)].gameObject;
                    HexTile tile = tileGO.GetComponent<HexTile>();
                    tile.coords = new Vector3(r, q, -r - q) + this.coords;
                    tile.chunk = this;
                    tile.biome = tileDatas[0].biome;
                    tile.tileType = tileDatas[0].tileType;

                    Vector3 pos = new Vector3(
                        tileGO.transform.localPosition.x,
                        0,//Game.World.HeightMap(tile),
                        tileGO.transform.localPosition.z
                    );
                    tileGO.transform.localPosition = pos;
                    tileGO.transform.localScale = tileGO.transform.localScale * Game.World.HexData.Size;
                    tileGO.name = "Tile (" + q + ", " + r + ")";
                    tileDatas.RemoveAt(0);
                    /*if (pos.y < Game.World.waterThreshold)
                    {
                        tile.SetColor(Game.World.water);
                    }
                    else if (pos.y < Game.World.grassThreshold)
                    {
                        tile.SetColor(Game.World.grass);
                    }
                    else
                    {
                        tile.SetColor(Game.World.stone);
                    }*/
                }
            }
        }

        public void GenerateNew()
        {
            //Debug.Log("Generate new");

            tiles = new Dictionary<Vector3, HexTile>();
            for (int r = -Game.GameConfig.chunkSize / 2; r <= Game.GameConfig.chunkSize / 2; r++)
            {
                for (int q = -Game.GameConfig.chunkSize / 2; q <= Game.GameConfig.chunkSize / 2; q++)
                {

                    GameObject tileGO = Instantiate(tilePrefab, transform);
                    tileGO.transform.localScale = tileGO.transform.localScale * Game.World.HexData.Size;
                    tileGO.name = "Tile (" + q + ", " + r + ")";
                    HexTile tile = tileGO.GetComponent<HexTile>();
                    tile.coords = new Vector3(r, q, -r - q) + this.coords;
                    tile.chunk = this;
                    tile.biome = "randomBiome";
                    tile.tileType = "randomTileType";
                    //tile.SetColor(new Color(0, .5f, 0, .5f));
                    tiles.Add(new Vector3(r, q, -r - q), tile);

                    Vector3 pos = new Vector3(
                        r * Game.World.HexData.Width + q * (.5f * Game.World.HexData.Width),
                        0,//Game.World.HeightMap(tile),
                        q * (Game.World.HexData.Height * .75f)
                    );

                    tileGO.transform.localPosition = pos;
                }
            }
            hasGenerated = true;
        }
        public void GenerateNew(List<TileData> tileDatas)
        {
            tiles = new Dictionary<Vector3, HexTile>();
            for (int r = -Game.GameConfig.chunkSize / 2; r <= Game.GameConfig.chunkSize / 2; r++)
            {
                for (int q = -Game.GameConfig.chunkSize / 2; q <= Game.GameConfig.chunkSize / 2; q++)
                {
                    GameObject tileGO = Instantiate(tilePrefab, transform);
                    tileGO.transform.localScale = tileGO.transform.localScale * Game.World.HexData.Size;
                    tileGO.name = "Tile (" + q + ", " + r + ")";
                    HexTile tile = tileGO.GetComponent<HexTile>();
                    tile.coords = new Vector3(r, q, -r - q) + this.coords;
                    tile.chunk = this;
                    tile.biome = tileDatas[0].biome;
                    tile.tileType = tileDatas[0].tileType;
                    tileDatas.RemoveAt(0);
                    //tile.SetColor(new Color(0, .5f, 0, .5f));
                    tiles.Add(new Vector3(r, q, -r - q), tile);

                    Vector3 pos = new Vector3(
                        r * Game.World.HexData.Width + q * (.5f * Game.World.HexData.Width),
                        0,//Game.World.HeightMap(tile),
                        q * (Game.World.HexData.Height * .75f)
                    );


                    tileGO.transform.localPosition = pos;

                }
            }
            hasGenerated = true;
        }
    }
}
