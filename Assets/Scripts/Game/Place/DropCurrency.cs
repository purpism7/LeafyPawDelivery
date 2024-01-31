using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Cysharp.Threading.Tasks;
using System.Threading;


namespace Game.PlaceEvent
{
    public class DropCurrency : Base, Game.DropItem.IListener
    {
        private Coroutine _dropCurrencyCoroutine = null;
        private YieldInstruction _waitSecDrop = null;

        //private CancellationTokenSource _cancellationTokenSource = new();

        public override Base Initialize(IPlace iPlace, IListener iListener, int placeId)
        {
            base.Initialize(iPlace, iListener, placeId);

            float randWaitSec = UnityEngine.Random.Range(30f, 40f);
            Debug.Log(randWaitSec);

            _waitSecDrop = new WaitForSeconds(randWaitSec);

            return this;
        }

        public void StartDrop()
        {
            StopDrop();
            _dropCurrencyCoroutine = StartCoroutine(CoDrop());
            //DropAsync().Forget();
        }

        public void StopDrop()
        {
            //_cancellationTokenSource?.Cancel();
            if (_dropCurrencyCoroutine != null)
            {
                StopCoroutine(_dropCurrencyCoroutine);
                _dropCurrencyCoroutine = null;
            }
        }

        private IEnumerator CoDrop()
        {
            var gameState = MainGameManager.Instance?.GameState;
            if (gameState == null)
                yield break;

            if (gameState.CheckState<Game.State.Edit>())
                yield break;

            var animalList = _iPlace?.AnimalList;
            if (animalList == null)
                yield break;

            if (animalList.Count <= 0)
                yield break;

            UI.ITop iTop = Game.UIManager.Instance?.Top;
            if (iTop != null)
            {
                if (iTop.CheckMaxDropAnimalCurrencyCnt)
                    yield break;
            }

            yield return _waitSecDrop;

            if (_dropCurrencyCoroutine == null)
            {
                StartDrop();

                yield break;
            }

            Drop();

            yield return null;

            StartDrop();
        }

        //private async UniTask DropAsync()
        //{
        //    var gameState = MainGameManager.Instance?.GameState;
        //    if (gameState == null)
        //        return;

        //    if (gameState.CheckState<Game.State.Edit>())
        //        return;

        //    var animalList = _iPlace?.AnimalList;
        //    if (animalList == null)
        //        return;

        //    if (animalList.Count <= 0)
        //        return;

        //    try
        //    {
        //        UI.ITop iTop = Game.UIManager.Instance?.Top;
        //        if (iTop == null)
        //        {
        //            StartDrop();

        //            return;
        //        }
        //        //_cancellationTokenSource = new();

        //        await UniTask.WaitUntil(() => !iTop.CheckMaxDropAnimalCurrencyCnt);

        //        await UniTask.WaitForSeconds(UnityEngine.Random.Range(30f, 40f));
        //        if (_cancellationTokenSource.IsCancellationRequested)
        //        {
        //            _cancellationTokenSource = new();

        //            StartDrop();

        //            return;
        //        }
        //    }
        //    catch (OperationCanceledException e)
        //    {

        //    }

        //    Drop();

        //    await UniTask.Yield();

        //    StartDrop();
        //}

        private void Drop()
        {
            var activateAnimalList = _iPlace.AnimalList.FindAll(animal => animal != null ? animal.IsActivate : false);
            if (activateAnimalList == null ||
                activateAnimalList.Count <= 0)
                return;

            var randomIndex = UnityEngine.Random.Range(0, activateAnimalList.Count);
            var randomAnimal = activateAnimalList[randomIndex];
            if (randomAnimal == null)
                return;

            if (randomAnimal.ElementData == null)
                return;

            var currencyData = new Game.DropItem.CurrencyData()
            {
                iListener = this,

                startPos = new Vector3(randomAnimal.transform.position.x, randomAnimal.transform.position.y),
                EElement = Type.EElement.Animal,
                Value = randomAnimal.ElementData.GetCurrency,
            };

            _iPlace?.CreateDropItem(currencyData);

            UI.ITop iTop = UIManager.Instance?.Top;
            iTop?.SetDropAnimalCurrencyCnt(1, out int dropCnt);

            if(iTop.CheckMaxDropAnimalCurrencyCnt)
            {
                StopDrop();
            }
        }

        #region Game.DropItem.IListener
        void Game.DropItem.IListener.GetDropItem(int dropCnt, Game.Type.EItemSub eItemSub)
        {
            Debug.Log("DropCurrency = " + dropCnt);
            UI.ITop iTop = UIManager.Instance?.Top;
            if(iTop != null)
            {
                if (dropCnt + 1 >= Game.Data.Const.MaxDropAnimalCurrencyCount)
                {
                    StartDrop();
                }
            }
        }
        #endregion
    }
}

