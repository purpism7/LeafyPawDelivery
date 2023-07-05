using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Game.Creature
{
    public class Animal : Base<Animal.Data>
    {
        public class Data : BaseData
        {
            public int Order = 0;
            public System.Action<DropItem, Transform> DropItemAction = null;
        }

        private AnimalRoot _animalRoot = null;
        private AnimalActionController _actionCtr = null;
        private SpriteRenderer _spriteRenderer = null;

        //private Data.Animal _animalData = null;
        private System.Action<DropItem, Transform> _dropItemAction = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            //_animalData = GameSystem.GameManager.Instance?.DataContainer?.GetAnimal(Id);

            _animalRoot = GetComponentInChildren<AnimalRoot>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if(_spriteRenderer != null)
            {
                _spriteRenderer.sortingOrder = data.Order;
            }

            _dropItemAction = data?.DropItemAction;

            InitActionController();

            //_dropItemAction?.Invoke(_animalData.DropItem, transform);
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

