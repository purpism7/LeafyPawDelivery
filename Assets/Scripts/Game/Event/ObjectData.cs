using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Event
{
    public class ObjectData : BaseData
    {
        public int id = 0;
    }

    public class AddObjectData : ObjectData
    {
        public OpenConditionData.EType eOpenConditionType = OpenConditionData.EType.None;
    }

    public class ArrangeObjectData : ObjectData
    {

    }
}

