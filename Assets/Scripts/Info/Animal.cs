using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        
        private int _fp = 0;
        private bool[] _getFpRewards = { false, false, false };
        
        public int FriendshipPoint
        {
            get { return _fp; }
        }

        public bool GetFriendshipReward(int index)
        {
            if (_getFpRewards == null)
                return true;

            if (_getFpRewards.Length <= index)
                return true;
            
            return _getFpRewards[index];
        }

        public void AddFriendshipPoint(int point)
        {
            _fp += point;
        }

        public void SetGetFriendshipRewards(bool[] getFpRewards)
        {
            if (_getFpRewards == null)
                return;

            for (int i = 0; i < getFpRewards.Length; ++i)
            {
                if(getFpRewards.Length <= i)
                    continue;

                _getFpRewards[i] = getFpRewards[i];
            }
        }
    }
}
