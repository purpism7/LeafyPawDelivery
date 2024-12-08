using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Game;
using GameSystem;
using Info;
using UnityEngine.Localization.Settings;

namespace  UI.Component
{
    public class FriendshipCell : BaseComponent<FriendshipCell.Data>, GiftItemCell.IListener, FriendshipGiftCell.IListener
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
        [SerializeField] 
        private AD giveGiftAD = null;
        
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
                            IListener = this,
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
                        ADData =  giftItem.EPayment == Type.EPayment.Advertising ? giveGiftAD?.Datas?.FirstOrDefault() : null,
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

        private void GiveGift(Item item, Vector3 startPos)
        {
            _data?.IListener?.GiveGift(item, startPos,
                () =>
                {
                    if (_data == null)
                        return;
                    
                    switch (item.EPayment)
                    {
                        case Type.EPayment.Cash:
                        {
                            var user = Info.UserManager.Instance.User;
                            if (user == null)
                                return;

                            user.SetCash(-item.Price);

                            break;
                        }

                        case Type.EPayment.ObjectCurrency:
                        {
                            var user = Info.UserManager.Instance.User;
                            if (user == null)
                                return;

                            user.SetCurrency(Type.EElement.Object, -item.Price);

                            break;
                        }
                    }
                    
                    ITop iTop = Game.UIManager.Instance?.Top;
                    iTop?.SetCurrency();
                    
                    MainGameManager.Get<AnimalManager>()?.AddFriendshipPoint(_data.Id, item.Value);
                    
                    SetPointInfo();
                    RefreshGiftItemCell();
                });
        }
        
        #region GiftCell.IListener

        void GiftItemCell.IListener.GiveGift(Item item, Vector3 startPos)
        {
            if (item == null)
                return;

            if (item.EPayment == Type.EPayment.Advertising)
            {
                var adData = giveGiftAD?.Datas?.FirstOrDefault();
                if (adData == null)
                    return;

                AdProvider.Get?.ShowAd(adData,
                    (rewardValue) =>
                    {
                        if(rewardValue > 0)
                            GiveGift(item, startPos);
                        // if(rewardValue > 0)
                        // {
                        //     SuccessActivateBoost(true);
                        // }
                        // else
                        // {
                        //     SetPlayTimer(false);
                        // }
                    });

                return;
            }

            GiveGift(item, startPos);
        }
        #endregion
        
        #region FriendshipGiftCell.IListener

        void FriendshipGiftCell.IListener.GetGift(int index)
        {
            if (_data == null)
                return;

            switch (index)
            {
                case 0:
                {
                    var list = new List<OpenCondition.Data>();
                    list.Clear();

                    var placeData = MainGameManager.Get<PlaceManager>()?.ActivityPlaceData;
                    if (placeData == null)
                        return;

                    int rewardAnimalCurrency = 500;
                    int rewardObjectCurrency = 10000;
                    
                    list.Add(
                        new OpenCondition.Data
                        {
                            ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetCurrencySprite(placeData.AnimalSpriteName),
                            Text = string.Format("{0}", rewardAnimalCurrency),
                            // refreshLayout = true,
                        });
                    
                    list.Add(
                        new OpenCondition.Data
                        {
                            ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetCurrencySprite(placeData.ObjectSpriteName),
                            Text = string.Format("{0}", rewardObjectCurrency),
                            // PossibleFunc = possibleFunc,
                            // refreshLayout = true,
                        });

                    new PopupCreator<GetReward, GetReward.Data>()
                        .SetReInitialize(true)
                        .SetData(new GetReward.Data
                        {
                            RewardDataList = list,
                            EndAction = () =>
                            {
                                UIManager.Instance?.Top?.CollectCurrencyAsync(transform.position, Type.EElement.Animal, rewardAnimalCurrency, false).Forget();
                                UIManager.Instance?.Top?.CollectCurrencyAsync(transform.position, Type.EElement.Object, rewardObjectCurrency, false, 0.1f).Forget();
                            },
                        }).Create();
                    
                    break;
                }

                case 1:
                {
                    var list = new List<OpenCondition.Data>();
                    list.Clear();

                    var placeData = MainGameManager.Get<PlaceManager>()?.ActivityPlaceData;
                    if (placeData == null)
                        return;

                    int rewardCash = 200;
                    
                    list.Add(
                        new OpenCondition.Data
                        {
                            ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.CurrencyCashSprite,
                            Text = string.Format("{0}", rewardCash),
                            // refreshLayout = true,
                        });

                    new PopupCreator<GetReward, GetReward.Data>()
                        .SetReInitialize(true)
                        .SetData(new GetReward.Data
                        {
                            RewardDataList = list,
                            EndAction = () =>
                            {
                                UIManager.Instance?.Top?.CollectCurrencyAsync(transform.position, Type.EElement.Animal, rewardCash, false).Forget();
                            },
                        }).Create();

                    break;
                }

                case 2:
                {
                    break;
                }
            }
            
            UserManager.Instance?.User?.GetFriendshipGift(_data.Id, index);
        }
        #endregion
    }
}
