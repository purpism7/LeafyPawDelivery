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

           
            var wayPoint1 = _data.EndPos - _data.StartPos;
            //wayPoint1.x += 200f;
            //wayPoint1.y += 200f;

            var wayPoint2 = _data.EndPos;
            wayPoint2.x += 100f;

            //wayPoint.x += 50f;

            var wayPoints = new Vector3[3];
            wayPoints[0] = _data.StartPos;
            wayPoints[1] = wayPoint1;
            //wayPoints[2] = wayPoint2;
            wayPoints[2] = _data.EndPos;

            Sequence sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(rectTm.DOMove(_data.StartPos, 0))
                .AppendCallback(() => { Activate(); })
                // .Append(rectTm.DOShakePosition(0.3f, 100f, 30))
                // .Append(rectTm.SetEase(_data.JumpEase))
                .Append(rectTm.DOPath(wayPoints, 1f, PathType.CatmullRom).SetEase(_data.MoveEase))
                //.Append(rectTm.DOMove(_data.EndPos, 1f).SetEase(_data.MoveEase))
                .Join(rectTm.DOShakeRotation(1.5f, 140f))
                .OnComplete(() =>
                {
                    Deactivate();

                    _data?.CollectEndAction();
                });
            sequence.Restart();
        }
    }
}

