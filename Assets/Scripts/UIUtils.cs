using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
