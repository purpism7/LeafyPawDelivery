using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
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
        public int ReqLv = 0;
        public long ReqLeaf = 0;
        public long ReqBerry = 0;
        public List<ReqOpenCondition> ReqList = new();
    }
}

