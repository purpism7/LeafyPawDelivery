using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.State
{
    public class Game : Base
    {
        public override bool CheckEditObject
        {
            get
            {
                return false;
            }
        }
    }
}
