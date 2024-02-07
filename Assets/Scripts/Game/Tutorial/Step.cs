using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tutorial
{
    public abstract class Step : MonoBehaviour
    {
        public abstract void Begin();
        public abstract void End();
        public abstract void ChainUpdate();
    }
}

