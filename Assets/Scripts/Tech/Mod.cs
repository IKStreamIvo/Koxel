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
    }
}
