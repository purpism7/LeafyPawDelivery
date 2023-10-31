using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PlaceEvent
{
    public class DropCurrency : Base
    {
        private Coroutine _dropCurrencyCoroutine = null;
        private YieldInstruction _waitSecDropCurrency = new WaitForSeconds(30f);

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
            if (MainGameManager.Instance.GameState.CheckState<Game.State.Edit>())
                yield break;

            var animalList = _iPlace?.AnimalList;
            if (animalList == null)
                yield break;

            if (animalList.Count <= 0)
                yield break;

            yield return _waitSecDropCurrency;

            if (_dropCurrencyCoroutine == null)
                yield break;

            Drop();

            yield return null;

            StartDrop();
        }

        private void Drop()
        {
            var activateAnimalList = _iPlace.AnimalList.FindAll(animal => animal != null ? animal.IsActivate : false);
            var randomIndex = UnityEngine.Random.Range(0, activateAnimalList.Count);
            var randomAnimal = activateAnimalList[randomIndex];
            if (randomAnimal == null)
                return;

            if (randomAnimal.ElementData == null)
                return;

            new DropItemCreator()
                .SetRootTm(_iPlace.CurrencyRootTm)
                .SetDropItemData(new DropItem.CurrencyData()
                {
                    startRootTm = randomAnimal.transform,
                    EElement = Type.EElement.Animal,
                    Value = randomAnimal.ElementData.GetCurrency,
                })
                .Create();
        }
    }
}

