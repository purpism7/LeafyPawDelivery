using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game
{
    public class DropItem : Game.Base<DropItem.Data>
    {
        public class Data : BaseData
        {
            public Transform startRootTm = null;
        }

        private Transform _startRootTm = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            _startRootTm = data.startRootTm;

            if (_startRootTm)
            {
                transform.position = new Vector3(_startRootTm.position.x, _startRootTm.position.y - 30f, 1f);
            }

            Sequence sequence = DOTween.Sequence()
                .Join(transform.DOLocalMoveZ(0f, 0f))
                .Join(transform.DOLocalMoveX(70f, 1f).SetEase(Ease.OutQuad))
                .Join(transform.DOLocalMoveY(-70f, 1f).SetEase(Ease.InQuad))
                .Append(transform.DOLocalJump(new Vector3(80f, -80f, 0), 10f, 1, 1f));

            sequence.Restart();
        }

        public override void ChainUpdate()
        {
            return;
        }

        public override void OnTouch(Touch touch)
        {
            base.OnTouch(touch);

        }
    }
}

