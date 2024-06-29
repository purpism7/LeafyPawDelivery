using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using Cysharp.Threading.Tasks;

public static class UIUtils
{
    public static void SetActive(this RectTransform rectTm, bool active)
    {
        if(!rectTm)
            return;

        SetActive(rectTm.gameObject, active);
    }

    public static void SetActive(this GameObject gameObj, bool active)
    { 
        if(!gameObj)
            return;

        gameObj.SetActive(active);
    }

    public static void SetActive(this Image img, bool active)
    {
        if (img == null)
            return;

        SetActive(img.gameObject, active);
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
        color.a = 0.6f;

        img.color = color;
    }

    public static void SetOriginColorImg(Image img)
    {
        if (img == null)
            return;
        
        img.color = Color.white;
    }

    public static void DestoryChild(this RectTransform rootTm)
    {
        if (!rootTm)
            return;

        foreach (RectTransform rectTm in rootTm)
        {
            if (!rectTm)
                continue;

            GameObject.Destroy(rectTm.gameObject);
        }
    }

    public static void ResetLocalScale(this Image img)
    {
        if (img == null)
            return;
        
        img.transform.localScale = Vector3.one;
    }

    public static void StartBlink(this Image image)
    {
        if (image == null)
            return;

        Sequence sequence = DOTween.Sequence()
            .SetAutoKill(false)
            .Append(image.DOFade(0, 0.4f))
            .Append(image.DOFade(1f, 0.4f))
            .AppendInterval(0.5f);

        sequence.Restart();
        sequence.SetLoops(-1);
    }

    public static void StopBlink(this Image image)
    {
        if (image == null)
            return;

        Sequence sequence = DOTween.Sequence()
           .SetAutoKill(false)
           .Append(image.DOFade(0, 0));

        sequence.Restart();
    }

    public static void ResetScrollPos(this ScrollRect scroll)
    {
        var scrollContent = scroll?.content;
        if (scrollContent == null)
            return;

        float x = scrollContent.anchoredPosition.x;
        scrollContent.anchoredPosition = new Vector3(x, 0, 0);
    }

    public static void MoveHorizontalScrollToIndex(this ScrollRect scroll, float cellSize, int index) 
    {
        if (scroll == null ||
            scroll.content == null)
            return;

        var scrollRectTm = scroll.GetComponent<RectTransform>();
        float scrollRectTmWidth = scrollRectTm.rect.width;
        float value = (index * cellSize) / (scroll.content.rect.width - scrollRectTmWidth);

        scroll.horizontalNormalizedPosition = value;
    }

    public static void MoveVerticalScrollToIndex(this ScrollRect scroll, float cellSize, int index, bool isAnim)
    {
        if (scroll == null ||
            scroll.content == null)
            return;

        var scrollRectTm = scroll.GetComponent<RectTransform>();
        float scrollRectTmHeight = scrollRectTm.rect.height;

        float resValue = (index * cellSize) / (scroll.content.rect.height - scrollRectTmHeight);

        if(isAnim)
        {
            Sequence sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(DOTween.To(() => 1f, value => scroll.verticalNormalizedPosition = value, 1f - resValue, resValue * 0.5f).SetEase(Ease.Linear))
                .OnComplete(() =>
                {
                    //completeAction?.Invoke();
                });
            sequence.Restart();
        }
        else
        {
            scroll.verticalNormalizedPosition = 1f - resValue;
        }
    }

    public static void SetInteractable(this Button btn, bool interactable)
    {
        if (btn == null)
            return;

        btn.interactable = interactable;
    }
}
