using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Edit<T> : BaseState<T> where T : Game.BaseElement
    {
        private Camera _gameCamera = null;
        private int _overlapCnt = 0;
        private GameSystem.Grid _grid = null;

        public Edit(Camera gameCamera, GameSystem.Grid grid)
        {
            _gameCamera = gameCamera;
            _grid = grid;
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

                        break;
                    }

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        _gameBaseElement.ActiveEdit(true);

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

            var gameBaseTm = _gameBaseElement.transform;

            float distance = _gameCamera.WorldToScreenPoint(gameBaseTm.position).z;
            Vector3 movePos = new Vector3(touch.position.x, touch.position.y, distance);
            Vector3 pos = _gameCamera.ScreenToWorldPoint(movePos);

            pos.y = Mathf.Clamp(pos.y, _grid.Limit.y, _grid.Limit2.y);

            gameBaseTm.position = pos;
        }

        public void Overlap(int cnt)
        {
            _overlapCnt += cnt;
        }
    }
}

