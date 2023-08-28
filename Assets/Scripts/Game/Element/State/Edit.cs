using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Element.State
{
    public class Edit<T> : BaseState<T> where T : Game.BaseElement
    {
        private GameSystem.GameCameraController _gameCameraCtr = null;
        private GameSystem.IGridProvider _iGridProvider = null;
        private int _overlapCnt = 0;

        public override BaseState<T> Create(GameSystem.GameCameraController gameCameraCtr, GameSystem.IGridProvider iGridProvider)
        {
            _gameCameraCtr = gameCameraCtr;
            _iGridProvider = iGridProvider;

            return this;
        }

        public override void Apply(T t)
        {
            _gameBaseElement = t;
        }

        public override void Touch(Touch touch)
        {
            if(_gameBaseElement == null)
                return;

            if (_gameBaseElement.EState_ != EState.Edit)
                return;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        _gameBaseElement.ActiveEdit(true);

                        break;
                    }

                case TouchPhase.Moved:
                    {
                        Drag(touch);

                        _gameBaseElement.ActiveEdit(false);

                        _gameCameraCtr?.SetStopUpdate(true);

                        break;
                    }

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        _gameBaseElement.ActiveEdit(true);

                        _gameCameraCtr?.SetStopUpdate(false);

                        break;
                    }

                //default:
                //    {
                //        _gameBaseElement.ActiveEdit(true);

                //        break;
                //    }
            }
        }

        private void Drag(Touch touch)
        {
            if (_gameBaseElement == null)
                return;

            var gameCamera = _gameCameraCtr?.GameCamera;
            if (gameCamera == null)
                return;

            var gameBaseTm = _gameBaseElement.transform;

            float distance = gameCamera.WorldToScreenPoint(gameBaseTm.position).z;
            Vector3 movePos = new Vector3(touch.position.x, touch.position.y, distance);
            Vector3 pos = gameCamera.ScreenToWorldPoint(movePos);

            pos.y = Mathf.Clamp(pos.y, _iGridProvider.LimitBottom.y, _iGridProvider.LimitTop.y);

            gameBaseTm.position = pos;
        }

        public void Overlap(int cnt)
        {
            _overlapCnt += cnt;
        }
    }
}

