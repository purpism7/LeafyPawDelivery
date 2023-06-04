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


            var container = FindObjectOfType<Container>();
            yield return StartCoroutine(container?.CoLoadData());


            yield return StartCoroutine(Info.UserManager.Instance.CoInit());
            yield return StartCoroutine(MainGameManager.Instance.CoInit(iProvider));
            yield return StartCoroutine(Game.UIManager.Instance.CoInit());

            DOTween.Init();
        }
    }
}
