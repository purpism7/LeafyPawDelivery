using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PlaceEvent
{
    public class BaseData
    {
       
    }

    public class DropItemData : BaseData
    {
        public Game.Type.EItemSub eItemSub = Type.EItemSub.None;
        public int currCnt = 0;
    }
}

