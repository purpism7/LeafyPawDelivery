using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

using DG.Tweening;

namespace GameSystem
{
    public class InputHandler : MonoBehaviour
    {
        private GameSystem.GameCameraController _gameCameraCtr = null;
        private Grid _grid = null;

        private Game.Base _gameBase = null;
        private bool _notTouchGameBase = false;

        public void Init(GameSystem.GameCameraController gameCameraCtr, Grid grid)
        {
            _gameCameraCtr = gameCameraCtr;
            _grid = grid;
        }

        public void ChainUpdate()
        {
            var gameMgr = GameManager.Instance;
            if (!gameMgr.GameState.Type.Equals(typeof(Game.State.Edit)))
            {
                return;
            }

            UpdateTouch();
        }

        private void UpdateTouch()
        {
            if (_gameCameraCtr == null)
            {
                return;
            }

            var touch = Input.GetTouch(0);
            var touchPoint = touch.position;
            var ray = _gameCameraCtr.GameCamera.ScreenPointToRay(touchPoint);

            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                if (touch.phase == TouchPhase.Began)
                {
                    if (_gameBase == null)
                    {
                        if(CheckGetGameBase(raycastHit, out Game.Base gameBase))
                        {
                            _gameBase = gameBase;
                            _gameBase?.OnTouchBegan(_gameCameraCtr.GameCamera, _grid);

                            _notTouchGameBase = false;
                            _gameCameraCtr.SetStopUpdate(true);
                        }                        
                    }
                    else
                    {
                        _notTouchGameBase = CheckGetGameBase(raycastHit, out Game.Base gameBase) == false;
                        _gameCameraCtr.SetStopUpdate(_notTouchGameBase == false);
                    }
                }
            }

            if(!_notTouchGameBase)
            {
                _gameBase?.OnTouch(touch);
            }
        }

        private bool CheckGetGameBase(RaycastHit raycastHit, out Game.Base gameBase)
        {
            gameBase = null;

            var collider = raycastHit.collider;
            if (collider == null)
            {
                return false;
            }

            gameBase = collider.GetComponentInParent<Game.Base>();
            if (gameBase == null)
            {
                return false;
            }

            return true;
        }

        //private void SetGameCameraUpdate(TouchPhase touchPhase)
        //{
        //    if (_gameCameraCtr == null)
        //    {
        //        return;
        //    }

        //    if (_gameBase != null)
        //    {
        //        switch (touchPhase)
        //        {
        //            case TouchPhase.Moved:
        //                {
        //                    if (!_gameCameraCtr.StopUpdate)
        //                    {
        //                        _gameCameraCtr.SetStopUpdate(true);
        //                    }
        //                }
        //                break;

        //            case TouchPhase.Ended:
        //            case TouchPhase.Canceled:
        //                {
        //                    if (_gameCameraCtr.StopUpdate)
        //                    {
        //                        _gameCameraCtr.SetStopUpdate(false);
        //                    }
        //                }
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        if(_gameCameraCtr.StopUpdate)
        //        {
        //            _gameCameraCtr.SetStopUpdate(false);
        //        }
        //    }
        //}
    }
}

