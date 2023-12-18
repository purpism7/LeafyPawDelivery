using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;


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
            void SelectPlace(int id);
        }

        [SerializeField]
        private int placeId = 0;
        [SerializeField]
        private RectTransform lockRectRootTm = null;
        [SerializeField]
        private Image placeIconImg = null;
        [SerializeField]
        private Button[] enterBtns = null;

        private Info.Connector _connector = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            InitializeIsLock();
            Debug.Log("Map Initialize");
        }

        public override void Activate()
        {
            base.Activate();

            Debug.Log("MapIcon Activate");

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

            if (_connector == null)
            {
                _connector = new();
            }

            bool isLock = true;
            if (_connector.OpenPlaceId > 0)
            {
                isLock = placeId >= _connector.OpenPlaceId;
            }
            else
            {
                isLock = placeId > lastPlaceId;
            }

            UIUtils.SetActive(lockRectRootTm, isLock);
            UIUtils.SetActive(placeIconImg?.gameObject, !isLock);

            SetInteractableEnterBtn(!isLock);
        }

        private void OpenPlace()
        {
            if(_connector == null)
            {
                _connector = new();
            }

            if (placeId != _connector.OpenPlaceId)
                return;

            _connector?.ResetOpenPlaceId();

            AnimOpenPlace();

            Game.Notification.Get?.Notify(Game.Notification.EType.OpenPlace);
        }

        private void AnimOpenPlace()
        {
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

        public void OnClick()
        {
            _data?.IListener?.SelectPlace(placeId);
        }
    }
}

