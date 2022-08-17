using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "SquarePoolSettings", menuName = "ScriptableObjects/SquarePoolSettings", order = 1)]
    public class SquarePoolSettingsScriptableObject : ScriptableObject
    {
        [SerializeField] private Vector3 squareFirstSpawnPos = Vector3.one * 1000f;
        [SerializeField] private Transform itemPrefab;
        
        public Vector3 SquareFirstSpawnPos => squareFirstSpawnPos;
        public Transform ItemPrefab => itemPrefab;
    }
}