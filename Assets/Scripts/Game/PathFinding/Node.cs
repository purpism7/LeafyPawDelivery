using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game.PathFinding
{
    public class Node
    {
        public bool IsWalkAble { get; private set; } = false;
        public int GCost { get; private set; } = 0;
        public int HCost { get; private set; } = 0;

        public Node ParentNode { get; private set; } = null;

        public int FCost { get { return GCost + HCost; } }

        public Node(bool isWalkAble, int GCost, int HCost)
        {

        }
    }
}

