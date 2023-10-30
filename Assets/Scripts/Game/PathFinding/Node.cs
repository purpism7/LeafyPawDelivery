using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game.PathFinding
{
    public class Node
    {
        public int Id { get; private set; } = 0;
        public bool IsWalkAble { get; private set; } = false;

        public Node ParentNode = null;

        public int HCost = 0;
        public int GCost = 0;
        public int FCost { get { return GCost + HCost; } }

        public int X { get; private set; } = 0;
        public int Y { get; private set; } = 0;

        public Node(int id, bool isWalkAble, int x, int y)
        {
            Id = id;
            IsWalkAble = isWalkAble;

            X = x;
            Y = y;
        }
    }
}

