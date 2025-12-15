using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

namespace Game.PathFinding
{
    public class AStar
    {
        public List<Node> Path { get; private set; } = new();

        // 캔슬 토큰을 받을 수 있게 하여, 씬 이동 시 연산을 멈출 수 있게 하는 것이 좋습니다.
        public async UniTask FindPathAsync(Node startNode, Node targetNode, System.Func<int, List<Node>> findNeighbourNodeFunc)
        {
            Path.Clear();

            var openList = new List<Node>();
            openList.Add(startNode);

            var closedSet = new HashSet<Node>();

            int limitLoopCnt = 5000; // 너무 크면 렉이 걸리니 적절히 조절
            int loopCnt = 0;

            // 성능 튜닝: 몇 번 루프마다 한 번씩 쉴지 결정
            // 매번 쉬면 너무 느리고, 안 쉬면 렉 걸림. 
            // 100~500번 정도 돌고 한 프레임 쉬는 것을 추천.
            int yieldInterval = 200;

            while (openList.Count > 0)
            {
                // [수정 1] 비동기 처리의 핵심: 
                // 일정 횟수마다 제어권을 유니티에 잠시 넘겨줘서 에디터가 멈추지 않게 함
                if (loopCnt % yieldInterval == 0)
                {
                    await UniTask.Yield();
                }

                // --- 기존 로직 ---
                var currentNode = openList[0];

                for (int i = 0; i < openList.Count; ++i)
                {
                    var node = openList[i];
                    if (node == null) continue;

                    if (node.FCost < currentNode.FCost ||
                        node.FCost == currentNode.FCost && node.HCost < currentNode.HCost)
                    {
                        currentNode = node;
                    }
                }

                openList.Remove(currentNode);
                closedSet.Add(currentNode);

                // 목적지 도착
                if (currentNode.Id == targetNode.Id)
                {
                    await RetracePathAsync(startNode, currentNode);
                    return;
                }

                // 루프 제한 초과
                if (loopCnt >= limitLoopCnt)
                {
                    Debug.LogWarning("AStar Loop Limit Reached!"); // 로그로 확인 필요
                    return;
                }

                var neighbourNodeList = findNeighbourNodeFunc?.Invoke(currentNode.Id);
                if (neighbourNodeList != null)
                {
                    for (int i = 0; i < neighbourNodeList.Count; ++i)
                    {
                        var neighbourNode = neighbourNodeList[i];

                        // closedSet 체크가 Contains보다 훨씬 빠름 (HashSet 사용중이라 OK)
                        if (!neighbourNode.IsWalkAble || closedSet.Contains(neighbourNode))
                            continue;

                        int cost = currentNode.GCost + GetDistanceCost(currentNode, neighbourNode);

                        // openList.Contains는 느리지만 일단 유지 (추후 최적화 포인트)
                        bool contain = openList.Contains(neighbourNode);

                        if (cost < neighbourNode.GCost || !contain)
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
                }

                // [수정 2] loopCnt 증가 위치를 밖으로 뺌 (안전장치 확보)
                ++loopCnt;
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
            var currentNode = targetNode;

            // [수정 3] 무한루프 방지용 안전장치 추가
            int sanityCheck = 0;

            while (currentNode.Id != startNode.Id)
            {
                path.Add(currentNode);
                currentNode = currentNode.ParentNode;

                if (currentNode == null || ++sanityCheck > 5000)
                    break;
            }

            path.Reverse();
            Path = path;

            await UniTask.Yield();
        }
    }
}

//namespace Game.PathFinding
//{
//    public class AStar
//    {
//        public List<Node> Path { get; private set; } = new();

//        public async UniTask FindPathAsync(Node startNode, Node targetNode, System.Func<int, List<Node>> findNeighbourNodeFunc)
//        {
//            Path.Clear();

//            var openList = new List<Node>();
//            openList.Clear();
//            openList.Add(startNode);

//            var closedSet = new HashSet<Node>();

//            int limitLoopCnt = 9999;
//            int loopCnt = 0;
//            while (openList.Count > 0)
//            {
//                var currentNode = openList[0];

//                for (int i = 0; i < openList.Count; ++i)
//                {
//                    var node = openList[i];
//                    if (node == null)
//                        continue;

//                    if (node.FCost < currentNode.FCost ||
//                        node.FCost == currentNode.FCost && node.HCost < currentNode.HCost)
//                    {
//                        currentNode = node;
//                    }
//                }

//                openList.Remove(currentNode);
//                closedSet.Add(currentNode);

//                if (currentNode.Id == targetNode.Id ||
//                    loopCnt >= limitLoopCnt)
//                {
//                    await RetracePathAsync(startNode, currentNode);

//                    return;
//                }

//                var neighbourNodeList = findNeighbourNodeFunc?.Invoke(currentNode.Id);
//                if (neighbourNodeList != null)
//                {
//                    for (int i = 0; i < neighbourNodeList.Count; ++i)
//                    {
//                        var neighbourNode = neighbourNodeList[i];
                        
//                        if (!neighbourNode.IsWalkAble ||
//                            closedSet.Contains(neighbourNode))
//                            continue;
                        
//                        int cost = currentNode.GCost + GetDistanceCost(currentNode, neighbourNode);
//                        bool contain = openList.Contains(neighbourNode);
//                        if (cost < neighbourNode.GCost ||
//                            !contain)
//                        {
//                            neighbourNode.GCost = cost;
//                            neighbourNode.HCost = GetDistanceCost(neighbourNode, targetNode);
//                            neighbourNode.ParentNode = currentNode;

//                            if (!contain)
//                            {
//                                openList.Add(neighbourNode);
//                            }
//                        }
//                    }
                    
//                    ++loopCnt;
//                }
//            }
//        }

//        private int GetDistanceCost(Node node, Node compNode)
//        {
//            int distX = Mathf.Abs(node.X - compNode.X);
//            int distY = Mathf.Abs(node.Y - compNode.Y);

//            if (distX > distY)
//                return 14 * distY + 10 * (distX - distY);

//            return 14 * distX + 10 * (distY - distX);
//        }

//        private async UniTask RetracePathAsync(Node startNode, Node targetNode)
//        {
//            var path = new List<Node>();
//            path.Clear();

//            var currentNode = targetNode;

//            while (currentNode.Id != startNode.Id)
//            {
//                path.Add(currentNode);

//                currentNode = currentNode.ParentNode;

//                if (currentNode == null)
//                    break;
//            }

//            path.Reverse();
//            Path = path;

//            await UniTask.Yield();
//        }
//    }
//}

