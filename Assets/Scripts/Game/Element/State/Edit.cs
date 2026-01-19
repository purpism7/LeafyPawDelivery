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
        
        private Vector3 _targetPosition = Vector3.zero;
        private Vector3 _currentVelocity = Vector3.zero;
        private bool _isDragging = false;
        private const float _smoothTime = 0.01f; // 매우 빠른 반응
        private const float _maxSpeed = 500f; // 최대 속도 매우 높게 설정
        
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

            // 초기 목표 위치를 현재 위치로 설정
            if (gameBaseElement != null)
            {
                _targetPosition = gameBaseElement.transform.position;
                _currentVelocity = Vector3.zero;
                _isDragging = false;
            }

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

            _isDragging = false;
            _currentVelocity = Vector3.zero;

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
                        _isDragging = false;
                        break;
                    }

                case TouchPhase.Moved:
                    {
                        if (_gameBaseElement.IsMoving)
                        {
                            Drag(touch);
                            _isDragging = true;
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
                        _isDragging = false;
                        break;
                    }
            }

            OverlapAsync().Forget();
        }

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

            float distance = gameCamera.WorldToScreenPoint(gameBaseTm.position).z;
            Vector3 movePos = new Vector3(touchPos.x, touchPos.y, distance);
            Vector3 pos = gameCamera.ScreenToWorldPoint(movePos);

            pos.y += 160f;
            pos.y = _iGrid.LimitPosY(pos.y);

            // 목표 위치만 설정하고, 실제 이동은 ChainUpdate에서 부드럽게 처리
            _targetPosition = pos;
        }
        
        public override void ChainUpdate()
        {
            base.ChainUpdate();
            
            if (_gameBaseElement == null || !_isDragging)
                return;
            
            // 매우 빠른 반응을 위한 부드러운 이동
            var gameBaseTm = _gameBaseElement.transform;
            Vector3 currentPos = gameBaseTm.position;
            
            // 거리가 매우 가까우면 즉시 이동
            float distance = Vector3.Distance(currentPos, _targetPosition);
            if (distance < 0.01f)
            {
                gameBaseTm.position = _targetPosition;
            }
            else
            {
                // Lerp를 사용하여 더 빠르고 부드럽게 이동 (t 값을 크게 하여 빠른 반응)
                float lerpSpeed = 20f; // Lerp 속도 (높을수록 빠름)
                Vector3 newPos = Vector3.Lerp(currentPos, _targetPosition, lerpSpeed * Time.deltaTime);
                gameBaseTm.position = newPos;
            }
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

