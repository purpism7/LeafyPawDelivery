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
        private Transform _rootTm = null;
        private int _order = 0;
        private Vector3 _pos = Vector3.zero;
        private System.Action<DropItem, Transform> _dropItemAction = null;

        public AnimalCreator SetAnimalId(int id)
        {
            _animalId = id;

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

        public AnimalCreator SetDropItemAction(System.Action<DropItem, Transform> dropItemAction)
        {
            _dropItemAction = dropItemAction;

            return this;
        }

        public override Game.Creature.Animal Create()
        {
            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return null;

            var activityPlace = mainGameMgr.placeMgr?.ActivityPlace;
            if (activityPlace == null)
                return null;

            var animal = ResourceManager.Instance?.Instantiate<Game.Creature.Animal>(_animalId, activityPlace.animalRootTm);
            if (animal == null)
                return null;

            animal.Initialize(new Game.Creature.Animal.Data()
            {
                Id = _animalId,
                Order = _order,
                Pos = _pos,
            });

            MainGameManager.Instance?.AnimalMgr?.AddAnimal(_animalId);

            return animal;
        }
    }
}
