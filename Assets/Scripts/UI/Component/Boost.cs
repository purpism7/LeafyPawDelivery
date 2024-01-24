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
            public Sprite iconSprite = null;
            public string adId = string.Empty;
            public Game.Type.EBoost eBoost = Game.Type.EBoost.None;
            public int timeSec = 0;
            public string localKey = string.Empty;
        }

        [SerializeField]
        private UnityEngine.UI.Image iconImg = null;
        [SerializeField]
        private TextMeshProUGUI remainTimeTMP = null;

        private bool _activate = false;
        private double _remainSec = 0;
        private System.DateTime? _endDateTime = null;

        private void OnApplicationPause(bool pause)
        {
            if(pause)
            {
                SaveRemainTime();
            }
        }

        private void OnApplicationQuit()
        {
            SaveRemainTime();
        }

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetIcon();
            SetRemainTime();
        }

        public override void ChainUpdate()
        {
            base.ChainUpdate();

            if (!_activate)
                return;

            if (_endDateTime == null)
                return;
            
            var remainTime = _endDateTime.Value.Subtract(System.DateTime.UtcNow);

            remainTimeTMP?.SetText(remainTime.ToString(@"mm\:ss"));
            _remainSec = remainTime.TotalSeconds;

            if(_remainSec <= 0)
            {
                EndBoost();
            }
        }

        private void SetIcon()
        {
            var sprite = _data?.iconSprite;
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

            double.TryParse(PlayerPrefs.GetString(_data.eBoost.ToString()), out _remainSec);
            if(_remainSec > 0)
            {
                ActivateBoost();
            }
        }

        private void ActivateBoost()
        {
            _endDateTime = System.DateTime.UtcNow.AddSeconds(_remainSec);

            _activate = true;
        }

        private void SaveRemainTime()
        {
            if (_data == null)
                return;

            if (!_activate)
                return;

            PlayerPrefs.SetString(_data.eBoost.ToString(), _remainSec.ToString());
        }

        private void EndBoost()
        {
            _endDateTime = null;
            _activate = false;

            remainTimeTMP?.SetText(string.Empty);

            PlayerPrefs.SetString(_data.eBoost.ToString(), _remainSec.ToString());
        }

        private UI.Boost.Data CreateBoostData
        {
            get
            {
                return new UI.Boost.Data()
                {
                    iListener = this,
                    iconSprite = _data.iconSprite,
                    adId = _data.adId,
                    localKey = _data.localKey,
                    timeSec = _data.timeSec,
                    activate = _activate,
                    endDateTime = _endDateTime,
                };
            }
        }

        #region UI.Boost.IListener
        UI.Boost.Data UI.Boost.IListener.Buy()
        {
            if (_data == null)
                return null;

            _remainSec = _data.timeSec;

            ActivateBoost();

            return CreateBoostData;
        }
        #endregion

        public void OnClick()
        {
            new GameSystem.PopupCreator<UI.Boost, UI.Boost.Data>()
                .SetData(CreateBoostData)
                .SetReInitialize(true)
                .Create();
        }       
    }
}

