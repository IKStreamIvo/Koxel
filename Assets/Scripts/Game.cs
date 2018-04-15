using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Koxel.World;
using Newtonsoft.Json;
using System.IO;
using Koxel.Tech;

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
        /// GameConfig manages contains all configurable settings for the game.
        /// </summary>
        public static GameConfig GameConfig { get; private set; }

        /// <summary>
        /// Path to the Game's folder.
        /// </summary>
        public static string GamePath { get; private set; }
        #endregion
        #region Editor Configs
        public GameObject ChunkPrefab;
        #endregion

        private void Start()
        {
            ///Setup Game
            SetupPaths();
            LoadGameConfig();
            SetupObjectPooler();
            SetupSaveManager();
            SetupWorld();

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
        
        private void SetupWorld()
        {
            GameObject WorldGO = new GameObject("World");
            World = WorldGO.AddComponent<WorldManager>();

            //Setup Chunk object pooling
            ObjectPooler.RegisterObject("Chunk", ChunkPrefab, GameConfig.renderDistance);
        }

        private void CreateWorld()
        {
            Debug.Log("Create world");
            World.Generator.loader = Camera.main.transform;
            World.Generator.ManageChunks();
        }
    }
}
