using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    [System.Serializable]
    public class Animal
    {
        public int Id = 0;
        public Vector3 Pos = Vector3.zero;
        public bool Arrangement = false;
        public int SkinId = 0;
        public List<int> SkinIdList = null;
        
        private int fp = 0;
        
        public int FriendshipPoint
        {
            get { return fp; }
        }

        public void AddFriendshipPoint(int point)
        {
            fp += point;
        }
    }
}
