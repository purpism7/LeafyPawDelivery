using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game.PathFinding
{
    public class NodeBase
    {
        public NodeBase Connection { get; private set; }

        public float G { get; private set; }
        public float H { get; private set; }

        public float F => G + H;

        public void SetConnection(NodeBase nodeBase) => Connection = nodeBase;

        public void SetG(float g) => G = g;
        public void SetH(float h) => H = h;


        public static List<NodeBase> FindPath(NodeBase startNode, NodeBase targetNode)
        {
            var toSearchList = new List<NodeBase>() { startNode, };
            var processedList = new List<NodeBase>();

            while(toSearchList.Any())
            {
                var currenct = toSearchList[0];

                foreach(var toSearch in toSearchList)
                {
                    if(toSearch.F < currenct.F ||
                       toSearch.F == currenct.F && toSearch.H < currenct.H)
                    {
                        currenct = toSearch;
                    }
                }

                processedList.Add(currenct);
                toSearchList.Remove(currenct);

                if(currenct == targetNode)
                {
                    var currentPath = targetNode;
                    var path = new List<NodeBase>();

                    while(currentPath != targetNode)
                    {
                        path.Add(currentPath);

                        currentPath = currentPath.Connection;
                    }

                    return path;
                }

                //foreach (var neighbor in currenct.Nei)
            }

            return null;
        }
    }
}

