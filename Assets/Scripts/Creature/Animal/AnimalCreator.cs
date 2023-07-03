using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Creature;
using UI;
using Game;

namespace GameSystem
{
    public class AnimalCreator : BaseCreator<Creature.Animal>
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

        public override Creature.Animal Create()
        {
            var animal = ResourceManager.Instance?.Instantiate<Creature.Animal>(_animalId, _rootTm);
            if (animal == null)
            {
                return null;
            }

            animal.Initialize(new Creature.Animal.Data_()
            {
                Order = _order,
                DropItemAction = _dropItemAction,
            });

            return MainGameManager.Instance?.AddAnimal(animal);
        }
    }
}

