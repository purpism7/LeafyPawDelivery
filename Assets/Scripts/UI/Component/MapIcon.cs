using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using UnityEngine.Localization.Settings;

using GameSystem;

namespace UI.Component
{
    public class MapIcon : Base<MapIcon.Data>
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
        }

        public interface IListener
        {
            void SelectPlace(int placeId, System.Action<RectTransform> action);
            void SetMyLocation(int placeId, System.Action<RectTransform> action);
        }

        [SerializeField]
        private int placeId = 0;
        [SerializeField]
        private RectTransform lockRectRootTm = null;
        [SerializeField]
        private Image placeIconImg = null;
        [SerializeField]
        private Button[] enterBtns = null;
        [SerializeField]
        private RectTransform myLocationRootRectTm = null;

        private bool _isLock = false;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            InitializeIsLock();

            data?.IListener?.SetMyLocation(placeId, SetMyLocation);
        }

        public override void Activate()
        {
            base.Activate();

            OpenPlace();
        }

        private void InitializeIsLock()
        {
            int lastPlaceId = 1;
            var user = Info.UserManager.Instance?.User;
            if (user != null)
            {
                lastPlaceId = user.LastPlaceId;
            }

            _isLock = true;

            var connector = Info.Connector.Get;
            if(connector != null)
            {
                if (connector.OpenPlaceId > 0)
                {
                    _isLock = placeId >= connector.OpenPlaceId;
                }
                else
                {
                    _isLock = placeId > lastPlaceId;
                }
            }

            UIUtils.SetActive(lockRectRootTm, _isLock);
            UIUtils.SetActive(placeIconImg?.gameObject, !_isLock);

            //SetInteractableEnterBtn(!_isLock);
        }

        private void OpenPlace()
        {
            var connector = Info.Connector.Get;
            if (connector == null)
                return;

            if (placeId != connector.OpenPlaceId)
                return;

            _isLock = false;

            AnimOpenPlace();

            connector?.ResetOpenPlace();

            //Game.Notification.Get?.Notify(Game.Notification.EType.OpenPlace);
        }

        private void AnimOpenPlace()
        {
            SetInteractableEnterBtn(false);

            UIUtils.SetActive(placeIconImg?.gameObject, true);
            placeIconImg.DOFade(0, 0);

            Sequence sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .OnStart(() => { _endTask = false; })
                .AppendInterval(0.5f)
                .AppendCallback(() => UIUtils.SetActive(lockRectRootTm, false))
                .AppendInterval(0.3f)
                .Append(placeIconImg.DOFade(1, 0.5f))
                .OnComplete(() =>
                {
                    SetInteractableEnterBtn(true);
                });
            sequence.Restart();
        }

        private void SetInteractableEnterBtn(bool interactable)
        {
            if (enterBtns == null)
                return;

            foreach(var btn in enterBtns)
            {
                if (btn == null)
                    continue;

                btn.interactable = interactable;
            }
        }

        private void SetMyLocation(RectTransform myLocationRectTm)
        {
            if (!myLocationRectTm)
                return;

            if (!myLocationRootRectTm)
                return;

            myLocationRectTm.SetParent(myLocationRootRectTm);
            myLocationRectTm.localPosition = Vector3.zero;            
        }

        public void OnClick()
        {
            EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);

            if(_isLock || placeId == 4)
            {
                var localKey = "desc_not_opened_yet";
                if (placeId == 4)
                {
                    localKey = "desc_see_you_spring";
                }

                var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", localKey, LocalizationSettings.SelectedLocale);

                Game.Toast.Get?.Show(local, localKey);

                return;
            }

            _data?.IListener?.SelectPlace(placeId, SetMyLocation);
        }
    }
}

