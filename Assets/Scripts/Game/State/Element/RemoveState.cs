using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class RemoveState : State.Element
    {
        public override bool Check
        {
            get
            {
                return true;
            }
        }
    }
}

