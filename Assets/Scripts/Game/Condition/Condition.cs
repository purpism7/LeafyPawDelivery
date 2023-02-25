using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{

    public class ConditionData
    {

    }

    public abstract class Condition
    {
        public abstract bool Check();
    }

    public abstract class GenericCondition<T> : Condition where T : ConditionData 
    {
        public abstract bool Init(T TData);  
    }
}

