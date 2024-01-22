using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public interface IUpdater
    {
        void ChainUpdate();
        void ChainFixedUpdate();
    }
}
