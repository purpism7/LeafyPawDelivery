using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Game;
using GameSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace  UI.Component
{
    public class FriendshipCell : BaseComponent<FriendshipCell.Data>
    {
        public class Data : BaseData
        {
            public int Id = 0;
        }

        // [SerializeField]
        // private RectTransform heartRootReectTm = null;
        [SerializeField] 
        private Image progressImg = null;
        [SerializeField] 
        private TextMeshProUGUI pointTMP = null;
        [SerializeField] 
        private FriendshipGiftCell[] friendshipGiftCells = null;
         
        // private List<HeartCell> _heartCellList = new();
        
        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            // _heartCellList?.Clear();

            // var component = new ComponentCreator<HeartCell, HeartCell.Data>()
            //     .SetRootRectTm(heartRootReectTm)
            //     .Create();
            
            // _heartCellList?.Add(component);

            
                
                
                
                // foreach (var cell in friendshipGiftCells)
                // {
                //     cell?.Initialize(
                //         new FriendshipGiftCell.Data()
                //         {
                //             
                //         });
                // }
            // }
        }

        public override void Activate()
        {
            base.Activate();

            if (_data == null)
                return;
            
            var animalInfo = MainGameManager.Get<AnimalManager>().GetAnimalInfo(_data.Id);
            if (animalInfo == null)
                return;
            
            Debug.Log("progress = " + (animalInfo.FriendshipPoint / (float)Game.Data.Const.MaxFriendshipPoint));
            SetPointTMP(animalInfo.FriendshipPoint);
            SetProgresss(animalInfo.FriendshipPoint);

            if (friendshipGiftCells != null)
            {
                for (int i = 1; i <= friendshipGiftCells.Length; ++i)
                {
                    int point = i * 30;
                    if (i >= friendshipGiftCells.Length)
                    {
                        point = Game.Data.Const.MaxFriendshipPoint;
                    }

                    friendshipGiftCells[i - 1]?.Activate(
                        new FriendshipGiftCell.Data()
                        {
                            Point =  point,
                        });
                }
            }

            // if (friendshipGiftCells != null)
            // {
            //     foreach (var cell in friendshipGiftCells)
            //     {
            //         cell?.Activate(
            //             new FriendshipGiftCell.Data()
            //             {
            //                 Point = 
            //             });
            //     }
            // }
            
            // if (_heartCellList != null)
            // {
            //     if (_data != null)
            //     {
            //         foreach (var cell in _heartCellList)
            //         {
            //             cell?.Activate(
            //                 new HeartCell.Data()
            //                 {
            //                     Id = _data.Id,
            //                 });
            //         }
            //     }
            // }

        }

        private void SetPointTMP(int point)
        {
            if (pointTMP == null)
                return;
            
            pointTMP.SetText($"{point}" + "/" + $"{Game.Data.Const.MaxFriendshipPoint}");
        }

        private void SetProgresss(int point)
        {
            if (progressImg == null)
                return;

            progressImg.fillAmount = point / (float)Game.Data.Const.MaxFriendshipPoint;
        }
    }
}
