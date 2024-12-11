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

        private List<GiftItemCell> _giftItemCell = null;
        
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
                        point = Games.Data.Const.MaxFriendshipPoint;

                    friendshipGiftCells[i - 1]?.Activate(
                        new FriendshipGiftCell.Data
                        {
                            IListener = this,
                            Id = _data.Id,
                            Point =  point,
                            Index = i - 1,
                        });
                }
            }

            if (_giftItemCell != null)
            {
                for (int i = 0; i < _giftItemCell.Count; ++i)
                {
                    _giftItemCell[i]?.Activate(
                        new GiftItemCell.Data
                        {
                            AnimalId = _data.Id
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
            
            SetPointTMP(animalInfo.FriendshipPoint);
            SetProgresss(animalInfo.FriendshipPoint);
        }

        private void SetPointTMP(int point)
        {
            if (pointTMP == null)
                return;
            
            var maxFriendshipPoint = Games.Data.Const.MaxFriendshipPoint;

            if (point > maxFriendshipPoint)
                point = maxFriendshipPoint;
            
            pointTMP.SetText($"{point}" + "/" + $"{maxFriendshipPoint}");
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

            if (_giftItemCell == null)
            {
                _giftItemCell = new();
                _giftItemCell.Clear();
            }

            for (int i = 0; i < giftItemList.Count; ++i)
            {
                var giftItem = giftItemList[i];
                if(giftItem == null)
                    continue;
                
                var cell = new ComponentCreator<GiftItemCell, GiftItemCell.Data>()
                    .SetData(new GiftItemCell.Data()
                    {
                        IListener = this,
                        AnimalId = _data.Id,
                        GiftItem = giftItem,
                        ADData =  giftItem.EPayment == Type.EPayment.Advertising ? giveGiftAD?.Datas?.FirstOrDefault() : null,
                    })
                    .SetRootRectTm(giftRootRectTm)
                    .Create();
                
                _giftItemCell?.Add(cell);
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

        // startPos 에서 주민에게 선물 주고, 친밀도 올리기.
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
                // Reward AnimalCurrency = 500 / ObjectCurrency = 10000 
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
                        });
                    
                    list.Add(
                        new OpenCondition.Data
                        {
                            ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetCurrencySprite(placeData.ObjectSpriteName),
                            Text = string.Format("{0}", rewardObjectCurrency),
                        });

                    var localDesc = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "desc_animal_give_gift", LocalizationSettings.SelectedLocale);
                    
                    new PopupCreator<GetReward, GetReward.Data>()
                        .SetReInitialize(true)
                        .SetData(new GetReward.Data
                        {
                            RewardDataList = list,
                            Desc = string.Format(localDesc, GameUtils.GetName(Type.EElement.Animal, _data.Id, Games.Data.Const.AnimalBaseSkinId)),
                            EndAction = () =>
                            {
                                UIManager.Instance?.Top?.CollectCurrencyAsync(transform.position, Type.EElement.Animal, rewardAnimalCurrency, false).Forget();
                                UIManager.Instance?.Top?.CollectCurrencyAsync(transform.position, Type.EElement.Object, rewardObjectCurrency, false, 0.1f).Forget();
                            },
                        }).Create();
                    
                    break;
                }

                // Reward Jewel = 500
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
                    
                    var localDesc = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "desc_animal_give_gift", LocalizationSettings.SelectedLocale);

                    new PopupCreator<GetReward, GetReward.Data>()
                        .SetReInitialize(true)
                        .SetData(new GetReward.Data
                        {
                            RewardDataList = list,
                            Desc = string.Format(localDesc, GameUtils.GetName(Type.EElement.Animal, _data.Id, Games.Data.Const.AnimalBaseSkinId)),
                            EndAction = () =>
                            {
                                UIManager.Instance?.Top?.CollectCashCurrency(transform.position, rewardCash);
                            },
                        }).Create();

                    break;
                }

                // Reward = SpecialObject
                case 2:
                {
                    if (_data == null)
                        return;
                    
                    var animalData = AnimalContainer.Instance?.GetData(_data.Id);
                    if (animalData == null)
                        return;

                    if (animalData.InteractionId <= 0)
                        return;
                    
                    Sequencer.EnqueueTask(
                        () =>
                        {
                            var popup = new GameSystem.PopupCreator<UI.Obtain, UI.Obtain.Data>()
                                .SetData(new UI.Obtain.Data()
                                {
                                    EElement = Type.EElement.Object,
                                    Id = animalData.InteractionId,
                                    ClickAction = () =>
                                    {
                                        
                                    },
                                })
                                .SetCoInit(true)
                                .SetReInitialize(true)
                                .Create();

                            return popup;
                        });
                    
                    MainGameManager.Instance?.Add(Type.EElement.Object, animalData.InteractionId);
                    
                    break;
                }
            }
            
            UserManager.Instance?.GetFriendshipGift(_data.Id, index);
        }
        #endregion
    }
}
