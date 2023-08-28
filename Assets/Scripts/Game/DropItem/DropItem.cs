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
            public int Value = 0;
        }

        [SerializeField]
        private SpriteRenderer spriteRenderer = null;

        private Transform _startRootTm = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            _startRootTm = data.startRootTm;

            if (_startRootTm)
            {
                transform.position = new Vector3(_startRootTm.position.x, _startRootTm.position.y);
            }

            spriteRenderer.gameObject.AddComponent<CapsuleCollider>();

            Drop();
        }

        public override void ChainUpdate()
        {
            return;
        }

        public override void OnTouch(Touch touch)
        {
            base.OnTouch(touch);

            Debug.Log("DropItem Touch");

        }

        private void SetSortingOrder()
        {
            if (_data == null)
                return;

            if (spriteRenderer == null)
                return;

            spriteRenderer.sortingOrder = -(int)transform.position.y - 40;
        }

        private void Drop()
        {
            float randomPosOffsetX = Random.Range(100f, 200f) * (Random.Range(0, 2) == 0 ? -1 : 1);
            float randomPosOffsetY = Random.Range(-50f, 50f);
            //randomPosX = 500f;
            var jumpLcoalPos = new Vector3(transform.position.x + randomPosOffsetX, transform.position.y + randomPosOffsetY);
            //Debug.Log("randomPosX" + randomPosOffsetX);
            Sequence sequence = DOTween.Sequence()
               .SetAutoKill(false)
               //.Join(transform.DOLocalMoveZ(0f, 0f))
               //.Join(transform.DOLocalMoveX(70f, 1f).SetEase(Ease.OutQuad))
               //.Join(transform.DOLocalMoveY(-70f, 1f).SetEase(Ease.InQuad))
               .Append(transform.DOJump(jumpLcoalPos, 20f, 1, 0.5f))
               .OnComplete(() =>
               {
                   SetSortingOrder();
               });

            sequence.Restart();
        }
    }
}

