using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Element.State
{
    public class Deactive : BaseState
    {
        public override BaseState Initialize()
        {
            base.Initialize();

            return this;
        }
    }
}
