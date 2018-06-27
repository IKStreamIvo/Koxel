using System.Collections.Generic;
using System.Text;
using Koxel.Tech;
using UnityEngine;

namespace Koxel.World{
    public class IslandGenerator {
        HexData HexData;

        public IslandGenerator(){
            HexData = new HexData(Game.GameConfig.hexSize);
        }

        public Island Generate(Island island){
            SimplexNoise.Simplex simplex = new SimplexNoise.Simplex(island.seed);

            //Square
            /*for (int q = -island.width/2; q < island.width/2; q++)
            {
                for (int r = -island.width/2; r < island.height/2; r++)
                {
                    GameObject tileGO = Game.ObjectPooler.GetPooledObject("Tile");
                    tileGO.transform.SetParent(Game.World.transform);
                    tileGO.transform.localScale = tileGO.transform.localScale * HexData.Size;
                    tileGO.name = "Tile (" + r + ", " + q + ")";
                    HexTile tile = tileGO.GetComponent<HexTile>();
                    tileGO.SetActive(true);
                    Vector3 pos = new Vector3(
                        q * HexData.Width + r * (.5f * HexData.Width),
                        Noise(q, r, simplex, island.width, island.height),
                        r * (HexData.Height * .75f)
                    );
                    tileGO.transform.localPosition = pos;

                    //Get lowest neighbour
                    float lowestHeight = Mathf.Infinity;
                    if(Noise(q - 1, r + 1, simplex, island.width, island.height) < lowestHeight){
                        lowestHeight = Noise(q-1, r+1, simplex, island.width, island.height);
                    }
                    if(Noise(q, r + 1, simplex, island.width, island.height) < lowestHeight){
                        lowestHeight = Noise(q, r+1, simplex, island.width, island.height);
                    }
                    if(Noise(q + 1, r, simplex, island.width, island.height) < lowestHeight){
                        lowestHeight = Noise(q+1, r, simplex, island.width, island.height);
                    }
                    if(Noise(q + 1, r - 1, simplex, island.width, island.height) < lowestHeight){
                        lowestHeight = Noise(q+1, r-1, simplex, island.width, island.height);
                    }
                    if(Noise(q, r - 1, simplex, island.width, island.height) < lowestHeight){
                        lowestHeight = Noise(q, r-1, simplex, island.width, island.height);
                    }
                    if(Noise(q - 1, r, simplex, island.width, island.height) < lowestHeight){
                        lowestHeight = Noise(q-1, r, simplex, island.width, island.height);
                    }
                    if(lowestHeight == Mathf.Infinity){
                        lowestHeight = pos.y;
                    }
                    tile.SetSize((lowestHeight - pos.y) / 2f);
                    tile.SetColor(new Color(0f, .5f, 0f, 1f));
                }
            }*/
            //Hex
            /*for(int q = -island.width/2; q <= island.width/2; q++){
                int r1 = Mathf.Max(-island.width, -q - island.width);
                int r2 = Mathf.Min(island.width, -q + island.width);
                for(int r = r1; r <= r2; r++){
                    GameObject tileGO = Game.ObjectPooler.GetPooledObject("Tile");
                    tileGO.transform.SetParent(Game.World.transform);
                    tileGO.transform.localScale = tileGO.transform.localScale * HexData.Size;
                    tileGO.name = "Tile (" + r + ", " + q + ")";
                    HexTile tile = tileGO.GetComponent<HexTile>();
                    tileGO.SetActive(true);
                    Vector3 pos = new Vector3(
                        r * HexData.Width + q * (.5f * HexData.Width),
                        0f,
                        q * (HexData.Height * .75f)
                    );
                    tileGO.transform.localPosition = pos;
                }
            }*/
            
            List<GameObject> water = new List<GameObject>();
            //Generate Rectangle
            for (int r = -island.height/2; r < island.height/2; r++)
            {
                int r_offset = (int) Mathf.Floor(r/2f);
                for (int q = -r_offset - island.width/2; q < island.width/2 - r_offset; q++)
                {
                    GameObject tileGO = Game.ObjectPooler.GetPooledObject("Tile");
                    tileGO.transform.SetParent(Game.World.transform);
                    tileGO.transform.localScale = tileGO.transform.localScale * HexData.Size;
                    tileGO.name = "Tile (" + r + ", " + q + ")";
                    HexTile tile = tileGO.GetComponent<HexTile>();
                    tileGO.SetActive(true);
                    Vector3 pos = new Vector3(
                        q * HexData.Width + r * (.5f * HexData.Width),
                        Noise(q, r, simplex, island.width, island.height),
                        r * (HexData.Height * .75f)
                    );
                    tileGO.transform.localPosition = pos;

                    //Get lowest neighbour
                    float lowestHeight = Mathf.Infinity;
                    if(Noise(q - 1, r + 1, simplex, island.width, island.height) < lowestHeight){
                        lowestHeight = Noise(q-1, r+1, simplex, island.width, island.height);
                    }
                    if(Noise(q, r + 1, simplex, island.width, island.height) < lowestHeight){
                        lowestHeight = Noise(q, r+1, simplex, island.width, island.height);
                    }
                    if(Noise(q + 1, r, simplex, island.width, island.height) < lowestHeight){
                        lowestHeight = Noise(q+1, r, simplex, island.width, island.height);
                    }
                    if(Noise(q + 1, r - 1, simplex, island.width, island.height) < lowestHeight){
                        lowestHeight = Noise(q+1, r-1, simplex, island.width, island.height);
                    }
                    if(Noise(q, r - 1, simplex, island.width, island.height) < lowestHeight){
                        lowestHeight = Noise(q, r-1, simplex, island.width, island.height);
                    }
                    if(Noise(q - 1, r, simplex, island.width, island.height) < lowestHeight){
                        lowestHeight = Noise(q-1, r, simplex, island.width, island.height);
                    }
                    if(lowestHeight == Mathf.Infinity){
                        lowestHeight = pos.y;
                    }
                    
                    tile.SetSize((lowestHeight - pos.y));

                    if(tile.transform.localPosition.y == 0f){
                        tile.SetColor(new Color(0.3970588f, 0.3970588f, 1f, 1f));
                        water.Add(tileGO);
                    }
                    else 
                    {
                        tile.SetColor(new Color(0f, 0.553f, 0f, .5f));
                    }
                }
            }
            foreach (GameObject tile in water)
            {
                Game.ObjectPooler.PoolObject(tile);
            }
            return island;
        }

        private float Noise(int x, int y, SimplexNoise.Simplex simplex, int width, int height){
            float d10 = simplex.Evaluate(x/10f, y/10f);
            float d100 = simplex.Evaluate(x/100f, y/100f);
            float d1000 = simplex.Evaluate(x/1000f, y/1000f) / 2f;
            float result = (d10) + (d100) + (d1000);

            float powered = Mathf.Sign(result) * Mathf.Pow(Mathf.Abs(result), 2.5f);

            float elevation = (result + powered);

            //Island Mask
            float distance_x = Mathf.Abs(x);
            float distance_y = Mathf.Abs(y);
            float distance = Mathf.Sqrt(distance_x * distance_x + distance_y * distance_y) / 1.25f; // circular mask
            float max_width = (width/2.5f) - 10.0f;
            float delta = distance / max_width;
            float gradient = delta * delta;

            elevation *= Mathf.Max(0.0f, 1.0f - gradient);
            if(elevation < 0f)
            elevation = 0f;

            if (float.IsNaN(elevation)){
                Debug.Log(elevation + ", " + gradient + ", " + max_width + ", " + distance_x + ", " + distance_y + ", " + result + ", " + powered + ", " + (result + powered));   
            }

            return elevation * 100f;
        }
    }
}