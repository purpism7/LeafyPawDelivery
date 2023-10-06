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
    public string ShortIconImgName = string.Empty;
    public string LargeIconImgName = string.Empty;

    public Game.Type.EAnimalSkin EAnimalSkin = Game.Type.EAnimalSkin.Base;

    public override void Initialize()
    {
        base.Initialize();

        System.Enum.TryParse(Type, out EAnimalSkin);

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
