using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Edit<T> : BaseState<T> where T : Game.Base
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
            _gameBase = t;
        }

        public override void Touch(Touch touch)
        {
            if(_gameBase == null)
                return;

            if (_gameBase.EState_ != EState.Edit)
            {
                return;
            }

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        _gameBase.ActiveEdit(true);
                    }
                    break;

                case TouchPhase.Moved:
                    {
                        DragObject(touch);

                        _gameBase.ActiveEdit(false);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        _gameBase.ActiveEdit(true);
                    }
                    break;
            }
        }

        private void DragObject(Touch touch)
        {
            if (_gameBase == null)
                return;

            var objectTm = _gameBase.transform;

            float distance = _gameCamera.WorldToScreenPoint(objectTm.position).z;
            Vector3 movePos = new Vector3(touch.position.x, touch.position.y, distance);
            Vector3 pos = _gameCamera.ScreenToWorldPoint(movePos);

            //Debug.Log(_grid.Limit);
            //Debug.Log(_grid.Limit2);
            //Debug.Log(pos);

            pos.y = Mathf.Clamp(pos.y, _grid.Limit.y, _grid.Limit2.y);

            objectTm.position = pos;
        }

        public void Overlap(int cnt)
        {
            _overlapCnt += cnt;
        }
    }
}

