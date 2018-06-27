using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Koxel.Tech
{
    public class ObjectPooler : MonoBehaviour
    {
        public Transform PoolBuffer;
        public Dictionary<string, GameObject> prefabs;
        public Dictionary<string, List<GameObject>> pooledObjects;


        void Awake()
        {
            prefabs = new Dictionary<string, GameObject>();
            pooledObjects = new Dictionary<string, List<GameObject>>();
        }

        public void RegisterObject(string tag, GameObject prefab, int startAmount)
        {
            //Don't add keys that already exist
            if (prefabs.ContainsKey(tag))
            {
                return;
            }

            //Create prefab entry
            prefabs.Add(tag, prefab);
            //Create list entry
            pooledObjects.Add(tag, new List<GameObject>());
            
            //Populate with start amount
            for (int i = 0; i < startAmount; i++)
            {
                CreateNew(tag);
            }
        }

        private GameObject CreateNew(string tag)
        {
            ///Code before this should already have checked if everything exists

            //Create and Add
            GameObject newObj = Instantiate(prefabs[tag], PoolBuffer);
            newObj.SetActive(false);
            newObj.name = newObj.name.Replace("(Clone)", "");
            pooledObjects[tag].Add(newObj);

            return pooledObjects[tag][pooledObjects[tag].Count-1];
        }

        public GameObject GetPooledObject(string tag)
        {
            List<GameObject> list;
            if(pooledObjects.TryGetValue(tag, out list))
            {
                bool found = false;
                GameObject foundObj = null;
                foreach (GameObject item in list)
                {
                    if (!item.activeSelf)
                    {
                        found = true;
                        foundObj = item;
                    }
                }

                if (!found)
                {
                    //create new
                    foundObj = CreateNew(tag);
                }

                return foundObj;
            }
            Debug.LogError("ObjectPooler: No pooled objects with the tag '" + tag + "' found.");
            return null;
        }

        public void PoolObject(GameObject item)
        {
            item.transform.parent = PoolBuffer.transform;
            item.SetActive(false);
        }
    }
}
