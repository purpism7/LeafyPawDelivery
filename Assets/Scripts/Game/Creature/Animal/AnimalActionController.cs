using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Creature
{
    public class AnimalActionController : MonoBehaviour, Action.IListener<AnimalAction>
    {
        public List<Action> AnimalActionList = new();

        private AnimalAction _currentAnimalAction = null;
        private IAnimal _iAnimal = null;

        public void Initialize(int id, IAnimal iAnimal, bool onlyIdle)
        {
            if (iAnimal == null)
                return;

            _iAnimal = iAnimal;
            
            if(!onlyIdle)
            {
                AnimalActionList.Add(CreateaAnimalAction<WalkAction>(id));
            }
            
            AnimalActionList.Add(CreateaAnimalAction<IdleAction>(id));
            AnimalActionList.Add(CreateaAnimalAction<SignatureAction>(id));

            StartIdleAction();
        }

        public void Deactivate()
        {
            StartIdleAction();
        }

        private void StartRandomAction()
        {
            if (_iAnimal == null)
                return;

            if(_iAnimal.EGameState == Type.EGameState.Edit ||
               _iAnimal.EGameState == Type.EGameState.Conversation)
                return;
            
            var randomAnimalAction = RandomAnimalAction;
            if (randomAnimalAction is WalkAction)
                randomAnimalAction.SetActionData(GetWalkActionData(GetRandomPos(_iAnimal.LocalPos.z)));
            
            _currentAnimalAction = randomAnimalAction;
            _currentAnimalAction?.StartAction();
        }

        public void ChainUpdate()
        {
            if(_currentAnimalAction != null &&
               _currentAnimalAction.IsUpdate)
            {
                _currentAnimalAction.ChainUpdate();
            }
        }

        private AnimalAction CreateaAnimalAction<T>(int id) where T : AnimalAction, new()
        {
            var data = new AnimalAction.Data()
            {
                IListener = this,
                Tm = transform,
                Animator = _iAnimal.Animator,

                Id = id,
                SprRenderer = _iAnimal.SpriteRenderer,
            };

            return new T().Create(data);
        }

        private AnimalAction RandomAnimalAction
        {
            get
            {
                if(AnimalActionList == null)
                    return null;

                // signature 는 제외.
                var randomIndex = UnityEngine.Random.Range(0, AnimalActionList.Count - 1);

                return AnimalActionList[randomIndex] as AnimalAction;
            }
        }

        private void StartAction<T>(AnimalAction.ActionData actionData = null) where T : AnimalAction
        {
            if (AnimalActionList == null)
                return;

            foreach (AnimalAction action in AnimalActionList)
            {
                if (action is T)
                {
                    _currentAnimalAction = action;
                    
                    action.SetActionData(actionData);
                    action.StartAction();

                    break;
                }
            }
        }

        private void StartIdleAction()
        {
            if (_currentAnimalAction is IdleAction)
                return;

            StartAction<IdleAction>();
        }

        public void StartSignatureAction()
        {
            StartAction<SignatureAction>();
        }

        public void MoveToTarget(Vector3 targetPos, System.Action endAction)
        {
            StartAction<WalkAction>(GetWalkActionData(targetPos, endAction));
        }

        private WalkAction.Data GetWalkActionData(Vector3 targetPos, System.Action endAction = null)
        {
            return new WalkAction.Data
            {
                TargetPos = targetPos,
                EndAction = endAction,
            };
        }
        
        private Vector3 GetRandomPos(float z)
        {
            var iGameCameraCtr = MainGameManager.Instance?.IGameCameraCtr;
            if (iGameCameraCtr == null)
                return Vector3.zero;

            var randomX = iGameCameraCtr.RandPosXInScreenRagne;
            var randomY = iGameCameraCtr.RandPosYInScreenRagne;

            return new Vector3(randomX, randomY, z);
        }

        #region Action.IListener
        void Action.IListener<AnimalAction>.StartAction(AnimalAction animalAction)
        {
            //Debug.Log("StartAction = " + animalAction.GetType());
        }

        void Action.IListener<AnimalAction>.EndAction(AnimalAction animalAction)
        {
            StartRandomAction();
        }
        #endregion
    }
}

