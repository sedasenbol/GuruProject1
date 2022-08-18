using System;
using System.Collections.Generic;
using Pool;
using UnityEngine;

namespace Managers.Grid
{
    public enum SquareState
    {
        Empty,
        Crossed
    }
    
    public class GridManager : Singleton<GridManager>
    {
        private const float CROSS_Z_POS = -0.01f;
        private const float CROSS_SCALE_MULTIPLIER = 0.9f;
        
        private List<List<GameObject>> grid => GridCreator.Instance.Grid;
        private List<List<SquareState>> gridState;
        private List<List<GameObject>> crossGrid;

        private int columnCount;
        private Vector3 crossScale;

        private Vector3 screenCenter => GridCreator.Instance.ScreenCenter;
        private float squareLength => GridCreator.Instance.SquareLength;

        private void OnEnable()
        {
            GridCreator.OnNewGridWasBuilt += OnNewGridWasBuilt;
        }

        private void OnDisable()
        {
            GridCreator.OnNewGridWasBuilt -= OnNewGridWasBuilt;
        }

        private void OnNewGridWasBuilt(int columnCount)
        {
            this.columnCount = columnCount;

            CleanupOldCrosses();
            
            InitializeGridState();

            SetCrossScale();
        }

        private void CleanupOldCrosses()
        {
            CrossPool.Instance.RecycleAllGameObjects();
        }
        
        private void SetCrossScale()
        {
            var currentScale = CrossPool.Instance.GetItemScale();

            var currentSquareLength = CrossPool.Instance.GetItemXLength();
            
            crossScale = currentScale * CROSS_SCALE_MULTIPLIER * squareLength / currentSquareLength;
        }

        private void InitializeGridState()
        {
            gridState = new List<List<SquareState>>();

            var rowCount = columnCount;
            
            for (var i = 0; i < columnCount; i++)
            {
                var newRow = new List<SquareState>();
                
                for (var j = 0; j < rowCount; j++)
                {
                    newRow.Add(SquareState.Empty);
                }
                
                gridState.Add(newRow);
            }
        }

        public void HandleChosenSquare(Transform squareTransform)
        {
            var squarePos = squareTransform.position;

            var i = ExtractIndex(squarePos, Vector3.right);
            var j = ExtractIndex(squarePos, Vector3.up);
            
            Debug.Log("i: " + i + "   j: " + j ); 

            if (gridState[i][j] == SquareState.Crossed) {return;}

            squarePos.z = CROSS_Z_POS;
            
            var cross = CrossPool.Instance.SpawnFromPool(squarePos, Quaternion.identity);
            cross.localScale = crossScale;

            gridState[i][j] = SquareState.Crossed;
        }

        // Inverse of the GridCreator.GetNthItemsPlaceOnTheAxis function
        private int ExtractIndex(Vector3 squarePos, Vector3 axis)
        {
            if (columnCount % 2 == 0)
            {
                var leftMiddleItem = columnCount / 2 - 1;

                return Mathf.RoundToInt(
                    (Vector3.Dot(squarePos, axis) - Vector3.Dot(screenCenter, axis)) /
                    squareLength + (leftMiddleItem + 0.5f));
            }
        
            var middleItem = columnCount / 2;

            return Mathf.RoundToInt((Vector3.Dot(squarePos, axis) - Vector3.Dot(screenCenter, axis)) /
                squareLength + middleItem);
        }
    }
}