using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using Random = System.Random;

namespace UI.Component
{
    public class CollectCurrency : Base<CollectCurrency.Data>
    {
        public class Data : BaseData
        {
            public Vector3 StartPos = Vector3.zero;
            public Vector3 EndPos = Vector3.zero;
            public DG.Tweening.Ease JumpEase = Ease.OutSine;
            public DG.Tweening.Ease MoveEase = Ease.OutQuad;
            public Action CollectEndAction = null;
        }
        
        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            Deactivate();
            
            Move();
        }

        private void Move()
        {
            var rectTm = GetComponent<RectTransform>();
            if (!rectTm)
                return;

            var wayPoint = (_data.EndPos - _data.StartPos) / 2f;
            wayPoint += _data.StartPos;
            wayPoint.x += UnityEngine.Random.Range(-150f, 150f);

            var wayPoints = new[] { _data.StartPos, wayPoint, _data.EndPos };

            Sequence sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(rectTm.DOMove(_data.StartPos, 0))
                .AppendCallback(() => { Activate(); })
                
                .Append(rectTm.DOPath(wayPoints, 1f, PathType.CatmullRom).SetEase(_data.MoveEase))
                //.Append(rectTm.DOMove(_data.EndPos, 1f).SetEase(_data.MoveEase))
                // .Join(rectTm.DOLocalRotate(new Vector3(0, 0, UnityEngine.Random.Range(-180f, 180f)), 0.5f))
                .Join(rectTm.DOLocalRotate(new Vector3(0, 0, UnityEngine.Random.Range(-180f, 180f)), 1f))
                .OnComplete(() =>
                {
                    Deactivate();

                    _data?.CollectEndAction();
                });
            sequence.Restart();
        }
    }
}

