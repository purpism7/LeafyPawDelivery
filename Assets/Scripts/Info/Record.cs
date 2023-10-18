using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    [System.Serializable]
    public class BaseRecord
    {
        public long Value = 0;
    }

    [System.Serializable]
    public class Record
    {
        public long Value = 0;

        [SerializeField]
        private string Acquire = string.Empty;
        [SerializeField]
        private string AcquireAction = string.Empty;

        public Game.Type.EAcquire EAcquire { get; private set; } = Game.Type.EAcquire.None;
        public Game.Type.EAcquireAction EAcquireAction { get; private set; } = Game.Type.EAcquireAction.None;

        public Record(Game.Type.EAcquire eAcquire, Game.Type.EAcquireAction eAcquireAction)
        {
            EAcquire = eAcquire;
            EAcquireAction = eAcquireAction;

            Acquire = EAcquire.ToString();
            AcquireAction = EAcquireAction.ToString();
        }
    }
}

