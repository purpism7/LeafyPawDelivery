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
            public GameData.Boost.Data boostData = null;
        }

        [SerializeField]
        private UnityEngine.UI.Image iconImg = null;
        [SerializeField]
        private TextMeshProUGUI remainTimeTMP = null;

        private bool _activate = false;
        public double _remainTimeSec = 0;

        public System.DateTime? EndDateTime { get; private set; } = null;
       
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

            if (EndDateTime == null ||
                !EndDateTime.HasValue)
            {
                EndBoost();

                return;
            }

            var remainTime = EndDateTime.Value - System.DateTime.UtcNow;

            remainTimeTMP?.SetText(remainTime.ToString(@"mm\:ss"));
            _remainTimeSec = remainTime.TotalSeconds;

            if(_remainTimeSec <= 0)
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
            EndDateTime = System.DateTime.UtcNow.AddSeconds(remainSec);
            
            _activate = true;
        }

        private void EndBoost()
        {
            EndDateTime = null;
            _activate = false;
            _remainTimeSec = 0;

            remainTimeTMP?.SetText(string.Empty);

            MainGameManager.Get<Game.BoostManager>()?.Save();
        }

        private UI.Boost.Data CreateBoostData
        {
            get
            {
                return new UI.Boost.Data()
                {
                    iListener = this,
                    activate = _activate,
                    endDateTime = EndDateTime,
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

            _remainTimeSec = boostData.timeSec;
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

