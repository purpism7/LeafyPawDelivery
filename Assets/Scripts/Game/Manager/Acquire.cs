using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Manager
{
    public class Acquire : Base<Acquire.Data>
    {
        public class Data : BaseData
        {

        }

        protected override void Initialize()
        {
            
        }

        public override IEnumerator CoInit(Data data)
        {
            yield return null;
        }
    }
}

