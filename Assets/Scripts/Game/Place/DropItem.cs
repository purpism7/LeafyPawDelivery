using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace Game.PlaceEvent
{
    public class DropItem : Base, Game.DropItem.IListener 
    {
        private Coroutine _dropItemCoroutine = null;
        private YieldInstruction _waitSecDrop = null;

        private Game.Type.EItemSub _eItemSub = Type.EItemSub.None;

        public override Base Initialize(IPlace iPlace, IListener iListener, int placeId)
        {
            base.Initialize(iPlace, iListener, placeId);

            float randWaitSec = UnityEngine.Random.Range(60f, 70f);

            _waitSecDrop = new WaitForSeconds(randWaitSec);

            return this;
        }

        public void StartDrop()
        {
            _eItemSub = Type.EItemSub.Letter;

            StopDrop();
            _dropItemCoroutine = StartCoroutine(CoDrop());
        }

        public void StopDrop()
        {
            if (_dropItemCoroutine != null)
            {
                StopCoroutine(_dropItemCoroutine);
                _dropItemCoroutine = null;
            }
        }

        private IEnumerator CoDrop()
        {
            yield return null;

            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                yield break;

            var eGameState = mainGameMgr.EGameState;
            if (eGameState == Game.Type.EGameState.Edit)
                yield break;

            if (_eItemSub == Type.EItemSub.Letter)
            {
                if (mainGameMgr.IsTutorial)
                    yield break;

                UI.ITop iTop = Game.UIManager.Instance?.Top;
                if (iTop != null)
                {
                    if (iTop.CheckMaxDropLetterCnt)
                        yield break;
                }
            }

            yield return _waitSecDrop;

            if (_dropItemCoroutine == null)
            {
                StartDrop();

                yield break;
            }

            Drop();

            yield return null;

            StartDrop();
        }

        private void Drop()
        {
            var iGameCameraCtr = MainGameManager.Instance?.IGameCameraCtr;
            if (iGameCameraCtr == null)
                return;

            var itemData = new Game.DropItem.ItemData()
            {
                iListener = this,

                startPos = new Vector3(iGameCameraCtr.RandPosXInScreenRagne, iGameCameraCtr.RandPosYInScreenRagne),

                activateProgress = true,
                totalProgress = 5,

                eItemSub = _eItemSub,
            };

            _iPlace?.CreateDropItem(itemData);

            if(_eItemSub == Type.EItemSub.Letter)
            {
                UI.ITop iTop = Game.UIManager.Instance?.Top;

                int currCnt = 0;
                iTop?.SetDropLetterCnt(1, out currCnt);

                _iListener?.Action(new DropItemData()
                {
                    eItemSub = _eItemSub,
                    currCnt = currCnt,
                });
            }
        }

        #region Game.DropItem.IListener
        void Game.DropItem.IListener.GetDropItem(int dropCnt, Game.Type.EItemSub eItemSub)
        {
            UI.ITop iTop = UIManager.Instance?.Top;
            if (iTop != null)
            {
                if(eItemSub == Type.EItemSub.Letter)
                {
                    if (dropCnt + 1 >= Game.Data.Const.MaxDropLetterCount)
                    {
                        StartDrop();
                    }
                }
            }
        }
        #endregion
    }
}

