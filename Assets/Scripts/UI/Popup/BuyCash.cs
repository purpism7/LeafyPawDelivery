using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UI.Component;

namespace UI
{
    public class BuyCash : BasePopup<BuyCash.Data>
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
            public int Cash = 0;
        }

        public interface IListener
        {
            void Buy();
        }

        [SerializeField]
        private Component.OpenCondition openCondition = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetOpenCondition();
        }

        public override void Activate()
        {
            base.Activate();

            openCondition?.Activate();
        }

        private void SetOpenCondition()
        {
            if (_data == null)
                return;

            if (openCondition == null)
                return;

            var user = Info.UserManager.Instance?.User;
            long userCash = 0;
            if (user != null)
            {
                userCash = user.Cash;
            }

            var openConditionData = new OpenCondition.Data()
            {
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetCurrencyCashSprite(),
                Text = "x" + _data.Cash,
                PossibleFunc = () => userCash >= _data.Cash,
            };

            openCondition.Initialize(openConditionData);
        }

        public void OnClickCancel()
        {
            Debug.Log("OnClickCancel");
            Deactivate();
        }

        public void OnClickBuy()
        {
            _data?.IListener?.Buy();

            Deactivate();
        }
    }
}

