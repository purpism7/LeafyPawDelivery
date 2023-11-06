using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Game.Creature;
using UI;
using Game;

namespace GameSystem
{
    public class AnimalCreator : BaseCreator<Game.Creature.Animal>
    {
        private int _animalId = 0;
        private int _skinId = 0;
        private Transform _rootTm = null;
        private int _order = 0;
        private Vector3 _pos = Vector3.zero;
        private bool _isEdit = true;
        private bool _isSpeechBubble = true;
        private IPlaceState _iPlaceState = null;

        //private System.Action<DropItem, Transform> _dropItemAction = null;

        public AnimalCreator SetAnimalId(int id)
        {
            _animalId = id;

            return this;
        }

        public AnimalCreator SetSkinId(int id)
        {
            _skinId = id;

            return this;
        }

        public AnimalCreator SetRootTm(Transform rootTm)
        {
            _rootTm = rootTm;

            return this;
        }

        public AnimalCreator SetOrder(int order)
        {
            _order = order;

            return this;
        }

        public AnimalCreator SetPos(Vector3 pos)
        {
            _pos = pos;

            return this;
        }

        public AnimalCreator SetIsEdit(bool isEdit)
        {
            _isEdit = isEdit;

            return this;
        }

        public AnimalCreator SetIsSpeechBubble(bool isSpeechBubble)
        {
            _isSpeechBubble = isSpeechBubble;

            return this;
        }

        public AnimalCreator SetIPlaceState(IPlaceState iPlaceState)
        {
            _iPlaceState = iPlaceState;

            return this;
        }

        //public AnimalCreator SetDropItemAction(System.Action<DropItem, Transform> dropItemAction)
        //{
        //    _dropItemAction = dropItemAction;

        //    return this;
        //}

        public override Game.Creature.Animal Create()
        {
            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return null;

            var rootTm = _rootTm;
            if (!rootTm)
            {
                var activityPlace = mainGameMgr.placeMgr?.ActivityPlace;
                if (activityPlace == null)
                    return null;

                rootTm = activityPlace.animalRootTm;
            }
            
            var animal = ResourceManager.Instance?.InstantiateAnimal(_animalId, _skinId, rootTm);
            if (animal == null)
                return null;

            animal.Initialize(new Game.Creature.Animal.Data()
            {
                Order = _order,
                Pos = _pos,
                IsEdit = _isEdit,
                IsSpeechBubble = _isSpeechBubble,
                IPlaceState = _iPlaceState,
            });

            return animal;
        }
    }
}

