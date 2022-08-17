using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace Pool
{
    public abstract class Pool : MonoBehaviour
    {
        [SerializeField] private SquarePoolSettingsScriptableObject squarePoolSettings;
        [SerializeField] private Transform containerTransform;
        
        private Queue<GameObject> itemPoolQueue;

        
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
            itemPoolQueue.Enqueue(go);
        }
    
        public void InitializeItemPoolDict(int size)
        {
            itemPoolQueue = new Queue<GameObject>(size);
            
            InitializeItemPool(size);
        }
   
        private void InitializeItemPool(int poolSize)
        {
            for (var j = 0; j < poolSize; j++)
            {
                itemPoolQueue.Enqueue(InstantiateNewItemForThePool());
            }
        }

        private GameObject InstantiateNewItemForThePool()
        {
            var newGO = Instantiate(squarePoolSettings.ItemPrefab, squarePoolSettings.SquareFirstSpawnPos, Quaternion.identity,
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