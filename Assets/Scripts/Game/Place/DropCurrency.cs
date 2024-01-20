using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PlaceEvent
{
    public class DropCurrency : Base
    {
        private Coroutine _dropCurrencyCoroutine = null;
        private YieldInstruction _waitSecDrop = null;

        public override Base Initialize(IPlace iPlace, IListener iListener)
        {
            base.Initialize(iPlace, iListener);

            float randWaitSec = UnityEngine.Random.Range(30f, 40f);
            Debug.Log(randWaitSec);

            _waitSecDrop = new WaitForSeconds(randWaitSec);

            return this;
        }

        public void StartDrop()
        {
            StopDrop();
            _dropCurrencyCoroutine = StartCoroutine(CoDrop());
        }

        public void StopDrop()
        {
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
            if (iTop == null)
            {
                StartDrop();

                yield break;
            }

            yield return new WaitUntil(() => !iTop.CheckMaxDropAnimalCurrencyCnt);

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
                startPos = new Vector3(randomAnimal.transform.position.x, randomAnimal.transform.position.y),
                EElement = Type.EElement.Animal,
                Value = randomAnimal.ElementData.GetCurrency,
            };

            _iPlace?.CreateDropItem(currencyData);

            UI.ITop iTop = UIManager.Instance?.Top;
            iTop?.SetDropAnimalCurrencyCnt(1);
        }
    }
}

