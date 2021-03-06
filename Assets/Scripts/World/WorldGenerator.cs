﻿using Koxel.Tech;
using Koxel.World;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Koxel.World
{
    public class WorldGenerator
    {
        private Dictionary<Vector3, Chunk> Chunks;
        public WorldGenerator(Dictionary<Vector3, Chunk> Chunks)
        {
            this.Chunks = Chunks;
        }

        Chunk currentChunk;
        Vector3 currentCoords = new Vector3();
        public Transform loader;

        //Procedural Terrain
        private Dictionary<Vector2, ChunkInfo> chunksStore = new Dictionary<Vector2, ChunkInfo>();
        private List<ChunkInfo> unloadChunks = new List<ChunkInfo>();
        private List<ChunkInfo> loadChunks = new List<ChunkInfo>();

        public delegate void ChunksManaged(Chunk originChunk);
        public static event ChunksManaged OnChunksManaged;

        [Header("Debug")]
        public int chunksStoreCount;
        public int unloadChunksCount;
        public int loadChunksCount;

        public void Update()
        {
            if (loader != null)
            {
                //TODO: calculate loaders position to the current chunk instead of raycast
                Vector2 pixelCoords = new Vector2(loader.position.x, loader.position.z);
                Vector3 hexCoords = HexMath.PixelToHex(pixelCoords.x, pixelCoords.y, Game.World.HexData);
                //Tile(-33,-8) -> Chunk(-3,-1)
                //-33/13 = -2.5 ~= -3, -8/13 = -0.6 ~= -1
                int chunkSize = Game.GameConfig.chunkSize;
                float x = Mathf.Round(hexCoords.x / chunkSize);
                float z = Mathf.Round(hexCoords.y / chunkSize);
                float y = Mathf.Round(hexCoords.z / chunkSize);
                Vector3 chunkCoords = new Vector3(x, y, z);
                Debug.Log(chunkCoords);
                /*if (Game.World.Chunks.ContainsKey(chunkCoords))
                {
                    Debug.Log("Exists");
                    ChangeChunk(chunkCoords);
                    //currentChunk = Game.World.Chunks[chunkCoords];
                    //Game.ThreadQueuer.StartThreadedFunction(ManageChunks);
                }*/
                ChangeChunk(chunkCoords);
            }

            UpdateChunks();
        }

        public void ChangeChunk(Vector3 newCoords)
        {
            currentChunk = new Chunk()
            {
                coords = newCoords
            };
            currentCoords = currentChunk.coords;
            Game.ThreadQueuer.StartThreadedFunction(ManageChunks);
            //ManageChunks();
        }

        public void ManageChunks()
        {
            if (currentChunk == null)
            {
                currentChunk = new Chunk()
                {
                    coords = currentCoords
                };
            }

            int N = Game.GameConfig.renderDistance;
            int xmin = (int)currentCoords.x - N;
            int ymin = (int)currentCoords.y - N;
            int zmin = (int)currentCoords.z - N;
            int xmax = (int)currentCoords.x + N;
            int ymax = (int)currentCoords.y + N;
            int zmax = (int)currentCoords.z + N;


            List<Vector3> results = new List<Vector3>();
            for (int x = xmin; x <= xmax; x++)
            {
                for (int y = Mathf.Max(ymin, -x - zmax); y <= Mathf.Min(ymax, -x - zmin); y++)
                {
                    int z = -x - y;
                    results.Add(new Vector3(x, y, z));
                }
            }

            /// This code has been created by the awesome JohnyCilhokla
            /// for an old version of Koxel. I (Pixel) edited to make it work
            /// for the latest version.
            /// Thank you so much, Johny! :D ❤
            /// https://twitter.com/JohnyCilohokla

            foreach (Vector3 coords in results)
            {
                bool create = false;
                if (chunksStore.ContainsKey(coords))
                {
                    ChunkInfo chunkInfo = chunksStore[coords];
                    lock (unloadChunks)
                    {
                        if (Monitor.TryEnter(chunkInfo, 0))
                        {
                            try
                            {
                                if (chunkInfo.chunk == null && chunkInfo.data == null || (!chunkInfo.isLoaded && !chunkInfo.isLoading))
                                {
                                    create = true;
                                }
                                else if (chunkInfo.isUnloading) // chunk is queued to be unloaded
                                {
                                    chunkInfo.isUnloading = false; // remove unload flag
                                    unloadChunks.Remove(chunkInfo); // remove from the unload queue
                                }
                            }
                            finally
                            {
                                Monitor.Exit(chunkInfo);
                            }
                        }
                    }
                }
                else
                {
                    create = true;
                }
                if (create)
                {
                    ChunkData data;
                    /// load the chunk data here
                    /// this will save the main thread from doing that
                    /// you could/should also generate the chunks here and save the generated data
                    /// (then just setup the tiles/create objects in the main thread)
                    if (Game.SaveManager.IsChunkSaved(coords))
                    {
                        data = Game.SaveManager.LoadChunk(coords);
                    }
                    else
                    {
                        data = new ChunkData(coords);
                    }

                    ChunkInfo chunkInfo = new ChunkInfo(data);
                    chunksStore[coords] = chunkInfo; // add to the chunk store map

                    lock (loadChunks)
                    {
                        chunkInfo.isLoading = true; // add load flag
                        loadChunks.Add(chunkInfo); // add to the load queue
                    }
                }
            }

            List<Vector2> removals = new List<Vector2>();
            var it = chunksStore.GetEnumerator();
            while (it.MoveNext()) // loop through each chunk 
            {
                Vector2 pos = it.Current.Key;
                ChunkInfo chunkInfo = it.Current.Value;

                if (Monitor.TryEnter(chunkInfo, 0))
                {
                    try
                    {
                        if ((chunkInfo.isLoaded && !chunkInfo.isUnloading) || (!chunkInfo.isLoaded && chunkInfo.isLoading))
                        {
                            if (pos.x < xmin - 1 || pos.x > xmax + 1 || pos.y < Mathf.Max(ymin, -pos.x - zmax) - 1 || pos.y > Mathf.Min(ymax, -pos.x - zmin) + 1) // check if the chunk it outside of the "view"
                            {
                                if (chunkInfo.isLoaded)
                                {
                                    lock (unloadChunks)
                                    {
                                        chunkInfo.isUnloading = true; // add unload flag
                                        unloadChunks.Add(chunkInfo); // add to the unload queue
                                    }
                                }
                                else
                                {
                                    lock (loadChunks)
                                    {
                                        chunkInfo.isLoading = false; // remove load flag
                                        loadChunks.Remove(chunkInfo); // remove from the load queue
                                    }
                                }
                            }
                        }

                        if (chunkInfo.data == null && chunkInfo.chunk == null)
                        {
                            removals.Add(pos);
                        }
                    }
                    finally
                    {
                        Monitor.Exit(chunkInfo);
                    }
                }
            }
            foreach (Vector2 removal in removals)
            {
                chunksStore.Remove(removal);
            }

        }

        void UpdateChunks()
        {
            while (true)
            {
                lock (unloadChunks)
                {
                    if (unloadChunks.Count <= 0) { break; }
                }

                // grab a chunk to unload
                ChunkInfo chunkInfo;
                lock (unloadChunks)
                {
                    chunkInfo = unloadChunks[0];
                    chunkInfo.isLoaded = false;
                    unloadChunks.RemoveAt(0);
                }

                lock (chunkInfo)
                {
                    RemoveChunk(chunkInfo.chunk);
                    chunkInfo.chunk = null;

                    lock (unloadChunks)
                    {
                        chunkInfo.isUnloading = false;
                    }
                }
            }

            int runs = 3; // load up to x chunks
            while (true)
            {
                lock (loadChunks)
                {
                    if (loadChunks.Count <= 0)
                    {
                        if (chunksStore.Count > 1)
                            if (OnChunksManaged != null)
                            {
                                //Debug.Log(currentChunk);
                                OnChunksManaged(currentChunk);
                            }
                        break;
                    }
                }

                // grab a chunk to load
                ChunkInfo chunkInfo;
                lock (loadChunks)
                {
                    chunkInfo = loadChunks[0];
                    loadChunks.RemoveAt(0);
                }

                lock (chunkInfo)
                {
                    chunkInfo.chunk = AddChunk(chunkInfo.data); //chunkInfo.data.Load(this);
                    chunkInfo.data = null;

                    lock (loadChunks)
                    {
                        chunkInfo.isLoaded = true;
                        chunkInfo.isLoading = false;
                    }
                }

                if ((runs--) <= 0)
                {
                    break;
                }
            }


            //debug
            chunksStoreCount = chunksStore.Count;
            unloadChunksCount = unloadChunks.Count;
            loadChunksCount = loadChunks.Count;
        }

        public Chunk AddChunk(ChunkData chunkData)
        {
            Vector3 coords = new Vector3(chunkData.coords[0], chunkData.coords[1], chunkData.coords[2]);
            Vector3 pos = new Vector3(coords.x * Game.GameConfig.chunkSize * Game.World.HexData.Width + coords.y * (Game.GameConfig.chunkSize / 2f * Game.World.HexData.Width), 0, coords.y * Game.GameConfig.chunkSize * (.75f * Game.World.HexData.Height));
            GameObject chunkGO = Game.ObjectPooler.GetPooledObject("Chunk");
            chunkGO.transform.parent = Game.World.transform;
            chunkGO.transform.localPosition = pos;
            chunkGO.name = "Chunk (" + coords.x + ", " + coords.y + ")";
            chunkGO.SetActive(true);
            Chunk chunk = chunkGO.GetComponent<Chunk>();
            chunk.coords = coords;
            if (chunkData.tiles == null)
                chunk.Generate();
            else
                chunk.Generate(chunkData.tiles);
            Chunks.Add(coords, chunk);
            Game.SaveManager.SaveChunk(chunk);
            return chunk;
        }
        public Chunk AddChunk(Vector3 coords)
        {
            Vector3 pos = new Vector3(coords.x * Game.GameConfig.chunkSize * Game.World.HexData.Width + coords.y * (Game.GameConfig.chunkSize / 2f * Game.World.HexData.Width), 0, coords.y * Game.GameConfig.chunkSize * (.75f * Game.World.HexData.Height));
            GameObject chunkGO = Game.ObjectPooler.GetPooledObject("Chunk");
            chunkGO.transform.parent = Game.World.transform;
            chunkGO.transform.localPosition = pos;
            chunkGO.name = "Chunk (" + coords.x + ", " + coords.y + ")";
            chunkGO.SetActive(true);
            Chunk chunk = chunkGO.GetComponent<Chunk>();
            chunk.coords = coords;
            //chunk.Generate();
            Chunks.Add(coords, chunk);

            Game.SaveManager.SaveChunk(chunk);
            return chunk;
        }

        public void RemoveChunk(Chunk chunk)
        {
            Chunks.Remove(chunk.coords);
            Game.ObjectPooler.PoolObject(chunk.gameObject);
        }
    }
}

class ChunkInfo
{
    public ChunkData data;
    public Chunk chunk;
    public bool isLoading;
    public bool isLoaded;
    public bool isUnloading;
    public bool remove;

    public ChunkInfo(ChunkData data)
    {
        this.data = data;
        this.chunk = null;
        this.isLoading = false;
        this.isLoaded = false;
        this.isUnloading = false;
        this.remove = false;
    }
}

