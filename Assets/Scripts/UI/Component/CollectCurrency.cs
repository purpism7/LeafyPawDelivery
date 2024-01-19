using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace UI.Component
{
    public class CollectCurrency : Base<CollectCurrency.Data>
    {
        public class Data : BaseData
        {
            public Vector3 StartPos = Vector3.zero;
            public Vector3 EndPos = Vector3.zero;
            public Sprite ImgSprite = null;
            public Action CollectEndAction = null;
        }

        [SerializeField]
        private UnityEngine.UI.Image currencyImg = null;
        
        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetCurrencyImg();

            Deactivate();
            
            Collect();
        }

        private void SetCurrencyImg()
        {
            if (_data == null ||
                _data.ImgSprite == null)
                return;

            if (currencyImg == null)
                return;

            currencyImg.sprite = _data.ImgSprite;
        }

        private void Collect()
        {
            var rectTm = GetComponent<RectTransform>();
            if (!rectTm)
                return;

            var wayPoint = (_data.EndPos - _data.StartPos) / 2f;
            wayPoint += _data.StartPos;
            wayPoint.x += UnityEngine.Random.Range(-150f, 150f);

            _data.StartPos.z = 50f;
            wayPoint.z = _data.StartPos.z;
            _data.EndPos.z = _data.StartPos.z;

            var wayPoints = new[] { _data.StartPos, wayPoint, _data.EndPos };
            var duration = 0.5f;

            Sequence sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(rectTm.DOMove(_data.StartPos, 0))
                .AppendCallback(() => { Activate(); })
                .Append(rectTm.DOPath(wayPoints, duration, PathType.CatmullRom).SetEase(Ease.Linear))
                .Join(rectTm.DOLocalRotate(new Vector3(0, 0, UnityEngine.Random.Range(-180f, 180f)), duration))
                .OnComplete(() =>
                {
                    Deactivate();

                    _data?.CollectEndAction();
                });
            sequence.Restart();
        }
    }
}

