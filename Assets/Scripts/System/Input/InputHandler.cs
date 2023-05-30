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
        private MainGameManager _mainGameMgr = null;

        public void Init(GameSystem.GameCameraController gameCameraCtr, Grid grid)
        {
            _gameCameraCtr = gameCameraCtr;
            _grid = grid;

            _mainGameMgr = MainGameManager.Instance;
            _mainGameMgr?.SetStartEditAction(StartEdit);
        }

        public void ChainUpdate()
        {
            if(_mainGameMgr == null)
                return;

            if (!_mainGameMgr.GameState.Type.Equals(typeof(Game.State.Edit)))
                return;

            UpdateTouch();
        }

        private void UpdateTouch()
        {
            if (_gameCameraCtr == null)
                return;

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
                            StartEdit(gameBase);
                        }                        
                    }
                    else
                    {
                        if(CheckGetGameBase(raycastHit, out Game.Base gameBase))
                        {
                            _notTouchGameBase = _gameBase.UId != gameBase.UId;
                        }
                        else
                        {
                            _notTouchGameBase = true;
                        }
                        
                        _gameCameraCtr.SetStopUpdate(_notTouchGameBase == false);
                    }
                }
            }

            if(!_notTouchGameBase)
            {
                _gameBase?.OnTouch(touch);
            }

            if(_gameBase != null)
            {
                EndEdit();
            }
        }

        private void StartEdit(Game.Base gameBase)
        {
            _gameBase = gameBase;
            _gameBase?.OnTouchBegan(_gameCameraCtr.GameCamera, _grid);

            _notTouchGameBase = false;
            _gameCameraCtr.SetStopUpdate(true);

            Game.UIManager.Instance?.Bottom?.DeactivateEditList();
        }

        private void EndEdit()
        {
            if(_gameBase == null)
                return;

            if(_gameBase.EState_ != Game.EState.Remove &&
               _gameBase.EState_ != Game.EState.Arrange)
                return;

            _gameBase = null;
            _notTouchGameBase = false;
            _gameCameraCtr.SetStopUpdate(false);

            Game.UIManager.Instance?.Bottom?.ActivateEditList();
        }

        private bool CheckGetGameBase(RaycastHit raycastHit, out Game.Base gameBase)
        {
            gameBase = null;

            var collider = raycastHit.collider;
            if (collider == null)
                return false;

            gameBase = collider.GetComponentInParent<Game.Base>();
            if (gameBase == null)
                return false;

            return true;
        }
    }
}

