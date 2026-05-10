using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Interfaces;
using UnityEngine;

namespace _Scripts.Lecture09
{
    public class ObjectPooler : MonoBehaviour
    {
        [Serializable]
        public class Pool
        {
            public string Tag;
            public GameObject Prefab;
            public int PoolSize;
        }

        public static Dictionary<string, Queue<GameObject>> PoolDictionary;
        public List<Pool> pools;

        public static ObjectPooler Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Start()
        {
            PoolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (var pool in pools)
            {
                var objectPool = new Queue<GameObject>();

                for (var i = 0; i < pool.PoolSize; i++)
                {
                    var obj = Instantiate(pool.Prefab, transform);
                    obj.SetActive(false);
                    
                    objectPool.Enqueue(obj);
                }
                
                PoolDictionary.Add(pool.Tag, objectPool);
            }
        }

        /// <summary>
        /// Activates an object from a specified pool to be used.
        /// </summary>
        public static GameObject GetFromPool(string tag, Vector3 pos, Quaternion rot)
        {
            if (!PoolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag: " + tag + " doesn't exist");
                return null;
            }

            var objectToSpawn = PoolDictionary[tag].Dequeue();

            objectToSpawn.transform.position = pos;
            objectToSpawn.transform.rotation = rot;
            objectToSpawn.SetActive(true);

            var pooledObject = objectToSpawn.GetComponent<IPooledObject>();
            pooledObject?.OnObjectSpawn();
            
            PoolDictionary[tag].Enqueue(objectToSpawn);

            return objectToSpawn;
        }

        /// <summary>
        /// Releases an object back to its pool with an optional delay, similar to Destroy() method.
        /// </summary>
        public static void ReleaseToPool(GameObject obj, float delay = 0)
        {
            if (delay <= 0)
            {
                obj.SetActive(false);
            }
            else
            {
                Instance.StartCoroutine(Instance.ReleaseAfterDelay(obj, delay));
            }
        }

        private IEnumerator ReleaseAfterDelay(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            obj.SetActive(false);
        }
    }
}
