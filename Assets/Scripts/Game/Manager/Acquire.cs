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

        public void Add(Type.EAcquire eAcquire, Type.EAcquireAction eAcquireAction, int value)
        {
            _acquireHolder?.Add(eAcquire, eAcquireAction, value);
        }
    }
}

 