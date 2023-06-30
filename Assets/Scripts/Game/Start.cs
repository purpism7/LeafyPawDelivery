using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GameSystem
{
    public class Start : GameSystem.Processing
    {
        public override void Initialize()
        {
            
        }

        public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
        {
            FirebaseManager.Instance.Init();

            yield return StartCoroutine(ResourceManager.Instance.CoInit());
        }
    }
}
