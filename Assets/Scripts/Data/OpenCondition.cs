using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    [CreateAssetMenu(menuName = "Animals/Data/OpenCondition")]
    public class OpenCondition : ScriptableObject
    {
        [Serializable]
        public class Data
        {
            public Type.EOpen EOpenType = Type.EOpen.None;
            public int Id = 0;
        }

        public Data Data_ = null;
        public bool Starter = false;
        
        public int ReqLeaf = 0;
        public Data[] ReqDatas = null;
        
        public bool AlreadExist { get; set; } = false;
    }
}

