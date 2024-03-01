using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Cysharp.Threading.Tasks;

namespace Game.Element.State
{
    public class Edit : BaseState
    {
        private GameSystem.GameCameraController _gameCameraCtr = null;
        private GameSystem.IGrid _iGrid = null;
        
        private bool _isOverlap = false;
        private float _initPosZ = 0;

        public override BaseState Initialize(GameSystem.GameCameraController gameCameraCtr, GameSystem.IGrid iGrid)
        {
            base.Initialize(gameCameraCtr, iGrid);

            Type = Game.Type.EElementState.Edit;

            _gameCameraCtr = gameCameraCtr;
            _iGrid = iGrid;

            _isOverlap = false;

            return this;
        }

        public override async UniTask Apply(BaseElement gameBaseElement)
        {
            await base.Apply(gameBaseElement);

            var tm = gameBaseElement.transform;
            if (tm)
            {
                _initPosZ = tm.localPosition.z;
                SetPosZ(-50f);
            }            

            Game.UIManager.Instance?.Bottom?.DeactivateEditList();
            MainGameManager.Instance?.GameState?.Get<Game.State.Edit>()?.SetEditElement(_gameBaseElement);

            await UniTask.WaitForFixedUpdate();

            Overlap();
        }

        public override void End()
        {
            var elementData = _gameBaseElement?.ElementData;

            SetPosZ(_initPosZ);

            var eTab = Game.Type.ETab.Animal;
            if (elementData != null)
            {
                eTab = elementData.EElement == Game.Type.EElement.Animal ? Game.Type.ETab.Animal : Game.Type.ETab.Object;
            }

            Game.UIManager.Instance?.Bottom?.ActivateEditList(eTab);
            MainGameManager.Instance?.GameState?.Get<Game.State.Edit>()?.SetEditElement(null);
        }

        public override void Touch(Touch touch)
        {
            if(_gameBaseElement == null)
                return;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        _gameBaseElement.ActiveEdit(true);
                        _gameCameraCtr?.SetStopUpdate(true);

                        break;
                    }

                case TouchPhase.Moved:
                    {
                        Drag(touch);

                        _gameBaseElement.ActiveEdit(false);

                        break;
                    }
                case TouchPhase.Stationary:
                    {
                        //Overlap();

                        break;
                    }

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        _gameBaseElement.ActiveEdit(true);
                        _gameCameraCtr?.SetStopUpdate(false);

                        break;
                    }
            }

            Overlap();
        }

        //private void EnableColliders(bool enable)
        //{
        //    if (_enableColliders == enable)
        //        return;

        //    _enableColliders = enable;

        //    MainGameManager.Get<PlaceManager>()?.ActivityPlace?.EnableCollider(enable);
        //}

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

            pos.y += -10f;
            pos.y = _iGrid.LimitPosY(pos.y);

            gameBaseTm.position = pos;
        }

        private void SetPosZ(float posZ)
        {
            var tm = _gameBaseElement.transform;
            if (!tm)
                return;

            tm.localPosition = new Vector3(tm.localPosition.x, tm.localPosition.y, posZ);
        }

        #region Overlap
        private void SetIsOverlap(bool isOverlap)
        {
            //if (_isOverlap == isOverlap)
            //    return;

            _gameBaseElement.SetColor(isOverlap ? Color.gray : Color.white);
            
            _isOverlap = isOverlap;
        }

        private void Overlap()
        {
            if (_gameBaseElement == null)
                return;

            var elementCollision = _gameBaseElement.ElementCollision;
            if (elementCollision == null)
                return;

            //bool isOverlapTarget = false;
            //if (_gameBaseElement is Game.Object)
            //{
            //    var obj = _gameBaseElement as Game.Object;
            //    isOverlapTarget = obj.IsOverlap;
            //}

            bool isOverlap = elementCollision.IsOverlap;
            //EnableColliders(true);



            //var gameBaseTm = _gameBaseElement.transform;
            //Collider gameBaseCollider = null;//_gameBaseElement.Collider;

            //Collider[] colliders = null;
            //List<GameObject> gameObjList = null;

            //switch(gameBaseCollider)
            //{
            //    case CapsuleCollider collider:
            //        {
            //            var direction = new Vector3 { [collider.direction] = 1 };
            //            var offset = collider.height / 2 - collider.radius;
            //            var localPoint0 = collider.center - direction * offset;
            //            var localPoint1 = collider.center + direction * offset;

            //            var startPos = gameBaseTm.TransformPoint(localPoint0);
            //            var endPos = gameBaseTm.TransformPoint(localPoint1);

            //            var r = gameBaseTm.TransformVector(collider.radius, collider.radius, collider.radius);
            //            var radius = Enumerable.Range(0, 3).Select(xyz => xyz == collider.direction ? 0 : r[xyz]).Select(Mathf.Abs).Max();

            //            colliders = Physics.OverlapCapsule(startPos, endPos, collider.radius);

            //            break;
            //        }

            //    case BoxCollider collider:
            //        {
            //            colliders = Physics.OverlapBox(collider.center + gameBaseTm.position, collider.size * 0.5f, gameBaseTm.rotation);

            //            break;
            //        }

            //    case SphereCollider collider:
            //        {
            //            colliders = Physics.OverlapSphere(gameBaseTm.position + collider.center, collider.radius);

            //            break;
            //        }
            //}

            //if (colliders == null)
            //{
            //    //var collider2D = _gameBaseElement.Collider2D as PolygonCollider2D;
            //    //if (collider2D != null)
            //    //{
            //    //    var collider2Ds = Physics2D.OverlapPointAll(collider2D.bounds.center);

            //    //    gameObjList = new();
            //    //    gameObjList.Clear();

            //    //    foreach (var collider in collider2Ds)
            //    //    {
            //    //        if (collider == null)
            //    //            continue;

            //    //        gameObjList.Add(collider.gameObject);
            //    //    }
            //    //}
            //}
            //else
            //{
            //    gameObjList = GetGameObjectList(colliders);
            //}

            //if (gameObjList != null)
            //{ 
            //    foreach (var gameObj in gameObjList)
            //    {
            //        if (!gameObj)
            //            continue;
                    
            //        var obj = gameObj.GetComponentInParent<Game.Object>();
            //        if (obj != null)
            //        {
            //            if (obj.Id == _gameBaseElement.Id &&
            //                obj.UId == _gameBaseElement.UId)
            //                continue;

            //            if (!isOverlapTarget)
            //            {
            //                if (!obj.IsOverlap)
            //                    continue;
            //            }

            //            isOverlap = true;

            //            break;
            //        }

            //        var animal = gameObj.GetComponentInParent<Game.Creature.Animal>();
            //        if (animal != null)
            //        {
            //            var elementData = _gameBaseElement.ElementData;
            //            if(elementData != null &&
            //               elementData.EElement == Game.Type.EElement.Animal)
            //            {
            //                if (animal.Id == _gameBaseElement.Id)
            //                    continue;
            //            }
                        
            //            isOverlap = true;

            //            break;
            //        }
            //    }
            //}

            SetIsOverlap(isOverlap);

            _gameBaseElement.InteractableArrangeBtn(!isOverlap);
        }

        //private List<GameObject> GetGameObjectList(Collider[] colliders)
        //{
        //    if (colliders == null)
        //        return null;

        //    List<GameObject> gameObjList = new();
        //    gameObjList.Clear();

        //    foreach(var collider in colliders)
        //    {
        //        if (collider == null)
        //            continue;

        //        gameObjList.Add(collider.gameObject);
        //    }

        //    return gameObjList;
        //}
        #endregion
    }
}

