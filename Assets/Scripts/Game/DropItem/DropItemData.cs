using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public partial class DropItem : Game.Base<DropItem.Data>
    {
        public class CurrencyData : Data
        {
            public Game.Type.EElement EElement = Type.EElement.None;

            public CurrencyData() : base(Type.EItem.Currency)
            {

            }
        }

        public class ItemData : Data 
        {
            public Game.Type.EItemSub eItemSub = Type.EItemSub.None;

            public ItemData() : base(Type.EItem.Item)
            {

            }
        }

    }
}
