using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EditState : State.Element
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

