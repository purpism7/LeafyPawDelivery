using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace GameSystem
{
    public class End : GameSystem.Processing
    {
        public override void Initialize()
        {
            
        }

        public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
        {
            yield return StartCoroutine(Info.UserManager.Instance?.CoInit());
            yield return StartCoroutine(MainGameManager.Instance?.CoInit(iProvider));
            yield return StartCoroutine(Game.UIManager.Instance?.CoInit(iProvider));

            yield return StartCoroutine(Game.Manager.IAP.Instance?.CoInit());

            //Sequencer.Create();
            DOTween.Init();

            Debug.Log("End Process End");

            yield return null;
        }
    }
}