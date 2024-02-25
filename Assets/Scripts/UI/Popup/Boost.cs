using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

using TMPro;

namespace UI
{
    public class Boost : BasePopup<Boost.Data>
    {
        public class Data : BaseData
        {
            public IListener iListener = null;
            public Sprite iconSprite = null;
            public string adId = string.Empty;
            public int timeSec = 0;
            public string localKey = string.Empty;
            public bool activate = false;
            public System.DateTime? endDateTime = null;
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

            var remainTime = _data.endDateTime.Value.Subtract(System.DateTime.UtcNow);

            remainTimeTMP?.SetText(remainTime.ToString(@"mm\:ss"));
  
            if (remainTime.TotalSeconds <= 0)
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

        private void SetDesc()
        {
            var localKey = _data?.localKey;
            var desc = string.Empty;

            if (!string.IsNullOrEmpty(localKey))
            {
                desc = LocalizationSettings.StringDatabase.GetLocalizedString("UI", localKey, LocalizationSettings.SelectedLocale);
                desc = string.Format(desc, _data.timeSec / 60f);
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
            if (!_initialize || !buy)
                return;

            //buyBtn?.SetInteractable(false);

            Game.Timer.Get?.Add(
                new Game.Timer.Data()
                {
                    initialize = _initialize,
                    key = _data.adId,
                    timeTMP = remainPlayTimeTMP,
                    btn = buyBtn,
                    addSec = 60f * 6f,
                    endAction = () =>
                    {
                        remainPlayTimeTMP.GetComponent<UnityEngine.Localization.Components.LocalizeStringEvent>()?.RefreshString();
                    }
                });
        }

        public void OnClickCancel()
        {
            Deactivate();
        }

        public void OnClickBuy()
        {
            if (_data == null)
                return;

            GameSystem.AdMob.Get?.ShowAd(_data.adId,
                () =>
                {
                    _data = _data?.iListener?.Buy();

                    if(_data != null)
                    {
                        SetBoostState(_data.activate);
                    }

                    SetPlayTimer(true);
                });
        }
    }
}

