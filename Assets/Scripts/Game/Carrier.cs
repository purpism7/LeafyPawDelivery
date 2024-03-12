using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

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
                if (!gameObj)
                    return;

                gameObj.transform.SetParent(MainGameManager.Instance?.transform);
                _instance = gameObj.GetOrAddComponent<Carrier>();
            }

            _instance?.Initialize(iGridCell);
        }

        public static async UniTask<List<Vector3>> MoveAsync(Vector3 targetPos)
        {
            if (_instance == null)
                return null;

            return await _instance.MoveByAStarAsync(targetPos);
        }

        private IGridCell _iGridCell = null;
        private PathFinding.AStar _aStar = null;

        private void Initialize(IGridCell iGridCell)
        {
            _iGridCell = iGridCell;

            _aStar = new();
        }

        private async UniTask<List<Vector3>> MoveByAStarAsync(Vector3 targetPos)
        {
            List<Vector3> pathPosList = null;

            var iGridCell = _iGridCell;
            if (iGridCell == null)
                return pathPosList;

            if (_aStar == null)
                return pathPosList;

            _aStar.Path.Clear();

            var cell = _iGridCell.GetCell(targetPos);
            if (cell == null)
                return pathPosList;

            var targetCell = _iGridCell.GetCell(GetRandomPos(targetPos.z));
            if (targetCell.IsOverlap)
                return pathPosList;

            var startNode = new PathFinding.Node(cell.Id, !cell.IsOverlap, cell.Row, cell.Column);
            var targetNode = new PathFinding.Node(targetCell.Id, true, targetCell.Row, targetCell.Column);

            await _aStar.FindPathAsync(startNode, targetNode, GetNeighbourNodeList);

            pathPosList = new();
            pathPosList.Clear();

            foreach (var path in _aStar.Path)
            {
                if (path == null)
                    continue;

                var pathCell = _iGridCell?.GetCell(path.Id);
                if (pathCell == null)
                    continue;

                pathPosList.Add(pathCell.transform.position);
            }

            return pathPosList;

        }

        private Vector3 GetRandomPos(float z)
        {
            var iGameCameraCtr = MainGameManager.Instance?.IGameCameraCtr;
            if (iGameCameraCtr == null)
                return Vector3.zero;

            var randomX = iGameCameraCtr.RandPosXInScreenRagne;
            var randomY = iGameCameraCtr.RandPosYInScreenRagne;

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

