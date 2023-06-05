using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public class OpenConditionManager : GameSystem.Processing
    {
        private List<Condition> _conditionList = new();

        public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
        {

            yield break;
        }
    }
}
