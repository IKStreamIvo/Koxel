using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Koxel.Modding
{
    public class ModManager
    {
        /// <summary>
        /// Mod Component Type
        /// </summary>
        public enum Component
        {
            TileType
        }

        public Dictionary<string, Mod> Mods;

        /// <summary>
        /// Contains all components loaded in the game. 
        /// Usage: `Components[Component.*][tag]`
        /// </summary>
        public Dictionary<Component, Dictionary<string, IModComponent>> Components;

        public ModManager()
        {
            Components = new Dictionary<Component, Dictionary<string, IModComponent>>();
        }

        public void RegisterComponent(Component component, IModComponent modComponent)
        {
            if (!Components.ContainsKey(component))
            {
                Components.Add(component, new Dictionary<string, IModComponent>());
            }

            if (!Components[component].ContainsKey(modComponent.tag))
            {
                Components[component].Add(modComponent.tag, modComponent);
            }
            else
            {
                Debug.LogError("ModManager - RegisterComponent: '" + modComponent.tag + "' was already registered!");
            }
        }

        public void SetupAllMods()
        {
            Mods = new Dictionary<string, Mod>();

            string json = File.ReadAllText(Game.GamePath + "/Config/LoadOrder.json");
            JToken jToken = JToken.Parse(json);
            string[] modsList = jToken["Mods"].ToObject<string[]>();

            foreach (string mod in modsList)
            {
                string path = Game.GamePath + "/Mods/" + mod + "/";
                if (Directory.Exists(path))
                {
                    LoadMod(path);
                }
                else
                {
                    Debug.LogError("Mod '" + mod + "' doesn't exist!");
                }
            }
        }

        void LoadMod(string path)
        {
            string json = File.ReadAllText(path + "ModInfo.json");
            Mod mod = JsonConvert.DeserializeObject<Mod>(json);
            mod.Load(path);
            Mods.Add(mod.modInfo.name, mod);
        }

        public void SetupComponents()
        {
            foreach (Component component in Component.GetValues(typeof(Component)))
            {
                foreach (Mod mod in Mods.Values)
                {
                    mod.LoadComponent(component);
                }
            }            
        }

       
    }
}