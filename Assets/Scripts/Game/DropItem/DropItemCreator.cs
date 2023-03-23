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
        private Data.DropItem _dropItemData = null;

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

        public DropItemCreator SetDropItemData(Data.DropItem dropItemData)
        {
            _dropItemData = dropItemData;

            return this;
        }

        public override DropItem Create()
        {
            var dropItem = ResourceManager.Instance.InstantiateGame<DropItem>(_rootTm);
            dropItem?.Init(new DropItem.Data()
            {
                startRootTm = _startRootTm,
            });

            return dropItem;
        }
    }
}

