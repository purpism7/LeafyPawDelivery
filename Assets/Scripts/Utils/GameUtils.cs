using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameUtils
{
    public static string GetName(Type.EElement EElement, int id)
    {
        if (EElement == Type.EElement.Animal)
        {
            var data = AnimalContainer.Instance?.GetData(id);
            if (data == null)
                return string.Empty;

            return data.Name;
        }
        else if (EElement == Type.EElement.Object)
        {
            var data = ObjectContainer.Instance?.GetData(id);
            if (data == null)
                return string.Empty;

            return data.Name;
        }

        return string.Empty;
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

            return atlasLoader?.GetSprite(atlasLoader.KeyAnimalIcon, data.ShortIconImgName);
        }
        else if (EElement == Type.EElement.Object)
        {
            var data = ObjectContainer.Instance?.GetData(id);
            if (data == null)
                return null;

            return atlasLoader?.GetSprite(atlasLoader.KeyObjectIcon, data.ShortIconImgName);
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
    
             return atlasLoader.GetSprite(atlasLoader.KeyAnimalIcon, data.LargeIconImgName);
         }
         else if (EElement == Type.EElement.Object)
         {
             var data = ObjectContainer.Instance?.GetData(id);
             if (data == null)
                 return null;
    
             return atlasLoader.GetSprite(atlasLoader.KeyObjectIcon, data.LargeIconImgName);
         }
    
         return null;
     }
}
