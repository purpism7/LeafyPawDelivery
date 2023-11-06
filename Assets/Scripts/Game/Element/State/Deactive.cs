using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Element.State
{
    public class Deactive<T> : BaseState<T> where T : Game.BaseElement
    {
        protected override BaseState<T> Initialize()
        {
            base.Initialize();

            return this;
        }
    }
}
