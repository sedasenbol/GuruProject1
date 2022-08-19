using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
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
        public static event Action OnCrossesAreCleanedUp;
        
        private const float CROSS_Z_POS = -0.01f;
        private const float CROSS_SCALE_MULTIPLIER = 0.9f;
        
        private List<List<GameObject>> grid => GridCreator.Instance.Grid;
        private List<List<SquareState>> gridState;
        private List<List<GameObject>> crossGrid;

        private List<int[]> crossedNeighbours;
        private List<int[]> neighbours;
        private List<int[]> AllCrossedNeighboursAroundSquare;

        private int columnCount;
        private Vector3 crossScale;

        private void Start()
        {
            neighbours = new List<int[]>();
            crossedNeighbours = new List<int[]>();
            AllCrossedNeighboursAroundSquare = new List<int[]>();
        }

        private void OnDestroy()
        {
            crossGrid = null;
        }

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
            MatchCount = 0;
            
            CleanupAllOldCrosses();
            
            InitializeGridState();
            InitializeCrossGrid();

            SetCrossScale();
        }

        private void CleanupAllOldCrosses()
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
        
        private void InitializeCrossGrid()
        {
            crossGrid = new List<List<GameObject>>();

            var rowCount = columnCount;

            for (var i = 0; i < columnCount; i++)
            {
                var newRow = new List<GameObject>();

                for (var j = 0; j < rowCount; j++)
                {
                    newRow.Add(null);
                }
                
                crossGrid.Add(newRow);
            }
        }
        
        public void HandleChosenSquare(Transform squareTransform)
        {
            var squarePos = squareTransform.position;

            var i = ExtractIndex(squarePos, Vector3.right);
            var j = ExtractIndex(squarePos, Vector3.up);
            
            if (gridState[i][j] == SquareState.Crossed) {return;}

            squarePos.z = CROSS_Z_POS;
            
            var cross = CrossPool.Instance.SpawnFromPool(squarePos, Quaternion.identity);
            cross.localScale = crossScale;
            
            crossGrid[i][j] = cross.gameObject;

            gridState[i][j] = SquareState.Crossed;
            
            AllCrossedNeighboursAroundSquare.Clear();
            AllCrossedNeighboursAroundSquare.Add(new int[]{i,j});
            
            CheckForTripleCrossAroundSquare(i,j);

            if (AllCrossedNeighboursAroundSquare.Count < 3) {return;}
            
            CleanupCrossesAroundSquare();
        }

        private void CleanupCrossesAroundSquare()
        {
            foreach (var indexes in AllCrossedNeighboursAroundSquare)
            {
                CrossPool.Instance.RecycleGameObject(crossGrid[indexes[0]][indexes[1]]);
                gridState[indexes[0]][indexes[1]] = SquareState.Empty;
            }

            MatchCount++;
            OnCrossesAreCleanedUp?.Invoke();
        }

        private void CheckForTripleCrossAroundSquare(int i, int j)
        {
            var crossedNeighboursAroundSquare = CheckNeighbourIndexesForCross(GetNeighbourIndexesOfTheSquare(i, j));

            crossedNeighboursAroundSquare.RemoveAll(x => AllCrossedNeighboursAroundSquare.Any(y => y[0] == x[0] && y[1] == x[1]));
            
            AllCrossedNeighboursAroundSquare.AddRange(crossedNeighboursAroundSquare);

            foreach (var neighbour in crossedNeighboursAroundSquare.ToList())
            {
                CheckForTripleCrossAroundSquare(neighbour[0], neighbour[1]);   
            }
        }
        

        private List<int[]> CheckNeighbourIndexesForCross(List<int[]> neighboursList)
        {
            crossedNeighbours.Clear();
            
            foreach (var indexes in neighboursList)
            {
                if (gridState[indexes[0]][indexes[1]] != SquareState.Crossed) {continue;}
                
                if (crossedNeighbours.Contains(indexes) || AllCrossedNeighboursAroundSquare.Contains(indexes)) {continue;}
                    
                crossedNeighbours.Add(indexes);
            }

            return crossedNeighbours;
        }

        private  List<int[]>  GetNeighbourIndexesOfTheSquare(int indexI, int indexJ)
        {
            neighbours.Clear();

            for (var i = indexI - 1; i < indexI + 2; i++)
            {
                for (var j = indexJ - 1; j < indexJ + 2; j++)
                {
                    if (Mathf.Abs(indexI - i) == Mathf.Abs(indexJ - j)) {continue;}
                    
                    if (i < 0 || i >= columnCount || j < 0 || j >= columnCount) {continue;}
                    
                    neighbours.Add(new int[]{i,j});
                }
            }

            return neighbours;
        }

        // Inverse of the GridCreator.GetNthItemsPlaceOnTheAxis function
        private int ExtractIndex(Vector3 squarePos, Vector3 axis)
        {
            if (columnCount % 2 == 0)
            {
                var leftMiddleItem = columnCount / 2 - 1;

                return Mathf.RoundToInt((Vector3.Dot(squarePos, axis) - Vector3.Dot(screenCenter, axis)) /
                    squareLength + (leftMiddleItem + 0.5f));
            }
        
            var middleItem = columnCount / 2;

            return Mathf.RoundToInt((Vector3.Dot(squarePos, axis) - Vector3.Dot(screenCenter, axis)) /
                squareLength + middleItem);
        }
        
        public int MatchCount { get; private set; }
    }
}