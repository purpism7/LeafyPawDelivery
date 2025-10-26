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

            int limitLoopCnt = 9999;
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
                    await RetracePathAsync(startNode, currentNode);

                    return;
                }

                var neighbourNodeList = findNeighbourNodeFunc?.Invoke(currentNode.Id);
                if (neighbourNodeList != null)
                {
                    for (int i = 0; i < neighbourNodeList.Count; ++i)
                    {
                        var neighbourNode = neighbourNodeList[i];
                        
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
        }

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
            path.Clear();

            var currentNode = targetNode;

            while (currentNode.Id != startNode.Id)
            {
                path.Add(currentNode);

                currentNode = currentNode.ParentNode;

                if (currentNode == null)
                    break;
            }

            path.Reverse();
            Path = path;

            await UniTask.Yield();
        }
    }
}

