using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "UISettings", menuName = "ScriptableObjects/UISettings", order = 1)]
    public class UISettingsScriptableObject : ScriptableObject
    {
        [SerializeField] private float invalidXValueTextDuration = 2f;

        public float InvalidXValueTextDuration => invalidXValueTextDuration;
    }
}