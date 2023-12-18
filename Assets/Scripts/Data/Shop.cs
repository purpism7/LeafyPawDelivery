using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class Shop : Data.Base
    {
        [SerializeField]
        private bool show = false;
        [SerializeField]
        private string category = string.Empty;
        [SerializeField]
        private string payment = string.Empty;
        [SerializeField]
        private int paymentValue = 0;
        [SerializeField]
        private int value = 0;
        [SerializeField]
        private string iconImg = string.Empty;
        [SerializeField]
        private string productId = string.Empty;

        public bool Show { get { return show; } }
        public Game.Type.ECategory ECategory { get; private set; } = Game.Type.ECategory.None;
        public Game.Type.EPayment EPayment { get; private set; } = Game.Type.EPayment.None;
        public int PaymentValue { get { return paymentValue; } }
        public int Value { get { return value; } }
        public string IconImg { get { return iconImg; } }
        public string ProductId { get { return productId; } }

        public override void Initialize()
        {
            base.Initialize();

            if (System.Enum.TryParse(category, out Game.Type.ECategory eCategory))
            {
                ECategory = eCategory;
            }

            if (System.Enum.TryParse(payment, out Game.Type.EPayment ePayment))
            {
                EPayment = ePayment;
            }
        }
    }
}


