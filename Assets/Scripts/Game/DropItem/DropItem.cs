using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GameSystem;

namespace Game
{
    public partial class DropItem : Game.Base<DropItem.Data>
    {
        public class Data : BaseData
        {
            public Game.Type.EItem EItem = Type.EItem.None;
            public Vector3 startPos = Vector3.zero;

            public bool activateProgress = false;
            public int totalProgress = 0;
            public int progress = 0;
           
            public int Value = 0;

            public Data(Type.EItem eItem)
            {
                EItem = eItem;
            }
        }

        [SerializeField]
        private SpriteRenderer spriteRenderer = null;
        [SerializeField]
        private Transform progressRootTm = null;
        [SerializeField]
        private SpriteRenderer progressSpriteRenderer = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            if(data != null)
            {
                //_startRootTm = data.startRootTm;
                transform.position = _data.startPos;
                ActivateProgress(data.activateProgress);
            }

            SetItemSprite();
            SetCollider();

            Drop();
        }

        private void ActivateProgress(bool activate)
        {
            if (!progressRootTm)
                return;

            progressRootTm.SetActive(activate);
        }

        public override void OnTouchBegan(Touch? touch, GameCameraController gameCameraCtr, IGrid iGrid)
        {
            base.OnTouchBegan(touch, gameCameraCtr, iGrid);

            if (_data == null)
                return;

            if (!touch.HasValue)
                return;

            if(_data.activateProgress)
            {
                SetProgress();

                if (_data.totalProgress > _data.progress)
                    return;
            }

            Collect(touch, gameCameraCtr);

            Deactivate();
        }

        public override void OnTouch(Touch touch)
        {
            base.OnTouch(touch);
        }

        private void SetItemSprite()
        {
            if (_data == null)
                return;

            if (spriteRenderer == null)
                return;

            Sprite itemSprite = null;
            if(_data.EItem == Type.EItem.Currency)
            {
                itemSprite = ResourceManager.Instance?.AtalsLoader?.GetAnimalCurrencySpriteByPlaceId(GameUtils.ActivityPlaceId);
            }
            else if(_data.EItem == Type.EItem.Item)
            {
                var itemData = _data as Game.DropItem.ItemData;
                if (itemData == null)
                    return;

                if(itemData.eItemSub == Type.EItemSub.Letter)
                {
                    itemSprite = ResourceManager.Instance?.AtalsLoader?.GetCurrencySprite("letter");
                }
            }

            if (itemSprite == null)
                return;

            spriteRenderer.SetSpritie(itemSprite);
        }

        private void SetCollider()
        {
            var collider = spriteRenderer.gameObject.GetOrAddComponent<CapsuleCollider>();
            //if (collider == null)
            //    return;

            //collider.radius += 5f;
        }

        private void SetSortingOrder()
        {
            if (_data == null)
                return;

            if (spriteRenderer == null)
                return;

            int offset = 0;
            if(_data.EItem == Type.EItem.Currency)
            {
                offset = -13;
            }

            int sortingOrder = -(int)transform.position.y;
            spriteRenderer.sortingOrder = sortingOrder + offset;

            if(_data.activateProgress)
            {
                if (progressSpriteRenderer != null)
                {
                    progressSpriteRenderer.sortingOrder = sortingOrder;
                }
            }
        }

        private void SetProgress()
        {
            if (_data == null)
                return;

            ++_data.progress;

            if (progressSpriteRenderer == null)
                return;

            float progress = (_data.totalProgress - _data.progress) * 0.1f;            

            progressSpriteRenderer.transform.DOScaleX(progress, 0.2f);
        }

        private void Collect(Touch? touch, GameCameraController gameCameraCtr)
        {
            if (gameCameraCtr == null)
                return;

            var startPos = gameCameraCtr.UICamera.ScreenToWorldPoint(touch.Value.position);
            startPos.z = 10f;

            switch(_data)
            {
                case CurrencyData currencyData:
                    {
                        UIManager.Instance?.Top?.CollectCurrency(startPos, currencyData.EElement, _data.Value);

                        UI.ITop iTop = UIManager.Instance?.Top;
                        iTop?.SetDropAnimalCurrencyCnt(-1);

                        break;
                    }

                case ItemData itemData:
                    {
                        if(itemData.eItemSub == Type.EItemSub.Letter)
                        {
                            UIManager.Instance?.Top?.CollectCurrency(startPos, Type.EElement.Animal, _data.Value);
                        }

                        UI.ITop iTop = UIManager.Instance?.Top;
                        iTop?.SetDropLetterCnt(-1);

                        break;
                    }
            }
        }

        private void Drop()
        {
            if (_data == null)
                return;

            float randomPosOffsetX = Random.Range(100f, 200f) * (Random.Range(0, 2) == 0 ? -1 : 1);
            float randomPosOffsetY = Random.Range(-50f, 50f);
            var jumpLcoalPos = new Vector3(transform.position.x + randomPosOffsetX, transform.position.y + randomPosOffsetY, _data.startPos.z);

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

