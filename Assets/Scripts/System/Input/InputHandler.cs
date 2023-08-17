using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using DG.Tweening;
using Game;
using UI.Component;

namespace GameSystem
{
    public class InputHandler : MonoBehaviour
    {
        readonly private float TouchInterval = 0.3f;
        
        private GameSystem.GameCameraController _gameCameraCtr = null;
        private Grid _grid = null;

        private Game.BaseElement _gameBaseElement = null;
        private bool _notTouchGameBase = false;
        private MainGameManager _mainGameMgr = null;

        private DateTime _touchDateTime;

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
            
            UpdateTouch();
        }

        private void UpdateTouch()
        {
            if (_gameCameraCtr == null)
                return;

            var touch = Input.GetTouch(0);
            var touchPosition = touch.position;
            var ray = _gameCameraCtr.GameCamera.ScreenPointToRay(touchPosition);
            
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                if (touch.phase == TouchPhase.Began)
                {
                    if (!CheckEdit(raycastHit))
                    { 
                        CollectCurrency(raycastHit, touchPosition);
                    }
                }
            }
            
            if (_mainGameMgr.GameState.CheckState<Game.State.Edit>())
            {
                if(!_notTouchGameBase)
                {
                    _gameBaseElement?.OnTouch(touch);
                }

                if(_gameBaseElement != null)
                {
                    EndEdit();
                }
            }
        }

        private void CollectCurrency(RaycastHit raycastHit, Vector2 touchPosition)
        {
            if ((DateTime.UtcNow - _touchDateTime).TotalSeconds < TouchInterval)
                return;
                        
            _touchDateTime = DateTime.UtcNow;
            
            Game.BaseElement gameBaseElement = null;
            if (!CheckGetGameBaseElement(raycastHit, out gameBaseElement))
                return;
            
            var startPos = _gameCameraCtr.UICamera.ScreenToWorldPoint(touchPosition);
            startPos.z = 10f;

            UIManager.Instance?.Top?.CollectCurrency(startPos);
        }
        
        #region  Edit
        private bool CheckEdit(RaycastHit raycastHit)
        {
            if (!_mainGameMgr.GameState.CheckState<Game.State.Edit>())
                return false;
            
            if (_gameBaseElement == null)
            {
                if(CheckGetGameBaseElement(raycastHit, out Game.BaseElement gameBaseElement))
                {
                    StartEdit(gameBaseElement);
                }                        
            }
            else
            {
                if(CheckGetGameBaseElement(raycastHit, out Game.BaseElement gameBaseElement))
                {
                    _notTouchGameBase = _gameBaseElement.UId != gameBaseElement.UId;
                }
                else
                {
                    _notTouchGameBase = true;
                }
                        
                _gameCameraCtr.SetStopUpdate(_notTouchGameBase == false);
            }

            return true;
        }

        private void StartEdit(Game.BaseElement gameBaseElement)
        {
            _gameBaseElement = gameBaseElement;
            _gameBaseElement?.OnTouchBegan(_gameCameraCtr.GameCamera, _grid);

            _notTouchGameBase = false;
            _gameCameraCtr.SetStopUpdate(true);

            Game.UIManager.Instance?.Bottom?.DeactivateEditList();
        }

        private void EndEdit()
        {
            if(_gameBaseElement == null)
                return;

            if(_gameBaseElement.EState_ != Game.EState.Remove &&
               _gameBaseElement.EState_ != Game.EState.Arrange)
                return;

            _gameBaseElement = null;
            _notTouchGameBase = false;
            _gameCameraCtr.SetStopUpdate(false);

            Game.UIManager.Instance?.Bottom?.ActivateEditList();
        }
        #endregion

        private bool CheckGetGameBaseElement(RaycastHit raycastHit, out Game.BaseElement gameBaseElement)
        {
            gameBaseElement = null;

            var collider = raycastHit.collider;
            if (collider == null)
                return false;

            gameBaseElement = collider.GetComponentInParent<Game.BaseElement>();
            if (gameBaseElement == null)
                return false;

            return true;
        }
    }
}

