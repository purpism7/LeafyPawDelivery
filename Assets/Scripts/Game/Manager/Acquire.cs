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

        private Dictionary<Type.EAcquire, AcquireData> _acquireDataDic = new();

        protected override void Initialize()
        {
            
        }

        public override IEnumerator CoInit(Data data)
        {
            yield return null;
        }

        public void Do(Type.EAcquire eAcquire, AcquireData acquireData)
        {

        }
    }
}

 