using System.Collections;
using System.Collections.Generic;
using System.Resources;
using GameSystem;
using UnityEngine;

namespace  UI.Component
{
    public class FriendshipCell : BaseComponent<FriendshipCell.Data>
    {
        public class Data : BaseData
        {
            public int Id = 0;
        }

        [SerializeField]
        private RectTransform heartRootReectTm = null;
        
        private List<HeartCell> _heartCellList = new();
        
        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            _heartCellList?.Clear();

            var component = new ComponentCreator<HeartCell, HeartCell.Data>()
                .SetRootRectTm(heartRootReectTm)
                .Create();
            
            _heartCellList?.Add(component);
        }

        public override void Activate()
        {
            base.Activate();
            
            if (_heartCellList != null)
            {
                if (_data != null)
                {
                    foreach (var cell in _heartCellList)
                    {
                        cell?.Activate(
                            new HeartCell.Data()
                            {
                                Id = _data.Id,
                            });
                    }
                }
            }
            
        }

        private void SetHeartList()
        {
            
        }
    }
}
