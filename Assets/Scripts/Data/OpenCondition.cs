using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;

[System.Serializable]
public class OpeCondition : Data.Base
{
    public enum EType
    {
        None,

        Starter,
        Buy,
    };
    
    public EType Type = EType.None;
    public int AnimalCurrency = 0;
    public int ObjectCurrency = 0;
    public int Cash = 0;
    public int[] ReqAnimalIds = null;
    public int[] ReqObjectIds = null;

    public void Initialize()
    {
        Debug.Log("Initialize()");
    }

}
