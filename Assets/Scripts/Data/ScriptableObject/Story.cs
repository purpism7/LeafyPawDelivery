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
            public Game.Type.EOpen EOpenType = Game.Type.EOpen.None;
            public int Id = 0;
        }
        
        [Serializable]
        public class Data
        {
            public ReqData[] ReqDatas = null;
            
            public GameObject PlayStory = null;

            public bool Completed = false;
        }

        public int PlaceId = 0;
        public Data[] Datas = null;
    }
}

