using System;
using System.Collections.Generic;
using System.Globalization;
using Pool;
using ScriptableObjects;
using TMPro;
using UnityEngine;

namespace Managers.Grid
{
    public class GridCreator : Singleton<GridCreator>
    {
        public static event Action<int> OnNewGridWasBuilt;
        public static event Action OnInvalidXValueWasEntered;

        [SerializeField] private GridSettingsScriptableObject gridSettings;
        [SerializeField] private RectTransform bottomPanelRectTransform;
        [SerializeField] private TMP_InputField columnCountInputField;

        private Camera mainCam;
        private int columnCount;
        private Vector3 squareScale;
        private Vector3 screenCenter;

        public void HandleGridRebuildRequest()
        {
            if (!CheckColumnCountValidity(out columnCount)) return;

            CleanupOldGrid();
            ExpandPools();
            SetSquareLength();
            SetSquareScale();
            BuildGrid();

            OnNewGridWasBuilt?.Invoke(columnCount);
        }

        private void ExpandPools()
        {
            SquarePool.Instance.ExpandItemPoolDict(columnCount * columnCount);
            CrossPool.Instance.ExpandItemPoolDict(columnCount * columnCount);
        }

        private void CleanupOldGrid()
        {
            SquarePool.Instance.RecycleAllGameObjects();
        }

        private void SetSquareLength()
        {
            var corners = new Vector3[4];
            bottomPanelRectTransform.GetWorldCorners(corners);
            var bottomLeftPos =  mainCam.ScreenToWorldPoint(corners[1]);

            var topRightPos = mainCam.ViewportToWorldPoint(Vector3.one);
            topRightPos.z = 0f;

            screenCenter = (bottomLeftPos + topRightPos) / 2;

            var horizontalLength = topRightPos.x - bottomLeftPos.x - 2 * gridSettings.HorizontalBoardMargin;
            var verticalLength = topRightPos.y - bottomLeftPos.y - 2 * gridSettings.VerticalBoardMargin;

            var rowCount = columnCount;

            SquareLength = Mathf.Min(horizontalLength / columnCount, verticalLength / rowCount);   
        }
        
        private void SetSquareScale()
        {
            var currentScale = SquarePool.Instance.GetItemScale();

            var currentSquareLength = SquarePool.Instance.GetItemXLength();

            squareScale = currentScale * SquareLength / currentSquareLength;
        }

        private void BuildGrid()
        {
            Grid = new List<List<GameObject>>();
            
            var rowCount = columnCount;
            
            for (var i = 0; i < columnCount; i++)
            {
                var newRow = new List<GameObject>(rowCount);

                for (var j = 0; j < rowCount; j++)
                {
                    newRow.Add(CreateGridItem(GetGridPosition(i, j)).gameObject);
                }
                
                Grid.Add(newRow);
            }
        }
        
        private Transform CreateGridItem(Vector3 pos)
        {
            var itemTransform = SquarePool.Instance.SpawnFromPool(pos, Quaternion.identity);

            itemTransform.localScale = squareScale;

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

                return Vector3.Dot(screenCenter, axis) + (i - leftMiddleItem - 0.5f) * SquareLength;
            }
        
            var middleItem = axisItemCount / 2;

            return Vector3.Dot(screenCenter, axis) + (i - middleItem) * SquareLength;
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
            Grid = null;
        }

        public List<List<GameObject>> Grid { get; private set; }

        public Vector3 ScreenCenter => screenCenter;
        public float SquareLength { get; private set; }
    }
}
