using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

namespace Game.PathFinding
{
    public class AStar
    {
        public List<Node> Path { get; private set; } = new();

        public async UniTask FindPathAsync(Node startNode, Node targetNode, System.Func<int, List<Node>> findNeighbourNodeFunc)
        {
            Path.Clear();

            var openList = new List<Node>();
            openList.Clear();
            openList.Add(startNode);

            var closedSet = new HashSet<Node>();

            float limitLoopCnt = 1000f;
            var gameCameraCtr = MainGameManager.Instance?.IGameCameraCtr as GameSystem.GameCameraController;
            if(gameCameraCtr != null)
            {
                limitLoopCnt = gameCameraCtr.GameCamera.orthographicSize;
            }

            int loopCnt = 0;
            while (openList.Count > 0)
            {
                var currentNode = openList[0];

                for (int i = 0; i < openList.Count; ++i)
                {
                    var node = openList[i];
                    if (node == null)
                        continue;

                    if (node.FCost < currentNode.FCost ||
                        node.FCost == currentNode.FCost && node.HCost < currentNode.HCost)
                    {
                        currentNode = node;
                    }
                }

                openList.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode.Id == targetNode.Id ||
                    loopCnt >= limitLoopCnt)
                {
                    Debug.Log(loopCnt);

                    await RetracePathAsync(startNode, currentNode);

                    return;
                }

                var neighbourNodeList = findNeighbourNodeFunc?.Invoke(currentNode.Id);
                foreach (var neighbourNode in neighbourNodeList)
                {
                    if (!neighbourNode.IsWalkAble ||
                        closedSet.Contains(neighbourNode))
                        continue;

                    int cost = currentNode.GCost + GetDistanceCost(currentNode, neighbourNode);
                    bool contain = openList.Contains(neighbourNode);
                    if (cost < neighbourNode.GCost ||
                       !contain)
                    {
                        neighbourNode.GCost = cost;
                        neighbourNode.HCost = GetDistanceCost(neighbourNode, targetNode);
                        neighbourNode.ParentNode = currentNode;

                        if (!contain)
                        {
                            openList.Add(neighbourNode);
                        }
                    }
                }

                ++loopCnt;
            }
        }

        //public void FindPath(Node startNode, Node targetNode, System.Func<int, List<Node>> findNeighbourNodeFunc)
        //{
        //    Path.Clear();

        //    var openList = new List<Node>();
        //    openList.Clear();
        //    openList.Add(startNode);

        //    var closedSet = new HashSet<Node>();

        //    while(openList?.Count > 0)
        //    {
        //        var currentNode = openList[0];

        //        for(int i = 0; i < openList.Count; ++i)
        //        {
        //            var node = openList[i];
        //            if (node == null)
        //                continue;

        //            if (node.FCost < currentNode.FCost ||
        //                node.FCost == currentNode.FCost && node.HCost < currentNode.HCost)
        //            {
        //                currentNode = node;
        //            }
        //        }

        //        openList.Remove(currentNode);
        //        closedSet.Add(currentNode);

        //        if(currentNode.Id == targetNode.Id)
        //        {
        //            RetracePath(startNode, currentNode);

        //            break;
        //        }

        //        var neighbourNodeList = findNeighbourNodeFunc?.Invoke(currentNode.Id);
        //        foreach(var neighbourNode in neighbourNodeList)
        //        {
        //            if (!neighbourNode.IsWalkAble ||
        //                closedSet.Contains(neighbourNode))
        //                continue;

        //            int cost = currentNode.GCost + GetDistanceCost(currentNode, neighbourNode);
        //            bool contain = openList.Contains(neighbourNode);
        //            if (cost < neighbourNode.GCost ||
        //               !contain)
        //            {
        //                neighbourNode.GCost = cost;
        //                neighbourNode.HCost = GetDistanceCost(neighbourNode, targetNode);
        //                neighbourNode.ParentNode = currentNode;

        //                if(!contain)
        //                {
        //                    openList.Add(neighbourNode);
        //                }
        //            }
        //        }
        //    }
        //}

        private int GetDistanceCost(Node node, Node compNode)
        {
            int distX = Mathf.Abs(node.X - compNode.X);
            int distY = Mathf.Abs(node.Y - compNode.Y);

            if (distX > distY)
                return 14 * distY + 10 * (distX - distY);

            return 14 * distX + 10 * (distY - distX);
        }

        private async UniTask RetracePathAsync(Node startNode, Node targetNode)
        {
            var path = new List<Node>();
            path?.Clear();

            var currentNode = targetNode;

            while (currentNode.Id != startNode.Id)
            {
                path?.Add(currentNode);

                currentNode = currentNode.ParentNode;

                if (currentNode == null)
                    break;
            }

            path?.Reverse();

            Path = path;

            await UniTask.Yield();
        }

        // 탐색 종료 최종 노드들의 parentNode 를 추적하여 리스트에 담는다.
        //private void RetracePath(Node startNode, Node targetNode)
        //{
        //    var path = new List<Node>();
        //    path?.Clear();

        //    var currentNode = targetNode;

        //    while(currentNode.Id != startNode.Id)
        //    {
        //        path?.Add(currentNode);
                
        //        currentNode = currentNode.ParentNode;

        //        if (currentNode == null)
        //        {
        //            Debug.Log("CurrentNode is null");

        //            break;
        //        }
        //    }

        //    path?.Reverse();

        //    Path = path;
        //}
    }
}

