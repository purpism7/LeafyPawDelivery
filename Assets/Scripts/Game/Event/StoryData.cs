using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Event
{
    public enum EState
    {
        None,

        Begin,
        End,
    }

    public class StoryData : BaseData
    {
        public int Id = 0;
        public EState EState = EState.None;
    }
}
