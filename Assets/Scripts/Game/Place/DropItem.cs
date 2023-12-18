using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PlaceEvent
{
    public class DropItem : Base
    {
        private Coroutine _dropItemCoroutine = null;
        private YieldInstruction _waitSecDrop = new WaitForSeconds(60f);

        public void StartDrop()
        {
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
            var gameState = MainGameManager.Instance?.GameState;
            if (gameState == null)
                yield break;

            if (gameState.CheckState<Game.State.Edit>())
                yield break;

            UI.ITop iTop = Game.UIManager.Instance?.Top;
            if(iTop == null)
            {
                StartDrop();

                yield break;
            }

            //iTop.
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
                Value = 10,

                activateProgress = true,
                totalProgress = 10,

                eItemSub = Type.EItemSub.Letter,
            };

            _iPlace?.CreateDropItem(itemData);

            UI.ITop iTop = Game.UIManager.Instance?.Top;
            iTop?.SetDropLetterCnt(1);
            //DropItemCreator.Get
            //    .SetRootTm(_iPlace.ItemRootTm)
            //    .SetDropItemData(new Game.DropItem.ItemData()
            //    {
            //        startPos = new Vector3(iGameCameraCtrProvider.RandPosXInScreenRagne, iGameCameraCtrProvider.RandPosYInScreenRagne, 310f),

            //        activateProgress = true,
            //        totalProgress = 10,

            //        eItemSub = Type.EItemSub.Letter,

            //        //startRootTm = randomAnimal.transform,
            //        //EElement = Type.EElement.Animal,
            //        //Value = randomAnimal.ElementData.GetCurrency,
            //    })
            //    .Create();
        }
    }
}

