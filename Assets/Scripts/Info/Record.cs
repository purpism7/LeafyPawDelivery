using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    [System.Serializable]
    public class BaseRecord
    {
        public int Value = 0;
    }

    [System.Serializable]
    public class AcquireRecord : BaseRecord
    {
        public Game.Type.EAcquire EAcquire = Game.Type.EAcquire.None;
        public Game.Type.EAcquireAction EAcquireAction  = Game.Type.EAcquireAction.None;
    }
}

