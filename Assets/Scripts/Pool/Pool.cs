using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace Pool
{
    public abstract class Pool : MonoBehaviour
    {
        [SerializeField] private PoolSettingsScriptableObject poolSettings;
        [SerializeField] private Transform containerTransform;
        
        private Queue<GameObject> itemPoolQueue;

        private void Start()
        {
            itemPoolQueue = new Queue<GameObject>();
        }

        public Vector3 GetItemScale()
        {
            return itemPoolQueue.Peek().transform.localScale;
        }

        public float GetItemXLength()
        {
            return itemPoolQueue.Peek().GetComponentInChildren<Renderer>().bounds.size.x;
        }
        
        public Transform SpawnFromPool(Vector3 position, Quaternion rotation)
        {
            var objectSpawned = itemPoolQueue.Dequeue();
            objectSpawned.SetActive(true);
        
            var objectSpawnedTransform = objectSpawned.transform;
            objectSpawnedTransform.position = position;
            objectSpawnedTransform.rotation = rotation;
            
            objectSpawnedTransform.SetParent(containerTransform);

            itemPoolQueue.Enqueue(objectSpawned);
            
            return objectSpawnedTransform;
        }

        public void RecycleGameObject(GameObject go)
        {
            go.transform.SetParent(containerTransform);
            go.SetActive(false);
        }

        public void RecycleAllGameObjects()
        {
            foreach (var item in itemPoolQueue)
            {
                item.transform.SetParent(containerTransform);
                item.SetActive(false);
            }
        }

        public void ExpandItemPoolDict(int size)
        {
            var currentSize = itemPoolQueue.Count;

            ExpandItemPool(Mathf.Max(size - currentSize, 0));
        }
   
        private void ExpandItemPool(int poolSize)
        {
            for (var j = 0; j < poolSize; j++)
            {
                itemPoolQueue.Enqueue(InstantiateNewItemForThePool());
            }
        }

        private GameObject InstantiateNewItemForThePool()
        {
            var newGO = Instantiate(poolSettings.ItemPrefab, poolSettings.FirstSpawnPos, Quaternion.identity,
                containerTransform).gameObject;
            
            newGO.SetActive(false);

            return newGO;
        }

        private void OnDisable()
        {
            itemPoolQueue = null;
        }
    }
}