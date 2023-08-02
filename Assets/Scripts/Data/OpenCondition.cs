using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;

[System.Serializable]
public class OpenCondition : Data.Base
{
    public enum EType
    {
        None,

        Starter,
        Buy,
    };

    public string Type = string.Empty;
    public int AnimalCurrency = 0;
    public int ObjectCurrency = 0;
    public int Cash = 0;
    public bool Advertising = false;
    public int[] ReqAnimalIds = null;
    public int[] ReqObjectIds = null;

    public EType EType_ = EType.None;

    public override void Initialize()
    {
        base.Initialize();
       
        System.Enum.TryParse(Type, out EType_);
    }
}
