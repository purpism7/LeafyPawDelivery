using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace Game.PlaceEvent
{
    public class DropItem : Base
    {
        private Coroutine _dropItemCoroutine = null;
        private YieldInstruction _waitSecDrop = new WaitForSeconds(56f);

        //private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private Game.Type.EItemSub _eItemSub = Type.EItemSub.None;

        public void StartDrop()
        {
            _eItemSub = Type.EItemSub.Letter;

            StopDrop();
            _dropItemCoroutine = StartCoroutine(CoDrop());
            //AsyncDrop().Forget();
            
        }

        public void StopDrop()
        {
            if (_dropItemCoroutine != null)
            {
                StopCoroutine(_dropItemCoroutine);
                _dropItemCoroutine = null;
            }

            //if (_cancellationTokenSource != null)
            //{
            //    _cancellationTokenSource.Cancel();

            //}
        }

        //private async UniTask AsyncDrop()
        //{
        //    var gameState = MainGameManager.Instance?.GameState;
        //    if (gameState == null)
        //    {
        //        StartDrop();

        //        return;
        //    }

        //    if (gameState.CheckState<Game.State.Edit>())
        //        return;

        //    if (_eItemSub == Type.EItemSub.Letter)
        //    {
        //        UI.ITop iTop = Game.UIManager.Instance?.Top;
        //        if (iTop == null)
        //        {
        //            StartDrop();

        //            return;
        //        }

        //        if (iTop.CheckMaxDropLetterCnt)
        //        {
        //            StartDrop();

        //            return;
        //        }
        //    }

        //    try
        //    {
        //        await UniTask.WaitForSeconds(UnityEngine.Random.Range(60f, 70f), false, PlayerLoopTiming.Update, _cancellationTokenSource.Token);

        //        if (_cancellationTokenSource.IsCancellationRequested)
        //        {
        //            StartDrop();

        //            return;
        //        }
        //    }
        //    catch(OperationCanceledException e)
        //    {

        //    }

        //    Drop();

        //    await UniTask.Yield();

        //    StartDrop();
        //}

        private IEnumerator CoDrop()
        {
            var gameState = MainGameManager.Instance?.GameState;
            if (gameState == null)
                yield break;

            if (gameState.CheckState<Game.State.Edit>())
                yield break;

            if (_eItemSub == Type.EItemSub.Letter)
            {
                UI.ITop iTop = Game.UIManager.Instance?.Top;
                if (iTop == null)
                {
                    StartDrop();

                    yield break;
                }

                yield return new WaitUntil(() => !iTop.CheckMaxDropLetterCnt);
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
            var iGameCameraCtrProvider = MainGameManager.Instance?.IGameCameraCtrProvider;
            if (iGameCameraCtrProvider == null)
                return;

            var itemData = new Game.DropItem.ItemData()
            {
                startPos = new Vector3(iGameCameraCtrProvider.RandPosXInScreenRagne, iGameCameraCtrProvider.RandPosYInScreenRagne, 310f),

                activateProgress = true,
                totalProgress = 10,

                eItemSub = _eItemSub,
            };

            _iPlace?.CreateDropItem(itemData);

            if(_eItemSub == Type.EItemSub.Letter)
            {
                UI.ITop iTop = Game.UIManager.Instance?.Top;
                iTop?.SetDropLetterCnt(1);
            }
        }
    }
}

