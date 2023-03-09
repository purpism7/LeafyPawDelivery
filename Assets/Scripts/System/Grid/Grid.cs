using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public class Grid : MonoBehaviour
    {
        public GridData GridData;
        public Cell Cell; 

        private int[,] _cellArray;

        //private LineRenderer _lineRenderer = null;

        private void Start()
        {
            //_lineRenderer = GetComponent<LineRenderer>();

            //InitLineRenderer();
            //Make();
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
            {
                return;
            }

            _cellArray = new int[GridData.Row, GridData.Column];

            for (int i = 0; i < GridData.Column; ++i)
            {
                for(int j = 0; j < GridData.Row; ++j)
                {
                    var cell = GameObject.Instantiate(Cell, transform);
                    if(cell == null)
                    {
                        continue;
                    }

                    cell.Init(new Cell.Data()
                    {
                        Row = j,
                        Column = i,
                        CellSize = GridData.CellSize,
                    });

                    //gameObj.transform.localPosition = new Vector3(i, j, 0) * GridData.CellSize;
                    //gameObj.name = "[" + i + ", " + j + "]";
                }
            }
        }

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
