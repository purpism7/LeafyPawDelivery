using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.State
{
    public class Edit : Base
    {
        public override bool CheckControlCamera
        {
            get
            {
                return false;
            }
        }
    }
}

