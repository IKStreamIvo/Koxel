using System.Collections.Generic;
using System.Text;
using Koxel.Tech;
using UnityEngine;
using SimplexNoise;

namespace Koxel.World{
    public class IslandGenerator {
        HexData HexData;
        Simplex Simplex;

        public IslandGenerator(){
            HexData = new HexData(Game.GameConfig.hexSize);
        }

        public Island Generate(Island island){
            Simplex = new SimplexNoise.Simplex(island.seed);
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
                        Noise(q, r, simplex),
                        r * (HexData.Height * .75f)
                    );
                    tileGO.transform.localPosition = pos;

                    //Get lowest neighbour
                    float lowestHeight = Mathf.Infinity;
                    if(Noise(q - 1, r + 1, simplex) < lowestHeight){
                        lowestHeight = Noise(q-1, r+1, simplex);
                    }
                    if(Noise(q, r + 1, simplex) < lowestHeight){
                        lowestHeight = Noise(q, r+1, simplex);
                    }
                    if(Noise(q + 1, r, simplex) < lowestHeight){
                        lowestHeight = Noise(q+1, r, simplex);
                    }
                    if(Noise(q + 1, r - 1, simplex) < lowestHeight){
                        lowestHeight = Noise(q+1, r-1, simplex);
                    }
                    if(Noise(q, r - 1, simplex) < lowestHeight){
                        lowestHeight = Noise(q, r-1, simplex);
                    }
                    if(Noise(q - 1, r, simplex) < lowestHeight){
                        lowestHeight = Noise(q-1, r, simplex);
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
            
            //Generate Rectangle
            for (int r = -island.height/2; r < island.height/2; r++)
            {
                int r_offset = (int) Mathf.Floor(r/2f);
                for (int q = -r_offset - island.width/2; q < island.width/2 - r_offset; q++)
                {
                    CreateTile(q, r);
                }
            }
            return island;
        }

        private float Noise(int x, int y){
            //note: X should be smaller then Y to increase roundness
            float result = 0f;
            result += Simplex.Evaluate(x/5f, y/3.5f) / 7f;//detail noise
            result += Simplex.Evaluate(x/11f, y/15f) / 1f;//smaller hills (/.5f for more spikes)
            result += Simplex.Evaluate(x/35f, y/50f) / 1.5f;//larger hills
            
            float powered = Mathf.Sign(result) * Mathf.Pow(Mathf.Abs(result), 2.5f);

            float elevation = (result + powered);

            //Redblob's island function
            float a = 3f;
            float b = 0.01f;
            float d = Mathf.Sqrt(x * x + y * y);
            float c = 1.5f;
            elevation = elevation + a - b * Mathf.Pow(d, c);

            elevation = elevation/5f;

            if(elevation < 0f)
                elevation = 0f;

            return elevation * 80f;
        }

        private void CreateTile(int q, int r){
            float noiseHeight = Noise(q, r);
            if(noiseHeight == 0f)
                return;

            GameObject tileGO = Game.ObjectPooler.GetPooledObject("Tile");
            tileGO.name = "Tile (" + r + ", " + q + ")";
            tileGO.transform.SetParent(Game.World.transform);
            tileGO.transform.localScale = tileGO.transform.localScale * HexData.Size;
            Vector3 pos = new Vector3(
                q * HexData.Width + r * (.5f * HexData.Width),
                noiseHeight,
                r * (HexData.Height * .75f)
            );
            tileGO.transform.localPosition = pos;

            HexTile tile = tileGO.GetComponent<HexTile>();

            //Set height
            float lowestNeighbour = Mathf.Infinity;
            List<float> neighbourNoise = new List<float>(){ //arg theyre so loud!
                Noise(q-1, r+1), Noise(q, r+1), Noise(q+1, r),
                Noise(q+1, r-1), Noise(q, r-1), Noise(q-1, r)
            };
            foreach(float neighbour in neighbourNoise){
                if(neighbour < lowestNeighbour){
                    lowestNeighbour = neighbour;
                }
            }
            if(lowestNeighbour == Mathf.Infinity){
                lowestNeighbour = pos.y;
            }
            tile.SetSize((lowestNeighbour - pos.y));

            //Green grass
            tile.SetColor(new Color(0f, 0.553f, 0f, .5f));

            tileGO.SetActive(true);
        }
    }
}