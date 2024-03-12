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
        
        public override BaseState Initialize(GameSystem.GameCameraController gameCameraCtr, GameSystem.IGrid iGrid)
        {
            base.Initialize(gameCameraCtr, iGrid);

            Type = Game.Type.EElementState.Edit;

            _gameCameraCtr = gameCameraCtr;
            _iGrid = iGrid;

            return this;
        }

        public override async UniTask Apply(BaseElement gameBaseElement)
        {
            await base.Apply(gameBaseElement);

            SetSelectedLocalPosZ();

            Game.UIManager.Instance?.Bottom?.DeactivateEditList();
            MainGameManager.Instance?.GameState?.Get<Game.State.Edit>()?.SetEditElement(_gameBaseElement);

            await UniTask.WaitForFixedUpdate();

            Overlap();
        }

        public override void End()
        {
            var elementData = _gameBaseElement?.ElementData;

            var eTab = Game.Type.ETab.Animal;
            if (elementData != null)
            {
                eTab = elementData.EElement == Game.Type.EElement.Animal ? Game.Type.ETab.Animal : Game.Type.ETab.Object;
            }

            Game.UIManager.Instance?.Bottom?.ActivateEditList(eTab);
            MainGameManager.Instance?.GameState?.Get<Game.State.Edit>()?.SetEditElement(null);
        }

        private void SetSelectedLocalPosZ()
        {
            if (_gameBaseElement == null)
                return;

            _gameBaseElement.SetLocalPosZ(_gameBaseElement.LocalPos.y * GameUtils.PosZOffset + GameUtils.GetPosZOrder(Game.Type.EPosZOrder.EditElement));
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

                        SetSelectedLocalPosZ();
                        _gameBaseElement.ActiveEdit(false);
                        
                        break;
                    }
                case TouchPhase.Stationary:
                    {
                   
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

        #region Overlap
        private void Overlap()
        {
            if (_gameBaseElement == null)
                return;

            var elementCollision = _gameBaseElement.ElementCollision;
            if (elementCollision == null)
                return;

            bool isOverlap = elementCollision.IsOverlap;
           
            _gameBaseElement.SetColor(isOverlap ? Color.gray : Color.white);
            _gameBaseElement.InteractableArrangeBtn(!isOverlap);
        }
        #endregion
    }
}

