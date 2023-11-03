using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;

namespace Game
{
    public class DropItemCreator : BaseCreator<DropItem>
    {
        private static DropItemCreator _instance = null;
        public static DropItemCreator Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DropItemCreator();
                }

                return _instance;
            }
        }

        private Transform _rootTm = null;
        private DropItem.Data _dropItemData = null;

        public DropItemCreator SetRootTm(Transform rootTm)
        {
            _rootTm = rootTm;

            return this;
        }

        public DropItemCreator SetDropItemData(DropItem.Data dropItemData)
        {
            _dropItemData = dropItemData;

            return this;
        }

        public override DropItem Create()
        {
            var dropItem = ResourceManager.Instance.InstantiateGame<DropItem>(_rootTm);
            dropItem?.Initialize(_dropItemData);

            return dropItem;
        }
    }
}

