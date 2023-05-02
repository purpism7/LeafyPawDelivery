using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GameSystem
{
    public class Init : GameSystem.Processing
    {
        public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
        {
            yield return StartCoroutine(ResourceManager.Instance.CoInit());
            yield return StartCoroutine(Info.UserManager.Instance.CoInit());
            yield return StartCoroutine(GameManager.Instance.CoInit(iProvider));
            yield return StartCoroutine(UIManager.Instance.CoInit());

            DOTween.Init();

            yield return null;
        }
    }
}
