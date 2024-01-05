using System;
using System.Collections;
using UnityEngine;

namespace Game.Element.State
{
    public class Game<T> : BaseState<T> where T : Game.BaseElement
    {
        readonly private float TouchInterval = 0.15f;

        private GameSystem.GameCameraController _gameCameraCtr = null;
        private DateTime _touchDateTime;

        public override BaseState<T> Initialize(GameSystem.GameCameraController gameCameraCtr, GameSystem.IGrid iGrid)
        {
            _gameCameraCtr = gameCameraCtr;

            return this;
        }

        public override void Touch(Touch touch)
        {
            if (_gameBaseElement == null)
                return;

            if (_gameBaseElement.EState_ == EState.Edit)
                return;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        CollectCurrnecy(touch);
                        StartSignatureAction();

                        break;
                    }

                case TouchPhase.Moved:
                    {
                        break;
                    }

                case TouchPhase.Stationary:
                    {
                        

                        break;
                    }

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        

                        break;
                    }
            }
        }

        private void CollectCurrnecy(Touch touch)
        {
            if (_gameCameraCtr == null)
                return;

            var elementData = _gameBaseElement?.ElementData;
            if (elementData == null)
                return;

            if (elementData.EElement != Game.Type.EElement.Object)
                return;

            var touchPosition = touch.position;
            if ((DateTime.UtcNow - _touchDateTime).TotalSeconds < TouchInterval)
                return;

            _touchDateTime = DateTime.UtcNow;

            var startPos = _gameCameraCtr.UICamera.ScreenToWorldPoint(touchPosition);

            UIManager.Instance?.Top?.CollectCurrency(startPos, elementData.EElement, elementData.GetCurrency);
        }

        private void StartSignatureAction()
        {
            if (_gameCameraCtr == null)
                return;

            var elementData = _gameBaseElement?.ElementData;
            if (elementData == null)
                return;

            if (elementData.EElement != Game.Type.EElement.Animal)
                return;

            var animal = _gameBaseElement as Creature.Animal;
            if (animal == null)
                return;

            animal.StartSignatureAction();

            Debug.Log("ActionSignature");
        }
    }
}
