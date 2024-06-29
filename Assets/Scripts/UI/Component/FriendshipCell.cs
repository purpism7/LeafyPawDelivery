using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Game;
using GameSystem;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using ResourceManager = System.Resources.ResourceManager;

namespace  UI.Component
{
    public class FriendshipCell : BaseComponent<FriendshipCell.Data>, GiftItemCell.IListener
    {
        public class Data : BaseData
        {
            public int Id = 0;
        }

        // [SerializeField]
        // private RectTransform heartRootReectTm = null;
        [Header("Friendship")]
        [SerializeField] 
        private Image progressImg = null;
        [SerializeField] 
        private TextMeshProUGUI pointTMP = null;
        [SerializeField] 
        private FriendshipGiftCell[] friendshipGiftCells = null;

        [Header("Gift")]
        [SerializeField] 
        private RectTransform giftRootRectTm = null;
        
        // private List<HeartCell> _heartCellList = new();
        
        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            foreach (var cell in friendshipGiftCells) 
            { 
                cell?.Initialize(null);
            }

            SetGiftItemList();
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

        private void SetGiftItemList()
        {
            var  giftItemList = ItemContainer.Instance?.GetDataList(Type.EItemSub.Gift);
            if (giftItemList == null)
                return;

            for (int i = 0; i < giftItemList.Count; ++i)
            {
                var giftItem = giftItemList[i];
                if(giftItem == null)
                    continue;
                
                var giftItemCell = new ComponentCreator<GiftItemCell, GiftItemCell.Data>()
                    .SetData(new GiftItemCell.Data()
                    {
                        IListener = this,
                        GiftItem = giftItem,
                    })
                    .SetRootRectTm(giftRootRectTm)
                    .Create();
            }
        }
        
        #region GiftCell.IListener

        void GiftItemCell.IListener.Select(IGiftCell iGiftCell)
        {
            
        }
        #endregion
    }
}
