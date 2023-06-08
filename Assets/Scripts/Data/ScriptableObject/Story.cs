using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    [CreateAssetMenu(menuName = "Animals/ScriptableObject/Story")]
    public class Story : ScriptableObject
    {
        [Serializable]
        public class ReqData
        {
            public Type.EOpen EOpenType = Type.EOpen.None;
            public int Id = 0;
        }
        
        [Serializable]
        public class Data
        {
            public ReqData[] ReqDatas = null;
            
            public GameObject PlayCutscene = null;

            public bool Completed = false;
        }
        
        public Data[] datas = null;
    }
}

