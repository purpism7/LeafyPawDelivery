using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSystem;

namespace Game.PathFinding
{
    public class AStar : MonoBehaviour
    {
        private static AStar _instance = null;

        public static void Create(IGridProvider iGridProvider)
        {
            if(_instance == null)
            {
                var gameObj = new GameObject(typeof(AStar).Name);
                _instance = gameObj.AddComponent<AStar>();

                _instance?.Initialize(iGridProvider);
            }
        }

        private void Initialize(IGridProvider iGridProvider)
        {

        }

        public static void FindPath(Node startNode, Node targetNode)
        {
            var openList = new List<Node>();
            openList.Clear();
            openList.Add(startNode);

            var closedList = new HashSet<Node>();

            while(openList?.Count > 0)
            {
                var currentNode = openList[0];

                foreach(var node in openList)
                {
                    if(node.FCost < currentNode.FCost ||
                       node.FCost == currentNode.FCost && node.HCost < currentNode.HCost)
                    {
                        currentNode = node;
                    }
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if(currentNode.Id == targetNode.Id)
                {
                    _instance?.RetracePath(startNode, targetNode);

                    break;
                }

                //foreach()
            }
        }

        private List<Node> GetNeighbourList(Node node)
        {
            var neighbourList = new List<Node>();
            neighbourList.Clear();

            return neighbourList;
        }

        // 탐색 종료  최종 노드들의 parentNode 를 추적하 리스트에 담는다.
        private void RetracePath(Node startNode, Node targetNode)
        {
            var path = new List<Node>();
            path.Clear();

            var currentNode = targetNode;

            while(currentNode.Id != startNode.Id)
            {
                path.Add(currentNode);
                currentNode = currentNode.ParentNode;
            }

            path.Reverse();
        }
    }
}

