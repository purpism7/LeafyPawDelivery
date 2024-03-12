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

    public static string GetName(Type.EElement EElement, int id, int skinId = 0)
    {
        var nameKey = string.Empty;

        if (EElement == Type.EElement.Animal)
        {
            var data = AnimalContainer.Instance?.GetData(id);
            if (data == null)
                return string.Empty;

            nameKey = string.Format("name_{0}_skin_{1}", id, skinId);

        }
        else if (EElement == Type.EElement.Object)
        {
            var data = ObjectContainer.Instance?.GetData(id);
            if (data == null)
                return string.Empty;

            nameKey = string.Format("name_{0}", id);
        }

        return LocalizationSettings.StringDatabase.GetLocalizedString(EElement.ToString(), nameKey, LocalizationSettings.SelectedLocale);
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
            var placeMgr = MainGameManager.Get<Game.PlaceManager>();
            if (placeMgr == null)
                return 0;

            return placeMgr.ActivityPlaceId;
        }
    }

    public static float RandomSeed
    {
        get
        {
            var random = Random.Range(-10000f, 10000f);
            Random.InitState((int)random);

            return random * 0.00000001f;
        }
    }

    public static float PosZOffset
    {
        get
        {
            return 0.01f;
        }
    }


    public static float CalcPosZ(float value)
    {
        return value * PosZOffset + RandomSeed;
    }

    public static float GetPosZOrder(Game.Type.EPosZOrder eType)
    {
        switch(eType)
        {
            case Type.EPosZOrder.EditElement:
                return -500f;

            case Type.EPosZOrder.Object:
                return 1000f;

            case Type.EPosZOrder.DropItem:
                return -500f;

            default:
                return 0;
        }
    }
}
