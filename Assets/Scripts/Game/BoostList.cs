using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSystem;

namespace Game
{
    public class BoostList : MonoBehaviour
    {
        [SerializeField]
        private Boost boost = null;

        private List<UI.Component.Boost> _boostCompList = null;

        public void Initialize()
        {
            SetBoost();
        }

        public void ChainUpdate()
        {
            if(_boostCompList != null)
            {
                for(int i = 0; i < _boostCompList.Count; ++i)
                {
                    _boostCompList[i]?.ChainUpdate();
                }
            }
        }

        private void SetBoost()
        {
            var datas = boost?.Datas;
            if (datas == null)
                return;

            if (datas.Length <= 0)
                return;

            _boostCompList = new();
            _boostCompList?.Clear();

            foreach (var data in datas)
            {
                if (data == null)
                    continue;

                var boost = new ComponentCreator<UI.Component.Boost, UI.Component.Boost.Data>()
                    .SetRootRectTm(GetComponent<RectTransform>())
                    .SetData(new UI.Component.Boost.Data()
                    {
                        iconSprite = data.iconSprite,
                        eBoost = data.eBoost,
                        timeSec = data.timeSec,
                        localKey = data.localKey,
                    })
                    .Create();

                _boostCompList?.Add(boost);
            }
        }
    }
}

