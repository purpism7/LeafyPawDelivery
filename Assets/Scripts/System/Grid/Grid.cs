using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace GameSystem
{
    public interface IGrid
    {
        float LimitPosY(float posY);
        void Overlap();
    }

    public interface IGridCell
    {
        //void Reset();
        Cell GetCell(int id);
        Cell GetCell(Vector3 pos);
        List<Cell> GetNeighbourList(int id);
    }

    public class Grid : MonoBehaviour, IGrid, IGridCell, IUpdater
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
            if (GridData == null)
                return;

            int index = 0;

            _cellArray = new Cell[GridData.Row, GridData.Column];

            for (int column = 0; column < GridData.Column; ++column)
            {
                for (int row = 0; row < GridData.Row; ++row)
                {
                    var cell = GameObject.Instantiate(Cell, transform);
                    if (cell == null)
                        continue;

                    cell.Init(new Cell.Data()
                    {
                        Id = index++,
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
                var cell = _cellArray[GridData.Row - 2, GridData.Column - 3];
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
        float IGrid.LimitPosY(float posY)
        {
            return Mathf.Clamp(posY, LimitBottom.y, LimitTop.y);
        }

        void IGrid.Overlap()
        {
            Overlap();
        }
        #endregion

        #region IGridCell
        //void IGridCell.Reset()
        //{
        //    for (int column = 0; column < GridData.Column; ++column)
        //    {
        //        for (int row = 0; row < GridData.Row; ++row)
        //        {
        //            var cell = _cellArray[row, column];
        //            if (cell == null)
        //                continue;

        //            cell.Path = false;
        //        }
        //    }
        //}

        Cell IGridCell.GetCell(int id)
        {
            for (int column = 0; column < GridData.Column; ++column)
            {
                for (int row = 0; row < GridData.Row; ++row)
                {
                    var cell = _cellArray[row, column];
                    if (cell == null)
                        continue;

                    if (cell.Id != id)
                        continue;

                    return cell;
                }
            }

            return null;
        }

        Cell IGridCell.GetCell(Vector3 pos)
        {
            var cellHalfSize = GridData.CellSize * 0.5f;

            for (int column = 0; column < GridData.Column; ++column)
            {
                for (int row = 0; row < GridData.Row; ++row)
                {
                    var cell = _cellArray[row, column];
                    if (cell == null)
                        continue;

                    var cellPos = cell.transform.position;

                    if (cellPos.x - cellHalfSize >= pos.x ||
                        cellPos.x + cellHalfSize < pos.x)
                        continue;

                    if (cellPos.y - cellHalfSize >= pos.y ||
                        cellPos.y + cellHalfSize < pos.y)
                        continue;

                    return cell;
                }
            }

            return null;
        }

        List<Cell> IGridCell.GetNeighbourList(int id)
        {
            List<Cell> cellList = new List<Cell>();
            cellList.Clear();

            for (int column = 0; column < GridData.Column; ++column)
            {
                for (int row = 0; row < GridData.Row; ++row)
                {
                    var cell = _cellArray[row, column];
                    if (cell == null)
                        continue;

                    if (!CheckNeighbour(id, cell.Id))
                        continue;

                    cellList.Add(cell);
                }
            }

            return cellList;
        }

        private bool CheckNeighbour(int targetId, int cellId)
        {
            int row = GridData.Row;

            int left = targetId - 1;
            int right = targetId + 1;
            int down = targetId - row;
            int up = targetId + row;
            int leftDown = down - 1;
            int rightDown = down + 1;
            int leftUp = up - 1;
            int rightUp = up + 1;

            if (cellId == up ||
               cellId == down)
                return true;

            if (cellId == left ||
               cellId == right)
                return cellId / row == targetId / row;

            if(cellId == leftDown)
                return down / row == leftDown / row;

            if(cellId == rightDown)
                return down / row == rightDown / row;

            if (cellId == leftUp)
                return up / row == leftUp / row;

            if (cellId == rightUp)
                return up / row == rightUp / row;

            return false;
        }
        #endregion

        #region IUpdater
        void IUpdater.ChainUpdate()
        {
            //foreach (var gridCell in _cellArray)
            //{
            //    gridCell?.ChainUpdate();
            //}
            return;
        }
        #endregion
    }
}
