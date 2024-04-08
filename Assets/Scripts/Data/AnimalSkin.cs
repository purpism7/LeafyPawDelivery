using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimalSkin : Data.Base
{
    public int AnimalId = 0;
    public int Cash = 0;
    public string Type = string.Empty;
    public bool Advertising = false;
    public string ImgName = string.Empty;
    [SerializeField]
    private int bonus = 0;

    public Game.Type.EAnimalSkin EAnimalSkin = Game.Type.EAnimalSkin.Base;
    public int Bonus { get { return bonus; } }

    public string GetShortIconImgName(int placeId)
    {
        return string.Format("EditIcon_Map{0}_Animal_{1}", placeId, AnimalId);
    }

    public string GetLargeIconImgName(int placeId)
    {
        return string.Format("BookIcon_Map{0}_Animal_{1}", placeId, AnimalId);
    }

    public override void Initialize()
    {
        base.Initialize();

        System.Enum.TryParse(Type, out EAnimalSkin);

        if(EAnimalSkin != Game.Type.EAnimalSkin.Base)
        {
            bonus = 5;
        }

        //if(string.IsNullOrEmpty(ShortIconImgName))
        //{
        //    var animalSkinData = AnimalSkinContainer.Instance?.GetBaseData(AnimalId);
        //    if(animalSkinData != null)
        //    {
        //        ShortIconImgName = animalSkinData.ShortIconImgName;
        //    }
        //}
    }
}
