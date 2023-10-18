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

        private Info.AcquireHolder _acquireHolder = new();

        protected override void Initialize()
        {
            
        }

        public override IEnumerator CoInit(Data data)
        {
            _acquireHolder?.LoadInfo();

            yield break;
        }
    }
}

 