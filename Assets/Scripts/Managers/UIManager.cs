using System.Collections;
using Managers.Grid;
using ScriptableObjects;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private UISettingsScriptableObject uISettings;
        [SerializeField] private GameObject invalidColumnCountGO;

        private YieldInstruction InvalidColumnCountWaitFor;
        
        private void OnEnable()
        {
            InvalidColumnCountWaitFor = new WaitForSeconds(uISettings.InvalidXValueTextDuration);
            
            GridCreator.OnInvalidXValueWasEntered += OnInvalidXValueWasEntered;
        }

        private void OnDisable()
        {
            GridCreator.OnInvalidXValueWasEntered -= OnInvalidXValueWasEntered;
        }

        private void OnInvalidXValueWasEntered()
        {
            StopAllCoroutines();
            
            EnableInvalidXValueGO();

            StartCoroutine(DisableInvalidXValueGOWithDelay());
        }

        private void EnableInvalidXValueGO()
        {
            invalidColumnCountGO.SetActive(true);
        }

        private IEnumerator DisableInvalidXValueGOWithDelay()
        {
            yield return InvalidColumnCountWaitFor;
            
            invalidColumnCountGO.SetActive(false);
        }
    }
}