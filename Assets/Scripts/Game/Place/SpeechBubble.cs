using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PlaceEvent
{
    public class SpeechBubble : Base
    {
        private Coroutine _speechBubbleCoroutine = null;
        private YieldInstruction _waitSecSpeechBubble = new WaitForSeconds(10f);

        public void Activate()
        {
            Deactivate();
            _speechBubbleCoroutine = StartCoroutine(CoRandomSpeechBubble());
        }

        public void Deactivate()
        {
            if (_speechBubbleCoroutine != null)
            {
                StopCoroutine(_speechBubbleCoroutine);
                _speechBubbleCoroutine = null;
            }

            var animalList = _iPlace?.AnimalList;
            if (animalList == null)
                return;

            foreach (var animal in animalList)
            {
                animal?.DeactivateSpeechBubble();
            }
        }

        private IEnumerator CoRandomSpeechBubble()
        {
            if (MainGameManager.Instance.GameState.CheckState<Game.State.Edit>())
                yield break;

            var animalList = _iPlace?.AnimalList;
            if (animalList == null)
                yield break;

            if (animalList.Count <= 0)
                yield break;

            yield return _waitSecSpeechBubble;

            if (_speechBubbleCoroutine == null)
                yield break;

            var activateAnimalList = animalList.FindAll(animal => animal != null ? animal.IsActivate : false);
            if (activateAnimalList == null ||
                activateAnimalList.Count <= 0)
                yield break;

            var randomIndex = UnityEngine.Random.Range(0, activateAnimalList.Count);
            var randomAnimal = activateAnimalList[randomIndex];
            if (randomAnimal == null)
                yield break;

            randomAnimal.ActivateSpeechBubble(
                () =>
                {
                    Activate();
                });
        }
    }
}

