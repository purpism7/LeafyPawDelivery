using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.Localization.Settings;

namespace UI.Component
{
    public class Boost : Base<Boost.Data>, UI.Boost.IListener
    {
        public class Data : BaseData
        {
            //public Sprite iconSprite = null;
            //public string adId = string.Empty;
            //public Game.Type.EBoost eBoost = Game.Type.EBoost.None;
            //public int timeSec = 0;
            //public string localKey = string.Empty;

            public GameData.Boost.Data boostData = null;
        }

        [SerializeField]
        private UnityEngine.UI.Image iconImg = null;
        [SerializeField]
        private TextMeshProUGUI remainTimeTMP = null;

        private bool _activate = false;
        private System.DateTime? _endDateTime = null;

        public double RemainTimeSec { get; private set; } = 0;
        public Game.Type.EBoost EBoost { get { return _data != null && _data.boostData != null ? _data.boostData.eBoost : Game.Type.EBoost.None; } }

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetIcon();
            SetRemainTime();
        }

        public override void ChainUpdate()
        {
            base.ChainUpdate();

            if (_data == null)
                return;

            if (!_activate)
                return;

            if (_endDateTime == null ||
                !_endDateTime.HasValue)
                return;

       
            var remainTime = _endDateTime.Value.Subtract(System.DateTime.UtcNow);

            remainTimeTMP?.SetText(remainTime.ToString(@"mm\:ss"));
            RemainTimeSec = remainTime.TotalSeconds;

            if(RemainTimeSec <= 0)
            {
                EndBoost();
            }
        }

        private void SetIcon()
        {
            var sprite = _data?.boostData?.iconSprite;
            if (sprite == null)
                return;

            if (iconImg == null)
                return;

            iconImg.sprite = sprite;
        }

        private void SetRemainTime()
        {
            _activate = false;

            remainTimeTMP?.SetText(string.Empty);

            if (_data == null)
                return;

            double remainSec = MainGameManager.Get<Game.BoostManager>().GetBoostRemainSec(EBoost);
            if(remainSec > 0)
            {
                ActivateBoost(remainSec);
            }
        }

        private void ActivateBoost(double remainSec)
        {
            _endDateTime = System.DateTime.UtcNow.AddSeconds(remainSec);

            _activate = true;
        }

        //private void SaveRemainTime()
        //{
        //    if (_data == null)
        //        return;

        //    if (!_activate)
        //        return;

        //    PlayerPrefs.SetString(_data.eBoost.ToString(), _remainSec.ToString());
        //}

        private void EndBoost()
        {
            _endDateTime = null;
            _activate = false;

            remainTimeTMP?.SetText(string.Empty);

            RemainTimeSec = 0;
            MainGameManager.Get<Game.BoostManager>()?.Save();
            //PlayerPrefs.SetString(_data.eBoost.ToString(), _remainSec.ToString());
        }

        private UI.Boost.Data CreateBoostData
        {
            get
            {
                return new UI.Boost.Data()
                {
                    iListener = this,
                    //iconSprite = _data.iconSprite,
                    //adId = _data.adId,
                    //localKey = _data.localKey,
                    //timeSec = _data.timeSec,
                    activate = _activate,
                    endDateTime = _endDateTime,

                    boostData = _data.boostData,
                };
            }
        }

        #region UI.Boost.IListener
        UI.Boost.Data UI.Boost.IListener.Buy()
        {
            var boostData = _data?.boostData;
            if (boostData == null)
                return null;

            RemainTimeSec = boostData.timeSec;
            ActivateBoost(boostData.timeSec);

            MainGameManager.Get<Game.BoostManager>()?.Save();

            return CreateBoostData;
        }
        #endregion

        public void OnClick()
        {
            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr != null)
            {
                if (mainGameMgr.IsTutorial)
                    return;
            }

            new GameSystem.PopupCreator<UI.Boost, UI.Boost.Data>()
                .SetData(CreateBoostData)
                .SetReInitialize(true)
                .Create();
        }       
    }
}

