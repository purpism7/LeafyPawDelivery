using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public static class GameUtils
{
    public static string GetName(Type.EElement EElement, int id)
    {
        var name = string.Empty;

        if (EElement == Type.EElement.Animal)
        {
            var data = AnimalContainer.Instance?.GetData(id);
            if (data == null)
                return string.Empty;
        }
        else if (EElement == Type.EElement.Object)
        {
            var data = ObjectContainer.Instance?.GetData(id);
            if (data == null)
                return string.Empty;
        }

        return LocalizationSettings.StringDatabase.GetLocalizedString(EElement.ToString(), "name_" + id, LocalizationSettings.SelectedLocale);
    }
    
    public static Sprite GetShortIconSprite(Type.EElement EElement, int id)
    {
        var atlasLoader = GameSystem.ResourceManager.Instance?.AtalsLoader;
        if (atlasLoader == null)
            return null;

        if (EElement == Type.EElement.Animal)
        {
            var data = AnimalContainer.Instance?.GetData(id);
            if (data == null)
                return null;

            return atlasLoader?.GetAnimalIconSprite(data.ShortIconImgName);
        }
        else if (EElement == Type.EElement.Object)
        {
            var data = ObjectContainer.Instance?.GetData(id);
            if (data == null)
                return null;

            return atlasLoader?.GetObjectIconSprite(data.ShortIconImgName);
        }

        return null;
    }
    
     public static Sprite GetLargeIconSprite(Type.EElement EElement, int id)
     {
         var atlasLoader = GameSystem.ResourceManager.Instance?.AtalsLoader;
         if (atlasLoader == null)
             return null;
    
         if (EElement == Type.EElement.Animal)
         {
             var data = AnimalContainer.Instance?.GetData(id);
             if (data == null)
                 return null;
    
             return atlasLoader.GetAnimalIconSprite(data.LargeIconImgName);
         }
         else if (EElement == Type.EElement.Object)
         {
             var data = ObjectContainer.Instance?.GetData(id);
             if (data == null)
                 return null;
    
             return atlasLoader.GetObjectIconSprite(data.LargeIconImgName);
         }
    
         return null;
     }
}
