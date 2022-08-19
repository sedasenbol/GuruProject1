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
        [SerializeField] private TMP_Text matchCountText;
        
        private YieldInstruction InvalidColumnCountWaitFor;
        
        private void OnEnable()
        {
            InvalidColumnCountWaitFor = new WaitForSeconds(uISettings.InvalidXValueTextDuration);
            
            GridCreator.OnInvalidXValueWasEntered += OnInvalidXValueWasEntered;
            GridManager.OnCrossesAreCleanedUp += OnCrossesAreCleanedUp;
            GridCreator.OnNewGridWasBuilt += OnNewGridWasBuilt;
        }

        private void OnDisable()
        {
            GridCreator.OnInvalidXValueWasEntered -= OnInvalidXValueWasEntered;
            GridManager.OnCrossesAreCleanedUp -= OnCrossesAreCleanedUp;
            GridCreator.OnNewGridWasBuilt -= OnNewGridWasBuilt;
        }

        private void OnNewGridWasBuilt(int columnCount)
        {
            UpdateMatchCountText();
        }

        private void OnCrossesAreCleanedUp()
        {
            UpdateMatchCountText();
        }

        private void UpdateMatchCountText()
        {
            matchCountText.text = $"Match Count: {GridManager.Instance.MatchCount}";
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