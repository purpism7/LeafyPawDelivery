using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSystem;

namespace Game
{
    public class BoostManager : Manager.Base<BoostManager.Data>, IUpdater
    {
        public class Data : Manager.BaseData
        {
            public RectTransform boostRootRectTm = null;
        }

        [SerializeField]
        private Boost boost = null;

        private List<UI.Component.Boost> _boostCompList = null;

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                //SaveRemainTime();
                Save();
            }
        }

        private void OnApplicationQuit()
        {
            //SaveRemainTime();
            Save();
        }

        protected override void Initialize()
        {
            
            
        }

        public override IEnumerator CoInitialize(Data data)
        {
            SetBoosts(data);

            yield return null;
        }

        private void SetBoosts(Data data)
        {
            if (data == null)
                return;

            var datas = boost?.Datas;
            if (datas == null)
                return;

            if (datas.Length <= 0)
                return;

            _boostCompList = new();
            _boostCompList?.Clear();

            foreach (var boostData in datas)
            {
                if (boostData == null)
                    continue;

                var boost = new GameSystem.ComponentCreator<UI.Component.Boost, UI.Component.Boost.Data>()
                    .SetRootRectTm(data.boostRootRectTm)
                    .SetData(new UI.Component.Boost.Data()
                    {
                        iconSprite = boostData.iconSprite,
                        eBoost = boostData.eBoost,
                        timeSec = boostData.timeSec,
                        localKey = boostData.localKey,
                    })
                    .Create();

                _boostCompList?.Add(boost);
            }
        }

        public override void ChainUpdate()
        {
            base.ChainUpdate();

            if(_boostCompList != null)
            {
                for (int i = 0; i < _boostCompList.Count; ++i)
                {
                    var boost = _boostCompList[i];
                    if (boost == null)
                        continue;

                    boost.ChainUpdate();
                }
            }
        }

        public void Save()
        {
            if (_boostCompList == null)
                return;

            for(int i = 0; i < _boostCompList.Count; ++i)
            {
                var boost = _boostCompList[i];
                if (boost == null)
                    continue;

                if (boost.EBoost == Type.EBoost.None)
                    continue;

                PlayerPrefs.SetString(boost.EBoost.ToString(), boost.RemainTimeSec.ToString());
        }
        }

        public double GetBoostRemainSec(Game.Type.EBoost eBoost)
        {
            double.TryParse(PlayerPrefs.GetString(eBoost.ToString()), out double remainSec);

            return remainSec;
        }

        public bool CheckActivateBoost(Game.Type.EBoost eBoost)
        {
            return GetBoostRemainSec(eBoost) > 0;
        }
    }
}

