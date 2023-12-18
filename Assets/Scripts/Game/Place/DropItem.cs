using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PlaceEvent
{
    public class DropItem : Base
    {
        private Coroutine _dropItemCoroutine = null;
        private YieldInstruction _waitSecDrop = new WaitForSeconds(60f);

        private Game.Type.EItemSub _eItemSub = Type.EItemSub.None;

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
            var gameState = MainGameManager.Instance?.GameState;
            if (gameState == null)
                yield break;

            if (gameState.CheckState<Game.State.Edit>())
                yield break;

            if(_eItemSub == Type.EItemSub.Letter)
            {
                UI.ITop iTop = Game.UIManager.Instance?.Top;
                if (iTop == null)
                {
                    StartDrop();

                    yield break;
                }

                if(iTop.CheckMaxDropLetterCnt)
                {
                    StartDrop();

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
            var iGameCameraCtrProvider = MainGameManager.Instance?.IGameCameraCtrProvider;
            if (iGameCameraCtrProvider == null)
                return;

            var itemData = new Game.DropItem.ItemData()
            {
                startPos = new Vector3(iGameCameraCtrProvider.RandPosXInScreenRagne, iGameCameraCtrProvider.RandPosYInScreenRagne, 310f),
                Value = 10,

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

