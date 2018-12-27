using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Koxel.Modding
{
    public class Mod
    {
        public string ModPath;
        public ModInfo modInfo;

        public void Load(string ModPath)
        {
            this.ModPath = ModPath;
            LoadInfo();
        }

        public void LoadInfo()
        {
            string json = File.ReadAllText(ModPath + "ModInfo.json");
            modInfo = JsonConvert.DeserializeObject<ModInfo>(json);
        }

        public void LoadComponent(ModManager.Component component)
        {
            switch (component)
            {
                case ModManager.Component.TileType:
                    TileTypes();
                    break;
                    
                case ModManager.Component.Biome:
                    Biomes();
                    break;
            }
        }

        public void TileTypes()
        {
            if (!Directory.Exists(ModPath + "TileTypes"))
                return;

            DirectoryInfo dir = new DirectoryInfo(ModPath + "TileTypes");
            FileInfo[] files = dir.GetFiles("*.json", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                string json = File.ReadAllText(file.FullName);
                JToken jToken = JToken.Parse(json);
                string tag = jToken["tag"].ToObject<string>();
                string display = jToken["display"].ToObject<string>();
                float[] color = jToken["color"].ToObject<float[]>();

                TileType tileType = new TileType(tag, display, color);
                Game.ModManager.RegisterComponent(ModManager.Component.TileType, tileType);
            }
        }

        public void Biomes()
        {
            if (!Directory.Exists(ModPath + "Biomes"))
                return;

            DirectoryInfo dir = new DirectoryInfo(ModPath + "Biomes");
            FileInfo[] files = dir.GetFiles("*.json", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                string json = File.ReadAllText(file.FullName);
                JToken jToken = JToken.Parse(json);

                string tag = jToken["tag"].ToObject<string>();
                string display = jToken["display"].ToObject<string>();

                Elevation elevation = new Elevation(jToken["elevation"]["value"]);
                
                Tile mainTile = null;
                /*foreach(JToken tile in jToken["tiles"]){
                    string type = tile["type"].ToObject<string>();
                    if(type == "main"){
                        List<TileSegment> segmentsList = new List<TileSegment>();
                        foreach(JToken layer in tile["layers"]){
                            TileType segType = (TileType)Game.ModManager.Components[ModManager.Component.TileType][layer["tag"].ToObject<string>()];
                            TileSegment segment = new TileSegment(segType, layer["size"].ToObject<float>());
                            Debug.Log(segment.tileType.tag);
                            segmentsList.Add(segment);
                        }
                        mainTile = new Tile(segmentsList.ToArray());
                    }
                }
                if(mainTile == null){
                    Game.Error(Game.Systems.ModManager, "No 'main' tile has been defined in biome '" + tag + "'. A 'main' tile is required.");
                    return;
                }*/

                Biome biome = new Biome(tag, display, elevation, mainTile);
                Game.ModManager.RegisterComponent(ModManager.Component.Biome, biome);
            }
        }
    }
}
