using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Creature
{
    public class Animal : Base
    {
        private AnimalRoot _animalRoot = null;
        private AnimalActionController _actionCtr = null;
        private SpriteRenderer _spriteRenderer = null;

        private Data.Animal _animalData = null;
        private System.Action<Data.DropItem, Transform> _dropItemAction = null;

        public override void Init(params object[] objs)
        {
            _animalData = GameSystem.GameManager.Instance?.DataContainer?.GetAnimal(Id);

            _animalRoot = GetComponentInChildren<AnimalRoot>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            if (objs != null &&
                objs.Length > 0)
            {
                if (int.TryParse(objs[0].ToString(), out int order))
                {
                    _spriteRenderer.sortingOrder = order;
                }

                _dropItemAction = objs[1] as System.Action<Data.DropItem, Transform>;
            }

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

