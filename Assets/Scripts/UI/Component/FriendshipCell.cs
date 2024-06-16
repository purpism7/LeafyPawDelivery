using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Game;
using GameSystem;
using UnityEditor.AnimatedValues;
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

            // var component = new ComponentCreator<HeartCell, HeartCell.Data>()
            //     .SetRootRectTm(heartRootReectTm)
            //     .Create();
            //
            // _heartCellList?.Add(component);
        }

        public override void Activate(Data data)
        {
            if (_data != null &&
                data != null &&
                _data.Id == data.Id)
            {
                base.Activate();
            }
            else
            {
                base.Activate(data);
            }

            SetHeartList();
            // if (_heartCellList != null)
            // {
            //     if (_data != null)
            //     {
            //         foreach (var cell in _heartCellList)
            //         {
            //             cell?.Activate();
            //         }
            //     }
            // }
        }

        private void SetHeartList()
        {
            if (_data == null)
                return;

            var animalInfo = MainGameManager.Get<AnimalManager>()?.GetAnimalInfo(_data.Id);
            if (animalInfo == null)
                return;

            int maxHeartCnt = Game.Data.Const.MaxFriendshipHeartCount;
            for (int i = 0; i < maxHeartCnt; ++i)
            {
                HeartCell heartCell = null;
                if (_heartCellList.Count <= i)
                {
                    heartCell = new ComponentCreator<HeartCell, HeartCell.Data>()
                        .SetRootRectTm(heartRootReectTm)
                        .Create();
                    
                    _heartCellList?.Add(heartCell);
                }
                else
                {
                    heartCell = _heartCellList[i];
                }


                float max = Game.Data.Const.MaxFriendshipPoint;
                float limit = max * (i + 1);
                // float friendshipPoint = animalInfo.FriendshipPoint / ();

                float friendshipPointProgress = 0;
                if (animalInfo.FriendshipPoint >= limit)
                {
                    friendshipPointProgress = 1f;
                }
                else
                {
                    var remain = animalInfo.FriendshipPoint - max * i;
                    if (remain > 0)
                    {
                        friendshipPointProgress = remain / max;
                    }
                }
                
                
                Debug.Log(friendshipPointProgress);
                // Debug.Log(animalInfo.FriendshipPoint % (Game.Data.Const.MaxFriendshipPoint));
                heartCell?.Activate(
                    new HeartCell.Data()
                    {
                        Progress = friendshipPointProgress,
                    });
            }
        }
    }
}
