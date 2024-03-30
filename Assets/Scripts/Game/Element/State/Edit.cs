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

        public override void Apply(BaseElement gameBaseElement)
        {
            base.Apply(gameBaseElement);

            gameBaseElement?.InteractableReturnBtn();
            SetSelectedLocalPosZ();

            Game.UIManager.Instance?.Bottom?.DeactivateEditList();
            MainGameManager.Instance?.GameState?.Get<Game.State.Edit>()?.SetEditElement(gameBaseElement);

            OverlapAsync().Forget();
        }

        public override void End()
        {
            var elementData = _gameBaseElement?.ElementData;

            _gameBaseElement?.SetColor(Color.white);

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

        public override void Touch(TouchPhase touchPhase, Touch? touch)
        {
            if(_gameBaseElement == null)
                return;

            switch (touchPhase)
            {
                case TouchPhase.Began:
                    {
                        //_gameBaseElement.ActiveEdit(true);
                        //_gameCameraCtr?.SetStopUpdate(true);
                        
                        break;
                    }

                case TouchPhase.Moved:
                    {
                        if (_gameBaseElement.IsMoving)
                        {
                            Drag(touch);

                            SetSelectedLocalPosZ();
                        }
                        //_gameBaseElement.ActiveEdit(false);
                        
                        break;
                    }
                case TouchPhase.Stationary:
                    {
                   
                        break;
                    }

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        //_gameBaseElement.ActiveEdit(true);
                        //_gameCameraCtr?.SetStopUpdate(false);

                        break;
                    }
            }

            OverlapAsync().Forget();
        }

        private void Drag(Touch? touch)
        {
            if (touch == null ||
                !touch.HasValue)
                return;

            Vector3 touchPos = touch.Value.position;

            if (_gameBaseElement == null)
                return;

            var gameCamera = _gameCameraCtr?.GameCamera;
            if (gameCamera == null)
                return;

            var gameBaseTm = _gameBaseElement.transform;

            float distance = gameCamera.WorldToScreenPoint(gameBaseTm.position).z;
            Vector3 movePos = new Vector3(touchPos.x, touchPos.y, distance);
            Vector3 pos = gameCamera.ScreenToWorldPoint(movePos);

            pos.y += 150f;
            pos.y = _iGrid.LimitPosY(pos.y);

            //_gameBaseElement?.SetLocalPos(pos);
            gameBaseTm.position = pos;
        }

        #region Overlap
        private async UniTask OverlapAsync()
        {
            if (_gameBaseElement == null)
                return;

            var elementCollision = _gameBaseElement.ElementCollision;
            if (elementCollision == null)
                return;

            await UniTask.WaitForFixedUpdate();

            bool isOverlap = elementCollision.IsOverlap;

            _gameBaseElement.SetColor(isOverlap ? Color.gray : Color.white);
            _gameBaseElement.InteractableArrangeBtn(!isOverlap);
        }
        #endregion
    }
}

