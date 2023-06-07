using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtils
{
    public static void SetActive(this Transform tm, bool active)
    {
        if(!tm)
        {
            return;
        }

        SetActive(tm.gameObject, active);
    }

    public static void SetActive(this GameObject gameObj, bool active)
    { 
        if(!gameObj)
        {
            return;
        }

        gameObj.SetActive(active);
    }

    public static void SetSpritie(this SpriteRenderer spriteRenderer, Sprite sprite)
    {
        if(spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.sprite = sprite;
    }

    public static void SetAnimalIcon(Image iconImg, string name)
    {
        if (iconImg == null)
            return;

        var atlasLoader = GameSystem.ResourceManager.Instance?.AtalsLoader;
        if (atlasLoader == null)
            return;


        iconImg.sprite = atlasLoader?.GetSprite(atlasLoader.KeyAnimalIcon, name);
    }

    public static void SetSilhouetteColorImg(Image img)
    {
        if (img == null)
            return;

        Color color = Color.black;
        color.a = 200f;

        img.color = color;
    }

    public static void SetOriginColorImg(Image img)
    {
        if (img == null)
            return;
        
        img.color = Color.white;
    }
}
