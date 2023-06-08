using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Manager
{
    public class Story : Base<Story.Data>
    {
        public class Data : BaseData
        {
            
        }

        public override IEnumerator CoInit(Data data)
        {
            yield return null;
            
            Debug.Log("Story Init");
        }
    }
}

