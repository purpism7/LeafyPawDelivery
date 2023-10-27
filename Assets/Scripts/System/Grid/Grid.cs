using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public interface IGridProvider
    {
        float LimitPosY(float posY);
        void Overlap();
    }

    public class Grid : MonoBehaviour, IGridProvider, IUpdater, Cell.IListener
    {
        public GridData GridData;
        public Cell Cell; 

        private Cell[,] _cellArray = null;

        //private LineRenderer _lineRenderer = null;

        private void Start()
        {
            Generated();
        }

        private void OnValidate()
        {
            //if (Row + Column > 0)
            //{
            //    Make();
            //}
        }

        public void Init()
        {
            //_cells[Row][Column]
        }

        private void Generated()
        {
            if(GridData == null)
                return;

            _cellArray = new Cell[GridData.Row, GridData.Column];

            for (int column = 0; column < GridData.Column; ++column)
            {
                for(int row = 0; row < GridData.Row; ++row)
                {
                    var cell = GameObject.Instantiate(Cell, transform);
                    if(cell == null)
                        continue;

                    cell.Init(new Cell.Data()
                    {
                        IListener = this,
                        Row = row,
                        Column = column,
                        CellSize = GridData.CellSize,
                    });

                    _cellArray[row, column] = cell;
                }
            }
        }

        Vector3 LimitBottom
        {
            get
            {
                var cell = _cellArray[1, 1];
                if (cell == null)
                {
                    //return V;
                }

                return cell.transform.position;
            }
        }

        Vector3 LimitTop
        {
            get
            {
                var cell = _cellArray[GridData.Row - 2, GridData.Column - 2];
                if (cell == null)
                {
                    //return V;
                }

                return cell.transform.position;
            }
        }

        private void Overlap()
        {
            for (int column = 0; column < GridData.Column; ++column)
            {
                for (int row = 0; row < GridData.Row; ++row)
                {
                    _cellArray[row, column]?.SetOverlap();
                }
            }
        }

        #region IGridProvider
        float IGridProvider.LimitPosY(float posY)
        {
            return Mathf.Clamp(posY, LimitBottom.y, LimitTop.y);
        }

        void IGridProvider.Overlap()
        {
            Overlap();
        }
        #endregion

        #region IUpdater
        void IUpdater.ChainUpdate()
        {
            foreach (var gridCell in _cellArray)
            {
                gridCell?.ChainUpdate();
            }
        }
        #endregion

        //private void InitLineRenderer()
        //{
        //    if(_lineRenderer == null)
        //    {
        //        return;
        //    }

        //    _lineRenderer.startWidth = 0.1f;
        //    _lineRenderer.endWidth = 0.1f;
        //    _lineRenderer.material.color = Color.white;
        //}

        //private void Make()
        //{
        //    if(_lineRenderer == null)
        //    {
        //        return;
        //    }

        //    var cellPosList = new List<Vector3>();
        //    cellPosList.Clear();

        //    float endPosZ = PosZ + Column * CellSize;
        //    var posY = transform.position.y;
        //    var currPos = new Vector3(PosX, posY, endPosZ);

        //    cellPosList.Add(new Vector3(PosX, posY, PosZ));
        //    cellPosList.Add(currPos);

        //    int toggle = -1;
        //    for(int i = 0; i < Row; ++i)
        //    {
        //        var nextPos = currPos;

        //        nextPos.x += CellSize;
        //        cellPosList.Add(nextPos);

        //        nextPos.z += (Column * toggle * CellSize);
        //        cellPosList.Add(nextPos);

        //        currPos = nextPos;
        //        toggle *= -1;
        //    }

        //    currPos.x = PosX;
        //    cellPosList.Add(currPos);

        //    int colToggle = 1;
        //    toggle = 1;
        //    if(currPos.z == endPosZ)
        //    {
        //        colToggle = -1;
        //    }

        //    for(int i = 0; i < Column; ++i)
        //    {
        //        var nextPos = currPos;

        //        nextPos.z += (colToggle * CellSize);
        //        cellPosList.Add(nextPos);

        //        nextPos.x += (Row * toggle * CellSize);
        //        cellPosList.Add(nextPos);

        //        currPos = nextPos;
        //        toggle *= -1;
        //    }

        //    _lineRenderer.positionCount = cellPosList.Count;
        //    _lineRenderer.SetPositions(cellPosList.ToArray());
        //    _lineRenderer.
        //}
    }
}
