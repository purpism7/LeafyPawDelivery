using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Creature
{
    public class Animal : Base<Animal.Data_>
    {
        public class Data_ : BaseData
        {
            public int Order = 0;
            public System.Action<Data.DropItem, Transform> DropItemAction = null;
        }

        private AnimalRoot _animalRoot = null;
        private AnimalActionController _actionCtr = null;
        private SpriteRenderer _spriteRenderer = null;

        private Data.Animal _animalData = null;
        private System.Action<Data.DropItem, Transform> _dropItemAction = null;

        public override void Init(Data_ data)
        {
            base.Init(data);
            
            _animalData = GameSystem.GameManager.Instance?.DataContainer?.GetAnimal(Id);

            _animalRoot = GetComponentInChildren<AnimalRoot>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if(_spriteRenderer != null)
            {
                _spriteRenderer.sortingOrder = data.Order;
            }

            _dropItemAction = data?.DropItemAction;

            InitActionController();

            _dropItemAction?.Invoke(_animalData.DropItem, transform);
        }

        private void InitActionController()
        {
            _actionCtr = GetComponent<AnimalActionController>();
            _actionCtr?.Init();
        }

        public override void ChainUpdate()
        {
            _actionCtr?.ChainUpdate();
        }
    }
}

