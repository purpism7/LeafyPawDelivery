using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSystem;

namespace Game
{
    public class Carrier : MonoBehaviour
    {
        private static Carrier _instance = null;

        public static void Create(IGridCell iGridCell)
        {
            if (_instance == null)
            {
                var gameObj = new GameObject(typeof(Carrier).Name);
                _instance = gameObj.AddComponent<Carrier>();
                
                _instance?.Initialize(iGridCell);
            }
        }

        public static bool Move(Vector3 targetPos, out List<Vector3> pathPosList)
        {
            pathPosList = null;

            if (_instance == null)
                return false;

            return _instance.MoveByAStar(targetPos, out pathPosList);
        }

        private IGridCell _iGridCell = null;
        private PathFinding.AStar _aStar = null;

        private void Initialize(IGridCell iGridCell)
        {
            _iGridCell = iGridCell;

            _aStar = new();
        }

        private bool MoveByAStar(Vector3 targetPos, out List<Vector3> pathPosList)
        {
            pathPosList = null;

            var iGridCell = _iGridCell;
            if (iGridCell == null)
                return false;

            if (_aStar == null)
                return false;

            var cell = _iGridCell.GetCell(targetPos);
            if (cell == null)
                return false;

            Debug.Log(cell.name);

            var targetCell = _iGridCell.GetCell(GetRandomPos(targetPos.z));
            if (targetCell.IsOverlap)
                return false;

            var startNode = new PathFinding.Node(cell.Id, !cell.IsOverlap, cell.Row, cell.Column);
            var targetNode = new PathFinding.Node(targetCell.Id, true, targetCell.Row, targetCell.Column);

            //_iGridCell.Reset();

            _aStar.FindPath(startNode, targetNode, GetNeighbourNodeList);

            pathPosList = new();
            pathPosList.Clear();

            foreach (var path in _aStar.Path)
            {
                if (path == null)
                    continue;

                var pathCell = _iGridCell?.GetCell(path.Id);
                if (pathCell == null)
                    continue;

                //pathCell.Path = true;
                pathPosList.Add(pathCell.transform.position);
            }

            return true;
        }

        private Vector3 GetRandomPos(float z)
        {
            Vector3 pos = Vector3.zero;
            var gameCameraCtr = MainGameManager.Instance?.GameCameraCtr;
            if (gameCameraCtr == null)
                return Vector3.zero;

            var center = gameCameraCtr.Center;
            var halfWidth = (gameCameraCtr.Width - 200f) / 2f;
            var halfHeight = (gameCameraCtr.Height - 850f) / 2f;

            var randomX = Random.Range(center.x - halfWidth, center.x + halfWidth);
            var randomY = Random.Range(center.y - halfHeight, center.y + halfHeight);
            randomY = gameCameraCtr.IGrid.LimitPosY(randomY);

            return new Vector3(randomX, randomY, z);
        }

        private List<PathFinding.Node> GetNeighbourNodeList(int id)
        {
            List<PathFinding.Node> neighbourNodeList = new();
            neighbourNodeList.Clear();

            var neighbourList = _iGridCell.GetNeighbourList(id);
            foreach (var neighbourCell in neighbourList)
            {
                if (neighbourCell == null)
                    continue;

                var node = new PathFinding.Node(neighbourCell.Id, !neighbourCell.IsOverlap, neighbourCell.Row, neighbourCell.Column);

                neighbourNodeList.Add(node);
            }

            return neighbourNodeList;
        }
    }
}

