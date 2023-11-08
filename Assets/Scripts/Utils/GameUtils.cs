using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

using Game;

public static class GameUtils
{
    public static void SetActive(this Transform tm, bool active)
    {
        if (!tm)
            return;

        SetActive(tm.gameObject, active);
    }

    public static void SetActive(this GameObject gameObj, bool active)
    {
        if (!gameObj)
            return;

        gameObj.SetActive(active);
    }

    public static bool IsActive(this GameObject gameObj)
    {
        if (!gameObj)
            return false;

        return gameObj.activeSelf;
    }

    public static bool IsActive(this Transform tm)
    {
        if (!tm)
            return false;

        return tm.gameObject.activeSelf;
    }

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
            var data = AnimalSkinContainer.Instance?.GetBaseData(id);
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
            var data = AnimalSkinContainer.Instance?.GetBaseData(id);
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


    /// <summary>
    /// 현재 활성화 상태인 Place Id.
    /// </summary>
    public static int ActivityPlaceId
    {
        get
        {
            var placeMgr = MainGameManager.Instance?.placeMgr;
            if (placeMgr == null)
                return 0;

            return placeMgr.ActivityPlaceId;
        }
    }
}
