using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "GridSettings", menuName = "ScriptableObjects/GridSettings", order = 1)]
    public class GridSettingsScriptableObject : ScriptableObject
    {
        [SerializeField] private int xMaxLimit = 40;
        [SerializeField] private int xMinLimit = 2;

        [SerializeField] private float horizontalBoardMargin = 2f;
        [SerializeField] private float verticalBoardMargin = 2f;

        [SerializeField] private Transform crossTransform;
        
        public int XMaxLimit => xMaxLimit;
        public int XMinLimit => xMinLimit;
        public float HorizontalBoardMargin => horizontalBoardMargin;
        public float VerticalBoardMargin => verticalBoardMargin;
        public Transform CrossTransform => crossTransform;
    }
}