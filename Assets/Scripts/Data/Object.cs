using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Object : ElementData
{
    public int Grade = 0;
    public int Count = 1;

    public Type.EObjectGrade EGrade = Type.EObjectGrade.None;

    public override Type.EElement EElement => Type.EElement.Object;

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

    public override int GetCurrency
    {
        get
        {
            switch(EGrade)
            {
                case Type.EObjectGrade.Epic:
                    return 10;

                case Type.EObjectGrade.Unique:
                    return 5;

                case Type.EObjectGrade.Rare:
                    return 3;

                case Type.EObjectGrade.Normal:
                    return 1;

                default:
                    return 0;
            }
        }
    }
}
