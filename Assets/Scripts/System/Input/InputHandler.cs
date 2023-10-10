using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using Game;
using UI.Component;

namespace GameSystem
{
    public class InputHandler : MonoBehaviour
    {   
        private GameSystem.GameCameraController _gameCameraCtr = null;
        private IGridProvider _iGridProvider = null;
        private MainGameManager _mainGameMgr = null;
        private Game.Base _gameBase = null;
        private bool _beganGameBase = false;
        
        public void Init(GameSystem.GameCameraController gameCameraCtr, IGridProvider iGridProvider)
        {
            _gameCameraCtr = gameCameraCtr;
            _iGridProvider = iGridProvider;

            _mainGameMgr = MainGameManager.Instance;
            _mainGameMgr?.SetStartEditAction(OnTouchBegan);
        }

        public void ChainUpdate()
        {
            if(_mainGameMgr == null)
                return;
            
            UpdateTouch();
        }

        private void UpdateTouch()
        {
            if (_gameCameraCtr == null)
                return;

            var touch = Input.GetTouch(0);
            var touchPosition = touch.position;
            var ray = _gameCameraCtr.GameCamera.ScreenPointToRay(touchPosition);

            RaycastHit hitInfo;
            bool isHitInfo = Physics.Raycast(ray, out hitInfo);
            bool gameStateEdit = _mainGameMgr.GameState.CheckState<Game.State.Edit>();
            Game.Base gameBase = null;
            
            if (touch.phase == TouchPhase.Began)
            {
                if (isHitInfo)
                {
                    if (CheckGetGameBase<Game.Base>(hitInfo, out gameBase))
                    {
                        OnTouchBegan(touch, gameBase);
                    }
                }

                if (!_beganGameBase)
                    return;
            }
            else
            {
                if (gameStateEdit)
                {
                    if (!_beganGameBase)
                        return;
                }
                else
                {
                    bool isGetGameBase = CheckGetGameBase<Game.Base>(hitInfo, out gameBase);
                    if (!isGetGameBase)
                        return;
                }

                _gameBase?.OnTouch(touch);

                if (touch.phase == TouchPhase.Ended)
                {
                    ReleaseGameBase();
                }
                else if (touch.phase == TouchPhase.Canceled)
                {
                    ReleaseGameBase();
                }
            }
        }

        private void OnTouchBegan(Touch? touch, Game.Base gameBase)
        {
            _beganGameBase = true;
            _gameBase = gameBase;

            gameBase?.OnTouchBegan(touch, _gameCameraCtr, _iGridProvider);
        }

        private void OnTouchBegan(Game.Base gameBase)
        {
            OnTouchBegan(null, gameBase);
        }

        private void ReleaseGameBase()
        {
            if(_gameBase == null)
                return;

            _beganGameBase = false;
            _gameBase = null;
        }

        private bool CheckGetGameBase<T>(RaycastHit raycastHit, out T t)
        {
            t = default(T);

            var collider = raycastHit.collider;
            if (collider == null)
                return false;

            t = collider.GetComponentInParent<T>();
            if (t == null)
                return false;

            return true;
        }
    }
}

