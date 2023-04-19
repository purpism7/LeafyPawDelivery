using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Edit : ObjectState
    {
        public EditCommand Command { get; private set; } = null;

        public override void Apply(Game.Object obj)
        {

        }
    }
}

