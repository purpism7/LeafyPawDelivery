using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

using GameSystem;
using System;

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

        private Coroutine _activateProgressCoroutine = null;
        private YieldInstruction _waitSecActivateProgress = new WaitForSeconds(3f);

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

            if(activate &&
               progressSpriteRenderer != null)
            {
                progressSpriteRenderer.transform.DOScaleX(0, 0);
            }
        }

        public override void OnTouchBegan(Touch? touch, GameCameraController gameCameraCtr, IGrid iGrid)
        {
            base.OnTouchBegan(touch, gameCameraCtr, iGrid);            

            if (_data == null)
                return;

            if (CheckIsLetter)
            {
                if(_activateProgressCoroutine != null)
                {
                    StopCoroutine(_activateProgressCoroutine);
                    _activateProgressCoroutine = null;
                }
            }

            if (!touch.HasValue)
                return;

            if(_data.activateProgress)
            {
                SetProgress();

                if (_data.totalProgress > _data.progress)
                {
                    if (CheckIsLetter)
                    {
                        _activateProgressCoroutine = StartCoroutine(CoActivateProgress());
                    }

                    return;
                }
            }

            Collect(touch, gameCameraCtr);

            Deactivate();
        }

        public override void OnTouch(Touch touch)
        {
            base.OnTouch(touch);
        }

        private IEnumerator CoActivateProgress()
        {
            yield return _waitSecActivateProgress;

            progressSpriteRenderer?.DOFade(0, 0.1f);
        }

        //private async UniTask AsyncActivateProgress()
        //{
        //    try
        //    {
        //        Debug.Log("AsyncActivateProgress");
        //        await UniTask.Delay(TimeSpan.FromSeconds(5f), cancellationToken: _cancellationTokenSource.Token);
        //        Debug.Log("AsyncActivateProgress22");

        //        Debug.Log("progress scale");
        //        progressSpriteRenderer?.transform.DOScaleX(0, 0);
        //    }
        //    catch(OperationCanceledException e)
        //    {

        //    }
        //}

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
            else
            {
                if(CheckIsLetter)
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
            if (collider == null)
                return;

            collider.radius += 3f;
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

            progressSpriteRenderer.DOFade(1f, 0);
            progressSpriteRenderer.transform.DOScaleX(progress, _data.progress <= 1 ? 0 : 0.2f);
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
                            var top = UIManager.Instance?.Top;

                            float random = UnityEngine.Random.Range(0, 100f);
                            Debug.Log("random = " + random);
                            if(random < 3f)
                            {
                                int value = UnityEngine.Random.Range(1, 5);
                                top?.CollectCashCurrency(startPos, value);
                            }
                            else if(random < 55f)
                            {
                                int value = UnityEngine.Random.Range(5, 15);
                                top?.CollectCurrency(startPos, Type.EElement.Animal, value);
                            }
                            else
                            {
                                int value = UnityEngine.Random.Range(50, 100);
                                top?.CollectCurrency(startPos, Type.EElement.Object, value);
                            }
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

            float randomPosOffsetX = UnityEngine.Random.Range(100f, 200f) * (UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1);
            float randomPosOffsetY = UnityEngine.Random.Range(-50f, 50f);
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

        private bool CheckIsLetter
        {
            get
            {
                var itemData = _data as Game.DropItem.ItemData;
                if (itemData == null)
                    return false;

                return itemData.eItemSub == Type.EItemSub.Letter;
            }
        }
    }
}

