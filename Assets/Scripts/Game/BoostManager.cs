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
        private GameData.Boost boost = null;

        private List<UI.Component.Boost> _boostCompList = null;
        //private Dictionary<Game.Type.EBoost, float> _boostRemainPlayTimeDic = null;

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                Save();
            }
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        public override MonoBehaviour Initialize()
        {
            return this;
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
                        boostData = boostData,
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

        public void Apply(Type.EBoost eBoost)
        {
            if (eBoost == Type.EBoost.AllPickUp)
            {
                IPlace iPlace = MainGameManager.Get<PlaceManager>().ActivityPlace;
                if (iPlace != null)
                {
                    // MainGameManager.Instance?.IGameCameraCtr?.SetPositionUICamera(false, Vector3.zero);
                    
                    iPlace.AllPickUpDropItem(MainGameManager.Instance.IGameCameraCtr.WorldToScreenPoint);
                }
            }

            // MainGameManager.Instance?.IGameCameraCtr?.SetPositionUICamera(true, Vector3.zero);
            
            Save();
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

                if (boost.EndDateTime == null ||
                    !boost.EndDateTime.HasValue)
                    continue;

                PlayerPrefs.SetString(boost.EBoost.ToString(), boost.EndDateTime.ToString());
            }
        }

        public double GetBoostRemainSec(Game.Type.EBoost eBoost)
        {
            if(System.DateTime.TryParse(PlayerPrefs.GetString(eBoost.ToString()), out System.DateTime endDateTime))
            {
                return (endDateTime - System.DateTime.UtcNow).TotalSeconds;
            }

            return 0;
        }

        public bool CheckActivateBoost(Game.Type.EBoost eBoost)
        {
            return GetBoostRemainSec(eBoost) > 0;
        }
    }
}

