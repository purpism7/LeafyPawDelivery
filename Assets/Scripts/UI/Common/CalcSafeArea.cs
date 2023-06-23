using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalcSafeArea : MonoBehaviour
{
    private void Awake()
    {
      
    }

    private void OnEnable()
    {
        Calc();
    }

    void Calc()
    {
        var rectTm = GetComponent<RectTransform>();
        if(!rectTm)
        {
            return;
        }

        var safeAreaPos = Screen.safeArea.position;

        var minAnchor = safeAreaPos;
        var maxAnchor = minAnchor + Screen.safeArea.size;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;

        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        rectTm.anchorMin = minAnchor;
        rectTm.anchorMax = maxAnchor;
    }
}
