using System.Collections;
using System.Collections.Generic;
using System.Resources;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Game;
using GameSystem;
using UnityEngine.Localization.Settings;

namespace  UI.Component
{
    public class FriendshipCell : BaseComponent<FriendshipCell.Data>, GiftItemCell.IListener
    {
        public class Data : BaseData
        {
            public int Id = 0;
            public IListener IListener = null;
        }
        
        public interface IListener
        {
            void GiveGift(Item item, Vector3 startPos, System.Action endAction);
        }

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
        
        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            foreach (var cell in friendshipGiftCells) 
            { 
                cell?.Initialize(null);
            }

            SetGiftItemList();
        }

        public override void Activate(Data data)
        {
            base.Activate(data);

            SetPointInfo();
            
            if (friendshipGiftCells != null)
            {
                for (int i = 1; i <= friendshipGiftCells.Length; ++i)
                {
                    int point = i * 30;
                    if (i >= friendshipGiftCells.Length)
                    {
                        point = Games.Data.Const.MaxFriendshipPoint;
                    }

                    friendshipGiftCells[i - 1]?.Activate(
                        new FriendshipGiftCell.Data()
                        {
                            Id = _data.Id,
                            Point =  point,
                            Index = i - 1,
                        });
                }
            }
        }

        private void SetPointInfo()
        {
            if (_data == null)
                return;
            
            var animalInfo = MainGameManager.Get<AnimalManager>().GetAnimalInfo(_data.Id);
            if (animalInfo == null)
                return;
            
            Debug.Log("progress = " + (animalInfo.FriendshipPoint / (float)Games.Data.Const.MaxFriendshipPoint));
            SetPointTMP(animalInfo.FriendshipPoint);
            SetProgresss(animalInfo.FriendshipPoint);
        }

        private void SetPointTMP(int point)
        {
            if (pointTMP == null)
                return;
            
            pointTMP.SetText($"{point}" + "/" + $"{Games.Data.Const.MaxFriendshipPoint}");
        }

        private void SetProgresss(int point)
        {
            if (progressImg == null)
                return;

            progressImg.fillAmount = point / (float)Games.Data.Const.MaxFriendshipPoint;
        }

        private void SetGiftItemList()
        {
            if (_data == null)
                return;
            
            var  giftItemList = ItemContainer.Instance?.GetDataList(Type.EItemSub.Gift);
            if (giftItemList == null)
                return;

            for (int i = 0; i < giftItemList.Count; ++i)
            {
                var giftItem = giftItemList[i];
                if(giftItem == null)
                    continue;
                
                new ComponentCreator<GiftItemCell, GiftItemCell.Data>()
                    .SetData(new GiftItemCell.Data()
                    {
                        IListener = this,
                        AnimalId = _data.Id,
                        GiftItem = giftItem,
                    })
                    .SetRootRectTm(giftRootRectTm)
                    .Create();
            }
        }

        private void RefreshGiftItemCell()
        {
            if (friendshipGiftCells == null)
                return;

            foreach (var cell in friendshipGiftCells)
            {
                cell?.Refresh();
            }
        }
        
        #region GiftCell.IListener

        void GiftItemCell.IListener.GiveGift(Item item, Vector3 startPos)
        {
            if (item == null)
                return;

            // if (item.EPayment == Type.EPayment.Cash)
            // {
            //     var user = Info.UserManager.Instance?.User;
            //     long userCash = 0;
            //     if (user != null)
            //         userCash = user.Cash;
            //
            //     if (userCash < item.Price)
            //     {
            //         var localDesc = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "not_enough_jewel", LocalizationSettings.SelectedLocale);
            //         Game.Toast.Get?.Show(localDesc);
            //         
            //         return;
            //     }
            // }
            
            _data?.IListener?.GiveGift(item, startPos,
                () =>
                {
                    if (_data == null)
                        return;
                    
                    MainGameManager.Get<AnimalManager>()?.AddFriendshipPoint(_data.Id, item.Value, -item.Price);

                    SetPointInfo();
                    RefreshGiftItemCell();
                });
        }
        #endregion
    }
}
