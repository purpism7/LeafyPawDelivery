using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using Cysharp.Threading.Tasks;

public static class UIUtils
{
    // public static void SetActive(this Component component, bool active)
    // {
    //     if (!component)
    //         return;
    //
    //     component.gameObject.SetActive(active);
    // }

    public static void SetSpritie(this SpriteRenderer spriteRenderer, Sprite sprite)
    {
        if(spriteRenderer == null)
            return;

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

    public static void MoveVerticalScrollToIndex(this ScrollRect scrollRect, RectTransform target, bool isAnim)
    {
        Canvas.ForceUpdateCanvases();

        if (scrollRect == null || 
            scrollRect.content == null || 
            target == null) 
            return;

        RectTransform viewport = scrollRect.viewport != null ? scrollRect.viewport : scrollRect.GetComponent<RectTransform>();
        float scrollableHeight = scrollRect.content.rect.height - viewport.rect.height;

        if (scrollableHeight <= 0)
        {
            scrollRect.verticalNormalizedPosition = 1f;
            return;
        }
        
        float targetPosY = -target.anchoredPosition.y; // �⺻ �Ÿ�

        float pivotOffset = (1f - target.pivot.y) * target.rect.height;
        targetPosY -= pivotOffset;

        float normalizePos = 1f - (targetPosY / scrollableHeight);

        
        normalizePos = Mathf.Clamp01(normalizePos);
        
        if (isAnim)
        {
            scrollRect.DOKill();
            scrollRect.DOVerticalNormalizedPos(normalizePos, 0.3f).SetEase(Ease.OutQuad);
        }
        else
        {
            scrollRect.verticalNormalizedPosition = normalizePos;
        }
    }

    public static void SetInteractable(this Button btn, bool interactable)
    {
        if (btn == null)
            return;

        btn.interactable = interactable;
    }
}
