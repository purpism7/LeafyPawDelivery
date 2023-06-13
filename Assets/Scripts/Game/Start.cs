using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GameSystem
{
    public class Start : GameSystem.Processing
    {
        protected override void Initialize()
        {
            
        }

        public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
        {
            yield return StartCoroutine(ResourceManager.Instance.CoInit());
        }
    }
}
