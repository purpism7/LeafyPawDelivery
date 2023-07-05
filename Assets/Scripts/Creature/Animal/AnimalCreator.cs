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
        private System.Action<DropItem, Transform> _dropItemAction = null;

        public AnimalCreator SetAnimalId(int id)
        {
            _animalId = id;

            return this;
        }

        public AnimalCreator SetRoot(Transform rootTm)
        {
            _rootTm = rootTm;

            return this;
        }

        public AnimalCreator SetOrder(int order)
        {
            _order = order;

            return this;
        }

        public AnimalCreator SetDropItemAction(System.Action<DropItem, Transform> dropItemAction)
        {
            _dropItemAction = dropItemAction;

            return this;
        }

        public override Game.Creature.Animal Create()
        {
            var animal = ResourceManager.Instance?.Instantiate<Game.Creature.Animal>(_animalId, _rootTm);
            if (animal == null)
            {
                return null;
            }

            animal.Initialize(new Game.Creature.Animal.Data()
            {
                Order = _order,
            });

            MainGameManager.Instance?.AnimalMgr?.AddAnimalInfo(_animalId);

            return animal;
        }
    }
}

