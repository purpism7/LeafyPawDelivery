using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Cysharp.Threading.Tasks;

namespace Game.Element.State
{
    public class Edit : Base
    {
        private GameSystem.GameCameraController _gameCameraCtr = null;
        private GameSystem.IGrid _iGrid = null;

        private Vector3 _currentVelocity = Vector3.zero; // 현재 속도를 저장할 변수
        public float smoothTime = 0.01f; // 도달하는 데 걸리는 시간 (작을수록 빠름, 클수록 부드러움)

        public override Base Initialize(GameSystem.GameCameraController gameCameraCtr, GameSystem.IGrid iGrid)
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
                        break;
                    }

                case TouchPhase.Moved:
                    {
                        if (_gameBaseElement.IsMoving)
                        {
                            Drag(touch);

                            SetSelectedLocalPosZ();
                        }
                        
                        break;
                    }
                case TouchPhase.Stationary:
                    {
                        break;
                    }

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        break;
                    }
            }

            OverlapAsync().Forget();
        }

        //private void Drag(Touch? touch)
        //{
        //    if (touch == null)
        //        return;

        //    Vector3 touchPos = touch.Value.position;

        //    if (_gameBaseElement == null)
        //        return;

        //    var gameCamera = _gameCameraCtr?.GameCamera;
        //    if (gameCamera == null)
        //        return;

        //    var gameBaseTm = _gameBaseElement.transform;

        //    float distance = gameCamera.WorldToScreenPoint(gameBaseTm.position).z;
        //    Vector3 movePos = new Vector3(touchPos.x, touchPos.y, distance);
        //    Vector3 pos = gameCamera.ScreenToWorldPoint(movePos);

        //    pos.y += 160f;
        //    pos.y = _iGrid.LimitPosY(pos.y);

        //    //_gameBaseElement?.SetLocalPos(pos);
        //    gameBaseTm.position = pos;
        //}

        private void Drag(Touch? touch)
        {
            if (touch == null)
                return;

            Vector3 touchPos = touch.Value.position;

            if (_gameBaseElement == null)
                return;

            var gameCamera = _gameCameraCtr?.GameCamera;
            if (gameCamera == null)
                return;

            var gameBaseTm = _gameBaseElement.transform;

            // 1. 목표 위치 계산 (기존 로직 유지)
            float distance = gameCamera.WorldToScreenPoint(gameBaseTm.position).z;
            Vector3 movePos = new Vector3(touchPos.x, touchPos.y, distance);
            Vector3 targetPos = gameCamera.ScreenToWorldPoint(movePos); // 변수명 pos -> targetPos로 변경 (명확성을 위해)

            targetPos.y += 160f;
            targetPos.y = _iGrid.LimitPosY(targetPos.y);

            // 2. 부드럽게 이동 적용 (수정된 부분)
            // gameBaseTm.position = targetPos; // <-- 기존의 딱딱한 이동

            gameBaseTm.position = Vector3.SmoothDamp(
                gameBaseTm.position, // 현재 위치
                targetPos,           // 목표 위치
                ref _currentVelocity,// 현재 속도 (함수가 실행되면서 계속 갱신됨)
                smoothTime           // 지연 시간 (보통 0.1f ~ 0.3f 추천)
            );
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

