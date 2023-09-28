using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GameSystem;

namespace Game
{
    public class DropItem : Game.Base<DropItem.Data>
    {
        public class Data : BaseData
        {
            public Transform startRootTm = null;
            public Game.Type.EElement EElement = Type.EElement.None;
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

            SetCollider();

            Drop();
        }

        public override void OnTouchBegan(Touch? touch, GameCameraController gameCameraCtr, IGridProvider iGridProvider)
        {
            base.OnTouchBegan(touch, gameCameraCtr, iGridProvider);

            if (_data == null)
                return;

            if (!touch.HasValue)
                return;

            var startPos = gameCameraCtr.UICamera.ScreenToWorldPoint(touch.Value.position);
            startPos.z = 10f;

            UIManager.Instance?.Top?.CollectCurrency(startPos, _data.EElement, _data.Value);

            Deactivate();
        }

        public override void OnTouch(Touch touch)
        {
            base.OnTouch(touch);
        }

        private void SetCollider()
        {
            var collider = spriteRenderer.gameObject.GetOrAddComponent<CapsuleCollider>();
            if (collider == null)
                return;

            collider.radius += 5f;
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
            var jumpLcoalPos = new Vector3(transform.position.x + randomPosOffsetX, transform.position.y + randomPosOffsetY);
            
            Sequence sequence = DOTween.Sequence()
               .SetAutoKill(false)
               .Append(transform.DOJump(jumpLcoalPos, 30f, 1, 0.5f))
               .OnComplete(() =>
               {
                   SetSortingOrder();
               });

            sequence.Restart();
        }
    }
}

