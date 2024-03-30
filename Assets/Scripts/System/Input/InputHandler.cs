using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Game;

namespace GameSystem
{
    public class InputHandler : MonoBehaviour
    {
        private const float LongPressTime = 1.5f;

        private GameSystem.GameCameraController _gameCameraCtr = null;
        private IGrid _iGrid = null;
        private MainGameManager _mainGameMgr = null;

        private Game.Base _gameBase = null;
        private bool _isMove = false;
        private bool _longPress = false;
        private bool _possibleTouch = false;
        private float _pressTime = 0;

        public void Init(GameSystem.GameCameraController gameCameraCtr)
        {
            _gameCameraCtr = gameCameraCtr;
            _iGrid = _gameCameraCtr.IGrid;

            _mainGameMgr = MainGameManager.Instance;
            _mainGameMgr?.SetStartEditAction(OnTouchBegan);
        }

        public void ChainUpdate()
        {
            if(_mainGameMgr == null)
                return;

            UpdateTouchGameBase();
        }

        // UI 와 Game 터치 분리.
        private bool IsPointerOverUIObject(Vector2 touchPos)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = touchPos,
            };

            List<RaycastResult> results = new List<RaycastResult>();

            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            return results.Count > 0;
        }

        private void UpdateTouchGameBase()
        {
            if (_gameCameraCtr == null)
                return;

            var touch = Input.GetTouch(0);
            var touchPosition = touch.position;
            if (IsPointerOverUIObject(touchPosition))
                return;
                
            var ray = _gameCameraCtr.GameCamera.ScreenPointToRay(touchPosition);

            //RaycastHit hitInfo;
            //bool isHitInfo = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Game", "Animal", "Object"));            
            var raycastHit2Ds = Physics2D.RaycastAll(ray.origin, ray.direction);
            //var raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction);

            bool gameStateEdit = false;
            var gameState = _mainGameMgr?.GameState;
            if (gameState != null)
            {
                gameStateEdit = gameState.CheckState<Game.State.Edit>();
            }

            Game.Base gameBase = GetTouchGameBase(raycastHit2Ds, gameStateEdit);
            
            switch(touch.phase)
            {
                case TouchPhase.Began:
                    {
                        _isMove = false;
                        _pressTime = 0;

                        SetPossibleTouch(true);

                        break;
                    }

                case TouchPhase.Moved:
                    {
                        if (!_possibleTouch)
                            return;

                        if(gameStateEdit)
                        {
                            _gameBase?.OnTouch(touch);
                        }

                        _isMove = true;
                        _pressTime = 0;

                        break;
                    }

                case TouchPhase.Ended:
                    {
                        if (!_possibleTouch)
                            return;

                        if (gameStateEdit)
                        {
                            if (_gameBase == null &&
                                !_isMove)
                            {
                                if(gameBase != null)
                                {
                                    OnTouchBegan(touch, gameBase);

                                    gameBase.SetTouchAction(SetPossibleTouch, ReleaseGameBase);
                                }
                            }
                            else if(_gameBase != null)
                            {
                                _gameBase.OnTouch(touch);
                            }
                        }
                        else
                        {
                            if(!_isMove)
                            {
                                if (gameBase != null)
                                {
                                    OnTouchBegan(touch, gameBase);
                                }
                            }

                            OnTouchEnded(touch);
                        }

                        SetPossibleTouch(false);

                        _isMove = false;
                        _pressTime = 0;

                        break;
                    }

                case TouchPhase.Stationary:
                    {
                        if (!gameStateEdit)
                        {
                            LongPress(gameBase);
                        }

                        break;
                    }

                case TouchPhase.Canceled:
                    {
                        OnTouchEnded(touch);

                        SetPossibleTouch(false);

                        _pressTime = 0;

                        break;
                    }
            }

            //if (touch.phase == TouchPhase.Began)
            //{
            //    if (_beganGameBase)
            //        return;

            //    {
            //        if (isHitInfo)
            //        {
            //            if (gameBase != null)
            //            {
            //                OnTouchBegan(touch, gameBase);

            //                return;
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    if (gameStateEdit)
            //    {
            //        if (!_beganGameBase)
            //            return;
            //    }
            //    else
            //    {
            //        if (gameBase == null)
            //            return;
            //    }

            //    _gameBase?.OnTouch(touch);

            //    if (touch.phase == TouchPhase.Ended)
            //    {
            //        OnTouchEnded(touch);
            //    }
            //    else if (touch.phase == TouchPhase.Canceled)
            //    {
            //        OnTouchEnded(touch);
            //    }
            //}
        }

        private void OnTouchBegan(Touch? touch, Game.Base gameBase)
        {
            _gameBase = gameBase;

            gameBase?.OnTouchBegan(touch, _gameCameraCtr, _iGrid);
        }

        // 배치 버튼으로 바로 선택하기.
        private void OnTouchBegan(Game.Base gameBase)
        {
            OnTouchBegan(null, gameBase);
            gameBase?.SetTouchAction(SetPossibleTouch, ReleaseGameBase);
        }

        private void OnTouchEnded(Touch? touch)
        {
            _gameBase?.OnTouchEnded(touch, _iGrid);

            ReleaseGameBase();
        }

        private void ReleaseGameBase()
        {
            if (_gameBase == null)
                return;

            _gameBase = null;

            SetPossibleTouch(false);
        }

        private void SetPossibleTouch(bool possibleTouch)
        {
            _possibleTouch = possibleTouch;
        }

        private void LongPress(Game.Base gameBase)
        {
            if (_longPress)
                return;

            _pressTime += Time.deltaTime;

            if (_pressTime < LongPressTime)
                return;

            if (gameBase != null)
            {
                SetPossibleTouch(false);

                _longPress = true;

                MainGameManager.Instance?.SetGameStateAsync(Game.Type.EGameState.Edit);

                Game.UIManager.Instance?.Bottom?.DeactivateAnim(
                    () =>
                    {       
                        OnTouchBegan(gameBase);

                        gameBase.SetTouchAction(SetPossibleTouch, ReleaseGameBase);

                        _longPress = false;
                        _pressTime = 0;
                    });
            }
        }

        //private bool CheckGetGameBase<T>(RaycastHit raycastHit, out T t)
        //{
        //    t = default(T);

        //    var collider = raycastHit.collider;
        //    if (collider == null)
        //        return false;

        //    t = collider.GetComponentInParent<T>();
        //    if (t == null)
        //        return false;

        //    return true;
        //}

        private bool CheckGetGameBase<T>(RaycastHit2D raycastHit2D, out T t)
        {
            t = default(T);

            var collider = raycastHit2D.collider;
            if (collider == null)
                return false;

            t = collider.GetComponentInParent<T>();
            if (t == null)
                return false;

            return true;
        }

        private Game.Base GetTouchGameBase(RaycastHit2D[] raycastHit2Ds, bool isEdit)
        {
            if (raycastHit2Ds == null)
                return null;

            Game.Base resGameBase = null;
            Game.Base hiddenObject= null;
            
            for (int i = 0; i < raycastHit2Ds.Length; ++i)
            {
                var raycastHit2D = raycastHit2Ds[i];

                if (CheckGetGameBase(raycastHit2D, out Game.Base gameBase))
                {
                    if (gameBase == null)
                        continue;

                    if (gameBase is Game.DropItem)
                    {
                        return gameBase;
                    }

                    if(resGameBase == null)
                    {
                        resGameBase = gameBase;

                        continue;
                    }

                    if (isEdit)
                    {
                        if(resGameBase is Game.Creature.Animal ||
                           gameBase is Game.Creature.Animal)
                        {
                            if (resGameBase.LocalPos.y > gameBase.LocalPos.y)
                            {
                                resGameBase = gameBase;
                            }
                        }
                        else if (resGameBase.LocalPos.z > gameBase.LocalPos.z)
                        {
                            resGameBase = gameBase;
                        }
                    }
                    else
                    {
                        if (resGameBase.LocalPos.y > gameBase.LocalPos.y)
                        {
                            resGameBase = gameBase;
                        }

                        IObject iObject = gameBase as Game.Object;
                        if (iObject != null &&
                            iObject.IsHiddenObject)
                        {
                            if(hiddenObject == null)
                            {
                                hiddenObject = gameBase;
                            }
                        }
                    }
                }
            }

            if(!isEdit)
            {
                if(resGameBase as Game.Creature.Animal)
                {
                    return resGameBase;
                }
                else
                {
                    if(hiddenObject != null)
                    {
                        return hiddenObject;
                    }
                }
            }

            return resGameBase;
        }
    }
}

