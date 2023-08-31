using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Object : ElementData
{
    public int Grade = 0;
    public int Count = 1;

    public Game.Type.EObjectGrade EGrade = Game.Type.EObjectGrade.None;

    public override Game.Type.EElement EElement => Game.Type.EElement.Object;

    public override void Initialize()
    {
        base.Initialize();

        System.Enum.TryParse(Grade.ToString(), out EGrade);

        Count = Grade;
        if(EGrade == Game.Type.EObjectGrade.None)
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
                case Game.Type.EObjectGrade.Epic:
                    return 1000;

                case Game.Type.EObjectGrade.Unique:
                    return 800;

                case Game.Type.EObjectGrade.Rare:
                    return 700;

                case Game.Type.EObjectGrade.Normal:
                    return 450;

                default:
                    return 0;
            }
        }
    }
}
