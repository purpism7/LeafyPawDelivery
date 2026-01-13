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
            target == null) return;

        RectTransform viewport = scrollRect.viewport != null ? scrollRect.viewport : scrollRect.GetComponent<RectTransform>();
        float scrollableHeight = scrollRect.content.rect.height - viewport.rect.height;

        // 스크롤할 공간이 없으면(컨텐츠가 화면보다 작으면) 맨 위로 보내고 종료
        if (scrollableHeight <= 0)
        {
            scrollRect.verticalNormalizedPosition = 1f;
            return;
        }

        // 3. 타겟의 Y 위치 계산 (Content의 최상단으로부터 얼마나 떨어져 있는지)
        // anchoredPosition.y는 보통 음수값(아래로 내려감)이므로 -를 붙여 양수 거리로 만듭니다.
        // Pivot(피벗) 보정: 타겟의 피벗이 중앙(0.5)이면, 상단 좌표를 구하기 위해 높이의 절반을 더해줍니다.

        float targetPosY = -target.anchoredPosition.y; // 기본 거리

        // (타겟 피벗이 Top(1)이 아니라면 보정 필요. 보통 UI는 Top(1)이나 Center(0.5)를 씁니다)
        // 타겟의 피벗이 1이면 그대로, 0.5면 높이/2 만큼 덜 내려간 위치가 상단임.
        float pivotOffset = (1f - target.pivot.y) * target.rect.height;
        targetPosY -= pivotOffset;

        // 4. 비율 계산 (0~1)
        // ScrollRect는 1이 맨 위, 0이 맨 아래
        float normalizePos = 1f - (targetPosY / scrollableHeight);

        // 5. 범위 제한 (오버슈트 방지)
        normalizePos = Mathf.Clamp01(normalizePos);

        // 6. 실행
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
