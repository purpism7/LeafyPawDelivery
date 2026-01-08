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
        public Type.ObjectType ObjectType { get; private set; } = Type.ObjectType.None;

        public AddObjectData WithObjectType(Type.ObjectType type)
        {
            ObjectType = type;
            return this;
        }
    }

    public class ArrangeObjectData : ObjectData
    {

    }
}

