using System;
using System.Collections.Generic;
using System.Globalization;
using Pool;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class GridCreator : MonoBehaviour
    {
        public static event Action OnInvalidXValueWasEntered;

        [SerializeField] private SquarePoolSettingsScriptableObject squarePoolSettings;
        [SerializeField] private GridSettingsScriptableObject gridSettings;
        [SerializeField] private RectTransform bottomPanelRectTransform;
        [SerializeField] private TMP_InputField columnCountInputField;
    
        private Camera mainCam;
        private List<List<Transform>> grid;
        private int columnCount;
        private float squareLength;
        private Vector3 itemScale;
        private Vector3 screenCenter;

        public void HandleGridRebuildRequest()
        {
            if (!CheckColumnCountValidity(out columnCount)) return;
            
            SquarePool.Instance.InitializeItemPoolDict(columnCount * columnCount);

            SetSquareLength();
            SetSquareScale();
            BuildGrid();
        }

        private void SetSquareLength()
        {
            var corners = new Vector3[4];
            bottomPanelRectTransform.GetWorldCorners(corners);
            var bottomLeftPos = corners[1];
            
            var topRightPos = mainCam.ViewportToWorldPoint(Vector3.one);
            topRightPos.z = 0f;

            screenCenter = (bottomLeftPos + topRightPos) / 2;

            var horizontalLength = topRightPos.x - bottomLeftPos.x - 2 * gridSettings.HorizontalBoardMargin;
            var verticalLength = topRightPos.y - bottomLeftPos.y - 2 * gridSettings.VerticalBoardMargin;

            var rowCount = columnCount;

            squareLength = Mathf.Min(horizontalLength / columnCount, verticalLength / rowCount);   
        }
        
        private void SetSquareScale()
        {
            var currentScale = SquarePool.Instance.GetItemScale();

            var currentSquareLength = SquarePool.Instance.GetItemXLength();

            itemScale = currentScale * squareLength / currentSquareLength;
        }

        private void BuildGrid()
        {
            grid = new List<List<Transform>>();
            
            var rowCount = columnCount;
            
            for (var i = 0; i < columnCount; i++)
            {
                var newRow = new List<Transform>(rowCount);

                for (var j = 0; j < rowCount; j++)
                {
                    newRow.Add(CreateGridItem(GetGridPosition(i, j)));
                }
                
                grid.Add(newRow);
            }
        }
        
        private Transform CreateGridItem(Vector3 pos)
        {
            var itemTransform = SquarePool.Instance.SpawnFromPool(pos, Quaternion.identity);

            itemTransform.localScale = itemScale;

            return itemTransform;
        }
        
        private Vector3 GetGridPosition(int i, int j)
        {
            var rowCount = columnCount;
            
            return new Vector3()
            {
                x = GetNthItemsPlaceOnTheAxis(i, columnCount, Vector3.right),
                y = GetNthItemsPlaceOnTheAxis(j, rowCount, Vector3.up),
                z = 0
            };
        }

        private float GetNthItemsPlaceOnTheAxis(int i, int axisItemCount, Vector3 axis)
        {
            if (axisItemCount % 2 == 0)
            {
                var leftMiddleItem = axisItemCount / 2 - 1;

                return Vector3.Dot(screenCenter, axis) + (i - leftMiddleItem - 0.5f) * squareLength;
            }
        
            var middleItem = axisItemCount / 2;

            return Vector3.Dot(screenCenter, axis) + (i - middleItem) * squareLength;
        }
        
        private bool CheckColumnCountValidity(out int columnCount)
        {
            if (int.TryParse(columnCountInputField.text, NumberStyles.Any, CultureInfo.DefaultThreadCurrentUICulture, out columnCount) && 
                columnCount <= gridSettings.XMaxLimit &&
                columnCount >= gridSettings.XMinLimit) return true;
            
            OnInvalidXValueWasEntered?.Invoke();
            return false;
        }

        private void Start()
        {
            mainCam = Camera.main;
        }

        private void OnDestroy()
        {
            mainCam = null;
        }

    }
}
