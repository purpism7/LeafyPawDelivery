using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenConditionContainer<T> : BaseContainer<T, OpeCondition> where T : new()
{

    public virtual void a(Type.EMain eMain, int id)
    {
        var data = GetData(id);



        
    } 
}