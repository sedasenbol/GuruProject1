using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "PoolSettings", menuName = "ScriptableObjects/PoolSettings", order = 1)]
    public class PoolSettingsScriptableObject : ScriptableObject
    {
        [SerializeField] private Vector3 firstSpawnPos = Vector3.one * 1000f;
        [SerializeField] private Transform itemPrefab;
        
        public Vector3 FirstSpawnPos => firstSpawnPos;
        public Transform ItemPrefab => itemPrefab;
    }
}