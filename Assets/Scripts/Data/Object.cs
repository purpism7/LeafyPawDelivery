using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Object : ElementData
{
    [SerializeField]
    private int Grade = 0;
    public int Count = 1;
    [SerializeField]
    private int order = 0;

    public Game.Type.EObjectGrade EGrade = Game.Type.EObjectGrade.None;
    public string ShortIconImgName = string.Empty;
    public string LargeIconImgName = string.Empty;

    public override Game.Type.EElement EElement => Game.Type.EElement.Object;

    public int Order { get { return order; } }

    public override void Initialize()
    {
        base.Initialize();

        System.Enum.TryParse(Grade.ToString(), out EGrade);

        Count = Grade;
        if(EGrade == Game.Type.EObjectGrade.None)
        {
            Count = 1;
        }

        string placeId = PlaceId > 9 ? PlaceId.ToString() : "0" + PlaceId;
        string id = Id > 9 ? Id.ToString() : "0" + Id;

        ShortIconImgName = string.Format("EditIcon_Map{0}_Object_{1}", placeId, id);
        LargeIconImgName = string.Format("BookIcon_Map{0}_Object_{1}", placeId, id);
    }

    public override int Currency
    {
        get
        {
            switch(EGrade)
            {
                case Game.Type.EObjectGrade.Unique:
                    return 50;

                case Game.Type.EObjectGrade.Epic:
                    return 40;

                case Game.Type.EObjectGrade.Rare:
                    return 35;

                case Game.Type.EObjectGrade.Normal:
                    return 23;

                default:
                    return 0;
            }
        }
    }
}
