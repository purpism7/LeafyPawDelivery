using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

using TMPro;

using GameSystem;

namespace UI
{
    public class Boost : BasePopup<Boost.Data>
    {
        public class Data : BaseData
        {
            public IListener iListener = null;
            public bool activate = false;
            public System.DateTime? endDateTime = null;

            public GameData.Boost.Data boostData = null;
        }

        public interface IListener
        {
            Data Buy();
        }

        [SerializeField]
        private Image iconImg = null;
        [SerializeField]
        private TextMeshProUGUI descTMP = null;
        [SerializeField]
        private TextMeshProUGUI remainTimeTMP = null;

        [SerializeField]
        private RectTransform buyRootRectTm = null;
        [SerializeField]
        private RectTransform activateRootRectTm = null;

        [SerializeField]
        private Button buyBtn = null;
        [SerializeField]
        private TextMeshProUGUI remainPlayTimeTMP = null;

        private bool _initialize = true;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetIcon();
            SetDesc();

            remainTimeTMP?.SetText(string.Empty);

            if (data != null)
            {
                SetBoostState(data.activate);
                SetPlayTimer(false);
            }

            if(_initialize)
            {
                _initialize = false;
            }
        }

        public override void ChainUpdate()
        {
            base.ChainUpdate();
            
            if (_data == null)
                return;

            if (!_data.activate)
                return;

            if (_data.endDateTime == null)
                return;

            var remainTime = _data.endDateTime.Value - System.DateTime.UtcNow;
            
            remainTimeTMP?.SetText(remainTime.ToString(@"mm\:ss"));
  
            if (remainTime.TotalSeconds <= 0)
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

        private void SetDesc()
        {
            float timeSec = 0;
            string localKey = string.Empty;

            var boostData = _data?.boostData;
            if (boostData != null)
            {
                timeSec = boostData.timeSec;
                localKey = boostData.localKey;
            }

            var desc = string.Empty;

            if (!string.IsNullOrEmpty(localKey))
            {
                desc = LocalizationSettings.StringDatabase.GetLocalizedString("UI", localKey, LocalizationSettings.SelectedLocale);
                desc = string.Format(desc, timeSec / 60f);
            }

            descTMP?.SetText(desc);
        }

        private void SetBoostState(bool activate)
        {
            UIUtils.SetActive(buyRootRectTm, !activate);
            UIUtils.SetActive(activateRootRectTm, activate);
        }

        private void EndBoost()
        {
            if (_data == null)
                return;

            remainTimeTMP?.SetText(string.Empty);

            SetBoostState(false);

            _data.activate = false;
            _data.endDateTime = null;
        }

        private void SetPlayTimer(bool buy)
        {
            if (!_initialize && !buy)
                return;

            var adData = _data?.boostData?.ad;

            Game.Timer.Get?.Add(
                new Game.Timer.Data()
                {
                    initialize = _initialize,
                    key = adData.adId,
                    timeTMP = remainPlayTimeTMP,
                    btn = buyBtn,
                    addSec = adData.coolTimeSec,
                    endAction = () =>
                    {
                        remainPlayTimeTMP.GetComponent<UnityEngine.Localization.Components.LocalizeStringEvent>()?.RefreshString();
                    }
                });
        }

        public void OnClickCancel()
        {
            EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);

            Deactivate();
        }

        public void OnClickBuy()
        {
            EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);

            var adData = _data?.boostData?.ad;
            if (adData == null)
                return;

            GameSystem.AdMob.Get?.ShowAd(adData.adId,
                (rewardValue) =>
                {
                    if(rewardValue > 0)
                    {
                        _data = _data?.iListener?.Buy();

                        if (_data != null)
                        {
                            SetBoostState(_data.activate);
                        }
                    }

                    SetPlayTimer(true);
                });
        }
    }
}

