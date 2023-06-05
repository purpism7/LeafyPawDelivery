using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    [System.Serializable]
    public class ReqOpenCondition
    {
        public enum EType
        {
            None,

            Animal,
            Object,
        }

        public EType EType_ = EType.None;
        public int Id = 0;
    }

  
    [System.Serializable]
    public class OpenCondition
    {
        public List<ReqOpenCondition> ReqList = new();
    }
}

