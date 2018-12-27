using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Koxel.World;
using Newtonsoft.Json;
using System.IO;
using Koxel.Tech;
using Koxel.Modding;

namespace Koxel
{
    public class Game : MonoBehaviour
    {
        #region Static References
        /// <summary>
        /// Reference to the World of Koxel.
        /// </summary>
        public static WorldManager World { get; private set; }
        
        /// <summary>
        /// Get and Return objects.
        /// </summary>
        public static ObjectPooler ObjectPooler { get; private set; }

        /// <summary>
        /// Managing saving and loading save files.
        /// </summary>
        public static SaveManager SaveManager { get; private set; }

        /// <summary>
        /// 'Multithreading' by Quil18
        /// </summary>
        public static ThreadQueuer ThreadQueuer { get; private set; }
        
        /// <summary>
        /// Manages mod parts and stores all references.
        /// </summary>
        public static ModManager ModManager { get; private set; }

        /// <summary>
        /// GameConfig manages contains all configurable settings for the game.
        /// </summary>
        public static GameConfig GameConfig { get; private set; }

        /// <summary>
        /// Path to the Game's folder.
        /// </summary>
        public static string GamePath { get; private set; }
        
        /// <summary>
        /// Reference to the current player.
        /// </summary>
        public static Player Player { get; private set; }
        #endregion
        
        #region Editor Configs
        public GameObject ChunkPrefab;
        public GameObject TilePrefab;
        public GameObject PlayerPrefab;
        #endregion

        private void Start()
        {
            ///Setup Game
            SetupPaths();
            LoadGameConfig();
            SetupObjectPooler();
            SetupThreadQueuer();
            SetupSaveManager();
            SetupWorld();

            ///MODDINGGGG
            LoadMods();

            ///Start Game
            CreateWorld();
        }

        private void SetupPaths()
        {
            GamePath = Directory.GetParent(Application.dataPath).FullName.Replace(@"\", "/");
        }

        private void LoadGameConfig()
        {
            string json = File.ReadAllText(GamePath + "/Config/GameConfig.json");
            GameConfig = JsonConvert.DeserializeObject<GameConfig>(json);
        }

        private void SetupObjectPooler()
        {
            GameObject PoolerGO = new GameObject("Object Pooler");
            PoolerGO.transform.SetParent(transform);
            ObjectPooler = gameObject.AddComponent<ObjectPooler>();
            ObjectPooler.PoolBuffer = PoolerGO.transform;
        }

        private void SetupSaveManager()
        {
            SaveManager = gameObject.AddComponent<SaveManager>();
        }

        private void SetupThreadQueuer()
        {
            ThreadQueuer = gameObject.AddComponent<ThreadQueuer>();
        }
        
        private void LoadMods()
        {
            ModManager = new ModManager();

            ModManager.SetupAllMods();
            ModManager.SetupComponents();

            foreach (ModManager.Component comp in ModManager.Components.Keys)
            {
                Dictionary<string, IModComponent> dict = ModManager.Components[comp];
                foreach (string tag in dict.Keys)
                {
                    IModComponent compo = dict[tag];
                    //Get the interface type with: compo.GetType(); and switch case over it
                    //Debug.Log(ModManager.Components[comp][tag].display);
                }
            }
        }

        private void SetupWorld()
        {
            //Setup Chunk object pooling
            //ObjectPooler.RegisterObject("Chunk", ChunkPrefab, GameConfig.renderDistance);
            ObjectPooler.RegisterObject("Tile", TilePrefab, 250);

            GameObject WorldGO = new GameObject("World");
            World = WorldGO.AddComponent<WorldManager>();
        }

        private void CreateWorld()
        {
            World.CreateIsland();
            //World.Generator.loader = Camera.main.transform;
            //World.Generator.ManageChunks();
        }

        private void CreatePlayer()
        {

        }

        public enum Systems{
            Game, ModManager, WorldManager, IslandGenerator, Player
        }
        public static void Error(Systems system, string error){
            string errorText = "";
            switch(system){
                case Systems.Game:
                    errorText += "[Game] ";
                    break;
                case Systems.ModManager:
                    errorText += "[ModManager] ";
                    break;
                case Systems.WorldManager:
                    errorText += "[WorldManager] ";
                    break;
                case Systems.IslandGenerator:
                    errorText += "[IslandGenerator] ";
                    break;
                case Systems.Player:
                    errorText += "[Player] ";
                    break;
            }
            errorText += error;
            Debug.LogError(errorText);
        }
    }
}
