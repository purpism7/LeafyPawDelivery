using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;

namespace Game
{
    public class DropItemCreator : BaseCreator<DropItem>
    {
        private Transform _rootTm = null;
        private Transform _startRootTm = null;
        private DropItem _dropItemData = null;

        public DropItemCreator SetRootTm(Transform rootTm)
        {
            _rootTm = rootTm;

            return this;
        }

        public DropItemCreator SetStartRootTm(Transform startRootTm)
        {
            _startRootTm = startRootTm;

            return this;
        }

        public DropItemCreator SetDropItemData(DropItem dropItemData)
        {
            _dropItemData = dropItemData;

            return this;
        }

        public override DropItem Create()
        {
            var dropItem = ResourceManager.Instance.InstantiateGame<DropItem>(_rootTm);
            dropItem?.Initialize(new DropItem.Data()
            {
                startRootTm = _startRootTm,
            });

            return dropItem;
        }
    }
}

