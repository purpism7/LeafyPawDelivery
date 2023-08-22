using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Object : Data.Base
{
    public int PlaceId = 0;
    public int Grade = 0;
    public int Count = 1;
    public string ShortIconImgName = string.Empty;
    public string LargeIconImgName = string.Empty;

    public Type.EObjectGrade EGrade = Type.EObjectGrade.None;

    public override void Initialize()
    {
        base.Initialize();

        System.Enum.TryParse(Grade.ToString(), out EGrade);

        Count = Grade;
        if(EGrade == Type.EObjectGrade.None)
        {
            Count = 1;
        }
    }
}
