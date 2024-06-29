using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Component
{
    public interface IGiftCell
    {
        
    }
    
    public class GiftItemCell : BaseComponent<GiftItemCell.Data>, BuyCash.IListener
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
            public Item GiftItem = null;
        }
        
        public interface IListener
        {
            void Select(IGiftCell iGiftCell);
        }
        
        [SerializeField]
        private Image iconImg = null;
        [SerializeField]
        private RectTransform buyRootRectTm = null;
        [SerializeField]
        private OpenCondition openCondition = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetOpenCondition();
        }

        public override void Activate()
        {
            
        }

        private void SetOpenCondition()
        {
            if (openCondition == null)
                return;

            var giftItem = _data?.GiftItem;
            if (giftItem == null)
                return;
            
            var user = Info.UserManager.Instance?.User;
            long userCash = 0;
            if(user != null)
            {
                userCash = user.Cash;
            }

            var openConditionData = new OpenCondition.Data()
            {
                ImgSprite = GiftItemSprite,
                Text = giftItem.Value.ToString(),
                refreshLayout = false,
            };
            
            openCondition.Initialize(openConditionData);
        }

        private Sprite GiftItemSprite
        {
            get
            {
                var giftItem = _data?.GiftItem;
                if (giftItem == null)
                    return null;
                
                return ResourceManager.Instance?.AtalsLoader?.GetItemSprite("Friendship_Gift_" + giftItem.Id);
            }
        }
        
        #region BuyCash.IListener

        void BuyCash.IListener.Buy(bool possible)
        {
            
        }
        #endregion

        public void OnClick()
        {
            var giftItem = _data?.GiftItem;
            if (giftItem == null)
                return;
            
            Sequencer.EnqueueTask(
                () =>
                {
                    var buyCash = new PopupCreator<BuyCash, BuyCash.Data>()
                        .SetReInitialize(true)
                        .SetData(new BuyCash.Data()
                        {
                            IListener = this,
                            Cash = giftItem.Price,
                            targetSprite = GiftItemSprite,
                        })
                        .Create();

                    return buyCash;
                });
        }
    }
}

