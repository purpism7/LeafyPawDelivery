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
        private IGrid _iGrid = null;
        private MainGameManager _mainGameMgr = null;
        private Game.Base _gameBase = null;
        private bool _beganGameBase = false;
        
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

        private void UpdateTouchGameBase()
        {
            if (_gameCameraCtr == null)
                return;

            var touch = Input.GetTouch(0);
            var touchPosition = touch.position;
            var ray = _gameCameraCtr.GameCamera.ScreenPointToRay(touchPosition);

            RaycastHit hitInfo;
            bool isHitInfo = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Game", "Animal", "Object"));            
            var raycastHit2Ds = Physics2D.RaycastAll(ray.origin, ray.direction);
            //var raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction);

            bool gameStateEdit = _mainGameMgr.GameState.CheckState<Game.State.Edit>();
            Game.Base gameBase = GetTouchGameBase(raycastHit2Ds, gameStateEdit);

            if (touch.phase == TouchPhase.Began)
            {
                if (_beganGameBase)
                    return;

                //if(gameStateEdit)
                //{
                //    if (CheckGetGameBase<Game.Base>(hitInfo, out gameBase))
                //    {
                //        OnTouchBegan(touch, gameBase);

                //        return;
                //    }
                //}
                //else
                {
                    if (isHitInfo)
                    {
                        if (gameBase != null)
                        {
                            OnTouchBegan(touch, gameBase);

                            return;
                        }
                    }
                }

                //if (CheckGetGameBase(raycastHit2D.Value, out gameBase))
                //{
                //    OnTouchBegan(touch, gameBase);

                //    return;
                //}
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
                    //bool isGetGameBase = CheckGetGameBase(raycastHit2D.Value, out gameBase);
                    //if(!isGetGameBase)
                    //{
                    //    isGetGameBase = CheckGetGameBase<Game.Base>(hitInfo, out gameBase);
                    //    if (!isGetGameBase)
                    //        return;
                    //}

                    if (gameBase == null)
                        return;
                }

                _gameBase?.OnTouch(touch);

                if (touch.phase == TouchPhase.Ended)
                {
                    OnTouchEnded(touch);
                }
                else if (touch.phase == TouchPhase.Canceled)
                {
                    OnTouchEnded(touch);
                }
            }
        }

        private void OnTouchBegan(Touch? touch, Game.Base gameBase)
        {
            _beganGameBase = true;
            _gameBase = gameBase;

            gameBase?.OnTouchBegan(touch, _gameCameraCtr, _iGrid);
        }

        private void OnTouchBegan(Game.Base gameBase)
        {
            OnTouchBegan(null, gameBase);
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

            _beganGameBase = false;
            _gameBase = null;
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
                        if (resGameBase.LocalPos.z > gameBase.LocalPos.z)
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

