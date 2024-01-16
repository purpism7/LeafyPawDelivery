using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Element.State
{
    public class Edit<T> : BaseState<T> where T : Game.BaseElement
    {
        private GameSystem.GameCameraController _gameCameraCtr = null;
        private GameSystem.IGrid _iGrid = null;
        private bool _enableColliders = false;
        private bool _isOverlap = false;

        public override BaseState<T> Initialize(GameSystem.GameCameraController gameCameraCtr, GameSystem.IGrid iGrid)
        {
            base.Initialize(gameCameraCtr, iGrid);

            _gameCameraCtr = gameCameraCtr;
            _iGrid = iGrid;

            _isOverlap = false;
            _enableColliders = false;

            return this;
        }

        public override void Apply(T t)
        {
            base.Apply(t);

            Overlap();
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

                        Overlap();

                        break;
                    }

                case TouchPhase.Moved:
                    {
                        Drag(touch);

                        _gameBaseElement.ActiveEdit(false);

                        _gameCameraCtr?.SetStopUpdate(true);

                        Overlap();

                        break;
                    }
                case TouchPhase.Stationary:
                    {
                        Overlap();

                        break;
                    }

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        _gameBaseElement.ActiveEdit(true);

                        _gameCameraCtr?.SetStopUpdate(false);

                        EnableColliders(false);
                        _gameBaseElement.EnableCollider(true);

                        break;
                    }
            }
        }

        private void EnableColliders(bool enable)
        {
            if (_enableColliders == enable)
                return;

            _enableColliders = enable;

            MainGameManager.Get<PlaceManager>()?.ActivityPlace?.EnableCollider(enable);
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

            pos.y = _iGrid.LimitPosY(pos.y);

            gameBaseTm.position = pos;
        }

        #region Overlap
        private void SetIsOverlap(bool isOverlap)
        {
            if (_isOverlap == isOverlap)
                return;

            _gameBaseElement.SetColor(isOverlap ? Color.gray : Color.white);
            
            _isOverlap = isOverlap;
        }

        private void Overlap()
        {
            if (_gameBaseElement == null)
                return;

            bool isOverlapTarget = false;
            if (_gameBaseElement is Game.Object)
            {
                var obj = _gameBaseElement as Game.Object;
                isOverlapTarget = obj.IsOverlap;
            }

            bool isOverlap = false;
            EnableColliders(true);

            var gameBaseTm = _gameBaseElement.transform;
            var gameBaseCollider = _gameBaseElement.Collider;

            Collider[] colliders = null;
            switch(gameBaseCollider)
            {
                case CapsuleCollider collider:
                    {
                        var direction = new Vector3 { [collider.direction] = 1 };
                        var offset = collider.height / 2 - collider.radius;
                        var localPoint0 = collider.center - direction * offset;
                        var localPoint1 = collider.center + direction * offset;

                        var startPos = gameBaseTm.TransformPoint(localPoint0);
                        var endPos = gameBaseTm.TransformPoint(localPoint1);

                        var r = gameBaseTm.TransformVector(collider.radius, collider.radius, collider.radius);
                        var radius = Enumerable.Range(0, 3).Select(xyz => xyz == collider.direction ? 0 : r[xyz]).Select(Mathf.Abs).Max();

                        colliders = Physics.OverlapCapsule(startPos, endPos, collider.radius);

                        break;
                    }

                case BoxCollider collider:
                    {
                        colliders = Physics.OverlapBox(collider.center + gameBaseTm.position, collider.size * 0.5f, gameBaseTm.rotation);

                        break;
                    }

                case SphereCollider collider:
                    {
                        colliders = Physics.OverlapSphere(gameBaseTm.position + collider.center, collider.radius);

                        break;
                    }

                //case PolygonCollider2D collider:
                //    {

                //        Physics2D.OverlapPoint()
                //    }

            }

            if (colliders != null)
            {
                foreach (var collider in colliders)
                {
                    if (collider == null)
                        continue;
                    
                    var obj = collider.gameObject.GetComponentInParent<Game.Object>();
                    if (obj != null)
                    {
                        if (obj.Id == _gameBaseElement.Id &&
                            obj.UId == _gameBaseElement.UId)
                            continue;

                        if (!isOverlapTarget)
                        {
                            if (!obj.IsOverlap)
                                continue;
                        }

                        isOverlap = true;

                        break;
                    }

                    var animal = collider.gameObject.GetComponentInParent<Game.Creature.Animal>();
                    if (animal != null)
                    {
                        var elementData = _gameBaseElement.ElementData;
                        if(elementData != null &&
                           elementData.EElement == Game.Type.EElement.Animal)
                        {
                            if (animal.Id == _gameBaseElement.Id)
                                continue;
                        }
                        
                        isOverlap = true;

                        break;
                    }
                }
            }

            SetIsOverlap(isOverlap);

            _gameBaseElement.InteractableArrangeBtn(!isOverlap);
        }
        #endregion
    }
}

