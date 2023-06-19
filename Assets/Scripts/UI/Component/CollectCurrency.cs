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
            
            Sequence sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(rectTm.DOMove(_data.StartPos, 0))
                .AppendCallback(() => { Activate(); })
                .Append(rectTm.DOShakePosition(0.3f, 100f, 30))
                // .Append(rectTm.DOLocalJump(new Vector2(0, 0), 100, 2, 0.3f).SetEase(_data.JumpEase))
                .Append(rectTm.DOMove(_data.EndPos, 1f).SetEase(_data.MoveEase))
                .Join(rectTm.DOShakeRotation(0.5f, 120f))
                .OnComplete(() =>
                {
                    Deactivate();
                });
            sequence.Restart();
        }
    }
}

