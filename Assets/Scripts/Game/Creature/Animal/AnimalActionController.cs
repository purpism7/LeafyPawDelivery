using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Creature
{
    public class AnimalActionController : MonoBehaviour, Action.IListener<AnimalAction>
    {
        public List<Action> AnimalActionList = new();

        private Animator _animator = null;
        private SpriteRenderer _sprRenderer = null;
        private AnimalAction _currentAnimalAction = null;

        //void OnDrawGizmos()
        //{
        //    Vector3 pos = Vector3.zero;
        //    var gameCameraCtr = MainGameManager.Instance?.GameCameraCtr;
        //    if (gameCameraCtr == null)
        //        return;

        //    var center = gameCameraCtr.Center;
        //    var halfWidth = (gameCameraCtr.Width - 100f) / 2f;
        //    var halfHeight = (gameCameraCtr.Height - 700f) / 2f;

        //    Gizmos.color = Color.black;
        //    var posY = center.y + halfHeight;
        //    posY = Mathf.Clamp(posY, gameCameraCtr.IGridProvider.LimitBottom.y, gameCameraCtr.IGridProvider.LimitTop.y);
        //    Gizmos.DrawLine(new Vector3(center.x - halfWidth, center.y - halfHeight), new Vector3(center.x + halfWidth, posY));
        //}

        public void Initialize(Animator animator, SpriteRenderer sprRenderer, bool onlyIdle)
        {
            _animator = animator;
            _sprRenderer = sprRenderer;

            AnimalActionList.Add(CreateaAnimalAction<IdleAction>());
            if(!onlyIdle)
            {
                AnimalActionList.Add(CreateaAnimalAction<WalkAction>());
            }

            StartIdleAction();
        }

        private void StartRandomAction()
        {
            _currentAnimalAction = RandomAnimalAction;
            _currentAnimalAction?.StartAction();
        }

        public void ChainUpdate()
        {
            if (MainGameManager.Instance.GameState.CheckState<Game.State.Edit>())
            {
                StartIdleAction();

                return;
            }

            _currentAnimalAction?.ChainUpdate();
        }

        private AnimalAction CreateaAnimalAction<T>() where T : AnimalAction, new()
        {
            var data = new AnimalAction.Data()
            {
                IListener = this,
                Tm = transform,
                Animator = _animator,

                SprRenderer = _sprRenderer,
            };

            return new T().Create(data);
        }

        private AnimalAction RandomAnimalAction
        {
            get
            {
                if(AnimalActionList == null)
                    return null;

                var randomIndex = UnityEngine.Random.Range(0, AnimalActionList.Count);

                return AnimalActionList[randomIndex] as AnimalAction;
            }
        }

        private void StartIdleAction()
        {
            if (_currentAnimalAction is IdleAction)
                return;

            foreach(AnimalAction action in AnimalActionList)
            {
                if(action is IdleAction)
                {
                    _currentAnimalAction = action;
                    action.StartAction();

                    break;
                }
            }
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

